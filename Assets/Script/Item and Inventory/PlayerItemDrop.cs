using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.Progress;

public class PlayerItemDrop : ItemDrop
{
    [Header("Player's drop")]
    //控制死亡是否掉落装备
    public bool canDropEquipment;
    [SerializeField] private float chanceToLoseEquipments;

    //控制死亡是否掉落材料
    public bool canDropMaterials;
    [SerializeField] private float chanceToLoseMaterials;
    

    public override void GenerateDrop()
    {
        Inventory inventory = Inventory.instance;

        List<InventoryItem> equipmentsToLose = new List<InventoryItem>();
        List<InventoryItem> materialsToLose = new List<InventoryItem>();

        //如果可以掉落装备
        if (canDropEquipment)
        {
            //丢装备
            if (inventory.GetInventoryList() == null) return;

            foreach (InventoryItem item in inventory.GetInventoryList())
            {
                if (Random.Range(0, 100) <= chanceToLoseEquipments)
                {
                    DropItem(item.data);
                    equipmentsToLose.Add(item);
                }
            }

            foreach (InventoryItem item in equipmentsToLose)
            {
                inventory.RemoveItem(item.data);
            }
        }

        //如果可以掉落材料
        if (canDropMaterials)
        {
            //丢材料
            if (inventory.GetStashList() == null) return;

            foreach (InventoryItem item in inventory.GetStashList())
            {
                if (Random.Range(0, 100) <= chanceToLoseMaterials)
                {
                    DropItem(item.data);
                    materialsToLose.Add(item);
                }
            }

            foreach (InventoryItem item in materialsToLose)
            {
                inventory.RemoveItem(item.data);
            }
        }
    }
}
