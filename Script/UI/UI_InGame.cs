using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using TMPro;
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
    [SerializeField] private TextMeshProUGUI currentSkillPoint;

    [Space]
    [SerializeField] private GameObject dashObject;
    [SerializeField] private GameObject parryObject;
    [SerializeField] private GameObject crystalObject;
    [SerializeField] private GameObject swordObject;
    [SerializeField] private GameObject blackholeObject;
    [SerializeField] private GameObject flaskObject;



    private SkillManager skills;

    private void Start()
    {
        skills = SkillManager.instance;
        CheckSkillPoint();
        
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
        CheckUIInput();
        CheckSkillPoint();

        CheckCooldownOf(dashImage, skills.dash.cooldown);
        CheckCooldownOf(parryImage, skills.parry.cooldown);
        CheckCooldownOf(crystalImage, skills.crystal.cooldown);
        CheckCooldownOf(swordImage, skills.sword.cooldown);
        CheckCooldownOf(blackholeImage, skills.blackHole.cooldown);
        CheckCooldownOf(flaskImage, Inventory.instance.flaskCooldown);
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
            if (Input.GetKeyDown(KeyCode.Mouse1))
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

    private void CheckCooldownOf(Image _image,float _cooldown)
    {
        if (_image.fillAmount > 0)
        {
            _image.fillAmount -= 1 / _cooldown * Time.deltaTime; 
        }
    }

    private void CheckSkillPoint()
    {
        currentSkillPoint.text = PlayerManager.instance.GetCurrencyAmount().ToString();
    }
}
