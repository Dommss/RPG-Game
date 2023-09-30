using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour {
    public static InventoryManager Instance;

    public List<InventoryItem> inventoryItems;
    public Dictionary<ItemData, InventoryItem> inventoryDictionary;

    [Header("Inventory UI")]
    [SerializeField] private Transform inventorySlotParent;
    private UIItemSlot[] itemSlot;

    private void Awake() {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start() {
        inventoryItems = new List<InventoryItem>();
        inventoryDictionary = new Dictionary<ItemData, InventoryItem>();
        itemSlot = inventorySlotParent.GetComponentsInChildren<UIItemSlot>();
    }

    private void UpdateSlotUI() {
        for (int i = 0; i < inventoryItems.Count; i++) {
            itemSlot[i].UpdateSlot(inventoryItems[i]);
        }
    }

    public void AddItem(ItemData _item) {
        if(inventoryDictionary.TryGetValue(_item, out InventoryItem value)) {
            value.AddStack();
        } else {
            InventoryItem newItem = new InventoryItem(_item);
            inventoryItems.Add(newItem);
            inventoryDictionary.Add(_item, newItem);
        }

        UpdateSlotUI();
    }

    public void RemoveItem (ItemData _item) {
        if (inventoryDictionary.TryGetValue(_item,out InventoryItem value)) {
            if(value.stackSize <= 1) {
                inventoryItems.Remove(value);
                inventoryDictionary.Remove(_item);
            } else {
                value.RemoveStack();
            }
        }

        UpdateSlotUI();
    }
}