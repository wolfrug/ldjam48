using System.Collections;
using System.Collections.Generic;
using Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

[System.Serializable]
public class InventoryOpened : UnityEvent<InventoryController> { }

[System.Serializable]
public class InventoryClosed : UnityEvent<InventoryController> { }

[System.Serializable]
public class InventoryItemAdded : UnityEvent<InventoryController, Item_DragAndDrop> { }

[System.Serializable]
public class InventoryItemRemoved : UnityEvent<InventoryController, Item_DragAndDrop> { }

public class InventoryController : MonoBehaviour {

    // Static list of all inventories (in lieu of managers etc)
    public static List<InventoryController> allInventories = new List<InventoryController> { };

    public InventoryData data;
    public InventoryType type = InventoryType.DEFAULT;
    public List<InventoryType> permittedItemSources = new List<InventoryType> { InventoryType.DEFAULT };
    public Canvas mainCanvas;
    public CanvasGroup canvasGroup;
    public TextMeshProUGUI inventoryName;
    public TextMeshProUGUI inventorySpaceLeft;
    private int defaultSortOrder = 0;
    public GameObject itemBoxPrefab;
    public Transform inventoryParent;
    public Inventory_StackManipulator stackManipulator;
    public InventoryContextMenuController contextMenuController;
    public InventoryCraftingController craftingController;

    public Inventory_ConsumeWatcher consumeWatcher;
    public Item_DragTarget mainDragTarget;
    public int maxSlots = 99;
    public bool isDragging = false;
    public float timeUntilDragStarted = 0.5f;
    public List<Item_DragAndDrop> allItemBoxes = new List<Item_DragAndDrop> { };
    public bool clearOnStart = false;
    public bool hideOnInit = true;

    public bool spawnItemBoxSeparately = true;
    private bool m_isActive = false;

    private static List<ItemData> allItemDatas = new List<ItemData> { };

    private Dictionary<string, string> savedInventory = new Dictionary<string, string> { };

    public InventoryOpened inventoryOpenedEvent;
    public InventoryClosed inventoryClosedEvent;

    public InventoryItemAdded itemAddedEvent;
    public InventoryItemRemoved itemRemovedEvent;

    void OnEnabled () {
        if (!allInventories.Contains (this)) {
            allInventories.Add (this);
        }
    }

    void OnDestroyed () {
        if (allInventories.Contains (this)) {
            allInventories.Remove (this);
        }
    }

    void Awake () {
        if (!allInventories.Contains (this)) {
            allInventories.Add (this);
        }
        LoadAllItemDatas ();
    }

    [NaughtyAttributes.Button]
    void SetUniqueName () {
        gameObject.name = gameObject.name + IndexOfInventoryID (data.m_id, this);
    }

    // Start is called before the first frame update
    void Start () {

        if (mainCanvas == null) {
            mainCanvas = GetComponent<Canvas> ();
        }
        if (mainCanvas == null) {
            canvasGroup = GetComponent<CanvasGroup> ();
        }
        defaultSortOrder = mainCanvas.sortingOrder;

        // Add all item boxes to the allItemBoxes list and add listeners to them
        allItemBoxes.Clear ();
        for (int i = 0; i < inventoryParent.childCount; i++) {
            Transform child = inventoryParent.GetChild (i);
            Item_DragAndDrop tryGetBox = child.GetComponentInChildren<Item_DragAndDrop> ();
            if (tryGetBox != null) {
                AddItemBox (tryGetBox);
            }
        }

        // Set up inventory at start (and clears existing inventory)
        InitInventory (data, clearOnStart);

        // This is the 'backup' dragtarget, letting you just drag and drop into the inventory
        if (mainDragTarget != null) {
            mainDragTarget.dragCompletedEvent.AddListener (OnDragCompleted);
        }
        // Add listeners to stack manipulator onfinished
        stackManipulator.combineFinishedEvent.AddListener (FinishCombineStackManipulation);
        stackManipulator.splitFinishedEvent.AddListener (FinishSplitStackManipulation);
        // Hide/unhide the inventory!
        if (hideOnInit) {
            HideInventory (true);
        } else {
            ShowInventory (true);
        }
    }

