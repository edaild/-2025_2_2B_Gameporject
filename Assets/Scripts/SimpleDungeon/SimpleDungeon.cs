using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SocialPlatforms.GameCenter;
using UnityEngine.UIElements;
using static UnityEditor.PlayerSettings;

public class SimpleDungeon : MonoBehaviour
{
    [Header("던전 설정")]
    public int roomCount = 8;                               // 전체 생성하고 싶은 방 개수 (시작/보스/보물/일반 포함)
    public int minSize = 4;                                 // 방 최소 크기(타입 단위, 가로/ 세로 동일)
    public int maxSize = 8;                                 // 방 최대 크기(타일 크기)

    [Header("스포너 설정")]
    public bool spawnEnemies = true;                       // 일반 방과 보스 방에 적을 생성 할지 여부
    public bool spawntreasures = true;                      // 보물 방에 보물을 생성 할지 여부
    public int enemiesperRoom = 2;                          // 일반 방 1개당 생성할 적의 수

    private Dictionary<Vector2Int, Room> rooms = new Dictionary<Vector2Int, Room>();                // 방 중심 좌표 -> 방정보 매핑, 방 메타데이터 보관
    private HashSet<Vector2Int> floors = new HashSet<Vector2Int>();                                 // floers : 바닥 타일 좌표 집합, 어떤 칸이 바닥인지 조화
    private HashSet<Vector2Int> walls = new HashSet<Vector2Int>();                                  // walls : 벽 타일 좌표 집합, 바닥 주변을 자도으로 채운다.

   

