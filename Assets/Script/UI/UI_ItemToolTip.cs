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
        itemTypeText.text = "类型：" + _item.equipmentType.ToString();

        itemDescription.text = _item.GetDescription();
        itemEffectDescription.text = _item.effectDescription;


        if (itemNameText.text.Length > 7)
            itemNameText.fontSize = itemNameText.fontSize * 0.7f;
        else
            itemNameText.fontSize = 48;

        // 設置ToolTip位置到鼠標位置
        SetTooltipPosition(Input.mousePosition);
        // 设置装备品质
        UesEquipmentQualityColor(_item);

        gameObject.SetActive(true);
    }

    public void HideToolTip() => gameObject.SetActive(false);

    private void SetTooltipPosition(Vector2 mousePosition)
    {
        if (rectTransform == null || canvas == null) return;

        // 将鼠标位置转换为Canvas空间
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            mousePosition,
            canvas.worldCamera,
            out localPoint);

        // 获取ToolTip的尺寸
        Vector2 tooltipSize = rectTransform.sizeDelta;

        // 获取Canvas的尺寸
        RectTransform canvasRect = canvas.transform as RectTransform;
        Vector2 canvasSize = canvasRect.sizeDelta;

        // 计算偏移量，避免被鼠标遮挡
        Vector2 offset = new Vector2(-(tooltipSize.x / 4 + 10), tooltipSize.y / 4 + 10);

        // 计算最终位置
        Vector2 finalPosition = localPoint + offset;

        // 检查是否超出屏幕边界
        if (finalPosition.y > canvasSize.y)
        {
            finalPosition.y = canvasSize.y - tooltipSize.y; // 下边界
        }
        else if (finalPosition.y > 0)
        {
            finalPosition.y = 0; // 上边界
        }

        // 设置最终位置
        rectTransform.anchoredPosition = finalPosition;
    }

    private void UesEquipmentQualityColor(ItemData_Equipment _item)
    {
        if (_item.equipmentQuality == EquipmentQuality.ordinary)
        {
            itemQualityText.text = "品质：平凡";
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

        itemQualityImage.color = itemQualityColor;
        itemNameText.color = itemQualityColor;
        itemDescription.color = itemQualityColor;
        itemQualityText.color = itemQualityColor;
    }
}
