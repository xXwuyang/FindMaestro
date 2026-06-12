using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class NPCdialogue : MonoBehaviour
{
    [System.Serializable]
    public class DialogueLine
    {
        public string speaker;
        [TextArea(2, 4)]
        public string text;
    }

    public List<DialogueLine> dialogueLines;
    public GameObject dialoguePanel;
    public TextMeshProUGUI dialogueText;
    public float typingSpeed = 0.05f;

    public bool autoStart = false;
    public float autoStartDelay = 0f;
    public GameObject playerController;

    private int currentLine = 0;
    private bool isTyping = false;
    private bool isDialogueActive = false;
    private Coroutine typingCoroutine;
    private StarterAssets.FirstPersonController fpsController;

    void Start()
    {
        if (dialoguePanel != null) dialoguePanel.SetActive(false);
        if (playerController != null)
            fpsController = playerController.GetComponent<StarterAssets.FirstPersonController>();
        if (autoStart)
            StartCoroutine(AutoStartCoroutine());
    }

    IEnumerator AutoStartCoroutine()
    {
        yield return new WaitForSeconds(autoStartDelay);
        StartDialogue();
    }

    public void StartDialogue()
    {
        Debug.Log("StartDialogue 被调用");
        if (isDialogueActive) return;

        // 确保 dialoguePanel 及其所有父级都被激活
        Transform t = dialoguePanel.transform;
        while (t != null)
        {
            if (!t.gameObject.activeSelf)
            {
                t.gameObject.SetActive(true);
                Debug.Log($"激活父物体: {t.name}");
            }
            t = t.parent;
        }

        // 如果父物体是 Canvas，确保 Canvas 组件启用
        Canvas canvas = dialoguePanel.GetComponentInParent<Canvas>();
        if (canvas != null && !canvas.enabled)
        {
            canvas.enabled = true;
            Debug.Log("重新启用 Canvas");
        }

        // 如果有 CanvasGroup，确保 alpha 不为 0 且 blocksRaycasts 为 true（如果需要交互）
        CanvasGroup cg = dialoguePanel.GetComponent<CanvasGroup>();
        if (cg != null)
        {
            cg.alpha = 1f;
            cg.interactable = true;
            cg.blocksRaycasts = true;
        }

        dialoguePanel.SetActive(true);
        Debug.Log($"dialoguePanel activeSelf: {dialoguePanel.activeSelf}");

        if (fpsController != null) fpsController.enabled = false;
        currentLine = 0;
        isDialogueActive = true;
        NextLine();
    }

    void Update()
    {
        if (!isDialogueActive) return;
        if (!isTyping && Input.GetKeyDown(KeyCode.E))
        {
            NextLine();
        }
    }

    void NextLine()
    {
        if (currentLine < dialogueLines.Count)
        {
            DialogueLine line = dialogueLines[currentLine];
            string displayText = $"<b>{line.speaker}:</b> {line.text}";
            typingCoroutine = StartCoroutine(TypeText(displayText));
            currentLine++;
        }
        else
        {
            EndDialogue();
        }
    }

    IEnumerator TypeText(string fullText)
    {
        isTyping = true;
        dialogueText.text = "";
        foreach (char c in fullText)
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }
        isTyping = false;
    }

    void EndDialogue()
    {
        isDialogueActive = false;
        dialoguePanel.SetActive(false);
        if (fpsController != null) fpsController.enabled = true;
        OnDialogueFinished();
    }

    void OnDialogueFinished()
    {
        Debug.Log("对话结束");
    }
}