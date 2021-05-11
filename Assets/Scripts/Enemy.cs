using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float _speed = 2.5f;

    [SerializeField] private float _bottomRespawnRange = -5.1f;
    [SerializeField] private float _topRespawnRange = 5.1f;

    private Rigidbody _enemyRB;
    private BoxCollider _enemyCollider;

    // Start is called before the first frame update
    void Start()
    {
        _enemyRB = GetComponent<Rigidbody>();
        _enemyCollider = GetComponent<BoxCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        EnemyMovement();
    }

    void EnemyMovement()
    {
        transform.Translate(Vector3.left * _speed * Time.deltaTime);

        EnemyBoundaries();
    }

    void EnemyBoundaries()
    {
        if (transform.position.x <= -12f)
        {
            float _randomY = Random.Range(_bottomRespawnRange, _topRespawnRange);
            Vector3 _enemyRespawnPosition = new Vector3(11.5f, _randomY, 0);
            transform.position = _enemyRespawnPosition;
        }

        if (transform.position.y <= -6.5f)
        {
            Destroy(this.gameObject);
        }

        if (transform.position.x <= -11.5f && _enemyRB.useGravity == true)
        {
            Destroy(this.gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Collided with " + other.transform.name + " at the location of " + other.transform.position);
        
        if (other.tag == "Player")
        {
            Player player = other.transform.GetComponent<Player>();
            
            if (player != null)
            {
                player.Damage();
            }

            Destroy(this.gameObject);
        }

        if (other.tag == "Bullet")
        {
            Destroy(other.gameObject);

            _enemyCollider.enabled = false;
            _enemyRB.useGravity = true;
        }
    }
}
