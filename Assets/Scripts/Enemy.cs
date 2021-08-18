using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float _speed = 2.5f;
    //varible for thrust for jellyfish
    private float _jellyfishThrust = 150f;

    [SerializeField] private float _bottomRespawnRange = -5.1f;
    [SerializeField] private float _topRespawnRange = 5.1f;

    //raandom int variable to roll for if enemy gets a shield
    private int _randomEnemyShield;

    //varibale for enemy ID
    //0 = piranha, 1 = jellyfish
    [SerializeField] private int _enemyID;

    //bool for if the shield is active
    private bool _isEnemyShieldActive = false;

    //bool for if the enemy is dead
    private bool _isEnemyDead = false;

    //variable for shield game object
    [SerializeField] private GameObject _enemyBubbleShield;

    private Rigidbody2D _enemyRB;
    private PolygonCollider2D _enemyCollider;
    private SpriteRenderer _spriteRenderer;

    private UIManager _uiManager;

    private Player _player;

    // Start is called before the first frame update
    void Start()
    {
        _enemyRB = GetComponent<Rigidbody2D>();

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

        if (_enemyID == 0)
        {
            _enemyCollider = GetComponent<PolygonCollider2D>();
            _spriteRenderer = GetComponent<SpriteRenderer>();

            //give range of 5 for random shield value
            _randomEnemyShield = Random.Range(0, 5);

            if (_randomEnemyShield == 0)
            {
                //set the shield game object to active
                _enemyBubbleShield.SetActive(true);

                //set avtive bool to true
                _isEnemyShieldActive = true;
            }
        }

        else if (_enemyID == 1)
        {
            StartCoroutine(JellyfishMovementRoutine());
        }
    }

    // Update is called once per frame
    void Update()
    {
        EnemyMovement();
    }

    void EnemyMovement()
    {
        //make movement for piranha only
        if (_enemyID == 0)
        {
            transform.Translate(Vector3.left * _speed * Time.deltaTime);
            PiranhaBoundaries();
        }

        else if (_enemyID == 1)
        {
            JellyfishBoundaries();
        }
    }

    IEnumerator JellyfishMovementRoutine()
    {
        while (_isEnemyDead == false)
        {
            _enemyRB.AddForce(transform.up * _jellyfishThrust);
            yield return new WaitForSeconds(1f);
        }
    }

    void PiranhaBoundaries()
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

    void JellyfishBoundaries()
    {
        if (transform.position.y >= 6.5f)
        {
            float randomX = Random.Range(-8f, 8f);
            Vector3 _jellyfishRespawnPosition = new Vector3(randomX, -6.5f, 0);
            transform.position = _jellyfishRespawnPosition;
        }
    }

    //method for checking enemy shields
    void EnemyShieldCheck()
    {
        //check if shield is active
        if (_isEnemyShieldActive == true)
        {
            _isEnemyShieldActive = false;

            _enemyBubbleShield.SetActive(false);

            return;
        }

        else
        {
            EnemyOnDeathBehavior();
        }
    }

    void EnemyOnDeathBehavior()
    {
        if (_enemyID == 0)
        {
            _enemyCollider.enabled = false;
            _spriteRenderer.color = Color.blue;
            _spriteRenderer.flipY = true;
            _enemyRB.gravityScale = 1f;
        }
        
        else if (_enemyID == 1)
        {
            _isEnemyDead = true;
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

            EnemyShieldCheck();
        }

        else if (other.tag == "Tusk")
        {
            if (other != null)
            {
                Destroy(other.gameObject);
            }

            //communicate with UI Manager
            //add 10 points to score
            if (_uiManager != null)
            {
                _uiManager.UpdateScore(10);
            }

            EnemyShieldCheck();
        }

        else if (other.tag == "Bubble_Blaster")
        {
            if (other != null)
            {
                Destroy(other.gameObject);
            }

            //debug message to see if tag is working
            Debug.Log("the enemy hit the bubble blaster");

            //add 10 points to the score
            if (_uiManager != null)
            {
                _uiManager.UpdateScore(10);
            }

            EnemyShieldCheck();
        }
    }
}
