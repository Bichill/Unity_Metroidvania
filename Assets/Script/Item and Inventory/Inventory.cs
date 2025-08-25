using System.Collections.Generic;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEditor;
using UnityEngine;
using static UnityEditor.Progress;  

// �@��һ������ϵ�y��ؓ؟����������
// �f����
// inventory����������stash���}�죩�����߶��Ǵ��InventoryItem�ļ��ϣ����Ǵ�ŵ���Ʒ��Ͳ�ͬ���b��/���ϣ�
// �@���OӋ����׌��҅^���b��Ͳ��ϣ���UI��Ҳ�ܷ����@ʾ
public class Inventory : MonoBehaviour, ISaveManager
{
    // ����ģʽ���_��ȫ��Ψһ����
    public static Inventory instance;

    public List<ItemData> startingItem;

    // �b�����P������b��
    public List<InventoryItem> equipment;
    public Dictionary<ItemData_Equipment, InventoryItem> equipmentDictionary;

    // �������P����Ÿ��N��Ʒ
    public List<InventoryItem> inventory;
    public Dictionary<ItemData, InventoryItem> inventoryDictionary;

    // �}�죨stash������Ų������Ʒ
    public List<InventoryItem> stash;
    public Dictionary<ItemData, InventoryItem> stashDictionary;

    [Header("Inventory UI")]
    // inventorySlotParent������UI�ĸ�������Á����������
    [SerializeField] private Transform inventorySlotParent;
    // stashSlotParent���}��UI�ĸ�������Á����}�����
    [SerializeField] private Transform stashSlotParent;
    // equipmentSlotParent: �b��UI�ĸ�������Á�����b�����
    [SerializeField] private Transform equipmentSlotParent;
    // statSlotParent: ��ɫ����UI�ĸ�������Á��������@ʾ
    [SerializeField] private Transform statSlotParent;

    // inventoryItemSlot������UI���ӵ�����
    private UI_ItemSlot[] inventoryItemSlot;
    // stashItemSlot���}��UI���ӵ�����
    private UI_ItemSlot[] stashItemSlot;
    // equipmentItemSlot���b��UI���ӵ�����
    private UI_EquipmentSlot[] equipmentItemSlot;
    // statItemSlot������UI���ӵ�����
    private UI_StatSlot[] statItemSlot;

    [Header("Item Cooldown")]
    private float lastTimeUseFlask;
    private float lastTimeUseArmor;

    public float flaskCooldown;
    private float armorCooldown;

    [Header("Data Base")]
    public List<InventoryItem> loadedItems;//��ȡ��װ��������б�
    public List<ItemData_Equipment> loadedEquipment;//��ȡ��װ�����б�
     
    private void Awake()
    {
        // ��ʼ���������_��ȫ��Ψһ
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
        // ��ʼ�����N�б�
        inventory = new List<InventoryItem>();
        inventoryDictionary = new Dictionary<ItemData, InventoryItem>();

        stash = new List<InventoryItem>();
        stashDictionary = new Dictionary<ItemData, InventoryItem>();

        equipment = new List<InventoryItem>();
        equipmentDictionary = new Dictionary<ItemData_Equipment, InventoryItem>();

        // �@ȡ�����͂}��UI���ӵ�����
        inventoryItemSlot = inventorySlotParent.GetComponentsInChildren<UI_ItemSlot>();
        stashItemSlot = stashSlotParent.GetComponentsInChildren<UI_ItemSlot>();
        equipmentItemSlot = equipmentSlotParent.GetComponentsInChildren<UI_EquipmentSlot>();
        statItemSlot = statSlotParent.GetComponentsInChildren<UI_StatSlot>();

        AddStartingItem();
    }

    private void AddStartingItem()
    {
        // ������Ѿ�������װ������ӽ���װ����
        foreach (ItemData_Equipment item in loadedEquipment)
        {
            EquipItem(item);
        }

        // ������d�����Ʒ����������d�����Ʒ
        if (loadedItems.Count > 0)
        {
            foreach (InventoryItem item in loadedItems)
            {
                for (int i = 0; i < item.stackSize; i++)
                {
                    AddItem(item.data);
                }
            }
            return; // ������d�����Ʒ���Ͳ���ӳ�ʼ��Ʒ
        }

        // �z���Ƿ��ѽ��o�^��ʼ�b�䣨��Ҫ�șz�� SaveManager �Ƿ���ڣ�
        if (SaveManager.instance != null && SaveManager.instance.GameData != null && SaveManager.instance.GameData.hasReceivedStartingItems)
        {
            return; // �ѽ��o�^��ʼ�b�䣬�������}�o
        }

        // ֻ���ڛ]���d����Ʒ�қ]�нo�^��ʼ�b��r������ӳ�ʼ�b��
        for (int i = 0; i < startingItem.Count; i++)
        {
            if (startingItem[i] != null)
                AddItem(startingItem[i]);
        }

        // ��ӛ�ѽ��o�^��ʼ�b��
        if (SaveManager.instance != null && SaveManager.instance.GameData != null)
        {
            SaveManager.instance.GameData.hasReceivedStartingItems = true;
        }
    }

