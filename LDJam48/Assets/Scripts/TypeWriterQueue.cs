using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WriterAction {
    [Multiline]
    public string writeString;
    public float pauseUntilNext = 0.5f;
    public WriterStarted startedEvent;
    public WriterStopped finishedEvent;
}

public class TypeWriterQueue : MonoBehaviour {
    [NaughtyAttributes.ReorderableList]
    public WriterAction[] strings;
    public TypeWriter typeWriter;
    public bool playOnStart = true;
    public float waitBetweenStrings = 0.1f;
    private Coroutine queue;
    [SerializeField]
    private int index;
    // Start is called before the first frame update
    void Start () {
        if (playOnStart) {
            StartQueue (0);
        }
    }

    public void StartQueue (int startPoint) {
        if (queue == null) {
            queue = StartCoroutine (Queue (startPoint));
        } else {
            StopCoroutine (queue);
            queue = StartCoroutine (Queue (startPoint));
        }
    }
    IEnumerator Queue (int startPoint) {
        index = startPoint;
        while (index < strings.Length) {
            typeWriter.Write (strings[index].writeString);
            yield return null;
            strings[index].startedEvent.Invoke (typeWriter);
            yield return new WaitUntil (() => !typeWriter.isWriting_);
            yield return new WaitForSeconds (strings[index].pauseUntilNext);
            strings[index].finishedEvent.Invoke (typeWriter);
            index++;
            yield return new WaitForSeconds (waitBetweenStrings);
        }
    }

    // Update is called once per frame
    void Update () {

    }
}