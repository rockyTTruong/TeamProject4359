using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShopMenu : MonoBehaviour
{
    private QuickSlotManager qms;
    public ItemData[] itemsForSale;

    public void ClickBuy(Button button)
    {
        ItemData Item = button.gameObject.GetComponent<ShopItem>().item4Sale;
        qms = FindObjectOfType<QuickSlotManager>();

        if (CanBuy(Item))
        {
            InventoryBox.Instance.AddItem(Item.id.ToString(),1);
            InventoryBox.Instance.RemoveItem("9999", Item.sellingPrice);
            qms.UpdateUI();
            CoinManager.Instance.UpdateUI();
        }
    }

    private bool CanBuy(ItemData Item)
    {
        if (Item.sellingPrice > InventoryBox.Instance.CheckInventory("9999").quantity)
        {
            return false;
        }

        return true;
    }
    
}
