using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powerup : MonoBehaviour
{
    [SerializeField] private float _speed = 3f;

    private Player _player;

    [SerializeField] private AudioClip _powerupSoundClip;

    //ID for Powerups
    //0 = Triple Tusk, 1 = Flipper Boost, 2 = Bubble Shield, 3 = Health, 4 = Tusk Ammo Reload , 5 = Player Penalty, 6 = Bubble Blaster
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
        transform.Translate(Vector3.left * _speed * Time.deltaTime);

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
                    case 3: //new case to hold my health collectible powerup
                        _player.Heal(); //need to make a healing method on my player script
                        break;
                    case 4: //new case to hold ammo reload powerup
                        //call payer to reload ammo. make reload method in player class. 
                        _player.PlayerReload();
                        break;
                    case 5: //new case for Player Penalty Powerup
                        //call player to start Player Panalty coroutine
                        Debug.Log("The Player Penalty Powerup collided with the Player");
                        _player.PlayerPenaltyActive();
                        break;
                    case 6: //new case for Bubble Blaster Powerup
                        //call player to start Bubble Blaster method
                        Debug.Log("The Bubble Blaster Powerup collided with the Player");
                        _player.BubbleBlasterActive();
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
