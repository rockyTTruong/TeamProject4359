using System.Collections;
using Cinemachine;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class OptionsMain : MonoBehaviour
{
    [SerializeField] private GameObject backButton;

    void Start()
    {

    }

 
    private void OnEnable()
    {
        EventSystem.current.SetSelectedGameObject(backButton);
    }



    public void backMenu()
    {
        PlayerPrefs.Save();
        SceneManager.LoadScene((int)Scene.MainMenu);
    }
}
