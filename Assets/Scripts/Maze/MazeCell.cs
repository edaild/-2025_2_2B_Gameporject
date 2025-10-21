using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MazeCell : MonoBehaviour
{

    public GameObject leftWall;
    public GameObject rightWall;
    public GameObject bottomWall;
    public GameObject topWall;
    public GameObject floor;

    public bool visited = false;                    // 방문한 셀인지 확인
    public int x;
    public int z;

    public void Initialize(int xPos, int zPos)     // 셀 초기화
    {
        x = xPos;
        z = zPos;
        visited = false;
        ShowAllWalls();
    }

    public void ShowAllWalls()
    {
        leftWall.SetActive(true);
        rightWall.SetActive(true);
        bottomWall.SetActive(true);
        topWall.SetActive(true);
        floor.SetActive(true);

    }

    public void RemoveWall(string dirction)
    {
        switch (dirction)
        {
            case "left":
                leftWall.SetActive(false);
                break;
            case "right":
                rightWall.SetActive(false);
                break;
            case "top":
                topWall.SetActive(false);
                break;
            case "bottomWall":
                bottomWall.SetActive(false);
                break;
        }
    }

    public void SetColor(Color color)               // 셀 색장 변경(경로 표시용)
    {
        floor.GetComponent<Renderer>().material.color = color;
    }
}
