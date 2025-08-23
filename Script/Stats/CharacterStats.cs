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

//���յĲ�ͬ״̬
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
    public bool isDead { get; private set; }// �Ƿ�����,�����ظ�����
    public bool isVulnerable;// �Ƿ�����

    [Header("Major stats")]
    public Stats strength; // ������ÿ��+3���� +1��������
    public Stats agility; // ���ݣ�ÿ��+1���� +1��������
    public Stats intelligence; // ������ÿ��+5��ǿ +1ħ��
    public Stats vitality; // ������ÿ��+10����

    [Header("Offensive stats")]
    public Stats damage;
    public Stats critChance; // ������
    public Stats critPower; // �����˺�

    [Header("Defensive stats")]
    public Stats maxHealth;
    public Stats armor; // ���ף����������˺�
    public Stats evasion; // ���ܣ����������˺�
    public Stats magicResistence; // ħ�����ԣ�����ħ���˺�

    [Header("Magic Ignite")]
    public Stats fireDamage;
    public bool isIgnited; // �Ƿ��ȼ�����������˺���
    private float ignitedTimer;
    private float igniteDamageCooldown = .2f;
    public List<IgniteStatus> igniteList = new List<IgniteStatus>();//�������
    [SerializeField] private GameObject igniteStampPrefab;//����ӡ��

    //�ϳ���������
    //private float igniteDamageTimer;
    //private int igniteDamage;

    [Header("Magic Chill")]
    public bool isChilled; // �Ƿ���������٣�
    public Stats iceDamage;
    private float chilledTimer;

    [Header("Magic Shock")]
    public Stats lightingDamgae;
    public bool isShocked; // �Ƿ�е磨���ˣ�
    [SerializeField] private GameObject shockStrikePrefab;
    private int shockDamage;
    private float shockedTimer;

    [Header("Ailments Timer")]
    public float ailmentsDuration = 4; // �쳣״̬����ʱ��

    [Header("Other")]
    public int currentHealth;

    public System.Action onHealthChanged;


    protected virtual void Start()
    {
        critPower.SetDefaultValue(150); // 150% �������
        
        // �_��Ѫ�����_��ʼ��
        currentHealth = CalculateMaxHealth();
        
        // �|�lѪ��׃���¼�
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
        // ��ʼ����״̬Э��
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

        // �_�����Ƴ��޸���֮ǰ�z�錦���Ƿ���Ȼ����
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
            //���㱩��
            totalDamage = CalculateCriticalDamage(totalDamage);
        }

        totalDamage = CheckTargetArmor(_targetStats, totalDamage);

        _targetStats.TakeDamage(totalDamage);
    }

    #region Magical Damage And Ailments

    // ����ÿ0.2�봦���߼�
    private void BurningTick()
    {
        if (currentHealth <= 0)
        {
            igniteList.Clear();
            isIgnited = false; // ����ʱ�Ƴ��쳣
            CancelInvoke(nameof(BurningTick)); // ����ʱֹͣ����
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


    // ����ħ���˺����쳣״̬
    public virtual void DoMagicalDamage(CharacterStats _targetStats)
    {

        //---------------------------------���㵥���˺�------------------------------
        int _fireDamage = fireDamage.GetValue();
        int _iceDamage = iceDamage.GetValue();
        int _lightningDamage = lightingDamgae.GetValue();

        int totalMagicDamage = CalculatetotalMagicDamage(Mathf.Max(_fireDamage, _iceDamage, _lightningDamage));

        totalMagicDamage = CheckTargetResistence(_targetStats, totalMagicDamage);

        _targetStats.TakeDamage(totalMagicDamage);

        //----------------------------------Debuff---------------------------------
        // Ԫ���˺�����Debuff
        if (Mathf.Max(_fireDamage, _iceDamage, _lightningDamage) <= 0)
        {
            return;
        }

        AttemptToApplyAliments(_targetStats, _fireDamage, _iceDamage, _lightningDamage);
    }

    // ����Ӧ���쳣״̬
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

        // Ԫ���˺�����Debuff
        if (canApplyIgnite)
        {
            // ��ȼ�˺�ΪԪ���˺���0.15-0.25��
            float igniteMultiplier = Random.Range(0.15f, 0.25f);
            int igniteDps = (int)(Mathf.Max(_fireDamage * igniteMultiplier, 1));
            _targetStats.ApplyIgnite(ailmentsDuration, igniteDps);
        }

        if (canApplyShock)
        {
            //�����˺�ΪԪ���˺���0.5��
            _targetStats.SetupShockDamage(Mathf.Max(Mathf.RoundToInt(_lightningDamage * 0.5f), 1));
        }
        _targetStats.ApplyAilments(canApplyIgnite, canApplyChill, canApplyShock, _targetStats);
        return;
    }

    // �쳣״̬��ʱ��
    private void AilmentsTimer()
    {
        ignitedTimer -= Time.deltaTime;
        chilledTimer -= Time.deltaTime;
        shockedTimer -= Time.deltaTime;

        //igniteDamageTimer -= Time.deltaTime;

        // �����쳣����
        if (ignitedTimer < 0)
        {
            isIgnited = false;
        }
        // �����쳣����
        if (chilledTimer < 0)
        {
            isChilled = false;
        }
        // �е��쳣����
        if (shockedTimer < 0)
        {
            isShocked = false;
        }
    }

    // Ӧ���쳣״̬
    public void ApplyAilments(bool _ignite, bool _chill, bool _shock, CharacterStats _target)
    {
        // ��ȼ
        if (_ignite)
        {
            isIgnited = _ignite;
            ignitedTimer = ailmentsDuration;
            fx.IgniteFxFor(ailmentsDuration);
            //����������������
            TrySwapnIgniteStamp(_target, igniteStampPrefab);
        }
        // ����
        if (_chill)
        {
            isChilled = _chill;
            chilledTimer = ailmentsDuration;

            float slowPercentage = .4f;

            GetComponent<Entity>().SlowEntityBy(slowPercentage, ailmentsDuration);
            fx.ChillFxFor(ailmentsDuration);
        }
        // �е�
        if (_shock)
        {
            if (!isShocked)
            {
                ApplyShock(_shock);
            }
            else
            {
                // ���׵�һ��Ŀ��
                GameObject ShockStrike = Instantiate(shockStrikePrefab, transform.position, Quaternion.identity);
                Debug.Log("Thunder shock Damage: " + shockDamage);
                ShockStrike.GetComponent<ThunderStrike_Controller>().Setup(shockDamage, _target);

                //-----------------------------------------------ͨ���е��׻��ڶ�������---------------------------------------------------

                //���������򲻽��ие�
                if (this.GetComponent<Player>() != null)
                    return;
                
                ShockStrikeClosestTarget(_target);
            }
        }
    }

    // Ӧ�øе�״̬
    public void ApplyShock(bool _shock)
    {
        if (isShocked) return;

        isShocked = _shock;
        shockedTimer = ailmentsDuration;

        fx.ShockFxFor(ailmentsDuration);
    }

    /* �˺����ѷϳ�
    private void ApplyIgniteDamge()
    {
        if (igniteDamageTimer < 0)
        {
            Debug.Log("Turn back fire : " + igniteDamage);
            DecreaseHealthBy(igniteDamage); // ���������˺�
            if (currentHealth <= 0)
            {
                isIgnited = false; // ����ʱ�Ƴ��쳣
                Die();
            }
            igniteDamageTimer = igniteDamageCooldown;
        }
    }
    */

    // ͨ���е��׻������Ŀ��
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

    //������ղ���
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
        
        // �O���cȼ��B
        isIgnited = true;
        ignitedTimer = ailmentsDuration;
        
        // ���ɻ���y��
        TrySwapnIgniteStamp(this, null);
    }

    //���ɻ�������
    public void TrySwapnIgniteStamp(CharacterStats _target,GameObject _igniteStamp)
    {
        //���Ŀ���ʱ�Ƿ����л�������
        var existing = _target.GetComponentInChildren<IgniteStamp_Controller>();
        if (existing != null)
        {
            return;
        }

        GameObject ignitestamp = Instantiate(igniteStampPrefab);
        ignitestamp.GetComponent<IgniteStamp_Controller>().Setup(_target);
        ignitestamp.transform.SetParent(_target.transform); //��ΪĿ��������壬�´��ܹ���existing��⵽
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

    // �Զ���ָ��Ļ�Ѫ����
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

    // �Զ����˺��Ŀ�Ѫ���������|�l�o��Ч����
    protected virtual void DecreaseHealthBy(int _damage)
    {
        if (isVulnerable)
        {
            _damage = Mathf.RoundToInt(_damage * 1.5f); // ����״̬���˺�����150%
        }

        currentHealth -= _damage;

        // �_��Ѫ���������0
        if (currentHealth < 0)
        {
            currentHealth = 0;
        }

        if (onHealthChanged != null)
        {
            onHealthChanged();
        }
    }

    /* �˺����ѷϳ�
    public void SetupIgniteDamage(int _damage)
    {
        igniteDamage = _damage;
    }
    */

    // �����׻��˺�
    public void SetupShockDamage(int _damage)
    {
        shockDamage = _damage;
    }

    protected virtual void Die()
    {
       ChangeDeadToTrue();
    }

    #region Calculate Stats

    // ���Ŀ���Ƿ񱩻�
    protected bool CheckTargetCrit()
    {
        int totalCriticalChance = critChance.GetValue();

        if (Random.Range(0, 100) <= totalCriticalChance)
            return true;
        else return false;
    }

    // ���Ŀ�껤�ײ����������˺�
    protected int CheckTargetArmor(CharacterStats _targetState, int totalDamage)
    {
        // ���Ŀ�괦�ڶ���״̬�����ٻ���ֵ
        if (_targetState.isChilled)
        {
            totalDamage -= Mathf.RoundToInt(_targetState.armor.GetValue() * 0.7f);
        }
        else
        {
            totalDamage -= _targetState.armor.GetValue(); // �����˺�ʱ��ȥĿ�껤��ֵ
        }

        totalDamage = Mathf.Max(totalDamage, 1); // ȷ���˺���С��1
        return totalDamage;
    }

    public virtual void OnEvasion()
    {

    }

    // ���Ŀ���Ƿ�����
    protected bool CheckTargetEvasion(CharacterStats _targetState)
    {
        int totalEvasion = CalculateTotalEvasion(_targetState);

        // �е�״̬������10%���ܸ���
        if (isShocked)
        {
            totalEvasion += 10;
        }

        if (Random.Range(0, 100) < totalEvasion) // ���ܸ���
        {
            _targetState.OnEvasion();
            return true;
        }

        return false;
    }

    // ���Ŀ��ħ�����Բ���������ħ���˺�
    private int CheckTargetResistence(CharacterStats _targetStats, int totalMagicalDamage)
    {
        totalMagicalDamage -= _targetStats.magicResistence.GetValue();
        totalMagicalDamage = Mathf.Max(totalMagicalDamage, 1);
        return totalMagicalDamage;
    }

    // ���㱩���˺�
    protected int CalculateCriticalDamage(int _damage)
    {
        // ���������ԓ�ǻ��A�����ı�����������ֱ�ӳ��Ա������ֵ
        // critPower.GetValue() ���ص��ǰٷֱȣ���Ҫ����100
        float critMultiplier = critPower.GetValue() / 100f;
        float critDamage = _damage * critMultiplier;

        return Mathf.RoundToInt(critDamage);
    }

    // �����������ֵ
    public int CalculateMaxHealth()
    {
        // ÿ������+10���������
        return maxHealth.GetValue() + vitality.GetValue() * 10;
    }

    // ����ħ���˺�
    public int CalculatetotalMagicDamage(int _damage)
    {
        // ÿ������+5��ħ���˺�
        return _damage + intelligence.GetValue() * 5;
    }

    private static int CalculateTotalEvasion(CharacterStats _targetState)
    {
        int totalEvasion = 0;

        // ÿ������+1������
        totalEvasion = _targetState.evasion.GetValue() + _targetState.agility.GetValue() * 1;

        return totalEvasion;
    }

    protected int CalculateBaseDamage()
    {
        // ÿ������+3�㹥��
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


