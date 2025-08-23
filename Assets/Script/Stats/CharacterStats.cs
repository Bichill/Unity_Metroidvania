using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Unity.Mathematics;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using Random = UnityEngine.Random;

public enum StatType
{
    strength,
    agility,
    intelligence,
    vitality,
    damage,
    critChance,
    critPower,
    health,
    armor,
    evasion,
    magicRes,
    fireDamage,
    iceDamage,
    lightingDamage
}

//灼烧的不同状态
public struct IgniteStatus
{
    public float duration;
    public float damagePerSecond;

    public IgniteStatus(float duration, float dps)
    {
        this.duration = duration;
        this.damagePerSecond = dps;
    }
}

public class CharacterStats : MonoBehaviour
{
    private EntityFX fx;
    public bool isDead { get; private set; }// 是否死亡,避免重复死亡
    public bool isVulnerable;// 是否易伤

    [Header("Major stats")]
    public Stats strength; // 力量，每点+3攻击 +1暴击倍率
    public Stats agility; // 敏捷，每点+1闪避 +1暴击概率
    public Stats intelligence; // 智力，每点+5法强 +1魔抗
    public Stats vitality; // 体力，每点+10生命

    [Header("Offensive stats")]
    public Stats damage;
    public Stats critChance; // 暴击率
    public Stats critPower; // 暴击伤害

    [Header("Defensive stats")]
    public Stats maxHealth;
    public Stats armor; // 护甲，减少物理伤害
    public Stats evasion; // 闪避，概率免疫伤害
    public Stats magicResistence; // 魔法抗性，减少魔法伤害

    [Header("Magic Ignite")]
    public Stats fireDamage;
    public bool isIgnited; // 是否点燃（持续火焰伤害）
    private float ignitedTimer;
    private float igniteDamageCooldown = .2f;
    public List<IgniteStatus> igniteList = new List<IgniteStatus>();//火焰层数
    [SerializeField] private GameObject igniteStampPrefab;//灼烧印记

    //废除变量区：
    //private float igniteDamageTimer;
    //private int igniteDamage;

    [Header("Magic Chill")]
    public bool isChilled; // 是否冰缓（减速）
    public Stats iceDamage;
    private float chilledTimer;

    [Header("Magic Shock")]
    public Stats lightingDamgae;
    public bool isShocked; // 是否感电（易伤）
    [SerializeField] private GameObject shockStrikePrefab;
    private int shockDamage;
    private float shockedTimer;

    [Header("Ailments Timer")]
    public float ailmentsDuration = 4; // 异常状态持续时间

    [Header("Other")]
    public int currentHealth;

    public System.Action onHealthChanged;


    protected virtual void Start()
    {
        critPower.SetDefaultValue(150); // 150% 暴擊傷害
        
        // 確保血量正確初始化
        currentHealth = CalculateMaxHealth();
        
        // 觸發血量變化事件
        if (onHealthChanged != null)
            onHealthChanged();

        fx = GetComponentInChildren<EntityFX>();
        InvokeRepeating("BurningTick", 0, 0.2f);
    }

    protected virtual void Update()
    {
        AilmentsTimer();
    }

    public void MakeVulnerable(float _duration)
    {
        // 开始易伤状态协程
        StartCoroutine(VulnerableCorountine(_duration));
    }

    private IEnumerator VulnerableCorountine(float _duration)
    {
        isVulnerable = true;
        yield return new WaitForSeconds(_duration);
        isVulnerable = false;
    }

    public virtual void IncreaseStatsBy(int _modifier, float _duration, Stats _statsToModifer)
    {
        // start corototuine for stat increase
        StartCoroutine(StatsModCorountine(_modifier, _duration, _statsToModifer));
    }

    private IEnumerator StatsModCorountine(int _modifier, float _duration, Stats _statsToModifer)
    {
        _statsToModifer.AddModifier(_modifier);

        yield return new WaitForSeconds(_duration);

        // 確保在移除修改器之前檢查對象是否仍然存在
        if (_statsToModifer != null)
        {
            _statsToModifer.RemoveModifier(_modifier);
        }
    }

    public void ChangeDeadToTrue() => isDead = true;

    public virtual void DoDamage(CharacterStats _targetStats,float _attackMultiplier = 1)
    {
        if (CheckTargetEvasion(_targetStats))
            return;

        int totalDamage = CalculateBaseDamage();

        if(_attackMultiplier > 0)
            totalDamage = Mathf.RoundToInt(totalDamage * _attackMultiplier);

        if (CheckTargetCrit())
        {
            //计算暴击
            totalDamage = CalculateCriticalDamage(totalDamage);
        }

        totalDamage = CheckTargetArmor(_targetStats, totalDamage);

        _targetStats.TakeDamage(totalDamage);
    }

