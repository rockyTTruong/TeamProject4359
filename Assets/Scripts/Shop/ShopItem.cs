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
        if (maxQuantity > InventoryBox.Instance.CheckInventory($"{item4Sale.id}").quantity)
        {
            itemTextMesh.text = $"{item4Sale.itemName}\n" +
                                $"<color=green>Price: {item4Sale.sellingPrice}</color>";
        }
        else
        {
            itemTextMesh.text = $"{item4Sale.itemName}\n" +
                                $"<color=red>Out of Stock</color>";
        }
        itemImage.sprite = item4Sale.sprite;
    }
}
