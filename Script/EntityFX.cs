using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.WSA;

public class EntityFX : MonoBehaviour
{
    private SpriteRenderer sr;
    [SerializeField] private float flashDuration;
    [SerializeField] private Material hitMat;
    private Material originaMat;


    [Header("Ailment colors")]
    [SerializeField] private Color[] chillColor;
    [SerializeField] private Color[] igniteColor;
    [SerializeField] private Color[] shockColor;

    private void Start()
    {
        sr =GetComponentInChildren<SpriteRenderer>();
        originaMat = sr.material;
    }

    //黑洞技能角色透明化
    public void MakeTransprent(bool _transprent)
    {
        if (_transprent)
            sr.color = Color.clear;
        else
            sr.color = Color.white;
    }

    public void SetAfterimageColor(float _red, float _blue, float _green, float _transparent)
    {
        sr.color = new Color(_red, _blue, _green, _transparent);
    }

    private IEnumerator FlashFX() 
    {
        sr.material = hitMat;
        Color currentColor = sr.color;
        sr.color = Color.white;

        yield return new WaitForSeconds(flashDuration);

        sr.color = currentColor;
        sr.material = originaMat;  
    }

    //已经由于与烧伤效果雷同，弃用
    private void RedColorBlink()
    {
        if (sr.color != Color.white)
        {
            sr.color = Color.white;
        }
        else sr.color = Color.red;
    }

    public void CancelColorChange()
    {
        CancelInvoke();
        sr.color = Color.white;
    }

    public void ShockFxFor(float _second)
    {
        InvokeRepeating("ShockColorFx", 0, 0.3f);
        Invoke("CancelColorChange", _second);
    }

    private void ShockColorFx()
    {
        if (sr.color != shockColor[0])
        {
            sr.color = shockColor[0];
        }
        else
        {
            sr.color = shockColor[1];
        }
    }

    public void ChillFxFor(float _second)
    {   
        InvokeRepeating("ChillColorFx", 0, 0.3f);
        Invoke("CancelColorChange", _second);
    }

    private void ChillColorFx()
    {
        if (sr.color != chillColor[0])
        {
            sr.color = chillColor[0];
        }
        else
        {
            sr.color = chillColor[1];
        }
    }

    public virtual void IgniteFxFor(float _second)
    {
        InvokeRepeating("IgniteColorFx", 0, 0.3f);
        Invoke("CancelColorChange", _second);
    }


    private void IgniteColorFx()
    {
        if (sr.color != igniteColor[0])
        {
            sr.color = igniteColor[0];
        }
        else
        {
            sr.color = igniteColor[1];
        }
    }
}
