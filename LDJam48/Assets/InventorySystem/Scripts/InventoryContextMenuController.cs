using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class ItemConsumed : UnityEvent<ItemData, int> { }

public class InventoryContextMenuController : MonoBehaviour {
    // Start is called before the first frame update
    public GenericContextMenu contextMenu;
    public InventoryController parentController;
    public InventoryController targetInventory;
    public Item_DragAndDrop currentlySelectedItem;

    public bool pickUpToPlayerInventory = true;
    public bool onlyUseInPlayerInventory = true;
    public ItemConsumed itemConsumedEvent;
    void Start () {
        contextMenu.selectedOptionEvent.AddListener (SelectOption);
    }

    public void SelectItem (UI_ItemBox selectedItem) {
        // Populate context menu and show....stuff
        Debug.Log ("Selected item: " + selectedItem);
        List<ContextMenuEntryType> addedEntries = new List<ContextMenuEntryType> { };
        if (selectedItem.draggable) { // Can be dragged, therefore can be moved around
            // Are there any inventories (not the parent) that accept the item in question? If so, add 'drop'.
            targetInventory = InventoryController.GetPermittedInventoryForType (parentController.type, parentController);
            if (targetInventory != null) {
                if (pickUpToPlayerInventory && targetInventory.type == InventoryType.PLAYER) {
                    addedEntries.Add (ContextMenuEntryType.UI_TAKE);
                } else {
                    addedEntries.Add (ContextMenuEntryType.UI_DROP);
                };
            }
        }
        if (selectedItem.consumable) { // Can be consumed -> add 'use'
            if (onlyUseInPlayerInventory && parentController.type == InventoryType.PLAYER) {
                addedEntries.Add (ContextMenuEntryType.UI_USE);
            } else if (!onlyUseInPlayerInventory) {
                addedEntries.Add (ContextMenuEntryType.UI_USE);
            }
        }
        contextMenu.PopulateDropDownDefaults (addedEntries, selectedItem.gameObject);
    }

    public void ForceSelectOption (ContextMenuEntryType actionType, UI_ItemBox selectedItem) {
        // DO this if we try to double-click for example on one or the other side
        if (actionType == ContextMenuEntryType.UI_DROP || actionType == ContextMenuEntryType.UI_TAKE) {
            if (selectedItem.draggable) { // Can be dragged, therefore can be moved around
                // Are there any inventories (not the parent) that accept the item in question? If so, add 'drop'.
                targetInventory = InventoryController.GetPermittedInventoryForType (parentController.type, parentController);
                if (targetInventory != null) {
                    SelectOption (actionType, selectedItem.gameObject);
                }
            }
        }
    }

    public void Cancel () {
        contextMenu.ShowMenu (false);
    }

    public void SelectOption (ContextMenuEntryType actionType, GameObject target) {
        Debug.Log ("Selected the option " + actionType);
        Item_DragAndDrop tryItem = target.GetComponentInParent<Item_DragAndDrop> ();
        if (tryItem == null) {
            Debug.LogWarning ("Inventory controller did not find the Item_DragAndDrop on target: " + target);
            return;
        }
        switch (actionType) {
            case (ContextMenuEntryType.UI_TAKE):
                {
                    targetInventory.TryTakeItem (tryItem, targetInventory.mainDragTarget);
                    break;
                }

            case (ContextMenuEntryType.UI_DROP):
                {
                    if (targetInventory != null) {
                        targetInventory.TryTakeItem (tryItem, targetInventory.mainDragTarget);
                    }
                    break;
                }
            case (ContextMenuEntryType.UI_USE):
                {
                    Debug.Log ("Attempting to consume item!");
                    int consumeSelf = tryItem.targetBox.ConsumeSelf ();
                    ItemData itemData = tryItem.targetBox.data;
                    if (consumeSelf > 0) {
                        if (tryItem.targetBox.StackSize <= 0) {
                            parentController.DestroyBox (tryItem);
                        }
                        itemConsumedEvent.Invoke (itemData, consumeSelf);
                    } else if (itemData.m_minimumNeededToConsume <= 0) { // consumable forever
                        itemConsumedEvent.Invoke (itemData, itemData.m_minimumNeededToConsume);
                    }
                    break;
                }
            case (ContextMenuEntryType.UI_LOOK):
                {
                    Debug.Log ("Attempting to look at item!");
                    break;
                }
        }
    }

    // Update is called once per frame
    void Update () {

    }
}