using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[System.Serializable]
public class StackManipulatorCombined : UnityEvent<Item_DragAndDrop, Item_DragAndDrop> { }

[System.Serializable]
public class StackManipulatorSplit : UnityEvent<Item_DragAndDrop, int> { }

public class Inventory_StackManipulator : MonoBehaviour {

    public CanvasGroup canvasGroup;
    public Slider slider;
    public Button okButton;
    public TextMeshProUGUI amountText;
    public bool startAtMax = true;

    [Header ("{0} current value, {1} max value")]
    public string valueFormat = "0 ({0}) {1}";
    private Item_DragAndDrop currentItem;
    private Item_DragAndDrop combinerItem;
    public StackManipulatorCombined combineFinishedEvent;
    public StackManipulatorSplit splitFinishedEvent;

    private bool m_active = false;

    // Start is called before the first frame update
    void Start () {
        okButton.onClick.AddListener (StopManipulator);
        SetVisible ();
    }

    public bool Active {
        get {
            return m_active;
        }
        set {
            m_active = value;
            SetVisible ();
        }
    }

    public void StartManipulator (Item_DragAndDrop targetBox, Item_DragAndDrop othercombinerItem) {
        currentItem = targetBox;
        combinerItem = othercombinerItem;
        if (CheckCompatibility (currentItem)) {
            if (currentItem.targetBox.StackSize == 1 && combinerItem != null) { // dropping a 1 always auto-combines
                slider.value = 1;
                StopManipulator ();
                return;
            }
            if (combinerItem == null) { // we're splitting
                slider.maxValue = currentItem.targetBox.StackSize;
            } else {
                int roomLeft = combinerItem.targetBox.data.m_maxStackSize - combinerItem.targetBox.StackSize;
                slider.maxValue = (float) Mathf.Clamp (roomLeft, 1, targetBox.targetBox.StackSize);
            }
            if (startAtMax) {
                slider.value = slider.maxValue;
            } else {
                slider.value = 1;
            };
            Active = true;
        };
    }

    public bool CheckCompatibility (Item_DragAndDrop item) { // check here stuff like "cannot stack" etc
        if (!item.targetBox.data.HasTrait (ItemTrait.STACKABLE)) { // main is not stackable -> return false
            return false;
        }
        if (combinerItem == null) { // trying to split
            if (!item.targetBox.data.HasTrait (ItemTrait.SPLITTABLE)) { // main is not splittable -> return false
                return false;
            }
        }
        // Otherwise we return true, yay!
        return true;
    }

    public void StopManipulator () {

        // if we left at 0, just skip it
        if (slider.value == 0) {
            Debug.Log ("Manipulator done (failed: value at 0)");
            combineFinishedEvent.Invoke (currentItem, combinerItem);
        } else if (combinerItem != null && combinerItem.targetBox.data == currentItem.targetBox.data) { // same item data type
            Combine (currentItem, combinerItem, (int) slider.value);
            // Now we invoke, and let god determine what happens to the poor, possibly empty, items
            combineFinishedEvent.Invoke (currentItem, combinerItem);
        } else { // Combineritem is null, which means we just split by removing from the given item and invoke
            Debug.Log ("Manipulator done (failed: combineritem null or data incompatible (somehow)");
            currentItem.targetBox.StackSize -= (int) slider.value;
            splitFinishedEvent.Invoke (currentItem, (int) slider.value);
        }
        currentItem = null;
        Active = false;
    }

    public void Combine (Item_DragAndDrop one, Item_DragAndDrop two, int amount) {

        //How much can we add to two?
        int roomLeft = two.targetBox.data.m_maxStackSize - two.targetBox.StackSize;
        if (roomLeft >= amount) { // there's ample room, no worries
            one.targetBox.StackSize -= amount;
            two.targetBox.StackSize += amount;
        } else if (amount > roomLeft) {
            two.targetBox.StackSize += roomLeft;
            one.targetBox.StackSize -= roomLeft;
        }
    }

    void SetVisible () { // pretty code here for active/deactivate
        canvasGroup.interactable = Active;
        canvasGroup.blocksRaycasts = Active;
        canvasGroup.alpha = Active ? 1f : 0f;
    }

    // Update is called once per frame
    void Update () {
        if (Active && currentItem != null) {
            amountText.text = string.Format (valueFormat, slider.value, slider.maxValue);
        }
    }
}