using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigSlowEnemy : EnemyAbstractClass
{
    [SerializeField]
    private GameObject[] _weaponPrefabs;

    [SerializeField]
    private bool _lasersOn;

    [SerializeField]
    private float _laserDelay;

    [SerializeField]
    private float _laserFire;

    [SerializeField]
    private bool _fireBreak;

    private Animator _bigSlowAnim;

    private float _randomPos;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        _hp = 10f;
        Instantiate(_powerUpPrefab, transform.position, Quaternion.identity);

        _bigSlowAnim = transform.GetComponent<Animator>();

        if (_bigSlowAnim == null)
        {
            Debug.Log("Animator is NULL");
        }
        
        PlayRandomAnimation();

        _randomPos = Random.Range(-3, 3);

        transform.parent.position = new Vector3(transform.parent.position.x, transform.parent.position.y + _randomPos, transform.parent.position.z);
    }    

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        WeaponFire();
    }

    private void PlayRandomAnimation()
    {
        int random = Random.Range(1, 3);
        _anim.SetInteger("RandomAnim", random );
    }
   

    protected override void WeaponFire()
    {
        if (Time.time > _fireCD && _lasersOn && !_fireBreak)
        {
            _fireCD = Time.time + _fireRate;
            _lasersOn = false;
            WeaponsDelay();
        }

       if (Time.time > _fireCD  && !_lasersOn && !_fireBreak)
        {
            _fireCD = Time.time + _fireRate;
            _lasersOn = true;
            WeaponsDelay();
        }
      
    }

    private void WeaponsDelay()
    {
        if (_lasersOn)
        {
            _lasersOn = false;
            StartCoroutine(FireSecondWeapon());
        }
        else
        {
            _lasersOn = true;
            StartCoroutine(FireFirstWeapon());
        }
    }

    private IEnumerator FireSecondWeapon()
    {

        yield return new WaitForSeconds(10f);
        Instantiate(_weaponPrefabs[1], _weaponPos.position, Quaternion.identity);
        yield return new WaitForSeconds(4f);
        _lasersOn = true;
        _fireBreak = true;
        StartCoroutine(FireBreak());
    }

    private IEnumerator FireFirstWeapon()
    {
        yield return new WaitForSeconds(10f);
        Instantiate(_weaponPrefabs[0], _weaponPos.position, Quaternion.identity);
        yield return new WaitForSeconds(4f);
        _lasersOn = false;
        _fireBreak = true;
        StartCoroutine(FireBreak());
    }

    private IEnumerator FireBreak()
    {
        yield return new WaitForSeconds(5f);
        _fireBreak = false;
    }
}
