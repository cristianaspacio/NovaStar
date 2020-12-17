using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField]
    private bool _stopSpawning = false;
    [SerializeField]
    private bool _playerDied = false;
    [SerializeField] private GameObject _enemyContainer;

    // credit to Brackeys start
    private enum SpawnState { SPAWNING, WAITING, COUNTING }

    // Serialized Fields start 
    [System.Serializable]
    public class Wave
    {
        public string name;
        public Transform[] enemy;
        public float count;
        public float rate;
        public GameObject _waveAnim;
    }
    public Wave[] waves;
    public Transform[] spawnPoints;
    public float timeBetweenWaves = 5f;

    [SerializeField] private int nextWave = 0;
    [SerializeField] private bool _checkpointReached = false;
    //Serialized Fields end 

    private float waveCountdown;
    private float searchCountdown = 1f;
    private SpawnState state = SpawnState.COUNTING;
    
    private GameObject[] objectsToDestroy;
    private GameObject[] powerupToDestroy;
    private GameObject _player;
    private WavePanelManager _wavePanelManager;
    private PlayerScore _playerScore;
    private Score_Display_UI _scoreDisplayUI;
    private PlayerHealthAndDamage _playerHP;
    private BackgroundColorChange _bgColor;
    private CanvasManager _canvasManager;

    void Start()
    {
        _player = GameObject.Find("Player");
        _wavePanelManager = GameObject.Find("Wave_Panel").GetComponent<WavePanelManager>();
        _playerScore = GameObject.Find("Player").GetComponent<PlayerScore>();
        _scoreDisplayUI = GameObject.Find("Canvas").GetComponent<Score_Display_UI>();
        _playerHP = GameObject.Find("Player").GetComponent<PlayerHealthAndDamage>();
        _bgColor = GameObject.Find("Environment").GetComponent<BackgroundColorChange>();
        _canvasManager = GameObject.Find("Canvas").GetComponent<CanvasManager>();

        if (_bgColor == null)
        {
            Debug.LogError("Cant find Background color change");
        }
        if (_playerHP == null)
        {
            Debug.LogError("Cant find Player Hp and Damage");
        }
        if (_player == null)
        {
            Debug.LogError("Cant find player for SpawnManager");
        }
        if (_wavePanelManager == null)
        {
            Debug.LogError("Cant find WavePanelManger ui component");
        }

        if(_scoreDisplayUI == null)
        {
            Debug.Log("Cant find the UI for SpawnManager");
        }

        if (spawnPoints.Length <= 0)
        {
            Debug.LogError("No SpawnPoints found");
        }

        waveCountdown = timeBetweenWaves;
    }

    void Update()
    {
        if (_checkpointReached == true && _playerDied == true)
        {
            _playerDied = false;
            _stopSpawning = false;
            _playerScore.SetScore(0f);
            _scoreDisplayUI.UpdateScore(0f);
            _canvasManager.SetPlayerDead();
        }

        if (!_player.activeInHierarchy)
        {
            _wavePanelManager.WaveIsDone();
            _stopSpawning = true;
        }

        if (!_stopSpawning)
        {
            if (state == SpawnState.WAITING)
            {

                if (!EnemyIsAlive())
                {
                    WaveCompleted();
                    _wavePanelManager.UpdateWave(nextWave);
                }
                else
                {
                    return;
                }
            }

            if (waveCountdown <= 0)
            {
                if (state != SpawnState.SPAWNING)
                {
                    StartCoroutine(SpawnWave(waves[nextWave]));
                }
            }
            else
            {
                waveCountdown -= Time.deltaTime;
            }
        }
        if (nextWave >= 3)
        {
            _checkpointReached = true;
        }
    }
    void WaveCompleted()
    {
        Debug.Log("Wave Completed " + nextWave);
        state = SpawnState.COUNTING;
        waveCountdown = timeBetweenWaves;

        if (nextWave + 1 > waves.Length - 1)
        {
            //nextWave = 0;
            Debug.Log("loop");
            _stopSpawning = true;
        }

        else
        {
            nextWave++;
        }
    }
    bool EnemyIsAlive()
    {
        searchCountdown -= Time.deltaTime;
        if (searchCountdown <= 0f)
        {
            searchCountdown = 1f;
            if (GameObject.FindGameObjectWithTag("Enemy") == null)
            {
                return false;
            }
        }
        return true;
    }

    IEnumerator SpawnWave(Wave _wave)
    {
        _wavePanelManager.WaveIsDone();
        state = SpawnState.SPAWNING;
        SpawnWaveAnim(_wave);

        for (int i = 0; i < _wave.count; i++)
        {
            var _randRate = Random.Range(_wave.rate, _wave.rate * 1.5f);
            SpawnEnemy(_wave.enemy[Random.Range(0, _wave.enemy.Length)]);
            yield return new WaitForSeconds(_randRate);
        }
        state = SpawnState.WAITING;
        yield break;
    }

    void SpawnEnemy(Transform _enemy)
    {
        Transform _sp = spawnPoints[Random.Range(0, spawnPoints.Length)];
        Instantiate(_enemy, _sp.position, _sp.rotation);
    }

    //spawn animation
    void SpawnWaveAnim(Wave _wave)
    {
        if (_wave._waveAnim != null)
        {
            Vector3 _animPos = new Vector3(0f, 0f, 0f);
            Instantiate(_wave._waveAnim, _animPos, Quaternion.identity);
        }
    }

    public void RestartFromCheckpoint()
    {
        if (nextWave >= 4 && nextWave <= 6)
        {
           nextWave = 3;
        }
        if (nextWave >= 7 && nextWave <= 13)
        {
            nextWave = 6;
        }
        if (nextWave >= 14)
        {
            nextWave = 13;
        }
        _playerDied = true;
        _player.SetActive(true);
        _player.transform.position = new Vector3(-20, 0, 0);
        _playerHP.PlayerRespawn();
        _bgColor.FinalBossNotActive();
        DestroyAllObjects();
        WaveCompleted();
    }
    public bool GetCheckPointStatus()
    {
        return _checkpointReached;
    }

    public bool DidWin()
    {
        if (nextWave + 1 > waves.Length - 1 && _stopSpawning)
        {
            return true;
        } else
        {
            return false;
        }
    }

    void DestroyAllObjects()
    {
        objectsToDestroy = GameObject.FindGameObjectsWithTag("Enemy");
        powerupToDestroy = GameObject.FindGameObjectsWithTag("PowerUp");

        for (var i = 0; i < objectsToDestroy.Length; i++)
        {
            Destroy(objectsToDestroy[i]);
        }
        for (var i = 0; i < powerupToDestroy.Length; i++)
        {
            Destroy(powerupToDestroy[i]);
        }
    }

    public float GetWave()
    {
        return nextWave;
    }
}
