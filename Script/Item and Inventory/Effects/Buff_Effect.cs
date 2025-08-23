using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Buff effect", menuName = "Data/ItemEffect/Buff effect")]
public class Buff_Effect : ItemEffect
{
    private PlayerStats stats;
    [SerializeField] private StatType buffType;
    [Range(0,1)]
    [SerializeField] private float buffPercent;
    [SerializeField] private float buffDuration;

    public override void ExecuteEffect(Transform _enemyPosition)
    {
        stats = PlayerManager.instance.player.GetComponent<PlayerStats>();
        
        if (stats == null)
        {
            Debug.LogError("PlayerStats not found!");
            return;
        }
        
        int buffAmount = Mathf.RoundToInt(buffPercent * stats.GetStat(buffType).GetValue());

        stats.IncreaseStatsBy(buffAmount, buffDuration, stats.GetStat(buffType));
    }


}
