using System.Collections;
using System.Collections.Generic;
using UnityEditor.Timeline;
using UnityEngine;

public class ThunderStrike_Controller : MonoBehaviour
{
    [SerializeField] private CharacterStats targetStats;
    [SerializeField] private float speed;

    private Animator anim;
    private bool triggered;
    private int damage;

    private void Start()
    {
        anim = GetComponentInChildren<Animator>();
    }

    public void Setup(int _damge, CharacterStats _targetStats)
    {
        damage = _damge;
        targetStats = _targetStats;
    }

    private void Update()
    {
        if (!targetStats || triggered)
            return;

        PursuitTarget();
    }

    private void PursuitTarget()
    {
        transform.position = Vector2.MoveTowards(transform.position, targetStats.transform.position, speed * Time.deltaTime);
        transform.up = targetStats.transform.position - transform.position;


        if (Vector2.Distance(transform.position, targetStats.transform.position) < .1f)
        {
            anim.transform.localRotation = Quaternion.identity;
            transform.localRotation = Quaternion.identity;

            AudioManager.instance.PlaySFX(14, transform, Random.Range(0.8f, 1.3f), 0.3f);
            triggered = true;
            anim.SetTrigger("Hit");
            Invoke("DamageAndSelfDestroy", .2f);
        }
    }

    private void DamageAndSelfDestroy()
    {
        targetStats.ApplyShock(true);//超模点，，可作为最终装备附带特效

        targetStats.TakeDamage(damage);
        Destroy(gameObject, .4f);
    }
}
