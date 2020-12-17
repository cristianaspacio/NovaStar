using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScore : MonoBehaviour
{
    [SerializeField]
    private float _score = 0;
    private Score_Display_UI _scoreUI;

    // Start is called before the first frame update
    void Start()
    {
        _scoreUI = GameObject.Find("Canvas").GetComponent<Score_Display_UI>();

        if (_scoreUI == null)
        {
            Debug.Log("Score Display is NULL");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void AddScore(float points)
    {
        _score += points;
        _scoreUI.UpdateScore(_score);
        Debug.Log("Current Score: " + _score);
    }

    public float GetScore()
    {
        return _score;
    }

    public void SetScore(float newScore)
    {
        _score = newScore;
    }
}
