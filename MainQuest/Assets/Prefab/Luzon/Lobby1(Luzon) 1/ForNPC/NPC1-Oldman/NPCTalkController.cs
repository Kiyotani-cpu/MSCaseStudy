using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using Cinemachine;
using UnityEngine.SceneManagement; // for scene loading

public class NPCTalkController : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public Animator animator;
    public CinemachineVirtualCamera npcCam;
    public CinemachineVirtualCamera playerCam;

    [Header("Dialogue UI")]
    public GameObject dialoguePanel;
    public TextMeshProUGUI dialogueText;
    public GameObject nameChoicePanel;
    public GameObject choicePanel;
    public Button nameButtonAlex;
    public Button yesButton;
    public Button noButton;
    public Button nextButton;
    public Button endButton;

    [Header("Dialogue Data")]
    [TextArea] public string[] npcDialogues;
    public AudioClip[] npcVoices;
    public AudioSource audioSource;

    [Header("Settings")]
    public float maxTalkDistance = 3f;
    public float wordDelay = 0.25f;

    [Header("BGM")]
    public BGMController bgmController;

    [Header("Scene Control")]
    public string nextSceneName = "NextScene"; // set this in Inspector

    private bool isTalking = false;
    private int currentLine = 0;
    private Coroutine typeCoroutine;
    private bool nameChosen = false;

    void Start()
    {
        dialoguePanel.SetActive(false);
        nameChoicePanel.SetActive(false);
        choicePanel.SetActive(false);

        if (nameButtonAlex != null) nameButtonAlex.onClick.AddListener(() => OnNameChosen());
        if (yesButton != null) yesButton.onClick.AddListener(() => OnChoiceMade(true));
        if (noButton != null) noButton.onClick.AddListener(() => OnChoiceMade(false));

        if (nextButton != null)
        {
            nextButton.onClick.AddListener(OnNextButton);
            nextButton.gameObject.SetActive(false);
        }

        if (endButton != null)
        {
            endButton.onClick.AddListener(StopTalking);
            endButton.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if (player == null) return;

        float distance = Vector3.Distance(player.position, transform.position);

        if (distance <= maxTalkDistance && Input.GetKeyDown(KeyCode.E))
        {
            if (!isTalking)
                StartTalking();
            else
                NextLineOrStop();
        }

        if (Input.GetMouseButtonDown(0) ||
            (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began))
        {
            Vector3 inputPos = Input.mousePresent ? Input.mousePosition : (Vector3)Input.GetTouch(0).position;
            Ray ray = Camera.main.ScreenPointToRay(inputPos);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform == transform && distance <= maxTalkDistance)
                {
                    if (!isTalking)
                        StartTalking();
                    else
                        NextLineOrStop();
                }
            }
        }

        if (isTalking && distance > maxTalkDistance)
            StopTalking();
    }

    void StartTalking()
    {
        isTalking = true;
        currentLine = 0;

        if (animator != null)
            animator.SetBool("isTalking", true);

        dialoguePanel.SetActive(true);
        ShowDialogueLine(currentLine);

        if (bgmController != null)
            bgmController.MuteBGM();

        if (npcCam != null && playerCam != null)
        {
            npcCam.Priority = 20;
            playerCam.Priority = 10;
        }

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    void NextLineOrStop()
    {
        if (!nameChosen && currentLine == 0)
            return;

        if (currentLine < npcDialogues.Length - 1)
        {
            currentLine++;
            ShowDialogueLine(currentLine);
        }
        else
        {
            StopTalking();
        }
    }

    void ShowDialogueLine(int index)
    {
        if (typeCoroutine != null)
            StopCoroutine(typeCoroutine);

        if (audioSource != null && npcVoices != null && index < npcVoices.Length && npcVoices[index] != null)
        {
            audioSource.Stop();
            audioSource.clip = npcVoices[index];
            audioSource.Play();
        }

        typeCoroutine = StartCoroutine(TypeText(npcDialogues[index], index));

        nameChoicePanel.SetActive(index == 0 && !nameChosen);
        choicePanel.SetActive(index == 2 && nameChosen);

        if (nextButton != null) nextButton.gameObject.SetActive(false);
        if (endButton != null) endButton.gameObject.SetActive(false);
    }

    IEnumerator TypeText(string line, int lineIndex)
    {
        dialogueText.text = "";
        string[] words = line.Split(' ');

        foreach (string word in words)
        {
            dialogueText.text += word + " ";
            yield return new WaitForSeconds(wordDelay);
        }

        typeCoroutine = null;

        // After text finishes typing
        if (lineIndex == 1 && nextButton != null)
        {
            nextButton.gameObject.SetActive(true);
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        if (lineIndex == 3) // Yes dialogue finished
        {
            yield return new WaitForSeconds(7f); // small pause
            if (!string.IsNullOrEmpty(nextSceneName))
                SceneManager.LoadScene(nextSceneName);
        }

        if (lineIndex == 4 && endButton != null) // No dialogue finished
        {
            endButton.gameObject.SetActive(true);
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }

    void StopTalking()
    {
        isTalking = false;

        if (animator != null)
            animator.SetBool("isTalking", false);

        dialoguePanel.SetActive(false);
        nameChoicePanel.SetActive(false);
        choicePanel.SetActive(false);

        if (audioSource != null)
            audioSource.Stop();

        currentLine = 0;

        if (typeCoroutine != null)
        {
            StopCoroutine(typeCoroutine);
            typeCoroutine = null;
        }

        if (bgmController != null)
            bgmController.UnmuteBGM();

        if (npcCam != null && playerCam != null)
        {
            playerCam.Priority = 20;
            npcCam.Priority = 10;
        }

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        if (nextButton != null) nextButton.gameObject.SetActive(false);
        if (endButton != null) endButton.gameObject.SetActive(false);
    }

    void OnNameChosen()
    {
        nameChosen = true;
        nameChoicePanel.SetActive(false);
        NextLineOrStop();
    }

    void OnChoiceMade(bool answerYes)
    {
        choicePanel.SetActive(false);

        if (answerYes)
        {
            currentLine = 3;
            ShowDialogueLine(currentLine);
        }
        else
        {
            currentLine = 4;
            ShowDialogueLine(currentLine);
        }
    }

    public void OnNextButton()
    {
        if (typeCoroutine != null)
        {
            StopCoroutine(typeCoroutine);
            dialogueText.text = npcDialogues[currentLine];
            typeCoroutine = null;
            return;
        }

        if (currentLine == 1)
        {
            if (currentLine < npcDialogues.Length - 1)
            {
                currentLine++;
                ShowDialogueLine(currentLine);
            }
            else
            {
                StopTalking();
            }
        }
    }
}
