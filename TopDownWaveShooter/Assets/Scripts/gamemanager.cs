using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class gamemanager : MonoBehaviour
{
    public static gamemanager instance;

    [Header("Wave Settings")]
    [SerializeField] GameObject enemyPrefab;
    [SerializeField] Transform[] spawnPoints;
    [SerializeField] float timeBetweenWaves = 3f;
    [SerializeField] int baseEnemiesPerWave = 3;
    [SerializeField] int maxWaves = 10;

    [SerializeField] GameObject menuActive;
    [SerializeField] GameObject menuPause;
    [SerializeField] GameObject menuWin;
    [SerializeField] GameObject menuLose;
    [SerializeField] TMP_Text gameGoalCountText;

    public Image playerHPBar;
    public GameObject playerDamageFlash;
    public GameObject player;
    public playerscript playerScript;
    public bool isPaused;
    float timeScaleOrig;
    int gameGoalCount;

    int currentWave = 0;
    bool spawningWave = false;
    public Image playerShield;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        instance = this;
        timeScaleOrig = Time.timeScale;
        player = GameObject.FindWithTag("Player");
        playerScript = player.GetComponent <playerscript>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Cancel"))
            if (menuActive == null)
            {
                statePause();
                menuActive = menuPause;
                menuActive.SetActive(true);
            }
            else if (menuActive == menuPause)
            {
                stateUnpause();
            }
    }
    public void statePause()
    {
        isPaused = true;
        Time.timeScale = 0;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
    public void stateUnpause()
    {
        isPaused = false;
        Time.timeScale = timeScaleOrig;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        menuActive.SetActive(false);
        menuActive = null;
    }
    public void updateGameGoal(int amount)
    {
        gameGoalCount += amount;
        gameGoalCountText.text = gameGoalCount.ToString("F0");
        if (gameGoalCount <= 0 && !spawningWave)
        {
            StartCoroutine(StartNextWave());
        }
    }
      public void youLose()
      {
        statePause();
        menuActive = menuLose;
        menuActive.SetActive(true);
      }

    IEnumerator StartNextWave()
    {
        spawningWave = true;
        yield return new WaitForSeconds(timeBetweenWaves);
        currentWave++;

        if (currentWave >= maxWaves)
        {
            statePause();
            menuActive = menuWin;
            menuActive.SetActive(true);
            yield break;
        }

        int enemiesToSpawn = baseEnemiesPerWave + currentWave;

        for (int i = 0;
            i < enemiesToSpawn;
            i++)
        {
            Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
            Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);
        }

        spawningWave = false;
    }

}
