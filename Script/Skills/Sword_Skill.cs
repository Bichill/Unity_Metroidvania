using System;
using UnityEngine;
using UnityEngine.UI;

public enum SwordType
{
    Regular,
    Bounce,
    Pierce,
    Spin
}

public class Sword_Skill : Skill
{
    public SwordType swordType = SwordType.Regular;

    [Header("Skill info")]
    [SerializeField] private UI_SkillTreeSlot unlockSwordButton;
    public bool swordUnlocked { get; private set;}
    [SerializeField] private GameObject swordPrefab;
    [SerializeField] private Vector2 launchForce;
    [SerializeField] private float swordGravity;
    [SerializeField] private float freezeTimeDuration;
    [SerializeField] private float vulnerableDuration;
    [SerializeField] private float returnSpeed;
    [SerializeField] private float maxSwordReturnDistance;

    [Header("Bounce info")]
    [SerializeField] private UI_SkillTreeSlot unlockBounceButton;
    [SerializeField] private int bounceAmount;
    [SerializeField] private float bounceSpeed;
    [SerializeField] private float bounceGravity;

    [Header("Peirce info")]
    [SerializeField] private UI_SkillTreeSlot unlockPierceButton;
    [SerializeField] private int pierceAmount;
    [SerializeField] private float pierceGravity;


    [Header("Spin info")]
    [SerializeField] private UI_SkillTreeSlot unlockSpinButton;
    [SerializeField] private float hitCooldown ;
    [SerializeField] private float maxTravelDistance ;
    [SerializeField] private float spinDuration ;
    [SerializeField] private float spinGravity;

    [Header("Passive info")]
    [SerializeField] private UI_SkillTreeSlot unlockTimeStopButton;
    public bool timeStopUnlocked { get; private set; }
    [SerializeField] private UI_SkillTreeSlot unlockVulnerableButton;
    public bool vulnerableUnlocked { get; private set; }


    private Vector2 finalDir;

    [Header("Aim dots")]
    [SerializeField] private int numberOfDots;
    [SerializeField] private float spaceBetweenDots;
    [SerializeField] private GameObject dotPrefab;
    [SerializeField] private Transform dotsParent;

    private GameObject[] dots;

    protected override void Start()
    {
        base.Start();

        GenereateDots();
        SetupGravity();

        unlockSwordButton.GetComponent<Button>().onClick.AddListener(UnlockSword);
        unlockBounceButton.GetComponent<Button>().onClick.AddListener(UnlockBounce);
        unlockPierceButton.GetComponent<Button>().onClick.AddListener(UnlockPierce);
        unlockSpinButton.GetComponent<Button>().onClick.AddListener(UnlockSpin);
        unlockTimeStopButton.GetComponent<Button>().onClick.AddListener(UnlockTimeStop);
        unlockVulnerableButton.GetComponent<Button>().onClick.AddListener(UnlockVulnerable);
    }

    private void SetupGravity()
    {
        if (swordType == SwordType.Bounce)
        {
            swordGravity = bounceGravity;
        }
        else if (swordType == SwordType.Pierce)
        {
            swordGravity = pierceGravity;
        }
        else if (swordType == SwordType.Spin)
        {
            swordGravity = spinGravity;
        }
    }

    protected override void Update()
    {
        if (Input.GetKeyUp(KeyCode.Mouse1))
        {
            finalDir = new Vector2(AimDirection().normalized.x * launchForce.x, AimDirection().normalized.y * launchForce.y);
        }

        if (Input.GetKey(KeyCode.Mouse1))
        {
            for (int i = 0; i < dots.Length; i++)
            {
                dots[i].transform.position = DotsPosition(i * spaceBetweenDots);
            }
        }
    }

    #region Unlock region

    private void UnlockSword()
    {
        if (unlockSwordButton.unlocked)
        {
            swordUnlocked = true;
            swordType = SwordType.Regular;
        }
    }

    private void UnlockBounce()
    {
        if (unlockBounceButton.unlocked)
        {
            swordUnlocked = true;
            swordType = SwordType.Bounce;
        }
    }

    private void UnlockPierce()
    {
        if (unlockPierceButton.unlocked)
        {
            swordUnlocked = true;
            swordType = SwordType.Pierce;
        }
    }

    private void UnlockSpin()
    {
        if (unlockSpinButton.unlocked)
        {
            swordUnlocked = true;
            swordType = SwordType.Spin;
        }
    }

    private void UnlockTimeStop()
    {
        if (unlockTimeStopButton.unlocked)
        {
            timeStopUnlocked = true;
        }
    }

    private void UnlockVulnerable()
    {
        if (unlockVulnerableButton.unlocked)
            vulnerableUnlocked = true;
    }

    #endregion

    #region skill
    public void CreatSword()
    {
        GameObject newSword = Instantiate(swordPrefab, player.transform.position + player.transform.right * .5f
            , transform.rotation); ;
        Sword_Skill_Controller newSwordScript = newSword.GetComponent<Sword_Skill_Controller>();

        switch (swordType)
        {
            case SwordType.Bounce:
                newSwordScript.SetupBounce(true, bounceAmount,bounceSpeed);
                break;
            case SwordType.Pierce:
                newSwordScript.SetupPierce(pierceAmount);
                break;
            case SwordType.Spin:
                newSwordScript.SetupSpin(true, maxTravelDistance, spinDuration,hitCooldown);
                break;
        }

        newSwordScript.SetupSword(finalDir, swordGravity, freezeTimeDuration, vulnerableDuration, returnSpeed,maxSwordReturnDistance);
        player.AssignNewSword(newSword);

        DotsActive(false);
    }

    #region Aim region

    public Vector2 AimDirection()
    {
        Vector2 playerPosition = player.transform.position;
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = mousePosition - playerPosition;

        return direction;
    }

    public void DotsActive(bool _isActive)
    {
        for (int i = 0; i < numberOfDots; i++)
        {
            dots[i].SetActive(_isActive);
        }
    }

    private void GenereateDots()
    {
        dots = new GameObject[numberOfDots];
        for (int i = 0; i < dots.Length; i++)
        {
            dots[i] = Instantiate(dotPrefab, player.transform.position, Quaternion.identity, dotsParent);
            dots[i].SetActive(false);
        }
    }

    private Vector2 DotsPosition(float t)
    {
        Vector2 position = (Vector2)player.transform.position + new Vector2(
            AimDirection().normalized.x * launchForce.x,
            AimDirection().normalized.y * launchForce.y) * t + .5f * (Physics2D.gravity * swordGravity) * (t * t);

        return position;
    }
    #endregion

    #endregion
}
