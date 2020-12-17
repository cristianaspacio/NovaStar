using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyAbstractClass : MonoBehaviour
{
    [SerializeField] protected float _hp;
    [SerializeField] protected float _speed = 2f;
    [SerializeField] protected float _points = 100;
    [SerializeField] protected float _explosionScale = 1.0f;

    protected float _hitTime;

    protected Animator _anim;
    protected BoxCollider _boxCollider;

    [SerializeField]
    protected GameObject _player;

    [SerializeField]
    protected PlayerScore _playerScore;

    [SerializeField] protected GameObject _explosionAnim;
    [SerializeField] protected GameObject _enemyWeapon;
    [SerializeField] protected GameObject _powerUpPrefab;
    [SerializeField] protected Transform _weaponPos;
    [SerializeField] protected float _fireCD;
    [SerializeField] 
    [Tooltip("The lower the number the faster it fires")]
    protected float _fireRate = 3.0f;

    [SerializeField] protected bool _beamHit;
    [SerializeField] protected bool _onScreen;
    [SerializeField] protected bool _dying;

    [SerializeField] protected float _iFrameTime;
    [SerializeField] protected float _beamDamage;

    [SerializeField] private AudioClip _explosionSound;
    [SerializeField] private float _explosionVol;

    //private GameObject _firedShot;

    [SerializeField] public float _shotSpeed;

    [SerializeField] private AudioClip _hitSound;
    [SerializeField] private HitFlash _hitFlash;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        _anim = transform.GetComponent<Animator>();
        _boxCollider = transform.GetComponent<BoxCollider>();
        //_hitFlash = transform.GetComponent<HitFlash>();

        _player = GameObject.Find("Player");

        if (_player != null)
        {
            _playerScore = _player.GetComponent<PlayerScore>();
        }

        PowerUp();

    }

    // Update is called once per frame
    protected virtual void Update()
    {
        //Most movement will be made in animation.
        OnScreenCheck();
    }

    protected virtual void Movement()
    {
        transform.Translate(Vector3.forward * _speed * Time.deltaTime);
    }
    protected virtual void WeaponFire()
    {
        if (Time.time > _fireCD)
        {
            _fireCD = Time.time + _fireRate;

            GameObject _firedShot = Instantiate(_enemyWeapon, _weaponPos.position, Quaternion.identity);
            _firedShot.GetComponent<EnemyShot>()._speed = _shotSpeed;
        }
    }
    public virtual void Damage(float _damageTaken)
    {
        if (_onScreen)
        {
            Debug.Log("Damage Taken");
            _hp -= _damageTaken;
            AudioManager.Instance.PlayEffect(_hitSound, 1f);
            //_hitFlash.DamageFlash();
        }

        if (_hp <= 0)
        {
            _speed = 0;
            _dying = true;

            _playerScore.AddScore(_points);
            Debug.Log("Enemy Destroyed");

            PowerUp();

            if (_explosionAnim != null)
            {
                Debug.Log("Explosion");

                GameObject explosion = Instantiate(_explosionAnim, transform.position, Quaternion.identity);

                explosion.gameObject.GetComponent<ExplosionAnim>()._sfxSource = _explosionSound;
                explosion.gameObject.GetComponent<ExplosionAnim>()._volume = _explosionVol;

                explosion.transform.localScale = new Vector3(_explosionScale, _explosionScale, _explosionScale);

            }

            Destroy(this.gameObject, 0.01f);

        }

    }  

    //PowerUp method

    protected virtual void PowerUp()
    {
        int randomNum = Random.Range(1, 6);
        if (randomNum == 1)
        {
            Instantiate(_powerUpPrefab, transform.position, Quaternion.identity);    
        }
    }
    protected virtual void OnTriggerStay(Collider other)
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

    protected virtual void OnScreenCheck()
    {
        if (transform.position.x < 33.0f && transform.position.x > -33.0f)
        {
            _onScreen = true;
        }
        else
        {
            _onScreen = false;
        }
    }
    IEnumerator HitTimer()
    {
        yield return new WaitForSeconds(_iFrameTime);
        _beamHit = false;
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            Damage(1.0f);

            if (other != null)
            {
                 other.GetComponent<PlayerHealthAndDamage>().PlayerDamage();
            } 
        }

        if(other.tag == "Fireball")
        {
            Destroy(other.gameObject);
        }


        if(other.tag == "Beam")
        {
            _iFrameTime = other.GetComponent<LaserBeam>()._hitDelay;
            _beamDamage = other.GetComponent<LaserBeam>()._damage;
        }


    }

}
