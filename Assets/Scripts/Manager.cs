using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class Manager : MonoBehaviour
{
    [DllImport("__Internal")]
    private static extern void onUnityLoaded();     // web 환경에서 로드 완료 시 실행할 함수

    // PREFAB FIELD
    [SerializeField]
    private GameObject[] __prefabs;                 // prefab array
    private GameObject __selectObject;              // 현재 선택된 prefab
    private Renderer __selectObjectRenderer;        // 현재 선택된 prefab renderer
    private Color __selectObjectColor;              // 현재 선택된 prefab color

    // CONTROL FIELD
    [SerializeField]
    private UnityEngine.Camera __camera;            // main camera
    [SerializeField]
    private GameObject __cameraObject;              // camera object
    private MonoBehaviour __cameraController;       // camera controller.cs

    [SerializeField]
    private GameObject __gridObject;                // grid object
    private GridController __gridController;         // grid controller.cs

    [SerializeField]
    private float __cellSize = 1f;                  // 이동할 단위 (Grid와 동일하게 맞춰야함)
    private bool __isDrag = false;                  // drag flag값
    private Vector3 __dragOffset;                   // 이동 제어용 벡터




    private void Start()
    {
        __cameraController = __cameraObject.GetComponent<CameraController>();
        __gridController = __gridObject.GetComponent<GridController>();
        onUnityLoaded();
    }


    private void Update()
    {
        SelectObject();
        Move();
        MoveOff();
    }




    // ===============================================================================
    // PUBLIC (JS 환경에서 실행용)
    // ===============================================================================

    // Prefab load
    public void LoadPrefab(string prefabName)
    {
        foreach (var prefab in __prefabs)
        {
            if (prefab.name == prefabName)
            {
                Instantiate(prefab, Vector3.zero, Quaternion.identity);
                return;
            }
        }

        Debug.LogWarning($"❌ Prefab '{prefabName}' not found in PrefabManager.");
    }


    // 선택 해제
    public void UnselectObject()
    {
        // 선택한 오브젝트 투명화 제거
        if (__selectObjectRenderer != null) __selectObjectRenderer.material.color = __selectObjectColor;
        __selectObject = null;
        __selectObjectRenderer = null;
    }


    // 선택한 오브젝트 삭제
    public void DeleteSelectObject()
    {
        if (__selectObject != null)
        {
            Destroy(__selectObject);
            __selectObject = null;
            __selectObjectRenderer = null;
        }
    }


    // Grid On/Off
    public void GridOnOff(string msg)
    {
        __gridController.GridOnOff(msg == "on");
    }




    // ===============================================================================
    // PRIVATE
    // ===============================================================================

    // 클릭 시 해당 오브젝트 선택
    private void SelectObject()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = __camera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                // 카메라 이동 막기
                if (__cameraController != null) __cameraController.enabled = false;

                __selectObject = hit.transform.gameObject;
                __selectObjectRenderer = __selectObject.GetComponent<Renderer>();
                __isDrag = true;
                __dragOffset = __selectObject.transform.position - hit.point;

                // 해당 오브젝트 투명하게
                if (__selectObjectRenderer != null)
                {
                    __selectObjectColor = __selectObjectRenderer.material.color;

                    Color originalColor = __selectObjectColor;
                    originalColor.a = 1f;
                    __selectObjectColor = originalColor;

                    Color aColor = __selectObjectColor;
                    aColor.a = 0.5f;
                    __selectObjectRenderer.material.color = aColor;

                    SetMaterialTransparent(__selectObjectRenderer.material);
                }
            }
            // 선택 해제
            else
            {
                // 선택한 오브젝트 투명화 제거
                if (__selectObjectRenderer != null) __selectObjectRenderer.material.color = __selectObjectColor;
                __selectObject = null;
                __selectObjectRenderer = null;
            }
        }
    }
    

    // 오브젝트 이동
    private void Move()
    {
        if (__selectObject != null && __isDrag)
        {
            Ray ray = __camera.ScreenPointToRay(Input.mousePosition);
            Plane groundPlane = new Plane(Vector3.up, Vector3.zero);

            if (groundPlane.Raycast(ray, out float distance))
            {
                Vector3 worldPoint = ray.GetPoint(distance) + __dragOffset;

                float snappedX = Mathf.Round(worldPoint.x / __cellSize) * __cellSize;
                float snappedZ = Mathf.Round(worldPoint.z / __cellSize) * __cellSize;

                __selectObject.transform.position = new Vector3(
                    snappedX,
                    __selectObject.transform.position.y,
                    snappedZ
                );
            }
        }
    }


    // 오브젝트 이동 종료
    private void MoveOff()
    {
        if (Input.GetMouseButtonUp(0))
        {
            if (__selectObject != null && __isDrag)
            {
                // 카메라 이동 활성화
                if (__cameraController != null) __cameraController.enabled = true;
            }

            __isDrag = false;
        }
    }


    // ✅ 머티리얼을 투명 모드로 전환하는 유틸리티 함수
    private void SetMaterialTransparent(Material mat)
    {
        mat.SetFloat("_Mode", 3); // Transparent
        mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        mat.SetInt("_ZWrite", 0);
        mat.DisableKeyword("_ALPHATEST_ON");
        mat.EnableKeyword("_ALPHABLEND_ON");
        mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        mat.renderQueue = 3000;
    }
}
