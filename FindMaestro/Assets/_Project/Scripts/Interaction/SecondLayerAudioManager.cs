using UnityEngine;

public class SecondLayerAudioManager : MonoBehaviour
{
    [Header("…®√Ë“Ù–ß£®—≠ª∑£©")]
    public AudioClip scanClip;
    [Range(0f, 1f)]
    public float scanVolume = 0.3f;

    private AudioSource scanAudioSource;
    private bool isInSecondLayer = false;

    void Awake()
    {
        scanAudioSource = gameObject.AddComponent<AudioSource>();
        scanAudioSource.clip = scanClip;
        scanAudioSource.loop = true;
        scanAudioSource.volume = scanVolume;
        scanAudioSource.playOnAwake = false;
    }

    public void EnterSecondLayer()
    {
        if (!isInSecondLayer && scanClip != null)
        {
            isInSecondLayer = true;
            if (!scanAudioSource.isPlaying)
                scanAudioSource.Play();
        }
    }

    public void ExitSecondLayer()
    {
        if (isInSecondLayer)
        {
            isInSecondLayer = false;
            if (scanAudioSource.isPlaying)
                scanAudioSource.Stop();
        }
    }
}