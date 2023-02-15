using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class ControlsTest : MonoBehaviour
{

    Controls _controls;

    private Vector2 _moveDirection;

    private PlayerInput _inputs;

    public float _maxSpeed;
    private float _currentSpeed;
    private float _currentAcceleration;
    public float _rotationRoll;

    public float _maxRotationSpeed=1;

    public CinemachineBrain _brain;
    public CinemachineVirtualCamera _camera1;
    public CinemachineVirtualCamera _camera2; 
    public CinemachineVirtualCamera _vcam;

    public Rigidbody _rb;
    public Animator[] _anim;
    public float test = 1;
    public ParticleSystem[] _FX;
    public GameObject roues;
   



    // Start is called before the first frame update
    void Start()
    {
        _inputs = GetComponent<PlayerInput>();

        InputAction move = _inputs.actions["Move"];

        move.started += OnMoveStarted;
        move.performed += OnMovePerformed;
        move.canceled += OnMoveCanceled;
        
        InputAction cameraChange = _inputs.actions["ChangeCamera"];

        cameraChange.performed += OnChangeCamera;

    }

    private void OnChangeCamera(InputAction.CallbackContext obj)
    {
        var currentCamera = _brain.ActiveVirtualCamera as CinemachineVirtualCamera;

        if (currentCamera == _camera1)
        {
            _camera1.Priority = 0;
            _camera2.Priority = 10;
        }
        else
        {
            _camera1.Priority = 10;
            _camera2.Priority = 0;
        }
    }

    private void OnMoveCanceled(InputAction.CallbackContext obj)
    {
        _moveDirection = Vector2.zero;
    }

    public void OnMovePerformed(InputAction.CallbackContext context)
    {
        Debug.Log($"Move performed : {context.ReadValue<Vector2>()}");
        
        _moveDirection = context.ReadValue<Vector2>();
    }

    private void OnMoveStarted(InputAction.CallbackContext context)
    {
        Debug.Log($"Move started : {context.ReadValue<Vector2>()}");
        
        _moveDirection = context.ReadValue<Vector2>();
    }

    private void Shake(bool _canShake)
    {
        if(_currentSpeed > 12)
        {
            _canShake = true;
        }
        else 
        { 
            _canShake = false; 
        }

        if(_canShake)
        {
            _vcam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = 2.0f;
        }
        else
        {
            _vcam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = 0f;
        }

    }

    // Update is called once per frame
    void Update()
    {
        Shake(true);
        //Lancer l'animation de rotation aux roues en fonction de la vitesse du player
        float _animationSpeed = _currentSpeed / _maxSpeed;
        _anim[0].SetFloat("Speed", _animationSpeed);

        for (int i = 0; i < _anim.Length; i++)
        {
            _anim[i].SetFloat("Speed", _animationSpeed);
        }

        for (int i = 0; i < _FX.Length; i++)
        {
            if(_currentSpeed > 15 && _currentSpeed < 16)
            {
                _FX[i].Play(); 
            }
        }
      

        // Récupère les données de mouvement
        float rotationAngle = _moveDirection.x;
        float acceleration = _moveDirection.y;

        // Si on accélère pas (laché)
        if (acceleration == 0)
        {
            _currentAcceleration = Mathf.Lerp(_currentAcceleration, 0, Time.deltaTime);
        }
        else if (acceleration < 0)
        {
            //Si on est en train d'avancer, on freine
            if (_currentAcceleration > 0)
            {
                _currentAcceleration -= Time.deltaTime;
            }
            else // On recule
            {
                _currentAcceleration += acceleration * Time.deltaTime;
            }
        }
        else
        {
            // On accélère progressivement
            _currentAcceleration += acceleration * Time.deltaTime;
        }

        _currentAcceleration = Mathf.Clamp(_currentAcceleration,-1,1);

        if (_currentAcceleration >= 0)
        {
            _currentSpeed = Mathf.Lerp(0, _maxSpeed, _currentAcceleration);
        }
        else
        {
            _currentSpeed = Mathf.Lerp(0, -_maxSpeed, -_currentAcceleration);
        }
        
        // Influence accelerations sur la rotation
        rotationAngle = rotationAngle * _currentAcceleration*_maxRotationSpeed*Time.deltaTime;
        
        transform.Rotate(0,rotationAngle,0);
        transform.position = transform.position +
                             transform.forward * (_currentSpeed * Time.deltaTime);

        //Fait tourner le volant et les roues
       
        //_rotationRoll = Mathf.Clamp(_rotationRoll, 180f, -180f);

        if (rotationAngle != 0)
        {
            _rotationRoll+= rotationAngle;
        }
       if (rotationAngle == 0)
        {
           _rotationRoll = 0;
        }
        Vector3 _rotate = new Vector3(0,0,-_rotationRoll);
        roues.transform.localEulerAngles = _rotate;
        
        
    }
}
