using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMgr : MonoBehaviour
{
    public static GameMgr instance { get; private set; }

    public AudioSource musicSource;
    public AudioSource sfxSource;

    [HideInInspector]public int row;
    [HideInInspector] public int col;

    public AudioClip hit;
    public AudioClip coin;
    public AudioClip normal;
    public AudioClip fight;
    public int difficulity;
    public int level;
    public bool canmove;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        DontDestroyOnLoad(gameObject);
        row = 10;
        col = 10;
        canmove = false;
    }
    private void Start()
    {
        difficulity = 1;
        SetGloabMusic(normal);
       
    }
    public void SetGloabMusic(AudioClip clip) 
    {
        musicSource.clip = clip;
        musicSource.Play();
    }
    public void SetSfxSound(AudioClip clip) 
    {
        this.sfxSource.clip = clip;
        sfxSource.Play();
    }
    private void OnDestroy()
    {
        if (instance != null)
            Destroy(instance);
    }
   
}
