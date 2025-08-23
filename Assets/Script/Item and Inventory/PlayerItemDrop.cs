using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.Progress;

public class PlayerItemDrop : ItemDrop
{
    [Header("Player's drop")]
    //���������Ƿ����װ��
    public bool canDropEquipment;
    [SerializeField] private float chanceToLoseEquipments;

    //���������Ƿ�������
    public bool canDropMaterials;
    [SerializeField] private float chanceToLoseMaterials;
    

    public override void GenerateDrop()
    {
        Inventory inventory = Inventory.instance;

        List<InventoryItem> equipmentsToLose = new List<InventoryItem>();
        List<InventoryItem> materialsToLose = new List<InventoryItem>();

        //������Ե���װ��
        if (canDropEquipment)
        {
            //��װ��
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

        //������Ե������
        if (canDropMaterials)
        {
            //������
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
