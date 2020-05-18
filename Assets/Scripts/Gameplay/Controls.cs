using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/**
 * TODO: - Organise and refactorise controls and messed stuff...
 *       - Also remember to add mouse controls to the Input System...
 */
public class Controls : MonoBehaviour
{
    // Script settings
    [SerializeField]
    GameObject cameraRotator;
    [SerializeField]
    float cameraSpeed = 10.0f;
    [SerializeField]
    sbyte cameraXInversion = 1;
    [SerializeField]
    sbyte cameraYInversion = 1;
    [SerializeField]
    [Range(0.0f, 360.0f)]
    float cameraMinXRotation = 300.0f;
    [SerializeField]
    [Range(0.0f, 360.0f)]
    float cameraMaxXRotation = 60.0f;
    [SerializeField]
    float rotationSpeed = 12.0f;
    [SerializeField]
    float walkSpeed = 3.0f;

    // Private properties
    Animator _animator;
    bool _playingWalkAnimation = false;
    PlayerControls _playerControls;

    float _playerControlsMoveUp = 0.0f;
    float _playerControlsMoveDown = 0.0f;
    float _playerControlsMoveLeft = 0.0f;
    float _playerControlsMoveRight = 0.0f;

    float _playerControlsLookUp = 0.0f;
    float _playerControlsLookDown = 0.0f;
    float _playerControlsLookLeft = 0.0f;
    float _playerControlsLookRight = 0.0f;

    void Awake()
    {
        _playerControls = new PlayerControls();

        _playerControls.Gameplay.MoveUp.performed += ctx => _playerControlsMoveUp = ctx.ReadValue<float>();
        _playerControls.Gameplay.MoveDown.performed += ctx => _playerControlsMoveDown = ctx.ReadValue<float>();
        _playerControls.Gameplay.MoveLeft.performed += ctx => _playerControlsMoveLeft = ctx.ReadValue<float>();
        _playerControls.Gameplay.MoveRight.performed += ctx => _playerControlsMoveRight = ctx.ReadValue<float>();

        _playerControls.Gameplay.LookUp.performed += ctx => _playerControlsLookUp = ctx.ReadValue<float>();
        _playerControls.Gameplay.LookDown.performed += ctx => _playerControlsLookDown = ctx.ReadValue<float>();
        _playerControls.Gameplay.LookLeft.performed += ctx => _playerControlsLookLeft = ctx.ReadValue<float>();
        _playerControls.Gameplay.LookRight.performed += ctx => _playerControlsLookRight = ctx.ReadValue<float>();

        _playerControls.Gameplay.MoveUp.canceled += ctx => _playerControlsMoveUp = 0.0f;
        _playerControls.Gameplay.MoveDown.canceled += ctx => _playerControlsMoveDown = 0.0f;
        _playerControls.Gameplay.MoveLeft.canceled += ctx => _playerControlsMoveLeft = 0.0f;
        _playerControls.Gameplay.MoveRight.canceled += ctx => _playerControlsMoveRight = 0.0f;

        _playerControls.Gameplay.LookUp.canceled += ctx => _playerControlsLookUp = 0.0f;
        _playerControls.Gameplay.LookDown.canceled += ctx => _playerControlsLookDown = 0.0f;
        _playerControls.Gameplay.LookLeft.canceled += ctx => _playerControlsLookLeft = 0.0f;
        _playerControls.Gameplay.LookRight.canceled += ctx => _playerControlsLookRight = 0.0f;
    }

    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateControls();
        UpdateAnimations();
        UpdateCameraRotation();
    }

    void LateUpdate()
    {
        UpdateRotatorPosition();
    }

    void OnEnable()
    {
        _playerControls.Gameplay.Enable();
    }

    void OnDisabled()
    {
        _playerControls.Gameplay.Disable();
    }

    void UpdateControls()
    {
        if (_playerControlsMoveUp > 0.5f)
        {
            MoveAlong(0.0f);
        }

        if (_playerControlsMoveDown > 0.5f)
        {
            MoveAlong(-180.0f);
        }

        if (_playerControlsMoveLeft > 0.5f)
        {
            MoveAlong(270.0f);
        }

        if (_playerControlsMoveRight > 0.5f)
        {
            MoveAlong(90.0f);
        }
    }

    void UpdateAnimations()
    {
        if (_playerControlsMoveUp > 0.5f || _playerControlsMoveDown > 0.5f || _playerControlsMoveLeft > 0.5f || _playerControlsMoveRight > 0.5f)
        {
            if (!_playingWalkAnimation)
            {
                _animator.Play("Walk");
                _playingWalkAnimation = true;
            }
        }
        else
        {
            if (_playingWalkAnimation)
            {
                _animator.Play("Idle");
                _playingWalkAnimation = false;
            }
        }
    }

    void MoveAlong(float angle)
    {
        var rotation = Quaternion.Euler(
            transform.eulerAngles.x,
            cameraRotator.transform.eulerAngles.y + angle,
            transform.eulerAngles.z
        );

        transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * rotationSpeed);
        transform.Translate(Vector3.forward * Time.deltaTime * walkSpeed);
    }

    void UpdateCameraRotation()
    {
        float inputMouseX = (_playerControlsLookLeft - _playerControlsLookRight) * cameraXInversion;
        float inputMouseY = (_playerControlsLookUp - _playerControlsLookDown) * cameraYInversion;

        if (inputMouseX != 0.0f)
        {
            cameraRotator.transform.Rotate(0.0f, inputMouseX * cameraSpeed, 0.0f, Space.World);
        }

        float rotationX = cameraRotator.transform.eulerAngles.x + (inputMouseY * cameraSpeed);

        if (rotationX < 0.0f)
        {
            rotationX += 360.0f;
        }
        else
        {
            if (rotationX > 360.0f)
            {
                rotationX -= 360.0f;
            }
        }

        if ((rotationX >= 0.0f && rotationX <= cameraMaxXRotation) || (rotationX >= cameraMinXRotation && rotationX <= 360.0f))
        {
            cameraRotator.transform.rotation = Quaternion.Euler(
                rotationX,
                cameraRotator.transform.eulerAngles.y,
                0.0f
            );
        }
    }

    void UpdateRotatorPosition()
    {
        cameraRotator.transform.position = transform.position;
    }
}
