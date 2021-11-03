using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnglerfishLight : MonoBehaviour
{
    private float _jawOpenWaitTime = 5f;

    [SerializeField] private int _lightHealth = 3;

    private bool _isJawOpen = false;

    [SerializeField] private float _lanternFireRate = 2f;

    private WaitForSeconds _waitForSecondsFireRate;

    private SpriteRenderer _anglerSpriteRenderer;

    [SerializeField] private Color _fullLightColor;
    [SerializeField] private Color _midLightColor;
    [SerializeField] private Color _lowLightColor;
    [SerializeField] private Color _invisibleAlpha;

    private Animator _anglerJawAnim;

    private Player _player;

    private Anglerfish _anglerfish;

    private UIManager _uIManager;

    [SerializeField] private GameObject _anglerWeakSpot;

    [SerializeField] private GameObject _lanternShot;

    private CircleCollider2D _lanternCollider;

    // Start is called before the first frame update
    void Start()
    {
        _uIManager = GameObject.Find("Canvas").GetComponent<UIManager>();

        if  (_uIManager == null)
        {
            Debug.LogError("The UI anager is NULL!");
        }

        _lanternCollider = GetComponent<CircleCollider2D>();

        if (_lanternCollider == null)
        {
            Debug.LogError("The COLLIDER on the LANTERn is NULL!");
        }

        _anglerSpriteRenderer = GetComponent<SpriteRenderer>();

        if (_anglerSpriteRenderer == null)
        {
            Debug.LogError("The Sprite Renderer on the Angler LIGHT is NULL");
        }

        _anglerJawAnim = GameObject.Find("Anglerfish_Jaw").GetComponent<Animator>();

        if (_anglerJawAnim == null)
        {
            Debug.LogError("The ANIMATOR on the Angler JAW is NULL");
        }

        if (_anglerWeakSpot == null)
        {
            Debug.LogError("The Angler WEAK SPOT is NULL");
        }

        _player = GameObject.Find("Player").GetComponent<Player>();

        if (_player == null)
        {
            Debug.Log("The Player class is NULL");
        }

        _anglerfish = GetComponentInParent<Anglerfish>();

        if (_anglerfish == null)
        {
            Debug.Log("The Anglerfish class is NULL");
        }

        _lanternCollider.enabled = false;

        _anglerSpriteRenderer.color = _invisibleAlpha;

        _waitForSecondsFireRate = new WaitForSeconds(_lanternFireRate);
    }

    public void AnglerfishLightDamage()
    {
        _lightHealth--;

        int lightHealthClamp = Mathf.Clamp(_lightHealth, 0, 3);

        _lightHealth = lightHealthClamp;

        _uIManager.UpdateScore(20);

        if (_lightHealth == 2)
        {
            _anglerSpriteRenderer.color = _midLightColor;
        }

        else if (_lightHealth == 1)
        {
            _anglerSpriteRenderer.color = _lowLightColor;
        }

        if (_lightHealth <= 0 && _isJawOpen == false)
        {
            _anglerSpriteRenderer.color = _invisibleAlpha;

            _lanternCollider.enabled = false;

            _isJawOpen = true;

            StartCoroutine(AnglerJawRoutine());
        }
    }

    IEnumerator AnglerJawRoutine()
    {
        _anglerfish.SpeedDown();

        if (_anglerJawAnim != null)
        {
            _anglerJawAnim.SetTrigger("JawOpen");
        }

        if (_anglerWeakSpot != null)
        {
            _anglerWeakSpot.SetActive(true);
        }

        yield return new WaitForSeconds(_jawOpenWaitTime);

        if (_anglerJawAnim != null)
        {
            _anglerJawAnim.SetTrigger("JawClosed");
        }

        if (_anglerWeakSpot != null)
        {
            _anglerWeakSpot.SetActive(false);
        }

        yield return new WaitForSeconds(0.5f);

        _anglerfish.RammingAnglerfish();

        yield return new WaitForSeconds(2f);

        _lightHealth = 3;
        _isJawOpen = false;
        _anglerSpriteRenderer.color = _fullLightColor;

        _lanternCollider.enabled = true;

        _anglerfish.SpeedUp();
        
        StartCoroutine(LanternFireRoutine());
    }

    public void StartLantern()
    {
        _anglerSpriteRenderer.color = _fullLightColor;
        _lanternCollider.enabled = true;
        StartCoroutine(LanternFireRoutine());
    }

    IEnumerator LanternFireRoutine()
    {
        while (_isJawOpen == false)
        {
            if (_player != null)
            {
                Instantiate(_lanternShot, transform.position, Quaternion.identity);
            }

            yield return _waitForSecondsFireRate;
        }
    }

    public bool IsJawOpen()
    {
        return _isJawOpen;
    }
}
