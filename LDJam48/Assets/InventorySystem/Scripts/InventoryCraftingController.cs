using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[System.Serializable]
public class ItemCrafted : UnityEvent<InventoryController, ItemBlueprintData> { }

public class InventoryCraftingController : MonoBehaviour {
    public InventoryController parentController;

    public List<ItemBlueprintType> craftableTypes = new List<ItemBlueprintType> { ItemBlueprintType.ANY };
    public CanvasGroup canvasGroup;
    public Button craftButton;

    public UI_ItemBox exampleResult;
    public bool startOff = true;
    private bool m_active;
    private bool m_checkingActive = true;

    public ItemBlueprintData targetBlueprint;
    private Coroutine crafting;
    private static List<ItemBlueprintData> allBlueprintDatas = new List<ItemBlueprintData> { };
    private Dictionary<ItemBlueprintType, List<ItemBlueprintData>> blueprintLookupDict = new Dictionary<ItemBlueprintType, List<ItemBlueprintData>> { };
    public List<ItemBlueprintData> craftableBlueprints = new List<ItemBlueprintData> { };

    public ItemCrafted itemCraftedEvent;

    void Awake () {
        LoadAllBlueprintDatas ();
        UpdateBlueprintList ();
        if (canvasGroup == null) {
            canvasGroup = GetComponent<CanvasGroup> ();
        }
        if (parentController == null) {
            parentController = GetComponentInParent<InventoryController> ();
        }
    }
    // Start is called before the first frame update
    void Start () {
        if (startOff) {
            Active = false;
        } else {
            UpdateCraftability = true;
            Active = true;
        }

        parentController.itemAddedEvent.AddListener (CheckCraftability);
        parentController.itemRemovedEvent.AddListener (CheckCraftability);

        craftButton.onClick.AddListener (Craft);
        CheckCraftability (null, null);
    }

    [NaughtyAttributes.Button]
    public void UpdateBlueprintList () {
        craftableBlueprints.Clear ();
        craftableBlueprints = AllowedBlueprints ();
    }

    public void CheckCraftability (InventoryController controller, Item_DragAndDrop item) { // Checks if it's possible to make anything!
        if (Active && UpdateCraftability) {
          //  Debug.Log ("Item has been moved to crafting parent controller " + parentController.name);
            foreach (ItemBlueprintData data in craftableBlueprints) {
                Debug.Log ("Can craft: " + data.m_id + " = " + CanCraft (data));
                if (CanCraft (data)) {
                    targetBlueprint = data;
                    SetCraftingButtonActive (true);
                    CreateExampleCopy (data.m_result, data.m_stackAmount);
                    return;
                }
            }
        }
        SetCraftingButtonActive (false);
        CreateExampleCopy (null, -1);
        targetBlueprint = null;
    }

    void CreateExampleCopy (ItemData example, int amount) {
        if (example == null) {
            exampleResult.gameObject.SetActive (false);
        } else {
            exampleResult.gameObject.SetActive (true);
            exampleResult.SetItemBoxData (example);
            exampleResult.StackSize = amount;
        }
    }

    public bool CanCraft (ItemBlueprintData data) {
        List<int> results = new List<int> { };
        // make a list of required results
        foreach (BlueprintComponent component in data.m_componentsNeeded) {
            int itemCount = parentController.CountItem (component.data);
            if (itemCount > 0) {
                if (itemCount >= component.amount) {
                    results.Add (1);
                } else {
                    results.Add (0);
                }
            } else {
                results.Add (0);
            }
        }
        return !results.Contains (0);
    }

    void Craft () {
        if (crafting == null) {
            crafting = StartCoroutine (Crafting (targetBlueprint));
        } else {
            SetCraftingButtonActive (false);
        }
    }

    IEnumerator Crafting (ItemBlueprintData data) {
        bool success = false;
        foreach (BlueprintComponent component in data.m_componentsNeeded) {
            success = parentController.DestroyItemAmount (component.data, component.amount);
        }
        if (success) {
            success = parentController.AddItem (data.m_result, data.m_stackAmount);
            Debug.Log ("<color=green> Successfully crafted " + data.m_result.m_id + "</color>");
            itemCraftedEvent.Invoke (parentController, data);
        }
        if (!success) {
            Debug.LogWarning ("<color=red>Failed to craft an object!</color>");
        }
        yield return null;
        crafting = null;
    }
    public bool Active {
        get {
            return m_active;
        }
        set {
            m_active = value;
            UpdateCraftability = value;
            SetVisible ();
        }
    }

    public bool UpdateCraftability {
        get {
            return m_checkingActive;
        }
        set {
            m_checkingActive = value;
        }
    }

    void SetCraftingButtonActive (bool active) {
        craftButton.interactable = active;
    }

    public List<ItemBlueprintData> AllowedBlueprints () {
        List<ItemBlueprintData> returnList = new List<ItemBlueprintData> { };
        if (craftableTypes.Contains (ItemBlueprintType.ANY)) { // Any -> create a copy of the alldata list and send it back!
            foreach (ItemBlueprintData data in allBlueprintDatas) {
                returnList.Add (data);
            }
        } else {
            foreach (ItemBlueprintType type in craftableTypes) {
                if (blueprintLookupDict.ContainsKey (type)) {
                    returnList.AddRange (blueprintLookupDict[type]);
                };
            };
        }
        return returnList;
    }

    void SetVisible () { // pretty code here for active/deactivate
        canvasGroup.interactable = Active;
        canvasGroup.blocksRaycasts = Active;
        canvasGroup.alpha = Active ? 1f : 0f;
    }

    void LoadAllBlueprintDatas () {
        // Load all blueprint datas!
        if (allBlueprintDatas.Count == 0) {
            Object[] loadedDatas = Resources.LoadAll ("Data/Blueprints", typeof (ItemBlueprintData));
            foreach (Object obj in loadedDatas) {
                allBlueprintDatas.Add (obj as ItemBlueprintData);
            }
        }
        blueprintLookupDict.Clear ();
        // Add to lookup dictionary for quick lookup of blueprints according to type
        foreach (ItemBlueprintData data in allBlueprintDatas) { // We don't add nones or anys, to allow for easy disabling
            if (data.m_type != ItemBlueprintType.NONE || data.m_type != ItemBlueprintType.ANY) {
                if (blueprintLookupDict.ContainsKey (data.m_type)) {
                    blueprintLookupDict[data.m_type].Add (data);
                    //Debug.Log ("<color=blue>Added blueprint to list: " + data.m_type + "</color>");
                } else {
                    List<ItemBlueprintData> newList = new List<ItemBlueprintData> { };
                    newList.Add (data);
                    blueprintLookupDict.Add (data.m_type, newList);
                    //Debug.Log ("<color=blue>Created new list: " + data.m_type + "</color>");
                }
            }
        };
    }

    void DebugTest () {
        Debug.Log ("Size: " + blueprintLookupDict.Count);
        foreach (KeyValuePair<ItemBlueprintType, List<ItemBlueprintData>> kvp in blueprintLookupDict) {
            Debug.Log ("Type: " + kvp.Key);
            foreach (ItemBlueprintData data in kvp.Value) {
                Debug.Log ("Value: " + data.m_id);
            }
        }
    }

    // Update is called once per frame
    void Update () {
        //if (Active && UpdateCraftability) {
        //    CheckCraftability ();
        //}
    }
}