using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirectionalLightning : MonoBehaviour
{
    [SerializeField]
    bool toggleCameraBackground = true;
    [SerializeField]
    float lightningRange = 10.0f;
    [SerializeField]
    Color lightningColor = Color.white;
    [SerializeField]
    float timingTime = 5.0f;
    [SerializeField]
    float timingBlinkTime = 0.25f;

    float time;
    float blinkTime;
    bool lightning;
    Light directionalLight;
    Color lightColor;
    float lightRange;
    Color cameraBackgroundColor;

    // Start is called before the first frame update
    void Start()
    {
        time = timingTime;
        blinkTime = timingBlinkTime;
        lightning = false;
        directionalLight = GetComponent<Light>();
        lightColor = directionalLight.color;
        lightRange = directionalLight.range;
        cameraBackgroundColor = Camera.main.backgroundColor;
    }

    // Update is called once per frame
    void Update()
    {
        time -= Time.deltaTime;

        if (time <= 0.0f)
        {
            if (blinkTime == timingBlinkTime)
            {
                ToggleLightning();
            }

            blinkTime -= Time.deltaTime;

            if (blinkTime <= 0.0f)
            {
                ToggleLightning();
                blinkTime = timingBlinkTime;
                time = timingTime;
            }
        }
    }

    void ToggleLightning()
    {
        if (lightning)
        {
            directionalLight.color = lightColor;
            directionalLight.range = lightRange;

            if (toggleCameraBackground)
            {
                Camera.main.backgroundColor = cameraBackgroundColor;
            }

            lightning = false;
        } else
        {
            directionalLight.color = lightningColor;
            directionalLight.range = lightningRange;

            if (toggleCameraBackground)
            {
                Camera.main.backgroundColor = lightningColor;
            }

            lightning = true;
        }
    }
}
