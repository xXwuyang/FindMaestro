using UnityEngine;
using TMPro;
using System.Collections;

public class TutorialManager : MonoBehaviour
{
    [Header("UI 组件")]
    public GameObject tutorialPanel;
    public TextMeshProUGUI tutorialText;

    [Header("提示文本")]
    public string movePrompt = "使用 WASD 移动";
    public string lookPrompt = "移动鼠标 转动视角";

    [Header("完成提示")]
    public string completeMessage = "Now, let's start exploring this dungeon";
    public float completeMessageDelay = 1f;      // 教程完成后等待多久显示完成语句
    public float completeMessageDuration = 3f;   // 完成语句显示多久后开始闪烁
    public float blinkDuration = 1.5f;           // 闪烁持续时间

    [Header("视角检测参数")]
    public float requiredRotation = 60f;

    private int stage = 0;
    private bool tutorialActive = true;
    private float accumulatedRotation = 0f;
    private bool isMouseLookCompleted = false;

    void Start()
    {
        tutorialPanel.SetActive(false);
    }

    public void StartTutorial()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        tutorialPanel.SetActive(true);
        ShowStage(0);
        tutorialActive = true;

        accumulatedRotation = 0f;
        isMouseLookCompleted = false;
    }

    void Update()
    {
        if (!tutorialActive) return;

        if (stage == 0 && IsMoveKeyPressed())
        {
            CompleteStage();
        }
        else if (stage == 1 && !isMouseLookCompleted)
        {
            float mouseX = Input.GetAxis("Mouse X");
            if (Mathf.Abs(mouseX) > 0.05f)
            {
                accumulatedRotation += Mathf.Abs(mouseX);

                if (accumulatedRotation >= requiredRotation)
                {
                    isMouseLookCompleted = true;
                    CompleteStage();
                }
            }
        }
    }

    bool IsMoveKeyPressed()
    {
        return Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.A) ||
               Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.D);
    }

    void CompleteStage()
    {
        stage++;
        if (stage >= 2)
        {
            tutorialActive = false;
            tutorialPanel.SetActive(false);
            Debug.Log("新手教程完成");

            // 等待一段时间后显示完成语句
            StartCoroutine(ShowCompleteMessageWithDelay());
        }
        else
        {
            ShowStage(stage);
        }
    }

    void ShowStage(int stageIndex)
    {
        switch (stageIndex)
        {
            case 0:
                tutorialText.text = movePrompt;
                break;
            case 1:
                tutorialText.text = lookPrompt;
                accumulatedRotation = 0f;
                isMouseLookCompleted = false;
                break;
        }
    }

    IEnumerator ShowCompleteMessageWithDelay()
    {
        // 等待延迟
        yield return new WaitForSeconds(completeMessageDelay);

        // 显示完成消息
        tutorialText.text = completeMessage;
        tutorialPanel.SetActive(true);

        // 保持显示指定时间
        yield return new WaitForSeconds(completeMessageDuration);

        // 闪烁消失（营造神秘感）
        yield return StartCoroutine(BlinkAndFade());

        // 最终确保隐藏
        tutorialPanel.SetActive(false);
        Debug.Log("探索提示已消失");
    }

    IEnumerator BlinkAndFade()
    {
        CanvasGroup cg = tutorialPanel.GetComponent<CanvasGroup>();
        if (cg == null)
        {
            cg = tutorialPanel.AddComponent<CanvasGroup>();
        }

        float timer = 0f;
        float blinkSpeed = 8f;  // 闪烁速度（越快闪烁越密）

        while (timer < blinkDuration)
        {
            timer += Time.deltaTime;
            // 使用正弦波制造忽明忽暗的闪烁效果
            float alpha = Mathf.Lerp(1f, 0f, timer / blinkDuration);
            // 叠加正弦波产生闪烁感
            alpha = alpha * (0.5f + 0.5f * Mathf.Sin(timer * blinkSpeed * Mathf.PI * 2f));
            cg.alpha = alpha;
            yield return null;
        }

        cg.alpha = 0f;
    }
}