using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class UI_CrystalMultiAmount : MonoBehaviour
{
    private TextMeshProUGUI amount;
    private Transform child;
    private Transform playerTransform;
    private Vector3 offsetFromPlayer;
    private Vector3 originalLocalScale;

    void Start()
    {
        amount = GetComponentInChildren<TextMeshProUGUI>();
        child = transform.GetChild(0);
        
        // ����ԭʼ�s��
        originalLocalScale = transform.localScale;

        playerTransform = transform.parent;
        // Ӌ���������ҵ�ƫ����
        offsetFromPlayer = transform.position - playerTransform.position;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateUI();
    }

    private void UpdateUI()
    {
        // �z�� SkillManager �� crystal �Ƿ���
        if (SkillManager.instance == null || SkillManager.instance.crystal == null)
        {
            Debug.LogWarning("SkillManager.instance �� crystal ���");
            return;
        }

        // ���S���λ�õ������S���D�Ϳs��
        if (playerTransform != null)
        {
            // �������ҵ��ӌ���ʹ������ƫ��
            transform.position = playerTransform.position + offsetFromPlayer;

            // ���Ʊ���UI��ԭʼ���D������������DӰ�
            transform.rotation = Quaternion.identity;
            // ���Ʊ���UI��ԭʼ�s�ţ���ֹ���Q���D
            transform.localScale = originalLocalScale;
        }

        // �z�����ˮ���Ƿ���i
        if (SkillManager.instance.crystal.crystalMultiUnlocked)
        {
            child.gameObject.SetActive(true);
            amount.text = "��" + SkillManager.instance.crystal.GetCurrentCrystalLeftAmount().ToString();
        }
        else
        {
            child.gameObject.SetActive(false);  
        }
    }
}
