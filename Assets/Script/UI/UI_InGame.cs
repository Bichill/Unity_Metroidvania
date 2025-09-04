using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UI_InGame : MonoBehaviour
{
    [SerializeField] private Slider slider; // 血条滑动条
    [SerializeField] private PlayerStats playerStats; // 玩家统计信息

    [SerializeField] private Image dashImage;
    [SerializeField] private Image parryImage;
    [SerializeField] private Image crystalImage;
    [SerializeField] private Image swordImage;
    [SerializeField] private Image blackholeImage;
    [SerializeField] private Image flaskImage;

    [Space]
    [SerializeField] private GameObject dashObject;
    [SerializeField] private GameObject parryObject;
    [SerializeField] private GameObject crystalObject;
    [SerializeField] private GameObject swordObject;
    [SerializeField] private GameObject blackholeObject;
    [SerializeField] private GameObject flaskObject;

    private SkillManager skills;

    [Header("Souls info")]
    [SerializeField] private TextMeshProUGUI currentSkillPoint;
    [SerializeField] private float skillpointAmount;
    [SerializeField] private float increaseRate = 100f;

    private void Start()
    {
        skills = SkillManager.instance;
        
        if (playerStats != null)
        {
            playerStats.onHealthChanged += UpdateHealthUI;
        }

        dashObject.SetActive(false);
        parryObject.SetActive(false);
        crystalObject.SetActive(false);
        swordObject.SetActive(false);
        blackholeObject.SetActive(false);
        flaskObject.SetActive(false);
    }

    private void Update()
    {
        UpdateSkillpoint();
        CheckUIInput();

        CheckCooldownOf(dashImage, skills.dash.cooldown, skills.dash.cooldownTimer);
        CheckCooldownOf(parryImage, skills.parry.cooldown, skills.parry.cooldownTimer);
        CheckCooldownOf(crystalImage, skills.crystal.cooldown, skills.crystal.cooldownTimer);
        CheckCooldownOf(swordImage, skills.sword.cooldown, skills.sword.cooldownTimer);
        CheckCooldownOf(blackholeImage, skills.blackHole.cooldown, skills.blackHole.cooldownTimer);

        //药水做特殊处理
        if (flaskImage.fillAmount > 0 && flaskImage != null)
        {
            flaskImage.fillAmount -= 1 / Inventory.instance.flaskCooldown * Time.deltaTime;
        }
    }

    private void UpdateSkillpoint()
    {
        if (skillpointAmount < PlayerManager.instance.GetCurrencyAmount())
            skillpointAmount += Time.deltaTime * increaseRate;
        else
            skillpointAmount = PlayerManager.instance.GetCurrencyAmount();

        currentSkillPoint.text = ((int)skillpointAmount).ToString();
    }

    private void CheckUIInput()
    {
        if (skills.dash.dashUnlocked)
        {
            dashObject.SetActive(true);
            if (Input.GetKeyDown(KeyCode.L))
                SetCooldownOf(dashImage);
        }

        if (skills.parry.parryUnlocked)
        {
            parryObject.SetActive(true);
            if (Input.GetKeyDown(KeyCode.H))
                SetCooldownOf(parryImage);
        }

        if (skills.crystal.crystalUnlocked)
        {
            crystalObject.SetActive(true);
            if (Input.GetKeyDown(KeyCode.F))
                SetCooldownOf(crystalImage);
        }

        if (skills.sword.swordUnlocked)
        {
            swordObject.SetActive(true);
            if (Input.GetKeyUp(KeyCode.Mouse1))
                SetCooldownOf(swordImage);
        }
        if (skills.blackHole.blackHoleUnlocked)
        {
            blackholeObject.SetActive(true);
            if (Input.GetKeyDown(KeyCode.R))
                SetCooldownOf(blackholeImage);
        }
        if (Inventory.instance.GetEquipment(EquipmentType.Flask) != null)
        {
            flaskObject.SetActive(true);
            if (Input.GetKeyDown(KeyCode.Q))
                SetCooldownOf(flaskImage);
        }
    }

    private void UpdateHealthUI()
    {
        if (playerStats == null || slider == null) return;

        if (playerStats.currentHealth <= 0)
        {
            gameObject.SetActive(false);
            return;
        }

        int maxHealth = playerStats.CalculateMaxHealth();
        if (maxHealth <= 0) return; // 防止除以零

        slider.maxValue = maxHealth; 
        slider.value = playerStats.currentHealth;

        // 確保血量不會超過最大值
        if (playerStats.currentHealth > maxHealth)
        {
            playerStats.currentHealth = maxHealth;
        }
    }

    private void SetCooldownOf(Image _image)
    {
        if (_image.fillAmount <= 0)
        {
            _image.fillAmount = 1;
        }
    }

    private void CheckCooldownOf(Image _image,float _cooldown,float _cooldownTimer)
    {
        if (_image.fillAmount > 0)
        {
            _image.fillAmount = _cooldownTimer / _cooldown; 
        }
    }
}
