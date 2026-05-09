using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Unity.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

/// <summary>
/// Class which manages the game
/// </summary>
public class GameManager : MonoBehaviour
{
    // The script that manages all others
    public static GameManager instance = null;

    [Tooltip("The player gameobject")]
    public GameObject player = null;

    [Header("Scores")]
    // The current player score in the game
    [Tooltip("The player's score")]
    [SerializeField] private int gameManagerScore = 0;

    // Static getter/setter for player score (for convenience)
    public static int score
    {
        get
        {
            return instance.gameManagerScore;
        }
        set
        {
            instance.gameManagerScore = value;
        }
    }

    // The highest score obtained by this player
    [Tooltip("The highest score acheived on this device")]
    public int highScore = 0;

    [Header("Game Progress / Victory Settings")]
    [Tooltip("Whether the game is winnable or not \nDefault: true")]
    public bool gameIsWinnable = true;
    [Tooltip("The number of enemies that must be defeated to win the game")]
    public int enemiesToDefeat = 10;

    // The number of enemies defeated in game
    private int enemiesDefeated = 0;

    [Tooltip("Whether or not to print debug statements about whether the game can be won or not according to the game manager's" +
        " search at start up")]
    public bool printDebugOfWinnableStatus = true;
    [Tooltip("Page index in the UIManager to go to on winning the game")]
    public int gameVictoryPageIndex = 0;
    [Tooltip("The effect to create upon winning the game")]
    public GameObject victoryEffect;

    //The number of enemies observed by the game manager in this scene at start up"
    private int numberOfEnemiesFoundAtStart;

    // ==================== Level 2 Settings ====================
    [Header("Level 2 Settings")]
    [Tooltip("Whether Level 2 is enabled")]
    public bool level2Enabled = true;
    [Tooltip("行星图案预制体")]
    public GameObject planetPrefab;
    [Tooltip("Level 2 需要击败的敌人数")]
    public int level2EnemiesToDefeat = 9;
    [Tooltip("Level 2 背景颜色")]
    public Color level2BackgroundColor = new Color(0.5f, 0.3f, 0.7f);
    [Tooltip("Level 2 的 UI 目标文字")]
    public string level2ObjectiveText = "Level 2: Defeat the planet's minions!";

    [Header("Level 2 Health")]
    [Tooltip("Level 2 玩家的最大生命值")]
    public int level2MaxHealth = 5;

    // Level 2 内部变量
    private bool level2Started = false;
    private int level2EnemiesDefeated = 0;
    private bool inLevel2 = false;
    private PlanetSpawner currentPlanet;
    private Color originalBackgroundColor;
    private Camera mainCamera;
    // ==================== Level 2 End ====================

    [Header("Game Over Settings:")]
    [Tooltip("The index in the UI manager of the game over page")]
    public int gameOverPageIndex = 0;
    [Tooltip("The game over effect to create when the game is lost")]
    public GameObject gameOverEffect;

    // Whether or not the game is over
    [HideInInspector]
    public bool gameIsOver = false;

