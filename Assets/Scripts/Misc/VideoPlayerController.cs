using UnityEngine;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem;
using UnityEngine.Video;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Collections;

public class VideoPlayerController : MonoBehaviour
{
    public GameObject canvasUI;
    public VideoPlayer videoPlayer;
    public GameObject skipTooltip;
    public PauseMenu pauseMenu;
    public Scene loadSceneAfterFinish;
    public Scene unloadScene;
    public int nextVideoIndexPlayAfterFinish = -1;
    public DialogueData postDialogue;
    public List<GameObject> activateObjectsBeforeDialogue = new List<GameObject>();
    public List<GameObject> activateObjectsAfterDialogue = new List<GameObject>();

    private void Update()
    {
        if (!videoPlayer.isPlaying) return;
        if (Gamepad.current != null)
        {
            if (Gamepad.current.selectButton.IsPressed())
            {
                OnVideoFinished(videoPlayer);
            }
        }
        if(Input.GetKeyUp(KeyCode.X))
        {
            OnVideoFinished(videoPlayer);
        }
    }

    public void PlayVideo()
    {
        AudioManager.Instance.MuteBGM();
        InputReader.Instance.DisableInput();
        canvasUI.SetActive(false);
        Time.timeScale = 0.0f;
        videoPlayer = GetComponent<VideoPlayer>();
        //videoPlayer.SetDirectAudioVolume(0, PlayerPrefs.GetFloat("CutsceneAudio"));
        skipTooltip.SetActive(true);
        videoPlayer.loopPointReached += OnVideoFinished;
        videoPlayer.Play();
        pauseMenu.videoPlaying = true;
    }

    private void OnVideoFinished(VideoPlayer vp)
    {
        videoPlayer.Stop();
        Time.timeScale = 1.0f;
        canvasUI.SetActive(true);
        InputReader.Instance.EnableInput();
        AudioManager.Instance.UnmuteBGM();
        videoPlayer.loopPointReached -= OnVideoFinished;
        skipTooltip.SetActive(false);
        pauseMenu.videoPlaying = false;
        if (loadSceneAfterFinish != Scene.None)
        {
            if (loadSceneAfterFinish == Scene.Credits)
            {
                SceneManager.LoadSceneAsync((int)loadSceneAfterFinish, LoadSceneMode.Single);
                return;
            }
            else
            {
                if (activateObjectsAfterDialogue.Count != 0)
                {
                    foreach (GameObject activateObject in activateObjectsAfterDialogue)
                    {
                        activateObject.SetActive(!activateObject.activeSelf);
                    }
                }
                StartCoroutine(LoadSceneCoroutine());
                return;
            }
        }
        if (postDialogue != null)
        {
            PlayerStateMachine psm = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStateMachine>();
            psm.SwitchState(new PlayerTalkingState(psm));

            if (nextVideoIndexPlayAfterFinish != -1)
            {
                DialogueManager.Instance.PlayVideoAfterDialogue(nextVideoIndexPlayAfterFinish);
            }
            if (activateObjectsBeforeDialogue.Count != 0)
            {
                foreach (GameObject activateObject in activateObjectsBeforeDialogue)
                {
                    activateObject.SetActive(!activateObject.activeSelf);
                }
            }
            if (activateObjectsAfterDialogue.Count != 0)
            {
                DialogueManager.Instance.ActivateObjectAfterDialogue(activateObjectsAfterDialogue);
            }
            DialogueManager.Instance.StartDialogue(postDialogue);
        }
        else if (nextVideoIndexPlayAfterFinish != -1)
        {
            VideoManager.Instance.PlayVideo(nextVideoIndexPlayAfterFinish);
        }
        else if (activateObjectsAfterDialogue.Count != 0)
        {
            foreach (GameObject activateObject in activateObjectsAfterDialogue)
            {
                activateObject.SetActive(!activateObject.activeSelf);
            }
        }
        this.gameObject.SetActive(false);
    }

    public IEnumerator LoadSceneCoroutine()
    {
        InputReader.Instance.DisableInput();

        FadeScreen.Instance.FadeOut();
        float fadeDuration = FadeScreen.Instance.GetFadeDuration();
        yield return new WaitForSeconds(fadeDuration);

        AsyncOperation loadOperation = SceneManager.LoadSceneAsync((int)loadSceneAfterFinish, LoadSceneMode.Additive);
        while (!loadOperation.isDone)
        {
            yield return null;
        }

        yield return new WaitForSeconds(2f);

        AsyncOperation unloadOperation = SceneManager.UnloadSceneAsync((int)unloadScene);
        while (!unloadOperation.isDone)
        {
            yield return null;
        }
        FadeScreen.Instance.FadeIn();
        yield return new WaitForSeconds(fadeDuration);

        InputReader.Instance.EnableInput();
        this.gameObject.SetActive(false);
        yield break;
    }
}
