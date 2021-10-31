using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnDeathColorLerp : MonoBehaviour
{
    private Color _maxHealthColor = Color.red;

    private Color _minHealthColor = Color.blue;

    private float _colorLerpValue = 1;

    private SpriteRenderer _spriteRenderer;

    // Start is called before the first frame update
    void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();

        if (_spriteRenderer == null)
        {
            Debug.LogError("The Sprite Renderer on the Anglerfish JAW is NULL!");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (_colorLerpValue > 0)
        {
            _colorLerpValue -= 0.005f;

            float _clampedColorValue = Mathf.Clamp(_colorLerpValue, 0, 1f);

            _colorLerpValue = _clampedColorValue;

            _spriteRenderer.color = Color.Lerp(_minHealthColor, _maxHealthColor, _colorLerpValue);
        }
    }
}
