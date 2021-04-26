using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class SpawnableObject { // this object will change itself a little bit if it can be cached - then the Object refers instead to something that exists in the level. 
    public string refTag;
    public GameObject Object;
    [HideInInspector]
    public GameObject spawnedObject;
    public string parentPath = "WriterCanvas";
    public bool cache = true;
    public bool cached = false;
}

[System.Serializable]
public class CustomTagEvent {
    public string targetTag;
    public TagFoundEvent tagFoundEvent;
}

[System.Serializable]
public class InkIntVariableChangedEvent {
    public string targetValue;
    public InkIntVariableChanged tagFoundEvent;
}

[System.Serializable]
public class InkStringVariableChangedEvent {
    public string targetValue;
    public InkStringVariableChanged tagFoundEvent;
}

[System.Serializable]
public class InkFloatVariableChangedEvent {
    public string targetValue;
    public InkFloatVariableChanged tagFoundEvent;
}

[System.Serializable]
public class InkBoolVariableChangedEvent {
    public string targetValue;
    public InkBoolVariableChanged tagFoundEvent;
}

[System.Serializable]
public class BindExternalFunction { // the event this calls needs to have an object[] array as its 'dynamic' entry point
    public string functionName;
    public BoundFunctionCalledEvent functionEvent;
}

[System.Serializable]
public class InkIntVariableChanged : UnityEvent<string, int> { }

[System.Serializable]
public class InkStringVariableChanged : UnityEvent<string, string> { }

[System.Serializable]
public class InkFloatVariableChanged : UnityEvent<string, float> { }

[System.Serializable]
public class InkBoolVariableChanged : UnityEvent<string, bool> { }

[System.Serializable]
public class BoundFunctionCalledEvent : UnityEvent<object[]> { }
public class TagListener : MonoBehaviour {
    [NaughtyAttributes.ReorderableList]
    public SpawnableObject[] storyObjects;
    [NaughtyAttributes.ReorderableList]
    public CustomTagEvent[] customEvents;
    public InkIntVariableChangedEvent[] intVariableChangedEvents;
    public InkStringVariableChangedEvent[] stringVariableChangedEvents;
    public InkFloatVariableChangedEvent[] floatVariableChangedEvents;
    public InkBoolVariableChangedEvent[] boolVariableChangedEvents;
    public BindExternalFunction[] boundExternalFunctionEvents;
    private Dictionary<string, SpawnableObject> storyObjectDict = new Dictionary<string, SpawnableObject> { };
    public bool selfInitialize = false;
    private bool initialized = false;
    void Start () {
        storyObjectDict.Clear ();
        foreach (SpawnableObject so in storyObjects) {
            storyObjectDict.Add (so.refTag, so);
        }
        if (selfInitialize) {
            StartCoroutine (SelfInitializeWaiter ());
        }
    }

    IEnumerator SelfInitializeWaiter () {
        yield return new WaitUntil (() => InkWriter.main != null);
        yield return new WaitUntil (() => InkWriter.main.story != null);
        InitListeners ();
    }

    public void InitListeners () {
        // Listen for variable changes
        if (!initialized) {
            // ints
            foreach (InkIntVariableChangedEvent evt in intVariableChangedEvents) {
                InkWriter.main.story.ObserveVariable (evt.targetValue, (string varName, object newValue) => {
                    if (enabled && gameObject.activeSelf) { evt.tagFoundEvent.Invoke (varName, (int) newValue); };
                });
            }
            // strings
            foreach (InkStringVariableChangedEvent evt in stringVariableChangedEvents) {
                InkWriter.main.story.ObserveVariable (evt.targetValue, (string varName, object newValue) => {
                    if (enabled && gameObject.activeSelf) { evt.tagFoundEvent.Invoke (varName, (string) newValue); };
                });
            }
            // float
            foreach (InkFloatVariableChangedEvent evt in floatVariableChangedEvents) {
                InkWriter.main.story.ObserveVariable (evt.targetValue, (string varName, object newValue) => {
                    if (enabled && gameObject.activeSelf) { evt.tagFoundEvent.Invoke (varName, (float) newValue); };
                });
            }
            foreach (InkBoolVariableChangedEvent evt in boolVariableChangedEvents) {
                InkWriter.main.story.ObserveVariable (evt.targetValue, (string varName, object newValue) => {
                    if (enabled && gameObject.activeSelf) { evt.tagFoundEvent.Invoke (varName, (bool) newValue); };
                });
            }
            // functions
            foreach (BindExternalFunction evt in boundExternalFunctionEvents) {
                InkWriter.main.story.BindExternalFunctionGeneral (evt.functionName, (object[] args) => { evt.functionEvent.Invoke (args); return ""; });
            }
            // tags
            InkWriter.main.tagEvent.AddListener (TagListenerFunction);
            // set initialized to true, to avoid having double listeners
            initialized = true;
        };
    }

    public void TagListenerFunction (string tag) {
        if (gameObject.activeSelf && enabled) {
            //        Debug.Log("Tag listener function caught tag: " + tag, gameObject);
            foreach (CustomTagEvent cte in customEvents) { // lets you make some simple custom events for tags for testing etc
                if (cte.targetTag == tag) {
                    cte.tagFoundEvent.Invoke (tag);
                }
            }
            //Debug.Log (tag + " heard in taglistener");
            if (tag.Contains ("spawn")) {
                tag = tag.Replace ("spawn.", "");
                //Debug.Log (tag);
                SpawnObject (tag);
            }
        };
    }

    public void SpawnObject (string refTag) {
        SpawnableObject tryObj;
        storyObjectDict.TryGetValue (refTag, out tryObj);
        if (tryObj != null) {
            SpawnObject (tryObj);
        }
    }
    public void SpawnObject (SpawnableObject obj) {
        if (obj.cache && obj.cached) {
            obj.Object.SetActive (true);
        } else {
            Transform parent = InkWriter.main.transform;
            if (obj.parentPath != "") {
                Transform childParent = parent.transform.Find (obj.parentPath);
                if (childParent != null) {
                    parent = childParent;
                }
            };
            GameObject newObj = Instantiate (obj.Object, parent);

            if (obj.cache) {
                obj.Object = newObj;
                obj.cached = true;
            } else {
                obj.spawnedObject = newObj;
            }
        }
    }
    public void DeSpawnObject (string refTag) {
        SpawnableObject tryObj;
        storyObjectDict.TryGetValue (refTag, out tryObj);
        if (tryObj != null) {
            DeSpawnObject (tryObj);
        }
    }
    public void DeSpawnObject (SpawnableObject obj) {
        if (obj.cache && obj.cached) {
            obj.Object.SetActive (false);
        } else {
            GameObject.Destroy (obj?.spawnedObject);
        }
    }
    public void GetInkVariable (string targetVariable) {
        string output = (string) InkWriter.main.story.variablesState[targetVariable];
        Debug.Log ("Got variable " + output);
    }

}