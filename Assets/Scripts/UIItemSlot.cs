using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIItemSlot : MonoBehaviour {
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
}