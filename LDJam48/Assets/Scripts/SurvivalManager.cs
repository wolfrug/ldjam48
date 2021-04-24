using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class HealthBarEmptyOrFull : UnityEvent<HealthMeter> { }

[System.Serializable]
public class HealthBarChanged : UnityEvent<HealthMeter, float> { }

public enum DamageType {
    NONE = 0000,
    HEALTH = 1000,
    HUNGER = 1001,
    TEMPERATURE = 1002,
    OXYGEN = 1003,
    MENTAL = 2000,
    SUPERNATURAL = 2001,
    COMFORT = 2002,
    THINGS_SEEN = 2003
}

[System.Serializable]
public class HealthMeter {
    public GenericHealthBar targetHealthbar;
    public DamageType type;
    public string displayName = "Health";
    public bool isEmpty = false;
    [Tooltip ("Set to false to be a 'empty once, die forever' type of thing")]
    public bool canBeImproved = true;
    public GameObject survivalEffectPrefab_Plus;
    public GameObject survivalEffectPrefab_Minus;
    public Transform survivalEffectParent;
    [Tooltip ("When the health bar is emptied")]
    public HealthBarEmptyOrFull emptyEvent;
    [Tooltip ("When the healthbar is filled again after being emptied")]
    public HealthBarEmptyOrFull notEmptyEvent;
    public HealthBarChanged changedEvent;
}

public class SurvivalManager : MonoBehaviour {

    public static SurvivalManager instance;
    public BasicAgent player;
    public HealthMeter[] healthbars;

    public Transform defaultSurvivalEffectParent;
    public GameObject defaultSurvivalEffectPrefab;

    private List<SurvivalEffect> currentEffects = new List<SurvivalEffect> { };
    private Dictionary<string, SurvivalEffect> permanentEffects = new Dictionary<string, SurvivalEffect> { };

    private Dictionary<DamageType, HealthMeter> healthMeters = new Dictionary<DamageType, HealthMeter> { };

    private bool m_active;

    void Awake () {
        if (instance == null) {
            instance = this;
        } else {
            Destroy (gameObject);
        }
    }

    // Start is called before the first frame update
    void Start () {
        player = GameManager.instance.Player;
        foreach (HealthMeter meter in healthbars) {
            healthMeters.Add (meter.type, meter);
        }
    }

    public bool Active {
        get {
            return m_active;
        }
        set {
            m_active = value;
            foreach (SurvivalEffect effect in currentEffects) {
                effect.active = value;
            }
        }
    }

    public float GetHealth (DamageType type) {
        HealthMeter returnMeter = null;
        healthMeters.TryGetValue (type, out returnMeter);
        if (returnMeter != null) {
            return returnMeter.targetHealthbar.currentHealth;
        } else {
            return -1f;
        }
    }
    public GenericHealthBar GetHealthBar (DamageType type) {
        HealthMeter returnMeter = null;
        healthMeters.TryGetValue (type, out returnMeter);
        if (returnMeter != null) {
            return returnMeter.targetHealthbar;
        } else {
            return null;
        }
    }
    public string GetDisplayName (DamageType type) {
        HealthMeter returnMeter = null;
        healthMeters.TryGetValue (type, out returnMeter);
        if (returnMeter != null) {
            return returnMeter.displayName;
        } else {
            return "";
        }
    }

    public SurvivalEffect SpawnHealthEffect (DamageType type, float dps, float duration) {
        HealthMeter targetMeter = null;
        healthMeters.TryGetValue (type, out targetMeter);
        if (targetMeter != null) {
            GameObject survivalEffect = defaultSurvivalEffectPrefab;
            Transform effectParent = defaultSurvivalEffectParent;
            if (dps >= 0f) {
                if (targetMeter.survivalEffectPrefab_Plus != null) {
                    survivalEffect = targetMeter.survivalEffectPrefab_Plus;
                }
            } else if (targetMeter.survivalEffectPrefab_Minus != null) {
                survivalEffect = targetMeter.survivalEffectPrefab_Minus; { }
            }
            if (targetMeter.survivalEffectParent != null) {
                effectParent = targetMeter.survivalEffectParent;
            }
            SurvivalEffect newEffect = Instantiate (survivalEffect, effectParent).GetComponent<SurvivalEffect> ();
            newEffect.type = type;
            newEffect.damagePerTick = dps;
            newEffect.timeLeft = duration;
            newEffect.healthbarTarget.minMaxHealth = new Vector2 (0f, duration);
            newEffect.nameText.text = TooltipText (type, dps);
            currentEffects.Add (newEffect);
            newEffect.effectDone.AddListener (RemoveEffect);
            newEffect.Init ();
            return newEffect;
        } else {
            Debug.LogWarning ("Tried to spawn effect for non-existing damage type: " + type.ToString ());
        }
        return null;
    }

