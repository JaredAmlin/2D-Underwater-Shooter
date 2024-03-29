using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Anglerfish : MonoBehaviour
{
    [SerializeField] private float _cruisingSpeed = 1f;
    [SerializeField] private float _battleSpeed = 5f;
    [SerializeField] private float _rammingSpeed = 10f;
    private float _currentSpeed;

    [SerializeField] private bool _hasEntered = false;
    [SerializeField] private bool _isBattleReady = false;
    [SerializeField] private bool _hasNextPosition = false;
    [SerializeField] private bool _isRamming = false;

    private Vector3 _entrancePosition = new Vector3(3f, 0.5f, 0);
    private Vector3 _battlePosition = new Vector3(8f, -0.25f, 0);
    private Vector3 _respawnPosition = new Vector3(20f, 0, 0);
    private Vector3 _nextPosition;

    [SerializeField] private GameObject _deadAnglerfish;

    private Player _player;

    private AnglerfishLight _anglerfishLight;

    private UIManager _uiManager;

    private SpawnManager _spawnManager;

    private Animator _anglerJawAnim;

    private WaitForSeconds _waitForTwoSeconds;

    private WaitForSeconds _waitForOneSecond;

    // Start is called before the first frame update
    void Start()
    {
        _player = GameObject.Find("Player").GetComponent<Player>();

        if (_player == null)
        {
            Debug.LogError("The player script is NULL");
        }

        _uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();

        if (_uiManager == null)
        {
            Debug.LogError("The UI Manager is NULL!");
        }

        _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>();

        if (_spawnManager == null)
        {
            Debug.LogError("The Spawn Manager is NULL!");
        }

        _anglerJawAnim = GameObject.Find("Anglerfish_Jaw").GetComponent<Animator>();

        if (_anglerJawAnim == null)
        {
            Debug.LogError("The ANIMATOR on the Angler JAW is NULL");
        }

        _anglerfishLight = GetComponentInChildren<AnglerfishLight>();

        {
            if (_anglerfishLight == null)
            {
                Debug.LogError("The AnglerfishLight script is NULL");
            }
        }

        _waitForTwoSeconds = new WaitForSeconds(2f);

        _waitForOneSecond = new WaitForSeconds(1f);

        _currentSpeed = _battleSpeed;

        StartCoroutine(EntranceRoutine());

        _uiManager.BossApproachingText();
    }

    IEnumerator EntranceRoutine()
    {
        while (transform.position != _entrancePosition && _hasEntered == false)
        {
            transform.position = Vector3.MoveTowards(transform.position, _entrancePosition, (_cruisingSpeed * Time.deltaTime));

            if (transform.position == _entrancePosition)
            {
                _hasEntered = true;

                _uiManager.BossFightText();

                yield return _waitForTwoSeconds;

                StartCoroutine(BattleReadyRoutine());
            }

            yield return null;
        }
    }

    IEnumerator BattleReadyRoutine()
    {
        while (_isBattleReady == false)
        {
            transform.position = Vector3.MoveTowards(transform.position, _battlePosition, (_cruisingSpeed * Time.deltaTime));

            if (transform.position == _battlePosition)
            {
                _isBattleReady = true;

                yield return _waitForTwoSeconds;
                
                float randomX = Random.Range(2f, 11f);
                float randomY = Random.Range(-2f, 2f);

                _nextPosition = new Vector3(randomX, randomY, 0);

                _hasNextPosition = true;
 
                StartCoroutine(BattleMovementRoutine());
                _spawnManager.SpawnPowerups();

                _anglerfishLight.StartLantern();
            }

            yield return null;
        }
    }

    IEnumerator BattleMovementRoutine()
    {
        while (_hasNextPosition == true && _isRamming == false)
        {
            transform.position = Vector3.MoveTowards(transform.position, _nextPosition, (_currentSpeed * Time.deltaTime));

            if (transform.position == _nextPosition)
            {
                _hasNextPosition = false;

                float randomX = Random.Range(2f, 11f);
                float randomY = Random.Range(-2f, 2f);

                _nextPosition = new Vector3(randomX, randomY, 0);

                _hasNextPosition = true;
            }

            yield return null;
        }
    }

    IEnumerator RammingRoutine()
    {
        _anglerJawAnim.SetBool("IsChomping", true);

        yield return _waitForOneSecond;

        while (_isRamming == true)
        {
            transform.Translate(Vector3.left * _rammingSpeed * Time.deltaTime);

            if (transform.position.x < -20f)
            {
                _isRamming = false;
                transform.position = _respawnPosition;

                _anglerJawAnim.SetBool("IsChomping", false);

                StartCoroutine(BattleMovementRoutine());
            }

            yield return null;
        }
    }

    public void RammingAnglerfish()
    {
        _isRamming = true;

        StartCoroutine(RammingRoutine());
    }

    public void SpeedDown()
    {
        _currentSpeed = _cruisingSpeed;
    }

    public void SpeedUp()
    {
        _currentSpeed = _battleSpeed;
    }

    public void IncrementSpeeds()
    {
        _cruisingSpeed++;
        _battleSpeed++;
        _rammingSpeed++;
    }

    public void OnDeathBehavior()
    {
        Instantiate(_deadAnglerfish, transform.position, Quaternion.identity);

        _spawnManager.OnPlayerDeath();

        _uiManager.WaveCompletedText();

        Destroy(this.gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (_player != null)
            {
                _player.Damage();
            }
        }
    }
}
