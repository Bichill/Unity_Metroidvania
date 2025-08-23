using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 這是一個邏輯類，負責實現邏輯功�?
// 說明�?
// inventory（背包）和stash（倉庫），兩者都是存儲InventoryItem的集合，但是存儲的物品類型不同（裝備/材料�?
// 這樣設計可以讓玩家區分裝備和材料，在UI中也能方便顯�?
public class Inventory : MonoBehaviour
{
    // 單例模式，確保全局唯一訪問
    public static Inventory instance;

    public List<ItemData> startingItem;

    // 裝備相關，裝備邏�?
    public List<InventoryItem> equipment;
    public Dictionary<ItemData_Equipment, InventoryItem> equipmentDictionary;

    // 背包相關，存儲裝備類物品
    public List<InventoryItem> inventory;
    public Dictionary<ItemData, InventoryItem> inventoryDictionary;

    // 倉庫（stash），存儲材料類物�?
    public List<InventoryItem> stash;
    public Dictionary<ItemData,InventoryItem> stashDictionary;

    [Header("Inventory UI")]
    // inventorySlotParent：背包UI的父物體，用於管理背包格�?
    [SerializeField] private Transform inventorySlotParent;
    // stashSlotParent：倉庫UI的父物體，用於管理倉庫格子
    [SerializeField] private Transform stashSlotParent;
    // equipmentSlotParent: 裝備UI的父物體，用於管理裝備格�?
    [SerializeField] private Transform equipmentSlotParent;
    // statSlotParent: 角色狀態UI的父物體，用於管理角色狀態欄
    [SerializeField] private Transform statSlotParent;

    // inventoryItemSlot：背包UI格子的腳本數�?
    private UI_ItemSlot[] inventoryItemSlot;
    // stashItemSlot：倉庫UI格子的腳本數�?
    private UI_ItemSlot[] stashItemSlot;
    // equipmentItemSlot：裝備欄UI格子的腳本數�?
    private UI_EquipmentSlot[] equipmentItemSlot;
    // statItemSlot：角色狀態UI格子的腳本數�?
    private UI_StatSlot[] statItemSlot;

    [Header("Item Cooldown")]
    private float lastTimeUseFlask;
    private float lastTimeUseArmor;

    public float flaskCooldown;
    private float armorCooldown;
     
    private void Awake()
    {
        // 初始化單例，確保全局唯一
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // 初始化數據結�?
        inventory = new List<InventoryItem>();
        inventoryDictionary = new Dictionary<ItemData, InventoryItem>();

        stash = new List<InventoryItem>();
        stashDictionary = new Dictionary<ItemData, InventoryItem>();

        equipment = new List<InventoryItem>();
        equipmentDictionary = new Dictionary<ItemData_Equipment, InventoryItem>();

        // 獲取背包和倉庫UI格子的腳本數�?
        inventoryItemSlot = inventorySlotParent.GetComponentsInChildren<UI_ItemSlot>();
        stashItemSlot = stashSlotParent.GetComponentsInChildren<UI_ItemSlot>();
        equipmentItemSlot = equipmentSlotParent.GetComponentsInChildren<UI_EquipmentSlot>();
        statItemSlot = statSlotParent.GetComponentsInChildren<UI_StatSlot>();

        AddStartingItem();
    }

    private void AddStartingItem()
    {
        for (int i = 0; i < startingItem.Count; i++)
        {
            if (startingItem[i]!=null)
                AddItem(startingItem[i]);
        }
    }

    // 裝備專用
    public void EquipItem(ItemData _item)
    {
        ItemData_Equipment newEquipment = _item as ItemData_Equipment;
        InventoryItem newItem = new InventoryItem(_item);

        ItemData_Equipment itemByRepeated = null;

        // 檢查當前裝備欄是否有重複裝備類型，如果有則記錄下�?
        foreach (KeyValuePair<ItemData_Equipment, InventoryItem> item in equipmentDictionary)
        {
            if (item.Key.equipmentType == newEquipment.equipmentType)
                itemByRepeated = item.Key;
        }

        // 移除重複裝備，並放回背包
        if (itemByRepeated != null)
        {
            UnequipItem(itemByRepeated);
            AddItem(itemByRepeated);
        }

        equipment.Add(newItem);// 裝備新裝�?
        equipmentDictionary.Add(newEquipment, newItem);
        newEquipment.AddModifiers();// 添加裝備屬性加�?

        RemoveItem(_item);// 從背包移除已裝備的物�?

        UpdateSlotUI();
    }

