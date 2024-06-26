using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxScrollingController : MonoBehaviour
{
    [SerializeField] private Sprite[] sprites;
    [SerializeField] private GameObject originObj;
    public bool isRandomPosY = false;

    private const int OBJCOUNT = 20;
    private const int REPOSITIONPOINT = 20;

    private float speedRate;
    private float biggerSpriteSize;
    private Transform[] objectTMs = new Transform[OBJCOUNT];
    private SpriteRenderer[] objectSprites = new SpriteRenderer[OBJCOUNT];

    private float screenLeft;
    private float maxPosX;
    private float standardY;

    private Vector2 objectLocalPos;

    private void Awake()
    {
        CreateScrollingObj();
        CalculateBiggerSpriteSize();

        screenLeft = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, 0)).x;

        screenLeft -= REPOSITIONPOINT;

        InitObjectsPos();

        maxPosX = biggerSpriteSize * OBJCOUNT + screenLeft;

        if(isRandomPosY)
        {
            standardY = transform.position.y;
        }

        objectLocalPos = transform.position;
    }

    private void Update()
    {
        for(int i = 0; i < OBJCOUNT;i++)
        {
            Transform objTM = objectTMs[i];
            Vector2 objPos = objTM.position;

            objPos.x += speedRate * -1f * Time.deltaTime;

            objTM.position = objPos;

            if(objTM.position.x <= screenLeft)
            {
                RepositionObject(i);
            }
        }
    }

    private void CreateScrollingObj()
    {
        for(int i = 0; i < OBJCOUNT;i++)
        {
            objectTMs[i] = Instantiate<GameObject>(originObj,transform).GetComponent<Transform>();
            objectSprites[i] = objectTMs[i].GetComponent<SpriteRenderer>();
            objectSprites[i].sprite = sprites[Random.Range(0, sprites.Length)];
        }

        speedRate = objectSprites[0].sortingOrder;
    }

    private void CalculateBiggerSpriteSize()
    {
        int count = sprites.Length;

        float widthSize = 0;

        for(int i = 0;  i<count; i++)
        {
            float spriteWidthSize = sprites[i].bounds.size.x;

            if(widthSize < spriteWidthSize)
            {
                widthSize = spriteWidthSize;
            }
        }

        biggerSpriteSize = widthSize;
    }

    private void InitObjectsPos()
    {
        float curPosX = screenLeft;
        biggerSpriteSize *= objectTMs[0].localScale.x;

        for(int i = 0; i<objectTMs.Length;i++)
        {
            if (isRandomPosY)
            {
                objectTMs[i].localPosition = new Vector2(curPosX, standardY + Random.Range(-2, 3));
            }
            else
            {
                objectTMs[i].localPosition = new Vector2(curPosX, 0);
            }

            curPosX += biggerSpriteSize;
        }
    }

    private void RepositionObject(int _objIdx)
    {
        if(isRandomPosY)
        {
            objectTMs[_objIdx].localPosition = new Vector2(maxPosX, standardY + Random.Range(-2,3));
        }
        else
        {
            objectTMs[_objIdx].localPosition = new Vector2(maxPosX, 0);
        }
        objectSprites[_objIdx].sprite = sprites[Random.Range(0, sprites.Length)];
    }
}
