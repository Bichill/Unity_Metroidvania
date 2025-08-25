using System.Collections.Generic;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEditor;
using UnityEngine;
using static UnityEditor.Progress;  

// 這是一個背包系統，負責管理背包功能
// 說明：
// inventory（背包）和stash（倉庫），兩者都是存放InventoryItem的集合，但是存放的物品類型不同（裝備/材料）
// 這樣設計可以讓玩家區分裝備和材料，在UI中也能方便顯示
public class Inventory : MonoBehaviour, ISaveManager
{
    // 單例模式，確保全局唯一實例
    public static Inventory instance;

    public List<ItemData> startingItem;

    // 裝備相關，存放裝備
    public List<InventoryItem> equipment;
    public Dictionary<ItemData_Equipment, InventoryItem> equipmentDictionary;

    // 背包相關，存放各種物品
    public List<InventoryItem> inventory;
    public Dictionary<ItemData, InventoryItem> inventoryDictionary;

    // 倉庫（stash），存放材料類物品
    public List<InventoryItem> stash;
    public Dictionary<ItemData, InventoryItem> stashDictionary;

    [Header("Inventory UI")]
    // inventorySlotParent：背包UI的父物件，用來管理背包格子
    [SerializeField] private Transform inventorySlotParent;
    // stashSlotParent：倉庫UI的父物件，用來管理倉庫格子
    [SerializeField] private Transform stashSlotParent;
    // equipmentSlotParent: 裝備UI的父物件，用來管理裝備格子
    [SerializeField] private Transform equipmentSlotParent;
    // statSlotParent: 角色屬性UI的父物件，用來管理屬性顯示
    [SerializeField] private Transform statSlotParent;

    // inventoryItemSlot：背包UI格子的引用
    private UI_ItemSlot[] inventoryItemSlot;
    // stashItemSlot：倉庫UI格子的引用
    private UI_ItemSlot[] stashItemSlot;
    // equipmentItemSlot：裝備UI格子的引用
    private UI_EquipmentSlot[] equipmentItemSlot;
    // statItemSlot：屬性UI格子的引用
    private UI_StatSlot[] statItemSlot;

    [Header("Item Cooldown")]
    private float lastTimeUseFlask;
    private float lastTimeUseArmor;

    public float flaskCooldown;
    private float armorCooldown;

    [Header("Data Base")]
    public List<InventoryItem> loadedItems;//读取的装备与材料列表
    public List<ItemData_Equipment> loadedEquipment;//读取已装备的列表
     
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
        // 初始化各種列表
        inventory = new List<InventoryItem>();
        inventoryDictionary = new Dictionary<ItemData, InventoryItem>();

        stash = new List<InventoryItem>();
        stashDictionary = new Dictionary<ItemData, InventoryItem>();

        equipment = new List<InventoryItem>();
        equipmentDictionary = new Dictionary<ItemData_Equipment, InventoryItem>();

        // 獲取背包和倉庫UI格子的引用
        inventoryItemSlot = inventorySlotParent.GetComponentsInChildren<UI_ItemSlot>();
        stashItemSlot = stashSlotParent.GetComponentsInChildren<UI_ItemSlot>();
        equipmentItemSlot = equipmentSlotParent.GetComponentsInChildren<UI_EquipmentSlot>();
        statItemSlot = statSlotParent.GetComponentsInChildren<UI_StatSlot>();

