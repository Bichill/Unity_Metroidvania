using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour,ISaveManager
{
    public static PlayerManager instance;
    public Player player;

    public int currency;
    private void Awake()
    {
        if(instance!=null)
           Destroy(instance.gameObject);
        else
            instance = this;
    }

    public bool HaveEnoughSkillPoint(int _skillPoint)
    {
        if (_skillPoint > currency)
        {
            Debug.Log("Not enough money");
            return false;
        }

        currency  = currency - _skillPoint;
        return true;
    }

    public int GetCurrencyAmount() => currency;

    public void LoadData(GameData _data)
    {
        this.currency = _data.currency;
    }

    public void SaveData(ref GameData _data)
    {
        _data.currency = this.currency;
    }
}
