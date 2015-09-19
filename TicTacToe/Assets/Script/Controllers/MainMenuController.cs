using UnityEngine;
using System.Collections;
using DG.Tweening;

public class MainMenuController : MonoBehaviour
{
    [SerializeField]
    GameObject mainMenu, optionsMenu;
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
        mainMenu.transform.DOMoveX(-5000, 3f);

        optionsMenu.transform.DOMoveX(Camera.main.pixelWidth/2 + 50, 2f);
    }
}
