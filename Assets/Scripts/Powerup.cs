using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powerup : MonoBehaviour
{
    [SerializeField] private float _speed = 3f;

    private Player _player;

    [SerializeField] private AudioClip _powerupSoundClip;

    //ID for Powerups
    [SerializeField] private int _powerupID; //0 = Triple Tusk, 1 = Flipper Boost, 2 = Bubble Shield, 3 = Health 

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
                    default:
                        Debug.LogError ("There is no powerup assigned for this case");
                        break;
                }
            }

            Destroy(this.gameObject);
        }
    }
}
