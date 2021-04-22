using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
// Unuse this if you're not using TextMeshPro for some reason
using TMPro;
using UnityEngine.Events;
using UnityEngine.UI;

[System.Serializable]
public class TooltipActivated : UnityEvent<GenericTooltip, bool> { }

// Add this directly to the UI object you want to create a tooltip for, e.g. a button. Must accept raycasts!
public class GenericTooltip : MonoBehaviour, IPointerExitHandler, IPointerEnterHandler {
    private bool m_IsActive = true;
    public bool moveToMousePosition = true;
    [Tooltip ("Time in seconds to wait until tooltip is shown.")]
    public float waitTime = 0.5f;

    public string tooltiptextPath = "";
    [Multiline]
    public string tooltiptext;
    // You can use a camera instead of Screen.height and Screen.width for the max if you want.
    // Uncomment this + the definition line in start + change the max in Update.
    //public Camera cam;
    public string tooltipImageName = "Image";
    public Transform tooltipCanvasParent;
    public GameObject tooltipPrefab;
    [Tooltip ("This is where the actual spawned tooltip object lives and is 'cached' between uses but you can also add an object here directly.")]
    public GameObject spawnedTooltip;
    private Transform targetMoveTransform;
    public TooltipActivated tooltipOpenedEvent;
    public TooltipActivated tooltipClosedEvent;
    public bool destroySelfAutomatically = true;
    Vector3 min, max;
    private RectTransform rect;
    private EventTrigger eventTrigger;
    [Tooltip ("Y-offset from parent object; make negative for UI elements that are at the bottom of the screen!")]
    public Vector2 offset = new Vector2 (0f, 15f);
    private Camera cam;

    // Start is called before the first frame update
    void Start () {
        if (cam == null) {
            cam = Camera.main;
        }
        // If this is null, we just make it the parent of the object itself, but this is a bit risky
        if (tooltipCanvasParent == null) {
            tooltipCanvasParent = transform.parent;
        }
        // Spawn and cache the tooltip prefab OR cache the already-existing one OR send a warning if neither exits.
        if (spawnedTooltip == null) {
            if (tooltipPrefab != null) {
                spawnedTooltip = Instantiate (tooltipPrefab, tooltipCanvasParent.transform, false);
                // Are we a canvas? If so, set us to SUPER HIGH and fuck being parented
                Canvas getCanvas = spawnedTooltip.GetComponent<Canvas> ();
                if (getCanvas != null) {
                    getCanvas.sortingOrder = 99;
                    spawnedTooltip.transform.SetParent (null);
                    spawnedTooltip.transform.position = Vector3.zero;
                    spawnedTooltip.transform.localScale = new Vector3 (1f, 1f, 1f);
                    //spawnedTooltip.transform.SetParent (tooltipCanvasParent.transform, true);
                    targetMoveTransform = spawnedTooltip.transform.GetChild (0);
                } else {
                    targetMoveTransform = spawnedTooltip.transform;
                }
                // Set the spawned tooltip at zero!
                if (moveToMousePosition) {
                    targetMoveTransform.localPosition = Vector3.zero;
                };
            } else {
                Debug.LogError ("No prefab or pre-made tooltip object assigned for " + name + "!", gameObject);
            }
        }

        // If there's a text set, grab the (tmppro in this case) text object and set the text - this is very simplistic though.
        if (tooltiptext != "") {
            SetTooltipText (tooltiptext);
        }

        Deactivate ();
        // if (cam == null) { cam = Camera.main; };
        rect = spawnedTooltip.GetComponent<RectTransform> ();
        if (rect == null) {
            Debug.LogError ("Spawned tooltip for " + name + " does not have rect transform component. Tooltips must be UI objects!", gameObject);
        }

    }

    public bool IsActive {
        get {
            return m_IsActive;
        }
        set {
            m_IsActive = value;
            if (!value) {
                Deactivate ();
            }
        }
    }

    public void SetTooltipText (string text) { // This is a very simplistic way to do this, but in the interest of keeping it all inside one component...change to suit!
        tooltiptext = text;
        if (tooltiptextPath == "") {
            spawnedTooltip.GetComponentInChildren<TextMeshProUGUI> ().text = tooltiptext;
        } else {
            TextMeshProUGUI targetText;
            Transform targetObj;
            targetObj = spawnedTooltip.transform.Find (tooltiptextPath);
            if (targetObj != null) {
                targetText = targetObj.GetComponent<TextMeshProUGUI> ();
                if (targetText != null) {
                    targetText.text = tooltiptext;
                    return;
                }
            }
            Debug.LogWarning ("Could not find text mesh pro ugui component at path " + tooltiptextPath, gameObject);
        }
    }
    public void SetTooltipImage (Sprite image) {
        if (tooltipImageName != "") {
            spawnedTooltip.transform.Find (tooltipImageName).GetComponent<Image> ().sprite = image;
        }
    }

    public void CopyTextFrom (TextMeshProUGUI targetText) { // a simple helper method that lets you quickly copy text into the tooltip text from another text!
        if (targetText != null) {
            SetTooltipText (targetText.text);
        }
    }
    public void CopyImageFrom (Image image) {
        SetTooltipImage (image.sprite);
    }
    void Activate () {
        if (spawnedTooltip != null) {
            spawnedTooltip.SetActive (true);
            tooltipOpenedEvent.Invoke (this, true);
        };
    }
    void Deactivate () {
        if (spawnedTooltip != null) {
            spawnedTooltip.SetActive (false);
            tooltipOpenedEvent.Invoke (this, false);
        };
    }

    public void OnPointerEnter (PointerEventData data) // Hint -> don't raycast the tooltip object, or it might block this event!
    {
        if (IsActive) {
            //Debug.Log("Pointer entered - activate tooltip after waittime");
            CancelInvoke ("Activate");
            Invoke ("Activate", waitTime);
        };
    }
    public void OnPointerExit (PointerEventData data) {
        //Debug.Log("Point exited, deactivate tooltip!");
        // We don't check for IsActive here, we just close it
        CancelInvoke ("Activate");
        Deactivate ();
    }

    // Update is called once per frame
    void LateUpdate () {
        if (IsActive && moveToMousePosition) {
            // You can place this into a separate function for more controlled resolution changes
            min = new Vector3 (0, 0, 0);
            //max = new Vector3 (cam.pixelWidth, cam.pixelHeight, 0);
            max = new Vector3 (Screen.width, Screen.height, 0f);
            //get the tooltip position with offset
            // This one also adds the width of the rect which doesn't work in a lot of situations so.
            Vector2 position = new Vector2 (Input.mousePosition.x, Input.mousePosition.y) + offset;
            //clamp it to the screen size so it doesn't go outside
            targetMoveTransform.position = position;
            //targetMoveTransform.position = new Vector3 (Mathf.Clamp (position.x, min.x + rect.rect.width / 2, max.x - rect.rect.width / 2), Mathf.Clamp (position.y, min.y + rect.rect.height / 2, max.y - rect.rect.height / 2), position.z);
        }
    }
    void OnDestroy () {
        if (destroySelfAutomatically) {
            Destroy (spawnedTooltip);
        }
    }
}