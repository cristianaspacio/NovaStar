using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShot : MonoBehaviour
{

    [SerializeField] public float _speed;

    private bool _isEnemyLaser = false;

    [SerializeField] protected GameObject _explosionAnim;

    [SerializeField]
    private AudioClip _sfxSource;

    [SerializeField]
    private AudioClip _hitSfxsource;

    [SerializeField]
    private float _fireVolume = 1.0f;

    [SerializeField]
    private float _hitVolume = 1.0f;

    // Start is called before the first frame update
    void Start()
    {
        AudioManager.Instance.PlayEffect(_sfxSource, _fireVolume);
    }

    // Update is called once per frame
    void Update()
    {
        MoveLeft();

    }

    void MoveLeft()
    {
        transform.Translate(Vector3.left* _speed * Time.deltaTime);
        
        if(transform.position.x < -35.0f)
        {
            if(transform.parent != null)
            {
                Destroy(transform.parent.gameObject);
            }

            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            PlayerHealthAndDamage player = other.GetComponent<PlayerHealthAndDamage>();

            if(player != null)
            {
                player.PlayerDamage();

                if (_explosionAnim != null)
                {

                    GameObject explosion = Instantiate(_explosionAnim, transform.position, Quaternion.identity);
                    explosion.gameObject.GetComponent<ExplosionAnim>()._sfxSource = _hitSfxsource;
                    explosion.gameObject.GetComponent<ExplosionAnim>()._volume = _hitVolume;

                }

                Destroy(gameObject);
            }
        }
    }
}
