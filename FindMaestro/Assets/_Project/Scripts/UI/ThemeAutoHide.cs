using UnityEngine;
public class ThemeAutoHide : MonoBehaviour
{
    void Start() { Invoke("Hide", 5f); }
    void Hide() { gameObject.SetActive(false); }
}