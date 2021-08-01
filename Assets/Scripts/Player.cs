using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    //variable to store the current speed of the player
    [SerializeField] private float _currentSpeed;
    //variable to store the default speed, which will be minimum
    [SerializeField] private float _defaultSpeed;
    //variable to use for clamping min speed value
    [SerializeField] private float _minSpeed = 4f;
    //variable to use for clamping max speed value
    [SerializeField] private float _maxSpeed = 10f;
    //variable to store flipper boost powerup speed
    [SerializeField] private float _flipperBoostSpeed = 7f;
    //variable for player to accelerate
    [SerializeField] private float _accelFactor = 0.1f;
    //variable for player to decelerate
    [SerializeField] private float _decelFactor = 0.2f;

    /*variable to hold the current thrust amount, 
    * changing as shift is held or released*/
    [SerializeField] private float _currentThrust;
    //variable to hold max thrust value
    [SerializeField] private float _maxThrust = 100f;
    //variable to hold min thrust value
    [SerializeField] private float _minThrust = 0f;
    //factor to increase thrust amount
    [SerializeField] private float _thrustDecelFactor = 0.2f;
    //factor to decrease thrust amount
    [SerializeField] private float _thrustAccelFactor = 0.1f;
    //cool down time to deactivate thruster after hitting zero
    [SerializeField] private float _thrusterCoolDownTime = 2f;

    [SerializeField] private float _startPosX = -6f;

    [SerializeField] private float _rightBoundary = 0f;
    [SerializeField] private float _leftBoundary = 0f;
    [SerializeField] private float _ceilingBoundary = 0f;
    [SerializeField] private float _floorBoundary = 0f;

    [SerializeField] private float _tuskFireRate = 0.5f;
    private float _canFireTusk = 0f;

    private float _bubbleBlasterFireRate = 0.1f;
    private float _canFireBubbleBlaster = 0f;

    //variable for current ammo count
    [SerializeField] private int _currentTuskAmmo;
    //variable for max ammo count
    [SerializeField] private int _maxTuskAmmo = 15;
    //variable for min ammo count
    private int _minTuskAmmo = 0;

    [SerializeField] private int _currentLives;
    [SerializeField] private int _maxLives = 3;
    private int _minLives = 0;
    //variable to hold my shield health
    [SerializeField] private int _shieldHealth;
    //variable to hold max value of shield health
    private int _maxShieldHealth = 3;
    //variable to hold min value of shield health 
    private int _minShieldHealth = 0;


    private bool _isTripleTuskActive = false;
    private bool _isFlipperBoostActive = false;
    private bool _isShieldActive = false;
    //variable for if Bubble Blaster is active
    [SerializeField] private bool _isBubbleBlasterActive = false;
    //variable to switch if out of ammo and powerup is spawned
    private bool _hasSpawnedAmmoReload = false;

    //variable to store if player penalty is active
    [SerializeField] private bool _isPlayerPenaltyActive = false;

    //bool to handle if my player can thrust
    [SerializeField] private bool _canThrust = false;
    //bool to handle if my deceleration rates should be active
    [SerializeField] private bool _isThrusting = false;
    //bool to set isThrusting in update one time when shift is pressed
    [SerializeField] private bool _isThrustingSwitch = false;

    //bool for if player is pulling powerups
    private bool _isPullingPowrups = false;

    [SerializeField] private GameObject _tuskPrefab;
    [SerializeField] private GameObject _tripleTuskPrefab;
    [SerializeField] private GameObject _bubbleShield;

    //game object variable for bubble blaster prefab special weapon
    [SerializeField] private GameObject _bubbleBlasterPrefab;

    //game object variables for player damage visualization
    [SerializeField] private GameObject _damageScarSingle;
    [SerializeField] private GameObject _damageScarDouble;

    //variable to hold my CameraShake class
    private CameraShake _cameraHolder;

    private PolygonCollider2D _playerCollider;
    private SpawnManager _spawnManager;
    private UIManager _uiManager;

    private Animator _animator;
    private SpriteRenderer _spriteRenderer;
    //variable for my shield sprite renderer
    private SpriteRenderer _shieldSpriteRenderer;
    [SerializeField] private AudioClip _pewPewSoundClip;
    [SerializeField] private AudioClip _bubblePopSoundClip;
    private AudioSource _audioSource;

    [SerializeField] private Color _shieldDamageColor1;
    [SerializeField] private Color _shieldDamageColor2;
  
    // Start is called before the first frame update
    void Start()
    {
        _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>();

        if (_spawnManager == null)
        {
            Debug.LogError("The Spawn Manager is NULL");
        }

        _uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();

        if (_uiManager == null)
        {
            Debug.LogError("The UI Manager is NULL");
        }

        _animator = GetComponent<Animator>();

        if (_animator == null)
        {
            Debug.LogError("The Animator is NULL");
        }

        _spriteRenderer = GetComponent<SpriteRenderer>();

        if (_spriteRenderer == null)
        {
            Debug.LogError("The Sprite Renderer is NULL");
        }

        _playerCollider = GetComponent<PolygonCollider2D>();

        if (_playerCollider == null)
        {
            Debug.LogError("The Player Collider is NULL");
        }

        _audioSource = GetComponent<AudioSource>();

        if (_audioSource == null)
        {
            Debug.LogError("The Audio Source on the Player is NULL");
        }

        transform.position = new Vector3(_startPosX, 0, 0);

        //set both current and default speeds to be the minimum value
        _currentSpeed = _minSpeed;
        _defaultSpeed = _minSpeed;

        //set shield health to be max value
        _shieldHealth = _maxShieldHealth;

        //set the lives to be the maximum value
        _currentLives = _maxLives;

        //set current thrust to be the max value
        _currentThrust = _maxThrust;

        //thruster is full so player canThrust
        _canThrust = true;

        //assign ammo count to max value
        _currentTuskAmmo = _maxTuskAmmo;

        //update UI manager to show full ammo count when game starts
        _uiManager.UpdateTuskAmmo(_currentTuskAmmo);

        //find the camera holder and get the CameraShake class component
        _cameraHolder = GameObject.Find("Camera_Holder").GetComponent<CameraShake>();

        //debug the GetComponent
        if (_cameraHolder == null)
        {
            Debug.LogError("The Camera Holder is NULL");
        }

        //get component and null check sprite render on the shield
        _shieldSpriteRenderer = _bubbleShield.GetComponent<SpriteRenderer>();
        if  (_shieldSpriteRenderer == null)
        {
            Debug.LogError("The Sprite Renderer on the Player Shield is NULL");
        }

    }

    // Update is called once per frame
    void Update()
    {
        PlayerMovement();

        if (Input.GetKey(KeyCode.Space) && Time.time > _canFireTusk)
        {
            FireWeapon();
        }

        PullPowerupsToPlayer();
    }

    void PlayerMovement()
    {
        float _horizontalInput = Input.GetAxisRaw("Horizontal");
        float _verticalInput = Input.GetAxisRaw("Vertical");

        PlayerBoundaries();

        FlipperThrusters();

        Vector3 _direction = new Vector3(_horizontalInput, _verticalInput, 0).normalized;

        //condition to check if player penalty is active or not
        if (_isPlayerPenaltyActive == true)
        {
            transform.Translate(-_direction * _currentSpeed * Time.deltaTime);
        }

        else
        {
            transform.Translate(_direction * _currentSpeed * Time.deltaTime);
        }
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
        if (_isBubbleBlasterActive == true && Time.time > _canFireBubbleBlaster)
        {
            _canFireBubbleBlaster = Time.time + _bubbleBlasterFireRate;
            Vector3 _firePoint = new Vector3(2f, 0.2f, 0);

            //Instantiate Bubble blaster
            Instantiate(_bubbleBlasterPrefab, transform.position + _firePoint, Quaternion.identity);
        }

        else if (_isBubbleBlasterActive == false)
        {
            _canFireTusk = Time.time + _tuskFireRate;
            Vector3 _firePoint = new Vector3(2f, 0.2f, 0);

            //add condition to fire if ammo count is greater than zero
            if (_isTripleTuskActive == true && _currentTuskAmmo > _minTuskAmmo)
            {
                Instantiate(_tripleTuskPrefab, transform.position + _firePoint, Quaternion.identity);

                //play fire sound if ammo available
                _audioSource.PlayOneShot(_pewPewSoundClip);
            }

            //add condition if ammo count is greater than zero
            else if (_currentTuskAmmo > _minTuskAmmo)
            {
                Instantiate(_tuskPrefab, transform.position + _firePoint, Quaternion.identity);

                //play fire sound if ammo available
                _audioSource.PlayOneShot(_pewPewSoundClip);
            }

            //if out of ammo debug log the player is out of ammo
            else
            {
                Debug.Log("The Player is out of Ammo");
                //update UI to show Ammo is out
                //play out of ammo sound clip
            }

            //reduce ammo count by 1
            _currentTuskAmmo--;

            //clamp ammo count between min and max to avoid negative ammo count
            int _tuskAmmoClamp = Mathf.Clamp(_currentTuskAmmo, _minTuskAmmo, _maxTuskAmmo);

            //assign current value to be clamped value
            _currentTuskAmmo = _tuskAmmoClamp;

            //update UI element for Ammo count after firing
            _uiManager.UpdateTuskAmmo(_currentTuskAmmo);

            //tell spawn manager to spawn a single ammo reload if the player is out of ammo
            //conditional bool so it doesn't keep spawning many powerups
            if (_currentTuskAmmo == _minTuskAmmo && _hasSpawnedAmmoReload == false)
            {
                _hasSpawnedAmmoReload = true;
                _spawnManager.PlayerOutOfAmmo();
            }
        }
    }

    void FlipperThrusters()
    {
        //check if the speed boost powerup is active when setting speed on shift release
        //check flipper boost powerup to set minSpeed value
        if (_isFlipperBoostActive == true)
        {
            //make sure flipper boost still works if true
            _minSpeed = _flipperBoostSpeed;
        }

        else if (_isFlipperBoostActive == false)
        {
            //reset minValue to default after being reassigned above
            _minSpeed = _defaultSpeed;
        }

        //clamp the speed to have max and mix values
        float _speedClamp = Mathf.Clamp(_currentSpeed, _minSpeed, _maxSpeed);

        //clamp the thrust value to min and max
        float _thrustClamp = Mathf.Clamp(_currentThrust, _minThrust, _maxThrust);

        //turn off ability to thrust if thrust value is minimum value
        if (_currentThrust <= _minThrust)
        {
            //start coroutine for thruster cool down
            ThrusterCoolDown();

            //tell UI manager to start the cool down for thrusters
            _uiManager.ThrusterCoolDown(_thrusterCoolDownTime);
        }

        //move the player at an increased rate while the shift key is pressed
        //decrease thruster value at an accelerated rate when shift is held down.
        if (Input.GetKey(KeyCode.LeftShift))
        {
            //keep isThrusting from returning true in update for as long as shift is held. One time set to true. 
            if (_isThrustingSwitch == false && _canThrust == true)
            {
                _isThrusting = true;
                _isThrustingSwitch = true;
            }

            //if there is a thrust amount remaining, let the player speed up
            if (_canThrust == true)
            {
                //increase the speed of the player
                //assign the current speed value to be the clamped speed
                //add acceleration factor to clamped speed value as long as shift is held
                _currentSpeed = _speedClamp += _accelFactor * Time.deltaTime;

                //decrease the thrust value of the player
                //assign clamped value
                //subtract deceleration factor when shift is held
                _currentThrust = _thrustClamp -= _thrustDecelFactor * Time.deltaTime;
            }
        }

        //when shift is released, set isThrusting to false and reset the switch 
        else if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            _isThrusting = false;
            _isThrustingSwitch = false;
        }

        //reset player speed to normal when shift is released
        //decelerate the player speed if current speed is greater than minimum speed
        if (_currentSpeed > _minSpeed && _isThrusting == false)
        {
            //assign current speed to speed clamp and subtract deceleration factor
            _currentSpeed = _speedClamp -= _decelFactor * Time.deltaTime;
        }

        //reset the min speed value to stop the decelerator
        else if (_currentSpeed <= _minSpeed)
        {
            //assign the current speed to be the minimum value
            _currentSpeed = _minSpeed;
        }

        //add value to the thrusters if the current value is less than max
        if (_currentThrust < _maxThrust && _isThrusting == false)
        {
            //increase the thruster value at an accelerated rate
            _currentThrust = _thrustClamp += _thrustAccelFactor * Time.deltaTime;
        }

        //reset the max value to stop the accellerator
        else if (_currentThrust >= _maxThrust)
        {
            _currentThrust = _maxThrust;
        }

        //update the UI for thrusters available
        _uiManager.UpdateThruster(_currentThrust);
    }

    public void Damage()
    {
        if (_isShieldActive == true)
        {
            //subtract one from the shield health
            _shieldHealth--;

            //clamp the min value to prevent null reference error
            int _shieldHealthClamp = Mathf.Clamp(_shieldHealth, _minShieldHealth, _maxShieldHealth);

            //assign shield health to be the clamped value
            _shieldHealth = _shieldHealthClamp;

            if (_shieldHealth == 2)
            {
                //change color of the shield via sprite renderer
                _shieldSpriteRenderer.color = _shieldDamageColor1;
            }

            else if (_shieldHealth == 1)
            {
                //change color again
                _shieldSpriteRenderer.color = _shieldDamageColor2;
            }

            //put shield death commands in if statement for zero shield health
            if (_shieldHealth <= _minShieldHealth)
            {
                _isShieldActive = false;
                _bubbleShield.SetActive(false);
                _audioSource.PlayOneShot(_bubblePopSoundClip);
                _shieldSpriteRenderer.color = Color.white;
            }
            
            return;
        }

        else
        {
            //start coroutine on the camera shake script.
            StartCoroutine(_cameraHolder.ShakeTheCamera(0.5f, 0.5f));

            _currentLives--;
            //_lives = _lives -1;
            //_lives -= 1;

            //safeguard to keep lives from going below zero
            int _currentLivesDamageClamp = Mathf.Clamp(_currentLives, _minLives, _maxLives);

            //assign the current life to the clamped value
            _currentLives = _currentLivesDamageClamp;

            _uiManager.UpdateLives(_currentLives);

            PlayerHurtAnimation();

            //trigger Player Damage animation
            _animator.SetTrigger("PlayerHurt");
        }

        //check if current lives is les than max, if so display damage scar
        if (_currentLives < _maxLives)
        {
            _damageScarSingle.SetActive(true);
        }

        //if current lives is less than 2 activate the second damage scar
        if (_currentLives < 2)
        {
            _damageScarDouble.SetActive(true);
        }

        //if lives is less than one, destroy the player
        if (_currentLives < 1)
        {
            _spawnManager.OnPlayerDeath();
            _uiManager.GameOverText();
            Destroy(this.gameObject);
        }
    }

    public void Heal()
    {
        //send a message to the console to let me know I got this far
        Debug.Log("the health powerup hit the player");
        
        //take the current value of lives and add one
        _currentLives++;

        //use Mathf to clamp the lives value between 0 and 3
        int _currentLivesHealClamp = Mathf.Clamp(_currentLives, _minLives, _maxLives);

        //assign current life to be the clamped value
        _currentLives = _currentLivesHealClamp;

        //tell the uiManager to update the lives sprites
        _uiManager.UpdateLives(_currentLives);

        //deactivate the second scar if the player has two or more lives
        if (_currentLives > 1)
        {
            _damageScarDouble.SetActive(false);
        }

        //deactivate the first scar if player has full life
        if (_currentLives == _maxLives)
        {
            _damageScarSingle.SetActive(false);
        }
    }

    public void PlayerReload()
    {
        //set the player's ammo to be the maximum value
        _currentTuskAmmo = _maxTuskAmmo;
        //update the UI to show max ammo visual
        _uiManager.UpdateTuskAmmo(_currentTuskAmmo);
        //reset bool to spawn ammo powerup if empty
        _hasSpawnedAmmoReload = false;
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
        _currentSpeed = _flipperBoostSpeed;
        StartCoroutine(FlipperBoostPowerDownRoutine());
    }

    IEnumerator FlipperBoostPowerDownRoutine()
    {
        _isFlipperBoostActive = true;

        yield return new WaitForSeconds(5f);

        _isFlipperBoostActive = false;

        _currentSpeed = _defaultSpeed;
    }

    public void BubbleBlasterActive()
    {
        //Start coroutine for bobble blaster
        StartCoroutine(BubbleBlasterPowerDownRoutine());
    }

    IEnumerator BubbleBlasterPowerDownRoutine()
    {
        _isBubbleBlasterActive = true;

        yield return new WaitForSeconds(5f);

        _isBubbleBlasterActive = false;
    }

     public void ShieldActive()
    {
        _shieldHealth = _maxShieldHealth;
        _isShieldActive = true;
        _shieldSpriteRenderer.color = Color.white;
        _bubbleShield.SetActive(true);
    }

    //method for thruster cool down
    void ThrusterCoolDown()
    {
        StartCoroutine(ThrusterCoolDownRoutine());
    }

    /*disable thrusters if thrust level hits zero for timed duration
    set is thrusting to false
    reset canThrust to true after time expires*/
    IEnumerator ThrusterCoolDownRoutine()
    {
        _canThrust = false;
        _isThrusting = false;
        _isThrustingSwitch = false;
        yield return new WaitForSeconds(_thrusterCoolDownTime);
        _canThrust = true;
    }

    public void PlayerPenaltyActive()
    {
        StartCoroutine(PlayerPenaltyPowerDownRoutine());
    }

    IEnumerator PlayerPenaltyPowerDownRoutine()
    {
        _isPlayerPenaltyActive = true;

        yield return new WaitForSeconds(5f);

        _isPlayerPenaltyActive = false;
    }

    //method to check C key and set bool for pulling poewrups
    void PullPowerupsToPlayer()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            _isPullingPowrups = true;
        }

        else if (Input.GetKeyUp(KeyCode.C))
        {
            _isPullingPowrups = false;
        }
    }

    //public method for powerup to get return on bool
    public bool PowerupPullCheck()
    {
        return _isPullingPowrups;
    }

    void PlayerHurtAnimation()
    {
        StartCoroutine(PlayerHurtAnimationPowerDownRoutine());
    }

    IEnumerator PlayerHurtAnimationPowerDownRoutine()
    {
        _playerCollider.enabled = false;

        yield return new WaitForSeconds(0.2f);

        _spriteRenderer.color = Color.clear;

        /*set both damage scars to inactive when 
        the sprite render is set to clear*/
        _damageScarSingle.SetActive(false);
        _damageScarDouble.SetActive(false);

        yield return new WaitForSeconds(0.2f);

        _spriteRenderer.color = Color.white;

        /*check current lives to set 
        damage scars active if needed*/
        if (_currentLives < _maxLives)
        {
            _damageScarSingle.SetActive(true);
        }

        if (_currentLives < 2)
        {
            _damageScarDouble.SetActive(true);
        }

        yield return new WaitForSeconds(0.2f);

        _spriteRenderer.color = Color.clear;

        /*set both damage scars to inactive when 
        the sprite render is set to clear*/
        _damageScarSingle.SetActive(false);
        _damageScarDouble.SetActive(false);

        yield return new WaitForSeconds(0.2f);

        _spriteRenderer.color = Color.white;

        /*check current lives to set 
        damage scars active if needed*/
        if (_currentLives < _maxLives)
        {
            _damageScarSingle.SetActive(true);
        }

        if (_currentLives < 2)
        {
            _damageScarDouble.SetActive(true);
        }

        yield return new WaitForSeconds(0.2f);

        _spriteRenderer.color = Color.clear;

        /*set both damage scars to inactive when 
        the sprite render is set to clear*/
        _damageScarSingle.SetActive(false);
        _damageScarDouble.SetActive(false);

        yield return new WaitForSeconds(0.2f);

        _spriteRenderer.color = Color.white;

        /*check current lives to set 
        damage scars active if needed*/
        if (_currentLives < _maxLives)
        {
            _damageScarSingle.SetActive(true);
        }

        if (_currentLives < 2)
        {
            _damageScarDouble.SetActive(true);
        }

        if (_playerCollider != null)
        {
            _playerCollider.enabled = true;
        }
    }
}