    void LoadAllItemDatas () {
        // Load all item datas!
        if (allItemDatas.Count == 0) {
            Object[] loadedDatas = Resources.LoadAll ("Data/Items", typeof (ItemData));
            foreach (Object obj in loadedDatas) {
                allItemDatas.Add (obj as ItemData);
            }
        }
    }

    [NaughtyAttributes.Button]
    void ClearAndReinitInventory () {
        InitInventory (data, true);
    }

    public void InitInventory (InventoryData newData, bool clearPrevious = false) {

        if (clearPrevious) {
            ClearInventory ();
        }
        if (newData != null) {
            data = newData;
        }
        if (data != null) {
            maxSlots = data.m_defaultMaxSlots;
            UpdateInventoryUI ();
            if (data.m_inventoryItemPrefab != null) {
                itemBoxPrefab = data.m_inventoryItemPrefab;
            }
            type = data.m_type;
            if (data.m_defaultContent.Length > 0) {
                // Create dictionary of weighted data
                Dictionary<RandomizedInventoryItem, float> randomWeightedDictionary = new Dictionary<RandomizedInventoryItem, float> ();
                // These are the items to add - we add guaranteed items to it right away
                List<RandomizedInventoryItem> itemsToAdd = new List<RandomizedInventoryItem> { };
                foreach (RandomizedInventoryItem data in data.m_defaultContent) {
                    if (data.guaranteed) {
                        itemsToAdd.Add (data);
                    } else {
                        randomWeightedDictionary.Add (data, data.weight);
                    };
                }
                // Sets the number of iterations we'll do
                int itemsToSpawn = data.m_defaultContent.Length - itemsToAdd.Count;
                if (data.m_minMaxRandomItemsSpawned.x > -1 && data.m_minMaxRandomItemsSpawned.y > 0) {
                    itemsToSpawn = Random.Range (data.m_minMaxRandomItemsSpawned.x, data.m_minMaxRandomItemsSpawned.y);
                }
                // iterate through the list and add as many as randomly determined to it...
                if (itemsToSpawn > 0) {
                    for (int i = 0; i < itemsToSpawn; i++) {
                        RandomizedInventoryItem randomItem = randomWeightedDictionary.RandomElementByWeight (e => e.Value).Key;
                        if (randomItem.random_unique) {
                            randomWeightedDictionary.Remove (randomItem);
                        }
                        itemsToAdd.Add (randomItem);
                    };
                };
                // and finally actually add them, with randomized stack sizes!
                foreach (RandomizedInventoryItem item in itemsToAdd) {
                    int randomStackSize = Random.Range (item.randomStackSize.x, item.randomStackSize.y);
                    if (randomStackSize > 0 && item.data != null) { // This setup allows for null datas, which can empty out slots!
                        AddItem (item.data, randomStackSize);
                    };
                }

            }
            if (data.m_allowContentFrom.Length > 0) {
                foreach (InventoryType type in data.m_allowContentFrom) {
                    if (!permittedItemSources.Contains (type)) {
                        permittedItemSources.Add (type);
                    }
                }
            }
            // INIT ENGINE
            if (consumeWatcher != null) {
                consumeWatcher.InitEngine ();
            }

        }
    }

    void ClearInventory () { // -destroys- the inventory, omg
        foreach (Item_DragAndDrop box in allItemBoxes) {
            GameObject.Destroy (box.gameObject);
        }
        allItemBoxes.Clear ();
    }
    void UpdateInventoryUI () {
        if (inventoryName != null) {
            inventoryName.text = data.m_displayName;
        }
        if (inventorySpaceLeft != null) {
            inventorySpaceLeft.text = string.Format ("{0}/{1}", maxSlots - SpaceLeft, maxSlots);
        }
    }

    public int SpaceLeft {
        get {
            int usedSlots = 0;
            foreach (Item_DragAndDrop item in allItemBoxes) {
                if (item.targetBox.data != null) {
                    usedSlots += item.targetBox.data.m_sizeInInventory;
                } else { // Default if it doesn't have data for whatever reason - make it 0 or 1
                    usedSlots += 0;
                }
            }
            return maxSlots - usedSlots;
        }
    }

    public bool AddItem (ItemData itemdata, int stackAmount = 1) { // add a box based on data, must spawn
        if (maxSlots >= allItemBoxes.Count) {
            Item_DragAndDrop newBox = SpawnBox (itemdata);
            newBox.targetBox.SetItemBoxData (itemdata);
            newBox.targetBox.StackSize = stackAmount;
            newBox.gameObject.name = itemdata.m_displayName + "_InventoryItem";
            return AddItemBox (newBox);
        } else {
            return false;
        }
    }

