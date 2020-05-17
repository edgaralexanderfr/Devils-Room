using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    void UpdateControls()
    {
        if (Input.GetKey(KeyCode.W))
        {
            MoveAlong(0.0f);
        }

        if (Input.GetKey(KeyCode.S))
        {
            MoveAlong(-180.0f);
        }

        if (Input.GetKey(KeyCode.A))
        {
            MoveAlong(270.0f);
        }

        if (Input.GetKey(KeyCode.D))
        {
            MoveAlong(90.0f);
        }
    }

    void UpdateAnimations()
    {
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
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
        float inputMouseX = Input.GetAxis("Mouse X") * cameraXInversion;
        float inputMouseY = Input.GetAxis("Mouse Y") * cameraYInversion;

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
