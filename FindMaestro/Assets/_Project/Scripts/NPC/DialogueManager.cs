using UnityEngine;
using TMPro;
using System.Collections;
using Cinemachine;

public class DialogueManager : MonoBehaviour
{
    [System.Serializable]
    public class DialogueLine
    {
        public string speaker;
        [TextArea(2, 4)]
        public string text;
    }

    public DialogueLine[] dialogueLines;
    public GameObject dialoguePanel;
    public TextMeshProUGUI dialogueText;
    public TextMeshProUGUI speakerNameText;
    public GameObject continuePrompt;
    public GameObject objectivePanel;
    public TextMeshProUGUI objectiveText;
    public float typingSpeed = 0.05f;
    public float objectiveDisplayTime = 3f;

    // 碎片计数器 UI（对话结束后显示）
    public GameObject fragmentCounterUI;

    // --- 新增：全局扫描音效 ---
    public AudioClip scanLoopSound;   // 拖入循环播放的激光/扫描音效
    private AudioSource scanAudioSource;

    // 引用玩家组件
    private GameObject player;
    private StarterAssets.FirstPersonController playerController;
    private CharacterController characterController;
    private CinemachineInputProvider inputProvider;
    private CinemachineVirtualCamera playerVirtualCamera;

    private int index;
    private bool isTyping = false;
    private int originalVcamPriority;

    void Awake()
    {
        // 初始化扫描音效的 AudioSource
        scanAudioSource = gameObject.AddComponent<AudioSource>();
        scanAudioSource.loop = true;
        scanAudioSource.playOnAwake = false;
        scanAudioSource.volume = 0.05f;   // 调低音量，不吓人
        if (scanLoopSound != null)
            scanAudioSource.clip = scanLoopSound;
    }

    void Start()
    {
        if (dialoguePanel != null) dialoguePanel.SetActive(false);
        if (continuePrompt != null) continuePrompt.SetActive(false);
        if (objectivePanel != null) objectivePanel.SetActive(false);
        if (fragmentCounterUI != null) fragmentCounterUI.SetActive(false);

        player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerController = player.GetComponent<StarterAssets.FirstPersonController>();
            characterController = player.GetComponent<CharacterController>();
            inputProvider = player.GetComponent<CinemachineInputProvider>();
            playerVirtualCamera = player.GetComponentInChildren<CinemachineVirtualCamera>();
            if (playerVirtualCamera != null)
                originalVcamPriority = playerVirtualCamera.Priority;
        }
    }

    public void StartDialogue()
    {
        Debug.Log("DialogueManager.StartDialogue 被调用");
        if (dialoguePanel == null)
        {
            Debug.LogError("dialoguePanel 未设置！");
            return;
        }

        // 播放全局扫描音效（循环）
        if (scanAudioSource != null && scanLoopSound != null && !scanAudioSource.isPlaying)
        {
            scanAudioSource.Play();
            Debug.Log("全局扫描音效已开始播放");
        }

        dialoguePanel.SetActive(true);
        Canvas canvas = dialoguePanel.GetComponentInParent<Canvas>();
        if (canvas != null && !canvas.gameObject.activeSelf) canvas.gameObject.SetActive(true);

        // 禁用移动和视角
        if (playerController != null) playerController.enabled = false;
        if (characterController != null) characterController.enabled = false;
        if (inputProvider != null) inputProvider.enabled = false;
        if (playerVirtualCamera != null) playerVirtualCamera.Priority = 0;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (speakerNameText != null)
        {
            speakerNameText.gameObject.SetActive(false);
            speakerNameText.gameObject.SetActive(true);
            speakerNameText.enabled = true;
            speakerNameText.color = Color.white;
        }

        index = 0;
        DisplayCurrentLine();
    }

    void DisplayCurrentLine()
    {
        if (speakerNameText != null)
        {
            speakerNameText.gameObject.SetActive(false);
            speakerNameText.text = dialogueLines[index].speaker;
            speakerNameText.gameObject.SetActive(true);
            speakerNameText.enabled = true;
            speakerNameText.color = Color.white;
        }
        StartCoroutine(TypeLine(dialogueLines[index].text));
    }

    IEnumerator TypeLine(string fullText)
    {
        isTyping = true;
        if (continuePrompt != null) continuePrompt.SetActive(false);

        dialogueText.text = "";
        foreach (char c in fullText.ToCharArray())
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
        if (continuePrompt != null) continuePrompt.SetActive(true);
    }

    void Update()
    {
        if (dialoguePanel != null && dialoguePanel.activeSelf && !isTyping && Input.GetKeyDown(KeyCode.E))
        {
            NextLine();
        }
    }

    void NextLine()
    {
        if (continuePrompt != null) continuePrompt.SetActive(false);
        index++;
        if (index < dialogueLines.Length)
        {
            DisplayCurrentLine();
        }
        else
        {
            dialoguePanel.SetActive(false);
            if (continuePrompt != null) continuePrompt.SetActive(false);
            if (speakerNameText != null)
            {
                speakerNameText.text = "";
                speakerNameText.gameObject.SetActive(false);
            }
            StartCoroutine(ShowObjectiveAndWait());
        }
    }

    IEnumerator ShowObjectiveAndWait()
    {
        if (objectivePanel != null && objectiveText != null)
        {
            objectiveText.text = "AVOID AI BRAINWASHING\nCOLLECT HUMAN CREATIVITY FRAGMENTS\n\nTIPS:Be careful not to get caught by AI surveillance, " +
                "and collect the glowing fragments in the scene at the same time.\n GOOD LUCK";
            objectivePanel.SetActive(true);
        }

        yield return new WaitForSeconds(objectiveDisplayTime);

        if (objectivePanel != null) objectivePanel.SetActive(false);

        // 恢复玩家控制
        if (playerController != null) playerController.enabled = true;
        if (characterController != null) characterController.enabled = true;
        if (inputProvider != null) inputProvider.enabled = true;
        if (playerVirtualCamera != null) playerVirtualCamera.Priority = originalVcamPriority;

        // 显示碎片计数器
        if (fragmentCounterUI != null)
        {
            fragmentCounterUI.SetActive(true);
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // 可选：如果离开第二层时需要主动停止音效，可调用此方法（场景切换时自动销毁，一般不需要）
    public void StopScanSound()
    {
        if (scanAudioSource != null && scanAudioSource.isPlaying)
            scanAudioSource.Stop();
    }

    private void OnDestroy()
    {
        if (scanAudioSource != null)
            scanAudioSource.Stop();
    }
}