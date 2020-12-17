using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitFlash : MonoBehaviour
{
    private MeshRenderer _enemyColor;
    private bool _damageFlash;
    // Start is called before the first frame update
    void Start()
    {
        _enemyColor = transform.GetComponent<MeshRenderer>();
    }

    public void DamageFlash()
    {
        _enemyColor.material.color = new Color(1, 1, 1, 0f);
        if (_damageFlash == false)
        {
            StartCoroutine(DamageFlashRoutine());
            _damageFlash = true;
        }
    }
    IEnumerator DamageFlashRoutine()
    {
        yield return new WaitForSeconds(.75f);
        _enemyColor.material.color = new Color(1, 1, 1, 1);
        _damageFlash = false;
    }
}
