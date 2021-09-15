using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] private GameObject _enemyContainer;
    [SerializeField] private GameObject[] _powerups;
    [SerializeField] private GameObject[] _enemies;

    private bool _stopSpawning = false;

    [SerializeField] private float _enemySpawnRate = 1f;

    private float _powerupSpawnRate;
    [SerializeField] private float _powerupSpawnRateMin = 7f;
    [SerializeField] private float _powerupSpawnRateMax = 12f;

    //spawn rate for bubble blaster
    private float _bubbleBlasterSpawnRate;
    [SerializeField] private float _bubbBlasterSpawnRateMin = 30f;
    [SerializeField] private float _bubbleBlasterSpawnRateMax = 45f;

    //rotation variable for the jellyfish instantiation
    private Quaternion _enemyRotation;

    //location for piranha spawn point
    Vector3 _piranhaSpawnPosition;
    //new location for jellyfish spawn point
    Vector3 _jellyfishSpawnPosition;
    //assignable enemySpawnPosition
    Vector3 _enemySpawnPosition;

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
            //location for piranhas to spawn
            _piranhaSpawnPosition = new Vector3(11.5f, Random.Range(-5.1f, 5.1f), 0);
            //new location for jellyfish spawn point
            _jellyfishSpawnPosition = new Vector3(Random.Range(-8f, 8f), -6.5f, 0);

            //make variable for random enemy spawn
            int _randomEmemy = Random.Range(0, _enemies.Length);

            //piranha spawn data
            if (_randomEmemy == 0 || _randomEmemy == 2 || _randomEmemy == 3)
            {
                _enemySpawnPosition = _piranhaSpawnPosition;
                _enemyRotation = Quaternion.identity;
            }

            //jellyfish spawn data
            else if (_randomEmemy == 1)
            {
                _enemySpawnPosition = _jellyfishSpawnPosition;
                float _randomZ = Random.Range(-30f, 30f); 
                _enemyRotation = Quaternion.Euler(0, 0, _randomZ);
            }

            GameObject newEnemy = Instantiate(_enemies[_randomEmemy], _enemySpawnPosition, _enemyRotation);
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
            Instantiate(_powerups[_randomPowerup], _powerupSpawnPosition, Quaternion.identity);
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
            Instantiate(_powerups[6], _powerupSpawnPosition, Quaternion.identity);
            yield return new WaitForSeconds(_bubbleBlasterSpawnRate);
        }
    }

    public void PlayerOutOfAmmo()
    {
        Vector3 _powerupSpawnPosition = new Vector3(11.5f, Random.Range(-5.1f, 5.1f), 0);
        Instantiate(_powerups[4], _powerupSpawnPosition, Quaternion.identity);
    }

    public void OnPlayerDeath()
    {
        _stopSpawning = true;
    }
}
