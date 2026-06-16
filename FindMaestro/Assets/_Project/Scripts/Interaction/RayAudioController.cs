using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class RayAudioController : MonoBehaviour
{
    public AudioClip raySound;
    private AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = raySound;
        audioSource.loop = true; // 打开循环
        audioSource.playOnAwake = false;
    }

    // 玩家进入射线范围
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (!audioSource.isPlaying)
                audioSource.Play();
        }
    }

    // 玩家离开射线范围
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (audioSource.isPlaying)
                audioSource.Stop();
        }
    }
}