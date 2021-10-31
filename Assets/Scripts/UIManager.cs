using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

public class UIManager : MonoBehaviour
{
    [SerializeField] private int _playerScore;

    private bool _isTutorialComplete = false;

    [SerializeField] private TMP_Text _scoreText;

    //text variable for my ammo to update
    [SerializeField] private TMP_Text _tuskAmmoText;

    [SerializeField] private TMP_Text _clockText;

    [SerializeField] private TMP_Text _gameText;

    [SerializeField] private TMP_Text _overText;

    [SerializeField] private TMP_Text _restartText;

    [SerializeField] private TMP_Text _waveText;

    [SerializeField] private TMP_Text _tutorialText;

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

    private SpawnManager _spawnManager;

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

        _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>();

        if (_spawnManager == null)
        {
            Debug.LogError("The Spawn Manager is NULL!");
        }

        _gameText.gameObject.SetActive(false);
        _overText.gameObject.SetActive(false);
        _waveText.gameObject.SetActive(false);
        _tutorialText.gameObject.SetActive(false);

        _playerScore = 0;

        StartCoroutine("TutorialTextRoutine");
        StartCoroutine(SkipTutorialRoutine());
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

        if (_currentWave == 3)
        {
            yield return new WaitForSeconds(5f);
            //CONGRATULATIONS!!!
            _waveText.text = "Congratulations!";
            _waveText.gameObject.SetActive(true);
            yield return new WaitForSeconds(2f);
            _waveText.gameObject.SetActive(false);
            yield return new WaitForSeconds(0.5f);

            //YOU WIN!
            _waveText.text = "You win!!!";
            _waveText.gameObject.SetActive(true);
            yield return new WaitForSeconds(2f);
            _waveText.gameObject.SetActive(false);
            yield return new WaitForSeconds(0.5f);

            _gameManager.GameOver();

            while (true)
            {
                //CONGRATULATIONS!!!
                _waveText.text = "Congratulations!";
                _waveText.gameObject.SetActive(true);
                //press r to restart or m for main menu
                _restartText.gameObject.SetActive(true);
                yield return new WaitForSeconds(2f);
                _waveText.gameObject.SetActive(false);
                yield return new WaitForSeconds(0.5f);

                //YOU WIN!
                _waveText.text = "You win!!!";
                _waveText.gameObject.SetActive(true);
                yield return new WaitForSeconds(2f);
                _waveText.gameObject.SetActive(false);
                yield return new WaitForSeconds(0.5f);
            }
        }
    }

    IEnumerator TutorialTextRoutine()
    {
        yield return new WaitForSeconds(1f);
        _tutorialText.text = "Hello!!!";
        _tutorialText.gameObject.SetActive(true);
        yield return new WaitForSeconds(2f);
        _tutorialText.gameObject.SetActive(false);
        yield return new WaitForSeconds(0.5f);

        _tutorialText.text = "Welcome to the ocean!";
        _restartText.text = "Press the 'T' key at any time to Skip the Tutorial";
        _tutorialText.gameObject.SetActive(true);
        _restartText.gameObject.SetActive(true);
        yield return new WaitForSeconds(2f);
        _tutorialText.gameObject.SetActive(false);
        yield return new WaitForSeconds(0.5f);

        _tutorialText.text = "Press the Escape Key at any time to quit the game!";
        _tutorialText.gameObject.SetActive(true);
        yield return new WaitForSeconds(5f);
        _tutorialText.gameObject.SetActive(false);
        yield return new WaitForSeconds(0.5f);

        _tutorialText.text = "Press the Arrow Keys or 'A,W,S,D'  to move the Player";
        _tutorialText.gameObject.SetActive(true);
        yield return new WaitForSeconds(5f);
        _tutorialText.gameObject.SetActive(false);
        yield return new WaitForSeconds(0.5f);

        _tutorialText.text = "Press the 'Left Shift' Key to use the Flipper Boost";
        _tutorialText.gameObject.SetActive(true);
        yield return new WaitForSeconds(5f);
        _tutorialText.gameObject.SetActive(false);
        yield return new WaitForSeconds(0.5f);

        _tutorialText.text = "Press the 'space bar' to fire weapons";
        _tutorialText.gameObject.SetActive(true);
        yield return new WaitForSeconds(5f);
        _tutorialText.gameObject.SetActive(false);
        yield return new WaitForSeconds(0.5f);

        _spawnManager.PlayerOutOfAmmo();

        _tutorialText.text = "Hold the 'C' key to pull powerups toward the player";
        _tutorialText.gameObject.SetActive(true);
        yield return new WaitForSeconds(5f);
        _tutorialText.gameObject.SetActive(false);
        yield return new WaitForSeconds(0.5f);

        _tutorialText.text = "Have Fun!";
        _tutorialText.gameObject.SetActive(true);
        yield return new WaitForSeconds(2f);
        _tutorialText.gameObject.SetActive(false);
        yield return new WaitForSeconds(0.5f);

        _tutorialText.text = "Good Luck!!!";
        _tutorialText.gameObject.SetActive(true);
        yield return new WaitForSeconds(2f);
        _tutorialText.gameObject.SetActive(false);
        _restartText.gameObject.SetActive(false);
        _restartText.text = "Press the 'R' key to try again!";

        _isTutorialComplete = true;

        yield return new WaitForSeconds(1f);

        _spawnManager.SpawnWave();

        yield return null;
    }

    IEnumerator SkipTutorialRoutine()
    {
        while (_isTutorialComplete == false)
        {
            if (Input.GetKeyDown(KeyCode.T))
            {
                _isTutorialComplete = true;
                _tutorialText.gameObject.SetActive(false);
                _restartText.gameObject.SetActive(false);
                _restartText.text = "Press the 'R' key to try again!";
                StopCoroutine("TutorialTextRoutine");

                yield return new WaitForSeconds(1f);

                _spawnManager.SpawnWave();
            }

            yield return null;
        }
    }

    public void BossApproachingText()
    {
        StartCoroutine(BossApproachingTextRoutine());
    }

    IEnumerator BossApproachingTextRoutine()
    {
        yield return new WaitForSeconds(2f);
        _waveText.color = Color.red;
        _waveText.text = "Warning!";
        _waveText.gameObject.SetActive(true);
        yield return new WaitForSeconds(2f);
        _waveText.gameObject.SetActive(false);
        yield return new WaitForSeconds(0.5f);
        _waveText.text = "Boss approaching";
        _waveText.gameObject.SetActive(true);
        yield return new WaitForSeconds(2f);
        _waveText.gameObject.SetActive(false);
        yield return new WaitForSeconds(0.5f);
        _waveText.text = "Warning!";
        _waveText.gameObject.SetActive(true);
        yield return new WaitForSeconds(2f);
        _waveText.gameObject.SetActive(false);
        yield return new WaitForSeconds(0.5f);
        _waveText.text = "Boss approaching";
        _waveText.gameObject.SetActive(true);
        yield return new WaitForSeconds(2f);
        _waveText.gameObject.SetActive(false);
        _waveText.color = Color.white;
    }

    public void BossFightText()
    {
        StartCoroutine(BossFightTextRoutine());
    }

    IEnumerator BossFightTextRoutine()
    {
        yield return new WaitForSeconds(2f);
        _waveText.text = "Ready?";
        _waveText.gameObject.SetActive(true);
        yield return new WaitForSeconds(2f);
        _waveText.gameObject.SetActive(false);
        yield return new WaitForSeconds(0.5f);
        _waveText.text = "Set!";
        _waveText.gameObject.SetActive(true);
        yield return new WaitForSeconds(2f);
        _waveText.gameObject.SetActive(false);
        yield return new WaitForSeconds(0.5f);
        _waveText.color = Color.red;
        _waveText.text = "Fight!!!";
        _waveText.gameObject.SetActive(true);
        yield return new WaitForSeconds(2f);
        _waveText.gameObject.SetActive(false);
        _waveText.color = Color.white;
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
