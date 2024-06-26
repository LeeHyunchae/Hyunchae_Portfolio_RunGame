using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InGameSceneController : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI scoreText;
    [SerializeField] CustomButton jumpBtn;
    [SerializeField] Image[] heartArray;

    private const string SCORE = "Score : ";
    private const int BASE_COIN_SPEED = 5;
    private const int MIN_GAME_SPEED = 5;
    private const int MAX_GAME_SPEED = 15;
    private const int SPEED_INCREASE_INTERVAL = 5;

    private float playerScore = 0;
    private float curGameSpeedTime = 0;

    private ScoreManager scoreManager;
    private PlayerController playerCtrl;
    private FloorController floorCtrl;
    private ObstacleController obstacleCtrl;
    private CoinController coinCtrl;
    private ItemController itemCtrl;
    private int curGameSpeed;
    private float flyObstacleInterval = 3f;
    private Camera mainCam;

    private bool isPlay = true;

    private float screenLeft;
    private float screenRight;

    private void Awake()
    {
        mainCam = Camera.main;

        screenLeft = mainCam.ScreenToWorldPoint(new Vector3(0, 0, 0)).x;
        screenRight = mainCam.ScreenToWorldPoint(new Vector3(Screen.width, 0, 0)).x;

        scoreManager = ScoreManager.getInstance;

        curGameSpeed = MIN_GAME_SPEED;

        InitPlayerCtrl();
        InitObstacleCtrl();
        InitCoinCtrl();
        InitItemCtrl();
        InitFloorCtrl();
        InitJumpBtn();

        SetSpeedRate();
        SetObstacles();
        SetCoins();
        SetItems();
    }

    private void InitPlayerCtrl()
    {
        playerCtrl = new PlayerController();
        playerCtrl.Init();
        playerCtrl.OnGetCoin = OnPlayerGetCoin;
        playerCtrl.OnIncreaseHP = OnIncreaseHP;
        playerCtrl.OnDecreaseHP = OnDecreaseHP;
    }

    private void InitFloorCtrl()
    {
        floorCtrl = new FloorController();
        floorCtrl.OnChaneCurFloor = ChangeCurFloor;
        floorCtrl.OnRepositionFloor = OnRepositionFloor;
        floorCtrl.SetScreenLeft(screenLeft);
        floorCtrl.Init();
        floorCtrl.SetPlayerHalfSize(playerCtrl.GetPlayerHalfSize);
        
    }
    private void InitObstacleCtrl()
    {
        obstacleCtrl = new ObstacleController();
        obstacleCtrl.SetScreenLeftRight(screenLeft, screenRight);
        obstacleCtrl.SetFlyObstacleInterval(flyObstacleInterval);
        obstacleCtrl.Init();
    }

    private void InitCoinCtrl()
    {
        coinCtrl = new CoinController();
        coinCtrl.SetScreenLeftRight(screenLeft, screenRight);
        coinCtrl.Init();
    }

    private void InitItemCtrl()
    {
        itemCtrl = new ItemController();
        itemCtrl.SetScreenLeftRight(screenLeft, screenRight);
        itemCtrl.Init();
    }

    private void InitJumpBtn()
    {
        jumpBtn.OnPointerClickEvent = playerCtrl.Jump;
        jumpBtn.OnPointerDownEvent = playerCtrl.LongJump;
        jumpBtn.SetEnable(isPlay);
    }

    private void FixedUpdate()
    {
        if(!isPlay)
        {
            return;
        }
        playerCtrl.FixedUpdate();
        obstacleCtrl.FixedUpdate();
    }
    private void Update()
    {
        //게임 일시정지
        if (!isPlay)
        {
            return;
        }

        if(curGameSpeed != MAX_GAME_SPEED)
        {
            curGameSpeedTime += Time.deltaTime;

            if(curGameSpeedTime > SPEED_INCREASE_INTERVAL)
            {
                curGameSpeedTime = 0;
                curGameSpeed++;
                SetSpeedRate();
            }
        }

        playerCtrl.Update();
        floorCtrl.Update();
        obstacleCtrl.Update();
        coinCtrl.Update();
        itemCtrl.Update();

    }

    private void SetSpeedRate()
    {
        floorCtrl.SetSpeedRate(curGameSpeed);
        obstacleCtrl.SetSpeedRate(curGameSpeed);
        playerCtrl.SetCoinSpeed(curGameSpeed + BASE_COIN_SPEED);
    }

    private void ChangeCurFloor(Floor _curFloor)
    {
        playerCtrl.SetCurFloor(_curFloor);
    }

    private void SetObstacles()
    {
        playerCtrl.SetObstacles(obstacleCtrl.GetObstacleArray);
    }

    private void SetCoins()
    {
        playerCtrl.SetCoins(coinCtrl.GetCoins);
    }

    private void SetItems()
    {
        playerCtrl.SetItems(itemCtrl.GetItems);
    }

    private void OnRepositionFloor(Floor _floor)
    {
        List<BaseObstacle> obstacles = obstacleCtrl.OnRepositionFloor(_floor);
        List<Coin> coins = coinCtrl.OnRepositionFloor(_floor,obstacles);
        itemCtrl.OnRepositionFloor(_floor, coins);
    }

    private void OnPlayerGetCoin(ECoinType _coinType)
    {
        playerScore += (int)_coinType + 1;

        scoreText.text = SCORE + playerScore;
    }

    private void OnIncreaseHP(int _hp)
    {
        if(_hp >= heartArray.Length)
        {
            return;
        }

        int idx = _hp - 1;
        heartArray[idx].enabled = true;
    }

    private void OnDecreaseHP(int _hp)
    {
        if (_hp <= 0)
        {
            //게임 종료
            isPlay = false;
            scoreManager.SetScore((int)playerScore);
            UIManager.getInstance.Show<GameOverPanel>("Prefabs/UI/GameOverPanel");
            return;
        }

        heartArray[_hp].enabled = false;
    }
}
