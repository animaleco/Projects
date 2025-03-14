using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [Header("Audio Source")]
    [SerializeField] private AudioSource[] sfx;
    [SerializeField] private AudioSource[] bgm;

    private int bgmIndex;

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);

        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }

        if(bgm.Length <= 0)
        {
            return;
        }

        InvokeRepeating(nameof(PlayMusicIfNeeded), 0, 2);
    }

    public void PlayMusicIfNeeded()
    {
        if (bgm[bgmIndex].isPlaying == false)
        {
            PlayRandomBGM();
        }
    }

    public void PlayRandomBGM()
    {
        bgmIndex = Random.Range(0, bgm.Length);
        PlayBGM(bgmIndex);
    }

    public void PlayBGM(int bgmToPlay)
    {
        if(bgm.Length <= 0)
        {
            Debug.LogWarning("You have no music on audio manager!");
            return;
        }

        for (int i = 0; i < bgm.Length; i++)
        {
            bgm[i].Stop();
        }

        bgmIndex = bgmToPlay; // Esto actualiza el valor para que la musica no se interrumpa cuando se haga el check
        bgm[bgmToPlay].Play();
    }

    public void PlaySFX(int sfxToPlay, bool randomPitch = true) //Con esto hacemos que suene el sonido solo cuando llamemos la funcion y con el bool le damos un sonido random cada vez que suene
    {
        if(sfxToPlay >= sfx.Length)
        {
            return;
        }

        if (randomPitch)
        {
            sfx[sfxToPlay].pitch = Random.Range(.9f, 1.1f);
        }


        sfx[sfxToPlay].Play();
    }

    public void StopSFX(int sfxToStop)
    {
        sfx[sfxToStop].Stop();
    }
}
