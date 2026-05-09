using System.Collections;
using UnityEngine;

public class PlanetSpawner : MonoBehaviour
{
    private GameManager gameManager;

    [Tooltip("How often the planet moves to a new position (seconds)")]
    public float moveInterval = 5f;

    [Tooltip("How many enemies to spawn each wave")]
    public int enemiesPerWave = 3;

    [Tooltip("How many waves total (0 = infinite)")]
    public int totalWaves = 3;

    [Tooltip("Enemy prefab to spawn")]
    public GameObject enemyPrefab;

    [Tooltip("Radius around planet to spawn enemies")]
    public float spawnRadius = 2.5f;

    private SpriteRenderer spriteRenderer;
    private int currentWave = 0;
    private bool spawningComplete = false;

    public void Initialize(GameManager manager)
    {
        gameManager = manager;
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (enemyPrefab == null)
        {
            Debug.LogError("Enemy prefab not assigned to PlanetSpawner!");
        }

        StartCoroutine(SpawnWaves());
    }

    IEnumerator SpawnWaves()
    {
        while (currentWave < totalWaves && gameManager != null && !gameManager.gameIsOver)
        {
            currentWave++;
            Debug.Log($"Planet Wave {currentWave}/{totalWaves} - Spawning {enemiesPerWave} enemies");

            for (int i = 0; i < enemiesPerWave; i++)
            {
                SpawnEnemy();
                yield return new WaitForSeconds(0.2f);
            }

            if (currentWave < totalWaves)
            {
                yield return new WaitForSeconds(moveInterval);
                MoveToRandomPosition();
            }
        }

        spawningComplete = true;
        Debug.Log("Planet has finished spawning all enemies!");
    }

    public void MoveToRandomPosition()
    {
        if (spawningComplete) return;

        Camera cam = Camera.main;
        if (cam == null) return;

        float height = cam.orthographicSize;
        float width = height * cam.aspect;

        float x = Random.Range(-width + 1.5f, width - 1.5f);
        float y = Random.Range(-height + 1.5f, height - 1.5f);

        transform.position = new Vector3(x, y, 0);
        StartCoroutine(FlashEffect());
    }

    IEnumerator FlashEffect()
    {
        if (spriteRenderer == null) yield break;

        Color originalColor = spriteRenderer.color;
        spriteRenderer.color = Color.yellow;
        yield return new WaitForSeconds(0.2f);
        spriteRenderer.color = originalColor;
    }

    void SpawnEnemy()
    {
        if (enemyPrefab == null)
        {
            Debug.LogError("Cannot spawn enemy: enemyPrefab is null!");
            return;
        }

        Vector2 randomCircle = Random.insideUnitCircle * spawnRadius;
        Vector3 spawnPos = transform.position + new Vector3(randomCircle.x, randomCircle.y, 0);

        GameObject newEnemy = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);

        Enemy enemyScript = newEnemy.GetComponent<Enemy>();
        if (enemyScript != null)
        {
            enemyScript.spawnedByPlanet = true;
        }

        Debug.Log($"Enemy spawned at offset: ({randomCircle.x:F1}, {randomCircle.y:F1})");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player touched the planet! Taking damage...");

            Health playerHealth = other.GetComponent<Health>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(1);

                // 确保 UI 更新
                if (UIManager.instance != null)
                {
                    UIManager.instance.UpdateHealthIcons(playerHealth.currentHealth);
                }
            }

            StartCoroutine(DamageFlashEffect());

            Rigidbody2D rb = other.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                Vector2 knockback = (other.transform.position - transform.position).normalized * 5f;
                rb.velocity = knockback;
            }
        }
    }

    IEnumerator DamageFlashEffect()
    {
        if (spriteRenderer == null) yield break;

        Color originalColor = spriteRenderer.color;
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.15f);
        spriteRenderer.color = originalColor;
    }
}