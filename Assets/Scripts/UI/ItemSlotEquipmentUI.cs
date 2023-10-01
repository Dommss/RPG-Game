using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemSlotEquipmentUI : ItemSlotUI {
    public EquipmentType equipmentSlotType;

    private void OnValidate() {
        gameObject.name = "Equipment - " + equipmentSlotType.ToString();
    }

    public override void OnPointerDown(PointerEventData eventData) {
        InventoryManager.Instance.UnequipItem(item.data as ItemDataEquipment);
        InventoryManager.Instance.AddItem(item.data as ItemDataEquipment);
        CleanupSlot();
    }
}