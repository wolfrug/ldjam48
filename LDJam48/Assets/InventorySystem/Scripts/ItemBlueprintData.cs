using System.Collections;
using UnityEngine;

public enum ItemBlueprintType {
    NONE = 0000,
    ANY = 1000,
    TYPE_1 = 2000,
    TYPE_2 = 3000,
    TYPE_3 = 4000,

}

[System.Serializable]
public class BlueprintComponent {
    public ItemData data;
    public ItemTrait trait = ItemTrait.NONE;
    [Tooltip ("Set to 0 to leave the item untouched after crafting")]
    public int amount = 1;
}

[CreateAssetMenu (fileName = "Data", menuName = "Item Blueprint", order = 1)]
public class ItemBlueprintData : ScriptableObject {
    public string m_id;
    public string m_displayName = "Craft Item";

    public ItemBlueprintType m_type = ItemBlueprintType.ANY;
    [Header ("Add data for a specific data, or use another trait than NONE to allow any itemData with that trait (but preference for what is in data)")]
    public BlueprintComponent[] m_componentsNeeded;
    [Header ("The resulting itemData from the successful combination, as well as an optional stack amount (1 = is global default)")]
    public ItemData m_result;
    public int m_stackAmount = 1;

    public int CompatibleData (ItemData data) { // returns -1 if it is not compatible, otherwise return the needed amount
        foreach (BlueprintComponent component in m_componentsNeeded) {
            if (component.data == data) {
                return component.amount;
            }
            if (component.trait != ItemTrait.NONE) {
                if (data.HasTrait (component.trait)) {
                    return component.amount;
                }
            }
        }
        return -1;
    }

}