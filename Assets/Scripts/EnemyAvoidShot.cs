using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAvoidShot : MonoBehaviour
{
    private float _speed = 2.5f;

    [SerializeField] private float _bottomRespawnRange = -5.1f;
    [SerializeField] private float _topRespawnRange = 5.1f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        EnemyMovement();
    }

    void EnemyMovement()
    {
        transform.Translate(Vector3.left * _speed * Time.deltaTime);

        RedPiranhaBoundaries();
    }

    void MoveUp()
    {
        //move the enemy up
        transform.Translate(Vector3.up * _speed * Time.deltaTime);
    }

    void MoveDown()
    {
        //move the enemy down
        transform.Translate(Vector3.down * _speed * Time.deltaTime);
    }

    void RedPiranhaBoundaries()
    {
        if (transform.position.x <= -12f)
        {
            float _randomY = Random.Range(_bottomRespawnRange, _topRespawnRange);
            Vector3 _enemyRespawnPosition = new Vector3(11.5f, _randomY, 0);
            transform.position = _enemyRespawnPosition;
        }
    }

    private void OnTriggerStay2D (Collider2D other)
    {
        if (other.tag == "Tusk")
        {
            Debug.Log("That was a close one!!!");

            if (other.transform.position.y > this.transform.position.y)
            {
                MoveDown();
            }

            else if (other.transform.position.y <= this.transform.position.y)
            {
                MoveUp();
            }
        }
    }
}
