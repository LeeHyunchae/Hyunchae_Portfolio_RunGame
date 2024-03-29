using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class CoinController
{
    private const string COIN_PATH = "Prefabs/Coin";
    private const float MIN_OBSTACLE_INTERVAL = 0.6f;
    private const int MIN_FLOOR_INTERVAL = 4;

    private const int COIN_CAPACITY = 500;

    private Coin[] coins;
    private List<Coin> repositionCoins = new List<Coin>();

    private float screenLeft;
    private float screenRight;

    private int prevCoinIdx;

    private Transform coinParent;


    public void Init()
    {
        CreateCoins();

    }
    private void InitCoins(GameObject[] _coins)
    {

        for (int i = 0; i < COIN_CAPACITY; i++)
        {
            Coin coin = new Coin();

            coin.Init(_coins[i]);
            coin.SetActive(false);

            coins[i] = coin;

            coin.SetParentTm(coinParent);

            coin.GetTransform.SetParent(coinParent);
        }
    }

    public void SetScreenLeftRight(float _screenLeft, float _screenRight)
    {
        screenLeft = _screenLeft;
        screenRight = _screenRight;
    }

    public Coin[] GetCoins => coins;

    private void CreateCoins()
    {
        coinParent = new GameObject("Coins").transform;

        coinParent.transform.position = Vector2.zero;

        coins = new Coin[COIN_CAPACITY];

        CreateCoinObject();
    }

    private void CreateCoinObject()
    {
        GameObject originCoinObj = (GameObject)Resources.Load(COIN_PATH);
        GameObject[] coinObjs = new GameObject[COIN_CAPACITY];

        for (int i = 0; i < COIN_CAPACITY; i++)
        {
            coinObjs[i] = GameObject.Instantiate<GameObject>(originCoinObj, Vector2.zero, Quaternion.identity, coinParent);
        }

        InitCoins(coinObjs);
    }
    public void Update()
    {
        CheckCoinPos();
    }

    private void CheckCoinPos()
    {
        for (int i = 0; i < COIN_CAPACITY; i++)
        {
            Coin coin = coins[i];

            if (!coin.GetActive)
            {
                continue;
            }

            if(CheckInScreenCoin(coin))
            {
                coin.SetIsInScreen(true);
            }

            if (CheckOutsideCoin(coin))
            {
                coin.SetActive(false);
                coin.SetIsInScreen(false);
            }
        }
    }
    private bool CheckInScreenCoin(Coin _coin)
    {
        return _coin.GetTransform.position.x < screenRight;
    }

    private bool CheckOutsideCoin(Coin _coin)
    {
        return _coin.GetTransform.position.x + _coin.GetWidth() * 0.5f <= screenLeft;
    }

    public List<Coin> OnRepositionFloor(Floor _rePosFloor, List<BaseObstacle> _obstacles)
    {
        repositionCoins.Clear();

        if (_rePosFloor.GetPrevFloorDistance > MIN_FLOOR_INTERVAL)
        {
            bool isSquarePattern = Random.value > 0.5f;

            if (isSquarePattern)
            {
                SetPosSquarePattern(_rePosFloor);
            }
        }

        if (_obstacles.Count == 0)
        {
            SetPosStrightRandomCoinPattern(_rePosFloor);
        }
        else
        {
            SetPosObstaclePattern(_rePosFloor, _obstacles);
        }

        return repositionCoins;
    }

    #region Patterns
    private void SetPosStrightRandomCoinPattern(Floor _floor)
    {
        Floor floor = _floor;

        int size = floor.GetFloorWidth();

        int coinGrade = Random.Range(0, (int)ECoinType.END);

        float remainder = size % 2;

        float floorHalfSize = (size - remainder) * 0.5f;

        float coinStartposX = -floorHalfSize;

        if (remainder != 0)
        {
            coinStartposX -= 0.5f;
        }

        for (int i = 0; i <= size; i++)
        {
            Coin coin;

            coin = coins[prevCoinIdx];
            prevCoinIdx = (prevCoinIdx + 1) % COIN_CAPACITY;

            coin.SetCoinGrade(coinGrade);

            Vector2 coinPos = floor.GetTransform.position;

            coinPos.x = coinStartposX + i;

            coinPos.y = (floor.GetFloorHeight() * 0.5f) + (coin.GetHeight() * 0.5f);

            coin.GetTransform.SetParent(floor.GetTransform);
            coin.GetTransform.localPosition = coinPos;

            coin.SetActive(true);

            repositionCoins.Add(coin);
        }

    }

    private void SetPosObstaclePattern(Floor _floor, List<BaseObstacle> _obstacles)
    {
        Floor floor = _floor;

        int size = floor.GetFloorWidth();

        int coinGrade = Random.Range(0, (int)ECoinType.END);

        int obstacleIdx = 0;

        int obstaclesCount = _obstacles.Count -1;

        float remainder = size % 2;

        float floorHalfSize = (size - remainder) * 0.5f;

        float coinStartposX = -floorHalfSize;

        if (remainder != 0)
        {
            coinStartposX -= 0.5f;
        }

        for (int i = 0; i <= size; i++)
        {
            Vector2 obstaclePos = _obstacles[obstacleIdx].GetTransform.localPosition;
            float obstacleHalfWidth = _obstacles[obstacleIdx].GetWidth() * 0.5f;
                
            Coin coin;

            coin = coins[prevCoinIdx];
            prevCoinIdx = (prevCoinIdx + 1) % COIN_CAPACITY;

            coin.SetCoinGrade(coinGrade);

            Vector2 coinPos = floor.GetTransform.position;

            coinPos.x = coinStartposX + i;

            coinPos.y = (floor.GetFloorHeight() * 0.5f) + (coin.GetHeight() * 0.5f);

            float distance = Mathf.Abs(coinPos.x - obstaclePos.x);

            if (distance < MIN_OBSTACLE_INTERVAL + obstacleHalfWidth)
            {
                coinPos.y += 2;
            }
            
            if(coinPos.x > obstaclePos.x && obstacleIdx != obstaclesCount)
            {
                obstacleIdx++;
            }

            coin.GetTransform.SetParent(floor.GetTransform);
            coin.GetTransform.localPosition = coinPos;

            coin.SetActive(true);

            repositionCoins.Add(coin);
        }

    }

    private void SetPosSquarePattern(Floor _floor)
    {
        Floor floor = _floor;

        int squareSize = Random.Range(MIN_FLOOR_INTERVAL, _floor.GetPrevFloorDistance);

        int coinGrade = Random.Range(0, (int)ECoinType.END);

        Vector2 coinStartPos = _floor.GetTransform.position;

        float floorWidthHalf = _floor.GetFloorWidth() * 0.5f;
        float prevFloorDistanceHalf = _floor.GetPrevFloorDistance * 0.5f;
        float squareSizeHalf = squareSize * 0.5f;
        float coinWidthHalf = coins[0].GetWidth() * 0.5f;

        coinStartPos.x = coinStartPos.x - floorWidthHalf - prevFloorDistanceHalf - squareSizeHalf - coinWidthHalf;
        coinStartPos.y = _floor.GetPrevFloorPos.y + squareSize + MIN_FLOOR_INTERVAL;

        float squareStartPosY = coinStartPos.y;

        for (int i = 0; i < squareSize * squareSize; i++)
        {
            Coin coin;

            coin = coins[prevCoinIdx];
            prevCoinIdx = (prevCoinIdx + 1) % COIN_CAPACITY;

            coin.SetCoinGrade(coinGrade);

            if (i % squareSize == 0)
            {
                coinStartPos.x += coin.GetWidth();
            }

            coinStartPos.y = squareStartPosY - (coin.GetHeight() * (i % squareSize));

            coin.GetTransform.SetParent(floor.GetTransform);
            coin.GetTransform.position = coinStartPos;

            coin.SetActive(true);

        }

    }
    #endregion
}
