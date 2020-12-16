using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserSound : MonoBehaviour
{
    [SerializeField] private AudioClip _laserClip;

    // Update is called once per frame
    void Update()
    {
        if (Time.deltaTime >= 1.3f)
        {
            Debug.Log("laserfire");
            AudioManager.Instance.PlayEffect(_laserClip, 1.0f);
        }
    }
}
