using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject newGameButton;
    [SerializeField] private GameObject loadGameButton;
    [SerializeField] private GameObject optionButton;
    [SerializeField] private GameObject creditButton;
    [SerializeField] private GameObject exitButton;

    private void OnEnable()
    {
        EventSystem.current.SetSelectedGameObject(newGameButton);    
    }

    public void NewGame()
    {
        SceneManager.LoadScene((int)Scene.PersistentScene);
        SceneManager.LoadScene((int)Scene.StartingZone, LoadSceneMode.Additive);
    }
    public void LoadGamge()
    {
        SceneManager.LoadScene((int)Scene.PersistentScene);
        SceneManager.LoadScene((int)Scene.StartingZone, LoadSceneMode.Additive);
    }
    public void Option()
    {
        SceneManager.LoadScene((int)Scene.OptionMenu);
    }

    public void Credits()
    {
        SceneManager.LoadScene((int)Scene.Credits);
    }
    public void exitGame()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.ExitPlaymode();
#endif
    }
}
