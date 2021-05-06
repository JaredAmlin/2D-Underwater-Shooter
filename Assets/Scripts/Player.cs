using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private float _speed = 3.5f;

    [SerializeField] private float _startPosX = -6f;

    [SerializeField] private float _rightBoundary = 0f;
    [SerializeField] private float _leftBoundary = 0f;
    [SerializeField] private float _ceilingBoundary = 0f;
    [SerializeField] private float _floorBoundary = 0f;

    [SerializeField] private float _bulletFireRate = 0.5f;
    private float _canFireBullet = 0f;

    [SerializeField] private int _lives = 3;

    [SerializeField] private GameObject _bulletPreFab;

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
    }

    // Update is called once per frame
    void Update()
    {
        PlayerMovement();

        if (Input.GetKey(KeyCode.Space) && Time.time > _canFireBullet)
        {
            FireBullet();
        }
    }

    void PlayerMovement()
    {
        PlayerBoundaries();

        float _horizontalInput = Input.GetAxisRaw("Horizontal");
        float _verticalInput = Input.GetAxisRaw("Vertical");

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

    void FireBullet()
    {
        _canFireBullet = Time.time + _bulletFireRate;
        Vector3 _firePoint = new Vector3(1f, 0, 0);
        Instantiate(_bulletPreFab, transform.position + _firePoint, Quaternion.Euler(0, 0, -90));
        //Debug.LogFormat("I am traveling at {0} per hour and starting from the {1} position", _speed, _startPosX);
    }

    public void Damage()
    {
        _lives --;
        //_lives = _lives -1;
        //_lives -= 1;

        if (_lives < 1)
        {
            _spawnManager.OnPlayerDeath();
            Destroy(this.gameObject);
        }
    }
}
