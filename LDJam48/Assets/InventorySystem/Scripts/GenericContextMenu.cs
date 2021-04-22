using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[System.Serializable]
public class OptionSelectedEvent : UnityEvent<ContextMenuEntryType, GameObject> { }

[System.Serializable]
public class ContextMenuOpenEvent : UnityEvent<bool> { }

public enum ContextMenuType { // use these with the helper methods to get your desired context menu without singleton pattern
 NONE = 0000,
 SINGLETON = 1000,
 WORLD = 2000,
 UI = 3000,
}

public enum ContextMenuEntryType {
    NONE = 1000,
    // WORLD STUFF
    LOOK_AT = 2000,
    PICK_UP = 3000,
    DROP = 3100,
    ACTIVATE = 4000,
    OPEN = 4200, // alt to activate
    TALK = 4001, // alt to activate
    DEACTIVATE = 4100,
    CLOSE = 4300, // alt to deactivate
    // UI STUFF
    UI_TAKE = 5000,
    UI_DROP = 5001,
    UI_USE = 5002,
    UI_LOOK = 5003,

}

[System.Serializable]
public class ContextMenuOption {
    public TMP_Dropdown.OptionData data;
    public ContextMenuEntryType type;
    public OptionSelectedEvent selectedEvent;
}

public class GenericContextMenu : MonoBehaviour { // Add to regular Unity dropdown menu. Slightly hacky. But works! Mix with clicker of some kind.

    // Can be used using singleton pattern, for very quick deployment
    //  public static GenericContextMenu instance;
    public ContextMenuType type = ContextMenuType.NONE;
    private static List<GenericContextMenu> allContextMenus = new List<GenericContextMenu> { };
    private bool active = true;
    public bool spawnAtMouse = true;
    [Tooltip ("Set to false to simply hide the first option (i.e. 'context menu title')")]
    public bool showFirstOptionDisabled = true;

    public GameObject currentContextMenuTarget;
    public TMP_Dropdown contextMenuDropDown;

    public string contextMenuTitle = "Choose an Action";

    public List<ContextMenuOption> options = new List<ContextMenuOption> { };

    public OptionSelectedEvent selectedOptionEvent;
    public ContextMenuOpenEvent menuOpened;
    public ContextMenuOpenEvent menuClosed;

    public bool currentlyVisible = false;
    public float waitBetweenOpen = 0.1f;
    private float lastOpen = 0f;
    private Dictionary<int, ContextMenuOption> currentMenuContent = new Dictionary<int, ContextMenuOption> { };
    private Dictionary<ContextMenuEntryType, ContextMenuOption> defaultMenuContent = new Dictionary<ContextMenuEntryType, ContextMenuOption> { };

    private int normalChildCount = 0;
    private Coroutine cancelWatcher = null;

    void Awake () {
        // Make default dictionary
        foreach (ContextMenuOption option in options) {
            defaultMenuContent.Add (option.type, option);
        }
        // Add to static list!
        if (!allContextMenus.Contains (this)) {
            allContextMenus.Add (this);
        }
        // singleton, comment out if not
        /*  if (instance == null) {
            instance = this;
        } else {
            Destroy (gameObject);
        }
        */
    }
    void OnDestroy () {
        // Remove from static list!
        if (allContextMenus.Contains (this)) {
            allContextMenus.Remove (this);
        }
    }
    void Start () {
        if (options.Count > 0) {
            PopulateDropDown (options);
            ShowMenu (false);
        }
        normalChildCount = contextMenuDropDown.transform.childCount;
        //Debug.Log ("Child count for dropdown: " + normalChildCount);
    }

    public bool Active {
        get {
            return active;
        }
        set {
            active = value;
            if (currentlyVisible && !active) {
                ShowMenu (false);
            }
        }
    }
    // Use this to populate based on a list of enums, using the 'defaults' listed on the context menu
    public void PopulateDropDownDefaults (List<ContextMenuEntryType> types, GameObject contextMenuTarget = null) {
        List<ContextMenuOption> actualOptionList = new List<ContextMenuOption> { };
        foreach (ContextMenuEntryType type in types) {
            ContextMenuOption outOption;
            defaultMenuContent.TryGetValue (type, out outOption);
            if (outOption != null) {
                actualOptionList.Add (outOption);
            }
        }
        if (actualOptionList.Count > 0) {
            PopulateDropDown (actualOptionList);
            currentContextMenuTarget = contextMenuTarget;
        }
    }

