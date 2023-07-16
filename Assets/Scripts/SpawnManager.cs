using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class SpawnManager : MonoBehaviour
{
    [System.Serializable]
    public class PowerUpEntry
    {
        public GameObject _powerup;
        public int _spawnWeight = 1;
    }
    
    [System.Serializable]
    public class EnemyEntry
    {
        public StandardEnemyScript.AttackStyle _attackStyle;
        public StandardEnemyScript.MovementStyle _movementStyle;
        public int _spawnWeight = 1;
    }

    [SerializeField]
    private float _enemyInterval = 2.0f;
    
    [SerializeField] private GameObject enemyPrefab;
    
    [SerializeField]
    public PowerUpEntry[] _powerups;

    [SerializeField]
    public EnemyEntry[] _enemies;

    [SerializeField]
    private bool _spawnEnemies = true;
    private bool _enemySpawnCoroutineActive = false;

    [SerializeField]
    private int[] _waves;
    [SerializeField]
    private int _waveNumber = 0;

    [SerializeField] 
    private GameObject _enemyContainer;

    [SerializeField]
    private bool _spawnPowerups = true;
    private bool _powerupSpawnCoroutineActive = false;

    private WeightedList<GameObject> _powerupList;
    private WeightedList<EnemyEntry> _enemyList;

    private UI_Manager _uiManager;

    [SerializeField]
    private float _percentShield = 0.2f;
    
    [SerializeField]
    private GameObject _bossPrefab;

    void Start()
    {
        _uiManager = GameObject.Find("Canvas").GetComponent<UI_Manager>();

        BuildPowerupList();

        BuildEnemyList();
    }

    private void BuildPowerupList()
    {
        _powerupList = new WeightedList<GameObject>();
        foreach (PowerUpEntry entry in _powerups)
        {
            _powerupList.AddItem(entry._powerup, entry._spawnWeight);
        }
    }

    private void BuildEnemyList()
    {
        _enemyList = new WeightedList<EnemyEntry>();
        foreach (EnemyEntry entry in _enemies)
        {
            _enemyList.AddItem(entry, entry._spawnWeight);
        }
    }

    IEnumerator SpawnEnemies()
    {
        yield return new WaitForSeconds(Random.Range(1f, 2f));

        if (_enemySpawnCoroutineActive) { yield break;  }

        _enemySpawnCoroutineActive = true;
        for (_waveNumber = 0; _waveNumber < _waves.Length; ++_waveNumber)
        {
            if (!_spawnEnemies)
            {
                // If we're signaled to end, we leave.
                yield break;
            }
            // Invoke Wave Change graphics
            _uiManager.ShowWaveText(_waveNumber + 1);
            yield return new WaitForSeconds(1f);

            for (int _enemyNumber = 0; _enemyNumber < _waves[_waveNumber]; ++_enemyNumber)
            {
                if (!_spawnEnemies)
                {
                    // If we're signaled to end, we leave.
                    yield break;
                }
                Vector3 spawnPosition = new Vector3(Random.Range(GameManager.lBound, GameManager.rBound), GameManager.uBound, 0);

                EnemyEntry entry = _enemyList.getItem();

                GameObject enemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
                enemy.transform.parent = _enemyContainer.transform;
                StandardEnemyScript enemyScript = enemy.GetComponent<StandardEnemyScript>();
                enemyScript.SetMovementStyle(entry._movementStyle);
                enemyScript.SetAttackStyle(entry._attackStyle);

                if (Random.Range(0, 1f) < _percentShield)
                {
                    enemyScript.AddShield();
                }

                yield return new WaitForSeconds(_enemyInterval);
            }

            Debug.Log("Waiting for all enemies to die");
            while (_enemyContainer.transform.childCount > 0)
            {
                if (!_spawnEnemies)
                {
                    // If we're signaled to end, we leave.
                    break;
                }
                Debug.Log($"{_enemyContainer.transform.childCount} enemies left!");
                // Wait for all of the enemies to die
                yield return new WaitForSeconds(0.25f);
            }
            Debug.Log("All dead!");
        }

        _uiManager.ShowWaveText(_waveNumber + 2);
        yield return new WaitForSeconds(1f);

        GameObject boss = Instantiate(_bossPrefab);
        while (boss != null)
        {
            yield return new WaitForSeconds(0.25f);
        }

        _uiManager.DisplayGameOver(true);

        _enemySpawnCoroutineActive= false;
         
        yield break;
    }

    IEnumerator SpawnPowerups()
    {
        yield return new WaitForSeconds(Random.Range(1f, 2f));

        if (_powerupSpawnCoroutineActive) { yield break; }

        _powerupSpawnCoroutineActive = true;
        while (_spawnPowerups)
        {
            Vector3 spawnPosition = new Vector3(Random.Range(GameManager.lBound, GameManager.rBound), GameManager.uBound, 0);
            
            GameObject randomPowerup = _powerupList.getItem();
            
            Instantiate(randomPowerup, spawnPosition, Quaternion.identity);

            yield return new WaitForSeconds(Random.Range(3f, 7f));
        }

        _powerupSpawnCoroutineActive = false;

        yield break;
    }

    public void StartSpawning()
    {
        StartSpawningEnemies();
        StartSpawningPowerups();
    }

    void StartSpawningEnemies()
    {
        _spawnEnemies = true;

        if (!_enemySpawnCoroutineActive)
        {
            // Restart the coroutine
            StartCoroutine(SpawnEnemies());
        }
    }

    public void StopSpawningEnemies()
    {
        _spawnEnemies = false;

        // Optionally force the coroutine to stop
        // I don't think this is needed at the time. If I did,
        // I'd probably want to delay doing so.
    }

    void StartSpawningPowerups()
    {
        _spawnPowerups= true;

        if (!_powerupSpawnCoroutineActive)
        {
            // Restart the coroutine
            StartCoroutine(SpawnPowerups());
        }
    }

    public void StopSpawningPowerups()
    {
        _spawnPowerups = false;

        // Optionally force the coroutine to stop
    }
}
