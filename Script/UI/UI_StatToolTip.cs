using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_StatToolTip : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI description;

    private RectTransform rectTransform;
    private Canvas canvas;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
    }
        
    public void ShowStatToolTip(string _text)
    {
        description.text = _text;

        // 設置ToolTip位置到鼠標位置
        SetTooltipPosition(Input.mousePosition);

        gameObject.SetActive(true);
    }

    public void HideStatToolTip()
    {
        description.text = "";
        gameObject.SetActive(false);
    }

    private void SetTooltipPosition(Vector2 mousePosition)
    {
        if (rectTransform == null || canvas == null) return;

        // 將鼠標位置轉換為Canvas空間
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform, 
            mousePosition, 
            canvas.worldCamera, 
            out localPoint);

        // 獲取ToolTip的尺寸
        Vector2 tooltipSize = rectTransform.sizeDelta;

        // 計算偏移量，避免被鼠標遮擋
        Vector2 offset = new Vector2(0, -tooltipSize.y);
        
        // 直接設置位置，讓ToolTip跟隨滑鼠
        rectTransform.anchoredPosition = localPoint + offset;
    }
}
