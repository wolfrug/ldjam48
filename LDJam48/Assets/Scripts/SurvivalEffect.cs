using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[System.Serializable]
public class EffectDone : UnityEvent<SurvivalEffect> { }

public class SurvivalEffect : MonoBehaviour { // spawn these to create damage-over-time type situations
    public DamageType type = DamageType.NONE;
    public float damagePerTick = 0.01f;
    public float timeLeft = 1f;
    public bool active = true;
    public bool countDown = true;
    public bool destroySelfWhenDone = true;
    public float destroySelfTimeOut = 2f;
    public GenericHealthBar healthbarTarget;
    public TMPro.TextMeshProUGUI nameText;
    public Animator animator;
    public EffectDone effectDone;
    // Start is called before the first frame update
    void Start () {

    }

    public void Init () {
        StartCoroutine (Effect ());
    }
    IEnumerator Effect () {
        if (animator != null) {
            animator.SetFloat ("Speed", 1f / timeLeft);
        }
        while (active) {
            yield return null; // once a frame, like update
            if (timeLeft >= 0f) {
                healthbarTarget.currentHealth = timeLeft;
                if (countDown) {
                    timeLeft -= Time.deltaTime;
                } else {
                    if (animator != null) {
                        animator.SetFloat ("Speed", 0f);
                    }
                }
                SurvivalManager.instance.ChangeHealth (type, damagePerTick);
            } else { // effect done!
                effectDone.Invoke (this);
                if (destroySelfWhenDone) {
                    Destroy (gameObject, destroySelfTimeOut);
                }
                yield break;
            }
        }
    }
}