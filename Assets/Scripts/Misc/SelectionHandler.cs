using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SelectionHandler : MonoBehaviour
{
    [SerializeField] private GameObject firstSelectButton;

    private void OnEnable()
    {
        StartCoroutine(SetSelectCursor());
    }

    private void OnDisable()
    {
        EventSystem.current.SetSelectedGameObject(null);
    }

    private IEnumerator SetSelectCursor()
    {
        yield return new WaitForSecondsRealtime(0.5f);
        EventSystem.current.SetSelectedGameObject(firstSelectButton);
    }
}
