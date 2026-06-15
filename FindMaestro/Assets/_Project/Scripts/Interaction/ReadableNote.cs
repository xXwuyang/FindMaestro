using UnityEngine;
using Cinemachine;
using TMPro;

public class ReadableNote : MonoBehaviour
{
    [Header("Cinemachine 相机")]
    public CinemachineVirtualCamera vcam;

    [Header("详情阅读 UI")]
    public GameObject infoPanel;
    public TextMeshProUGUI infoTextUI;
    public RectTransform infoTextRect;

    [Header("提示 UI")]
    public GameObject hintPanel;
    public TextMeshProUGUI hintTextUI;

    [Header("3D 提示")]
    public GameObject pressE_3D;

    [Header("Quick Outline（描边组件）")]
    public Outline outline;   // 直接引用 Quick Outline 组件

    [Header("文本内容")]
    [TextArea(5, 10)]
    public string infoText = "笔记内容...";

    private static int globalHintCount = 0;
    private const int MAX_HINT_COUNT = 2;
    private string hintMessage =
        "When an object shows a yellow stroke, press E can learn more about this clue";

    private GameObject player;
    private StarterAssets.FirstPersonController playerController;
    private bool isViewing = false;
    private bool isPlayerNear = false;

    private Vector2 originalPos;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
            playerController = player.GetComponent<StarterAssets.FirstPersonController>();

        if (vcam != null) vcam.Priority = 0;
        if (infoPanel != null) infoPanel.SetActive(false);
        if (hintPanel != null) hintPanel.SetActive(false);
        if (pressE_3D != null) pressE_3D.SetActive(false);

        // 确保开始时 outline 是关闭的
        if (outline != null)
            outline.enabled = false;
    }

    void Update()
    {
        HandleInput();

        if (isViewing)
            ApplyReadingDistortion();

        // ✔ 稳定版 Outline 控制（关键修复）
        if (outline != null)
        {
            outline.enabled = isPlayerNear;
        }
    }

    void HandleInput()
    {
        if (isPlayerNear && Input.GetKeyDown(KeyCode.E) && !isViewing)
            StartViewing();
        else if (isViewing && Input.GetKeyDown(KeyCode.E))
            StopViewing();
    }

    void StartViewing()
    {
        isViewing = true;

        if (infoTextRect != null)
            originalPos = infoTextRect.anchoredPosition;

        if (ExposureSystem.Instance != null)
        {
            ExposureSystem.Instance.freezeDecay = true;
            ExposureSystem.Instance.AddExposure(12f);
        }

        if (playerController != null)
            playerController.enabled = false;

        if (vcam != null)
            vcam.Priority = 10;

        if (infoPanel != null)
        {
            infoTextUI.text = infoText;
            infoPanel.SetActive(true);
        }

        if (hintPanel != null) hintPanel.SetActive(false);
        if (pressE_3D != null) pressE_3D.SetActive(false);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        UnityEngine.EventSystems.EventSystem.current?.SetSelectedGameObject(null);
    }

    void StopViewing()
    {
        isViewing = false;

        if (ExposureSystem.Instance != null)
            ExposureSystem.Instance.freezeDecay = false;

        if (playerController != null)
            playerController.enabled = true;

        if (vcam != null)
            vcam.Priority = 0;

        if (infoPanel != null)
            infoPanel.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void ApplyReadingDistortion()
    {
        if (ExposureSystem.Instance == null) return;
        if (infoTextUI == null) return;

        float t = ExposureSystem.Instance.exposure / ExposureSystem.Instance.maxExposure;

        float alpha = Mathf.Lerp(1f, 0.15f, t);
        infoTextUI.color = new Color(1f, 1f, 1f, alpha);

        if (infoTextRect != null)
        {
            float intensity = t * 10f;
            Vector2 shake = Random.insideUnitCircle * intensity;
            infoTextRect.anchoredPosition = originalPos + shake;
        }

        if (t > 0.35f)
        {
            infoTextUI.text = GetCorruptedText(infoText, Mathf.Lerp(0f, 0.4f, t));
        }
        else
        {
            infoTextUI.text = infoText;
        }
    }

    string GetCorruptedText(string input, float intensity)
    {
        char[] chars = input.ToCharArray();
        for (int i = 0; i < chars.Length; i++)
        {
            if (chars[i] == ' ') continue;
            if (Random.value < intensity)
                chars[i] = GetRandomGlitchChar();
        }
        return new string(chars);
    }

    char GetRandomGlitchChar()
    {
        string pool = "!@#$%^&*_-+=?/\\|[]{}<>;:0123456789";
        return pool[Random.Range(0, pool.Length)];
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = true;
            // 高亮将在 Update 中通过 outline.enabled = true 开启

            if (globalHintCount < MAX_HINT_COUNT)
            {
                if (hintPanel != null && hintTextUI != null)
                {
                    hintTextUI.text = hintMessage;
                    hintPanel.SetActive(true);
                }
                if (pressE_3D != null)
                    pressE_3D.SetActive(true);
                globalHintCount++;
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = false;
            // 高亮将在 Update 中通过 outline.enabled = false 关闭

            if (hintPanel != null)
                hintPanel.SetActive(false);
            if (pressE_3D != null)
                pressE_3D.SetActive(false);
        }
    }
}