    #region Magical Damage And Ailments

    // 灼烧每0.2秒处理逻辑
    private void BurningTick()
    {
        if (currentHealth <= 0)
        {
            igniteList.Clear();
            isIgnited = false; // 死亡时移除异常
            CancelInvoke(nameof(BurningTick)); // 死亡时停止灼烧
            Die();
            return;
        }

        for (int i = 0; i <= igniteList.Count - 1; i++)
        {
            IgniteStatus ignite = igniteList[i];
            Debug.Log("ignite i: " + i + " ignite: " + ignite.damagePerSecond);
            DecreaseHealthBy(Mathf.Max(Mathf.RoundToInt(ignite.damagePerSecond), 1));
            ignite.duration -= igniteDamageCooldown;
            if (ignite.duration <= 0)
            {
                igniteList.RemoveAt(i);
            }
            else
            {
                igniteList[i] = ignite;
            }
        }
    }


    // 处理魔法伤害和异常状态
    public virtual void DoMagicalDamage(CharacterStats _targetStats)
    {

        //---------------------------------计算单次伤害------------------------------
        int _fireDamage = fireDamage.GetValue();
        int _iceDamage = iceDamage.GetValue();
        int _lightningDamage = lightingDamgae.GetValue();

        int totalMagicDamage = CalculatetotalMagicDamage(Mathf.Max(_fireDamage, _iceDamage, _lightningDamage));

        totalMagicDamage = CheckTargetResistence(_targetStats, totalMagicDamage);

        _targetStats.TakeDamage(totalMagicDamage);

        //----------------------------------Debuff---------------------------------
        // 元素伤害触发Debuff
        if (Mathf.Max(_fireDamage, _iceDamage, _lightningDamage) <= 0)
        {
            return;
        }

        AttemptToApplyAliments(_targetStats, _fireDamage, _iceDamage, _lightningDamage);
    }

    // 尝试应用异常状态
    private void AttemptToApplyAliments(CharacterStats _targetStats, int _fireDamage, int _iceDamage, int _lightningDamage)
    {
        bool canApplyIgnite = _fireDamage > _iceDamage && _fireDamage > _lightningDamage;
        bool canApplyChill = _iceDamage > _fireDamage && _iceDamage > _lightningDamage;
        bool canApplyShock = _lightningDamage > _fireDamage && _lightningDamage > _iceDamage;

        while (!canApplyIgnite && !canApplyChill && !canApplyShock)
        {
            if (Random.value < .33f && _fireDamage > 0)
            {
                canApplyIgnite = true;
                //_targetStats.ApplyAilments(canApplyIgnite, canApplyChill, canApplyShock, _targetStats);
                break;
            }
            if (Random.value < .66f && _iceDamage > 0)
            {
                canApplyChill = true;
                //_targetStats.ApplyAilments(canApplyIgnite, canApplyChill, canApplyShock, _targetStats);
                break;
            }
            if (Random.value < 1f && _lightningDamage > 0)
            {
                canApplyShock = true;
                //_targetStats.ApplyAilments(canApplyIgnite, canApplyChill, canApplyShock, _targetStats);
                break;
            }
        }

        // 元素伤害触发Debuff
        if (canApplyIgnite)
        {
            // 点燃伤害为元素伤害的0.15-0.25倍
            float igniteMultiplier = Random.Range(0.15f, 0.25f);
            int igniteDps = (int)(Mathf.Max(_fireDamage * igniteMultiplier, 1));
            _targetStats.ApplyIgnite(ailmentsDuration, igniteDps);
        }

        if (canApplyShock)
        {
            //落雷伤害为元素伤害的0.5倍
            _targetStats.SetupShockDamage(Mathf.Max(Mathf.RoundToInt(_lightningDamage * 0.5f), 1));
        }
        _targetStats.ApplyAilments(canApplyIgnite, canApplyChill, canApplyShock, _targetStats);
        return;
    }

    // 异常状态计时器
    private void AilmentsTimer()
    {
        ignitedTimer -= Time.deltaTime;
        chilledTimer -= Time.deltaTime;
        shockedTimer -= Time.deltaTime;

        //igniteDamageTimer -= Time.deltaTime;

        // 火焰异常处理
        if (ignitedTimer < 0)
        {
            isIgnited = false;
        }
        // 冰缓异常处理
        if (chilledTimer < 0)
        {
            isChilled = false;
        }
        // 感电异常处理
        if (shockedTimer < 0)
        {
            isShocked = false;
        }
    }

