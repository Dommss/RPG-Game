using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;

public class InventoryManager : MonoBehaviour {
    public static InventoryManager Instance;

    public List<InventoryItem> inventory;
    public Dictionary<ItemData, InventoryItem> inventoryDictionary;

    public List<InventoryItem> stash;
    public Dictionary<ItemData, InventoryItem> stashDictionary;

    [Header("Inventory UI")]
    [SerializeField] private Transform inventorySlotParent;
    [SerializeField] private Transform stashSlotParent;
    private ItemSlotUI[] inventoryItemSlot;
    private ItemSlotUI[] stashItemSlot;

    private void Awake() {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start() {
        inventory = new List<InventoryItem>();
        inventoryDictionary = new Dictionary<ItemData, InventoryItem>();

        stash = new List<InventoryItem>();
        stashDictionary = new Dictionary<ItemData, InventoryItem>();

        inventoryItemSlot = inventorySlotParent.GetComponentsInChildren<ItemSlotUI>();
        stashItemSlot = stashSlotParent.GetComponentsInChildren<ItemSlotUI>();
    }

    private void UpdateSlotUI() {
        for (int i = 0; i < inventory.Count; i++) {
            inventoryItemSlot[i].UpdateSlot(inventory[i]);
        }

        for (int i = 0; stash.Count > i; i++) {
            stashItemSlot[i].UpdateSlot(stash[i]);
        }
    }

    public void AddItem(ItemData _item) {
        if (_item.itemType == ItemType.Equipment) {
            AddToInventory(_item);
        } else if (_item.itemType == ItemType.Material) {
            AddToStash(_item);
        }

        UpdateSlotUI();
    }

    private void AddToStash(ItemData _item) {
        if (stashDictionary.TryGetValue(_item, out InventoryItem value)) {
            value.AddStack();
        } else {
            InventoryItem newItem = new InventoryItem(_item);
            stash.Add(newItem);
            stashDictionary.Add(_item, newItem);
        }
    }

    private void AddToInventory(ItemData _item) {
        if (inventoryDictionary.TryGetValue(_item, out InventoryItem value)) {
            value.AddStack();
        } else {
            InventoryItem newItem = new InventoryItem(_item);
            inventory.Add(newItem);
            inventoryDictionary.Add(_item, newItem);
        }
    }

    public void RemoveItem(ItemData _item) {
        if (inventoryDictionary.TryGetValue(_item, out InventoryItem value)) {
            if (value.stackSize <= 1) {
                inventory.Remove(value);
                inventoryDictionary.Remove(_item);
            } else {
                value.RemoveStack();
            }
        }

        if (stashDictionary.TryGetValue(_item, out InventoryItem stashValue)) {
            if (stashValue.stackSize <= 1) {
                stash.Remove(stashValue);
                stashDictionary.Remove(_item);
            } else {
                stashValue.RemoveStack();
            }
        }

        UpdateSlotUI();
    }
}