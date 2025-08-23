using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IgniteStamp_Controller : MonoBehaviour
{
    private CharacterStats targetStats;

    private Animator anim;

    private void Start()
    {
        anim = GetComponentInChildren<Animator>();
    }

    public void Setup(CharacterStats _targetStats)
    {
        targetStats = _targetStats;
    }

    private void Update()
    {
        UpdateIgniteStamp();
        stampOffset();
    }

    private void UpdateIgniteStamp()
    {
        if (targetStats == null) return;
        // 1. 动画层数同步
        int igniteStampCount = targetStats.igniteList.Count;
        anim.SetInteger("igniteStampCount", igniteStampCount);
        // 2. 层数为0时销毁
        if (igniteStampCount == 0)
        {
            Destroy(gameObject);
        }
    }

    private void stampOffset()
    {
        if (targetStats != null)
        {
            Vector3 pos = targetStats.transform.position;
            RaycastHit2D hit = Physics2D.Raycast(pos, Vector2.down, 15f, LayerMask.GetMask("Ground"));
            if (hit.collider != null)
            {
                pos.y = hit.point.y;
            }
            else
            {
                pos.y = 0f; // 默认地面
            }
            transform.position = pos;
        }
    }
}
