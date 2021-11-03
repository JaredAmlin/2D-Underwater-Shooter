using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powerup : MonoBehaviour
{
    [SerializeField] private float _speed = 3f;

    [SerializeField] private float _playerPullSpeed = 5f;

    private Player _player;

    private bool _isPlayerPullingPowerups = false;

    [SerializeField] private AudioClip _powerupSoundClip;

    //0 = Triple Tusk, 1 = Flipper Boost, 2 = Bubble Shield, 3 = Health, 4 = Tusk Ammo Reload , 5 = Player Penalty, 6 = Bubble Blaster, 7 = Homing Tusk
    [SerializeField] private int _powerupID;

    // Start is called before the first frame update
    private void Start()
    {
        _player = GameObject.Find("Player").GetComponent<Player>();

        if (_player == null)
        {
            Debug.Log("The Player is NULL");
        }
    }

    // Update is called once per frame
    void Update()
    {
        PowerupMovement();
    }

    void PowerupMovement()
    {
        if (_player != null)
        {
            _isPlayerPullingPowerups = _player.PowerupPullCheck();
        }

        if (_player != null && _isPlayerPullingPowerups == true)
        {
            transform.position = Vector2.MoveTowards(this.transform.position, _player.transform.position, _playerPullSpeed * Time.deltaTime);
        }

        else
        {
            transform.Translate(Vector3.left * _speed * Time.deltaTime);
        }

        PowerupBoundaries();
    }

    void PowerupBoundaries()
    {
        if (transform.position.x <= -11.5f)
        {
            Destroy(this.gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            AudioSource.PlayClipAtPoint(_powerupSoundClip, new Vector3 (0, 0, -10));

            if (_player != null)
            {
                switch (_powerupID)
                {
                    case 0:
                        _player.TripleTuskActive();
                        break;
                    case 1:
                        _player.FlipperBoostActive();
                        break;
                    case 2:
                        _player.ShieldActive();
                        break;
                    case 3: 
                        _player.Heal(); 
                        break;
                    case 4:  
                        _player.PlayerReload();
                        break;
                    case 5:
                        _player.PlayerPenaltyActive();
                        break;
                    case 6: 
                        _player.BubbleBlasterActive();
                        break;
                    case 7:
                        _player.HomingTuskActive();
                        break;
                    default:
                        Debug.LogError ("There is no powerup assigned for this case");
                        break;
                }
            }

            Destroy(this.gameObject);
        }
    }
}
