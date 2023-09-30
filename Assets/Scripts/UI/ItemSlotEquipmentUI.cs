using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSlotEquipmentUI : ItemSlotUI {
    public EquipmentType equipmentSlotType;

    private void OnValidate() {
        gameObject.name = "Equipment - " + equipmentSlotType.ToString();
    }
}