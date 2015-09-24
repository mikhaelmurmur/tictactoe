using UnityEngine;
using System.Collections;
using DG.Tweening;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    [SerializeField]
    GameObject mainMenu, optionsMenu, gamemodeMenu;
    [SerializeField]
    Toggle fxToggle, bgToggle;
    GameObject menuMoved;
    [SerializeField]
    Toggle AIToggle, playerToggle;

    void OnEnable()
    {
        EventManager.Instance.Add(EventManager.events.getSoundPrefs, SetSoundOptions);
    }
    void OnDisable()
    {
        if (EventManager.Instance != null)
            EventManager.Instance.Remove(EventManager.events.getSoundPrefs, SetSoundOptions);
    }

    void Start()
    {
        FindObjectOfType<AudioManager>().CallChange();
    }

    void SetSoundOptions(object[] par)
    {
        Debug.Log("Music is set!");
        fxToggle.isOn = (bool)par[1];
        bgToggle.isOn = (bool)par[0];
    }

    public void NewGame()
    {
        Application.LoadLevel("GameCore");
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void ShowOptions()
    {
        mainMenu.transform.DOMoveX(-200, 2f);
        menuMoved = optionsMenu;
        optionsMenu.transform.DOMoveX(Camera.main.pixelWidth / 2 + 50, 2f);
    }

    public void ShowGamemodes()
    {
        if(PlayerPrefs.HasKey("mode"))
        {
            int mode = PlayerPrefs.GetInt("mode");
            if(mode==1)
            {
                playerToggle.isOn = true;            
            }
            else
            {
                AIToggle.isOn = true;
            }
        }
        else
        {
            AIToggle.isOn = true;
        }
        mainMenu.transform.DOMoveX(-200, 2f);
        menuMoved = gamemodeMenu;
        gamemodeMenu.transform.DOMoveX(Camera.main.pixelWidth / 2 + 50, 2f);
    }

    public void BackToMainMenu()
    {
        mainMenu.transform.DOMoveX(Camera.main.pixelWidth / 2, 2f);
        menuMoved.transform.DOMoveX(Camera.main.pixelWidth + 100, 2f);
        menuMoved = null;
    }

    public void BgAudioChange(bool desicion)
    {
        EventManager.Instance.Call(EventManager.events.setSoundOption, new object[] { 0, desicion });
    }

    public void FXAudioChange(bool desicion)
    {
        EventManager.Instance.Call(EventManager.events.setSoundOption, new object[] { 1, desicion });
    }

    public void SetAIMode()
    {
        PlayerPrefs.SetInt("mode", 0);
    }

    public void SetPlayerMode()
    {
        PlayerPrefs.SetInt("mode", 1);
    }
}
