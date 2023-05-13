using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab;
    private bool _coroutineActive = false;

    [SerializeField]
    private bool _spawnEnemies = true;
    
    [SerializeField] 
    private GameObject _enemyContainer;
    private Coroutine _coroutine;

    void Start()
    {
        StartSpawning();
    }

    IEnumerator SpawnEnemies()
    {
        if (_coroutineActive) { yield break;  }

        _coroutineActive = true;
        while (_spawnEnemies)
        {
            Vector3 spawnPosition = new Vector3(Random.Range(-8f, 8f), 8f, 0);
            GameObject enemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
            enemy.transform.parent = _enemyContainer.transform;

            yield return new WaitForSeconds(5.0f);
        }

        _coroutineActive= false;
         
        yield break;
    }

    void StartSpawning()
    {
        _spawnEnemies = true;

        if (!_coroutineActive)
        {
            // Restart the coroutine
            _coroutine = StartCoroutine(SpawnEnemies());
        }
    }

    public void StopSpawning()
    {
        _spawnEnemies = false;

        // Optionally force the coroutine to stop
        // I don't think this is needed at the time. If I did,
        // I'd probably want to delay doing so.
    }
}
