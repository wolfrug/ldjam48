using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[System.Serializable]
public class DragEntered : UnityEvent<Item_DragAndDrop, Item_DragTarget> { }

[System.Serializable]
public class DragExited : UnityEvent<Item_DragAndDrop, Item_DragTarget> { }

[System.Serializable]
public class DragCompleted : UnityEvent<Item_DragAndDrop, Item_DragTarget> { }

[System.Serializable]
public class PointerDown : UnityEvent<Item_DragTarget, PointerEventData> { }

public class Item_DragTarget : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerUpHandler, IPointerDownHandler {

    public bool interactable = true;
    public PointerDown pointerDownEvent;
    public DragEntered dragEnteredEvent;
    public DragExited dragExitedEvent;
    public DragCompleted dragCompletedEvent;

    public GDDE_Enter pointerEnter;
    public GDDE_Exit pointerExit;

    public bool pointerIn = false;

    // Start is called before the first frame update
    void Start () {

    }
    public void OnPointerEnter (PointerEventData coll) {
        if (interactable) {
            //Debug.Log ("Pointer enter " + coll);
            if (Item_DragAndDrop.currentDragTarget != null) {
                pointerIn = true;
                dragEnteredEvent.Invoke (Item_DragAndDrop.currentDragTarget, this);
            }
            pointerEnter.Invoke (coll);
        };
    }
    public void OnPointerExit (PointerEventData coll) {
        if (interactable) {
            //Debug.Log ("Pointer exit " + coll);
            if (Item_DragAndDrop.currentDragTarget != null) {
                pointerIn = false;
                dragExitedEvent.Invoke (Item_DragAndDrop.currentDragTarget, this);
            }
            pointerExit.Invoke (coll);
        };
    }

    public void OnPointerUp (PointerEventData coll) {
        if (interactable && pointerIn) {
//            Debug.Log ("Drag finished " + coll);
            pointerIn = false;
            dragCompletedEvent.Invoke (Item_DragAndDrop.lastDragTarget, this);
        }
    }

    public void OnPointerDown (PointerEventData coll) {
        if (interactable) {
            //Debug.Log("Pointer down" + coll);
            pointerDownEvent.Invoke (this, coll);
        };
    }

    // Update is called once per frame
    void Update () {
        if (interactable && pointerIn) {
            if (Input.GetMouseButtonUp (0)) {
                OnPointerUp (null);
            }
        }
    }
}