    // you can create your own context menu options also!
    public void PopulateDropDown (List<ContextMenuOption> entries) {
        if (active && (Time.time > lastOpen + waitBetweenOpen) && !currentlyVisible) { // waitbetweenopen hardcoded!
            contextMenuDropDown.ClearOptions (); // Clear old menu
            currentMenuContent.Clear (); // clear old menu
            List<TMP_Dropdown.OptionData> optionsToadd = new List<TMP_Dropdown.OptionData> { };

            // Add empty index 0 entry
            TMP_Dropdown.OptionData m_NewData = new TMP_Dropdown.OptionData ();
            m_NewData.text = contextMenuTitle;
            optionsToadd.Add (m_NewData);

            for (int i = 0; i < entries.Count; i++) { // Make a dictionary that corresponds the index with the type
                currentMenuContent.Add (i + 1, entries[i]); // makes dictionary of entries, with the i+1 being the proper index
                optionsToadd.Add (entries[i].data);

            };
            contextMenuDropDown.AddOptions (optionsToadd); // actually add the options from the list
            if (spawnAtMouse) {
                contextMenuDropDown.transform.position = Input.mousePosition;
            }
            contextMenuDropDown.SetValueWithoutNotify (-1);
            ShowMenu (true);
        }
    }

    public void ShowMenu (bool show) {
        if (show && active) {
            contextMenuDropDown.Show ();
            currentlyVisible = true;
            menuOpened.Invoke (true);
            // Slightly hacky
            GameObject firstOption = GameObject.Find ("Item 0: " + contextMenuTitle);
            firstOption.GetComponent<Toggle> ().interactable = false;
            if (!showFirstOptionDisabled) {
                firstOption.SetActive (false);
            }
            if (cancelWatcher == null) {
                cancelWatcher = StartCoroutine (CancelEventWatcher ());
            } else {
                StopCoroutine (cancelWatcher);
                cancelWatcher = StartCoroutine (CancelEventWatcher ());
            }
        } else {
            contextMenuDropDown.Hide ();
            currentlyVisible = false;
            menuClosed.Invoke (false);
            lastOpen = Time.time;
        }
    }

    public void SelectOption (int index) {
        StopCoroutine (cancelWatcher);
        ShowMenu (false);
        ContextMenuOption outType = null;
        currentMenuContent.TryGetValue (index, out outType);
        if (outType != null) {
            selectedOptionEvent.Invoke (outType.type, currentContextMenuTarget);
            outType.selectedEvent.Invoke (outType.type, currentContextMenuTarget);
            Debug.Log ("Selected index " + index + " which corresponds to type " + outType.type);
        }
    }

    public bool SelectOption (ContextMenuEntryType type) { // Used for other / manual attempts, returns if it succeeded or not
        if (currentlyVisible) {
            foreach (KeyValuePair<int, ContextMenuOption> kvp in currentMenuContent) {
                if (kvp.Value.type == type) {
                    SelectOption (kvp.Key);
                    return true;
                }
            }
        };
        return false;
    }

    IEnumerator CancelEventWatcher () {
        // Watch for 'cancel' event
        normalChildCount = contextMenuDropDown.transform.childCount;
        while (currentlyVisible) {
            yield return new WaitForSeconds (0.1f);
            Debug.Log ("Current childcount: " + contextMenuDropDown.transform.childCount + " normal childcount: " + normalChildCount);
            if (contextMenuDropDown.transform.childCount < normalChildCount) { // cancel event!!
                Debug.Log ("Context Menu cancelled");
                ShowMenu (false);
            }
        };
        cancelWatcher = null;
    }

    // STATIC HELPER FUNCTIONS
    public static GenericContextMenu GetMenuOfType (ContextMenuType type) {
        foreach (GenericContextMenu menu in allContextMenus) {
            if (menu.type == type) {
                return menu;
            }
        }
        Debug.LogWarning ("Did not find context menu of type " + type.ToString () + ": have you added one?");
        return null;
    }

    void Update () {

    }
}