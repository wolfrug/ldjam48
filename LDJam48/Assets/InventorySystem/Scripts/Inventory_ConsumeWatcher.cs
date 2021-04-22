using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class EngineStarted : UnityEvent<Inventory_ConsumeWatcher> { } // engine + item box being burned

[System.Serializable]
public class EngineStopped : UnityEvent<Inventory_ConsumeWatcher> { } // engine

[System.Serializable]
public class EngineRunning : UnityEvent<Inventory_ConsumeWatcher, Item_DragAndDrop> { } // engine running + item box being burned

[System.Serializable]
public class EngineFuelUnitDone : UnityEvent<Inventory_ConsumeWatcher, ItemData> { } // whenever a fuel unit is done (not necessarily stopping)

public class Inventory_ConsumeWatcher : MonoBehaviour {
    [Header ("Consumes any of the item(s) allowed as fuel, and sends events when it is turned on, off and while on.")]

    public InventoryController engineInventory;
    public ItemData[] fuelItems;
    [Tooltip ("Items with these traits can be used in lieu of specific itemDatas")]
    public ItemTrait[] fuelTypes;
    public List<Item_DragAndDrop> activeFuel = new List<Item_DragAndDrop> { };
    public Item_DragAndDrop currentlyBurningFuel;
    public float consumeInterval = 0.1f;
    [Tooltip ("Override the item-specific 'consumed per use' setting")]
    public Vector2Int overrideConsumeAmountPerTick = new Vector2Int (-1, -1);
    public bool destroyBoxOnFinish = true;
    public EngineStarted engineStartedEvent;
    public EngineStopped engineStoppedEvent;
    public EngineStopped engineFailedToStartEvent;
    public EngineRunning engineRunningEvent;
    public EngineFuelUnitDone engineFuelUnitDoneEvent;
    private bool m_active;
    private Coroutine engineLoop;
    // Start is called before the first frame update
    void Start () {
        //Invoke ("InitEngine", 0.1f); -> it should be initialized from the InventoryController
    }

    public void InitEngine () {
        if (engineInventory != null && (fuelItems.Length > 0 || fuelTypes.Length > 0)) {
            StartCoroutine (MainLoop ());
        } else {
            Debug.LogWarning ("Cannot start engine without supported fuel types or attached inventory!", gameObject);
        }
    }

    public bool Active {
        get {
            return m_active;
        }
        set {
            m_active = value;
            if (engineLoop == null && value) {
                UpdateActiveFuel ();
                if (activeFuel.Count > 0) {
                    engineStartedEvent.Invoke (this);
                    engineLoop = StartCoroutine (MainLoop ());
                } else { // cannot activate on empty fuel :(
                    m_active = false;
                    engineFailedToStartEvent.Invoke (this);
                }
            } else if (engineLoop != null && !value) {
                ResetFuelDraggable ();
                engineStoppedEvent.Invoke (this);
                StopCoroutine (engineLoop);
                engineLoop = null;
                currentlyBurningFuel = null;
            }
        }
    }

    IEnumerator MainLoop () {
        // The main 'loop' for burning fuel!

        while (Active) {
            if (currentlyBurningFuel != null) {
                engineRunningEvent.Invoke (this, currentlyBurningFuel);
                Consume (currentlyBurningFuel);
            } else { // Ran out of fuel or setting up for the next fuel source
                UpdateActiveFuel ();
                if (activeFuel.Count > 0) {
                    currentlyBurningFuel = activeFuel[0];
                    SetFuelUndraggable ();
                } else { // no more fuel, cancel!
                    Active = false;
                    yield break;
                }
            }
            yield return new WaitForSeconds (consumeInterval);
        };
    }

    void SetFuelUndraggable () { // sets the fuel to undraggable in case it is normally
        currentlyBurningFuel.targetBox.SetDraggable (false);
        currentlyBurningFuel.UpdateInteractability ();
    }
    void ResetFuelDraggable () { // resets the fuel to its default draggable state
        if (currentlyBurningFuel != null) {
            if (currentlyBurningFuel.targetBox.data.HasTrait (ItemTrait.DRAGGABLE)) {
                currentlyBurningFuel.targetBox.SetDraggable (true);
            }
            currentlyBurningFuel.UpdateInteractability ();
        };
    }

    void UpdateActiveFuel () {
        activeFuel.Clear ();
        foreach (ItemTrait trait in fuelTypes) {
            activeFuel.AddRange (engineInventory.ItemsByTrait (trait));
        }
        foreach (ItemData data in fuelItems) {
            activeFuel.AddRange (engineInventory.ItemsByData (data));
        }
    }

    void Consume (Item_DragAndDrop item, int amount = 0) { // set amount to > 0 to eat that amount right away
        ItemData data = item.targetBox.data;
        int consumeAmount = amount;
        if (consumeAmount < 1) {
            if (overrideConsumeAmountPerTick.x > -1 && overrideConsumeAmountPerTick.y > -1) { // note this also bypasses the 'minimum consume amount needed' option!
                consumeAmount = Random.Range (overrideConsumeAmountPerTick.x, overrideConsumeAmountPerTick.y);
            } else {
                consumeAmount = data.ConsumeAmount (true); // force consume, mwahaha
            }
        }
        if (consumeAmount <= 0) {
            Debug.LogWarning ("Attempting to consume 0 fuel in engine - are you sure this is what you want? " + data.m_id + ")", gameObject);
        }
        int stackLeft = item.targetBox.StackSize;
        if (stackLeft <= 0) {
            Debug.LogWarning ("Found 0 stack fuel item - this will cause stack overflow! Cancelling (" + data.m_id + ")", gameObject);
            return;
        }
        // CONSUME TIME!

        // We ate what was left in the box, lol
        if (consumeAmount >= stackLeft) {
            item.targetBox.StackSize = 0;
            consumeAmount -= stackLeft;
            if (destroyBoxOnFinish) {
                engineInventory.DestroyBox (item);
            };
            engineFuelUnitDoneEvent.Invoke (this, data);

            // Update active fuel and see if we have another piece of the -same- fuel
            List<Item_DragAndDrop> sameFuel = new List<Item_DragAndDrop> { };
            sameFuel.AddRange (engineInventory.ItemsByData (data));
            if (sameFuel.Count > 0) { // we do, continue consuming!
                currentlyBurningFuel = sameFuel[0];
                SetFuelUndraggable ();
                Consume (currentlyBurningFuel, consumeAmount);
            } else { // we do not -> back to main loop
                currentlyBurningFuel = null;
                return;
            }
        } else { // We ate part of the box
            item.targetBox.StackSize -= consumeAmount;
        }
    }

    // Update is called once per frame
    void Update () {

    }
}