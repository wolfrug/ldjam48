using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class SceneLoaded : UnityEvent<SceneObject> { }

public enum SceneEntrance {
    None,
    Left,
    Right,
    Up,
    Down,
    Middle,
    Default,
}

[System.Serializable]
public class Entrance {
    public SceneEntrance type;
    public Interactable_Object door;
    public SceneObject connectsTo;
    public SceneEntrance connectEntrance = SceneEntrance.Default;
    public bool active = true;
}
public class SceneObject : MonoBehaviour {

    [Tooltip ("Game object that will be deactivated on deactivation")]
    public GameObject allparent;
    public SceneEntrance defaultEntrance = SceneEntrance.Left;
    [Tooltip ("References to the entrances of the scene")]
    public List<Entrance> entrances;
    [SerializeField]
    private bool m_activeSelf = true;

    public SceneLoaded sceneLoadedEvent;
    public SceneLoaded sceneUnloadedEvent;
    private Dictionary<SceneEntrance, Entrance> entranceDict = new Dictionary<SceneEntrance, Entrance> { };
    // Start is called before the first frame update
    void Awake () {
        if (allparent == null) {
            allparent = gameObject;
        }
        foreach (Entrance entrance in entrances) {
            entranceDict.Add (entrance.type, entrance);
            entrance.door.openedObjectEvent.AddListener ((arg0, arg1) => GoThroughEntrance (entrance.type));
        }
        entranceDict.Add (SceneEntrance.Default, GetEntrance (defaultEntrance)); // add the default entrance
        if (m_activeSelf) {
            ActivateSelf ();
        } else {
            DeactivateSelf ();
        }
    }

    public void DeactivateSelf () {
        allparent.SetActive (false);
        sceneLoadedEvent.Invoke (this);
    }
    public void ActivateSelf () {
        allparent.SetActive (true);
        sceneUnloadedEvent.Invoke (this);
    }

    public Entrance GetEntrance (SceneEntrance type) {
        Entrance returnVar = null;
        entranceDict.TryGetValue (type, out returnVar);
        if (returnVar != null) {
            return returnVar;
        } else {
            Debug.LogWarning ("Tried to query non-existant entrance from " + gameObject + "(" + type + ")");
            return null;
        }
    }
    public bool GoThroughEntrance (SceneEntrance type) {
        Entrance targetEntrance = GetEntrance (type);
        if (targetEntrance != null) {
            if (targetEntrance.active) {
                return SceneController.instance.SetScene (targetEntrance.connectsTo, targetEntrance.connectEntrance);
            } else {
                return false;
            }
        } else {
            return false;
        }
    }
    public void SetEntranceActive (SceneEntrance type, bool active) {
        Entrance target = GetEntrance (type);
        if (target != null) {
            target.active = active;
        }
    }

#if UNITY_EDITOR
    void OnDrawGizmos () {
        foreach (Entrance entrance in entrances) {
            Vector3 start;
            Vector3 end = Vector3.zero;
            Color color = Color.blue;
            if (entrance.connectsTo != null && entrance.active) {
                start = entrance.door.transform.position;
                foreach (Entrance targetEntrance in entrance.connectsTo.entrances) {
                    if (targetEntrance.connectsTo == this) {
                        end = targetEntrance.door.transform.position;
                        color = Color.green;
                        break;
                    } else {
                        end = entrance.connectsTo.transform.position;
                    }
                }
                Gizmos.color = color;
                Gizmos.DrawLine (start, end);
            }
        }
    }
#endif

    // Update is called once per frame
    void Update () {

    }
}