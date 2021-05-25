using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powerup : MonoBehaviour
{
    [SerializeField] private float _speed = 3f;

    //ID for Powerups
    [SerializeField] private int _powerupID; //0 = Triple Tusk, 1 = Flipper Boost, 2 = Armor

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
            Player player = other.transform.GetComponent<Player>();

            if (player != null)
            {
                switch (_powerupID)
                {
                    case 0:
                        player.TripleTuskActive();
                        break;
                    case 1:
                        player.FlipperBoostActive();
                        break;
                    case 2:
                        player.ShieldActive();
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
