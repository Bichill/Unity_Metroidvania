using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//标记后支持序列化
//从头到尾都只有一个统一数据类，只是数据类中包含了所有模块所需的数据
//这个数据类有多个接口，每个模块只取与其相关的数据
[System.Serializable]
public class GameData 
{
    //存储货币
    public int currency;
    
    //存储装备
    public SerializableDictionary<string, int> inventory;
    //标记是否已经给过初始装备
    public bool hasReceivedStartingItems;
    //记录穿戴的装备
    public List<string> equipmentId;

    //存储技能
    public SerializableDictionary<string, bool> skillTree;

    //存储节点
    public SerializableDictionary<string, bool> checkpoints;
    public string closestCheckpointId;

    //死亡掉落
    public float lostCurrencyX;
    public float lostCurrencyY;
    public int lostCurrencyAmount;
    public bool isPickedUpLostCurrency;

    public GameData()
    {
        this.currency = 0;
        this.lostCurrencyX = 0;
        this.lostCurrencyY = 0;
        this.lostCurrencyAmount = 0;
        
        skillTree = new SerializableDictionary<string, bool>();
        inventory = new SerializableDictionary<string, int>();
        hasReceivedStartingItems = false;
        equipmentId = new List<string>();
        isPickedUpLostCurrency = true;

        closestCheckpointId = string.Empty;
        checkpoints = new SerializableDictionary<string, bool>();
    }
}
