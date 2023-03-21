using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Speedometer : MonoBehaviour
{
    [SerializeField]
    private GameObject _needle;
    [SerializeField]
    private float _startPosition = 220.0f, _endPosition = -46.0f, _desiredPosition;
    [SerializeField]
    private ControlsTest _speed;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        NeedleUptade();
        
    }
    
    void NeedleUptade()
    {
        _desiredPosition = _startPosition - _endPosition;
        float tremp = _speed._currentSpeed / 180;
        _needle.transform.eulerAngles = new Vector3(0,0, (_startPosition - tremp * _desiredPosition));
    }
}