    public bool AddItemBox (Item_DragAndDrop newItemBox = null) { // Set to null to have it spawn one, otherwise it'll make one

        if (SpaceLeft > 0) {
            if (newItemBox == null) { // We spawn a new box if the item is null, though this is a bit risky...
                Debug.LogWarning ("Attempted to add empty item! Use AddItem to spawn from data!");
                return false;
            } else {
                if (newItemBox.transform.parent != inventoryParent) { // if we're adding from outside the inventory
                    newItemBox.transform.SetParent (inventoryParent);
                }
            }
            if (!allItemBoxes.Contains (newItemBox)) { // Add to list and also begin listening to the dragtarget
                allItemBoxes.Add (newItemBox);
                if (newItemBox.targetBox.dragTarget != null) {
                    newItemBox.targetBox.dragTarget.dragCompletedEvent.AddListener (OnDragCompleted);
                    newItemBox.targetBox.dragTarget.pointerDownEvent.AddListener (OnClickedItemBox);
                };
                newItemBox.dragEnded.AddListener (OnDragEnd);
                newItemBox.dragStarted.AddListener (OnDragStart);
                newItemBox.UpdateInteractability ();
            }
            UpdateInventoryUI ();
            itemAddedEvent.Invoke (this, newItemBox);
            return true;
        } else {
            Debug.Log ("Not enough space!");
            return false;
        }
    }

    public bool RemoveItemBox (Item_DragAndDrop targetItem) { // Mainly to remove all the listeners - does -not- change the object's parent!
        if (allItemBoxes.Contains (targetItem)) {
            allItemBoxes.Remove (targetItem);
            if (targetItem.targetBox.dragTarget != null) {
                targetItem.targetBox.dragTarget.dragCompletedEvent.RemoveListener (OnDragCompleted);
                targetItem.targetBox.dragTarget.pointerDownEvent.RemoveListener (OnClickedItemBox);
            };

            targetItem.dragEnded.RemoveListener (OnDragEnd);
            targetItem.dragStarted.RemoveListener (OnDragStart);
            UpdateInventoryUI ();
            itemRemovedEvent.Invoke (this, targetItem);
            return true;
        } else {
            return false;
        }
    }

    Item_DragAndDrop SpawnBox (ItemData data) {
        GameObject spawnedObj = Instantiate (itemBoxPrefab, inventoryParent);
        Item_DragAndDrop spawnedItem = spawnedObj.GetComponent<Item_DragAndDrop> ();
        if (spawnItemBoxSeparately) { // we assume the item box -isn't- included in the prefab
            GameObject itemBox = Instantiate (data.m_prefab, spawnedObj.transform);
            UI_ItemBox targetBox = itemBox.GetComponent<UI_ItemBox> ();
            targetBox.data = data;
            spawnedItem.targetBox = targetBox;
            spawnedItem.targetTransform = itemBox.transform;
        }
        return spawnedItem;
    }
    public void DestroyBox (Item_DragAndDrop target) { // can also be used to destroy 'foreign' boxes
        if (allItemBoxes.Contains (target)) { // local box, np
            RemoveItemBox (target);
            Destroy (target.gameObject);
        } else {
            InventoryController targetObjectParent = target.GetComponentInParent<InventoryController> ();
            if (targetObjectParent == null) { // No parent? Floating magical lonely boy. Oh well, kill then!
                Destroy (target.gameObject);
            } else { // send it to them to destroy
                targetObjectParent.DestroyBox (target);
            }
        }
    }

    int clicked = 0;
    float clicktime = 0;
    float clickdelay = 0.5f;
    void OnClickedItemBox (Item_DragTarget target, PointerEventData coll) { // Context menu!
        if (coll.button == PointerEventData.InputButton.Right) {
            Debug.Log ("Right-clicked Clicked " + target.gameObject.name);
            if (contextMenuController != null) {
                contextMenuController.SelectItem (target.gameObject.GetComponent<UI_ItemBox> ()); // no let's not do this
            }
        } else if (coll.button == PointerEventData.InputButton.Left) {
            clicked++;
            if (clicked == 1) clicktime = Time.time;
            if (clicked > 1 && Time.time - clicktime < clickdelay) {
                clicked = 0;
                clicktime = 0;
                contextMenuController.ForceSelectOption (ContextMenuEntryType.UI_DROP, target.gameObject.GetComponent<UI_ItemBox> ());
            } else if (clicked > 2 || Time.time - clicktime > 1) clicked = 0;
        }
    }

