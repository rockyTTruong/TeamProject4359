using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BossDefeatCutsceneTrigger : MonoBehaviour
{
    public Character character;
    public int videoIndex;
    public bool triggerCutscene;
    public GameObject skullAndScroll; 

    private void Start()
    {
        character = GetComponent<Character>();
        character.DieEvent += CutsceneTrigger;
    }

    public void CutsceneTrigger(GameObject thisObject)
    {
        if (gameObject != thisObject) return;
        StartCoroutine(TriggerCoroutine());
    }

    public IEnumerator TriggerCoroutine()
    {
        InputReader.Instance.DisableInput();
        float fadeDuration = FadeScreen.Instance.GetFadeDuration();

        FadeScreen.Instance.FadeOut();
        yield return new WaitForSeconds(fadeDuration);

        if (skullAndScroll != null) skullAndScroll.SetActive(true);
        PlayerStateMachine psm = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStateMachine>();
        psm.controller.enabled = false;
        psm.transform.position = psm.savePosition;
        psm.transform.rotation = psm.saveRotation; 
        psm.controller.enabled = true;

        FadeScreen.Instance.FadeIn();
        VideoManager.Instance.PlayVideo(videoIndex);
        if (!triggerCutscene) this.gameObject.SetActive(false);
    }
}