    // 应用异常状态
    public void ApplyAilments(bool _ignite, bool _chill, bool _shock, CharacterStats _target)
    {
        // 点燃
        if (_ignite)
        {
            isIgnited = _ignite;
            ignitedTimer = ailmentsDuration;
            fx.IgniteFxFor(ailmentsDuration);
            //火焰层数标记在这里
            TrySwapnIgniteStamp(_target, igniteStampPrefab);
        }
        // 冰缓
        if (_chill)
        {
            isChilled = _chill;
            chilledTimer = ailmentsDuration;

            float slowPercentage = .4f;

            GetComponent<Entity>().SlowEntityBy(slowPercentage, ailmentsDuration);
            fx.ChillFxFor(ailmentsDuration);
        }
        // 感电
        if (_shock)
        {
            if (!isShocked)
            {
                ApplyShock(_shock);
            }
            else
            {
                // 落雷第一个目标
                GameObject ShockStrike = Instantiate(shockStrikePrefab, transform.position, Quaternion.identity);
                Debug.Log("Thunder shock Damage: " + shockDamage);
                ShockStrike.GetComponent<ThunderStrike_Controller>().Setup(shockDamage, _target);

                //-----------------------------------------------通过感电雷击第二个怪物---------------------------------------------------

                //如果是玩家则不进行感电
                if (this.GetComponent<Player>() != null)
                    return;
                
                ShockStrikeClosestTarget(_target);
            }
        }
    }

    // 应用感电状态
    public void ApplyShock(bool _shock)
    {
        if (isShocked) return;

        isShocked = _shock;
        shockedTimer = ailmentsDuration;

        fx.ShockFxFor(ailmentsDuration);
    }

    /* 此函数已废除
    private void ApplyIgniteDamge()
    {
        if (igniteDamageTimer < 0)
        {
            Debug.Log("Turn back fire : " + igniteDamage);
            DecreaseHealthBy(igniteDamage); // 持续火焰伤害
            if (currentHealth <= 0)
            {
                isIgnited = false; // 死亡时移除异常
                Die();
            }
            igniteDamageTimer = igniteDamageCooldown;
        }
    }
    */

