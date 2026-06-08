using UnityEngine;
using Cinemachine;
using TMPro;
using System.Collections;

public class ReadableNote : MonoBehaviour
{
    [Header("Cinemachine 相机")]
    public CinemachineVirtualCamera vcam;   // 拖入 VCam_Notepad

    [Header("UI 面板")]
    public GameObject infoPanel;
    public TextMeshProUGUI infoTextUI;

    [Header("3D 提示文字")]
    public GameObject pressE_3D;            // Notepad 上方的 "Press E" 3D 文字

    [Header("高亮材质")]
    public Material normalMaterial;          // 普通材质
    public Material highlightMaterial;       // 高亮材质
    public Renderer noteRenderer;            // Notepad 的 Renderer

    [Header("文字内容")]
    [TextArea(5, 10)]
    public string infoText = "笔记内容...";

    private bool isViewing = false;
    private bool isPlayerNear = false;
    private MonoBehaviour playerController;   // 玩家控制器引用
    private GameObject player;                // 玩家对象引用
    private Renderer[] playerRenderers;       // 玩家的所有渲染器
    private bool playerWasVisible = true;     // 记录玩家原本是否可见

    void Start()
    {
        // 获取玩家对象
        player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            // 获取玩家控制器
            playerController = player.GetComponent<StarterAssets.FirstPersonController>();
            // 获取玩家身上的所有渲染器
            playerRenderers = player.GetComponentsInChildren<Renderer>();

            // 记录玩家原本的可见状态
            if (playerRenderers != null && playerRenderers.Length > 0)
            {
                playerWasVisible = playerRenderers[0].enabled;
            }
        }

        // 初始状态：VCam 优先级为 0
        if (vcam != null)
            vcam.Priority = 0;

        // UI 面板默认隐藏
        if (infoPanel != null)
            infoPanel.SetActive(false);

        // 3D 提示文字默认隐藏
        if (pressE_3D != null)
            pressE_3D.SetActive(false);

        // 应用普通材质
        if (noteRenderer != null && normalMaterial != null)
            noteRenderer.material = normalMaterial;
    }

    void Update()
    {
        // 玩家在范围内且按下 E 键，且没有正在查看
        if (isPlayerNear && Input.GetKeyDown(KeyCode.E) && !isViewing)
        {
            StartViewing();
        }
        // 正在查看时按 E 键退出
        else if (isViewing && Input.GetKeyDown(KeyCode.E))
        {
            StopViewing();
        }
    }

    void StartViewing()
    {
        isViewing = true;

        // 禁用玩家控制器（锁定移动和视角）
        if (playerController != null)
            playerController.enabled = false;

        // 隐藏玩家模型（避免在镜头中看到胶囊人）
        SetPlayerVisible(false);

        // 启用 Cinemachine 相机
        if (vcam != null)
            vcam.Priority = 10;

        // 显示 UI 面板
        if (infoPanel != null)
        {
            infoTextUI.text = infoText;
            infoPanel.SetActive(true);
        }

        // 隐藏 3D 提示文字
        if (pressE_3D != null)
            pressE_3D.SetActive(false);

        // 恢复普通材质（取消高亮）
        if (noteRenderer != null && normalMaterial != null)
            noteRenderer.material = normalMaterial;

        // 解锁鼠标
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void StopViewing()
    {
        isViewing = false;

        // 恢复玩家控制器
        if (playerController != null)
            playerController.enabled = true;

        // 注意：不恢复玩家模型！保持隐藏状态
        // 第一人称游戏本来就不该看到自己的身体
        // SetPlayerVisible(true);  // 这行已删除

        // 恢复原来的相机（优先级设回 0）
        if (vcam != null)
            vcam.Priority = 0;

        // 隐藏 UI 面板
        if (infoPanel != null)
            infoPanel.SetActive(false);

        // 锁定鼠标
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // 控制玩家模型可见性的辅助方法
    void SetPlayerVisible(bool visible)
    {
        if (playerRenderers != null)
        {
            foreach (Renderer r in playerRenderers)
            {
                r.enabled = visible;
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = true;

            // 显示 3D 提示文字
            if (pressE_3D != null)
                pressE_3D.SetActive(true);

            // 应用高亮材质
            if (noteRenderer != null && highlightMaterial != null)
                noteRenderer.material = highlightMaterial;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = false;

            // 隐藏 3D 提示文字
            if (pressE_3D != null)
                pressE_3D.SetActive(false);

            // 恢复普通材质
            if (noteRenderer != null && normalMaterial != null)
                noteRenderer.material = normalMaterial;
        }
    }
}