    // Start is called before the first frame update
    void Start()
    {
        Generate();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            Clear();
            Generate();
        }
    }
    public void Generate()
    {
        // 방 여러개를 규칙적으로 만든다.
        CreateRoom();
        // 방과 방 사이를 복도로 연결 한다.
        ConneectRooms();
        // 바닥 주변 타일에 벽을 자동 배치한다.
        CreateWalls();
        // 실제 Unity상에서 Cube로 타일을 그린다.
        Render();
        // 방 타입에 따라 적/보물을 배치한다.
        SpawnObjects();
    }


    // 시작방 1개 설정, 나머지는 기존 방 근처(상/하/좌/우)에 오프셋을 두고 시도
    // 마지막 생성 방은 보스방으로 지정, 일반 방 일부를 보물방으로 변환

    void CreateRoom()
    {
        // 시작 방 : 기준점 (0,0)에 배치
        Vector2Int pos = Vector2Int.zero;
        int size= Random.Range(minSize, maxSize);
        AddRoom(pos, size, RoomType.Start);         // 방 등록

        // 나머지 방들 생성 시도
        for(int i= 0; i < roomCount; i++)
        {
            var roomList = new List<Room>(rooms.Values);                    // 이미 만들어진 방 중 하나를 기준으로
            Room baseRoom = roomList[Random.Range(0, roomList.Count)];

            Vector2Int[] dirs =
            {
                Vector2Int.up *6, Vector2Int.down *6, Vector2Int.left *6, Vector2Int.right *6,              // 기존 방에서 상/하/좌/우 일정거리 새 방 후보
            };

            foreach(var dir in dirs)
            {
                Vector2Int newPos = baseRoom.centor + dir;              // 세 방 중심 좌표
                int newSize = Random.Range(minSize, maxSize);           // 세 방 크기 설정
                RoomType type = (i == roomCount - 1) ? RoomType.Boss : RoomType.Normal;
                if (AddRoom(newPos, newSize, type)) break;                          // 방 영역이 기존 바닥과 겹치지 않으면 추가 성공 -> 다음방 생성으로 진행
            }
        }

        // 일반방 중 일정 비율을 보물방으로 변화
        int treasureCount = Mathf.Max(1, roomCount / 4);                                // 현재방중 일반방만 수집
        var normalRooms = new List<Room>();

        foreach(var room in rooms.Values)
        {
            if(room.type == RoomType.Normal)
                normalRooms.Add(room);          
        }

        for(int i = 0; i< treasureCount && normalRooms.Count > 0; i++)                  // 무작위 일반방을 보물방으로 바꾼다
        {
           int idx = Random.Range(0, normalRooms.Count);
            normalRooms[idx].type = RoomType.Treasure;
            normalRooms.RemoveAt(idx);
        }
    }

    // 실제로 방 하느를 floor  타일로 추가
    // 기존 방과 겹치면 flase 반화, 겹치지 않을 경우 floor 타일로 채우고 rooms에 방 메타를 들록
    bool AddRoom(Vector2Int center, int size, RoomType type)
    {
        // 겹침 검사
        for(int x= -size / 2; x < size / 2; x++)
        {
            for(int y = -size / 2; y < size / 2; y++)
            {
                Vector2Int tile = center + new Vector2Int(x, y);
                if(floors.Contains(tile))              // 한칸이라도 겹치면 실패
                    return false;
            }
        }

        // 2. 방메타데이터 등록
        Room room = new Room(center, size, type);
        rooms[center] = room;

        // 3. 방 영역을 floors에 채운다.
        for(int x= -size / 2;x < size / 2; x++)
        {
            for(int y= -size / 2;y < size / 2; y++)
            {
                floors.Add(center + new Vector2Int(x, y));
            }
        }
        return true;
    }
    // 모든 방을 직선 복도로 연결 한다.
    // 지금 구현 단순 List -> MST , A*
    void ConneectRooms()
    {
        var roomLIst = new List<Room>(rooms.Values);

        for(int i= 0; i< roomLIst.Count - 1; i++)
        {
            CreateCorridor(roomLIst[i].centor, roomLIst[i + 1].centor);
        }
    }

    // 두 좌표 사이를 x축 -> y 축 순서로 직선 복도로 판다.
    // 굽이 치는 L자 모양이 나온다

    void CreateCorridor(Vector2Int start, Vector2Int end)
    {
        Vector2Int current = start;

        // X축 정렬 : start.x -> end.x 로 한칸식 이동하며 바닥 타일 추가
        while(current.x != end.x)
        {
            floors.Add(current);
            current.x += (end.x > current.x)?  1 : -1;
        }

        // Y축 정렬 : x가 같아진 뒤 start.y -> end.y 로 한칸식 이동
        while (current.x != end.x)
        {
            floors.Add(current);
            current.y += (end.y > current.x) ? 1 : -1;
        }
        floors.Add(end);                // 마지막 목적지 칸도 바닥 처리
    }
    // 바닥 주변의 8방향을 스캔하여, 바닥이 아니 칸을 walis로 채운다.
    void CreateWalls()
    {

        Vector2Int[] dirs =
        {
                Vector2Int.up , Vector2Int.down , Vector2Int.left, Vector2Int.right,
                new Vector2Int(1,1), new Vector2Int(1,-1), new Vector2Int(-1,1), new Vector2Int(-1, -1)
        };

        // 모든 바닥 타일을 기준으로 주변 검사
        foreach(var floor in floors)
        {
            foreach(var dir in dirs)
            {
                Vector2Int waliPos = floor + dir;
                if (!floors.Contains(waliPos))          // 주변 칸이 바닥이 아니면 "벽 칸" 으로 등록
                {
                   floors.Add(waliPos);
                }
            }
        }
    }

    // 타일을 Unity 오브젝트로 랜더리
    // 바닥 얇을 Cube (0.1) , 벽 Cube(1) , 방 색 구분
    void Render()
    {
        // 바닥 타일 렌더링
        foreach(var Pos in floors)
        {
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.transform.position = new Vector3(Pos.x, 0, Pos.y);                 // Y = 0; 평면에 배치
            cube.transform.localScale = new Vector3(1f,0.1f,1f);                    // 얇은 바닥
            cube.transform.SetParent(transform);                                    // 부모 지정

            Room room = GetRoom(Pos);
           if(room != null)
            {
                cube.GetComponent<Renderer>().material.color = room.getColor();
            }
            else
            {
                cube.GetComponent<Renderer>().material.color= Color.white;
            }
        }
        // 벽 타일 랜더링
        foreach (var pos in walls)
        {
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.transform.position = new Vector3(pos.x, 0.5f, pos.y);
            cube.transform.SetParent(transform);
            cube.GetComponent<Renderer>().material.color = Color.black;
        }
    }

    Room GetRoom(Vector2Int pos)
    {
        foreach(var room in rooms.Values)
        {
            int halfSize = room.size / 2;
            if(Mathf.Abs(pos.x - room.centor.x) < halfSize && Mathf.Abs(pos.y = room.centor.y) < halfSize)
            {
                return room;
            }
        }
        return null;
    }

    void CreateEnemy(Vector3 position)
    {
        GameObject enemy = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        enemy.transform.position = position;
        enemy.transform.localScale = Vector3.one * 0.8f;
        enemy.transform.SetParent(transform);
        enemy.name = "Enemy";
        enemy.GetComponent<Renderer>().material.color = Color.red;
    }

    void CreateBoss(Vector3 position)
    {
        GameObject boss = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        boss.transform.position = position;
        boss.transform.localScale = Vector3.one * 0.8f;
        boss.transform.SetParent(transform);
        boss.name = "Boss";
        boss.GetComponent<Renderer>().material.color = Color.red;
    }

    void CreateTreasure(Vector3 position)
    {
        GameObject boss = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        boss.transform.position = position;
        boss.transform.localScale = Vector3.one * 0.8f;
        boss.transform.SetParent(transform);
        boss.name = "Treasure";
        boss.GetComponent<Renderer>().material.color = Color.red;
    }

    Vector3 GetRandomPositioninRoom(Room room)
    {
        float halfSize = room.size / 2f - 1f;               // -1 테두리
        float randomX = room.centor.x + Random.Range(-halfSize, halfSize);
        float randomZ = room.centor.y + Random.Range(-halfSize, halfSize);

        return new Vector3(randomX, 0.5f, randomZ);
    }

    void SpawnEnemiesInRoom(Room room)
    {
        for(int i = 0; i < enemiesperRoom; i++)
        {
            Vector3 spawnPos = GetRandomPositioninRoom(room);
            CreateEnemy(spawnPos);
        }
    }

    void SpawnBossInRoom(Room room)
    {
        Vector3 spawnPos = new Vector3(room.centor.x, 1f, room.centor.y);
        CreateEnemy(spawnPos);
    }

    void SpawntreasureInRoom(Room room)
    {
        Vector3 spawnPos = new Vector3(room.centor.x, 0.5f, room.centor.y);
        CreateTreasure(spawnPos);
    }

    void Clear()
    {
        rooms.Clear();
        floors.Clear();
        walls.Clear();

        foreach(Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }

    void SpawnObjects()
    {
       foreach(var room in rooms.Values)
        {
            switch (room.type)
            {
                case RoomType.Start:
                    break;

                case RoomType.Normal:
                    if(spawnEnemies)
                       SpawnEnemiesInRoom(room);
                    break;

                case RoomType.Treasure:
                    if (spawnEnemies)
                        SpawntreasureInRoom(room);
                    break;

                case RoomType.Boss:
                    if(spawnEnemies)
                        SpawnBossInRoom(room);
                    break;
            }
        }
    }
}
