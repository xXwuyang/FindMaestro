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

    // 新增：碎片计数器 UI（对话结束后才显示）
    public GameObject fragmentCounterUI;

    // 引用玩家组件
    private GameObject player;
    private StarterAssets.FirstPersonController playerController;
    private CharacterController characterController;
    private CinemachineInputProvider inputProvider;
    private CinemachineVirtualCamera playerVirtualCamera;

    private int index;
    private bool isTyping = false;
    private int originalVcamPriority;

    void Start()
    {
        if (dialoguePanel != null) dialoguePanel.SetActive(false);
        if (continuePrompt != null) continuePrompt.SetActive(false);
        if (objectivePanel != null) objectivePanel.SetActive(false);
        if (fragmentCounterUI != null) fragmentCounterUI.SetActive(false); // 初始隐藏

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

        dialoguePanel.SetActive(true);
        Canvas canvas = dialoguePanel.GetComponentInParent<Canvas>();
        if (canvas != null && !canvas.gameObject.activeSelf) canvas.gameObject.SetActive(true);

        // 禁用移动（会产生警告但不影响游戏）
        if (playerController != null) playerController.enabled = false;
        if (characterController != null) characterController.enabled = false;

        // 禁用视角旋转
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

        // 对话结束，显示碎片计数器
        Debug.Log("准备激活 fragmentCounterUI，引用是否为 null: " + (fragmentCounterUI == null));
        if (fragmentCounterUI != null)
        {
            fragmentCounterUI.SetActive(true);
            Debug.Log("已激活 fragmentCounterUI，当前 activeSelf: " + fragmentCounterUI.activeSelf);
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Debug.Log("目标提示结束，玩家恢复控制，碎片计数器已显示");
    }
}