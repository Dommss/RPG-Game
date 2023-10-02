using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;

public class InventoryManager : MonoBehaviour {
    public static InventoryManager Instance;

    public List<ItemData> startingEquipment;

    public List<InventoryItem> inventory;
    public Dictionary<ItemData, InventoryItem> inventoryDictionary;

    public List<InventoryItem> stash;
    public Dictionary<ItemData, InventoryItem> stashDictionary;

    public List<InventoryItem> equipment;
    public Dictionary<ItemDataEquipment, InventoryItem> equipmentDictionary;

    [Header("Inventory UI")]
    [SerializeField] private Transform inventorySlotParent;
    [SerializeField] private Transform stashSlotParent;
    [SerializeField] private Transform equipmentSlotParent;
    private ItemSlotUI[] inventoryItemSlot;
    private ItemSlotUI[] stashItemSlot;
    private ItemSlotEquipmentUI[] equipmentItemSlot;

    private float lastTimeUsedFlask;
    private float lastTimeUsedArmor;

    private void Awake() {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start() {
        inventory = new List<InventoryItem>();
        inventoryDictionary = new Dictionary<ItemData, InventoryItem>();

        stash = new List<InventoryItem>();
        stashDictionary = new Dictionary<ItemData, InventoryItem>();

        equipment = new List<InventoryItem>();
        equipmentDictionary = new Dictionary<ItemDataEquipment, InventoryItem>();

        inventoryItemSlot = inventorySlotParent.GetComponentsInChildren<ItemSlotUI>();
        stashItemSlot = stashSlotParent.GetComponentsInChildren<ItemSlotUI>();
        equipmentItemSlot = equipmentSlotParent.GetComponentsInChildren<ItemSlotEquipmentUI>();
        AddStartingItems();
    }

    private void AddStartingItems() {
        for (int i = 0; i < startingEquipment.Count; i++) {
            AddItem(startingEquipment[i]);
        }
    }

    public void EquipItem(ItemData _item) {
        ItemDataEquipment newEquipment = _item as ItemDataEquipment;
        InventoryItem newItem = new InventoryItem(newEquipment);

        ItemDataEquipment oldEquipment = null;

        foreach (KeyValuePair<ItemDataEquipment, InventoryItem> item in equipmentDictionary) {
            if (item.Key.equipmentType == newEquipment.equipmentType) {
                oldEquipment = item.Key;
            }
        }

        if (oldEquipment != null) {
            UnequipItem(oldEquipment);
            AddItem(oldEquipment);
        }

        equipment.Add(newItem);
        equipmentDictionary.Add(newEquipment, newItem);
        newEquipment.AddModifiers();

        RemoveItem(_item);

        UpdateSlotUI();
    }

    public void UnequipItem(ItemDataEquipment itemToRemove) {
        if (equipmentDictionary.TryGetValue(itemToRemove, out InventoryItem value)) {
            equipment.Remove(value);
            equipmentDictionary.Remove(itemToRemove);
            itemToRemove.RemoveModifiers();
        }
    }

    private void UpdateSlotUI() {
        for (int i = 0; i < equipmentItemSlot.Length; i++) {
            foreach (KeyValuePair<ItemDataEquipment, InventoryItem> item in equipmentDictionary) {
                if (item.Key.equipmentType == equipmentItemSlot[i].equipmentSlotType) {
                    equipmentItemSlot[i].UpdateSlot(item.Value);
                }
            }
        }

        for (int i = 0; i < inventoryItemSlot.Length; i++) {
            inventoryItemSlot[i].CleanupSlot();
        }

        for (int i = 0; i < stashItemSlot.Length; i++) {
            stashItemSlot[i].CleanupSlot();
        }

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

    public bool CanCraft(ItemDataEquipment _itemToCraft, List<InventoryItem> _craftingMats) {
        List<InventoryItem> materialsToRemove = new List<InventoryItem>();

        for (int i = 0; i < _craftingMats.Count; i++) {
            if (stashDictionary.TryGetValue(_craftingMats[i].data, out InventoryItem stashValue)) {
                if (stashValue.stackSize < _craftingMats[i].stackSize) {
                    Debug.Log("Not enough materials!");
                    return false;
                } else {
                    materialsToRemove.Add(stashValue);
                }
            } else {
                Debug.Log("Not enough materials!");
                return false;
            }
        }

        for (int i = 0; i < materialsToRemove.Count; i++) {
            RemoveItem(materialsToRemove[i].data);
        }

        AddItem(_itemToCraft);
        Debug.Log("Here is your crafted item: " + _itemToCraft.name);

        return true;
    }

    public List<InventoryItem> GetEquipmentList() => equipment;

    public List<InventoryItem> GetStashList() => stash;

    public ItemDataEquipment GetEquipment(EquipmentType _type) {
        ItemDataEquipment equippedItem = null;

        foreach (KeyValuePair<ItemDataEquipment, InventoryItem> item in equipmentDictionary) {
            if (item.Key.equipmentType == _type) {
                equippedItem = item.Key;
            }
        }

        return equippedItem;
    }

    public void UseFlask() {
        ItemDataEquipment currentFlask = GetEquipment(EquipmentType.Flask);

        if (currentFlask == null) return;

        bool canUseFlask = Time.time > lastTimeUsedFlask + currentFlask.itemCooldown;

        if (canUseFlask) {
            currentFlask.ExecuteItemEffect(null);
            lastTimeUsedFlask = Time.time;
        } else {
            Debug.Log("Flask on cooldown!");
        }
    }

    public bool CanUseArmor() {
        ItemDataEquipment currentArmor = GetEquipment(EquipmentType.Armor);

        if (Time.time > lastTimeUsedArmor + currentArmor.itemCooldown) {
            lastTimeUsedArmor = Time.time;
            return true;
        }

        Debug.Log("Armor on cooldown!");
        return false;
    }
}