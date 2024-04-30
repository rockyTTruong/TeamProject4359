using UnityEngine;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem;
using UnityEngine.Video;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class VideoPlayerController : MonoBehaviour
{
    public GameObject canvasUI;
    public VideoPlayer videoPlayer;
    public GameObject skipTooltip;
    public PauseMenu pauseMenu;
    public Scene loadSceneAfterFinish;
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
            SceneManager.LoadSceneAsync((int)loadSceneAfterFinish, LoadSceneMode.Single);
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
}
