using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Crystal_Skill : Skill
{
    [SerializeField] private float crystalDuration;
    [SerializeField] private GameObject crystalPrefab;
    private GameObject currentCrystal;// ��ǰˮ������
    [SerializeField] Color defaultCrystalColor;
    [SerializeField] Color currentCrystalColor;

    [Header("Simple crystal")]
    [SerializeField] private UI_SkillTreeSlot crystalUnlockButton;
    public bool crystalUnlocked { get; private set; }

    [Header("Explosive crystal")]
    [SerializeField] private UI_SkillTreeSlot crystalExplosiveUnlockButton;
    public bool crystalExplosiveUnlocked { get; private set; }

    [Header("Moving crystal")]
    [SerializeField] private UI_SkillTreeSlot crystalMovingUnlockButton;
    public bool crystalMovingUnlocked { get; private set; }
    [SerializeField] private float moveSpeed;
    [SerializeField] private float randomTargetRadius;

    [Header("Crystal mirage")]
    [SerializeField] private UI_SkillTreeSlot crystalMirageUnlockButton;
    public bool crystalMirageUnlocked { get; private set; }

    [Header("Multi stacking crystal")]
    [SerializeField] private UI_SkillTreeSlot crystalMultiUnlockButton;
    public bool crystalMultiUnlocked { get; private set; }//����ˮ������״̬
    public int amountOfCrystal;//����ˮ������
    public float multiStackCooldown;//����ˮ����ȴʱ��
    [SerializeField] private List<GameObject> crystalLeft = new List<GameObject>();

    protected override void Start()
    {
        base.Start();
        // CheckUnlocked() ���ڻ�� Start() ���Ԅ��{��
        crystalUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockCrystal);

        crystalUnlockButton.GetComponent<Button>()?.onClick.AddListener(UnlockCrystal);
        crystalExplosiveUnlockButton.GetComponent<Button>()?.onClick.AddListener(UnlockCrystalExplosive);
        crystalMovingUnlockButton.GetComponent<Button>()?.onClick.AddListener(UnlockCrystalMoving);
        crystalMirageUnlockButton.GetComponent<Button>()?.onClick.AddListener(UnlockCrystalMirage);
        crystalMultiUnlockButton.GetComponent<Button>()?.onClick.AddListener(UnlockCrystalMulti);
    }

    protected override void Update()
    {
        base.Update();
        
        // ������ڶ���ˮ��ģʽ����ˢ��ˮ��
        if (crystalMultiUnlocked)
        {
            RefilCrystal();
        }
    }

    public override bool CanUseSkill()
    {
        // �z���Ƿ��ѽ��i
        if (!crystalUnlocked)
        {
            Debug.Log("Crystal skill not unlocked!");
            return false;
        }

        // ����Ƕ���ˮ��ģʽ��ʹ���������ȴ����
        if (crystalMultiUnlocked)
        {
            // ����ˮ��ģʽ��ֻҪ��ˮ���Ϳ��Է��䣬û����ȴ����
            if (crystalLeft.Count > 0)
            {
                UseSkill();
                return true;
            }
            return false;
        }
        
        // ��ͨģʽ��ʹ�û�������ȴ����
        if (cooldownTimer < 0)
        {
            UseSkill();
            cooldownTimer = cooldown;
            return true;
        }
    
        return false;
    }

    //���ܽ�������
    #region Unlock Skill
    public void UnlockCrystal()
    {
        if (crystalUnlockButton.unlocked)
            crystalUnlocked = true;
    }

    public void UnlockCrystalExplosive()
    {
        if(crystalExplosiveUnlockButton.unlocked)
            crystalExplosiveUnlocked = true;
    }

    public void UnlockCrystalMirage()
    {
        if (crystalMirageUnlockButton.unlocked)
            crystalMirageUnlocked = true;
    }

    public void UnlockCrystalMoving()
    {
        if(crystalMovingUnlockButton.unlocked)
            crystalMovingUnlocked = true;
    }

    public void UnlockCrystalMulti()
    {
        if(crystalMultiUnlockButton.unlocked)
            crystalMultiUnlocked = true;
    }

    #endregion

    protected override void CheckUnlocked()
    {
        UnlockCrystal();
        UnlockCrystalExplosive();
        UnlockCrystalMirage();
        UnlockCrystalMoving();
        UnlockCrystalMulti();
    }

    //����ʵ������
    #region Skill
    public override void UseSkill()
    {
        base.UseSkill();

        if (CanUseMultiCrystal()) return;//ˮ������

        if (currentCrystal == null)
        {
            CreateCrystal(crystalMovingUnlocked);
        }
        else
        {
            if (crystalMovingUnlocked) return;
            Vector2 playerPos = player.transform.position;
            player.transform.position = currentCrystal.transform.position;
            currentCrystal.transform.position = playerPos;

            if (crystalMirageUnlocked)//ˮ���滻�ɻ�Ӱ
            {
                if (SkillManager.instance != null && SkillManager.instance.clone != null)
            {
                SkillManager.instance.clone.CreatClone(currentCrystal.transform, Vector3.zero);
                }
                Destroy(currentCrystal);
            }
            else
            {
                currentCrystal.GetComponent<Crystal_Skill_Controller>()?.CrystalDestroyLogic();
            }
        }
    }

    public void CreateCrystal(bool _isTrackCrystal)
    {
        GameObject newCrystal = Instantiate(crystalPrefab, player.transform.position, Quaternion.identity);
        Crystal_Skill_Controller newCrystalScript = newCrystal.GetComponent<Crystal_Skill_Controller>();
        
        if (_isTrackCrystal)
        {
            // ������Ϊ��¡������ֵ�ˮ�������磬��Clone_Skill���û�Crystal_Skill�Լ�����Ϊ����ģʽʱ������
            // ����Ӧ�ô��ͣ�������ɫΪĬ����ɫ
            newCrystal.GetComponent<SpriteRenderer>().color = defaultCrystalColor;
            // ��Ҫ�ģ���Ҫ������ֵ��currentCrystal���������Ͳ��ᱻ���ڴ��Ͳ���
            //crystalMovingUnlocked = true;
            newCrystalScript.SetupCrystal(crystalDuration, crystalExplosiveUnlocked, true, moveSpeed, randomTargetRadius);
            newCrystalScript.ChooseRandomTarget();

        }
        else
        {
            currentCrystal = newCrystal;
            currentCrystal.GetComponent<SpriteRenderer>().color = currentCrystalColor;
            //crystalMovingUnlocked = false;//ֱ�Ӵ�����ˮ����λ��
            newCrystalScript.SetupCrystal(crystalDuration, crystalExplosiveUnlocked, false, moveSpeed, randomTargetRadius);
        }

        
    }

    public void SetupCrystalTarget(GameObject crystal)
    {
        if (crystal == null) return;

        var crystalScript = crystal.GetComponent<Crystal_Skill_Controller>();
        if (crystalScript != null)
        {
            crystalScript.ChooseRandomTarget();
        }
    }

    private bool CanUseMultiCrystal()
    {
        if (crystalMultiUnlocked)
        {
            if (crystalLeft.Count > 0)
            {
                //ˮ�����б���ȡ����������Ԥ����
                GameObject crystalToSpawn = crystalLeft[crystalLeft.Count - 1];
                GameObject newCrystal = Instantiate(crystalToSpawn, player.transform.position, Quaternion.identity);
                //ˮ����ȡ�����Ҫ���б�ɾ��
                crystalLeft.Remove(crystalToSpawn);

                //����ˮ��ʹ��Ĭ����ɫ
                newCrystal.GetComponent<SpriteRenderer>().color = defaultCrystalColor;

                var crystalScript = newCrystal.GetComponent<Crystal_Skill_Controller>();
                crystalScript.SetupCrystal(crystalDuration, crystalExplosiveUnlocked, crystalMovingUnlocked, moveSpeed, randomTargetRadius);
                crystalScript.ChooseRandomTarget();

                return true; // �ɹ�����ˮ��
            }
            return false; // û��ˮ���ɷ���
        }
        return false;// ����δ����
    }

    private void RefilCrystal()
    {
        if (crystalLeft.Count >= amountOfCrystal)
            return;
            
        if (cooldownTimer <= 0)
        {
            crystalLeft.Add(crystalPrefab);
            cooldownTimer = multiStackCooldown;
        }
    }

    public int GetCurrentCrystalLeftAmount()
    {
        return crystalLeft.Count;
    }
    #endregion 


}
