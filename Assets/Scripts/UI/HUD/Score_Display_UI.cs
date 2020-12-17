using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Score_Display_UI : MonoBehaviour
{
    [SerializeField]
    private Text _scoreText;

    private PlayerScore _playerScore;

    // Start is called before the first frame update
    void Start()
    {
        _playerScore = GameObject.Find("Player").GetComponent<PlayerScore>();

        if (_playerScore == null)
        {
            Debug.Log("Player Score is NULL");
        }

        _scoreText.text = "SCORE: " + 0;
    }

    public void UpdateScore(float playerScore)
    {
        _scoreText.text = "SCORE: " + playerScore.ToString();
    }

    public float GetUIScore()
    {
        return _playerScore.GetScore();
    }
}
