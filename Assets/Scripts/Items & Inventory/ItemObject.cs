using UnityEngine;

public class ItemObject : MonoBehaviour {
    private SpriteRenderer sr;

    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private ItemData itemData;

    private void SetupVisuals() {
        if (itemData == null) return;

        GetComponent<SpriteRenderer>().sprite = itemData.icon;
        gameObject.name = "Item Object - " + itemData.name;
    }

    public void SetupItem(ItemData _itemData, Vector2 _velocity) {
        itemData = _itemData;
        rb.velocity = _velocity;

        SetupVisuals();
    }

    public void PickupItem() {
        InventoryManager.Instance.AddItem(itemData);
        Destroy(gameObject);
    }
}