        AddStartingItem();
    }

    private void AddStartingItem()
    {
        // 如果有已经穿戴的装备，添加进入装备栏
        foreach (ItemData_Equipment item in loadedEquipment)
        {
            EquipItem(item);
        }

        // 如果有載入的物品，優先添加載入的物品
        if (loadedItems.Count > 0)
        {
            foreach (InventoryItem item in loadedItems)
            {
                for (int i = 0; i < item.stackSize; i++)
                {
                    AddItem(item.data);
                }
            }
            return; // 如果有載入的物品，就不添加初始物品
        }

        // 檢查是否已經給過初始裝備（需要先檢查 SaveManager 是否存在）
        if (SaveManager.instance != null && SaveManager.instance.GameData != null && SaveManager.instance.GameData.hasReceivedStartingItems)
        {
            return; // 已經給過初始裝備，不再重複給
        }

        // 只有在沒有載入物品且沒有給過初始裝備時，才添加初始裝備
        for (int i = 0; i < startingItem.Count; i++)
        {
            if (startingItem[i] != null)
                AddItem(startingItem[i]);
        }

        // 標記已經給過初始裝備
        if (SaveManager.instance != null && SaveManager.instance.GameData != null)
        {
            SaveManager.instance.GameData.hasReceivedStartingItems = true;
        }
    }

    // 裝備使用
    public void EquipItem(ItemData _item)
    {
        ItemData_Equipment newEquipment = _item as ItemData_Equipment;
        InventoryItem newItem = new InventoryItem(_item);

        ItemData_Equipment itemByRepeated = null;

        // 檢查當前裝備欄是否有重複裝備類型，如果有就卸下
        foreach (KeyValuePair<ItemData_Equipment, InventoryItem> item in equipmentDictionary)
        {
            if (item.Key.equipmentType == newEquipment.equipmentType)
                itemByRepeated = item.Key;
        }

        // 移除重複裝備，放回背包
        if (itemByRepeated != null)
        {
            UnequipItem(itemByRepeated);
            AddItem(itemByRepeated);
        }

        equipment.Add(newItem);// 裝備新物品
        equipmentDictionary.Add(newEquipment, newItem);
        newEquipment.AddModifiers();// 添加裝備屬性加成

        RemoveItem(_item);// 從背包移除已裝備的物品

        UpdateSlotUI();
    }

    // 卸下裝備，移除裝備
    public void UnequipItem(ItemData_Equipment _itemToRemove)
    {
        if (equipmentDictionary.TryGetValue(_itemToRemove, out InventoryItem value))
        {
            equipment.Remove(value);
            equipmentDictionary.Remove(_itemToRemove);
            _itemToRemove.RemoveModifiers(); // 移除裝備屬性加成
        }
    }

    // 刷新UI顯示
    private void UpdateSlotUI()
    {
        // 添加空值檢查，避免在初始化過程中調用時出錯
        if (inventoryItemSlot == null || stashItemSlot == null || equipmentItemSlot == null)
        {
            Debug.LogWarning("UI slots are not initialized yet, skipping UpdateSlotUI");
            return;
        }

        // 清除原有倉庫和背包UI
        for (int i = 0; i < inventoryItemSlot.Length; i++)
        {
            // 排除合成格子
            if (inventoryItemSlot[i] is UI_CraftSlot)
                continue;
            if (inventoryItemSlot[i] != null)
            {
            inventoryItemSlot[i].CleanUpSlot();
            }
        }

        for (int i = 0; i < stashItemSlot.Length; i++)
        {
            // 排除合成格子
            if (stashItemSlot[i] is UI_CraftSlot)
                continue;
            if (stashItemSlot[i] != null)
            {
            stashItemSlot[i].CleanUpSlot();
            }
        }

        for (int i = 0; i < equipmentItemSlot.Length; i++)
        {
            if (equipmentItemSlot[i] != null)
        {
            equipmentItemSlot[i].CleanUpSlot();
            }
        }

        // 更新新的倉庫和背包UI
        for (int i = 0; i < inventory.Count; i++)
        {
            if (i < inventoryItemSlot.Length && inventoryItemSlot[i] != null)
        {
            inventoryItemSlot[i].UpdateSlot(inventory[i]);
            }
        }

        for (int i = 0; i < stash.Count; i++)
        {
            if (i < stashItemSlot.Length && stashItemSlot[i] != null)
        {
            stashItemSlot[i].UpdateSlot(stash[i]);
            }
        }

        // 確保裝備顯示在對應的裝備欄
        for (int i = 0; i < equipmentItemSlot.Length; i++)
        {
            if (equipmentItemSlot[i] != null)
            {
                foreach (KeyValuePair<ItemData_Equipment, InventoryItem> item in equipmentDictionary)
                {
                if (item.Key.equipmentType == equipmentItemSlot[i].slotType)
                {
                    equipmentItemSlot[i].UpdateSlot(item.Value);
                    }
                }
            }
        }

        UpdateStatsUI();
    }

    public void UpdateStatsUI()
    {
        // 添加空值檢查，避免在初始化過程中調用時出錯
        if (statItemSlot == null || statItemSlot.Length == 0)
        {
            Debug.LogWarning("statItemSlot is not initialized yet, skipping UpdateStatsUI");
            return;
        }

        for (int i = 0; i < statItemSlot.Length; i++)
        {
            if (statItemSlot[i] != null)
        {
            statItemSlot[i].UpdateStatValueUI();
            }
        }
    }

    // 添加物品，區分背包和倉庫
    public void AddItem(ItemData _item)
    {
        if (_item.itemType == ItemType.Equipment && CanAddItem())
            AddToInventory(_item); // 裝備放入背包
        else if (_item.itemType == ItemType.Material)
            AddToStash(_item);     // 材料放入倉庫
        
        UpdateSlotUI();
    }

    // 添加材料到倉庫
    private void AddToStash(ItemData _item)
    {
        if (_item == null) return;

        if (stashDictionary.TryGetValue(_item, out InventoryItem value))
        {
            value.AddStack(); // 增加堆疊數量
        }
        else
        {
            InventoryItem newItem = new InventoryItem(_item);
            stash.Add(newItem);
            stashDictionary.Add(_item, newItem);
        }
    }

    // 添加裝備到背包
    private void AddToInventory(ItemData _item)
    {
        if (_item == null) return;

        if (inventoryDictionary.TryGetValue(_item, out InventoryItem value))
        {
            value.AddStack(); // 增加堆疊數量 
        }
        else
        {
            InventoryItem newItem = new InventoryItem(_item);
            inventory.Add(newItem);
            inventoryDictionary.Add(_item, newItem);
        }
    }

    // 移除物品，背包和倉庫都會嘗試移除
    public void RemoveItem(ItemData _item)
    {
        if (_item == null) return;

        // 先從背包移除
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
        // 再從倉庫移除
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
                    return false; // 材料不足，無法合成
                }
                else
                {
                    materialsToRemove.Add(stashValue); // 記錄需要移除的材料
                } 
            }
            else
            {
                Debug.Log("Crafting failed: Missing required materials.");
                return false; // 缺少材料，無法合成
            }   
        }

        // 所有材料都滿足要求，開始合成
        for (int i = 0; i < materialsToRemove.Count; i++)
        {
            RemoveItem(materialsToRemove[i].data); // 從倉庫中移除材料
        }

        // 將合成的裝備添加到背包
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
            // 施放藥水效果並進入冷卻
            currentFlask.Effect(null);
            flaskCooldown = currentFlask.itemCooldown;
            lastTimeUseFlask = Time.time;
        }
        else
        {
            float remaining = Mathf.Max(0f, lastTimeUseFlask + flaskCooldown - Time.time);
            Debug.Log($"Flask on cooldown: {remaining:F1}s remaining");
        }
        
        // 更新UI
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
        // 施放藥水效果
        currentFlask.Effect(null);

        // 使用後立即移除藥水（一次性使用）
        UnequipItem(currentFlask);

        // 更新UI顯示
        UpdateSlotUI();

        Debug.Log("Flask used and consumed: " + currentFlask.itemName);
    }


    //存档装备
    public void SaveData(ref GameData _data)
    {
        _data.inventory.Clear();
        _data.equipmentId.Clear();

        foreach (KeyValuePair<ItemData, InventoryItem> pair in inventoryDictionary)
        {
            _data.inventory.Add(pair.Key.itemId, pair.Value.stackSize);
        }

        foreach (KeyValuePair<ItemData, InventoryItem> pair in stashDictionary)
        {
            _data.inventory.Add(pair.Key.itemId, pair.Value.stackSize);
        }

        foreach (KeyValuePair<ItemData_Equipment, InventoryItem> pair in equipmentDictionary)
        {
            _data.equipmentId.Add(pair.Key.itemId);
        }
    }

    // 载入装备
    public void LoadData(GameData _data)
    {
        foreach (KeyValuePair<string, int> pair in _data.inventory)
        {
            foreach (var item in GetItemDataBase())
            {
                if (item != null && item.itemId == pair.Key)
                {
                    InventoryItem itemToLoad = new InventoryItem(item);
                    itemToLoad.stackSize = pair.Value;

                    loadedItems.Add(itemToLoad);
                }
            }
        }

        //根据记录的已穿戴装备ID附加装备槽装备
        foreach (string loadedItemId in _data.equipmentId)
        {
            foreach (var item in GetItemDataBase())
            {
                if (item != null && loadedItemId == item.itemId)
                {
                    loadedEquipment.Add(item as ItemData_Equipment);
                }
            }
        }

    }

    // 获取物品图鉴——Asset/Data/Items中所有物品的信息
    // 注意这不是存档
    private List<ItemData> GetItemDataBase()
    {
        List<ItemData> itemDataBase = new List<ItemData>();
        string[] assetNames = AssetDatabase.FindAssets("t:ItemData", new[] { "Assets/Data/Items" });
        foreach (string SOName in assetNames)
        {
            var SOpath = AssetDatabase.GUIDToAssetPath(SOName);
            // 檢查路徑是否為文件（不是文件夾）
            if (!SOpath.EndsWith("/") && !System.IO.Directory.Exists(SOpath))
            {
                var itemData = AssetDatabase.LoadAssetAtPath<ItemData>(SOpath);
                if (itemData != null)
                {
                    itemDataBase.Add(itemData);
                }
            }
        }
        
        return itemDataBase;
    }
}
