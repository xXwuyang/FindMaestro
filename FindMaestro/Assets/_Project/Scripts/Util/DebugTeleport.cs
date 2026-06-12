using UnityEngine;

public class DebugTeleport : MonoBehaviour
{
    [Header("传送目标（第二层起始点）")]
    public Transform teleportTarget;

    [Header("第二层对话触发器")]
    public GameObject secondLevelDialogueTrigger;

    private GameObject player;
    private CharacterController characterController;
    private StarterAssets.FirstPersonController fpsController;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            characterController = player.GetComponent<CharacterController>();
            fpsController = player.GetComponent<StarterAssets.FirstPersonController>();
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            TeleportToSecondLevel();
        }
    }

    void TeleportToSecondLevel()
    {
        if (player == null || teleportTarget == null)
        {
            Debug.LogError("玩家或传送目标未设置！");
            return;
        }

        // 临时禁用控制器，防止位置被重置
        if (fpsController != null) fpsController.enabled = false;
        if (characterController != null) characterController.enabled = false;

        // 强制传送
        player.transform.position = teleportTarget.position;
        player.transform.rotation = teleportTarget.rotation;

        // 重新启用控制器
        if (characterController != null) characterController.enabled = true;
        if (fpsController != null) fpsController.enabled = true;

        Debug.Log($"已传送到第二层: {teleportTarget.position}");

        // 激活并启动对话
        if (secondLevelDialogueTrigger != null)
        {
            secondLevelDialogueTrigger.SetActive(true);
            DialogueManager dm = secondLevelDialogueTrigger.GetComponent<DialogueManager>();
            if (dm != null)
            {
                dm.StartDialogue();
                Debug.Log("已启动第二层对话");
            }
            else
            {
                Debug.LogError("对话触发器上没有 DialogueManager 组件！");
            }
        }
        else
        {
            Debug.LogError("未设置 secondLevelDialogueTrigger！");
        }
    }
}