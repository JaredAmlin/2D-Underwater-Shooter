using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] private GameObject _piranhaPrefab;
    [SerializeField] private GameObject _enemyContainer;

    [SerializeField] private GameObject[] powerups;

    private bool _stopSpawning = false;

    [SerializeField] private float _enemySpawnRate = 1f;
    private float _powerupSpawnRate;
    [SerializeField] private float _powerupSpawnRateMin = 7f;
    [SerializeField] private float _powerupSpawnRateMax = 12f;

    // Start is called before the first frame update
    void Start()
    {
        _powerupSpawnRate = Random.Range(_powerupSpawnRateMin, _powerupSpawnRateMax);

        StartCoroutine(SpawnEnemyRoutine());

        StartCoroutine(SpawnPowerupRoutine());
    }

    IEnumerator SpawnEnemyRoutine()
    {
        while (_stopSpawning == false)
        {
            Vector3 _enemySpawnPosition = new Vector3(11.5f, Random.Range(-5.1f, 5.1f), 0);
            GameObject newEnemy = Instantiate(_piranhaPrefab, _enemySpawnPosition, Quaternion.identity);
            newEnemy.transform.parent = _enemyContainer.transform;
            yield return new WaitForSeconds(_enemySpawnRate);
        }
    }

    IEnumerator SpawnPowerupRoutine()
    {
        while (_stopSpawning == false)
        {
            Vector3 _powerupSpawnPosition = new Vector3(11.5f, Random.Range(-5.1f, 5.1f), 0);
            int _randomPowerup = Random.Range(0, 3);
            Instantiate(powerups[_randomPowerup], _powerupSpawnPosition, Quaternion.identity);
            yield return new WaitForSeconds(_powerupSpawnRate);
        }
    }

    public void OnPlayerDeath()
    {
        _stopSpawning = true;
    }
}
