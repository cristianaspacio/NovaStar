using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeaponsFire : MonoBehaviour
{

    [SerializeField]
    public GameObject[] _weaponsPrefab;

    [SerializeField]
    private GameObject _chargeSprite;

    [SerializeField]
    private float _fireRate = 0.25f;

    [SerializeField]
    private float _willFire;

    [SerializeField]
    private string[] _weaponLevelString;

    private int _totalWepCount;

    public int _weaponPowerLevel = 0;

    private GameObject newCharge;

    private GameObject newLaser;

    private GameObject laserSprite;

    private Collider _targetCollider = null;

    private float _totalCharge;

    private bool _canShoot = true;
    
    private Animator _laserAnim;

    private Weapons_Display_UI _weaponUI;

    private float _powerUpPointVal = 200.0f;

    private enum CurrentWeapon : int
    {
        FirstWeapon,
        SecondWeapon,
        ThirdWeapon,
        FourthWeapon,
        FifthWeapon,
        SixthWeapon
    }

    [SerializeField]
    private CurrentWeapon _weaponCurrentlyOn;


    // Start is called before the first frame update
    void Start()
    {
        transform.position = new Vector3(0, 0, 0);
        _weaponPowerLevel = 0;
        _totalWepCount = _weaponsPrefab.Length - 1;

        _weaponUI = GameObject.Find("Canvas").GetComponent<Weapons_Display_UI>();

        if (_weaponUI == null)
        {
            Debug.Log("Weapon Display is NULL");
        }

        UpdateWeaponLevel();

    }

    // Update is called once per frame
    void Update()
    { 

        if (Input.GetKey(KeyCode.Space) && Time.time > _willFire && _canShoot)
        {
            FireShot();
        }

    }

    public void FireShot()
    {
        _willFire = Time.time + _fireRate;

        switch (_weaponCurrentlyOn)
        {

            case CurrentWeapon.FirstWeapon:
                Instantiate(_weaponsPrefab[0], transform.position + new Vector3(5, 0, 0), Quaternion.identity);
                break;

            case CurrentWeapon.SecondWeapon:
                Instantiate(_weaponsPrefab[1], transform.position + new Vector3(5, 0, 0), Quaternion.identity);
                break;

            case CurrentWeapon.ThirdWeapon:
                Instantiate(_weaponsPrefab[2], transform.position + new Vector3(5, 0, 0), Quaternion.identity);
                break;

            case CurrentWeapon.FourthWeapon:
                Instantiate(_weaponsPrefab[3], transform.position + new Vector3(5, 0, 0), Quaternion.identity);
                break;

            case CurrentWeapon.FifthWeapon:
                Instantiate(_weaponsPrefab[4], transform.position + new Vector3(5, 0, 0), Quaternion.identity);
                break;

            case CurrentWeapon.SixthWeapon:
                _canShoot = false;           
                newCharge = Instantiate(_chargeSprite, transform);
                newCharge.GetComponent<LaserCharge>()._playerObj = gameObject;
                break;

            default:
                Debug.Log("Invalid ID!");
                break;
        }

    }

    public void FireLaser(float chargeTime)
    {

        Destroy(newCharge);

        _totalCharge = chargeTime;

        newLaser = Instantiate(_weaponsPrefab[5], transform);

        laserSprite = newLaser.transform.GetChild(0).gameObject;

        _laserAnim = laserSprite.GetComponent<Animator>();

        _laserAnim.SetTrigger("laserStart");

        if (_laserAnim.GetBool("laserEnd") == true)
        {
            _laserAnim.ResetTrigger("laserEnd");
        }

        StartCoroutine(LaserTimer());
    }

    public void ChargeCancel()
    {
        _canShoot = true;
    }
   


    private void OnTriggerEnter(Collider other)
    {

        if (other.tag == "PowerUp")
        {
            Destroy(other.gameObject);
            transform.GetComponent<PlayerScore>().AddScore(_powerUpPointVal);
            PlayerHealthAndDamage playerHealth = GameObject.Find("Player").GetComponent<PlayerHealthAndDamage>();
            if (_weaponPowerLevel < _totalWepCount)
            {
                _weaponPowerLevel++;
                UpdateWeaponLevel();
            }
            if (playerHealth.health < playerHealth.maximumHealth)
            {
                playerHealth.health++;
            }
            SwitchPowerUp();
            
        }
    }

    public void UpdateWeaponLevel()
    {
        _weaponUI.UpdateWeaponLevel(_weaponLevelString[_weaponPowerLevel]);
        SwitchPowerUp();
    }

    IEnumerator LaserTimer()
    {
        float fireTime = _totalCharge;

        while (fireTime > 0)
        {
            fireTime -= Time.deltaTime;
            yield return null;
        }

        _laserAnim.SetTrigger("laserEnd");

        if (_laserAnim.GetBool("laserStart") == true)
        {
            _laserAnim.ResetTrigger("laserStart");
        }

        if (_laserAnim.GetCurrentAnimatorStateInfo(0).normalizedTime > 1)
        {
            Debug.Log("Anim End");

            //newLaser.GetComponent<Collider>().enabled = false;

            Destroy(newLaser, 0.3f);
        }

        _canShoot = true;
    }

    public void SwitchPowerUp()
    {
        switch (_weaponPowerLevel)
        {

            case 0:
                _weaponCurrentlyOn = CurrentWeapon.FirstWeapon;
                _weaponLevelString[0] = "LEVEL 1: FIREBALL";
                break;

            case 1:
                _weaponCurrentlyOn = CurrentWeapon.SecondWeapon;
                _weaponLevelString[1] = "LEVEL 2: DOUBLE FIREBALL";
                break;

            case 2:
                _weaponCurrentlyOn = CurrentWeapon.ThirdWeapon;
                _weaponLevelString[2] = "LEVEL 3: TRIPLE FIREBALL";
                break;

            case 3:
                _weaponCurrentlyOn = CurrentWeapon.FourthWeapon;
                _weaponLevelString[3] = "LEVEL 4: WAVE";
                break;

            case 4:
                _weaponCurrentlyOn = CurrentWeapon.FifthWeapon;
                _weaponLevelString[4] = "LEVEL 5: BIG FIREBALL";
                break;

            case 5:
                _weaponCurrentlyOn = CurrentWeapon.SixthWeapon;
                _weaponLevelString[5] = "LEVEL 6: LASER";
                break;

            default:
                Debug.Log("Invalid ID!");
                break;
        }


    }
}
