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

    //range to get chance for ppowerup drops
    private float _powerupRarity;

    //variable to store random enemy value
    private int _randomEmemy;

    //variable to store random powerup value
    private int _randomPowerup;

    //ID for Powerups
    //0 = Triple Tusk, 1 = Flipper Boost, 2 = Bubble Shield, 3 = Health, 4 = Tusk Ammo Reload , 5 = Player Penalty, 6 = Bubble Blaster, 7 = Homing Tusk
    private int _tripleTusk = 0, _flipperBoost = 1, _bubbleShield = 2, _health = 3, _ammoReload = 4, _playerPenalty = 5, _bubbleBlaster = 6, _homingTusk = 7;
    
    //ID for enemies
    //0 = piranha, 1 = jellyfish, 2 = red piranha, 3 = blowfish
    private int _piranha = 0, _jellyfish = 1, _redPiranha = 2, _blowfish = 3;

    //values to check the playyer health and shield
    private int _playerHealth, _playerShields;
    //max value for player health and shield
    private int _maxPlayerHealthAndShields = 3;
    //random value for uncommon powerup role
    private int _randomUncommonPowerup;
    //random value for rare powerup role
    private int _randomRarePowerup;


    //rotation variable for the jellyfish instantiation
    private Quaternion _enemyRotation;

    //location for piranha spawn point
    Vector3 _piranhaSpawnPosition;
    //new location for jellyfish spawn point
    Vector3 _jellyfishSpawnPosition;
    //assignable enemySpawnPosition
    Vector3 _enemySpawnPosition;

    private Player _player;

    // Start is called before the first frame update
    void Start()
    {
        _player = GameObject.Find("Player").GetComponent<Player>();
        
        if (_player == null)
        {
            Debug.Log("The Player is NULL");
        }

        StartCoroutine(SpawnEnemyRoutine());

        StartCoroutine(SpawnPowerupRoutine());
    }

    IEnumerator SpawnEnemyRoutine()
    {
        while (_stopSpawning == false)
        {
            //location for piranhas to spawn
            _piranhaSpawnPosition = new Vector3(11.5f, Random.Range(-5.1f, 5.1f), 0);
            //new location for jellyfish spawn point
            _jellyfishSpawnPosition = new Vector3(Random.Range(-8f, 8f), -6.5f, 0);

            RandomWeightedEnemy();

            EnemySpawnPosition();

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

            RandomWeightedPowerup();

            _powerupSpawnRate = Random.Range(_powerupSpawnRateMin, _powerupSpawnRateMax);
            Instantiate(_powerups[_randomPowerup], _powerupSpawnPosition, Quaternion.identity);
            yield return new WaitForSeconds(_powerupSpawnRate);
        }
    }

    private void RandomWeightedEnemy()
    {
        //generate random value between 0 and 1
        float weightedEnemy = Random.value;

        Debug.Log($"The selected enemy is the {weightedEnemy} value");

        if (weightedEnemy >= 0.45f)
        {
            //instantiate piranha prefab == 0
            _randomEmemy = _piranha;

        }

        else if (weightedEnemy < 0.45f && weightedEnemy >= 0.3f)
        {
            //instantiate jellyfish == 1
            _randomEmemy = _jellyfish;
        }

        else if (weightedEnemy < 0.3f && weightedEnemy >= 0.15f)
        {
            //instantiate Red Piranha == 2
            _randomEmemy = _redPiranha;
        }

        else if (weightedEnemy < 0.15f)
        {
            //instantiate Blowfish == 3
            _randomEmemy = _blowfish;
        }
    }

    private void EnemySpawnPosition()
    {
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
    }

    private void RandomWeightedPowerup()
    {
        //check player for max shield and/ or health

        if (_player != null)
        {
            _playerHealth = _player.PlayerLives();
            Debug.Log("The player health is " + _playerHealth);

            //check player for shield health
            _playerShields = _player.PlayerShield();
            Debug.Log("The player shields are " + _playerShields);
        }

        if (_playerHealth < _maxPlayerHealthAndShields && _playerShields < _maxPlayerHealthAndShields)
        {
            //run normal powerup spawn balance
            StandardPowerupDropTable();
        }

        else if (_playerHealth == _maxPlayerHealthAndShields || _playerShields == _maxPlayerHealthAndShields)
        {
            //exlude health from powerup drop
            //increase rare drop rate
            BetterPowerupDropTable();
        }

        else if (_playerHealth == _maxPlayerHealthAndShields && _playerShields == _maxPlayerHealthAndShields)
        {
            //exclude health and shield drop
            //increase rare drop rate most
            BestPowerupDropTable();
        }
    }

    private void StandardPowerupDropTable()
    {
        _powerupRarity = Random.value;
        Debug.Log($"The powerup rarity is {_powerupRarity}");

        if (_powerupRarity >= 0.7f)
        {
            //spawn common powerup = ammo
            _randomPowerup = _ammoReload;
        }

        else if (_powerupRarity < 0.7f && _powerupRarity >= 0.2f)
        {
            //spawn uncommon powerup
            RandomUncommonPowerup();
        }

        else if (_powerupRarity < 0.2f)
        {
            //spawn rare powerup
            RandomRarePowerup();
        }
    }

    private void BetterPowerupDropTable()
    {
        _powerupRarity = Random.value;
        Debug.Log($"The powerup rarity is {_powerupRarity}");

        if (_powerupRarity >= 0.7f)
        {
            //spawn common powerup = ammo
            _randomPowerup = _ammoReload;
        }

        else if (_powerupRarity < 0.7f && _powerupRarity >= 0.25f)
        {
            //spawn uncommon powerup
            RandomUncommonPowerup();
        }

        else if (_powerupRarity < 0.25f)
        {
            //spawn rare powerup
            RandomRarePowerup();
        }
    }

    private void BestPowerupDropTable()
    {
        _powerupRarity = Random.value;
        Debug.Log($"The powerup rarity is {_powerupRarity}");

        if (_powerupRarity >= 0.7f)
        {
            //spawn common powerup = ammo
            _randomPowerup = _ammoReload;
        }

        else if (_powerupRarity < 0.7f && _powerupRarity >= 0.3f)
        {
            //spawn uncommon powerup
            RandomUncommonPowerup();
        }

        else if (_powerupRarity < 0.3f)
        {
            //spawn rare powerup
            RandomRarePowerup();
        }
    }

    private void RandomUncommonPowerup()
    {
        //instantiate uncommon powerup

        if (_playerShields < _maxPlayerHealthAndShields)
        {
            _randomUncommonPowerup = Random.Range(0, 4);
            Debug.Log("The random uncommon powerup range is " + _randomUncommonPowerup);
        }

        else if (_playerShields == _maxPlayerHealthAndShields)
        {
            _randomUncommonPowerup = Random.Range(0, 3);
        }

        switch (_randomUncommonPowerup)
        {
            case 0:
                //instantiate triple shot
                _randomPowerup = _tripleTusk;
                break;
            case 1:
                //instantiate speed boost
                _randomPowerup = _flipperBoost;
                break;
            case 2:
                //instantiate player penalty
                _randomPowerup = _playerPenalty;
                break;
            case 3:
                //instanitate player shield
                _randomPowerup = _bubbleShield;
                break;
            default:
                Debug.LogError("There is no UNCOMMON powerup drop assigned for this case");
                break;
        }
    }

    private void RandomRarePowerup()
    {
        //instantiate rare powerup
        
        if (_playerHealth < _maxPlayerHealthAndShields)
        {
            _randomRarePowerup = Random.Range(0, 3);
        }

        else if (_playerHealth == _maxPlayerHealthAndShields)
        {
            _randomRarePowerup = Random.Range(0, 2);
        }

        Debug.Log("The random rare powerup ramge is " + _randomRarePowerup);

        switch (_randomRarePowerup)
        {
            case 0:
                //instantite health
                _randomPowerup = _bubbleBlaster;
                break;
            case 1:
                //instantiate bubble blaster
                _randomPowerup = _homingTusk;
                break;
            case 2:
                //instantiate homing tusk
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
        _stopSpawning = true;
    }
}
