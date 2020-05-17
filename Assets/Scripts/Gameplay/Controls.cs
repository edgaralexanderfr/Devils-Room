using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controls : MonoBehaviour
{
    [SerializeField]
    GameObject cameraRotator;
    [SerializeField]
    float cameraSpeed = 20.0f;
    [SerializeField]
    [Range(0.0f, 360.0f)]
    float cameraMinXRotation = 300.0f;
    [SerializeField]
    [Range(0.0f, 360.0f)]
    float cameraMaxXRotation = 60.0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        UpdateCameraRotation();
    }

    void LateUpdate()
    {
        UpdateRotatorPosition();
    }

    void UpdateCameraRotation()
    {
        float inputMouseX = Input.GetAxis("Mouse X");
        float inputMouseY = Input.GetAxis("Mouse Y");

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
