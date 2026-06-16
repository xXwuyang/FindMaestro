using UnityEngine;

public class OldDevKit : MonoBehaviour
{
    public EndingManager endingManager;   // 在 Inspector 中手动拖拽

    void Start()
    {
        if (endingManager == null)
            endingManager = FindObjectOfType<EndingManager>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("彩蛋被触碰！");   // 检查控制台
            if (endingManager != null)
            {
                endingManager.SetHasOldKit(true);
                endingManager.OnEggFound();
            }
            Destroy(gameObject);
        }
    }
}