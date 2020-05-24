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
    GameObject neck;
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
    [SerializeField]
    [Range(0.0f, 360.0f)]
    float neckMaxXRotation = 42.0f;
    [SerializeField]
    [Range(0.0f, 360.0f)]
    float neckMaxYRotation = 70.0f;
    [SerializeField]
    [Range(0.0f, 360.0f)]
    float neckResetYAngle = 140.0f;

    struct PlayerControlsValues {
        public float MoveUp;
        public float MoveDown;
        public float MoveLeft;
        public float MoveRight;

        public float LookUp;
        public float LookDown;
        public float LookLeft;
        public float LookRight;

        public PlayerControlsValues(
            float moveUp = 0.0f,
            float moveDown = 0.0f,
            float moveLeft = 0.0f,
            float moveRight = 0.0f,

            float lookUp = 0.0f,
            float lookDown = 0.0f,
            float lookLeft = 0.0f,
            float lookRight = 0.0f
        )
        {
            MoveUp = moveUp;
            MoveDown = moveDown;
            MoveLeft = moveLeft;
            MoveRight = moveRight;

            LookUp = lookUp;
            LookDown = lookDown;
            LookLeft = lookLeft;
            LookRight = lookRight;
        }
    }

    // Private properties
    Animator _animator;
    bool _playingWalkAnimation = false;
    PlayerControls _playerControls;
    PlayerControlsValues _playerControlsValues;

    void Awake()
    {
        _playerControls = new PlayerControls();
        _playerControlsValues = new PlayerControlsValues();

        _playerControls.Gameplay.MoveUp.performed += ctx => _playerControlsValues.MoveUp = ctx.ReadValue<float>();
        _playerControls.Gameplay.MoveDown.performed += ctx => _playerControlsValues.MoveDown = ctx.ReadValue<float>();
        _playerControls.Gameplay.MoveLeft.performed += ctx => _playerControlsValues.MoveLeft = ctx.ReadValue<float>();
        _playerControls.Gameplay.MoveRight.performed += ctx => _playerControlsValues.MoveRight = ctx.ReadValue<float>();

        _playerControls.Gameplay.LookUp.performed += ctx => _playerControlsValues.LookUp = ctx.ReadValue<float>();
        _playerControls.Gameplay.LookDown.performed += ctx => _playerControlsValues.LookDown = ctx.ReadValue<float>();
        _playerControls.Gameplay.LookLeft.performed += ctx => _playerControlsValues.LookLeft = ctx.ReadValue<float>();
        _playerControls.Gameplay.LookRight.performed += ctx => _playerControlsValues.LookRight = ctx.ReadValue<float>();

        _playerControls.Gameplay.MoveUp.canceled += ctx => _playerControlsValues.MoveUp = 0.0f;
        _playerControls.Gameplay.MoveDown.canceled += ctx => _playerControlsValues.MoveDown = 0.0f;
        _playerControls.Gameplay.MoveLeft.canceled += ctx => _playerControlsValues.MoveLeft = 0.0f;
        _playerControls.Gameplay.MoveRight.canceled += ctx => _playerControlsValues.MoveRight = 0.0f;

        _playerControls.Gameplay.LookUp.canceled += ctx => _playerControlsValues.LookUp = 0.0f;
        _playerControls.Gameplay.LookDown.canceled += ctx => _playerControlsValues.LookDown = 0.0f;
        _playerControls.Gameplay.LookLeft.canceled += ctx => _playerControlsValues.LookLeft = 0.0f;
        _playerControls.Gameplay.LookRight.canceled += ctx => _playerControlsValues.LookRight = 0.0f;
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
        UpdateNeckRotation();
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
        if (_playerControlsValues.MoveUp > 0.5f)
        {
            MoveAlong(0.0f);
        }

        if (_playerControlsValues.MoveDown > 0.5f)
        {
            MoveAlong(-180.0f);
        }

        if (_playerControlsValues.MoveLeft > 0.5f)
        {
            MoveAlong(270.0f);
        }

        if (_playerControlsValues.MoveRight > 0.5f)
        {
            MoveAlong(90.0f);
        }
    }

    void UpdateAnimations()
    {
        if (_playerControlsValues.MoveUp > 0.5f || _playerControlsValues.MoveDown > 0.5f || _playerControlsValues.MoveLeft > 0.5f || _playerControlsValues.MoveRight > 0.5f)
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
        float inputMouseX = (_playerControlsValues.LookLeft - _playerControlsValues.LookRight) * cameraXInversion * -1;
        float inputMouseY = (_playerControlsValues.LookUp - _playerControlsValues.LookDown) * cameraYInversion * -1;

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

    void UpdateNeckRotation()
    {
        float angleXDiff = Mathf.DeltaAngle(cameraRotator.transform.eulerAngles.x, transform.eulerAngles.x);
        float angleYDiff = Mathf.DeltaAngle(cameraRotator.transform.eulerAngles.y, transform.eulerAngles.y);
        float angleXDiffAbs = Mathf.Abs(angleXDiff);
        float angleYDiffAbs = Mathf.Abs(angleYDiff);
        float angleX, angleY;

        if (angleXDiffAbs <= neckMaxXRotation)
        {
            angleX = cameraRotator.transform.eulerAngles.x;
        }
        else
        {
            if (angleXDiff < 0.0f)
            {
                angleX = transform.eulerAngles.x + neckMaxXRotation;
            }
            else
            {
                angleX = transform.eulerAngles.x - neckMaxXRotation;
            }
        }

        if (angleYDiffAbs <= neckMaxYRotation)
        {
            angleY = cameraRotator.transform.eulerAngles.y;
        }
        else
        {
            if (angleYDiffAbs >= neckResetYAngle)
            {
                angleY = transform.eulerAngles.y;
            }
            else
            {
                if (angleYDiff < 0.0f)
                {
                    angleY = transform.eulerAngles.y + neckMaxYRotation;
                }
                else
                {
                    angleY = transform.eulerAngles.y - neckMaxYRotation;
                }
            }
        }

        neck.transform.rotation = Quaternion.Euler(
            angleX,
            angleY,
            neck.transform.eulerAngles.z
        );
    }
}
