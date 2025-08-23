using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "Blade light effect", menuName = "Data/ItemEffect/Blade light effect")]
public class BladeLight_Effect : ItemEffect
{
    private int damaged;

    [SerializeField] private GameObject bladeLightPrefab;

    public override void ExecuteEffect(Transform _enemyPosition)
    {
        Player player = PlayerManager.instance.player;
        
        // �_�� primaryAttack ����
        if (player.primaryAttack == null)
        {
            Debug.LogWarning("Player primaryAttack is null!");
            return;
        }

        // �z���Ƿ������ι���
        if (player.primaryAttack.comboCounter != 2)
            return;

        int randomNum = Random.Range(0, 2);//Range����ҿ�

        if (randomNum == 0)
        {
            damaged = player.stats.iceDamage.GetValue();
            Debug.Log("ice damage: " + damaged);
        }
        else if (randomNum == 1)
        {
            damaged = player.stats.fireDamage.GetValue();
            Debug.Log("fire damage: " + damaged);
        }
        else
        {
            Debug.Log("BladeLight RandomNum Error!!");
            return;
        }

        // �_�������A�u�w����
        if (bladeLightPrefab == null)
        {
            Debug.LogError("BladeLight prefab is null!");
            return;
        }

        // ��������ǰ���ɵ����A�u�w
        Vector3 spawnPosition = player.attackCheck.position;
        GameObject newBladeLight = Instantiate(bladeLightPrefab, spawnPosition, Quaternion.identity);

        // �O�õ��⳯��
        newBladeLight.transform.right = player.facingDir * Vector3.right;

        newBladeLight.GetComponent<BladeLight_Controller>().Setup(damaged, randomNum);
    }
}
