using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class CraftSlotUI : ItemSlotUI {

    private void OnEnable() {
        UpdateSlot(item);
    }

    public override void OnPointerDown(PointerEventData eventData) {
        ItemDataEquipment craftData = item.data as ItemDataEquipment;

        InventoryManager.Instance.CanCraft(craftData, craftData.craftingMats);
    }
}