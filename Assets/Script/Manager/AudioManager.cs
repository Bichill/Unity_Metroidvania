using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [SerializeField] private AudioSource[] sfx;
    [SerializeField] private AudioSource[] bgm;
    [SerializeField] private float sfxMinimumDistance;

    public bool playBgm;
    [SerializeField] private int bgmIndex;

    private bool canPlaySfx;

    private void Awake()
    {
        if (instance != null)
            Destroy(instance.gameObject);
        else
            instance = this;

        Invoke("AllowSfx", 0.1f);
    }

    private void Update()
    {
        if (!playBgm)
            StopAllBGM();
        else
        {
            if (!bgm[bgmIndex].isPlaying)
                PlayBGM(bgmIndex);
        }
    }

    public void PlaySFX(int _sfxIndex, Transform _source, float _pitch = 1, float _volume = 1)
    {
        if (!canPlaySfx)
            return;

        // 如果声音对象存在且超出最远
        if (_source != null &&
            Vector2.Distance(PlayerManager.instance.player.transform.position, _source.position) > sfxMinimumDistance)
            return;

        if (_sfxIndex < sfx.Length)
        {
            sfx[_sfxIndex].pitch = _pitch;
            sfx[_sfxIndex].volume = _volume;
            sfx[_sfxIndex].Play();
        }
    }

    public void StopSFX(int _index) => sfx[_index].Stop();

    public void PlayBGM(int _bgmIndex, float _pitch = 1, float _volume = 1)
    {
        StopAllBGM();
        bgmIndex = _bgmIndex;
        bgm[bgmIndex].pitch = _pitch;
        bgm[bgmIndex].volume = _volume;

        bgm[bgmIndex].Play();
    }

    public void StopAllBGM()
    {
        for (int i = 0; i < bgm.Length; i++)
        {
            bgm[i].Stop();
        }
    }

    public void AllowSfx() => canPlaySfx = true;
}
