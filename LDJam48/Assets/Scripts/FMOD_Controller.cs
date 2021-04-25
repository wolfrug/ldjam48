using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using UnityEngine;

public class FMOD_Controller : MonoBehaviour {

    public float minValue = 1;
    public float maxValue = 25;
    public string valueName = "Gravity";
    public float currentValue = 10;
    public float currentEffectiveValue;
    public StudioEventEmitter emitter;
    [SerializeField]
    private bool m_active = true;
    // Start is called before the first frame update

    public bool Active {
        get {
            return m_active;
        }
        set {
            m_active = value;
            emitter.enabled = value;
        }
    }
    void Awake () {
        //if (emitter == null) { emitter = GetComponent<StudioEventEmitter> (); };
    }

    public void SetValue (float newValue) {
        currentValue = newValue;
    }
    public void SetValuePositive (float newValue) {
        // Sets the value but as its positive equivalent
        if (newValue < 0f) {
            newValue *= -1f;
        }
        SetValue (newValue);
    }

    // Update is called once per frame
    void Update () {
        if (Active) {
            currentEffectiveValue = Mathf.Lerp (minValue, maxValue, currentValue);
            emitter.SetParameter (valueName, currentEffectiveValue);
        }
    }

}