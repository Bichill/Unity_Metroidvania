using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UI_CraftWindow : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI itemName;//�b�����Q
    [SerializeField] private TextMeshProUGUI itemDescription;//�b������
    [SerializeField] private Image itemIcon;//�b��icon
    [SerializeField] private Button craftButton;//�ϳɰ��o

    [SerializeField] private Image[] materialImage;//���ψDƬ

    [Header("Equipment Quality")]
    [SerializeField] private TextMeshProUGUI itemQualityText;
    private Color itemQualityColor;

    public void SetupCraftWindow(ItemData_Equipment _data)
    {
        craftButton.onClick.RemoveAllListeners();

        //��ʼ���b��ĈDƬ�����Q������
        itemIcon.sprite = _data.icon;
        itemName.text = _data.itemName;
        itemDescription.text = _data.GetDescription();

        UesEquipmentQualityColor(_data);

        //Ȼ���ʼ���b�����
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
            itemQualityText.text = "Ʒ�ʣ���ͨ";
            itemQualityColor = Color.white;
        }
        else if (_item.equipmentQuality == EquipmentQuality.excellent)
        {
            itemQualityText.text = "Ʒ�ʣ�����";
            itemQualityColor = Color.green;
        }
        else if (_item.equipmentQuality == EquipmentQuality.Sophisticated)
        {
            itemQualityText.text = "Ʒ�ʣ�����";
            itemQualityColor = Color.blue;
        }
        else if (_item.equipmentQuality == EquipmentQuality.epic)
        {
            itemQualityText.text = "Ʒ�ʣ�ʷʫ";
            itemQualityColor = Color.magenta;
        }
        else if (_item.equipmentQuality == EquipmentQuality.legend)
        {
            itemQualityText.text = "Ʒ�ʣ���˵";
            itemQualityColor = new Color(255, 223, 0);
        }
        else if (_item.equipmentQuality == EquipmentQuality.evilSpirits)
        {
            itemQualityText.text = "Ʒ�ʣ�а��";
            itemQualityColor = Color.red;
        }

        itemName.color = itemQualityColor;
        itemDescription.color = itemQualityColor;
        itemQualityText.color = itemQualityColor;
    }
}
