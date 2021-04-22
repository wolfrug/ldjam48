using System.Collections;
using System.Collections.Generic;
using Ink.Runtime;
using UnityEngine;

[CreateAssetMenu (fileName = "Data", menuName = "Ink Story Object", order = 1)]
public class InkStoryObject : ScriptableObject {

    public string storyName = "default";
    public TextAsset inkJsonAsset;
    public TagListener[] tagListeners;

    public void Init () { // spawn all the listeners!
        foreach (TagListener listener in tagListeners) {
            Instantiate (listener.gameObject, InkWriter.main.transform);
        }
        // Start their listeners!
        foreach (TagListener listener in FindObjectsOfType<TagListener> ()) {
            listener.InitListeners ();
        }
    }
}