    void OnDragStart (Item_DragAndDrop target) {
        isDragging = true;
        // Debug.Log ("Started drag");
        target.transform.SetAsLastSibling ();
        mainCanvas.sortingOrder = 999;
    }
    void OnDragEnd (Item_DragAndDrop target) {
        isDragging = false;
        //  Debug.Log ("Ended drag");
        target.ResetDragTargetPosition ();
        mainCanvas.sortingOrder = defaultSortOrder;
    }
    void OnDragCompleted (Item_DragAndDrop dragAndDropItem, Item_DragTarget target) {
        //  Debug.Log (name + " completed drag of " + dragAndDropItem.name + " on " + target.name, target.gameObject);
        TryTakeItem (dragAndDropItem, target);
    }

    public void TryTakeItem (Item_DragAndDrop dragAndDropItem, Item_DragTarget target) {
        if (allItemBoxes.Contains (dragAndDropItem)) {
            //   Debug.Log ("Move inside own inventory!");
            if (!TryCombineInternal (dragAndDropItem, target.GetComponentInParent<Item_DragAndDrop> ())) { // try to combine, if not, switch places
                SwitchPlacesInsideInventory (dragAndDropItem, target.GetComponentInParent<Item_DragAndDrop> ());
            };
        } else {
            //    Debug.Log ("Moved from different inventory! (report from: " + name + ")", gameObject);
            if (!TryCombineOrSplitExternal (dragAndDropItem, target.GetComponentInParent<Item_DragAndDrop> ())) {
                TryTakeItemFromInventory (dragAndDropItem, target.GetComponentInParent<Item_DragAndDrop> ());
            };
        }
    }

    public void TryTakeAll (InventoryController targetController = null) { // simple attempt to take all, ignoring splitting etc

        //   Debug.Log ("Trying to take all");
        if (targetController == null || targetController == this) { // if null, try to find the first inventory active that lets us steal their shit
            List<InventoryController> permittedInventories = InventoryController.GetPermittedInventoriesForType (type, this);
            if (permittedInventories.Count > 0) {
                foreach (InventoryController inv in permittedInventories) {
                    if (inv.allItemBoxes.Count > 0) {
                        targetController = inv;
                        break;
                    }
                }
            }
            //targetController = InventoryController.GetPermittedInventoryForType (type, this);
        }
        if (targetController != null && targetController.allItemBoxes.Count > 0) {

            List<Item_DragAndDrop> copyList = new List<Item_DragAndDrop> (targetController.allItemBoxes);
            //    Debug.Log ("Attempting to iterate through list of length " + copyList.Count);
            foreach (Item_DragAndDrop item in copyList) {
                TryTakeItemFromInventory (item, null);
            }
        };
    }

    bool TryCombineInternal (Item_DragAndDrop draggedItem, Item_DragAndDrop targetItem) {
        // Both items are internal -> see if they are compatible and try to make a stack
        //   Debug.Log ("Trying to do an internal combination!");
        if (targetItem != null) {
            if (draggedItem.targetBox.data == targetItem.targetBox.data) {
                if (targetItem.targetBox.StackSize < targetItem.targetBox.data.m_maxStackSize) { // there's space!
                    stackManipulator.StartManipulator (draggedItem, targetItem);
                    return true;
                };
            } else {
                //      Debug.Log ("Internal combination failed: incompatible data");
            }
        } else {
            //   Debug.Log ("Internal combination failed: targetItem null");
        }
        return false;
    }
    bool TryCombineOrSplitExternal (Item_DragAndDrop draggedItem, Item_DragAndDrop targetItem) {
        //   Debug.Log ("Trying to do an external combination!");
        // Check if there's space, to prevent splitting letting you go over max inventory size
        if (SpaceLeft > 0 || targetItem != null) {
            // Dragged item is external; if it has a stack size, let's let the player pick how much is dropped
            if (draggedItem.targetBox.StackSize > 1 || (draggedItem.targetBox.StackSize == 1 && targetItem != null)) {
                stackManipulator.StartManipulator (draggedItem, targetItem);
                return true;
            }
        }
        return false;
    }

