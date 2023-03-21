using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ControlsTest : MonoBehaviour
{

    Controls _controls;

    private Vector2 _moveDirection;

    private PlayerInput _inputs;

    private int _tabposition = 0;
    public float _maxSpeed, _nitroSpeed, _normalSpeed;
    public float _currentSpeed;
    private float _currentAcceleration;
    public float _rotationRoll;

    public float _maxRotationSpeed = 1;

    public CinemachineBrain _brain;
    public CinemachineVirtualCamera _firstcam;
    public CinemachineVirtualCamera _POVcam;
    public CinemachineVirtualCamera _thirdcam;
    public CinemachineVirtualCamera _backcam;
    public CinemachineVirtualCamera[] _tabCam;

    public Rigidbody _rb;
    public Animator[] _anim;
    public float test = 1;
    public ParticleSystem[] _FX;
    public GameObject roues;
   

    public float _nitroTimer;
   
    public bool _canSpeed;
    public Image _nitroImage;




    // Start is called before the first frame update
    void Start()
    {
        _normalSpeed = _maxSpeed;
        _canSpeed = true;
        _inputs = GetComponent<PlayerInput>();

        InputAction move = _inputs.actions["Move"];

        move.started += OnMoveStarted;
        move.performed += OnMovePerformed;
        move.canceled += OnMoveCanceled;

        InputAction cameraChange = _inputs.actions["ChangeCamera"];

        cameraChange.started += OnChangeCamera;

    }

    private void OnChangeCamera(InputAction.CallbackContext obj)
    {
        var currentCamera = _brain.ActiveVirtualCamera as CinemachineVirtualCamera;

        if (currentCamera == _thirdcam)
        {
            _thirdcam.Priority = 0;
            _firstcam.Priority = 10;
        }

        if (currentCamera == _firstcam)
        {
            _firstcam.Priority = 0;
            _POVcam.Priority = 10;

        }
        if (currentCamera == _POVcam)
        {
            _POVcam.Priority = 0;
            _thirdcam.Priority = 10;
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
        if (_currentSpeed > 12)
        {
            _canShake = true;
        }
        else
        {
            _canShake = false;
        }

        if (_canShake)
        {
            _thirdcam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = 2.0f;
        }
        else
        {
            _thirdcam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = 0f;
        }

    }
    private void Nitro()
    {
        if (_canSpeed && Input.GetKey(KeyCode.Space))
        {
            StartCoroutine(StartNitro());
        }

    }
    private IEnumerator StartNitro()
    {
        float _lerpDuration = 0.5f;
        float _timer = 0f;
        while (_timer < _lerpDuration)
        {
            _timer += Time.deltaTime;
            _maxSpeed = Mathf.Lerp(_normalSpeed, _nitroSpeed, _timer / _lerpDuration);
            _nitroImage.fillAmount = Mathf.Lerp(1f, 0f, _timer / _lerpDuration);
            _canSpeed = false;
            yield return null;
        }
        yield return new WaitForSeconds(_nitroTimer);
        _timer = 0f;
        while (_timer < _lerpDuration)
        {
            _timer += Time.deltaTime;
            _maxSpeed = Mathf.Lerp(_nitroSpeed, _normalSpeed, _timer / _lerpDuration);
            yield return null;
        } 
        yield return new WaitForSeconds(_nitroTimer);
        _timer = 0f;
        while(_timer < _lerpDuration)
        {
            _timer += Time.deltaTime;
            _nitroImage.fillAmount = Mathf.Lerp(0f, 1f, _timer / _lerpDuration);
            _canSpeed = true;
            yield return null;
        }
        
    }
    



        // Update is called once per frame
    void Update()
        {

            Shake(true);
            Nitro();
            //Lancer l'animation de rotation aux roues en fonction de la vitesse du player
            float _animationSpeed = _currentSpeed / _maxSpeed;
            _anim[0].SetFloat("Speed", _animationSpeed);

            for (int i = 0; i < _anim.Length; i++)
            {
                _anim[i].SetFloat("Speed", _animationSpeed);
            }

            for (int i = 0; i < _FX.Length; i++)
            {
                if (_currentSpeed > 15 && _currentSpeed < 16)
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

            _currentAcceleration = Mathf.Clamp(_currentAcceleration, -1, 1);

            if (_currentAcceleration >= 0)
            {
                _currentSpeed = Mathf.Lerp(0, _maxSpeed, _currentAcceleration);
            }
            else
            {
                _currentSpeed = Mathf.Lerp(0, -_maxSpeed, -_currentAcceleration);
            }

            // Influence accelerations sur la rotation
            rotationAngle = rotationAngle * _currentAcceleration * _maxRotationSpeed * Time.deltaTime;

            transform.Rotate(0, rotationAngle, 0);
            transform.position = transform.position +
                                 transform.forward * (_currentSpeed * Time.deltaTime);

            //Fait tourner le volant et les roues

            //_rotationRoll = Mathf.Clamp(_rotationRoll, 180f, -180f);

            if (rotationAngle != 0)
            {
                _rotationRoll += rotationAngle;
            }
            if (rotationAngle == 0)
            {
                _rotationRoll = 0;
            }
            Vector3 _rotate = new Vector3(0, 0, -_rotationRoll);
            roues.transform.localEulerAngles = _rotate;


        }
}
