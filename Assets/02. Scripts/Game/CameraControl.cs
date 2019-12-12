using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour {

    [SerializeField]
    private float cameraSpeedScale = 1.0f;
    [SerializeField]
    private float cameraTrackingSpeed = 0.2f;

    [SerializeField]
    private float highestPosSpan = 0.6f;
    [SerializeField]
    private float lowestPosSpan = -0.3f;
    [SerializeField]
    private float spanExtensionPerFloor;
    private Vector3 pinPosition;

    private Vector3 dragOrigin = Vector3.zero;
    private Vector3 clickOrigin = Vector3.zero;
    private Vector3 basePos;

    private Vector3 lerpEnd;

    private float lerpDistance = 0.0f;
    private bool isDown = true;

    void Start()
    {
        basePos = transform.position;
        pinPosition = transform.position;
    }

    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            if (!isDown)
            {
                clickOrigin = Input.mousePosition;
                basePos = transform.position;
                isDown = true;
            }
            dragOrigin = Input.mousePosition;
        }

        if (!Input.GetMouseButton(0))
        {
            if (isDown)
            {
                isDown = false;
                lerpEnd = new Vector3(transform.position.x
                    , basePos.y + ((clickOrigin.y - dragOrigin.y) * .01f), -10);
            }
            transform.position = Vector3.Lerp(transform.position, lerpEnd, cameraTrackingSpeed);
            clickOrigin = Vector3.zero;
            checkLimitLength();
            return;
        }

        Vector3 movePosition = new Vector3(transform.position.x
            , basePos.y + ((clickOrigin.y - dragOrigin.y) * .01f) * cameraSpeedScale, -10);
        if (clickOrigin.y - dragOrigin.y == 0)
        {
            lerpDistance = 1f;
        }
        else
        {
            lerpDistance = cameraTrackingSpeed;
            transform.position = Vector3.Lerp(transform.position, movePosition, lerpDistance);
        }

        checkLimitLength();
    }

    private void checkLimitLength()
    {
        float highPosition = pinPosition.y + highestPosSpan;
        float lowPosition = pinPosition.y + lowestPosSpan - (spanExtensionPerFloor * (FloorHandler.Instance.floors.Count - 1));
        if (transform.position.y >= highPosition)
        {
            transform.position = new Vector3(transform.position.x, highPosition, transform.position.z);
        }
        else if (transform.position.y <= lowPosition)
        {
            transform.position = new Vector3(transform.position.x, lowPosition, transform.position.z);
        }
    }
}
