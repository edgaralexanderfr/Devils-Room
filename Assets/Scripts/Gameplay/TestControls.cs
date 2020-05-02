using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestControls : MonoBehaviour
{
    RaycastHit hit;
    Vector3 cameraLocalPosition;

    // Start is called before the first frame update
    void Start()
    {
        cameraLocalPosition = Camera.main.transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.W))
        {
            transform.Translate(Vector3.forward * Time.deltaTime * 3.0f);
        }

        if (Input.GetKey(KeyCode.S))
        {
            transform.Translate(Vector3.back * Time.deltaTime * 3.0f);
        }

        if (Input.GetKey(KeyCode.A))
        {
            transform.Rotate(0.0f, -3.0f, 0.0f);
        }

        if (Input.GetKey(KeyCode.D))
        {
            transform.Rotate(0.0f, 3.0f, 0.0f);
        }

        if (Physics.Linecast(transform.position, transform.position + transform.localRotation * cameraLocalPosition, out hit))
        {
            Camera.main.transform.localPosition = new Vector3(cameraLocalPosition.x, cameraLocalPosition.y, -Vector3.Distance(transform.position, hit.point));
        }
        else
        {
            Camera.main.transform.localPosition = Vector3.Lerp(Camera.main.transform.localPosition, cameraLocalPosition, Time.deltaTime);
        }
    }
}
