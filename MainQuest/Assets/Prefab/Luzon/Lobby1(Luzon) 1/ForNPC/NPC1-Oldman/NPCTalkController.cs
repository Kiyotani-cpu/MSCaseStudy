using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using Cinemachine;

public class NPCTalkController : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public Animator animator;
    public CinemachineVirtualCamera npcCam;
    public CinemachineVirtualCamera playerCam;

    [Header("Dialogue UI")]
    public GameObject dialoguePanel;      // Panel for general dialogue
    public TextMeshProUGUI dialogueText;
    public GameObject nameChoicePanel;    // Panel to choose name (Alex)
    public GameObject choicePanel;        // Panel for Yes/No choice
    public Button nameButtonAlex;
    public Button yesButton;
    public Button noButton;
    public Button nextButton;             // NEW: Next button for 2nd dialogue

    [Header("Dialogue Data")]
    [TextArea] public string[] npcDialogues;   // Regular dialogue lines (5 elements)
    public AudioClip[] npcVoices;              // Optional voice lines
    public AudioSource audioSource;

    [Header("Settings")]
    public float maxTalkDistance = 3f;
    public float wordDelay = 0.25f; // seconds per word

    [Header("BGM")]
    public BGMController bgmController;

    private bool isTalking = false;
    private int currentLine = 0;
    private Coroutine typeCoroutine;
    private bool nameChosen = false;

    void Start()
    {
        // Hide panels initially
        dialoguePanel.SetActive(false);
        nameChoicePanel.SetActive(false);
        choicePanel.SetActive(false);

        // Button listeners
        if (nameButtonAlex != null) nameButtonAlex.onClick.AddListener(() => OnNameChosen());
        if (yesButton != null) yesButton.onClick.AddListener(() => OnChoiceMade(true));
        if (noButton != null) noButton.onClick.AddListener(() => OnChoiceMade(false));

        // Next button setup
        if (nextButton != null)
        {
            nextButton.onClick.AddListener(OnNextButton);
            nextButton.gameObject.SetActive(false); // hidden by default
        }
        else
        {
            Debug.LogWarning("Next button not assigned in inspector (nextButton).");
        }
    }

    void Update()
    {
        if (player == null) return;

        float distance = Vector3.Distance(player.position, transform.position);

        // --- Keyboard (E) ---
        if (distance <= maxTalkDistance && Input.GetKeyDown(KeyCode.E))
        {
            if (!isTalking)
                StartTalking();
            else
                NextLineOrStop();
        }

        // --- Mouse (PC) OR Touch (Mobile) ---
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

        // --- Stop talking if player walks away ---
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

        // Mute BGM
        if (bgmController != null)
            bgmController.MuteBGM();

        // Switch to NPC camera
        if (npcCam != null && playerCam != null)
        {
            npcCam.Priority = 20;
            playerCam.Priority = 10;
        }

        // Enable mouse cursor for UI
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    void NextLineOrStop()
    {
        // If waiting for player input on name (index 0), don't advance automatically
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

        // Play audio if available
        if (audioSource != null && npcVoices != null && index < npcVoices.Length && npcVoices[index] != null)
        {
            audioSource.Stop();
            audioSource.clip = npcVoices[index];
            audioSource.Play();
        }

        // Start typing text
        typeCoroutine = StartCoroutine(TypeText(npcDialogues[index]));

        // Special panels
        if (index == 0 && !nameChosen)
            nameChoicePanel.SetActive(true);
        else
            nameChoicePanel.SetActive(false);

        if (index == 2 && nameChosen) // Third line = Yes/No choice
            choicePanel.SetActive(true);
        else
            choicePanel.SetActive(false);

        // NEXT button visible only on second dialogue (index == 1)
        if (nextButton != null)
            nextButton.gameObject.SetActive(index == 1);
    }

    IEnumerator TypeText(string line)
    {
        dialogueText.text = "";
        string[] words = line.Split(' ');

        foreach (string word in words)
        {
            dialogueText.text += word + " ";
            yield return new WaitForSeconds(wordDelay);
        }

        // finished typing
        typeCoroutine = null;
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

        // Unmute BGM
        if (bgmController != null)
            bgmController.UnmuteBGM();

        // Switch back to Player camera
        if (npcCam != null && playerCam != null)
        {
            playerCam.Priority = 20;
            npcCam.Priority = 10;
        }

        // Hide mouse cursor again
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        // Hide next button if active
        if (nextButton != null)
            nextButton.gameObject.SetActive(false);
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
            currentLine = 3; // Yes dialogue
        else
            currentLine = 4; // No dialogue

        ShowDialogueLine(currentLine);
    }

    // PUBLIC so you can also hook via the Inspector (optional)
    public void OnNextButton()
    {
        // If typing is still happening, finish the typing first (show full line)
        if (typeCoroutine != null)
        {
            StopCoroutine(typeCoroutine);
            dialogueText.text = npcDialogues[currentLine];
            typeCoroutine = null;
            return;
        }

        // Only allow this next-button flow when we're on the 2nd dialogue (index 1)
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
