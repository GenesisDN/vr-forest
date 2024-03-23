using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameStartMenu : MonoBehaviour
{
    [Header("UI Pages")]
    public GameObject mainMenu;
    public GameObject dog;
    public GameObject forest;
    public GameObject forestdog;

    [Header("Main Menu Buttons")]
    public Button bothButton;
    public Button dogButton;
    public Button forestButton;
    public Button quitButton;

    public List<Button> returnButtons;

    // Start is called before the first frame update
    void Start()
    {
        EnableMainMenu();

        //Hook events
        bothButton.onClick.AddListener(EnableBothScene);
        dogButton.onClick.AddListener(EnableDogScene);
        forestButton.onClick.AddListener(EnableForestScene);

        foreach (var item in returnButtons)
        {
            item.onClick.AddListener(EnableMainMenu);
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void EnableBothScene()
    {
        //HideAll();
        SceneTransitionManager.singleton.GoToSceneAsync(1);
    }

    public void HideAll()
    {
        mainMenu.SetActive(false);
        dog.SetActive(false);
        forest.SetActive(false);
     
    }

    public void EnableMainMenu()
    {
        mainMenu.SetActive(true);
        dog.SetActive(false);
        forest.SetActive(false);
  
    }
    public void EnableDogScene()
    {
        //HideAll();
        SceneTransitionManager.singleton.GoToSceneAsync(2);
    }
    public void EnableForestScene()
    {

        //HideAll();
        SceneTransitionManager.singleton.GoToSceneAsync(3);
    }
}
