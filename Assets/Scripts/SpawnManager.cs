using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] private GameObject _enemyContainer;
    [SerializeField] private GameObject[] _powerups;
    [SerializeField] private GameObject[] _enemies;
    [SerializeField] private GameObject _anglerfish;

    [SerializeField] private bool _isSpawningEnemies = false;
    [SerializeField] private bool _isSpawningPowerups = false;

    [SerializeField] private bool _isWaveTimerComplete = false;

    [SerializeField] private float _enemySpawnRate = 1f;

    private float _powerupSpawnRate;
    [SerializeField] private float _powerupSpawnRateMin = 7f;
    [SerializeField] private float _powerupSpawnRateMax = 12f;

    [SerializeField] private int _currentWave = 0;

    private float _powerupRarity;

    private int _randomEmemy;

    private int _randomPowerup;

    //0 = Triple Tusk, 1 = Flipper Boost, 2 = Bubble Shield, 3 = Health, 4 = Tusk Ammo Reload , 5 = Player Penalty, 6 = Bubble Blaster, 7 = Homing Tusk
    private int _tripleTusk = 0, _flipperBoost = 1, _bubbleShield = 2, _health = 3, _ammoReload = 4, _playerPenalty = 5, _bubbleBlaster = 6, _homingTusk = 7;
    
    //0 = piranha, 1 = jellyfish, 2 = red piranha, 3 = blowfish
    private int _piranha = 0, _jellyfish = 1, _redPiranha = 2, _blowfish = 3;

    private int _playerHealth, _playerShields;
    private int _maxPlayerHealthAndShields = 3;
 
    private int _randomUncommonPowerup;
    private int _randomRarePowerup;

    private Quaternion _enemyRotation;

    Vector3 _piranhaSpawnPosition;
    
    Vector3 _jellyfishSpawnPosition;
    
    Vector3 _enemySpawnPosition;

    Vector3 _anglerfishSpawnPosition = new Vector3(20f, 0, 0);

    private Player _player;

    [SerializeField] private UIManager _uiManager;

    // Start is called before the first frame update
    void Start()
    {
        _player = GameObject.Find("Player").GetComponent<Player>();
        
        if (_player == null)
        {
            Debug.Log("The Player is NULL");
        }

        if (_uiManager == null)
        {
            Debug.LogError("The UI Manager is NULL");
        }
    }

    IEnumerator SpawnEnemyRoutine()
    {
        while (_isSpawningEnemies == true)
        {
            _piranhaSpawnPosition = new Vector3(11.5f, Random.Range(-5.1f, 5.1f), 0);
        
            _jellyfishSpawnPosition = new Vector3(Random.Range(-8f, 8f), -6.5f, 0);

            RandomWeightedEnemy();

            EnemySpawnPosition();

            GameObject newEnemy = Instantiate(_enemies[_randomEmemy], _enemySpawnPosition, _enemyRotation);
            newEnemy.transform.parent = _enemyContainer.transform;
            yield return new WaitForSeconds(_enemySpawnRate);
        }
    }

    public void SpawnWave()
    {
        StartCoroutine(SpawnWaveRoutine());
    }

    IEnumerator SpawnWaveRoutine()
    {
        _currentWave++;

        _enemySpawnRate--;

        if (_enemySpawnRate < 1f)
        {
            _enemySpawnRate = 1f;
        }

        _isSpawningEnemies = true;
        _isSpawningPowerups = true;

        _uiManager.UpdateWaveText(_currentWave);

        yield return new WaitForSeconds(6f);

        StartCoroutine(SpawnEnemyRoutine());
        StartCoroutine(SpawnPowerupRoutine());
        yield return new WaitForSeconds(30f);

        _isSpawningEnemies = false;
        _isSpawningPowerups = false;
        _isWaveTimerComplete = true;
        
        StartCoroutine(FindEnemiesRemainingRoutine());
    }

    IEnumerator FindEnemiesRemainingRoutine()
    {
        while (_isWaveTimerComplete == true)
        {
            if (_currentWave != 3)
            {
                if (GameObject.FindGameObjectWithTag("Enemy") == null)
                {
                    _isWaveTimerComplete = false;
                    _uiManager.WaveCompletedText();

                    yield return new WaitForSeconds(6f);
                    
                    SpawnWave();

                    yield return null;
                }
            }

            else if (_currentWave == 3)
            {
                if (GameObject.FindGameObjectWithTag("Enemy") == null)
                {
                    _isWaveTimerComplete = false;
                   
                    Instantiate(_anglerfish, _anglerfishSpawnPosition, Quaternion.identity);

                    yield return null;
                }
            }

            yield return new WaitForSeconds(2f);
        }
    }

    public void SpawnPowerups()
    {
        _isSpawningPowerups = true;
        StartCoroutine(SpawnPowerupRoutine());
    }

    IEnumerator SpawnPowerupRoutine()
    {
        while (_isSpawningPowerups == true)
        {
            if (_player != null)
            {
                Vector3 _powerupSpawnPosition = new Vector3(11.5f, Random.Range(-5.1f, 5.1f), 0);

                RandomWeightedPowerup();

                _powerupSpawnRate = Random.Range(_powerupSpawnRateMin, _powerupSpawnRateMax);
                Instantiate(_powerups[_randomPowerup], _powerupSpawnPosition, Quaternion.identity);
                yield return new WaitForSeconds(_powerupSpawnRate);
            }
        }
    }

    private void RandomWeightedEnemy()
    {
        float weightedEnemy = Random.value;

        if (weightedEnemy >= 0.45f)
        {
            _randomEmemy = _piranha;

        }

        else if (weightedEnemy < 0.45f && weightedEnemy >= 0.3f)
        {
            _randomEmemy = _jellyfish;
        }

        else if (weightedEnemy < 0.3f && weightedEnemy >= 0.15f)
        {
            _randomEmemy = _redPiranha;
        }

        else if (weightedEnemy < 0.15f)
        {
            _randomEmemy = _blowfish;
        }
    }

    private void EnemySpawnPosition()
    {
        if (_randomEmemy == 0 || _randomEmemy == 2 || _randomEmemy == 3)
        {
            _enemySpawnPosition = _piranhaSpawnPosition;
            _enemyRotation = Quaternion.identity;
        }

        else if (_randomEmemy == 1)
        {
            _enemySpawnPosition = _jellyfishSpawnPosition;
            float _randomZ = Random.Range(-30f, 30f);
            _enemyRotation = Quaternion.Euler(0, 0, _randomZ);
        }
    }

    private void RandomWeightedPowerup()
    {
        if (_player != null)
        {
            _playerHealth = _player.PlayerLives();
            
            _playerShields = _player.PlayerShield();
        }

        if (_playerHealth < _maxPlayerHealthAndShields && _playerShields < _maxPlayerHealthAndShields)
        {
            StandardPowerupDropTable();
        }

        else if (_playerHealth == _maxPlayerHealthAndShields || _playerShields == _maxPlayerHealthAndShields)
        {
            BetterPowerupDropTable();
        }

        else if (_playerHealth == _maxPlayerHealthAndShields && _playerShields == _maxPlayerHealthAndShields)
        {
            BestPowerupDropTable();
        }
    }

    private void StandardPowerupDropTable()
    {
        _powerupRarity = Random.value;

        if (_powerupRarity >= 0.7f)
        {
            _randomPowerup = _ammoReload;
        }

        else if (_powerupRarity < 0.7f && _powerupRarity >= 0.2f)
        {
            RandomUncommonPowerup();
        }

        else if (_powerupRarity < 0.2f)
        {
            RandomRarePowerup();
        }
    }

    private void BetterPowerupDropTable()
    {
        _powerupRarity = Random.value;

        if (_powerupRarity >= 0.7f)
        {
            _randomPowerup = _ammoReload;
        }

        else if (_powerupRarity < 0.7f && _powerupRarity >= 0.25f)
        {
            RandomUncommonPowerup();
        }

        else if (_powerupRarity < 0.25f)
        {
            RandomRarePowerup();
        }
    }

    private void BestPowerupDropTable()
    {
        _powerupRarity = Random.value;

        if (_powerupRarity >= 0.7f)
        {
            _randomPowerup = _ammoReload;
        }

        else if (_powerupRarity < 0.7f && _powerupRarity >= 0.3f)
        {
            RandomUncommonPowerup();
        }

        else if (_powerupRarity < 0.3f)
        {
            RandomRarePowerup();
        }
    }

    private void RandomUncommonPowerup()
    {
        if (_playerShields < _maxPlayerHealthAndShields)
        {
            _randomUncommonPowerup = Random.Range(0, 4);
        }

        else if (_playerShields == _maxPlayerHealthAndShields)
        {
            _randomUncommonPowerup = Random.Range(0, 3);
        }

        switch (_randomUncommonPowerup)
        {
            case 0:
                _randomPowerup = _tripleTusk;
                break;
            case 1:
                _randomPowerup = _flipperBoost;
                break;
            case 2:
                _randomPowerup = _playerPenalty;
                break;
            case 3:
                _randomPowerup = _bubbleShield;
                break;
            default:
                Debug.LogError("There is no UNCOMMON powerup drop assigned for this case");
                break;
        }
    }

    private void RandomRarePowerup()
    {   
        if (_playerHealth < _maxPlayerHealthAndShields)
        {
            _randomRarePowerup = Random.Range(0, 3);
        }

        else if (_playerHealth == _maxPlayerHealthAndShields)
        {
            _randomRarePowerup = Random.Range(0, 2);
        }

        switch (_randomRarePowerup)
        {
            case 0:
                _randomPowerup = _bubbleBlaster;
                break;
            case 1:
                _randomPowerup = _homingTusk;
                break;
            case 2:
                _randomPowerup = _health;
                break;
            default:
                Debug.LogError("There is no RARE powerup drop assigned for this case");
                break;
        }
    }

    public void PlayerOutOfAmmo()
    {
        Vector3 _powerupSpawnPosition = new Vector3(11.5f, Random.Range(-5.1f, 5.1f), 0);
        Instantiate(_powerups[4], _powerupSpawnPosition, Quaternion.identity);
    }

    public void OnPlayerDeath()
    {
        _isSpawningEnemies = false;
        _isSpawningPowerups = false;
    }
}
