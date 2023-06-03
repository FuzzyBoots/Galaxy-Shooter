using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [System.Serializable]
    public class PowerUpEntry
    {
        public GameObject _powerup;
        public float _spawnWeight = 1;
    }

    [SerializeField] private GameObject enemyPrefab;
    
    [SerializeField]
    public PowerUpEntry[] _powerups;

    [SerializeField]
    private bool _spawnEnemies = true;
    private bool _enemySpawnCoroutineActive = false;

    [SerializeField] 
    private GameObject _enemyContainer;

    [SerializeField]
    private bool _spawnPowerups = true;
    private bool _powerupSpawnCoroutineActive = false;

    private List<GameObject> _powerupList;

    void Start()
    {
        BuildPowerupList();
    }

    private void BuildPowerupList()
    {
        _powerupList = new List<GameObject>();
        foreach (PowerUpEntry entry in _powerups)
        {
            for (int i = 0; i < entry._spawnWeight; ++i)
            {
                _powerupList.Add(entry._powerup);
            }
        }
    }

    IEnumerator SpawnEnemies()
    {
        yield return new WaitForSeconds(Random.Range(1f, 2f));

        if (_enemySpawnCoroutineActive) { yield break;  }

        _enemySpawnCoroutineActive = true;
        while (_spawnEnemies)
        {
            Vector3 spawnPosition = new Vector3(Random.Range(-8f, 8f), 8f, 0);
            GameObject enemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
            enemy.transform.parent = _enemyContainer.transform;
            
            yield return new WaitForSeconds(5.0f);
        }

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
            Vector3 spawnPosition = new Vector3(Random.Range(-8f, 8f), 8f, 0);
            int index = Random.Range(0, _powerupList.Count());
            
            GameObject randomPowerup = _powerupList[index];
            // Debug.Log($"Powerup Name: {randomPowerup.name}");
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
