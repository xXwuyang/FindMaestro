using UnityEngine;
using Cinemachine;
using TMPro;
using System.Collections;

public class ReadableNote : MonoBehaviour
{
    [Header("Cinemachine 相机")]
    public CinemachineVirtualCamera vcam;   // 每个物品独立的 VCam

    [Header("详情阅读 UI")]
    public GameObject infoPanel;            // 显示详细文字的面板（带滚动条）
    public TextMeshProUGUI infoTextUI;      // 详情文字

    [Header("提示 UI（全局共用）")]
    public GameObject hintPanel;            // 屏幕固定位置的提示面板
    public TextMeshProUGUI hintTextUI;      // 提示文字内容

    [Header("3D 提示文字")]
    public GameObject pressE_3D;            // 物体上方的 3D "Press E" 文字

    [Header("高亮材质")]
    public Material normalMaterial;
    public Material highlightMaterial;
    public Renderer noteRenderer;

    [Header("文字内容")]
    [TextArea(5, 10)]
    public string infoText = "笔记内容...";

    // 全局共享计数器（所有可交互物体共用前两次提示）
    private static int globalHintCount = 0;
    private const int MAX_HINT_COUNT = 2;

    // 提示文字（完整说明）
    private string hintMessage = "When an object shows a yellow stroke, press E can learn more about this clue";

    private GameObject player;
    private StarterAssets.FirstPersonController playerController;
    private Renderer[] playerRenderers;
    private bool isViewing = false;
    private bool isPlayerNear = false;

    // Quick Outline（可选）
    private Outline objectOutline;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerController = player.GetComponent<StarterAssets.FirstPersonController>();
            playerRenderers = player.GetComponentsInChildren<Renderer>();
        }

        objectOutline = GetComponent<Outline>();
        if (objectOutline != null) objectOutline.enabled = false;

        if (vcam != null) vcam.Priority = 0;
        if (infoPanel != null) infoPanel.SetActive(false);
        if (hintPanel != null) hintPanel.SetActive(false);
        if (pressE_3D != null) pressE_3D.SetActive(false);
        if (noteRenderer != null && normalMaterial != null)
            noteRenderer.material = normalMaterial;
    }

    void Update()
    {
        if (isPlayerNear && Input.GetKeyDown(KeyCode.E) && !isViewing)
        {
            StartViewing();
        }
        else if (isViewing && Input.GetKeyDown(KeyCode.E))
        {
            StopViewing();
        }
    }

    void StartViewing()
    {
        isViewing = true;

        if (playerController != null) playerController.enabled = false;
        if (playerRenderers != null)
        {
            foreach (Renderer r in playerRenderers) r.enabled = false;
        }

        if (vcam != null) vcam.Priority = 10;
        if (infoPanel != null)
        {
            infoTextUI.text = infoText;
            infoPanel.SetActive(true);
        }

        // 查看时隐藏提示面板和3D文字
        if (hintPanel != null) hintPanel.SetActive(false);
        if (pressE_3D != null) pressE_3D.SetActive(false);
        if (noteRenderer != null && normalMaterial != null)
            noteRenderer.material = normalMaterial;
        if (objectOutline != null) objectOutline.enabled = false;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void StopViewing()
    {
        isViewing = false;

        if (playerController != null) playerController.enabled = true;
        // 不恢复玩家模型（保持纯第一人称）
        if (vcam != null) vcam.Priority = 0;
        if (infoPanel != null) infoPanel.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = true;

            // 前两次：同时显示 UI 提示面板 和 3D "Press E" 文字
            if (globalHintCount < MAX_HINT_COUNT)
            {
                // 屏幕下方的 UI 提示面板（完整说明）
                if (hintPanel != null && hintTextUI != null)
                {
                    hintTextUI.text = hintMessage;
                    hintPanel.SetActive(true);
                }

                // 物体上方的 3D 文字（简短的 "Press E"）
                if (pressE_3D != null)
                {
                    // 可以确保 3D 文字上的 TextMeshPro 组件内容已经是 "Press E"
                    pressE_3D.SetActive(true);
                }

                globalHintCount++;
            }

            // 高亮材质始终启用（无论第几次）
            if (noteRenderer != null && highlightMaterial != null)
                noteRenderer.material = highlightMaterial;
            if (objectOutline != null)
                objectOutline.enabled = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = false;

            // 隐藏所有提示
            if (hintPanel != null) hintPanel.SetActive(false);
            if (pressE_3D != null) pressE_3D.SetActive(false);
            if (noteRenderer != null && normalMaterial != null)
                noteRenderer.material = normalMaterial;
            if (objectOutline != null)
                objectOutline.enabled = false;
        }
    }
}