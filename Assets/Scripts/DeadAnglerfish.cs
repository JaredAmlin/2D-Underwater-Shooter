using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadAnglerfish : MonoBehaviour
{
    private float _deathSpeed = 0.5f;

    private Color _maxHealthColor = Color.red;

    private Color _minHealthColor = Color.blue;

    private float _colorLerpValue = 1;

    private SpriteRenderer _anglerfishSpriteRenderer;

    // Start is called before the first frame update
    void Start()
    {
        _anglerfishSpriteRenderer = GetComponent<SpriteRenderer>();

        if (_anglerfishSpriteRenderer == null)
        {
            Debug.LogError("The Sprite Renderer on the Anglerfish is NULL!");
        }
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.left * _deathSpeed * Time.deltaTime);

        if (_colorLerpValue > 0)
        {
            _colorLerpValue -= 0.005f;

            float _clampedColorValue = Mathf.Clamp(_colorLerpValue, 0, 1f);

            _colorLerpValue = _clampedColorValue;

            _anglerfishSpriteRenderer.color = Color.Lerp(_minHealthColor, _maxHealthColor, _colorLerpValue);
        }

        if (transform.position.y < -10f)
        {
            Destroy(this.gameObject);
        }
    }
}
