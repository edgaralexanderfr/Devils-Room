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
    GameObject playerCamera;
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
    float cameraZSpeed = 1.5f;
    [SerializeField]
    float rotationSpeed = 12.0f;
    [SerializeField]
    float walkSpeed = 3.0f;
    [SerializeField]
    float runSpeed = 6.0f;
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
    Rigidbody _rigidbody;
    bool _playingWalkAnimation = false;
    bool _playingRunAnimation = false;
    PlayerControls _playerControls;
    PlayerControlsValues _playerControlsValues;
    Vector3 _playerCameraInitialLocalPosition;
    RaycastHit _hit;
    bool _running = false;

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

        _playerControls.Gameplay.Run.performed += ctx => _running = !_running;

        _playerControls.Gameplay.LookX.performed += ctx => {
            float value = Mathf.Clamp(ctx.ReadValue<float>() / 10.0f, -1.0f, 1.0f);

            if (value >= 0.0f)
            {
                _playerControlsValues.LookRight = value;
            }
            else
            {
                _playerControlsValues.LookLeft = Mathf.Abs(value);
            }
        };

        _playerControls.Gameplay.LookY.performed += ctx => {
            float value = Mathf.Clamp(ctx.ReadValue<float>() / 10.0f, -1.0f, 1.0f);

            if (value >= 0.0f)
            {
                _playerControlsValues.LookUp = value;
            }
            else
            {
                _playerControlsValues.LookDown = Mathf.Abs(value);
            }
        };

        _playerControls.Gameplay.MoveUp.canceled += ctx => _playerControlsValues.MoveUp = 0.0f;
        _playerControls.Gameplay.MoveDown.canceled += ctx => _playerControlsValues.MoveDown = 0.0f;
        _playerControls.Gameplay.MoveLeft.canceled += ctx => _playerControlsValues.MoveLeft = 0.0f;
        _playerControls.Gameplay.MoveRight.canceled += ctx => _playerControlsValues.MoveRight = 0.0f;

        _playerControls.Gameplay.LookUp.canceled += ctx => _playerControlsValues.LookUp = 0.0f;
        _playerControls.Gameplay.LookDown.canceled += ctx => _playerControlsValues.LookDown = 0.0f;
        _playerControls.Gameplay.LookLeft.canceled += ctx => _playerControlsValues.LookLeft = 0.0f;
        _playerControls.Gameplay.LookRight.canceled += ctx => _playerControlsValues.LookRight = 0.0f;

        _playerControls.Gameplay.LookX.canceled += ctx => _playerControlsValues.LookRight = _playerControlsValues.LookLeft = 0.0f;
        _playerControls.Gameplay.LookY.canceled += ctx => _playerControlsValues.LookUp = _playerControlsValues.LookDown = 0.0f;
    }

    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponent<Animator>();
        _rigidbody = GetComponent<Rigidbody>();
        _playerCameraInitialLocalPosition = playerCamera.transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateAnimations();
        UpdateCameraRotation();
        UpdateCameraPosition();
    }

    void LateUpdate()
    {
        UpdateRotatorPosition();
        UpdateNeckRotation();
    }

    void FixedUpdate()
    {
        UpdateControls();
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
        if (_playerControlsValues.MoveUp > 0.0f || _playerControlsValues.MoveDown > 0.0f || _playerControlsValues.MoveLeft > 0.0f || _playerControlsValues.MoveRight > 0.0f)
        {
            MoveAlong();
        }

        if (_playerControlsValues.MoveUp == 0.0f
            && _playerControlsValues.MoveDown == 0.0f
            && _playerControlsValues.MoveLeft == 0.0f
            && _playerControlsValues.MoveRight == 0.0f
        )
        {
            _running = false;
        }
    }

    void UpdateAnimations()
    {
        float horizontalMove = _playerControlsValues.MoveRight - _playerControlsValues.MoveLeft;
        float verticalMove = _playerControlsValues.MoveUp - _playerControlsValues.MoveDown;

        if (horizontalMove != 0.0f || verticalMove != 0.0f
            && (_playerControlsValues.MoveUp > 0.0f || _playerControlsValues.MoveDown > 0.0f || _playerControlsValues.MoveLeft > 0.0f || _playerControlsValues.MoveRight > 0.0f)
        )
        {
            if (_running)
            {
                if (!_playingRunAnimation)
                {
                    _animator.speed = 1.0f;
                    _animator.Play("Run");
                    _playingWalkAnimation = false;
                    _playingRunAnimation = true;
                }
            }
            else
            {
                _animator.speed = Mathf.Max(Mathf.Max(Mathf.Max(_playerControlsValues.MoveUp, _playerControlsValues.MoveDown), _playerControlsValues.MoveLeft), _playerControlsValues.MoveRight);

                if (!_playingWalkAnimation)
                {
                    _animator.Play("Walk");
                    _playingWalkAnimation = true;
                    _playingRunAnimation = false;
                }
            }
        }
        else
        {
            if (_playingWalkAnimation || _playingRunAnimation)
            {
                _animator.speed = 1.0f;
                _animator.Play("Idle");
                _playingWalkAnimation = false;
                _playingRunAnimation = false;
            }
        }
    }

    void MoveAlong()
    {
        float x = _playerControlsValues.MoveRight - _playerControlsValues.MoveLeft;
        float y = _playerControlsValues.MoveUp - _playerControlsValues.MoveDown;
        float angle = Utils.Mathf.Vector2Angle(x, y);

        var rotation = Quaternion.Euler(
            transform.eulerAngles.x,
            cameraRotator.transform.eulerAngles.y + angle,
            transform.eulerAngles.z
        );

        float localSpeed = (_running && (x != 0.0f || y != 0.0f)) ? 1.0f : Mathf.Max(Mathf.Abs(x), Mathf.Abs(y)) ;

        transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.fixedDeltaTime * rotationSpeed);
        _rigidbody.MovePosition(transform.position + (transform.forward * Time.fixedDeltaTime * localSpeed * (_running ? runSpeed : walkSpeed)));
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

    void UpdateCameraPosition()
    {
        bool collided = Physics.Linecast(
            cameraRotator.transform.position,
            playerCamera.transform.position,
            out _hit,
            ~LayerMask.GetMask("Player")
        );

        if (collided)
        {
            playerCamera.transform.localPosition = new Vector3(
                _playerCameraInitialLocalPosition.x,
                _playerCameraInitialLocalPosition.y,
                -Vector3.Distance(cameraRotator.transform.position, _hit.point)
            );
        }
        else
        {
            playerCamera.transform.localPosition = Vector3.Lerp(
                playerCamera.transform.localPosition,
                _playerCameraInitialLocalPosition,
                Time.deltaTime * cameraZSpeed
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
