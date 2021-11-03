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

    [SerializeField] private TMP_Text _tuskAmmoText;

    [SerializeField] private TMP_Text _clockText;

    [SerializeField] private TMP_Text _gameText;

    [SerializeField] private TMP_Text _overText;

    [SerializeField] private TMP_Text _restartText;

    [SerializeField] private TMP_Text _waveText;

    [SerializeField] private TMP_Text _tutorialText;

    [SerializeField] private Image _livesImage;
    [SerializeField] private Sprite[] _livesSprites;

    [SerializeField] private Image _thrusterFillImage;

    [SerializeField] private Image _tuskAmmoFillImage;

    private Color _maxThrusterColor = Color.white;
    [SerializeField] private Color _minThrusterColor;
    [SerializeField] private Color _minThrusterTransparentColor;

    private bool _isThrusterCoolDownActive = false;

    private GameManager _gameManager;

    private SpawnManager _spawnManager;

    private int _maxPlayerThrust = 100;
    private int _maxPlayerTuskAmmo = 15;

    [SerializeField] private int _currentWave;

    private WaitForSeconds _waitForActiveText;
    private WaitForSeconds _waitForInactiveText;
    private WaitForSeconds _waitForTutorialText;

    private WaitForSeconds _thrusterFillImageSeconds;

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

        _waitForActiveText = new WaitForSeconds(2f);
        _waitForInactiveText = new WaitForSeconds(0.5f);
        _waitForTutorialText = new WaitForSeconds(5f);
        _thrusterFillImageSeconds = new WaitForSeconds(0.2f);

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

    public void UpdateTuskAmmo(int currentTuskAmmo)
    {
        _tuskAmmoText.text = $"Tusks: {currentTuskAmmo}";

        float _currentTuskAmmoFillAmount = ((float) currentTuskAmmo) / ((float) _maxPlayerTuskAmmo);

        _tuskAmmoFillImage.fillAmount = _currentTuskAmmoFillAmount;
    }

    public void UpdateThruster(float currentThrust)
    {
        _thrusterFillImage.fillAmount = currentThrust / _maxPlayerThrust;

        if (_isThrusterCoolDownActive == false)
        { 
            float _thrusterFillAmount = _thrusterFillImage.fillAmount;
           
            _thrusterFillImage.color = Color.Lerp(_minThrusterColor, _maxThrusterColor, _thrusterFillAmount);
        }
    }

    public void ThrusterCoolDown(float duration)
    {
        StartCoroutine(ThrusterCoolDownDuration(duration));
        StartCoroutine(ThrusterCoolDownRoutine());
    }

    IEnumerator ThrusterCoolDownDuration(float duration)
    {
        _isThrusterCoolDownActive = true;
    
        yield return new WaitForSeconds(duration);
        
        _isThrusterCoolDownActive = false;
    }
 
    IEnumerator ThrusterCoolDownRoutine()
    {
        while (_isThrusterCoolDownActive == true)
        {
            _thrusterFillImage.color = _minThrusterTransparentColor;
            yield return _thrusterFillImageSeconds;
          
            _thrusterFillImage.color = _minThrusterColor;
            yield return _thrusterFillImageSeconds;
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
        yield return _waitForActiveText;

        _waveText.text = "Ready?";
        yield return _waitForActiveText;

        _waveText.text = "GO!";
        yield return _waitForActiveText;
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
        yield return _waitForActiveText;

        _waveText.text = "Good Job!!!";
        yield return _waitForActiveText;
        _waveText.gameObject.SetActive(false);

        if (_currentWave == 3)
        {
            yield return new WaitForSeconds(5f);
            //CONGRATULATIONS!!!
            _waveText.text = "Congratulations!";
            _waveText.gameObject.SetActive(true);
            yield return _waitForActiveText;
            _waveText.gameObject.SetActive(false);
            yield return _waitForInactiveText;

            //YOU WIN!
            _waveText.text = "You win!!!";
            _waveText.gameObject.SetActive(true);
            yield return _waitForActiveText;
            _waveText.gameObject.SetActive(false);
            yield return _waitForInactiveText;

            _gameManager.GameOver();

            while (true)
            {
                //CONGRATULATIONS!!!
                _waveText.text = "Congratulations!";
                _waveText.gameObject.SetActive(true);
                //press r to restart or m for main menu
                _restartText.gameObject.SetActive(true);
                yield return _waitForActiveText;
                _waveText.gameObject.SetActive(false);
                yield return _waitForInactiveText;

                //YOU WIN!
                _waveText.text = "You win!!!";
                _waveText.gameObject.SetActive(true);
                yield return _waitForActiveText;
                _waveText.gameObject.SetActive(false);
                yield return _waitForInactiveText;
            }
        }
    }

    IEnumerator TutorialTextRoutine()
    {
        yield return new WaitForSeconds(1f);
        _tutorialText.text = "Hello!!!";
        _tutorialText.gameObject.SetActive(true);
        yield return _waitForActiveText;
        _tutorialText.gameObject.SetActive(false);
        yield return _waitForInactiveText;

        _tutorialText.text = "Welcome to the ocean!";
        _restartText.text = "Press the 'T' key at any time to Skip the Tutorial";
        _tutorialText.gameObject.SetActive(true);
        _restartText.gameObject.SetActive(true);
        yield return _waitForActiveText;
        _tutorialText.gameObject.SetActive(false);
        yield return _waitForInactiveText;

        _tutorialText.text = "Press the Escape Key at any time to quit the game!";
        _tutorialText.gameObject.SetActive(true);
        yield return _waitForTutorialText;
        _tutorialText.gameObject.SetActive(false);
        yield return _waitForInactiveText;

        _tutorialText.text = "Press the Arrow Keys or 'A,W,S,D'  to move the Player";
        _tutorialText.gameObject.SetActive(true);
        yield return _waitForTutorialText;
        _tutorialText.gameObject.SetActive(false);
        yield return _waitForInactiveText;

        _tutorialText.text = "Press the 'Left Shift' Key to use the Flipper Boost";
        _tutorialText.gameObject.SetActive(true);
        yield return _waitForTutorialText;
        _tutorialText.gameObject.SetActive(false);
        yield return _waitForInactiveText;

        _tutorialText.text = "Press the 'space bar' to fire weapons";
        _tutorialText.gameObject.SetActive(true);
        yield return _waitForTutorialText;
        _tutorialText.gameObject.SetActive(false);
        yield return _waitForInactiveText;

        _spawnManager.PlayerOutOfAmmo();

        _tutorialText.text = "Hold the 'C' key to pull powerups toward the player";
        _tutorialText.gameObject.SetActive(true);
        yield return _waitForTutorialText;
        _tutorialText.gameObject.SetActive(false);
        yield return _waitForInactiveText;

        _tutorialText.text = "Have Fun!";
        _tutorialText.gameObject.SetActive(true);
        yield return _waitForActiveText;
        _tutorialText.gameObject.SetActive(false);
        yield return _waitForInactiveText;

        _tutorialText.text = "Good Luck!!!";
        _tutorialText.gameObject.SetActive(true);
        yield return _waitForActiveText;
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
