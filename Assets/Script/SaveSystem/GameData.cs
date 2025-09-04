using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//��Ǻ�֧�����л�
//��ͷ��β��ֻ��һ��ͳһ�����ֻ࣬���������а���������ģ�����������
//����������ж���ӿڣ�ÿ��ģ��ֻȡ������ص�����
[System.Serializable]
public class GameData 
{
    //�洢����
    public int currency;
    
    //�洢װ��
    public SerializableDictionary<string, int> inventory;
    //����Ƿ��Ѿ�������ʼװ��
    public bool hasReceivedStartingItems;
    //��¼������װ��
    public List<string> equipmentId;

    //�洢����
    public SerializableDictionary<string, bool> skillTree;

    //�洢�ڵ�
    public SerializableDictionary<string, bool> checkpoints;
    public string closestCheckpointId;

    //��������
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
