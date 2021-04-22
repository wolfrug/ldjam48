using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class TriggerEntered : UnityEvent<GameObject> { }

[System.Serializable]
public class TriggerExited : UnityEvent<GameObject> { }

[System.Serializable]
public class TriggerStay : UnityEvent<GameObject> { };

[System.Serializable]
public class TriggerOutOfActivations : UnityEvent<GameObject> { };
public class GenericTrigger : MonoBehaviour {

    public List<string> tags = new List<string> { };
    public TriggerEntered triggerEntered;
    public TriggerExited triggerExited;
    public TriggerStay triggerStay;
    public TriggerOutOfActivations triggerOutOfActivations;
    public bool useStay = true;
    [Header ("Action times left (negative numbers = infinite). Does -not- apply to Trigger Exit")]
    [SerializeField]
    private int activationTimesLeft = -1;
    public List<GameObject> contents = new List<GameObject> { };
    private List<GameObject> objectsFlaggedForRemoval = new List<GameObject> ();

    // Start is called before the first frame update
    void Start () { }

    public int activationTimes {
        get {
            return activationTimesLeft;
        }
        set {
            activationTimesLeft = value;
        }
    }

    void OnTriggerEnter (Collider other) {
        if (enabled) {
            if (tags.Contains (other.transform.tag) || tags.Count == 0) {
                if (!contents.Contains (other.gameObject)) {
                    if (activationTimes > 0 || activationTimes < 0) {
                        triggerEntered.Invoke (other.gameObject);
                        contents.Add (other.gameObject);
                        activationTimes--;
                        if (activationTimes == 0) {
                            triggerOutOfActivations.Invoke (gameObject);
                        }
                    };

                    //Debug.Log(other.gameObject);
                };
            }
        };
    }
    void OnTriggerExit (Collider other) {
        if (enabled) {
            if (contents.Contains (other.gameObject)) {
                contents.Remove (other.gameObject);
                triggerExited.Invoke (other.gameObject);
            }
        };
    }

    void OnTriggerEnter2D (Collider2D other) {
        if (enabled) {
            if (tags.Contains (other.transform.tag) || tags.Count == 0) {
                if (!contents.Contains (other.gameObject)) {
                    if (activationTimes > 0 || activationTimes < 0) {
                        triggerEntered.Invoke (other.gameObject);
                        contents.Add (other.gameObject);
                        activationTimes--;
                        if (activationTimes == 0) {
                            triggerOutOfActivations.Invoke (gameObject);
                        }
                    };
                };
            }
        };
    }
    void OnTriggerExit2D (Collider2D other) {
        if (enabled) {
            if (contents.Contains (other.gameObject)) {
                contents.Remove (other.gameObject);
                triggerExited.Invoke (other.gameObject);
            }
        };
    }

    void FixedUpdate () {

        if (useStay) {
            if (contents.Count > 0) {
                objectsFlaggedForRemoval.Clear ();
                for (int i = 0; i < contents.Count; i++) {
                    {
                        GameObject obj = contents[i];
                        if (obj.activeInHierarchy) {
                            if (activationTimes > 0 || activationTimes < 0) {
                                triggerStay.Invoke (obj);
                                activationTimes--;
                                if (activationTimes == 0) {
                                    triggerOutOfActivations.Invoke (gameObject);
                                }
                            };
                        } else {
                            objectsFlaggedForRemoval.Add (obj);
                        }
                    }
                };
                foreach (GameObject obj in objectsFlaggedForRemoval) {
                    contents.Remove (obj);
                }
            };
        }
    }
}