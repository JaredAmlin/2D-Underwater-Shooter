using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powerup : MonoBehaviour
{
    [SerializeField] private float _speed = 3f;

    private Player _player;

    //ID for Powerups
    [SerializeField] private int _powerupID; //0 = Triple Tusk, 1 = Flipper Boost, 2 = Armor

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
                    default:
                        Debug.LogError ("There is no powerup assigned for this case");
                        break;
                }
            }

            Destroy(this.gameObject);
        }
    }
}
