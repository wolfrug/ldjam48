using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[System.Serializable]
public class RadioTuned : UnityEvent<RadioTunerObject> { }

public class RadioTunerObject : MonoBehaviour {
    public string tunerId = "tuningTest";
    public FMOD_Controller tunerController;
    [SerializeField]
    private bool active = true;
    public float optimalDistance = 0.5f;
    private float currentValue = 0f;
    public bool invokeOnce = true;
    private bool invoked = false;
    public Animator gaugeAnimator;
    public Button radioButton;
    public RadioTuned tunedEvent;
    public RadioTuned detunedEvent;

    // Update is called once per frame
    void Update () {
        if (active) {
            float distance = Vector3.Distance (GameManager.instance.Player.transform.position, transform.position);
            currentValue = optimalDistance / distance;
            tunerController.SetValue (currentValue);
            UpdateAnimator ();
            if (tunerController.currentEffectiveValue == tunerController.maxValue) {
                if (!invoked) {
                    tunedEvent.Invoke (this);
                    invoked = true;
                }
            } else if (invoked && !invokeOnce) {
                invoked = false;
                detunedEvent.Invoke (this);
            }
        }
    }
    public void ActivateButton () {
        if (radioButton != null) {
            radioButton.interactable = true;
            radioButton.onClick.AddListener (() => RunKnot ());
        }
    }
    public void DeactivateButton () {
        if (radioButton != null) {
            radioButton.interactable = false;
            radioButton.onClick.RemoveListener (() => RunKnot ());
        }
    }
    public void UpdateAnimator () {
        if (gaugeAnimator != null) {
            gaugeAnimator.SetFloat ("Value", currentValue);
        }
    }

    void RunKnot () {
        InkWriter.main.GoToKnot (tunerId);
        Active = false;
        DeactivateButton ();
    }

    public void SetTunerID () {
        InkWriter.main.story.variablesState["tunerID"] = tunerId;
    }
    public void DesetTunerID () {
        InkWriter.main.story.variablesState["tunerID"] = "";
    }

    public bool Active {
        get {
            return active;
        }
        set {
            active = value;
            tunerController.Active = value;
            if (!value) {
                tunerController.emitter.Stop ();
                DeactivateButton ();
                currentValue = 0f;
                UpdateAnimator ();
            } else {
                tunerController.emitter.Play ();
            }
        }
    }

    public void SlowStop (float timeout) {
        Invoke (nameof (SlowStop), timeout);
    }
    void SlowStop () {
        Active = false;
    }
    void OnDrawGizmos () {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere (transform.position, optimalDistance);
    }
}