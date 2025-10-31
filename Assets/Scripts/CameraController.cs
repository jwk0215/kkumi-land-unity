using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform __target;        // 카메라가 주시할 타겟
    [SerializeField] private Transform __camera;        // 카메라
    [SerializeField] private float __followSpeed;       // 카메라가 타겟을 따라가는 속도
    [SerializeField] private float __sensitivity;       // 감도
    [SerializeField] private float __minDistance;       // 카메라와 타겟 사이의 최소 거리
    [SerializeField] private float __maxDistance;       // 카메라와 타겟 사이의 최대 거리
    [SerializeField] private float __smoothness;        // 부드러움

    private Vector3 __normalizedDirection;              // 카메라 방향
    private Vector3 __finalDirection;                   // 최종 카메라 방향
    private float __applyDistance;                      // 현재 적용되는 거리
    private float __clampAngle = 70f;                   // 제한 각도
    private float __rotateX;                            // x 회전 값
    private float __rotateY;                            // y 회전 값




    private void Awake()
    {
        __normalizedDirection = __camera.localPosition.normalized;
        __applyDistance = __camera.localPosition.magnitude;
    }

    private void Start()
    {
        StartCoroutine(SetTransparentBackground());
    }

    private void Update()
    {
        Movement();
        Rotation();
        Zoom();
    }




    // Camera Movement
    private void Movement()
    {
        transform.position = Vector3.MoveTowards(
            transform.position,
            __target.position,
            __followSpeed * Time.deltaTime
        );

        __finalDirection = transform.TransformPoint(__normalizedDirection * __applyDistance);

        __camera.localPosition = Vector3.Lerp(
            __camera.localPosition,
            __normalizedDirection * __applyDistance,
            Time.deltaTime * __smoothness
        );
    }


    // Camera Rotation
    private void Rotation()
    {
        if (Input.GetMouseButton(0) && !Input.GetMouseButton(1))
        {
            __rotateX += -Input.GetAxisRaw("Mouse Y") * __sensitivity * Time.deltaTime;
            __rotateY += Input.GetAxisRaw("Mouse X") * __sensitivity * Time.deltaTime;
            __rotateX = Mathf.Clamp(__rotateX, -__clampAngle, __clampAngle);

            Quaternion rotation = Quaternion.Euler(__rotateX, __rotateY, 0);
            transform.rotation = rotation;
        }
    }


    // Camera Zoom
    private void Zoom()
    {
        float wheelInput = Input.GetAxis("Mouse ScrollWheel");

        if (wheelInput != 0)
        {
            __applyDistance -= wheelInput * __sensitivity * Time.deltaTime;
            __applyDistance = Mathf.Clamp(__applyDistance, __minDistance, __maxDistance);
        }
    }
    

    // BACKGROUND COLOR SETTING Coroutine
    private IEnumerator SetTransparentBackground()
    {
        yield return null;      // 한 프레임 대기 후 적용

        // 배경 설정
        var mainCam = UnityEngine.Camera.main ?? FindObjectOfType<UnityEngine.Camera>();

        if (mainCam != null)
        {
            mainCam.clearFlags = CameraClearFlags.SolidColor;
            mainCam.backgroundColor = new Color(0, 0, 0, 0);
        }
        else
        {
            Debug.LogWarning("No Camera found for transparent background");
        }
    }
}