    /// <summary>
    /// Description:
    /// Standard Unity function called when the script is loaded, called before start
    /// </summary>
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            DestroyImmediate(this);
        }

        if ((player == null) && (FindObjectOfType<Controller>() != null))
        {
            player = FindObjectOfType<Controller>().gameObject;
        }
        else if ((player == null) && (SceneManager.GetActiveScene().name != "MainMenu"))
        {
            Debug.Log("Player is not set and cannot find it in the scene. This is not a problem in non-playable scenes, such as the Main Menu.");
        }
    }

    /// <summary>
    /// Description:
    /// Standard Unity function called once before the first Update
    /// </summary>
    private void Start()
    {
        HandleStartUp();
    }

    /// <summary>
    /// Description:
    /// Handles necessary activities on start up
    /// </summary>
    void HandleStartUp()
    {
        if (PlayerPrefs.HasKey("highscore"))
        {
            highScore = PlayerPrefs.GetInt("highscore");
        }
        if (PlayerPrefs.HasKey("score"))
        {
            score = PlayerPrefs.GetInt("score");
        }
        UpdateUIElements();
        if (printDebugOfWinnableStatus)
        {
            FigureOutHowManyEnemiesExist();
        }

        // 保存原始背景颜色
        mainCamera = Camera.main;
        if (mainCamera != null)
        {
            originalBackgroundColor = mainCamera.backgroundColor;
        }

        // ===== 新增：游戏开始时设置 5 条命 =====
        SetPlayerMaxHealth(5);
    }

    /// <summary>
    /// 设置玩家的最大生命值（游戏开始时调用）
    /// </summary>
    void SetPlayerMaxHealth(int maxHealth)
    {
        if (player != null)
        {
            Health playerHealth = player.GetComponent<Health>();
            if (playerHealth != null)
            {
                playerHealth.maximumHealth = maxHealth;
                playerHealth.currentHealth = maxHealth;

                // 更新 UI 图标
                if (UIManager.instance != null)
                {
                    UIManager.instance.UpdateHealthIcons(maxHealth);
                }

                Debug.Log($"Player max health set to {maxHealth}");
            }
        }
    }

    /// <summary>
    /// Description:
    /// Searches the level for all spawners and static enemies.
    /// </summary>
    private void FigureOutHowManyEnemiesExist()
    {
        List<EnemySpawner> enemySpawners = FindObjectsOfType<EnemySpawner>().ToList();
        List<Enemy> staticEnemies = FindObjectsOfType<Enemy>().ToList();

        int numberOfInfiniteSpawners = 0;
        int enemiesFromSpawners = 0;
        int enemiesFromStatic = staticEnemies.Count;
        foreach (EnemySpawner enemySpawner in enemySpawners)
        {
            if (enemySpawner.spawnInfinite)
            {
                numberOfInfiniteSpawners += 1;
            }
            else
            {
                enemiesFromSpawners += enemySpawner.maxSpawn;
            }
        }
        numberOfEnemiesFoundAtStart = enemiesFromSpawners + enemiesFromStatic;

        if (gameIsWinnable)
        {
            if (numberOfInfiniteSpawners > 0)
            {
                Debug.Log("There are " + numberOfInfiniteSpawners + " infinite spawners so the level will always be winnable");
            }
            else if (enemiesToDefeat > numberOfEnemiesFoundAtStart)
            {
                Debug.LogWarning("There are " + enemiesToDefeat + " enemies to defeat but only " + numberOfEnemiesFoundAtStart + " enemies found at start");
            }
            else
            {
                Debug.Log("There are " + enemiesToDefeat + " enemies to defeat and " + numberOfEnemiesFoundAtStart + " enemies found at start");
            }
        }
    }

    /// <summary>
    /// Description:
    /// Increments the number of enemies defeated by 1
    /// </summary>
    public void IncrementEnemiesDefeated()
    {
        if (inLevel2)
        {
            // Level 2 模式
            level2EnemiesDefeated++;
            UpdateUIElements();

            Debug.Log($"Level 2 Progress: {level2EnemiesDefeated}/{level2EnemiesToDefeat}");

            if (level2EnemiesDefeated >= level2EnemiesToDefeat)
            {
                CompleteLevel2();
            }

            // 每杀一个敌人，行星移动到新位置
            if (currentPlanet != null)
            {
                currentPlanet.MoveToRandomPosition();
            }
        }
        else
        {
            // Level 1 模式
            enemiesDefeated++;

            if (enemiesDefeated >= enemiesToDefeat && gameIsWinnable)
            {
                if (level2Enabled && planetPrefab != null)
                {
                    StartLevel2();
                }
                else
                {
                    LevelCleared();
                }
            }
        }

        // 动态难度
        UpdateDynamicDifficulty();
    }

    // ==================== Level 2 Methods ====================

    /// <summary>
    /// Starts Level 2
    /// </summary>
    public void StartLevel2()
    {
        inLevel2 = true;
        level2EnemiesDefeated = 0;

        // 改变背景颜色
        if (mainCamera != null)
        {
            mainCamera.backgroundColor = level2BackgroundColor;
        }

        Debug.Log("=== LEVEL 2 STARTED ===");
        Debug.Log($"Defeat {level2EnemiesToDefeat} enemies spawned by the planet.");

        // 生成行星
        SpawnPlanet();
    }

    void SetLevel2PlayerHealth()
    {
        if (player != null)
        {
            Health playerHealth = player.GetComponent<Health>();
            if (playerHealth != null)
            {
                playerHealth.maximumHealth = level2MaxHealth;
                playerHealth.currentHealth = level2MaxHealth;
                playerHealth.SetHealth(level2MaxHealth); // 触发 UI 更新

                if (UIManager.instance != null)
                {
                    UIManager.instance.UpdateHealthIcons(level2MaxHealth);
                }

                Debug.Log($"Level 2: Player max health set to {level2MaxHealth}");
            }
        }
    }

    /// <summary>
    /// 玩家受伤时更新 UI
    /// </summary>
    public void UpdatePlayerHealthUI()
    {
        if (player != null && UIManager.instance != null)
        {
            Health playerHealth = player.GetComponent<Health>();
            if (playerHealth != null)
            {
                UIManager.instance.UpdateHealthIcons(playerHealth.currentHealth);
            }
        }
    }

    /// <summary>
    /// Spawns the planet at a random position on screen
    /// </summary>
    void SpawnPlanet()
    {
        if (planetPrefab != null)
        {
            Vector3 randomPos = GetRandomPositionOnScreen();
            GameObject planetObj = Instantiate(planetPrefab, randomPos, Quaternion.identity);
            currentPlanet = planetObj.GetComponent<PlanetSpawner>();

            if (currentPlanet != null)
            {
                currentPlanet.Initialize(this);
            }
            else
            {
                Debug.LogWarning("Planet prefab is missing PlanetSpawner component!");
            }
        }
        else
        {
            Debug.LogWarning("Planet prefab not assigned in GameManager!");
            CompleteLevel2();
        }
    }

    /// <summary>
    /// Called by PlanetSpawner when an enemy is spawned
    /// </summary>
    public void OnPlanetEnemySpawned()
    {
        Debug.Log("Planet spawned an enemy!");
    }

    /// <summary>
    /// Gets a random position within camera view
    /// </summary>
    Vector3 GetRandomPositionOnScreen()
    {
        if (mainCamera == null) return Vector3.zero;

        float height = mainCamera.orthographicSize;
        float width = height * mainCamera.aspect;

        float x = Random.Range(-width + 1f, width - 1f);
        float y = Random.Range(-height + 1f, height - 1f);

        return new Vector3(x, y, 0);
    }

    /// <summary>
    /// Completes Level 2 and wins the game
    /// </summary>
    void CompleteLevel2()
    {
        inLevel2 = false;

        if (currentPlanet != null)
        {
            Destroy(currentPlanet.gameObject);
        }

        if (mainCamera != null)
        {
            mainCamera.backgroundColor = originalBackgroundColor;
        }

        Debug.Log("=== LEVEL 2 COMPLETE! VICTORY! ===");

        LevelCleared();
    }

    /// <summary>
    /// Increases enemy speed based on score (dynamic difficulty)
    /// </summary>
    void UpdateDynamicDifficulty()
    {
        float speedMultiplier = 1f + (score / 500f);
        speedMultiplier = Mathf.Min(speedMultiplier, 2.5f);

        Enemy[] allEnemies = FindObjectsOfType<Enemy>();
        int enemiesUpdated = 0;
        foreach (Enemy enemy in allEnemies)
        {
            enemy.SetSpeedMultiplier(speedMultiplier);
            enemiesUpdated++;
        }

        if (enemiesUpdated > 0 && score % 100 == 0 && score > 0)
        {
            Debug.Log($"Dynamic Difficulty: Score={score}, Speed Multiplier={speedMultiplier:F2}, Enemies affected={enemiesUpdated}");
        }
    }

    /// <summary>
    /// Call this when player is hurt
    /// </summary>
    public void OnPlayerHurt()
    {
        UpdateDynamicDifficulty();
        UpdatePlayerHealthUI();
    }

    // ==================== Original Methods ====================

    private void OnApplicationQuit()
    {
        SaveHighScore();
        ResetScore();
    }

    public static void AddScore(int scoreAmount)
    {
        score += scoreAmount;
        if (score > instance.highScore)
        {
            SaveHighScore();
        }
        UpdateUIElements();
    }

    public static void ResetScore()
    {
        PlayerPrefs.SetInt("score", 0);
        score = 0;
    }

    public static void SaveHighScore()
    {
        if (score > instance.highScore)
        {
            PlayerPrefs.SetInt("highscore", score);
            instance.highScore = score;
        }
        UpdateUIElements();
    }

    public static void ResetHighScore()
    {
        PlayerPrefs.SetInt("highscore", 0);
        if (instance != null)
        {
            instance.highScore = 0;
        }
        UpdateUIElements();
    }

    public static void UpdateUIElements()
    {
        if (UIManager.instance != null)
        {
            UIManager.instance.UpdateUI();
        }
    }

    public void LevelCleared()
    {
        PlayerPrefs.SetInt("score", score);
        if (UIManager.instance != null)
        {
            if (player != null)
            {
                player.SetActive(false);
            }
            UIManager.instance.allowPause = false;
            UIManager.instance.GoToPage(gameVictoryPageIndex);
            if (victoryEffect != null)
            {
                Instantiate(victoryEffect, transform.position, transform.rotation, null);
            }
        }
    }

    public void GameOver()
    {
        gameIsOver = true;
        if (gameOverEffect != null)
        {
            Instantiate(gameOverEffect, transform.position, transform.rotation, null);
        }
        if (UIManager.instance != null)
        {
            UIManager.instance.allowPause = false;
            UIManager.instance.GoToPage(gameOverPageIndex);
        }
    }
}