using System.Text;
using UnityEngine;

public enum ItemType
{
    Material,
    Equipment,
}

[CreateAssetMenu(fileName ="New Item Data", menuName = "Data/Item")]
public class ItemData : ScriptableObject
{ 
    public ItemType itemType;
    public string itemName;
    public Sprite icon;
    public string itemId;

    [Range(0,100)]
    public float dropChance; // µôÂä¼¸ÂÊ

    protected StringBuilder sb = new StringBuilder();

    private void OnValidate()
    {

    }

    public virtual string GetDescription()
    {
        return "";
    }
}
