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

    [SerializeField] private TMP_Text _clockText;

    [SerializeField] private TMP_Text _gameText;

    [SerializeField] private TMP_Text _overText;

    [SerializeField] private TMP_Text _restartText;

    [SerializeField] private Image _livesImage;
    [SerializeField] private Sprite[] _livesSprites;

    private GameManager _gameManager;

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
