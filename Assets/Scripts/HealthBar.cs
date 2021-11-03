using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar : MonoBehaviour
{
    private int _maxHealth = 100;

    private Color _maxHealthColor = Color.green;

    private Color _minHealthColor = Color.red;

    private SpriteRenderer _healthBarFillSpriteRenderer;

    private Vector3 _healthBarScale;

    // Start is called before the first frame update
    void Start()
    {
        _healthBarFillSpriteRenderer = GetComponent<SpriteRenderer>();

        if (_healthBarFillSpriteRenderer == null)
        {
            Debug.LogError("The Sprite Renderer on the angler HEALTH FILL is NULL");
        }

        _healthBarFillSpriteRenderer.color = _maxHealthColor;

        _healthBarScale = transform.localScale;
    }

    public void UpdateHealth(int currentHealth)
    {
        float currentFillAmount = ((float) currentHealth) /(float) (_maxHealth);

        _healthBarScale.x = currentFillAmount;

        transform.localScale = _healthBarScale;

        _healthBarFillSpriteRenderer.color = Color.Lerp(_minHealthColor, _maxHealthColor, currentFillAmount);
    }
}