    // 卸下裝備，移除裝�?
    public void UnequipItem(ItemData_Equipment _itemToRemove)
    {
        if (equipmentDictionary.TryGetValue(_itemToRemove, out InventoryItem value))
        {
            equipment.Remove(value);
            equipmentDictionary.Remove(_itemToRemove);
            _itemToRemove.RemoveModifiers(); // 移除裝備屬性加�?
        }
    }

    // 刷新UI顯示
    private void UpdateSlotUI()
    {
        // 刪除原有倉庫和背包UI
        for (int i = 0; i < inventoryItemSlot.Length; i++)
        {
            // 排除合成格子
            if (inventoryItemSlot[i] is UI_CraftSlot)
                continue;
            inventoryItemSlot[i].CleanUpSlot();
        }

        for (int i = 0; i < stashItemSlot.Length; i++)
        {
            // 排除合成格子
            if (stashItemSlot[i] is UI_CraftSlot)
                continue;
            stashItemSlot[i].CleanUpSlot();
        }

        for (int i = 0; i < equipmentItemSlot.Length; i++)
        {
            equipmentItemSlot[i].CleanUpSlot();
        }

        // 更新新的倉庫和背包UI
        for (int i = 0; i < inventory.Count; i++)
        {
            inventoryItemSlot[i].UpdateSlot(inventory[i]);
        }

        for (int i = 0; i < stash.Count; i++)
        {
            stashItemSlot[i].UpdateSlot(stash[i]);
        }

        // 確保裝備顯示在對應的裝備�?
        for (int i = 0; i < equipmentItemSlot.Length; i++)
        {
            foreach (KeyValuePair<ItemData_Equipment, InventoryItem> item in equipmentDictionary)
            {
                if (item.Key.equipmentType == equipmentItemSlot[i].slotType)
                {
                    equipmentItemSlot[i].UpdateSlot(item.Value);
                }
            }
        }

        UpdateStatsUI();
    }

    public void UpdateStatsUI()
    {
        for (int i = 0; i < statItemSlot.Length; i++)
        {
            statItemSlot[i].UpdateStatValueUI();
        }
    }

    // 添加物品，區分背包和倉庫
    public void AddItem(ItemData _item)
    {
        if(_item.itemType == ItemType.Equipment && CanAddItem())
            AddToInventory(_item); // 裝備放入背包
        else if (_item.itemType == ItemType.Material)
            AddToStash(_item);     // 材料放入倉庫
        
        UpdateSlotUI();
    }

    // ���Ӳ��ϵ��}��
    private void AddToStash(ItemData _item)
    {
        if (_item == null) return;

        if (stashDictionary.TryGetValue(_item, out InventoryItem value))
        {
            value.AddStack(); // ���ӶѯB����
        }
        else
        {
            InventoryItem newItem = new InventoryItem(_item);
            stash.Add(newItem);
            stashDictionary.Add(_item, newItem);
        }
    }

    // �����b�䵽����
    private void AddToInventory(ItemData _item)
    {
        if (_item == null) return;

        if (inventoryDictionary.TryGetValue(_item, out InventoryItem value))
        {
            value.AddStack(); // ���ӶѯB���� 
        }
        else
        {
            InventoryItem newItem = new InventoryItem(_item);
            inventory.Add(newItem);
            inventoryDictionary.Add(_item, newItem);
        }
    }

    // �Ƴ���Ʒ�������͂}�춼���Lԇ�Ƴ�
    public void RemoveItem(ItemData _item)
    {
        if (_item == null) return;

        // �ȏı����Ƴ�
        if (inventoryDictionary.TryGetValue(_item, out InventoryItem value))
        {
            if (value.stackSize <= 1)
            {
                inventory.Remove(value);
                inventoryDictionary.Remove(_item);
            }
            else
            {
                value.RemoveStack();
            }
        }
        // �ُĂ}���Ƴ�
        if (stashDictionary.TryGetValue(_item, out InventoryItem stashValue))
        {
            if (stashValue.stackSize <= 1)
            {
                stash.Remove(stashValue);
                stashDictionary.Remove(_item);
            }
            else
            {
                stashValue.RemoveStack();
            }
        }

        UpdateSlotUI();
    }

