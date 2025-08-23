using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Ghostly windbreaker effect", menuName = "Data/ItemEffect/Ghostly windbreaker effect")]
public class GhostlyWindbreaker_Effect : ItemEffect
{
    private PlayerStats stats;  

    public override void ExecuteEffect(Transform _enemyPosition)
    {
        stats = PlayerManager.instance.player.GetComponent<PlayerStats>();

        if (stats == null)
        {
            Debug.LogError("PlayerStats not found!");
            return;
        }

        //��������У�ֱ�ӿ۳��������ֵ����������֤����Ч��
        stats.TakeDamage(stats.maxHealth.GetValue()*2);
    }
}
