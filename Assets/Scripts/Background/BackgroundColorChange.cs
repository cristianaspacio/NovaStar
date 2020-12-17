using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundColorChange : MonoBehaviour
{
    private Animator _anim;
    [SerializeField] private bool _finalBossActive;

    // Start is called before the first frame update
    void Start()
    {
        _anim = GetComponent<Animator>();

        if (_anim == null)
        {
            Debug.LogError("No Anim");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (_finalBossActive == true)
        {
            _anim.SetBool("FinalBoss", true);
        }
        else
        {
            _anim.SetBool("FinalBoss", false);
        }
    }
    public void FinalBossActive()
    {
        _finalBossActive = true;
    }
    public void FinalBossNotActive()
    {
        _finalBossActive = false;
    }
}