    public bool CanAddItem()
    {
        if (inventory.Count >= inventoryItemSlot.Length)
        {
            return false;
        }

        return true;
    }


    // 檢查要合成的裝備，檢查材料表
    public bool CanCraft(ItemData_Equipment _itemToCraft, List<InventoryItem> _requiredMaterials)
    {
        // 檢查輸入參數
        if (_itemToCraft == null)
        {
            Debug.LogError("ItemData_Equipment is null in CanCraft");
            return false;
        }

        if (_requiredMaterials == null)
        {
            Debug.LogError("Required materials list is null in CanCraft");
            return false;
        }

        List<InventoryItem> materialsToRemove = new List<InventoryItem>();

        for (int i = 0; i < _requiredMaterials.Count; i++)
        {
            if (_requiredMaterials[i] == null || _requiredMaterials[i].data == null)
            {
                Debug.LogError($"Required material at index {i} is null");
                return false;
            }

            if (stashDictionary.TryGetValue(_requiredMaterials[i].data, out InventoryItem stashValue))
            {
                // 檢查倉庫是否有足夠的材料
                if (stashValue.stackSize < _requiredMaterials[i].stackSize)
                {
                    Debug.Log("Crafting failed: Not enough materials in stash.");
                    return false; // 材料不足，無法合�?
                }
                else
                {
                    materialsToRemove.Add(stashValue); // 記錄需要移除的材料
                } 
            }
            else
            {
                Debug.Log("Crafting failed: Missing required materials.");
                return false; // 缺少材料，無法合�?
            }   
        }

        // 所有材料都滿足要求，進行合成
        for (int i = 0; i < materialsToRemove.Count; i++)
        {
            RemoveItem(materialsToRemove[i].data); // 從倉庫中移除材�?
        }

        // 將合成的裝備添加到背�?
        AddItem(_itemToCraft);
        Debug.Log("Crafting successful: " + _itemToCraft.name);

        return true; // 合成成功
    }

    public List<InventoryItem> GetStashList() => stash;// 返回倉庫
    public List<InventoryItem> GetInventoryList() => inventory;// 返回背包

    public ItemData_Equipment GetEquipment(EquipmentType _type)
    {
        ItemData_Equipment equipedItem = null;

        foreach (KeyValuePair<ItemData_Equipment, InventoryItem> item in equipmentDictionary)
        {
            if (item.Key.equipmentType == _type)
                equipedItem = item.Key;
        }

        return equipedItem;
    }

    public void UseFlask()
    {
        ItemData_Equipment currentFlask = GetEquipment(EquipmentType.Flask);

        if (currentFlask == null)
            return;

        if (Time.time > lastTimeUseFlask + flaskCooldown)
        {
            // ʩ��ˎˮЧ���K�M����s
            currentFlask.Effect(null);
            flaskCooldown = currentFlask.itemCooldown;
            lastTimeUseFlask = Time.time;
        }
        else
        {
            float remaining = Mathf.Max(0f, lastTimeUseFlask + flaskCooldown - Time.time);
            Debug.Log($"Flask on cooldown: {remaining:F1}s remaining");
        }
        
        // ����UI
        UpdateSlotUI();
    }

    public float GetFlaskCooldownRemaining()
    {
        return Mathf.Max(0f, lastTimeUseFlask + flaskCooldown - Time.time);
    }

    public bool CanUseArmor()
    {
        ItemData_Equipment currentArmor = GetEquipment(EquipmentType.Armor);

        if (Time.time > lastTimeUseArmor + armorCooldown)
        {
            armorCooldown = currentArmor.itemCooldown;
            lastTimeUseArmor = Time.time;
            return true;
        }

        Debug.Log("Armor on cooldown");
        return false;
    }

    public void Us1eFlask()
    {
        ItemData_Equipment currentFlask = GetEquipment(EquipmentType.Flask);

        if (currentFlask == null)
            return;
        // 執行藥水效果
        currentFlask.Effect(null);

        // 使用後立即移除藥水（一次性使用）
        UnequipItem(currentFlask);

        // 更新UI顯示
        UpdateSlotUI();

        Debug.Log("Flask used and consumed: " + currentFlask.itemName);
    }
}
