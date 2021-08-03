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

    //spawn rate for bubble blaster
    private float _bubbleBlasterSpawnRate;
    [SerializeField] private float _bubbBlasterSpawnRateMin = 30f;
    [SerializeField] private float _bubbleBlasterSpawnRateMax = 45f;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(SpawnEnemyRoutine());

        StartCoroutine(SpawnPowerupRoutine());

        StartCoroutine(SpawnBubbleBlasterRoutine());
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
            int _randomPowerup = Random.Range(0, 6);
            _powerupSpawnRate = Random.Range(_powerupSpawnRateMin, _powerupSpawnRateMax);
            Instantiate(powerups[_randomPowerup], _powerupSpawnPosition, Quaternion.identity);
            yield return new WaitForSeconds(_powerupSpawnRate);
        }
    }

    IEnumerator SpawnBubbleBlasterRoutine()
    {
        //wait the minimum spawn rate before spawning first powerup
        yield return new WaitForSeconds(_bubbBlasterSpawnRateMin);
        //seperate while loop for bubble blaster spawning
        while (_stopSpawning == false)
        {
            Vector3 _powerupSpawnPosition = new Vector3(11.5f, Random.Range(-5.1f, 5.1f), 0);
            _bubbleBlasterSpawnRate = Random.Range(_bubbBlasterSpawnRateMin, _bubbleBlasterSpawnRateMax);
            Instantiate(powerups[6], _powerupSpawnPosition, Quaternion.identity);
            yield return new WaitForSeconds(_bubbleBlasterSpawnRate);
        }
    }

    public void PlayerOutOfAmmo()
    {
        Vector3 _powerupSpawnPosition = new Vector3(11.5f, Random.Range(-5.1f, 5.1f), 0);
        Instantiate(powerups[4], _powerupSpawnPosition, Quaternion.identity);
    }

    public void OnPlayerDeath()
    {
        _stopSpawning = true;
    }
}
