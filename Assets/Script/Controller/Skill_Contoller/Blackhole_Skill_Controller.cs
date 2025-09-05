using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blackhole_Skill_Controller : MonoBehaviour
{
    [SerializeField] private GameObject hotKeyPrefab;
    [SerializeField] private List<KeyCode> keyCodeList;

    public float maxSize;
    public float growSpeed;
    public float shrinkSpeed;
    
    private bool canGrow=true;
    private bool canShrink;
    private bool canCreatHotKeys = true;
    private bool cloneAttackReleased;

    private int amountOfAttacks;
    private float cloneAttackCooldown;
    private float cloneAttackTimer;
    private float timeOutTimer;//超时计时器
    private bool isTimeOut = false;

    private List<Transform> targets = new List<Transform>();//私有化变量需要手动初始化
    private List<GameObject> createdHotKey = new List<GameObject>();
    private List<int> remainingAttacks = new List<int>();//每个敌人剩余的攻击次数

    public void SetupBlackHole(float _maxSize, float _growSpeed, float _shrinkSpeed, int _amountOfAttack,
        float _cloneAttackCooldown,float _timeOutTimer)
    {
        maxSize = _maxSize;
        growSpeed = _growSpeed;
        shrinkSpeed = _shrinkSpeed;
        amountOfAttacks = _amountOfAttack;
        cloneAttackCooldown = _cloneAttackCooldown;
        timeOutTimer = _timeOutTimer;
        AudioManager.instance.PlaySFX(4, transform, 1f, 1f);
        AudioManager.instance.PlaySFX(23, transform, 1f, 1f);
    }

    private void Update()
    {
        cloneAttackTimer -= Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.R) && !cloneAttackReleased && !PlayerManager.instance.player.isBusy)
        {
            ReleaseCloneAttack();
        }

        // 更新超时计时器
        if (!isTimeOut && !cloneAttackReleased)
        {
            timeOutTimer -= Time.deltaTime;
            if (timeOutTimer <= 0)
            {
                // 超时，结束技能
                Debug.Log("Blackhole timed out!");
                FinishBlackHoleAbility();
            }
        }

        CloneAttackLogic();

        if (canGrow && !canShrink)
        {
            transform.localScale = Vector2.Lerp(transform.localScale,
                new Vector2(maxSize, maxSize), growSpeed * Time.deltaTime);
        }

        if (canShrink)
        {
            transform.localScale = Vector2.Lerp(transform.localScale,
                new Vector2(-1, -1), shrinkSpeed * Time.deltaTime);

            if (transform.localScale.x <= 0)
            {
                Destroy(gameObject);
            }
            PlayerManager.instance.player.StartCoroutine("BusyFor", .3f);
        }
    }

    private void ReleaseCloneAttack()
    {
        DestroyHotKeys();
        cloneAttackReleased = true;
        canCreatHotKeys = false;
        PlayerManager.instance.player.fx.MakeTransprent(true);
        
        remainingAttacks.Clear();
        for (int i = 0; i < targets.Count; i++)
        {
            remainingAttacks.Add(amountOfAttacks);
        }
    }

    private void CloneAttackLogic()
    {
        if (cloneAttackTimer < 0 && cloneAttackReleased)
        {
            cloneAttackTimer = cloneAttackCooldown;

            // 找出还有剩余攻击次数的敌人
            List<int> validTargets = new List<int>();
            for (int i = 0; i < remainingAttacks.Count; i++)
            {
                if (remainingAttacks[i] > 0)
                {
                    validTargets.Add(i);
                }
            }

            if (validTargets.Count == 0)
            {
                Invoke("FinishBlackHoleAbility", .5f);
                return;
            }

            //随机选择一个还有攻击次数的敌人
            int randomIndex = validTargets[Random.Range(0, validTargets.Count)];

            float xOffset = Random.Range(0, 10) > 5 ? 2 : -2;
            SkillManager.instance.clone.CreatClone(targets[randomIndex], new Vector3(xOffset, 0));

            remainingAttacks[randomIndex]--;
        }
    }

    private void FinishBlackHoleAbility()
    {
        PlayerManager.instance.player.ExitBlackHoleAbility();
        canShrink = true;
        cloneAttackReleased = false;
        DestroyHotKeys();
    }

    private void DestroyHotKeys()
    {
        if (createdHotKey.Count <= 0)
        {
            return;
        }

        for (int i = 0; i < createdHotKey.Count; i++)
        {
            Destroy(createdHotKey[i]);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Enemy>() != null)
        {
            collision.GetComponent<Enemy>().FreezeTimer(true);
            CreateHotKey(collision);
        }
    }


    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<Enemy>() != null)
        {
            collision.GetComponent<Enemy>().FreezeTimer(false);
        }
    }

    //private void OnTrrigerExit2D(Collider2D collision) => collision.GetComponent<Enemy>()?.FreezeTimer(false);

    private void CreateHotKey(Collider2D collision)
    {
        if (keyCodeList.Count <= 0)
        {
            Debug.LogWarning("Not enough hot key in a key code list");
        }    

        if (!canCreatHotKeys)
        {
            return;
        }

        GameObject newHotKey = Instantiate(hotKeyPrefab, collision.transform.position
                        + new Vector3(0, 2), Quaternion.identity);
        createdHotKey.Add(newHotKey);

        KeyCode choosenKey = keyCodeList[Random.Range(0, keyCodeList.Count)];
        keyCodeList.Remove(choosenKey);
         
        Blackhole_HotKey_Controller newHotKeyScript = newHotKey.GetComponent<Blackhole_HotKey_Controller>();

        newHotKeyScript.SetupHotKey(choosenKey, collision.transform, this);
    }

    public void AddEnemyToList(Transform _enemyTransform) => targets.Add(_enemyTransform);
}
