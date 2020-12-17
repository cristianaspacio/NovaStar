using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WavePanelManager : MonoBehaviour
{
    [SerializeField] private Text _wave;

    // Start is called before the first frame update
    void Start()
    {
        _wave.gameObject.SetActive(true);
        _wave.text = "Enemies Incoming";
        Invoke("StartSequence", 3);
    }

    // Update is called once per frame
    void Update()
    {
    }

    void StartSequence()
    {
        _wave.gameObject.SetActive(false);
    }
    public void UpdateWave(float wave)
    {
        wave = wave + 1;
        _wave.text = "Wave " + wave.ToString();
        _wave.gameObject.SetActive(true);
    }

    public void WaveIsDone()
    {
        _wave.gameObject.SetActive(false);
    }
}
