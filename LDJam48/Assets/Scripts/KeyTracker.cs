using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[System.Serializable]
public class KeyPressed : UnityEvent<KeyTracker, KeyCode> { }

public class KeyTracker : MonoBehaviour { // use this to assign input to e.g. buttons 'externally'
    // Start is called before the first frame update
    //public string keyCode = "enter";
    public KeyCode keyCode;
    public Button targetButton;
    public KeyPressed customAction;
    [SerializeField]
    private bool active = true;
    void Start () {

    }
    public void Activate (bool activate) {
        active = activate;
    }

    // Update is called once per frame
    void Update () {
        if (active) {
            if (Input.GetKeyDown (keyCode)) {
                if (targetButton != null && targetButton.enabled && targetButton.interactable) {
                    targetButton.onClick.Invoke ();
                };
                customAction.Invoke (this, keyCode);
            }
        }
    }
}