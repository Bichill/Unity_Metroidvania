using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "Thunder strike effect", menuName = "Data/ItemEffect/Thunder strike")]
public class ThunderStrike_Effect : ItemEffect
{
    [SerializeField] private GameObject thunderStrikePrefab;

    public override void ExecuteEffect(Transform _enemyPosition)
    {
        GameObject newThunderStrike = Instantiate(thunderStrikePrefab, _enemyPosition.position, Quaternion.identity);
        
        CharacterStats target = _enemyPosition.GetComponent<CharacterStats>();

        // ���_�O������A�u�w�Ă���ֵ��Ŀ��
        int shockDamage = PlayerManager.instance.player.stats.lightingDamgae.GetValue();
        newThunderStrike.GetComponent<ThunderStrike_Controller>().Setup(shockDamage, target);

        //PlayerManager.instance.player.stats.DoMagicalDamage(target);
    }
}
