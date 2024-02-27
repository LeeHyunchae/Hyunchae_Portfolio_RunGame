using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//랜덤액세스 3회이상이면 캐싱해주기

public class InGameSceneController : MonoBehaviour
{
    private PlayerController playerCtrl;
    private FloorController floorCtrl;
    private ObstacleController obstacleCtrl;
    private CoinController coinCtrl;
    private int curGameSpeed = 5;
    private float flyObstacleInterval = 3f;
    private Camera mainCam;

    private void Awake()
    {
        mainCam = Camera.main;

        InitPlayerCtrl();
        InitObstacleCtrl();
        InitCoinCtrl();
        InitFloorCtrl();

        SetSpeedRate();
        SetObstacles();
        SetCoins();
    }

    private void InitPlayerCtrl()
    {
        playerCtrl = new PlayerController();
        playerCtrl.Init();
    }

    private void InitFloorCtrl()
    {
        floorCtrl = new FloorController();
        floorCtrl.OnChaneCurFloor = ChangeCurFloor;
        floorCtrl.OnRepositionFloor = OnRepositionFloor;
        floorCtrl.SetScreenLeft(mainCam.ScreenToWorldPoint(new Vector3(0, 0, 0)).x);
        floorCtrl.Init();
        floorCtrl.SetPlayerHalfSize(playerCtrl.GetPlayerHalfSize);
        
    }
    private void InitObstacleCtrl()
    {
        obstacleCtrl = new ObstacleController();
        obstacleCtrl.SetScreenLeftRight(mainCam.ScreenToWorldPoint(new Vector3(0, 0, 0)).x, mainCam.ScreenToWorldPoint(new Vector3(Screen.width,0,0)).x);
        obstacleCtrl.SetFlyObstacleInterval(flyObstacleInterval);
        obstacleCtrl.Init();
    }

    private void InitCoinCtrl()
    {
        coinCtrl = new CoinController();
        coinCtrl.SetScreenLeftRight(mainCam.ScreenToWorldPoint(new Vector3(0, 0, 0)).x, mainCam.ScreenToWorldPoint(new Vector3(Screen.width, 0, 0)).x);
        coinCtrl.Init();
        coinCtrl.SetPlayerHalfSize(playerCtrl.GetPlayerHalfSize);
    }

    private void FixedUpdate()
    {
        playerCtrl.FixedUpdate();
        obstacleCtrl.FixedUpdate();
    }
    private void Update()
    {
        playerCtrl.Update();
        floorCtrl.Update();
        obstacleCtrl.Update();
        coinCtrl.Update();

        if(Input.GetKeyDown(KeyCode.RightArrow))
        {
            curGameSpeed++;
            SetSpeedRate();
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            curGameSpeed--;
            SetSpeedRate();
        }
    }

    private void SetSpeedRate()
    {
        floorCtrl.SetSpeedRate(curGameSpeed);
        obstacleCtrl.SetSpeedRate(curGameSpeed);
    }

    private void ChangeCurFloor(Floor _curFloor)
    {
        playerCtrl.SetCurFloor(_curFloor);
    }

    private void SetObstacles()
    {
        playerCtrl.SetObstacles(obstacleCtrl.GetObstacles);
    }

    private void SetCoins()
    {
        playerCtrl.SetCoins(coinCtrl.GetCoins);
    }

    private void OnRepositionFloor(Floor _floor)
    {
        List<BaseObstacle> obstacles = obstacleCtrl.OnRepositionFloor(_floor);
        coinCtrl.OnRepositionFloor(_floor,obstacles);
    }

}
