using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_SpeedCruiser : EnemyAbstractClass
{
    // Boolean controls enemy movement.
    private bool moveActive;

    // Controls the amount of time the enemy will stop for.
    private float _stopTime;

    // Tracks if the enemy has fired a single shot each loop around.
    private bool _shotFired = false;


    [SerializeField]
    private bool _stopPatternActive = true;
    [SerializeField]
    private bool _chargerActive;
    [SerializeField]
    private bool _randomPlacementActive = true;

    [SerializeField]
    private float _baseSpeed = 40.0f;
    [SerializeField]
    private float _fastSpeed = 60.0f;

    private float newPos;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        if (_randomPlacementActive)
        {
            newPos = Random.Range(-16f, 16f);
        }
        else
        {
            newPos = transform.position.y;
        }

        transform.position = new Vector3(40.0f, newPos, 0);


        _stopTime = Random.Range(0.3f, 0.5f);
        moveActive = true;

        if (_stopPatternActive)
        {
            StartCoroutine(StopMovement());
        }

        if (_chargerActive)
        {
            _speed = _fastSpeed;
        }

        _fireCD = _stopTime + 0.3f;
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        OnScreenCheck();
        Movement();

        if (!_stopPatternActive && !_chargerActive)
        {
            WeaponFire();
        }

    }

    protected override void WeaponFire()
    {
        if (!_stopPatternActive)
        {
            if (Time.time > _fireCD)
            {
                _fireCD = Time.time + _fireRate;

                GameObject _firedShot = Instantiate(_enemyWeapon, _weaponPos.position, Quaternion.identity);
                _firedShot.GetComponent<EnemyShot>()._speed = _shotSpeed;
            }
        }
        else
        {
            GameObject _firedShot = Instantiate(_enemyWeapon, _weaponPos.position, Quaternion.identity);
            _firedShot.GetComponent<EnemyShot>()._speed = _shotSpeed;
        }
    }

    protected override void Movement()
    {
        if (moveActive)
        {
            transform.Translate(Vector3.forward * _speed * Time.deltaTime);

            if (transform.position.x < -70.0f)
            {
                if (_randomPlacementActive)
                {
                    newPos = Random.Range(-16f, 16f);
                }
                else
                {
                    newPos = transform.position.y;
                }

                if (_chargerActive)
                {
                    _speed = _fastSpeed;
                }
                else
                {
                    _speed = _baseSpeed;
                }

                StopAllCoroutines();

                _shotFired = false;

                _fireCD = Time.time + 0.3f;

                if (_stopPatternActive)
                {
                    _stopTime = Random.Range(0.3f, 0.6f);
                    StartCoroutine(StopMovement());
                }

                transform.position = new Vector3(40.0f, newPos, 0);
            }
        }
        else
        {
            StartCoroutine(StartMovement());
        }

        if (_dying)
        {
            Debug.Log("Dying, stopping speed");
            _speed = 0;
        }

    }


    IEnumerator StopMovement()
    {
        yield return new WaitForSeconds(_stopTime);
        moveActive = false;
    }


    IEnumerator StartMovement()
    {
        yield return new WaitForSeconds(0.3f);
        if (!_shotFired && !_chargerActive)
        {
            _shotFired = true;
            WeaponFire();
        }

        yield return new WaitForSeconds(1.0f);

        if (_dying == false)
        {
            _speed = _fastSpeed;
        }
        moveActive = true;

    }
}
