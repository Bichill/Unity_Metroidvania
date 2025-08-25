using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_SkillTreeSlot : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler,ISaveManager
{
    private UI ui;
    private Image skillImage;

    [SerializeField] private int skillPoint;
    [SerializeField] private string skillName;
    [TextArea]
    [SerializeField] private string skillDescription;
    [SerializeField] private Color lockedSkillColor;
    
    public bool unlocked;

    [SerializeField] private UI_SkillTreeSlot[] shouldBeUnlocked;
    [SerializeField] private UI_SkillTreeSlot[] shouldBeLocked;


    private void OnValidate()
    {
        gameObject.name = "SkillTreeSlot_UI - " + skillName;
    }

    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(() => UnlockSkillSlot());
    }

    private void Start()
    {
        skillImage = GetComponent<Image>();
        ui = GetComponentInParent<UI>();

        // 根據當前解鎖狀態設置顏色
        UpdateSkillVisual();
    }

    public void UnlockSkillSlot()
    {
        if (unlocked)
        {
            Debug.Log("You have already unlocked this skill!!");
            return;
        }

        if (!PlayerManager.instance.HaveEnoughSkillPoint(skillPoint)) 
            return;

        for (int i = 0; i < shouldBeUnlocked.Length; i++)
        {
            if (shouldBeUnlocked[i].unlocked == false)
            {
                Debug.Log("Can't unlock skill");
                return;
            }
        }

        for (int i = 0; i < shouldBeLocked.Length; i++)
        {
            if (shouldBeLocked[i].unlocked == true)
            {
                Debug.Log("Can't unlock skill");
                return;
            }
        }

        unlocked = true;
        UpdateSkillVisual();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        ui.skillToolTip.ShowToolTip(skillDescription,skillName,skillPoint);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ui.skillToolTip.HideToolTip();
    }

    private void UpdateSkillVisual()
    {
        if (skillImage == null)
            skillImage = GetComponent<Image>();

        skillImage.color = unlocked ? Color.white : lockedSkillColor;
    }

    public void LoadData(GameData _data)
    {
        if (_data.skillTree.TryGetValue(skillName, out bool value))
        {
            unlocked = value;
            // 更新UI視覺效果
            UpdateSkillVisual();
        }
    }

    //因为此脚本是槽，因此有很多，每个槽保存一个数据即可，不需要foreach
    public void SaveData(ref GameData _data)
    {
        //设计if、else是为了避免重复添加，即一键多值的情况出现
        if (_data.skillTree.TryGetValue(skillName, out bool value))
        {
            //所以要先清除再添加
            _data.skillTree.Remove(skillName);
            _data.skillTree.Add(skillName, unlocked);
        }
        else
        {
            _data.skillTree.Add(skillName, unlocked);
        }
    }
}
