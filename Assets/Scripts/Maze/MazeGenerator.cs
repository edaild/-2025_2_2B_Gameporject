using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeGenerator : MonoBehaviour
{
    public static MazeGenerator Instance;
    [Header("미로 설정")]
    public int width = 10;
    public int height = 10;
    public GameObject cellprefab;
    public float cellSize = 2f;

    [Header("시각화 설정")]
    public bool visualizeGeneration = false;                      // 생상 과정 보기
    public float visaulzationSpeed = 0.5f;                       // 속도
    public Color visitedColor = Color.cyan;                      // 방문한 칸 색상
    public Color currentColor = Color.yellow;                    // 현재 칸 색상
    public Color backtrackColor = Color.magenta;               // 뒤로 가기 색상

    private MazeCell[,] maze;                       
    private Stack<MazeCell> cellstack;                          //DFS를 위한 스택
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GenerateMaze()
    {
        maze = new MazeCell[width, height];
        cellstack = new Stack<MazeCell>();

        CreateCells();                          // 모든 셀 생성

        if (visualizeGeneration)
        {

        }
        else
        {
            GenerateWithDFS();
        }
    }

    void GenerateWithDFS()
    {
        MazeCell current = maze[0, 0];
        current.visited = true;
        cellstack.Push(current);

        while(cellstack.Count > 0)
        {
            current = cellstack.Peek();

            // 방문하지 않은 이웃 찾기
            List<MazeCell> unvisitedNeighbors = GetUnvisitedNetighbors(current);    // 방문하지 않은 이웃 찾기

            if(unvisitedNeighbors.Count > 0)
            {
               
                MazeCell next = unvisitedNeighbors[Random.Range(0, unvisitedNeighbors.Count)];       // 랜덤하게 이웃 선택
                RemoveWaliBetween(current, next);
                next.visited = true;
                cellstack.Push(next);
            }
            else
            {
                cellstack.Pop();            // 백 드래킹
            }
        }
    }

    void CreateCells()          // 셀 생성 함수
    {
        if(cellprefab == null)
        {
            Debug.LogError("셀 프리랩이 없음");
            return;
        }

        for(int x = 0; x < width; x++)
        {
            for(int z= 0; z < height; z++)
            {
                Vector3 pos = new Vector3(x * cellSize, 0, z * cellSize);
                GameObject cellObj = Instantiate(cellprefab, pos, Quaternion.identity, transform);
                cellObj.name = $"Cell_{x}_{z}";

                MazeCell cell = cellObj.GetComponent<MazeCell>();
                if(cell == null)
                {
                    Debug.LogError("MazeCell 스크립트 없음");
                    return;
                }
                cell.Initialize(x, z);
                maze[x,z] = cell;
            }
        }
    }
    List<MazeCell> GetUnvisitedNetighbors(MazeCell cell)
    {
        List<MazeCell> neighbors = new List<MazeCell>();

        // 상하좌우 체크
        //if (cell.x > 0 && !maze[cell.x - 1, cell.z].visited)
        //    neighbors.Add(maze(cell.x - 1, cell.z));

        //if (cell.x > 0 && !maze[cell.x + 1, cell.z].visited)
        //    neighbors.Add(maze(cell.x + 1, cell.z));

        //if (cell.x > 0 && !maze[cell.x, cell.z -1].visited)
        //    neighbors.Add(maze(cell.x, cell.z - 1));

        //if (cell.x > 0 && !maze[cell.x, cell.z + 1].visited)
        //    neighbors.Add(maze(cell.x, cell.z + 1));

        return neighbors;
    }
    
    void RemoveWaliBetween(MazeCell crrent, MazeCell next)
    {
        if (crrent.x < next.x)                  
        {
            crrent.RemoveWall("right");         // 오른쪽
            next.RemoveWall("left");
        }

        if (crrent.x < next.x)                  // 왼쪽
        {
            crrent.RemoveWall("left");
            next.RemoveWall("right");
        }

        if (crrent.x < next.x)              // 위
        {
            crrent.RemoveWall("top");
            next.RemoveWall("bottom");
        }

        if (crrent.x < next.x)          // 아래
        {
            crrent.RemoveWall("bottom");
            next.RemoveWall("top");
        }
    }

   // 특정 위치의 셀 가져오기
    public MazeCell GetCell(int x, int z)
    {
        if(x>= 0 && x < width && z>= 0 && z < height)
            return maze[x, z];

        return null;
    }

    IEnumerator GenerateithDRSVusyakuzed()
    {
        MazeCell current = maze[0, 0];
        current.visited = true;

        current.SetColor(currentColor);             // 시각화 추가 코딩
        cellstack.Clear();                          // 시각화 추가 코딩

        cellstack.Push(current);                    // 첫번째 현재칸을 스택에 넣는다

        yield return new WaitForSeconds(visaulzationSpeed);     //+

        int totalcells = width * height;
        int visitedCount = 1;

        while (cellstack.Count > 0)             
        {
            current = cellstack.Peek();

            current.SetColor(currentColor);
            yield return new WaitForSeconds(visaulzationSpeed);

            // 방문하지 않은 이웃 찾기
            List<MazeCell> unvisitedNeighbors = GetUnvisitedNetighbors(current);    // 방문하지 않은 이웃 찾기

            if (unvisitedNeighbors.Count > 0)
            { 
                MazeCell next = unvisitedNeighbors[Random.Range(0, unvisitedNeighbors.Count)];       // 랜덤하게 이웃 선택
                RemoveWaliBetween(current, next);
                current.SetColor(visitedColor);
                next.visited = true;
                visitedCount++;
                cellstack.Push(next);

                next.SetColor(currentColor);
                yield return new WaitForSeconds(visaulzationSpeed);
            }
            else
            {
                current.SetColor(backtrackColor);
                yield return new WaitForSeconds(visaulzationSpeed);
                cellstack.Pop();            // 백 드래킹
            }
            yield return new WaitForSeconds(visaulzationSpeed);
            ResetAllColors();
        }

        void ResetAllColors()
        {
            for(int x =0; x <width; x++)
            {
                for(int z = 0; z <height; z++)
                {
                    maze[x, z].SetColor(Color.white);  
                }
            }
        }
    }
}