    // �b��ʹ��
    public void EquipItem(ItemData _item)
    {
        ItemData_Equipment newEquipment = _item as ItemData_Equipment;
        InventoryItem newItem = new InventoryItem(_item);

        ItemData_Equipment itemByRepeated = null;

        // �z�鮔ǰ�b����Ƿ������}�b����ͣ�����о�ж��
        foreach (KeyValuePair<ItemData_Equipment, InventoryItem> item in equipmentDictionary)
        {
            if (item.Key.equipmentType == newEquipment.equipmentType)
                itemByRepeated = item.Key;
        }

        // �Ƴ����}�b�䣬�Żر���
        if (itemByRepeated != null)
        {
            UnequipItem(itemByRepeated);
            AddItem(itemByRepeated);
        }

        equipment.Add(newItem);// �b������Ʒ
        equipmentDictionary.Add(newEquipment, newItem);
        newEquipment.AddModifiers();// ����b����Լӳ�

        RemoveItem(_item);// �ı����Ƴ����b�����Ʒ

        UpdateSlotUI();
    }

    // ж���b�䣬�Ƴ��b��
    public void UnequipItem(ItemData_Equipment _itemToRemove)
    {
        if (equipmentDictionary.TryGetValue(_itemToRemove, out InventoryItem value))
        {
            equipment.Remove(value);
            equipmentDictionary.Remove(_itemToRemove);
            _itemToRemove.RemoveModifiers(); // �Ƴ��b����Լӳ�
        }
    }

    // ˢ��UI�@ʾ
    private void UpdateSlotUI()
    {
        // ��ӿ�ֵ�z�飬�����ڳ�ʼ���^�����{�Õr���e
        if (inventoryItemSlot == null || stashItemSlot == null || equipmentItemSlot == null)
        {
            Debug.LogWarning("UI slots are not initialized yet, skipping UpdateSlotUI");
            return;
        }

        // ���ԭ�Ђ}��ͱ���UI
        for (int i = 0; i < inventoryItemSlot.Length; i++)
        {
            // �ų��ϳɸ���
            if (inventoryItemSlot[i] is UI_CraftSlot)
                continue;
            if (inventoryItemSlot[i] != null)
            {
            inventoryItemSlot[i].CleanUpSlot();
            }
        }

        for (int i = 0; i < stashItemSlot.Length; i++)
        {
            // �ų��ϳɸ���
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

        // �����µĂ}��ͱ���UI
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

        // �_���b���@ʾ�ڌ������b���
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
        // ��ӿ�ֵ�z�飬�����ڳ�ʼ���^�����{�Õr���e
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

    // �����Ʒ���^�ֱ����͂}��
    public void AddItem(ItemData _item)
    {
        if (_item.itemType == ItemType.Equipment && CanAddItem())
            AddToInventory(_item); // �b����뱳��
        else if (_item.itemType == ItemType.Material)
            AddToStash(_item);     // ���Ϸ���}��
        
        UpdateSlotUI();
    }

    // ��Ӳ��ϵ��}��
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

    // ����b�䵽����
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


    // �z��Ҫ�ϳɵ��b�䣬�z����ϱ�
    public bool CanCraft(ItemData_Equipment _itemToCraft, List<InventoryItem> _requiredMaterials)
    {
        // �z��ݔ�녢��
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
                // �z��}���Ƿ������Ĳ���
                if (stashValue.stackSize < _requiredMaterials[i].stackSize)
                {
                    Debug.Log("Crafting failed: Not enough materials in stash.");
                    return false; // ���ϲ��㣬�o���ϳ�
                }
                else
                {
                    materialsToRemove.Add(stashValue); // ӛ���Ҫ�Ƴ��Ĳ���
                } 
            }
            else
            {
                Debug.Log("Crafting failed: Missing required materials.");
                return false; // ȱ�ٲ��ϣ��o���ϳ�
            }   
        }

        // ���в��϶��M��Ҫ���_ʼ�ϳ�
        for (int i = 0; i < materialsToRemove.Count; i++)
        {
            RemoveItem(materialsToRemove[i].data); // �Ă}�����Ƴ�����
        }

        // ���ϳɵ��b����ӵ�����
        AddItem(_itemToCraft);
        Debug.Log("Crafting successful: " + _itemToCraft.name);

        return true; // �ϳɳɹ�
    }

    public List<InventoryItem> GetStashList() => stash;// ���؂}��
    public List<InventoryItem> GetInventoryList() => inventory;// ���ر���

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
        // ʩ��ˎˮЧ��
        currentFlask.Effect(null);

        // ʹ���������Ƴ�ˎˮ��һ����ʹ�ã�
        UnequipItem(currentFlask);

        // ����UI�@ʾ
        UpdateSlotUI();

        Debug.Log("Flask used and consumed: " + currentFlask.itemName);
    }


    //�浵װ��
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

    // ����װ��
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

        //���ݼ�¼���Ѵ���װ��ID����װ����װ��
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

    // ��ȡ��Ʒͼ������Asset/Data/Items��������Ʒ����Ϣ
    // ע���ⲻ�Ǵ浵
    private List<ItemData> GetItemDataBase()
    {
        List<ItemData> itemDataBase = new List<ItemData>();
        string[] assetNames = AssetDatabase.FindAssets("t:ItemData", new[] { "Assets/Data/Items" });
        foreach (string SOName in assetNames)
        {
            var SOpath = AssetDatabase.GUIDToAssetPath(SOName);
            // �z��·���Ƿ���ļ��������ļ��A��
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
