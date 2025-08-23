using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDrop : MonoBehaviour
{
    [SerializeField] private ItemData[] possibleDrop;
    private List<ItemData> dropList = new List<ItemData>();//�����б�

    [SerializeField] private GameObject dropPrefab;

    public virtual void GenerateDrop()
    {
        for (int i = 0; i < possibleDrop.Length; i++)
        {
            if (Random.Range(0, 100) <= possibleDrop[i].dropChance)
            {
                dropList.Add(possibleDrop[i]);
            }
        }

        if (dropList.Count == 0)
            return;//��������б�Ϊ�գ��򲻵����κ���Ʒ
                   // ʹ��whileѭ����ֱ��������Ʒ��������
        while (dropList.Count > 0)
        {
            ItemData randomItem = dropList[Random.Range(0, dropList.Count)];
            dropList.Remove(randomItem);
            DropItem(randomItem);
        }

    }

    protected void DropItem(ItemData _itemData)
    {
        GameObject newDrop = Instantiate(dropPrefab, transform.position, Quaternion.identity);

        Vector2 randomVelocit = new Vector2(Random.Range(-7, +7), Random.Range(17, 20));

        newDrop.GetComponent<ItemObject>().SetupItem(_itemData, randomVelocit);
    }
}
