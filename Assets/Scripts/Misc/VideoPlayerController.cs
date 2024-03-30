using UnityEngine;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class VideoPlayerController : MonoBehaviour
{
    public GameObject canvasUI;
    public VideoPlayer videoPlayer;
    public GameObject skipTooltip;
    public PauseMenu pauseMenu;
    public Scene loadSceneAfterFinish;
    public DialogueData postDialogue;

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

            DialogueManager.Instance.StartDialogue(postDialogue);
        }
        this.gameObject.SetActive(false);
    }
}
