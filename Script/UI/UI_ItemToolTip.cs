using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_ItemToolTip : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI itemNameText;
    [SerializeField] private TextMeshProUGUI itemTypeText;
    [SerializeField] private TextMeshProUGUI itemDescription;
    [SerializeField] private TextMeshProUGUI itemEffectDescription;
    [SerializeField] private TextMeshProUGUI itemQualityText;
    [SerializeField] private Image itemQualityImage;
    private Color itemQualityColor;

    private RectTransform rectTransform;
    private Canvas canvas;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        itemQualityImage = GetComponent<Image>();
    }

    public void ShowToolTip(ItemData_Equipment _item)
    {
        if (_item == null)
            return;

        itemNameText.text = _item.itemName;
        itemTypeText.text = "���ͣ�" + _item.equipmentType.ToString();

        itemDescription.text = _item.GetDescription();
        itemEffectDescription.text = _item.effectDescription;


        if (itemNameText.text.Length > 7)
            itemNameText.fontSize = itemNameText.fontSize * 0.7f;
        else
            itemNameText.fontSize = 48;

        // �O��ToolTipλ�õ����λ��
        SetTooltipPosition(Input.mousePosition);
        // ����װ��Ʒ��
        UesEquipmentQualityColor(_item);

        gameObject.SetActive(true);
    }

    public void HideToolTip() => gameObject.SetActive(false);

    private void SetTooltipPosition(Vector2 mousePosition)
    {
        if (rectTransform == null || canvas == null) return;

        // �����λ��ת��ΪCanvas�ռ�
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            mousePosition,
            canvas.worldCamera,
            out localPoint);

        // ��ȡToolTip�ĳߴ�
        Vector2 tooltipSize = rectTransform.sizeDelta;

        // ��ȡCanvas�ĳߴ�
        RectTransform canvasRect = canvas.transform as RectTransform;
        Vector2 canvasSize = canvasRect.sizeDelta;

        // ����ƫ���������ⱻ����ڵ�
        Vector2 offset = new Vector2(-(tooltipSize.x / 4 + 10), tooltipSize.y / 4 + 10);

        // ��������λ��
        Vector2 finalPosition = localPoint + offset;

        // ����Ƿ񳬳���Ļ�߽�
        if (finalPosition.y > canvasSize.y)
        {
            finalPosition.y = canvasSize.y - tooltipSize.y; // �±߽�
        }
        else if (finalPosition.y > 0)
        {
            finalPosition.y = 0; // �ϱ߽�
        }

        // ��������λ��
        rectTransform.anchoredPosition = finalPosition;
    }

    private void UesEquipmentQualityColor(ItemData_Equipment _item)
    {
        if (_item.equipmentQuality == EquipmentQuality.ordinary)
        {
            itemQualityText.text = "Ʒ�ʣ�ƽ��";
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

        itemQualityImage.color = itemQualityColor;
        itemNameText.color = itemQualityColor;
        itemDescription.color = itemQualityColor;
        itemQualityText.color = itemQualityColor;
    }
}
