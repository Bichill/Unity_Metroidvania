using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class Clone_Skill : Skill
{
    [Header("Clone info")]
    [SerializeField] private float attackMultiplier;//��ҹ�������
    [SerializeField] private GameObject clonePrefab;
    [SerializeField] private float cloneDuration;

    // ��¡����
    [Header("Attack clone")]
    [SerializeField] private UI_SkillTreeSlot attackCloneUnlockButton;
    public bool unlockAttackClone { get; private set; }
    [SerializeField] private float cloneAttackMultiplier;//��¡��������

    //�����й����ԵĿ�¡ �ܹ�ʹ��װ������
    [Header("More aggresive clone")]
    [SerializeField] private UI_SkillTreeSlot aggresiveCloneUnlockButton;
    public bool unlockAggresiveClone { get; private set; }
    [SerializeField] private float aggresiveCloneAttackMultiplier;//ǿ����¡��������

    //���ѿ�¡
    [Header("Multiple clone")]
    [SerializeField] private UI_SkillTreeSlot multipleCloneUnlockButton;
    public bool unlockMultipleClone { get; private set; }
    [SerializeField] private float multiCloneAttackMultiplier;//���ѿ�¡��������
    [SerializeField] private bool canDuplicateClone;
    [SerializeField] private float chanceToDuplicate;

    //��¡�滻ˮ��
    [Header("Crystal instead of clone")]
    [SerializeField] private UI_SkillTreeSlot insteadCloneUnlockButton;
    public bool unlockInsteadClone { get; private set; }
    [SerializeField] private bool crystalInsteadOfClone;

    protected override void Start()
    {
        base.Start();

        attackCloneUnlockButton.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(UnlockCloneAttack);
        aggresiveCloneUnlockButton.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(UnlockAggresiveClone);
        multipleCloneUnlockButton.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(UnlockMultipleClone);
        insteadCloneUnlockButton.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(UnlockInsteadClone);
    }

    #region Unlock clone

    private void UnlockCloneAttack()
    {
        if (attackCloneUnlockButton.unlocked)
        {
            unlockAttackClone = true;
            attackMultiplier = cloneAttackMultiplier;
        }
    }

    private void UnlockAggresiveClone()
    {
        if (aggresiveCloneUnlockButton.unlocked)
        {
            unlockAggresiveClone = true;
            attackMultiplier = aggresiveCloneAttackMultiplier;
        }
    }

    private void UnlockMultipleClone()
    {
        if (multipleCloneUnlockButton.unlocked)
        {
            unlockMultipleClone = true;
            attackMultiplier = multiCloneAttackMultiplier;
            canDuplicateClone = true;
        }
    }

    private void UnlockInsteadClone()
    {
        if (insteadCloneUnlockButton.unlocked)
        {
            unlockInsteadClone = true;
            crystalInsteadOfClone = true;
        }
    }

    #endregion

    public void CreatClone(Transform _clonePosition , Vector3 _offset)
    {
        if (crystalInsteadOfClone)
        {
            SkillManager.instance.crystal.CreateCrystal(true);//�滻��¡����ֵ�ˮ���޷���λ
            return;
        }
        
        GameObject newClone = Instantiate(clonePrefab);
        newClone.GetComponent<Clone_Skill_Controller>().SetupClone(_clonePosition, cloneDuration, _offset
            , FindClosestEnemy(_clonePosition.transform), canDuplicateClone, chanceToDuplicate,attackMultiplier);
    } 

    public void CreateCloneWithDelay(Transform _enemyTransform)
    {
        StartCoroutine(CloneDelayCorotine(_enemyTransform, new Vector3(2 * player.facingDir, 0)));
    }

    private IEnumerator CloneDelayCorotine(Transform _transform,Vector3 _offset)
    {
        yield return new WaitForSeconds(.2f);
            CreatClone(_transform,_offset);
    }
}
