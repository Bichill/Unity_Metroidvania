using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    private Animator anim;
    public string checkPointId;
    public bool activationStatus; 

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    [ContextMenu("Generate checkPoint Id")]
    private void GenerateId()
    {
        checkPointId = System.Guid.NewGuid().ToString();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (activationStatus) return;

        if (collision.GetComponent<Player>()!=null)
        {
            ActiveCheckPoint();
        }
    }

    public void ActiveCheckPoint()
    {
        activationStatus = true; 
        anim.SetBool("active", true);

        AudioManager.instance.PlaySFX(13, transform, 1f, 2f);

    }
}
