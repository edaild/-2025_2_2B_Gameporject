using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room 
{
    public Vector2Int centor;
    public int size;
    public RoomType type;

    public Room(Vector2Int centor,int size, RoomType type)
    {
        this.centor = centor;
        this.size = size;
        this.type = type;
    }

    public Color getColor()
    {
        switch (type)
        {
            case RoomType.Start:
                return Color.green;
            case RoomType.Treasure:
                return Color.yellow;
            case RoomType.Boss:
                return Color.red;
             default:
                return Color.white;
        }
    }
}