    string TooltipText (DamageType type, float damage) {
        string startText = GetDisplayName (type);
        string addText = "";
        string color = "<color=red>";
        if (damage > 0f) {
            addText = "+";
            color = "<color=green>";
        }
        string returnText = color + startText + "\n" + addText + damage.ToString () + "/s" + "</color>";
        return returnText;
    }

    public void StartPermanentEffect (DamageType type, float dps, float dissipateDuration, string id) {
        if (permanentEffects.ContainsKey (id)) {
            StopPermanentEffect (id);
        };
        SurvivalEffect newEffect = SpawnHealthEffect (type, dps, dissipateDuration);
        newEffect.countDown = false;
        permanentEffects.Add (id, newEffect);
    }
    public void StopPermanentEffect (string id) {
        if (permanentEffects.ContainsKey (id)) {
            permanentEffects[id].countDown = true;
            permanentEffects[id].animator.SetFloat ("Speed", 1f / permanentEffects[id].timeLeft);
            permanentEffects.Remove (id);
        }
    }
    public bool HasPermanentEffect (string id) {
        return (permanentEffects.ContainsKey (id));
    }

    void RemoveEffect (SurvivalEffect effect) {
        if (effect != null) {
            if (currentEffects.Contains (effect)) {
                currentEffects.Remove (effect);
            }
        }
    }

    public bool ChangeHealth (DamageType type, float change) { // use negative numbers to damage
        HealthMeter targetMeter = null;
        healthMeters.TryGetValue (type, out targetMeter);
        if (targetMeter != null) {
            float returnChangeValue = change;
            if (targetMeter.targetHealthbar.currentHealth < change) {
                returnChangeValue = targetMeter.targetHealthbar.currentHealth;
            } else if (targetMeter.targetHealthbar.currentHealth <= 0f) {
                returnChangeValue = 0f;
            }
            targetMeter.targetHealthbar.currentHealth += change;
            targetMeter.changedEvent.Invoke (targetMeter, returnChangeValue);
            if (targetMeter.targetHealthbar.currentHealth <= 0f) { // the healthbar is empty
                if (!targetMeter.isEmpty) { // only invoke once
                    targetMeter.emptyEvent.Invoke (targetMeter);
                    targetMeter.isEmpty = true;
                };
            } else if (targetMeter.isEmpty && change > 0f) { // meter is empty, but the change is positive
                if (targetMeter.canBeImproved) {
                    targetMeter.isEmpty = false;
                    targetMeter.notEmptyEvent.Invoke (targetMeter);
                }
            }
        } else {
            Debug.Log ("No health meter for " + type.ToString ());
            return false;
        }
        return true;
    }

    public void ResetMeters () {

        List<string> effectList = new List<string> { };
        foreach (KeyValuePair<string, SurvivalEffect> kvp in permanentEffects) {
            effectList.Add (kvp.Key);
        }
        foreach (string id in effectList) {
            StopPermanentEffect (id);
        }

        foreach (SurvivalEffect effect in currentEffects) {
            effect.StopAllCoroutines ();
            Destroy (effect.gameObject);
        }
        foreach (KeyValuePair<DamageType, HealthMeter> kvp in healthMeters) {
            kvp.Value.targetHealthbar.currentHealth = kvp.Value.targetHealthbar.minMaxHealth.y;
            kvp.Value.isEmpty = false;
        }
    }

    [NaughtyAttributes.Button]
    void TestDamage () {
        SpawnHealthEffect (DamageType.OXYGEN, -0.1f, 1f);
        //SpawnHealthEffect (DamageType.MENTAL, -0.04f, 6f);
    }

    [NaughtyAttributes.Button]
    void StartEffect () {
        StartPermanentEffect (DamageType.OXYGEN, -0.01f, .1f, "test");
    }

    [NaughtyAttributes.Button]
    void StopEffect () {
        StopPermanentEffect ("test");
    }

    // Update is called once per frame
    void Update () {

    }
}