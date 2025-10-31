using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridController : MonoBehaviour
{
    [SerializeField]
    private int __gridSize = 10;                    // Grid 크기

    [SerializeField]
    private float __cellSize = 1f;                  // cell 단위

    [SerializeField]
    private Material __lineMaterial;                // line material

    [SerializeField]
    private Color __lineColor = Color.gray;         // color

    // grid line object를 저장할 배열
    private List<GameObject> __gridList = new List<GameObject>();




    private void Awake()
    {
        for (int i = 0; i <= __gridSize; i++)
        {
            float offset = i * __cellSize - (__gridSize * __cellSize / 2);
            createGridLine(new Vector3(-__gridSize * __cellSize / 2, 0, offset), new Vector3(__gridSize * __cellSize / 2, 0, offset));      // Z선
            createGridLine(new Vector3(offset, 0, -__gridSize * __cellSize / 2), new Vector3(offset, 0, __gridSize * __cellSize / 2));      // X선
        }
    }





    // Grid line 생성
    private void createGridLine(Vector3 start, Vector3 end)
    {
        GameObject lineObj = new GameObject("GridLine");
        lineObj.transform.parent = this.transform;

        LineRenderer lr = lineObj.AddComponent<LineRenderer>();
        lr.positionCount = 2;
        lr.SetPosition(0, start);
        lr.SetPosition(1, end);
        lr.startWidth = 0.005f;
        lr.endWidth = 0.005f;
        lr.material = __lineMaterial;
        lr.useWorldSpace = true;

        __gridList.Add(lineObj);
    }


    // Grid on/off
    private void ChangeGridState(bool state)
    {
        foreach (GameObject grid in __gridList)
        {
            grid.GetComponent<LineRenderer>().enabled = state;
        }
    }


    // Grid On/Off (Manager에서 실행용)
    public void GridOnOff(bool state)
    {
        ChangeGridState(state);
    }
}
