using UnityEngine;

public class CreativityFragment : MonoBehaviour
{
    public int fragmentValue = 1;
    private bool isCollected = false;

    void OnTriggerEnter(Collider other)
    {
        if (isCollected) return;
        if (other.CompareTag("Player"))
        {
            isCollected = true;
            FragmentManager.Instance.AddFragment(fragmentValue);
            // 禁用物体，而不是销毁，以便后续重置时重新激活
            gameObject.SetActive(false);
        }
    }

    // 供 FragmentManager 调用的重置方法
    public void ResetFragment()
    {
        isCollected = false;
        gameObject.SetActive(true);
    }
}