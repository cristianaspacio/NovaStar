using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartBoss : MonoBehaviour
{
    [SerializeField] private BackgroundColorChange _environment;

    // Start is called before the first frame update
    void Start()
    {
        _environment = GameObject.Find("Environment").GetComponent<BackgroundColorChange>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void StartBossActive()
    {
        _environment.FinalBossActive();
        Debug.Log("Change Bg Color");
    }
}
