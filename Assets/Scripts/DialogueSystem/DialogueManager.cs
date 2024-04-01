using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Playables;

public class DialogueManager : SingletonMonobehaviour<DialogueManager>
{
    [SerializeField] private bool auto;
    [SerializeField] private TextMeshProUGUI textMesh;
    [SerializeField] private TextMeshProUGUI speakerNameMesh;
    [SerializeField] private GameObject dialogueBox;
    [SerializeField] private float textSpeed = 0.05f;
    [SerializeField] private float autoSpeed = 0.3f;

    private Queue<Dialogue> dialogueQueue = new Queue<Dialogue>();
    private Coroutine typing;
    private Dialogue currentDialogue;
    private DialogueData currentDialogueData;
    private UnityAction action;

    protected override void Awake()
    {
        base.Awake();
    }

    public void StartDialogue(DialogueData dialogueData)
    {
        GameManager.Instance.SetInMenuBool(true);
        currentDialogueData = dialogueData;
        LoadDialogue(dialogueData.textJson);
        dialogueBox.SetActive(true);
        GoToNextDialogue();
    }

    public void ActionAfterDialogue(UnityAction action)
    {
        this.action = action;
    }

    public void LoadDialogue(TextAsset jsonFile)
    {
        if (jsonFile != null)
        {
            string jsonText = jsonFile.text;
            Dialogues dialogues = JsonUtility.FromJson<Dialogues>(jsonText);
            foreach (Dialogue dialogue in dialogues.dialogues)
            {
                dialogueQueue.Enqueue(dialogue);
            }
        }
        else Debug.LogError("JSON file not found.");
    }

    public void HandleUserInput()
    {
        if (typing == null)
        {
            GoToNextDialogue();
        }
        else
        {
            StopCoroutine(typing);
            typing = null;
            textMesh.text = currentDialogue.message;
        }
    }

    private void GoToNextDialogue()
    {
        if (dialogueQueue.Count == 0)
        {
            EndDialogue();
            return;
        }
        Dialogue dialogue = dialogueQueue.Dequeue();
        if (dialogue.message == null)
        {
            GoToNextDialogue();
            return;
        }

        currentDialogue = dialogue;
        speakerNameMesh.text = dialogue.speaker;
        typing = StartCoroutine(TypeText(dialogue.message));
    }

    private void EndDialogue()
    {
        GameManager.Instance.SetInMenuBool(false);
        dialogueBox.SetActive(false);
        PlayerStateMachine psm = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStateMachine>();
        psm.SwitchState(new PlayerFreeLookState(psm));

        currentDialogueData.RaiseFlag();
        if (action != null) 
        { 
            action.Invoke(); 
            action = null;
        }
    }

    private IEnumerator TypeText(string text)
    {
        textMesh.text = "";
        foreach (char letter in text)
        {
            textMesh.text += letter;
            yield return new WaitForSeconds(textSpeed);
        }
        typing = null;
        if (auto)
        {
            yield return new WaitForSeconds(autoSpeed);
            GoToNextDialogue();
        }
    }
}
