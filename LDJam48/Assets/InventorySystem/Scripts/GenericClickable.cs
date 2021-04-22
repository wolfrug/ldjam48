using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

[System.Serializable]
public class OnMouseEnter : UnityEvent<GenericClickable> { }

[System.Serializable]
public class OnMouseDown : UnityEvent<GenericClickable> { }

[System.Serializable]
public class OnRightMouseDown : UnityEvent<GenericClickable> { }

[System.Serializable]
public class OnDoubleClick : UnityEvent<GenericClickable> { }

[System.Serializable]
public class OnMouseExit : UnityEvent<GenericClickable> { }

[System.Serializable]
public class OnMouseOver : UnityEvent<GenericClickable> { }

[System.Serializable]
public class OnMouseUp : UnityEvent<GenericClickable> { }

[System.Serializable]
public class OnMouseDragging : UnityEvent<GenericClickable> { }

[System.Serializable]
public class OnMouseBeginDrag : UnityEvent<GenericClickable> { }

[System.Serializable]
public class OnMouseEndDrag : UnityEvent<GenericClickable> { }

public class GenericClickable : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler, IBeginDragHandler, IDragHandler, IEndDragHandler {
    
    public LayerMask targetMasks;
    public OnMouseDown onMouseDownEvent;
    public bool onMouseDownActive = true;

    public OnRightMouseDown onRightMouseDownEvent;
    public bool onRightMouseDownActive = true;
    public OnDoubleClick onDoubleClickEvent;
    public bool onDoubleClickActive = true;
    public OnMouseEnter onMouseEnterEvent;
    public bool onMouseEnterActive = true;
    public OnMouseExit onMouseExitEvent;
    public bool onMouseExitActive = true;
    public OnMouseOver onMouseOverEvent;
    public bool onMouseOverActive = true;
    public OnMouseOver onMouseUpEvent;
    public bool onMouseUpActive = true;
    public OnMouseDragging onMouseDraggingEvent;
    public bool onMouseDraggingActive = true;
    public OnMouseBeginDrag onMouseBeginDragEvent;
    public bool onMouseBeginDragActive = true;
    public OnMouseEndDrag onMouseEndDragEvent;
    public bool onMouseEndDragActive = true;

    int clicked = 0;
    float clicktime = 0;
    float clickdelay = 0.5f;

    void Start () { }

    public void OnMouseDownActive (bool active) {
        onMouseDownActive = active;
    }
    public void OnRightMouseDownActive (bool active) {
        onRightMouseDownActive = active;
    }
    public void OnMouseEnterActive (bool active) {
        onMouseEnterActive = active;
    }
    public void OnMouseExitActive (bool active) {
        onMouseExitActive = active;
    }
    public void OnMouseOverActive (bool active) {
        onMouseOverActive = active;
    }

    // Add directly to object you wish to mouse over/click
    void OnMouseDown () {
        if (onMouseDownActive) {
            onMouseDownEvent.Invoke (this);
        };
        if (onDoubleClickActive) {
            clicked++;
            if (clicked == 1) clicktime = Time.time;
            if (clicked > 1 && Time.time - clicktime < clickdelay) {
                clicked = 0;
                clicktime = 0;
                onDoubleClickEvent.Invoke (this);
            } else if (clicked > 2 || Time.time - clicktime > 1) clicked = 0;
        }
    }
    void OnMouseEnter () {
        if (onMouseEnterActive) {
            onMouseEnterEvent.Invoke (this);
        };
    }
    void OnMouseExit () {
        if (onMouseExitActive) {
            onMouseExitEvent.Invoke (this);
        };
    }
    void OnMouseOver () {
        if (onMouseOverActive) {
            onMouseOverEvent.Invoke (this);
        }
    }
    public void OnPointerClick (PointerEventData eventData) { // REQUIRES EVENT SYSTEM AND PHYSICS RAYCASTER ON CAMERA!
        if (eventData.button == PointerEventData.InputButton.Right && onRightMouseDownActive) {
            onRightMouseDownEvent.Invoke (this);
        };
        if (onDoubleClickActive) {
            clicked++;
            if (clicked == 1) clicktime = Time.time;
            if (clicked > 1 && Time.time - clicktime < clickdelay) {
                clicked = 0;
                clicktime = 0;
                onDoubleClickEvent.Invoke (this);
            } else if (clicked > 2 || Time.time - clicktime > 1) clicked = 0;
        }
    }

    public void OnPointerEnter (PointerEventData coll) {
        if (onMouseEnterActive) {
            onMouseEnterEvent.Invoke (this);
        };
    }
    public void OnPointerExit (PointerEventData coll) {
        if (onMouseExitActive) {
            onMouseExitEvent.Invoke (this);
        };
    }
    public void OnPointerDown (PointerEventData coll) {
        if (onMouseDownActive) {
            onMouseDownEvent.Invoke (this);
        };
        if (coll.button == PointerEventData.InputButton.Right && onRightMouseDownActive) {
            onRightMouseDownEvent.Invoke (this);
        };
    }
    public void OnPointerUp (PointerEventData coll) {
        if (onMouseUpActive) {
            onMouseUpEvent.Invoke (this);
        };
    }

    public void OnBeginDrag (PointerEventData eventData) {
        //Debug.Log ("OnBeginDrag: ");
        if (onMouseBeginDragActive) {
            onMouseBeginDragEvent.Invoke (this);
        };
    }

    // Drag the selected item.
    public void OnDrag (PointerEventData data) {
        if (onMouseDraggingActive) {
            onMouseDraggingEvent.Invoke (this);
        };
    }

    public void OnEndDrag (PointerEventData eventData) {
        //Debug.Log ("OnEndDrag: ");
        if (onMouseEndDragActive) {
            onMouseEndDragEvent.Invoke (this);
        };
    }

}