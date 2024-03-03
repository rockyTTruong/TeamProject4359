using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CoinManager : SingletonMonobehaviour<CoinManager>
{
    [SerializeField] private Image arrowImage;
    [SerializeField] private Image greenCoinImage;
    [SerializeField] private TextMeshProUGUI arrowQuantityTextMesh;
    [SerializeField] private TextMeshProUGUI greenCoinQuantityTextMesh;

    private InventorySlot greenCoinSlot;
    private InventorySlot arrowSlot;

    private void Start()
    {
        UpdateCoinInfo("");
        EventHandler.UseItemEvent += UpdateCoinInfo;
        EventHandler.PickUpItemEvent += UpdateCoinInfo;
    }

    private void OnApplicationQuit()
    {
        EventHandler.UseItemEvent -= UpdateCoinInfo;
        EventHandler.PickUpItemEvent -= UpdateCoinInfo;
    }

    public void UpdateUI()
    {
        if (greenCoinSlot != null)
        {
            greenCoinQuantityTextMesh.text = $"{greenCoinSlot.quantity}";
        }
        else
        {
            greenCoinQuantityTextMesh.text = "0";
        }

        if (arrowSlot != null)
        {
            arrowQuantityTextMesh.text = $"{arrowSlot.quantity}";
        }
        else
        {
            arrowQuantityTextMesh.text = "0";
        }
    }

    public void UpdateCoinInfo(string itemGuid)
    {
        greenCoinSlot = InventoryBox.Instance.CheckInventory("9999");
        arrowSlot = InventoryBox.Instance.CheckInventory("5003");
        UpdateUI();
    }
}
