using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using NaughtyAttributes;

[System.Serializable]
public class TMPLinkClickedEvent : UnityEvent<string> { }

[RequireComponent (typeof (TextMeshProUGUI))]
public class TMP_LinkWatcher : MonoBehaviour, IPointerClickHandler {

    public TMP_Text pTextMeshPro;
    public TMPLinkClickedEvent linkClickedEvent;
    //public Camera pCamera;
    void Start () {
        /* if (pCamera == null) {
             pCamera = Camera.main;
         }*/
    }
    public void OnPointerClick (PointerEventData eventData) {
        int linkIndex = TMP_TextUtilities.FindIntersectingLink (pTextMeshPro, Input.mousePosition, null);
        //Debug.Log ("Clicked: " + linkIndex);
        if (linkIndex != -1) { // was a link clicked?
            TMP_LinkInfo linkInfo = pTextMeshPro.textInfo.linkInfo[linkIndex];
            Debug.Log ((linkInfo.GetLinkID ()));
            linkClickedEvent.Invoke (linkInfo.GetLinkID ());
        }
    }
}