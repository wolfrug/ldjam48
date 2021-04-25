using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_ItemBox : MonoBehaviour {
    public ItemData data;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI descriptionText;
    public TextMeshProUGUI amountText;
    public Image itemImage;
    public GenericTooltip tooltip;
    public Item_DragTarget dragTarget;
    public Animator animator;
    private int m_stackAmount = 1;
    public bool draggable = true;
    public bool consumable = true;
    public bool highlight = true;

    public bool stackable = true;
    // Start is called before the first frame update

    void Awake () {
        if (dragTarget == null) {
            dragTarget = GetComponent<Item_DragTarget> ();
        }
        if (tooltip == null) {
            tooltip = GetComponent<GenericTooltip> ();
        }
        if (animator == null) {
            animator = GetComponent<Animator> ();
        }
    }
    void Start () {
        UpdateItemBox ();
        if (highlight) {
            dragTarget.pointerEnter.AddListener (Highlight);
            dragTarget.pointerExit.AddListener (Dehighlight);
        }
    }

    public void SetItemBoxData (ItemData newData) {
        data = newData;
        UpdateItemBox ();
    }

    [NaughtyAttributes.Button]
    void UpdateItemBox () {
        if (data != null) {
            descriptionText.text = string.Format (data.m_descriptionTextFormat, data.m_description, data.m_displayName, StackSize, data.m_maxStackSize);
            itemImage.sprite = data.m_image;
            nameText.text = data.m_displayName;
            ParseItemData ();
            highlight = true;
        } else {
            itemImage.sprite = null;
            descriptionText.text = "";
            nameText.text = "";
            tooltip.IsActive = false;
            SetDraggable (false);
            highlight = false;
            amountText.text = "";
        }
    }

    public void SetDraggable (bool canDrag) {
        draggable = canDrag;
        if (animator != null) {
            animator.SetBool ("Draggable", canDrag);
        }
    }
    public int StackSize {
        get {
            return m_stackAmount;
        }
        set {
            m_stackAmount = value;
            amountText.text = string.Format (data.m_stackNumberFormat, value, data.m_maxStackSize);
            descriptionText.text = string.Format (data.m_descriptionTextFormat, data.m_description, data.m_displayName, StackSize, data.m_maxStackSize);
        }
    }

    public int ConsumeSelf () { // According to data - otherwise one can just use StackSize directly
        int randomConsumeAmount = data.ConsumeAmount ();
        int stackSize = StackSize;
        int minimumToConsume = data.m_minimumNeededToConsume;
        int returnValue = 0;
        if (randomConsumeAmount < minimumToConsume || stackSize < minimumToConsume) {
            returnValue = 0;
        } else {
            if (randomConsumeAmount > StackSize) { // consumed it all - go down to zero
                returnValue = StackSize;
                StackSize = 0;
            } else {
                returnValue = randomConsumeAmount;
                StackSize -= randomConsumeAmount;
            }
        }
        return returnValue;
    }

    public void Highlight (PointerEventData coll) {
        if (animator != null && highlight) {
            animator.SetBool ("Highlighted", true);
        }
    }
    public void Dehighlight (PointerEventData coll) {
        if (animator != null) {
            animator.SetBool ("Highlighted", false);
        }
    }

    void ParseItemData () { // parses the item data and sets stuff accordingly
        tooltip.IsActive = false;
        SetDraggable (false);
        consumable = false;
        foreach (ItemTrait trait in data.m_traits) {
            switch (trait) {
                case ItemTrait.HAS_TOOLTIP:
                    {
                        tooltip.IsActive = true;
                        break;
                    }
                case ItemTrait.DRAGGABLE:
                    {
                        SetDraggable (true);
                        break;
                    }
                case ItemTrait.CONSUMABLE:
                    {
                        consumable = true;
                        break;
                    }
            }
        }
    }

    // Update is called once per frame
    void Update () {

    }
}