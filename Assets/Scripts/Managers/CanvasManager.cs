using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasManager : MonoBehaviour
{
    [SerializeField] private GameObject _endPanel;
    [SerializeField] private GameObject _wavePanel;
    private Button _checkPointButton;
    private PlayerHealthAndDamage _player;
    private SpawnManager _spawnManager;
    private Text _winText;
    private Text _diedText;
    private bool _playerDead;
    void Start()
    {
        _spawnManager = GameObject.Find("SpawnManager").GetComponent<SpawnManager>();
        _player = GameObject.Find("Player").GetComponent<PlayerHealthAndDamage>();
        _wavePanel.SetActive(true);
        
        if (_player == null)
        {
            Debug.Log("Missing Player Health");
        }
        if (_spawnManager == null)
        {
            Debug.Log("Cant find spawn manager");
        }
    }
    void Update()
    {
        CheckEndPanelStatus();
        CheckWinStatus();
    }

    void CheckEndPanelStatus()
    {
        if (!_player.getPlayerStatus())
        {
            _endPanel.SetActive(true);
            _wavePanel.SetActive(false);
            _playerDead = true;
            _diedText = GameObject.Find("Died_Text").GetComponent<Text>();
            _diedText.enabled = true;
            if (_spawnManager.GetCheckPointStatus())
            {
                _checkPointButton = GameObject.Find("Checkpoint_Button").GetComponent<Button>();
                _checkPointButton.interactable = true;
            }
        }
        else
        {
            _endPanel.SetActive(false);
            _wavePanel.SetActive(true);
        }
    }

    void CheckWinStatus()
    {
        if (_spawnManager.DidWin()  && _playerDead == false)
        {       
            _wavePanel.SetActive(false);
            _endPanel.SetActive(true);

            _diedText = GameObject.Find("Died_Text").GetComponent<Text>();
            _diedText.enabled = false;

            _winText = GameObject.Find("Win_Text").GetComponent<Text>();
            _winText.enabled = true;         
        }
    }
    public void SetPlayerDead()
    {
        _playerDead = false;       
    }
}
