using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

[CreateAssetMenu(fileName = "Freezing enemies effect", menuName = "Data/ItemEffect/Freezing enemies effect")]
public class FreezingEnemies_Effect : ItemEffect
{
    private int damaged;    
    [SerializeField] private int freezeDuration;
    [SerializeField] private GameObject freezingPrefab;

    public override void ExecuteEffect(UnityEngine.Transform _enemyPosition)
    {
        if (!Inventory.instance.CanUseArmor())
            return;

        Player player = PlayerManager.instance.player;
        damaged = player.stats.iceDamage.GetValue();

        // �_���A�u�w����
        if (freezingPrefab == null)
        {
            Debug.LogError("FreezingEnemiesPrefab is null!");
            return;
        }

        // ������λ�������A�u�w
        Vector3 spawnPosition = player.transform.position;
        GameObject newFreezingPrefab = Instantiate(freezingPrefab, spawnPosition, Quaternion.identity);

        newFreezingPrefab.GetComponent<FreezingEnemies_Controller>().Setup(damaged, freezeDuration);
    }
}
