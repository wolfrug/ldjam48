using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (GenericTrigger))]
public class Survival_AreaEffect : MonoBehaviour {
    public string uniqueId = "areaEffect";
    public DamageType damageType;
    public float damageAmount = -0.1f;
    public float damageTime = 1f;
    public float damageInterval = 5f;
    private float lastDamageTime = 0f;
    public bool startActive = true;
    private bool m_active = true;
    // Start is called before the first frame update
    void Start () {
        Active = startActive;
    }

    public string GetObjectName () { // if you name a number of critters the same thing, the aura won't stack!
        return uniqueId + gameObject.name;
    }

    public bool Active {
        get {
            return m_active;
        }
        set {
            m_active = value;
            if (!value) {
                StopAuraDamage ();
            }
        }
    }

    public void StartAuraDamage () {
        if (Active) {
            if (!SurvivalManager.instance.HasPermanentEffect (GetObjectName ())) {
                SurvivalManager.instance.StartPermanentEffect (damageType, damageAmount, damageTime, GetObjectName ());
            };
        }
    }
    public void StopAuraDamage () {
        SurvivalManager.instance.StopPermanentEffect (GetObjectName ());
    }

    public void DoSingleDamage () {
        if (Active) {
            if (lastDamageTime < damageInterval + Time.time) {
                SurvivalManager.instance.SpawnHealthEffect (damageType, damageAmount, damageTime);
                lastDamageTime = Time.time;
            }
        }
    }

    // Update is called once per frame
    void Update () {

    }
}