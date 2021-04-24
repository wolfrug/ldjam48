using System.Collections;
using System.Collections.Generic;
using Doozy.Engine;
using UnityEngine;

public class SceneController : MonoBehaviour {
    public static SceneController instance;
    public List<SceneObject> sceneParents;
    public int currentScene;

    void Awake () {
        if (instance == null) {
            instance = this;
        } else {
            Destroy (gameObject);
        }
    }

    public void Init () {
        SetScene (currentScene);
    }

    public bool SetScene (SceneObject target, SceneEntrance entrance = SceneEntrance.Default) {
        if (target != null) {
            for (int i = 0; i < sceneParents.Count; i++) {
                if (sceneParents[i] == target) {
                    SetScene (i, entrance);
                    return true;
                }
            }
        }
        Debug.LogWarning ("No such scene in scene list! " + target);
        return false;
    }
    public bool SetScene (int index, SceneEntrance entrance = SceneEntrance.Default) {
        if (index > 0 || index < sceneParents.Count) {
            GameEventMessage.SendEvent ("HideScene");
            StartCoroutine (LoadScene (index, entrance));
            return true;
        } else {
            Debug.LogWarning ("Tried to load invalid scene index: " + index);
            return false;
        }
    }
    IEnumerator LoadScene (int index, SceneEntrance entrance = SceneEntrance.Default) {
        yield return new WaitForSeconds (1f); // wait for doozy to hide the scene
        foreach (SceneObject go in sceneParents) {
            if (go != sceneParents[index]) {
                go.DeactivateSelf ();
            }
        }
        sceneParents[index].ActivateSelf ();
        // Move player!
        Vector3 newPos = sceneParents[index].GetEntrance (entrance).door.self.transform.position;
        Debug.Log ("Attempted move pos " + newPos);
        Debug.DrawLine (GameManager.instance.Player.transform.position, newPos);
        if (!GameManager.instance.Player.navMeshAgent.Warp (newPos)) { // attempt warp, otherwise force setpo
            GameManager.instance.Player.transform.position = newPos;
        }
        yield return new WaitForSeconds (0.1f);
        GameEventMessage.SendEvent ("ShowScene");
    }

    [NaughtyAttributes.Button]
    public void SetAllActive () { // For inits etc
        foreach (SceneObject go in sceneParents) {
            go.transform.position = transform.position;
            go.ActivateSelf ();
        }
    }

    [NaughtyAttributes.Button]
    void EditorSetScene () {
        foreach (SceneObject go in sceneParents) {
            if (go != sceneParents[currentScene]) {
                go.DeactivateSelf ();
            }
        }
        sceneParents[currentScene].ActivateSelf ();
    }

    // Update is called once per frame
    void Update () {

    }
}