    // 通过感电雷击最近的目标
    private void ShockStrikeClosestTarget(CharacterStats _target)
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 10);

        float closestDistance = Mathf.Infinity;

        Transform closestEnemy = null;

        foreach (var hit in colliders)
        {
            if (hit.GetComponent<Enemy>() != null && hit.GetComponent<CharacterStats>() != _target)
            {
                float distanceToEnemy = Vector2.Distance(transform.position, hit.transform.position);

                if (distanceToEnemy < closestDistance)
                {
                    closestDistance = distanceToEnemy;
                    closestEnemy = hit.transform;
                }
            }
        }

        if (closestEnemy != null)
        {
            GameObject ShockStrike1 = Instantiate(shockStrikePrefab, transform.position, Quaternion.identity);

            ShockStrike1.GetComponent<ThunderStrike_Controller>().Setup(shockDamage, closestEnemy.GetComponent<CharacterStats>());
        }
    }

    //添加灼烧层数
    public void ApplyIgnite(float duration, float dps)
    {
        if (igniteList.Count >= 2)
        {
            int idx = igniteList[0].duration < igniteList[1].duration ? 0 : 1;
            igniteList[idx] = new IgniteStatus(duration, dps);
        }
        else
        {
            igniteList.Add(new IgniteStatus(duration, dps));
        }
        
        // 設置點燃狀態
        isIgnited = true;
        ignitedTimer = ailmentsDuration;
        
        // 生成火焰紋章
        TrySwapnIgniteStamp(this, null);
    }

    //生成火焰纹章
    public void TrySwapnIgniteStamp(CharacterStats _target,GameObject _igniteStamp)
    {
        //检查目标此时是否已有火焰纹章
        var existing = _target.GetComponentInChildren<IgniteStamp_Controller>();
        if (existing != null)
        {
            return;
        }

        GameObject ignitestamp = Instantiate(igniteStampPrefab);
        ignitestamp.GetComponent<IgniteStamp_Controller>().Setup(_target);
        ignitestamp.transform.SetParent(_target.transform); //作为目标的子物体，下次能够被existing检测到
    }

    #endregion

    public virtual void TakeDamage(int _damage)
    {
        DecreaseHealthBy(_damage);

        GetComponent<Entity>().DamageImpact();
        fx.StartCoroutine("FlashFX");

        if (currentHealth <= 0 && isDead)
        {
            Die();
        }
    }

    // 自定义恢复的回血方法
    public virtual void IncreaseHealthBy(int _amount)
    {
        currentHealth += _amount;

        if (currentHealth > CalculateMaxHealth())
        {
            currentHealth = CalculateMaxHealth();
        }

        if (onHealthChanged != null)
        {
            onHealthChanged();
        }
    }

    // 自定义伤害的扣血方法（不觸發護甲效果）
    protected virtual void DecreaseHealthBy(int _damage)
    {
        if (isVulnerable)
        {
            _damage = Mathf.RoundToInt(_damage * 1.5f); // 易伤状态下伤害增加150%
        }

        currentHealth -= _damage;

        // 確保血量不會低於0
        if (currentHealth < 0)
        {
            currentHealth = 0;
        }

        if (onHealthChanged != null)
        {
            onHealthChanged();
        }
    }

    /* 此函数已废除
    public void SetupIgniteDamage(int _damage)
    {
        igniteDamage = _damage;
    }
    */

    // 设置雷击伤害
    public void SetupShockDamage(int _damage)
    {
        shockDamage = _damage;
    }

    protected virtual void Die()
    {
       ChangeDeadToTrue();
    }

    #region Calculate Stats

    // 检查目标是否暴击
    protected bool CheckTargetCrit()
    {
        int totalCriticalChance = critChance.GetValue();

        if (Random.Range(0, 100) <= totalCriticalChance)
            return true;
        else return false;
    }

    // 检查目标护甲并计算最终伤害
    protected int CheckTargetArmor(CharacterStats _targetState, int totalDamage)
    {
        // 如果目标处于冻伤状态，减少护甲值
        if (_targetState.isChilled)
        {
            totalDamage -= Mathf.RoundToInt(_targetState.armor.GetValue() * 0.7f);
        }
        else
        {
            totalDamage -= _targetState.armor.GetValue(); // 物理伤害时减去目标护甲值
        }

        totalDamage = Mathf.Max(totalDamage, 1); // 确保伤害不小于1
        return totalDamage;
    }

    public virtual void OnEvasion()
    {

    }

    // 检查目标是否闪避
    protected bool CheckTargetEvasion(CharacterStats _targetState)
    {
        int totalEvasion = CalculateTotalEvasion(_targetState);

        // 感电状态下增加10%闪避概率
        if (isShocked)
        {
            totalEvasion += 10;
        }

        if (Random.Range(0, 100) < totalEvasion) // 闪避概率
        {
            _targetState.OnEvasion();
            return true;
        }

        return false;
    }

    // 检查目标魔法抗性并计算最终魔法伤害
    private int CheckTargetResistence(CharacterStats _targetStats, int totalMagicalDamage)
    {
        totalMagicalDamage -= _targetStats.magicResistence.GetValue();
        totalMagicalDamage = Mathf.Max(totalMagicalDamage, 1);
        return totalMagicalDamage;
    }

    // 计算暴击伤害
    protected int CalculateCriticalDamage(int _damage)
    {
        // 暴擊傷害應該是基礎傷害的倍數，而不是直接乘以暴擊傷害值
        // critPower.GetValue() 返回的是百分比，需要除以100
        float critMultiplier = critPower.GetValue() / 100f;
        float critDamage = _damage * critMultiplier;

        return Mathf.RoundToInt(critDamage);
    }

    // 计算最大生命值
    public int CalculateMaxHealth()
    {
        // 每点体力+10点最大生命
        return maxHealth.GetValue() + vitality.GetValue() * 10;
    }

    // 计算魔法伤害
    public int CalculatetotalMagicDamage(int _damage)
    {
        // 每点智力+5点魔法伤害
        return _damage + intelligence.GetValue() * 5;
    }

    private static int CalculateTotalEvasion(CharacterStats _targetState)
    {
        int totalEvasion = 0;

        // 每点敏捷+1点闪避
        totalEvasion = _targetState.evasion.GetValue() + _targetState.agility.GetValue() * 1;

        return totalEvasion;
    }

    protected int CalculateBaseDamage()
    {
        // 每点力量+3点攻击
        return damage.GetValue() + strength.GetValue() * 3; 
    }

    #endregion

    public Stats GetStat(StatType _buffType)
    {
        switch (_buffType)
        {
            case StatType.strength:
                return strength;
            case StatType.agility:
                return agility;
            case StatType.intelligence:
                return intelligence;
            case StatType.vitality:
                return vitality;
            case StatType.damage:
                return damage;
            case StatType.critChance:
                return critChance;
            case StatType.critPower:
                return critPower;
            case StatType.health:
                return maxHealth;
            case StatType.armor:
                return armor;
            case StatType.evasion:
                return evasion;
            case StatType.magicRes:
                return magicResistence;
            case StatType.fireDamage:
                return fireDamage;
            case StatType.iceDamage:
                return iceDamage;
            case StatType.lightingDamage:
                return lightingDamgae;
            default:
                Debug.LogError("Invalid buff type specified.");
                return null;
        }
    }
}


