using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadEnemy : MonoBehaviour
{
    [SerializeField] private float _deathSpeed = 3.5f;

    private SpriteRenderer _enemySpriteRenderer;

    // Start is called before the first frame update
    void Start()
    {
        _enemySpriteRenderer = GetComponent<SpriteRenderer>();

        if (_enemySpriteRenderer == null)
        {
            Debug.LogError("The Sprite Renderer on the Dead Enemy is NULL!");
        }
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.left * _deathSpeed * Time.deltaTime);

        if (transform.position.x < -12f || transform.position.y < -7f)
        {
            Destroy(this.gameObject);
        }
    }
}
