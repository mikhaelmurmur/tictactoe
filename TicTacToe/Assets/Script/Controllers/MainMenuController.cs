using UnityEngine;
using System.Collections;
using DG.Tweening;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    [SerializeField]
    GameObject mainMenu, optionsMenu;
    [SerializeField]
    Toggle fxToggle, bgToggle;

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
        optionsMenu.transform.DOMoveX(Camera.main.pixelWidth / 2 + 50, 2f);
    }

    public void BackToMainMenu()
    {
        mainMenu.transform.DOMoveX(Camera.main.pixelWidth / 2, 2f);
        optionsMenu.transform.DOMoveX(Camera.main.pixelWidth + 100, 2f);
    }

    public void BgAudioChange(bool desicion)
    {
        EventManager.Instance.Call(EventManager.events.setSoundOption, new object[] { 0, desicion });
    }

    public void FXAudioChange(bool desicion)
    {
        EventManager.Instance.Call(EventManager.events.setSoundOption, new object[] { 1, desicion });
    }
}
