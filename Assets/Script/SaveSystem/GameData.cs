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

    public GameData()
    {
        this.currency = 0;
        skillTree = new SerializableDictionary<string, bool>();
        inventory = new SerializableDictionary<string, int>();
        hasReceivedStartingItems = false;
        equipmentId = new List<string>();
    }
}
