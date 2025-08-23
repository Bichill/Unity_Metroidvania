using TMPro;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_ItemSlot : MonoBehaviour,IPointerDownHandler,IPointerEnterHandler,IPointerExitHandler
{
    [SerializeField] protected Image itemImage;
    [SerializeField] protected TextMeshProUGUI itemText;

    public InventoryItem item;

    protected UI ui;

    protected virtual void Start()
    {
        ui = GetComponentInParent<UI>();
    }

    public virtual void UpdateSlot(InventoryItem _newItem)  
    {
        item = _newItem;

        itemImage.color = Color.white; // Reset color to white before updating the sprite

        if (item != null && item.data != null)
        {
            itemImage.sprite = item.data.icon;

            if (item.stackSize > 1)
            {
                itemText.text = item.stackSize.ToString();
            }
            else
            {
                itemText.text = "";
            }
        }
        else
        {
            itemImage.sprite = null;
            itemText.text = "";
        }
    }

    public virtual void CleanUpSlot()
    {
        item = null;

        itemImage.sprite = null;
        itemImage.color = Color.clear;
        itemText.text = "";
    }

    public virtual void OnPointerDown(PointerEventData eventData)
    {
        if (item == null || item.data == null)
            return;// �����Ʒ��գ��������κβ���

        if (Input.GetKey(KeyCode.LeftShift))
        {
            Inventory.instance.RemoveItem(item.data);
            return; 
        }

        if (item.data.itemType == ItemType.Equipment)
            Inventory.instance.EquipItem(item.data);
 
        ui.itemToolTip.HideToolTip();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (item == null || item.data == null) return;
        
        // �z���Ƿ���b�����
        if (item.data.itemType != ItemType.Equipment) return;
        
        // ��ȫ�D�Q�� ItemData_Equipment
        ItemData_Equipment equipment = item.data as ItemData_Equipment;
        if (equipment == null) return;
        
        ui.itemToolTip.ShowToolTip(equipment);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (item == null || item.data == null) return;
        ui.itemToolTip.HideToolTip();
    }
}
