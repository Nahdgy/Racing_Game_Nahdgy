using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarSound : MonoBehaviour
{ 
    [SerializeField]
    private  AudioSource _source, _honk, _soukou;
    [SerializeField] 
    private AudioClip _soukouSound;
    public float minPitch, maxPitch, idleVolume, maxVolume;
    public float volumeChangeRateUp = 48.0f;
    public float volumeChangeRateDown = 16.0f;
    [SerializeField]
    private KeyCode _key;

public void PrecessContinuousAudioPitch(float ratio)
{
    _source.pitch = Mathf.Lerp(minPitch, maxPitch, ratio);
    if (!_source.isPlaying && _source.isActiveAndEnabled)
    {
        _source.Play();
    }
    _source.loop = true;
    ProcessVolume(ratio, volumeChangeRateUp, volumeChangeRateDown);
}
public void ProcessVolume(float ratio, float changeRateUp, float changeRateDown)
    {
        float volume = Mathf.Lerp(idleVolume, maxVolume, ratio);
        float changeRate = volume > _source.volume ? changeRateUp : changeRateDown;
        _source.volume = Mathf.Lerp(_source.volume, volume, Time.deltaTime * changeRate);
    }
private void Honk()
    {
        if(Input.GetKey(_key))
        {
            _honk.Play();

        }
    }
private void Rupt()
    {
        if(Input.GetKey(KeyCode.Z)&& Input.GetKey(KeyCode.S))
        {
        
            _soukou.PlayOneShot(_soukouSound, Time.deltaTime);
        }
    }
    private void Update()
    {
        Rupt();
        Honk();
    }
}



