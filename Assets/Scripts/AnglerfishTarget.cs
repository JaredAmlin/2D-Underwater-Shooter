using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnglerfishTarget : MonoBehaviour
{
    [SerializeField] private int _currentHealth;
    private int _maxHealth = 100;
    private int _minHealth = 0;

    private float _cameraShakeMagnitude;

    private bool _hasIncremented = false;
    private bool _maxIncrement = false;

    //variable to hold my CameraShake class
    private CameraShake _cameraHolder;

    private HealthBar _anglerHealthBar;

    private Anglerfish _anglerfish;

    private UIManager _uiManager;

    // Start is called before the first frame update
    void Start()
    {
        //find the camera holder and get the CameraShake class component
        _cameraHolder = GameObject.Find("Camera_Holder").GetComponent<CameraShake>();

        //debug the GetComponent
        if (_cameraHolder == null)
        {
            Debug.LogError("The Camera Holder is NULL");
        }

        _uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();

        if (_uiManager == null)
        {
            Debug.LogError("The UI Manager is NULL!");
        }

        _anglerHealthBar = GameObject.FindGameObjectWithTag("HealthBar").GetComponent<HealthBar>();

        if (_anglerHealthBar == null)
        {
            Debug.LogError("The HEALTH BAR on the Anglerfish is NULL");
        }

        _anglerfish = GetComponentInParent<Anglerfish>();

        if (_anglerfish == null)
        {
            Debug.LogError("The Anglerfish script is NULL!");
        }

        _cameraShakeMagnitude = Random.Range(0.2f, 0.4f);

        _currentHealth = _maxHealth;
    }

    public void AnglerfishTargetDamage(int damage)
    {
        _currentHealth -= damage;

        int anglerHealthClamp = Mathf.Clamp(_currentHealth, _minHealth, _maxHealth);

        _currentHealth = anglerHealthClamp;

        _anglerHealthBar.UpdateHealth(_currentHealth);

        Debug.Log("The Anglerfish TARGET has been DAMAGED!");

        _uiManager.UpdateScore(50);

        if (_cameraHolder != null)
        {
            //start coroutine on the camera shake script.
            StartCoroutine(_cameraHolder.ShakeTheCamera(0.3f, _cameraShakeMagnitude));
        }

        if (_hasIncremented == false && _currentHealth < 50)
        {
            _hasIncremented = true;
            _anglerfish.IncrementSpeeds();
        }

        else if (_currentHealth < 20 && _maxIncrement == false)
        {
            _maxIncrement = true;
            _anglerfish.IncrementSpeeds();
        }

        if (_currentHealth <= _minHealth)
        {
            _anglerfish.OnDeathBehavior();
        }
    }
}