    void SwitchPlacesInsideInventory (Item_DragAndDrop switcher, Item_DragAndDrop switchee) { // switches places internally, using sibling index
        if (switchee != null) {
            int targetIndex = switchee.transform.GetSiblingIndex ();
            if (Input.mousePosition.x > switchee.transform.position.x) { // mouse is to the right of it, place it in front
                if (targetIndex < inventoryParent.childCount - 1) {
                    switcher.transform.SetSiblingIndex (targetIndex + 1);
                } else {
                    switcher.transform.SetAsLastSibling ();
                }
            } else { // mouse is to the left of it, place it behind
                switcher.transform.SetSiblingIndex (targetIndex);
            }
        }
    }

    void TryTakeItemFromInventory (Item_DragAndDrop item, Item_DragAndDrop targetSlot) { // if allowed - assumption is if inventories are visible, they'll take stuff
        InventoryController targetObjectParent = item.GetComponentInParent<InventoryController> ();
        if (targetObjectParent == null) { // No parent? Floating magical lonely boy. Oh well, yoink.
            AddItemBox (item);
        } else {
            if (permittedItemSources.Contains (targetObjectParent.type)) { // permitted inventory type!
                bool attemptTake = AddItemBox (item);
                if (attemptTake) { // success! remove the box from the other inventory
                    targetObjectParent.RemoveItemBox (item);
                    SwitchPlacesInsideInventory (item, targetSlot);
                    //Debug.Log (name + " stole item " + item.name + " successfully from " + targetObjectParent.name);
                } else { // else we fail and just let it do whatever
                    //Debug.Log (name + " attempted to steal item " + item.name + " from " + targetObjectParent.name + " but failed (no space)");
                }
            } else {
                //Debug.Log (name + " attempted to steal item " + item.name + " from " + targetObjectParent.name + " but failed (not permitted)");
            }
        }
    }

    void FinishCombineStackManipulation (Item_DragAndDrop selectedBox, Item_DragAndDrop combiner) {
        //Debug.Log ("Finished Combine Action. Result: ");
        if (combiner == null) { // basically a 'failed' combine, can just ignore
            //Debug.Log ("failed (combiner null)");
            return;
        }
        if (selectedBox.targetBox.StackSize < 1) { //the dragged box is empty! Destroy it
            //Debug.Log ("success (targetBox empty)");
            DestroyBox (selectedBox);
        } else if (combiner.targetBox.StackSize < 1) { // erm, other box is empty? destroy it?
            //Debug.Log ("success (combiner box empty)");
            DestroyBox (combiner);
        }
    }
    void FinishSplitStackManipulation (Item_DragAndDrop selectedBox, int stackAmount) {
        //Debug.Log ("Finished Split Action. Result: ");
        if (stackAmount == 0) { // Failed split, ignore
            //  Debug.Log ("failed (0 stack)");
            return;
        }
        if (selectedBox.targetBox.StackSize < 1) { // split all from it somehow? Destroy
            //  Debug.Log ("success: dragged box emptied");
            DestroyBox (selectedBox);
        }
        if (stackAmount > 0) { // create a new copy of the box and add it to ourselves
            //  Debug.Log ("success: created new stacked box");
            AddItem (selectedBox.targetBox.data, stackAmount);
        }
    }

    public List<Item_DragAndDrop> ItemsByTrait (ItemTrait trait) {
        List<Item_DragAndDrop> returnList = new List<Item_DragAndDrop> { };
        foreach (Item_DragAndDrop item in allItemBoxes) {
            if (item.targetBox.data.HasTrait (trait)) {
                returnList.Add (item);
            }
        }
        return returnList;
    }
    public List<Item_DragAndDrop> ItemsByData (ItemData data) {
        List<Item_DragAndDrop> returnList = new List<Item_DragAndDrop> { };
        foreach (Item_DragAndDrop item in allItemBoxes) {
            if (item.targetBox.data == data) {
                returnList.Add (item);
            }
        }
        return returnList;
    }

