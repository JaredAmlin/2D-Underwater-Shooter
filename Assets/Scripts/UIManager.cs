using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

public class UIManager : MonoBehaviour
{
    [SerializeField] private int _playerScore;

    [SerializeField] private TMP_Text _scoreText;

    //text variable for my ammo to update
    [SerializeField] private TMP_Text _tuskAmmoText;

    [SerializeField] private TMP_Text _clockText;

    [SerializeField] private TMP_Text _gameText;

    [SerializeField] private TMP_Text _overText;

    [SerializeField] private TMP_Text _restartText;

    [SerializeField] private TMP_Text _waveText;

    [SerializeField] private Image _livesImage;
    [SerializeField] private Sprite[] _livesSprites;

    //variable to store the thruster UI fill Image
    [SerializeField] private Image _thrusterFillImage;

    //variable to store the ammo UI fill image
    [SerializeField] private Image _tuskAmmoFillImage;

    //var to store default sprite color
    private Color _maxThrusterColor = Color.white;
    //var to store and select min thruster color
    [SerializeField] private Color _minThrusterColor;
    //var to store transparent color of min thruster color
    [SerializeField] private Color _minThrusterTransparentColor;

    private bool _isThrusterCoolDownActive = false;

    private GameManager _gameManager;

    private int _maxPlayerThrust = 100;
    private int _maxPlayerTuskAmmo = 15;

    //variable to store current wave being passed from spawn manager
    [SerializeField] private int _currentWave;

    // Start is called before the first frame update
    void Start()
    {
        _gameManager = GameObject.Find("Game_Manager").GetComponent<GameManager>();

        if (_gameManager == null)
        {
            Debug.LogError("The Game Manager is NULL");
        }

        _gameText.gameObject.SetActive(false);
        _overText.gameObject.SetActive(false);
        _waveText.gameObject.SetActive(false);
        _playerScore = 0;
    }

    // Update is called once per frame
    void Update()
    {
        _clockText.text = DateTime.Now.ToString();
    }

    public void UpdateScore(int points)
    {
        _playerScore += points;
        _scoreText.text = "Score: " + _playerScore.ToString();
    }

    public void UpdateLives(int currentLives)
    {
        _livesImage.sprite = _livesSprites[currentLives];
    }

    //public method to update current ammo
    //require current int ammo count from player
    public void UpdateTuskAmmo(int currentTuskAmmo)
    {
        //tell ammo text to update Text: + current ammo
        _tuskAmmoText.text = $"Tusks: {currentTuskAmmo}";

        float _currentTuskAmmoFillAmount = ((float) currentTuskAmmo) / ((float) _maxPlayerTuskAmmo);
        //Debug.Log(_currentTuskAmmoFillAmount);

        _tuskAmmoFillImage.fillAmount = _currentTuskAmmoFillAmount;
    }

    //public method to call to send current thrust value to the UI
    public void UpdateThruster(float currentThrust)
    {
        //set the fill of the UI image to refelct the value of the thrusters being used. 
        _thrusterFillImage.fillAmount = currentThrust / _maxPlayerThrust;

        //only use color lerp if thruster cool down is not active
        if (_isThrusterCoolDownActive == false)
        {
            //local float variable to hold fill amount on fill image 
            float _thrusterFillAmount = _thrusterFillImage.fillAmount;
            //use lerp for color fade on thruster use
            _thrusterFillImage.color = Color.Lerp(_minThrusterColor, _maxThrusterColor, _thrusterFillAmount);
        }
    }

    /*public method for player to pass duration of thruster 
     * penalty time when current thrust amount is zero*/
    //require player to pass float of duration time
    public void ThrusterCoolDown(float duration)
    {
        //start the duration cool down to handle the while loop
        StartCoroutine(ThrusterCoolDownDuration(duration));
        //start coroutine for thruster cool down animation
        StartCoroutine(ThrusterCoolDownRoutine());
    }

    //coroutine to pass time duration from player while thrusters are disabled
    //pass duration from player to coroutine through cool down method
    IEnumerator ThrusterCoolDownDuration(float duration)
    {
        //set bool to control while loop animation
        _isThrusterCoolDownActive = true;
        //use duration from player
        yield return new WaitForSeconds(duration);
        //set bool to stop while loop
        _isThrusterCoolDownActive = false;
    }

    //coroutine for cool down animation 
    IEnumerator ThrusterCoolDownRoutine()
    {
        //run while loop for duration of cool down
        //flash color animation while thrusters are cooling down
        while (_isThrusterCoolDownActive == true)
        {
            //set color of thruster fill UI to min color with 50% transparency
            _thrusterFillImage.color = _minThrusterTransparentColor;
            yield return new WaitForSeconds(0.2f);
            //set color of thruster fill UI to min color
            _thrusterFillImage.color = _minThrusterColor;
            yield return new WaitForSeconds(0.2f);
        }
    }

    public void UpdateWaveText(int currentWave)
    {
        _currentWave = currentWave;
        StartCoroutine(UpdateWaveTextRoutine());
    }

    IEnumerator UpdateWaveTextRoutine()
    {
        _waveText.text = "Wave  " + _currentWave.ToString();

        _waveText.gameObject.SetActive(true);
        yield return new WaitForSeconds(2f);

        _waveText.text = "Ready?";
        yield return new WaitForSeconds(2f);

        _waveText.text = "GO!";
        yield return new WaitForSeconds(2f);
        _waveText.gameObject.SetActive(false);
    }

    public void WaveCompletedText()
    {
        StartCoroutine(WaveCompleteTextRoutine());
    }

    IEnumerator WaveCompleteTextRoutine()
    {
        _waveText.text = $"Wave {_currentWave} Complete!";
        _waveText.gameObject.SetActive(true);
        yield return new WaitForSeconds(2f);
        _waveText.text = "Good Job!!!";
        yield return new WaitForSeconds(2f);
        _waveText.gameObject.SetActive(false);
    }

    public void GameOverText()
    {
        _gameManager.GameOver();
        StartCoroutine(GameOverTextRoutine());
    }

    IEnumerator GameOverTextRoutine()
    {
        _gameText.gameObject.SetActive(true);
        yield return new WaitForSeconds(1f);
        _overText.gameObject.SetActive(true);
        yield return new WaitForSeconds(1f);
        _gameText.gameObject.SetActive(false);
        _overText.gameObject.SetActive(false);
        yield return new WaitForSeconds(1f);

        while(true)
        {
            _gameText.gameObject.SetActive(true);
            _overText.gameObject.SetActive(true);
            _restartText.gameObject.SetActive(true);
            yield return new WaitForSeconds(1f);
            _gameText.gameObject.SetActive(false);
            _overText.gameObject.SetActive(false);
            yield return new WaitForSeconds(1f);
        }
    }
}
