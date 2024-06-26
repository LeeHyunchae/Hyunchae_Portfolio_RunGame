using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Floor
{
    private const int LEFT = 0;
    private const int MIDDLE = 1;
    private const int RIGHT = 2;
    private const float HALFSIZE = 0.5f;

    private SpriteRenderer[] floors = new SpriteRenderer[3];
    private Transform _transform;

    public Transform GetTransform => _transform;
    private Vector2 prevFloorPos;
    private int prevFloorDistance;

    public Vector2 GetPrevFloorPos => prevFloorPos;
    public int GetPrevFloorDistance => prevFloorDistance;

    public void Init(GameObject _floorObj)
    {
        floors = _floorObj.GetComponentsInChildren<SpriteRenderer>();
        _transform = _floorObj.GetComponent<Transform>();
        SetMiddleSize(1);
    }

    public void SetMiddleSize(int _middleSize)
    {
        floors[MIDDLE].size = new Vector2(_middleSize, 1);

        floors[LEFT].transform.localPosition = new Vector2(-_middleSize * HALFSIZE - HALFSIZE, 0);
        floors[RIGHT].transform.localPosition = new Vector2(_middleSize * HALFSIZE + HALFSIZE, 0);
    }

    public void SetPrevFloorPos(Vector2 _prevFloorPos, int _distance)
    {
        prevFloorPos = _prevFloorPos;
        prevFloorDistance = _distance;
    }

    public int GetFloorWidth()
    {
        return (int)(floors[LEFT].size.x + floors[MIDDLE].size.x + floors[RIGHT].size.x);
    }

    public float GetFloorHeight()
    {
        return 1;
    }

    public int GetFloorMiddleSize()
    {
        return (int)floors[MIDDLE].size.x;
    }
}
