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
        
        // 檢查是否有裝備可以卸下
        if (item == null || item.data == null)
            return;
            
        // 卸下裝備並添加到背包
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
