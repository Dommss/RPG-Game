using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemSlotUI : MonoBehaviour, IPointerDownHandler {
    [SerializeField] private Image itemImage;
    [SerializeField] private TextMeshProUGUI itemText;

    public InventoryItem item;

    public void UpdateSlot(InventoryItem _newItem) {
        item = _newItem;

        itemImage.color = Color.white;
        itemText.color = Color.white;

        if (item != null) {
            itemImage.sprite = item.data.icon;

            if (item.stackSize > 1) {
                itemText.text = item.stackSize.ToString();
            } else {
                itemText.text = "";
            }
        }
    }

    public void CleanupSlot() {
        item = null;
        itemImage.sprite = null;
        itemImage.color = Color.clear;
        itemText.text = "";
    }

    public virtual void OnPointerDown(PointerEventData eventData) {
        if (item.data.itemType == ItemType.Equipment) {
            InventoryManager.Instance.EquipItem(item.data);
        }
    }
}