using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float _speed = 2.5f;

    [SerializeField] private float _bottomRespawnRange = -5.1f;
    [SerializeField] private float _topRespawnRange = 5.1f;

    private Rigidbody2D _enemyRB;
    private PolygonCollider2D _enemyCollider;
    private SpriteRenderer _spriteRenderer;

    private UIManager _uiManager;

    private Player _player;

    // Start is called before the first frame update
    void Start()
    {
        _enemyRB = GetComponent<Rigidbody2D>();
        _enemyCollider = GetComponent<PolygonCollider2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();

        _player = GameObject.Find("Player").GetComponent<Player>();

        if (_player == null)
        {
            Debug.Log("The Player is NULL");
        }

        _uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();

        if (_uiManager == null)
        {
            Debug.LogError("The UI Manager is NULL");
        }
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

        if (transform.position.x <= -11.5f && _enemyRB.gravityScale == 1)
        {
            Destroy(this.gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Enemy collided with " + other.transform.name + " at the location of " + other.transform.position);
        
        if (other.tag == "Player")
        {
            if (_player != null)
            {
                _player.Damage();
            }

            _enemyCollider.enabled = false;
            _spriteRenderer.color = Color.blue;
            _spriteRenderer.flipY = true;
            _enemyRB.gravityScale = 1f;
        }

        if (other.tag == "Tusk")
        {
            Destroy(other.gameObject);

            //communicate with UI Manager
            //add 10 points to score
            if (_uiManager != null)
            {
                _uiManager.UpdateScore(10);
            }

            _enemyCollider.enabled = false;
            _spriteRenderer.color = Color.blue;
            _spriteRenderer.flipY = true;
            _enemyRB.gravityScale = 1f;
        }
    }
}
