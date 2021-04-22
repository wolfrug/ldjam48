using System.Collections;
using UnityEngine;

public enum ItemTrait { // These work by positive connotation, i.e. if they don't have 'movable' then they are non-movable etc.
 NONE = 0000,
 SPLITTABLE = 1000,
 STACKABLE = 1100,
 CONSUMABLE = 3000,
 DRAGGABLE = 4000,
 HAS_TOOLTIP = 5000,
}

[CreateAssetMenu (fileName = "Data", menuName = "Inventory Item Data", order = 1)]
public class ItemData : ScriptableObject {
    public string m_id;
    public string m_displayName;
    [Multiline]
    public string m_description;

    [Header ("{0} = description text, {1} = displayName, {2} = current stack amount {3} = max stack amount")]
    [Multiline]
    public string m_descriptionTextFormat = "{1}\n{0}\n\n\n{2}/{3}";
    [Header ("{0} = current stack, {1} = max stack (might not fit very well)")]
    public string m_stackNumberFormat = "{0}";
    public Sprite m_image;
    public ItemTrait[] m_traits = { ItemTrait.STACKABLE, ItemTrait.SPLITTABLE, ItemTrait.DRAGGABLE, ItemTrait.HAS_TOOLTIP };
    public int m_maxStackSize = 1;
    public int m_sizeInInventory = 1;

    public Vector2Int m_amountConsumedPerUse = new Vector2Int (1, 1);
    public int m_minimumNeededToConsume = 0; // Make this bigger to make the consume not trigger if it's less.
    public GameObject m_prefab;

    public bool HasTrait (ItemTrait trait) { // Helper function
        foreach (ItemTrait t in m_traits) {
            if (t == trait) {
                return true;
            }
        }
        return false;
    }
    public int ConsumeAmount (bool forceConsume = false) {
        if (HasTrait (ItemTrait.CONSUMABLE) || forceConsume) {
            return Mathf.Clamp (Random.Range (m_amountConsumedPerUse.x, m_amountConsumedPerUse.y), m_minimumNeededToConsume, m_amountConsumedPerUse.y);
        } else {
            return 0;
        }
    }
}