using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarSound : MonoBehaviour
{
    public AudioSource _source;
    public float minPitch, maxPitch, idleVolume, maxVolume;
    public float volumeChangeRateUp = 48.0f;
    public float volumeChangeRateDown = 16.0f;

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
}



