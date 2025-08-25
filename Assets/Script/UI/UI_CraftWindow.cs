using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UI_CraftWindow : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI itemName;//裝備名稱
    [SerializeField] private TextMeshProUGUI itemDescription;//裝備描述
    [SerializeField] private Image itemIcon;//裝備icon
    [SerializeField] private Button craftButton;//合成按鈕

    [SerializeField] private Image[] materialImage;//材料圖片

    [Header("Equipment Quality")]
    [SerializeField] private TextMeshProUGUI itemQualityText;
    private Color itemQualityColor;

    public void SetupCraftWindow(ItemData_Equipment _data)
    {
        craftButton.onClick.RemoveAllListeners();

        //初始化裝備的圖片、名稱、描述
        itemIcon.sprite = _data.icon;
        itemName.text = _data.itemName;
        itemDescription.text = _data.GetDescription();

        UesEquipmentQualityColor(_data);

        //然後初始化裝備材料
        for (int i = 0; i < materialImage.Length; i++)
        {
            materialImage[i].color = Color.clear;
            materialImage[i].GetComponentInChildren<TextMeshProUGUI>().color = Color.clear;
        }

        for (int i = 0; i < _data.craftingMaterials.Count; i++)
        {
            if (_data.craftingMaterials.Count > materialImage.Length)
                Debug.LogWarning("You have more material amount than you have material slots in craft window!");

            if (_data.craftingMaterials[i].data.icon == null)
                Debug.LogWarning("This Equipment material icon is Null!");

            materialImage[i].sprite = _data.craftingMaterials[i].data.icon;
            materialImage[i].color = Color.white;

            TextMeshProUGUI materialText = materialImage[i].GetComponentInChildren<TextMeshProUGUI>();

            materialText.text = _data.craftingMaterials[i].stackSize.ToString();
            materialText.color = Color.white;
        }

        craftButton.onClick.AddListener(() => Inventory.instance.CanCraft(_data, _data.craftingMaterials));
    }

    private void UesEquipmentQualityColor(ItemData_Equipment _item)
    {
        if (_item.equipmentQuality == EquipmentQuality.ordinary)
        {
            itemQualityText.text = "品质：普通";
            itemQualityColor = Color.white;
        }
        else if (_item.equipmentQuality == EquipmentQuality.excellent)
        {
            itemQualityText.text = "品质：优秀";
            itemQualityColor = Color.green;
        }
        else if (_item.equipmentQuality == EquipmentQuality.Sophisticated)
        {
            itemQualityText.text = "品质：精良";
            itemQualityColor = Color.blue;
        }
        else if (_item.equipmentQuality == EquipmentQuality.epic)
        {
            itemQualityText.text = "品质：史诗";
            itemQualityColor = Color.magenta;
        }
        else if (_item.equipmentQuality == EquipmentQuality.legend)
        {
            itemQualityText.text = "品质：传说";
            itemQualityColor = new Color(255, 223, 0);
        }
        else if (_item.equipmentQuality == EquipmentQuality.evilSpirits)
        {
            itemQualityText.text = "品质：邪灵";
            itemQualityColor = Color.red;
        }

        itemName.color = itemQualityColor;
        itemDescription.color = itemQualityColor;
        itemQualityText.color = itemQualityColor;
    }
}
