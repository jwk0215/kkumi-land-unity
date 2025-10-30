using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTargetMovement : MonoBehaviour
{
    [SerializeField] private Transform __camera;        // 카메라

    private bool __isMove = false;                      // 드래그 플래그 값
    private Vector3 __lastMousePosition;                // 이전 마우스 위치



    private void Update()
    {
        Movement();
    }
    

    // Camera Target Movement
    private void Movement()
    {
        if (Input.GetMouseButton(0) && Input.GetMouseButton(1))
        {
            if (!__isMove)
            {
                __isMove = true;
                __lastMousePosition = Input.mousePosition;
            }
            else
            {
                Vector3 currentMousePosition = Input.mousePosition;
                Vector3 delta = currentMousePosition - __lastMousePosition;

                Vector3 cameraF = __camera.forward.normalized;
                Vector3 cameraR = __camera.right.normalized;

                cameraF.y = 0;
                cameraR.y = 0;

                transform.position -= (cameraR * delta.x + cameraF * delta.y) * 0.015f;
                __lastMousePosition = currentMousePosition;
            }
        }
        else
        {
            __isMove = false;
        }
    }
}