    public int CountItem (ItemData itemData) {
        int returnValue = 0;
        foreach (Item_DragAndDrop item in allItemBoxes) {
            if (item.targetBox.data == itemData) {
                returnValue += item.targetBox.StackSize;
            }
        }
        return returnValue;
    }
    public bool DestroyItemAmount (ItemData itemData, int amount) { // returns true if successful
        if (CountItem (itemData) < amount) { // we don't have enough, return
            return false;
        }
        int amountLeft = amount;
        foreach (Item_DragAndDrop item in allItemBoxes) { // iterate through all and destroy until we reach the end
            if (item.targetBox.data == itemData) {
                if (item.targetBox.StackSize >= amountLeft) {
                    item.targetBox.StackSize -= amountLeft;
                    if (item.targetBox.StackSize <= 0) {
                        DestroyBox (item);
                    }
                    return true;
                } else {
                    amountLeft -= item.targetBox.StackSize;
                    DestroyBox (item);
                }
            }
        }
        return true;
    }

    public bool Visible {
        get {
            return m_isActive;
        }
        set {
            if (value) {
                ShowInventory ();
            } else {
                HideInventory ();
            }
        }
    }

    public void ShowInventory (bool force = false) {
        // Fancy code for showing inventory, maybe some doozy control graphs
        if (!Visible || force) { // Only want to invoke event if actually hidden
            canvasGroup.interactable = true;
            canvasGroup.alpha = 1f;
            canvasGroup.blocksRaycasts = true;
            m_isActive = true;
            inventoryOpenedEvent.Invoke (this);
        };
    }
    public void HideInventory (bool force = false) {
        // Fancy code for hiding inventory, maybe some doozy control graphs
        if (Visible || force) { // we don't want to invoke the event for this unless we're actually hidden
            canvasGroup.interactable = false;
            canvasGroup.alpha = 0f;
            canvasGroup.blocksRaycasts = false;
            m_isActive = false;
            stackManipulator.Active = false;
            inventoryClosedEvent.Invoke (this);
        };
    }

    // HELPER FUNCTIONS

    public static List<InventoryController> GetAllInventories (InventoryType type = InventoryType.NONE, InventoryController excluded = null, bool onlyActive = true) { // returns list of all inventories of type, optionally excluding one (typically 'self')
        List<InventoryController> returnList = new List<InventoryController> ();
        foreach (InventoryController inv in allInventories) {
            if ((onlyActive && inv.Visible) || !onlyActive) {
                if (inv.type == type || type == InventoryType.NONE) {
                    if (excluded != null) {
                        if (inv != excluded) {
                            returnList.Add (inv);
                        }
                    } else {
                        returnList.Add (inv);
                    }
                }
            }
        };
        return returnList;
    }
    public static InventoryController GetInventoryOfType (InventoryType type, InventoryController excluded = null, bool onlyActive = true) { // returns the first inventory of type it finds, optionally excluding one (typically 'self')
        foreach (InventoryController inv in allInventories) {
            if ((onlyActive && inv.Visible) || !onlyActive) {
                if (inv.type == type) {
                    if (excluded != null) {
                        if (inv != excluded) {
                            return inv;
                        }
                    } else {
                        return inv;
                    }
                }
            }
        }
        return null;
    }

    public static InventoryController GetPermittedInventoryForType (InventoryType type, InventoryController excluded = null, bool onlyActive = true) {
        foreach (InventoryController inv in allInventories) {
            if ((onlyActive && inv.Visible) || !onlyActive) {
                if (inv.data.AllowsContentFrom (type)) {
                    if (excluded != null) {
                        if (inv != excluded) {
                            return inv;
                        }
                    } else {
                        return inv;
                    }
                }
            }
        }
        return null;
    }
    public static List<InventoryController> GetPermittedInventoriesForType (InventoryType type, InventoryController excluded = null, bool onlyActive = true) {
        List<InventoryController> returnList = new List<InventoryController> ();
        foreach (InventoryController inv in allInventories) {
            if ((onlyActive && inv.Visible) || !onlyActive) {
                if (inv.data.AllowsContentFrom (type)) {
                    if (excluded != null) {
                        if (inv != excluded) {
                            returnList.Add (inv);
                        }
                    } else {
                        returnList.Add (inv);
                    }
                }
            }
        }
        return returnList;
    }

