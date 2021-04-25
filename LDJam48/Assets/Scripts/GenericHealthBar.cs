using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[System.Serializable]
public class HealthPercentage : UnityEvent<float> { }

public class GenericHealthBar : MonoBehaviour {
    public Image healthBar;
    public float healthPercentage = 1f;
    public Vector2 minMaxHealth = new Vector2 (0f, 1f);
    public Animator animator;
    [SerializeField]
    private float m_currentHealth = 1f;
    public HealthPercentage healthListener;
    // Start is called before the first frame update
    void Start () {

    }

    public float currentHealth {
        get {
            return m_currentHealth;
        }
        set {
            m_currentHealth = Mathf.Clamp (value, minMaxHealth.x, minMaxHealth.y);
        }
    }

    // Update is called once per frame
    void Update () {
        if (currentHealth > minMaxHealth.x && currentHealth > 0f) {
            healthPercentage = Mathf.Lerp (0f, 1f, currentHealth / minMaxHealth.y);
        } else {
            healthPercentage = 0f;
        }
        healthBar.fillAmount = healthPercentage;
        if (animator != null) {
            animator.SetFloat ("percentage", healthPercentage);
        }
        healthListener.Invoke (healthPercentage);
    }
}