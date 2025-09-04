using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class LostCurrencyController : MonoBehaviour
{
    public int currency;
    private AudioSource audioSource;
    private Animator anim;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Player>()!=null)
        {
            GameManager.instance.isPickedUpLostCurrency = true;
            anim.SetTrigger("fade");
            AudioManager.instance.PlaySFX(3, transform, 1.3f, 0.5f);

            PlayerManager.instance.currency += currency;
            Destroy(this.gameObject,5);
        }
    }
}
