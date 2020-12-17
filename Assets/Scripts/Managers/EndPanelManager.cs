using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndPanelManager : MonoBehaviour
{
    private GameObject _endPanel;
    private SpawnManager _spawnManager;
    private Text _finalScore;
    private Text _finalWave;
    private Score_Display_UI _playerScore;
    void Start()
   {
        _endPanel = GameObject.Find("End_Panel");
        _playerScore = GameObject.Find("Canvas").GetComponent<Score_Display_UI>();
        _spawnManager = GameObject.Find("SpawnManager").GetComponent<SpawnManager>();
        _finalScore = GameObject.Find("Endpanel_Score_Text").GetComponent<Text>();
        _finalWave = GameObject.Find("Wave_Number").GetComponent<Text>();
        
        if (_playerScore == null)
        {
            Debug.LogError("Cant find the HUD in EndPanel");
        }

        if(_endPanel == null)
        {
            Debug.Log("Missing end panel");
        }
        
        if (_spawnManager == null)
        {
            Debug.Log("Cant find spawn manager");
        }
    }

    private void Update()
    {
        //_finalScore.text = "Final Score: " + _playerScore.GetUIScore();
        _finalWave.text = "You survived " + _spawnManager.GetWave().ToString() + " total waves";
    }

    public void LoadLevel()
    {
        // Load the Game Scene
        SceneManager.LoadScene(2);
    }

    public void LoadCheckpoint()
    {
        _spawnManager.RestartFromCheckpoint();
        _endPanel.SetActive(false);
    }

    public void QuitLevel()
    {
        // Load the Main Menu Scene
        SceneManager.LoadScene(0);
    }
}