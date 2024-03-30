using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopItem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI itemTextMesh;
    [SerializeField] private Image itemImage;

    public ItemData item4Sale;
    public int maxQuantity;

    private void OnEnable()
    {
        UpdateShopItem();
    }

    public void UpdateShopItem()
    {
        int haveQuantity = InventoryBox.Instance.CheckInventory($"{item4Sale.id}").quantity;
        if (maxQuantity > haveQuantity)
        {
            itemTextMesh.text = $"<color=white>{item4Sale.itemName}</color>\n" +
                                $"<color=green>Price: {item4Sale.sellingPrice}</color>\n" +
                                $"<color=white>Have: {haveQuantity}";
        }
        else
        {
            itemTextMesh.text = $"<color=white>{item4Sale.itemName}</color>\n" +
                                $"<color=red>Out of Stock</color>\n" +
                                $"<color=white>Have: {haveQuantity}";
        }
        itemImage.sprite = item4Sale.sprite;
    }
}
