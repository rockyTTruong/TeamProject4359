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

    private IEnumerator SetSelectCursor()
    {
        yield return new WaitForSecondsRealtime(0.5f);
        EventSystem.current.SetSelectedGameObject(firstSelectButton);
    }
}
