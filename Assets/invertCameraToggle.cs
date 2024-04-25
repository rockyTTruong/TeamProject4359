using System.Collections;
using Cinemachine;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;


public class InvertXX : MonoBehaviour
{
    public CinemachineFreeLook freeLookCamera;
    [SerializeField] Toggle toggle;


    private void Start()
    {
        toggle.onValueChanged.AddListener(OnToggleValueChanged);
    }

    public void OnToggleValueChanged(bool newValue)
    {
        freeLookCamera.m_YAxis.m_InvertInput = !freeLookCamera.m_YAxis.m_InvertInput;
        //freeLookCamera.m_XAxis.m_InvertInput = !freeLookCamera.m_XAxis.m_InvertInput;
    }
}