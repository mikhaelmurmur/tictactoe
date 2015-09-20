using UnityEngine;
using System.Collections;
using System;

public enum SoundTypes
{
    background,
    click
}

public class AudioManager : MonoBehaviour
{
    public AudioClip backgroundSound, clickSound;
    public AudioSource bgPlayer, FXPlayer;
    bool isBgMusic = true;
    bool isFxMusic = true;
    static int numberOfInstaces = 0;


    void OnEnable()
    {
        EventManager.Instance.Add(EventManager.events.playClick, PlaySoundEvent);
        EventManager.Instance.Add(EventManager.events.setSoundOption, SetOptions);
        numberOfInstaces++;
        if (numberOfInstaces > 1)
        {
            Destroy(this.gameObject);
            numberOfInstaces--;
        }
    }

    void OnDisable()
    {
        if (EventManager.Instance != null)
        {
            EventManager.Instance.Remove(EventManager.events.playClick, PlaySoundEvent);
            EventManager.Instance.Remove(EventManager.events.setSoundOption, SetOptions);
        }
    }
    void Start()
    {
        if (PlayerPrefs.HasKey("bgMusic"))
        {
            isBgMusic = Convert.ToBoolean(PlayerPrefs.GetInt("bgMusic"));

        }
        if (isBgMusic)
        {
            PlaySound(SoundTypes.background);
        }

        if (PlayerPrefs.HasKey("fxMusic"))
        {
            isFxMusic = Convert.ToBoolean(PlayerPrefs.GetInt("fxMusic"));
        }
        DontDestroyOnLoad(this);
        EventManager.Instance.Call(EventManager.events.getSoundPrefs, new object[] { isBgMusic, isFxMusic });
    }

    public void CallChange()
    {
        EventManager.Instance.Call(EventManager.events.getSoundPrefs, new object[] { isBgMusic, isFxMusic });
    }

    void PlaySoundEvent(object[] param)
    {
        SoundTypes type = (SoundTypes)param[0];
        PlaySound(type);
    }

    public void PlaySound(SoundTypes type)
    {
        switch (type)
        {
            case SoundTypes.background:
                bgPlayer.clip = backgroundSound;
                bgPlayer.Play();
                break;
            case SoundTypes.click:
                if (isFxMusic)
                {
                    FXPlayer.clip = clickSound;
                    FXPlayer.Play();
                }
                break;
        }
    }

    public void SetFxOption(bool play)
    {
        isFxMusic = play;
        if (play)
            PlayerPrefs.SetInt("fxMusic", 1);
        else
            PlayerPrefs.SetInt("fxMusic", 0);
    }

    public void SetBgOption(bool play)
    {
        isBgMusic = play;
        if (play)
        {
            PlayerPrefs.SetInt("bgMusic", 1);
            bgPlayer.Play();
        }
        else
        {
            PlayerPrefs.SetInt("bgMusic", 0);
            bgPlayer.Stop();
        }

    }

    void SetOptions(object[] par)
    {
        if ((int)par[0] == 0)
        {
            SetBgOption((bool)par[1]);
        }
        else
        {
            SetFxOption((bool)par[1]);
        }
        EventManager.Instance.Call(EventManager.events.getSoundPrefs, new object[] { isBgMusic, isFxMusic });
    }

    public bool GetFX()
    {
        return isFxMusic;
    }

    public bool GetBG()
    {
        return isBgMusic;
    }
}
