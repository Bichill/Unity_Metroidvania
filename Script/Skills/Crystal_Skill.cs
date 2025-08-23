using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Crystal_Skill : Skill
{
    [SerializeField] private float crystalDuration;
    [SerializeField] private GameObject crystalPrefab;
    private GameObject currentCrystal;// 这个变量现在只代表通过F键创建的水晶
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
    public bool crystalMultiUnlocked { get; private set; }//水晶多重的开关
    public int amountOfCrystal;//水晶多重最多存储多少颗水晶
    public float multiStackCooldown;//水晶装填时间
    [SerializeField] private List<GameObject> crystalLeft = new List<GameObject>();

    protected override void Start()
    {
        base.Start();

        crystalUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockCrystal);
        crystalExplosiveUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockCrystalExplosive);
        crystalMovingUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockCrystalMoving);
        crystalMirageUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockCrystalMirage);
        crystalMultiUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockCrystalMulti);
    }

    protected override void Update()
    {
        base.Update();
        
        // 處理水晶裝填
        if (crystalMultiUnlocked)
        {
            RefilCrystal();
        }
    }

    public override bool CanUseSkill()
    {
        // 如果是多重水晶模式，使用特殊的冷卻邏輯
        if (crystalMultiUnlocked)
        {
            // 多重水晶模式：只要有水晶就可以發射，沒有冷卻限制
            if (crystalLeft.Count > 0)
            {
                UseSkill();
                return true;
            }
            return false;
        }
        
        // 普通模式：使用基類的冷卻邏輯
        return base.CanUseSkill();
    }

    //技能解锁区域
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

    //技能实现区域
    #region Skill
    public override void UseSkill()
    {
        base.UseSkill();

        if (CanUseMultiCrystal()) return;//水晶多重

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

            if (crystalMirageUnlocked)//水晶替换为幻影
            {
                SkillManager.instance.clone.CreatClone(currentCrystal.transform, Vector3.zero);
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
            // 这是作为克隆替身出现的水晶（例如，由Clone_Skill调用或Crystal_Skill自身设置为替身模式时创建）。
            // 它不应被换位，并且颜色为默认颜色。
            newCrystal.GetComponent<SpriteRenderer>().color = defaultCrystalColor;
            // 重要的：不要将它赋值给currentCrystal，这样它就不会被用于换位操作。
            //crystalMovingUnlocked = true;
            newCrystalScript.SetupCrystal(crystalDuration, crystalExplosiveUnlocked, true, moveSpeed, randomTargetRadius);
            newCrystalScript.ChooseRandomTarget();

        }
        else
        {
            currentCrystal = newCrystal;
            currentCrystal.GetComponent<SpriteRenderer>().color = currentCrystalColor;
            //crystalMovingUnlocked = false;//直接创建的水晶不位移
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
                //水晶从列表中取出并且生成预制体
                GameObject crystalToSpawn = crystalLeft[crystalLeft.Count - 1];
                GameObject newCrystal = Instantiate(crystalToSpawn, player.transform.position, Quaternion.identity);
                //水晶被取出后就要从列表删除
                crystalLeft.Remove(crystalToSpawn);

                //多重水晶使用默认颜色
                newCrystal.GetComponent<SpriteRenderer>().color = defaultCrystalColor;

                var crystalScript = newCrystal.GetComponent<Crystal_Skill_Controller>();
                crystalScript.SetupCrystal(crystalDuration, crystalExplosiveUnlocked, crystalMovingUnlocked, moveSpeed, randomTargetRadius);
                crystalScript.ChooseRandomTarget();

                return true; // 成功發射水晶
            }
            return false; // 沒有水晶可發射
        }
        return false;// 技能未解锁
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
