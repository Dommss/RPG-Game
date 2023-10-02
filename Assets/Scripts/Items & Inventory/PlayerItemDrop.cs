using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerItemDrop : ItemDrop {
    [Header("Player Drops")]
    [SerializeField] private float chanceToLooseItems;
    [SerializeField] private float chanceToLooseMats;

    public override void GenerateDrop() {
        InventoryManager inventory = InventoryManager.Instance;

        List<InventoryItem> itemsToUnequip = new List<InventoryItem>();
        List<InventoryItem> materialsToLose = new List<InventoryItem>();

        foreach (InventoryItem item in inventory.GetEquipmentList()) {
            if (Random.Range(0, 100) <= chanceToLooseItems) {
                DropItem(item.data);
                itemsToUnequip.Add(item);
            }
        }

        for (int i = 0; i < itemsToUnequip.Count; i++) {
            inventory.UnequipItem(itemsToUnequip[i].data as ItemDataEquipment);
        }

        foreach (InventoryItem item in inventory.GetStashList()) {
            if (Random.Range(0, 100) <= chanceToLooseMats) {
                DropItem(item.data);
                materialsToLose.Add(item);
            }
        }

        for (int i = 0; i < materialsToLose.Count; i++) {
            inventory.RemoveItem(materialsToLose[i].data);
        }
    }
}