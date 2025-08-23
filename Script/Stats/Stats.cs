using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public class Stats 
{
    [SerializeField] private int baseValue;

    public List<int> modifiers = new List<int>();

    private void Update()
    {

    }

    public int GetValue()
    {
        int finalValue = baseValue;
        foreach (int modifier in modifiers)
        {
            finalValue += modifier;
        }

        return finalValue;
    }

    // 獲取基礎值（不包括修改器）
    public int GetBaseValue()
    {
        return baseValue;
    }

    public void SetDefaultValue(int _value)
    {
        baseValue = _value;
    }

    public void AddModifier(int _modifier)
    {
        modifiers.Add(_modifier);
    }

    public void RemoveModifier(int _modifier) 
    {
        if (modifiers.Contains(_modifier))
        {
            modifiers.Remove(_modifier);
        }
    }
}
