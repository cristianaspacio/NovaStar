using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinalBoss : MonoBehaviour
{
    //1st attack pattern
    //Boss turn red and fires charged beam
    [SerializeField] private float _maxHp = 100;
    [SerializeField] private float _curHp;

    //base movement
    [SerializeField] private float _speed = 4.0f;
    //_curTarget will set a new target throughout game
    [SerializeField] private Transform _curTarget;
    [SerializeField] private int _randDirection;
    private bool _newDirection;

    //phase 2 movement
    [SerializeField] private bool _p2Active;
    private bool _activateP2;
    private bool _initialP2;
    private bool _stopHpCheck;
    private bool _startP2Move;

    //Phase 1 Ability
    [SerializeField] private GameObject _laserBomb;
    [SerializeField] private Transform _fireHolder;
    private float _laserBombCD;
    private float _laserBombRate;

    //Phase 1 Mini Laser
    [SerializeField] private GameObject _miniLaser;
    [SerializeField] private Transform _miniLaserSpawn1;
    [SerializeField] private Transform _miniLaserSpawn2;
    private float _miniLaserCD;
    private float _miniLaserFireRate;

    //Phase 1 Normal Laser
    [SerializeField] private GameObject _normalLaser;
    [SerializeField] private Transform _normLaserSpawn;
    private float _normLaserCD;
    private float _normLaserFireRate;

    //Phase 2 Bomb routine
    [SerializeField] private Transform _bombHolder1;
    [SerializeField] private Transform _bombHolder2;
    [SerializeField] private Transform _bombHolder3;
    [SerializeField] private GameObject _bombPrefab;
    [SerializeField] private Transform _curBombSpawn;
    private int _randBomb;
    private float _bombCD;
    private float _bombFireRate;

    private Animator _anim;

    [SerializeField] private GameObject _explosionPrefab;
    [SerializeField] private GameObject _smoke1;
    [SerializeField] private GameObject _smoke2;
    [SerializeField] private GameObject _explosionPos;

    [SerializeField] protected bool _beamHit;
    [SerializeField] protected float _iFrameTime = 0.2f;
    [SerializeField] protected float _beamDamage = 1.0f;
    protected float _hitTime;

    [SerializeField] private AudioClip _fightMusicSource;
    [SerializeField] private AudioClip _endMusicSource;
    [SerializeField] private float _musicVolume = .9f;
    [SerializeField] private bool _startBGMusic = true;

    [SerializeField] private AudioClip _explosionSound;
    [SerializeField] private float _explosionVol;
    [SerializeField] protected float _explosionScale = 1.0f;

    [SerializeField] private AudioClip _hitSound;
    [SerializeField] private AudioClip _normalLaserClip;
    private bool _normalLaserClipStart;

    [SerializeField] private AudioClip _bombDropClip;
    [SerializeField] private AudioClip _orbShotClip;
    [SerializeField] private AudioClip _miniLaserClip;

    [SerializeField] private GameObject _bgColorChange;
    //[SerializeField] private HitFlash _hitFlash;

    private CameraShake _cameraShake;
    // Start is called before the first frame update
    void Start()
    {
        //assigning current hp
        _curHp = _maxHp;
        transform.Rotate(new Vector3(0, 270, 0));
        _anim = GetComponent<Animator>();
        //_hitFlash = GetComponent<HitFlash>();

        if (_anim == null)
        {
            Debug.LogError("No Anim");
        }
        _cameraShake = GameObject.FindWithTag("MainCamera").GetComponent<CameraShake>();
        if (_cameraShake == null)
        {
            Debug.LogError("Camera Shake is NULL.");
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (_startBGMusic == true && _anim.GetCurrentAnimatorStateInfo(0).IsTag("2"))
        {
            AudioManager.Instance.PlayMusic(_fightMusicSource, _musicVolume);
            _startBGMusic = false;
        }
        //activates the second phase of the fight
        if (_curHp <= _maxHp * .5 && _stopHpCheck == false)
        {
            _activateP2 = true;
            _initialP2 = true;
            //stops this if statement from being checked to prevent activate bool being set to true
            _stopHpCheck = true;
            _smoke1.SetActive(true);
        }
        if (_curHp <= _maxHp * .25)
        {
            _smoke2.SetActive(true);
        }
        //if second phase abil not active then standard movement
        if (_p2Active == false)
        {
            if (_activateP2 == true)
            {
                //run through initial coroutine only once
                if (_initialP2 == true)
                {
                    StartCoroutine(InitialP2());
                    _initialP2 = false;
                    _activateP2 = false;
                }
                else if (_initialP2 == false)
                {
                    StartCoroutine(ActivateP2());
                    _activateP2 = false;
                }                
            }
        }
        //else stop standard movement, enable second phase movement
        else if (_p2Active == true)
        {
            SecondPhaseAbility();
        }
        P1MiniLaser();
        P1Ability();
        P1NormalLaser();
        P2BombDrop();
    }
    private void SecondPhaseAbility()
    {
        if (_startP2Move == true)
        {
            _anim.SetBool("Charge", true);
        }
        if (transform.position.x <= -40.0f)
        {
            _anim.SetBool("Charge", false);
            _anim.SetBool("BeamRoutine", true);
            _startP2Move = false;          
        }
        if (_anim.GetCurrentAnimatorStateInfo(0).IsTag("2") && _startP2Move == false)
        {
            _p2Active = false;
        }
    }
    //Initial P2 Coroutine has a shorter wait for to start faster
    IEnumerator InitialP2()
    {
        yield return new WaitForSeconds(5.0f);
        //controls which movement function to use
        _p2Active = true;
        //controls coroutine bool
        _activateP2 = true;
        //control movement to first point
        _startP2Move = true;
    }
    IEnumerator ActivateP2()
    {
        yield return new WaitForSeconds(Random.Range(25.0f, 30.0f));
        _p2Active = true;
        _activateP2 = true;
        _startP2Move = true;
    }
    private void P1Ability()
    {
        //activate laser bomb
        //deactivate when finished
        if (_p2Active == false && _anim.GetCurrentAnimatorStateInfo(0).IsTag("2"))
        {
            _laserBombRate = Random.Range(5.0f, 10.0f);
            if (Time.time > _laserBombCD)
            {
                _laserBombCD = Time.time + _laserBombRate;
                Instantiate(_laserBomb, _fireHolder.position, Quaternion.identity);
                AudioManager.Instance.PlayEffect(_orbShotClip, 1.0f);
            }          
        }
    }
    private void P1MiniLaser()
    {
        if (_p2Active == false && _anim.GetCurrentAnimatorStateInfo(0).IsTag("2"))
        {
            _miniLaserFireRate = Random.Range(3.0f, 6.0f);
            if (Time.time > _miniLaserCD)
            {
                _miniLaserCD = Time.time + _miniLaserFireRate;
                Instantiate(_miniLaser, _miniLaserSpawn1.position, Quaternion.identity, _miniLaserSpawn1.transform);
                AudioManager.Instance.PlayEffect(_miniLaserClip, .75f);
            }
        }
    }
    private void P1NormalLaser()
    {
        //if p2Active false
        if (_p2Active == false && _anim.GetCurrentAnimatorStateInfo(0).IsTag("2"))
        {
            _normLaserFireRate = Random.Range(10.0f, 15.0f);
            if (Time.time > _normLaserCD)
            {
                _normLaserCD = Time.time + _normLaserFireRate;
                Instantiate(_normalLaser, _normLaserSpawn.position, Quaternion.identity, _normLaserSpawn.transform);
                _normalLaserClipStart = true;
            }
            if (_normalLaserClipStart == true)
            {
                StartCoroutine(NormalLaserClipPlay());
                _normalLaserClipStart = false;
            }
        }
    }
    IEnumerator NormalLaserClipPlay()
    {
        yield return new WaitForSeconds(1.5f);
        AudioManager.Instance.PlayEffect(_normalLaserClip, .5f);
    }
    private void P2BombDrop()
    {
        //Only active during P2 BombRoutine
        //ends when p2 over
        if (_anim.GetCurrentAnimatorStateInfo(0).IsTag("1"))
        {
            //determines which bomb holder to spawn from
            _randBomb = Random.Range(0, 3);
            if (_randBomb == 0)
            {
                _curBombSpawn = _bombHolder1;
            }
            else if (_randBomb == 1)
            {
                _curBombSpawn = _bombHolder2;
            }
            else if (_randBomb == 2)
            {
                _curBombSpawn = _bombHolder3;
            }

            _bombFireRate = Random.Range(.5f, 1f);
            if (Time.time > _bombCD)
            {
                _bombCD = Time.time + _bombFireRate;
                Instantiate(_bombPrefab, _curBombSpawn.position, Quaternion.identity);
                AudioManager.Instance.PlayEffect(_bombDropClip, 1.0f);
            }            
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Damage(1f);
            if (other != null)
            {
                other.GetComponent<PlayerHealthAndDamage>().PlayerDamage();
            }
        }
        if (other.CompareTag("Fireball"))
        {
            Damage(1f);
            Destroy(other.gameObject);
        }
        if (other.CompareTag("Wave"))
        {
            Damage(1f);
        }
    }
    private void OnTriggerStay(Collider other)
    {
        //Beam Collision
        if (other.CompareTag("Beam"))
        {
            Debug.Log("Hit detected");
            if (_beamHit != true)
            {
                Debug.Log("Damage Dealt");
                Damage(_beamDamage);
                _hitTime = Time.time;
                _beamHit = true;
                StartCoroutine(HitTimer());
            }
        }
    }
    IEnumerator HitTimer()
    {
        yield return new WaitForSeconds(_iFrameTime);
        _beamHit = false;
    }

    private void Damage(float _damage)
    {
        _curHp -= _damage;
        AudioManager.Instance.PlayEffect(_hitSound, 1.0f);
        //_hitFlash.DamageFlash();

        if (_curHp <= 0f)
        {
            _anim.enabled = false;
            this.gameObject.GetComponent<BoxCollider>().enabled = false;
            StartCoroutine(_cameraShake.Shake(3.0f, 3.0f));
            Instantiate(_explosionPrefab, _explosionPos.transform.position, Quaternion.identity);
            AudioManager.Instance.PlayEffect(_explosionSound, 1.0f);
            Destroy(gameObject, 3.0f);
        }       
    }
}