    public static InventoryController SpawnInventory (InventoryData data, bool visibleOnStart = true) { // spawns a new inventory with data
        InventoryController newController = null;
        if (data != null) {
            if (data.m_inventoryCanvasPrefab != null) {
                newController = Instantiate (data.m_inventoryCanvasPrefab).GetComponent<InventoryController> ();
                if (newController != null) {
                    newController.data = data;
                    newController.hideOnInit = !visibleOnStart;
                }
            }
        }
        return newController;
    }

    public static void CloseAllInventories (InventoryType ofType = InventoryType.NONE) { // closes all inventories/all inventories of type
        foreach (InventoryController inv in allInventories) {
            if (ofType == InventoryType.NONE) {
                inv.Visible = false;
            } else {
                if (inv.type == ofType) {
                    inv.Visible = false;
                }
            }
        }
    }
    public static void OpenAllInventories (InventoryType ofType = InventoryType.PLAYER) { // opens all inventories / all inventories of type
        foreach (InventoryController inv in allInventories) {
            if (ofType == InventoryType.NONE) {
                inv.Visible = true;
            } else {
                if (inv.type == ofType) {
                    inv.Visible = true;
                }
            }
        }
    }

    public static ItemData GetDataByID (string id) {
        foreach (ItemData data in allItemDatas) {
            if (data.m_id == id) {
                return data;
            }
        }
        return null;
    }

    public static int IndexOfInventoryID (string dataID, InventoryController controller) {
        // returns -1 if it is the only controller with that data.m_id, otherwise returns their index in allInventories
        bool duplicatesFound = false;
        int indexOf = -1;
        for (int i = 0; i < allInventories.Count; i++) {
            InventoryController inv = allInventories[i];
            if (inv == controller) {
                indexOf = i;
            } else {
                if (inv.data.m_id == dataID) { // multiples of same id, use their index instead
                    duplicatesFound = true;
                }
            }
        }
        if (duplicatesFound) {
            return indexOf;
        } else {
            return -1;
        }
    }

    [NaughtyAttributes.Button]
    void DebugSaveInventory () {
        SaveInventory ();
    }

    [NaughtyAttributes.Button]
    void DebugLoadInventory () {
        LoadInventory ();
    }

    private List<string> savedStrings = new List<string> { };
    public void SaveInventory () {
        int index = IndexOfInventoryID (data.m_id, this);
        savedInventory.Clear ();
        for (int i = 0; i < allItemBoxes.Count; i++) {
            // We add each item separately by their index + stack amount and their ID
            string itemId = allItemBoxes[i].targetBox.data.m_id;
            string indexWithStack = string.Format ("{0}_{1}", i, allItemBoxes[i].targetBox.StackSize);
            savedInventory.Add (indexWithStack, itemId);
        }
        // Saves based on game object name
        string inventorySaveName = string.Format ("{0}({1})", data.m_id.ToString (), gameObject.name);
        if (savedStrings.Contains (inventorySaveName)) {
            Debug.LogWarning ("Warning: two inventories cannot have the same game object name and inventory m_id, they will override one another! (" + inventorySaveName + ")", gameObject);
        }
        ES3.Save<Dictionary<string, string>> ("SavedInventory_" + inventorySaveName, savedInventory);
        Debug.Log ("Saved inventory: " + inventorySaveName);
    }

    public void LoadInventory () {
        int index = IndexOfInventoryID (data.m_id, this);
        string inventorySaveName = string.Format ("{0}({1})", data.m_id.ToString (), gameObject.name);
        savedInventory = ES3.Load<Dictionary<string, string>> ("SavedInventory_" + inventorySaveName, savedInventory);
        ClearInventory ();
        LoadAllItemDatas ();
        foreach (KeyValuePair<string, string> kvp in savedInventory) {
            ItemData data = GetDataByID (kvp.Value);
            int itemIndex = int.Parse (kvp.Key.Split ('_') [0]); // we don't actually use the index thoughhhh
            int itemStack = int.Parse (kvp.Key.Split ('_') [1]); // stack size!
            AddItem (data, itemStack);
        }
        Debug.Log ("Loaded inventory: " + inventorySaveName);
    }

    public void ClearSavedInventory (int index) {
        string inventorySaveName = string.Format ("{0}({1})", data.m_id.ToString (), index);
        ES3.DeleteKey (inventorySaveName);
    }

    // Update is called once per frame
    void Update () {

    }
}