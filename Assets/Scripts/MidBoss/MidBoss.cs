using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MidBoss : MonoBehaviour
{
    [SerializeField]
    private float _health = 50;
    [SerializeField]
    private float _speed = 5.0f;
    private Vector3 _targetPosition;
    [SerializeField]
    private GameObject _bombPrefab;
    [SerializeField]
    private GameObject _bombPlacement;
    private float _nextFire = 0f;
    private float _fireRate = 5f;
    [SerializeField]
    private int _phase = 1;
    [SerializeField]
    private GameObject _turret1, _turret2;
    private Quaternion _targetRotation;
    [SerializeField]
    private GameObject _forwardTurret;
    [SerializeField]
    private GameObject _smoke1, _smoke2;
    [SerializeField]
    private GameObject _explosionPrefab;
    private bool _isDead = false;

    [SerializeField]
    private AudioClip _fightMusicSource;

    [SerializeField]
    private AudioClip _endMusicSource;


    [SerializeField]
    private float _musicVolume = 1.0f;

    [SerializeField] private AudioClip _explosionSound;
    [SerializeField] private float _explosionVol;
    [SerializeField] protected float _explosionScale = 1.0f;

    [SerializeField] protected GameObject _powerUpPrefab;

    [SerializeField] protected bool _beamHit;
    [SerializeField] protected float _iFrameTime = 0.2f;
    [SerializeField] protected float _beamDamage = 1.0f;
    protected float _hitTime;

    [SerializeField] AudioClip _hitSound;
    //[SerializeField] private HitFlash _hitFlash;

    // Start is called before the first frame update
    void Start()
    {
        AudioManager.Instance.PlayMusic(_fightMusicSource, _musicVolume);

        _targetPosition = new Vector3(19f, 0f, 0f);
        _turret1.SetActive(true);
        _turret2.SetActive(true);
        _targetRotation = Quaternion.Euler(0, 270, 0);
        transform.rotation = Quaternion.Euler(0, 90, 0);

        //_hitFlash = GetComponent<HitFlash>();
    }

    // Update is called once per frame
    void Update()
    {
        Movement();
        
        if (_phase == 1)
        {
            FireBullet();
        }
        else
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, _targetRotation, 1f * Time.deltaTime);
            _turret1.SetActive(false);
            _turret2.SetActive(false);
            _forwardTurret.SetActive(true);
        }
    }

    private void Movement()
    {
        if (_targetPosition == transform.position)
        {
            float x = Random.Range(10f, 23f);
            float y = Random.Range(-14f, 15f);
            _targetPosition = new Vector3(x, y, 0f);
        }

        transform.position = Vector3.MoveTowards(transform.position, _targetPosition, _speed * Time.deltaTime);
    }


    private void FireBullet()
    {
        if (Time.time > _nextFire)
        {
            _nextFire = Time.time + _fireRate;
            Instantiate(_bombPrefab, _bombPlacement.transform.position, Quaternion.identity);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            Damage(1f);
            if (other != null)
            {
                other.GetComponent<PlayerHealthAndDamage>().PlayerDamage();
            }
        }
        if(other.CompareTag("Fireball"))
        {
            Damage(1f);
            Destroy(other.gameObject);
        }
        if (other.CompareTag("Wave"))
        {
            Damage(1f);
        }
    }

    private void Damage(float _damage)
    {
        _health -= _damage;
        AudioManager.Instance.PlayEffect(_hitSound, 1.0f);
        //_hitFlash.DamageFlash();

        if (_health <= 0 && !_isDead)
        {
            _isDead = true;
            Instantiate(_explosionPrefab, transform.position, Quaternion.identity);
       
            AudioManager.Instance.PlayEffect(_explosionSound, _explosionVol);

            AudioManager.Instance.PlayMusic(_endMusicSource, _musicVolume);

            PowerUp();
            GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerScore>().AddScore(1000f);
            Destroy(gameObject, 0.5f);
        }
        else if (_health <= 25)
        {
            _phase = 2;
            _smoke2.SetActive(true);
        }
        else if (_health <= 30)
        {
            _smoke1.SetActive(true);
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

    private void PowerUp()
    {

        float randomPos = Random.Range(0, 4);
        Vector3 newPosition = new Vector3(transform.position.x+ randomPos, transform.position.y+ randomPos, transform.position.z);
        Instantiate(_powerUpPrefab, newPosition, Quaternion.identity);

        randomPos = Random.Range(0, 4);
        newPosition = new Vector3(transform.position.x + randomPos, transform.position.y + randomPos, transform.position.z);
        Instantiate(_powerUpPrefab, newPosition, Quaternion.identity);

        randomPos = Random.Range(0, 4);
        newPosition = new Vector3(transform.position.x + randomPos, transform.position.y + randomPos, transform.position.z);
        Instantiate(_powerUpPrefab, newPosition, Quaternion.identity);
    }



    IEnumerator HitTimer()
    {
        yield return new WaitForSeconds(_iFrameTime);
        _beamHit = false;
    }
}
