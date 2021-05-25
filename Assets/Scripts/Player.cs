using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private float _speed;
    [SerializeField] private float _defaultSpeed = 4f;
    [SerializeField] private float _flipperBoostSpeed = 7f;

    [SerializeField] private float _startPosX = -6f;

    [SerializeField] private float _rightBoundary = 0f;
    [SerializeField] private float _leftBoundary = 0f;
    [SerializeField] private float _ceilingBoundary = 0f;
    [SerializeField] private float _floorBoundary = 0f;

    [SerializeField] private float _tuskFireRate = 0.5f;
    private float _canFireTusk = 0f;

    [SerializeField] private int _lives = 3;

    private bool _isTripleTuskActive = false;
    private bool _isFlipperBoostActive = false;
    private bool _isShieldActive = false;

    [SerializeField] private GameObject _tuskPrefab;
    [SerializeField] private GameObject _tripleTuskPrefab;
    [SerializeField] private GameObject _shieldDummy;

    private SpawnManager _spawnManager;
  
    // Start is called before the first frame update
    void Start()
    {
        _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>();

        if (_spawnManager == null)
        {
            Debug.LogError("The Spawn Manager is NULL");
        }

        transform.position = new Vector3(_startPosX, 0, 0);

        _speed = _defaultSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        PlayerMovement();

        if (Input.GetKey(KeyCode.Space) && Time.time > _canFireTusk)
        {
            FireWeapon();
        }
    }

    void PlayerMovement()
    {
        float _horizontalInput = Input.GetAxisRaw("Horizontal");
        float _verticalInput = Input.GetAxisRaw("Vertical");

        PlayerBoundaries();

        FlipperBoost();
         
        Vector3 _direction = new Vector3(_horizontalInput, _verticalInput, 0).normalized;
        transform.Translate(_direction * _speed * Time.deltaTime);
    }

    void PlayerBoundaries()
    {
        float _xMovementClamp = Mathf.Clamp(transform.position.x, _leftBoundary, _rightBoundary);
        float _yMovementClamp = Mathf.Clamp(transform.position.y, _floorBoundary, _ceilingBoundary);
        Vector3 _limitPlayerMovement = new Vector3(_xMovementClamp, _yMovementClamp, 0);
        transform.position = _limitPlayerMovement;
    }

    void FireWeapon()
    {
        _canFireTusk = Time.time + _tuskFireRate;
        Vector3 _firePoint = new Vector3(2f, 0.2f, 0);

        if (_isTripleTuskActive == true)
        {
            Instantiate(_tripleTuskPrefab, transform.position + _firePoint, Quaternion.identity);
        }

        else
        {
            Instantiate(_tuskPrefab, transform.position + _firePoint, Quaternion.identity);
        }
    }

    void FlipperBoost()
    {
        if (_isFlipperBoostActive == false)
        {
            _speed = _defaultSpeed;
        }

        else
        {
            _speed = _flipperBoostSpeed;
        }
    }

    public void Damage()
    {
        if (_isShieldActive == true)
        {
            _isShieldActive = false;
            _shieldDummy.SetActive(false);
            return;
        }

        else
        {
            _lives--;
            //_lives = _lives -1;
            //_lives -= 1;
        }

        if (_lives < 1)
        {
            _spawnManager.OnPlayerDeath();
            Destroy(this.gameObject);
        }
    }

    public void TripleTuskActive()
    {
        StartCoroutine(TripleTuskPowerDownRoutine());
    }

    IEnumerator TripleTuskPowerDownRoutine()
    {
        _isTripleTuskActive = true;

        yield return new WaitForSeconds(5f);

        _isTripleTuskActive = false;
    }

    public void FlipperBoostActive()
    {
        StartCoroutine(FlipperBoostPowerDownRoutine());
    }

    IEnumerator FlipperBoostPowerDownRoutine()
    {
        _isFlipperBoostActive = true;

        yield return new WaitForSeconds(5f);

        _isFlipperBoostActive = false;
    }

     public void ShieldActive()
    {
        _isShieldActive = true;
        _shieldDummy.SetActive(true);
    }
}
