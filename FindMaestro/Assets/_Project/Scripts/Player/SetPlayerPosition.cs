using UnityEngine;

public class SetPlayerPosition : MonoBehaviour
{
    public Transform spawnPoint;  // Õœ»ÎSpawnPoint

    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null && spawnPoint != null)
        {
            player.transform.position = spawnPoint.position;
            player.transform.rotation = spawnPoint.rotation;
        }
    }
}