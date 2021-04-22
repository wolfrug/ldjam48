using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class Player_Consume_Action {
    public ItemData itemType;
    public ItemConsumed action;
}

[System.Serializable]
public class Inventory_Manipulated_Action {
    public InventoryData inventoryData;
    public InventoryType inventoryType = InventoryType.NONE;
    public ItemData itemType;
    public InventoryOpened inventoryOpenedEvent;
    public InventoryClosed inventoryClosedEvent;
    public InventoryItemAdded itemAddedEvent;
    public InventoryItemRemoved itemRemovedEvent;
    public ItemCrafted itemCraftedEvent;
}

public class Inventory_ActionListener : MonoBehaviour // Use to listen to various inventory actions and react accordingly, e.g. for animations or the like (cross scene also)
{
    public InventoryController playerInventory;
    public InventoryController actionBarInventory;
    public Player_Consume_Action[] defaultPlayerConsumeActions;
    public Inventory_Manipulated_Action[] defaultInventoryManipulatedActions;
    // Start is called before the first frame update
    IEnumerator Start () {
        yield return new WaitForSeconds (0.5f); // to make sure all is initialized
        if (playerInventory == null) {
            playerInventory = InventoryController.GetInventoryOfType (InventoryType.PLAYER, null, false);
        }
        if (actionBarInventory == null) {
            actionBarInventory = InventoryController.GetInventoryOfType (InventoryType.ACTIONBAR, null, false);
        }

        playerInventory.contextMenuController.itemConsumedEvent.AddListener (ItemConsumed);
        actionBarInventory.contextMenuController.itemConsumedEvent.AddListener (ItemConsumed);
        foreach (InventoryController controller in InventoryController.allInventories) {
            controller.inventoryOpenedEvent.AddListener (OnInventoryOpen);
            controller.inventoryClosedEvent.AddListener (OnInventoryClosed);
            controller.itemAddedEvent.AddListener (OnItemAdded);
            controller.itemRemovedEvent.AddListener (OnItemRemoved);
            if (controller.craftingController != null) {
                controller.craftingController.itemCraftedEvent.AddListener (OnItemCrafted);
            }
        }
    }

    void ItemConsumed (ItemData data, int amount) {
        foreach (Player_Consume_Action act in defaultPlayerConsumeActions) {
            if (act.itemType == data) {
                act.action.Invoke (data, amount);
            }
        }
    }

    void OnInventoryOpen (InventoryController controller) {
        foreach (Inventory_Manipulated_Action act in defaultInventoryManipulatedActions) {
            if (act.inventoryData == null) {
                if (act.inventoryType == controller.type) {
                    act.inventoryOpenedEvent.Invoke (controller);
                }
            } else {
                if (act.inventoryData == controller.data) {
                    act.inventoryOpenedEvent.Invoke (controller);
                }
            }
        }
    }

    void OnInventoryClosed (InventoryController controller) {
        foreach (Inventory_Manipulated_Action act in defaultInventoryManipulatedActions) {
            if (act.inventoryData == null) {
                if (act.inventoryType == controller.type) {
                    act.inventoryClosedEvent.Invoke (controller);
                }
            } else {
                if (act.inventoryData == controller.data) {
                    act.inventoryClosedEvent.Invoke (controller);
                }
            }
        }
    }

    void OnItemRemoved (InventoryController controller, Item_DragAndDrop item) {
        foreach (Inventory_Manipulated_Action act in defaultInventoryManipulatedActions) {
            if (act.inventoryData == null) {
                if (act.inventoryType == controller.type) {
                    if (act.itemType == null) {
                        act.itemRemovedEvent.Invoke (controller, item);
                    } else if (act.itemType == item.targetBox.data) {
                        act.itemRemovedEvent.Invoke (controller, item);
                    }
                }
            } else {
                if (act.inventoryData == controller.data) {
                    if (act.itemType == null) {
                        act.itemRemovedEvent.Invoke (controller, item);
                    } else if (act.itemType == item.targetBox.data) {
                        act.itemRemovedEvent.Invoke (controller, item);
                    }
                }
            }
        }
    }

    void OnItemAdded (InventoryController controller, Item_DragAndDrop item) {
        foreach (Inventory_Manipulated_Action act in defaultInventoryManipulatedActions) {
            if (act.inventoryData == null) {
                if (act.inventoryType == controller.type) {
                    if (act.itemType == null) {
                        act.itemAddedEvent.Invoke (controller, item);
                    } else if (act.itemType == item.targetBox.data) {
                        act.itemAddedEvent.Invoke (controller, item);
                    }
                }
            } else {
                if (act.inventoryData == controller.data) {
                    if (act.itemType == null) {
                        act.itemAddedEvent.Invoke (controller, item);
                    } else if (act.itemType == item.targetBox.data) {
                        act.itemAddedEvent.Invoke (controller, item);
                    }
                }
            }
        }
    }

    void OnItemCrafted (InventoryController controller, ItemBlueprintData data) {
        foreach (Inventory_Manipulated_Action act in defaultInventoryManipulatedActions) {
            if (act.inventoryData == null) {
                if (act.inventoryType == controller.type) {
                    if (act.itemType == null) {
                        act.itemCraftedEvent.Invoke (controller, data);
                    } else if (act.itemType == data.m_result) {
                        act.itemCraftedEvent.Invoke (controller, data);
                    }
                }
            } else {
                if (act.inventoryData == controller.data) {
                    if (act.itemType == null) {
                        act.itemCraftedEvent.Invoke (controller, data);
                    } else if (act.itemType == data.m_result) {
                        act.itemCraftedEvent.Invoke (controller, data);
                    }
                }
            }
        }
    }

    // Update is called once per frame
    void Update () {

    }
}