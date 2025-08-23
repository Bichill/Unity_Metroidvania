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
        
        // 確保 primaryAttack 存在
        if (player.primaryAttack == null)
        {
            Debug.LogWarning("Player primaryAttack is null!");
            return;
        }

        // 檢查是否為第三次攻擊
        if (player.primaryAttack.comboCounter != 2)
            return;

        int randomNum = Random.Range(0, 2);//Range左闭右开

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

        // 確保刀光預製體存在
        if (bladeLightPrefab == null)
        {
            Debug.LogError("BladeLight prefab is null!");
            return;
        }

        // 在主角身前生成刀光預製體
        Vector3 spawnPosition = player.attackCheck.position;
        GameObject newBladeLight = Instantiate(bladeLightPrefab, spawnPosition, Quaternion.identity);

        // 設置刀光朝向
        newBladeLight.transform.right = player.facingDir * Vector3.right;

        newBladeLight.GetComponent<BladeLight_Controller>().Setup(damaged, randomNum);
    }
}
