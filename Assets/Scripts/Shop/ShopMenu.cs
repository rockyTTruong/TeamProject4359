using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShopMenu : MonoBehaviour
{
    [SerializeField] private AudioSource buySuccessAudio;
    [SerializeField] private AudioSource buyFailureAudio;

    private QuickSlotManager qms;

    public void ClickBuy(Button button)
    {
        ShopItem shopItem = button.gameObject.GetComponent<ShopItem>();
        ItemData itemData = shopItem.item4Sale;

        qms = FindObjectOfType<QuickSlotManager>();

        if (CanBuy(shopItem))
        {
            InventoryBox.Instance.AddItem(itemData.id.ToString(), 1);
            InventoryBox.Instance.RemoveItem("9999", itemData.sellingPrice);
            qms.UpdateUI();
            CoinManager.Instance.UpdateUI();
            shopItem.UpdateShopItem();
        }
    }

    private bool CanBuy(ShopItem shopItem)
    {
        if (shopItem.item4Sale.sellingPrice > InventoryBox.Instance.CheckInventory("9999").quantity ||
            shopItem.maxQuantity <= InventoryBox.Instance.CheckInventory($"{shopItem.item4Sale.id}").quantity)
        {
            buyFailureAudio.Play();
            return false;

        }

        buySuccessAudio.Play();
        return true;
    }
}
