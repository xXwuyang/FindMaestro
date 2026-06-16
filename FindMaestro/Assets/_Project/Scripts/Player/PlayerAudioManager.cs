using UnityEngine;

public class PlayerAudioManager : MonoBehaviour
{
    // 在 Unity 编辑器中把这个变量拖拽赋值
    public AudioClip alarmSound;
    private AudioSource audioSource;

    void Start()
    {
        // 获取玩家身上的 AudioSource 组件
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    // 这个方法供触发“重置”逻辑的地方调用
    public void PlayResetSound()
    {
        if (alarmSound != null && audioSource != null)
        {
            // PlayOneShot 用于播放短促、不重叠的音效
            audioSource.PlayOneShot(alarmSound);
        }
    }
}