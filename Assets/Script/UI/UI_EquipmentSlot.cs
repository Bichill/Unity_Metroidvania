using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_EquipmentSlot : UI_ItemSlot
{
    public EquipmentType slotType;

    private void OnValidate()
    {
        gameObject.name = "Equipment slot - " + slotType.ToString();
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        if(eventData == null)
            return;
        
        // �z���Ƿ����b�����ж��
        if (item == null || item.data == null)
            return;
            
        // ж���b��K��ӵ�����
        ItemData_Equipment equipmentToUnequip = item.data as ItemData_Equipment;
        if (equipmentToUnequip != null)
        {
            Inventory.instance.UnequipItem(equipmentToUnequip);
            Inventory.instance.AddItem(equipmentToUnequip);
        }

        ui.itemToolTip.HideToolTip();

        CleanUpSlot();
    }
}
