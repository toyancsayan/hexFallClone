using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class HexObject : MonoBehaviour
{
    [SerializeField]
    private MarkerObject[] markerObjects;

    [SerializeField]
    private TextMeshProUGUI bombCountDownText;

    private int m_rowIndex;
    private int m_columnIndex;
    private int m_columnCount;
    private int m_rowCount;
    private int nextIndex;

    private Color m_hexColor;

    private MarkerManager m_markerManager;
    private HexManager m_hexManager;

    private bool isFalling = false;

    private List<List<HexObject>> m_mainList;

    private int bombCountDown;
    private bool m_isBomb;

    Animator m_animator;

    public static event Action GameOver = delegate { };
    public void InitAtStart(int columnIndex, int rowIndex, HexManager hexManager,MarkerManager markerManager,int columnCount,int rowCount, List<List<HexObject>> mainList)
    {
   
        m_rowIndex = rowIndex;
        m_columnIndex = columnIndex;
        m_hexManager = hexManager;
        m_markerManager = markerManager;
        m_mainList = mainList;
        m_columnCount = columnCount;
        m_rowCount = rowCount;

        m_animator = GetComponent<Animator>();

        InitMarkerObjects();


    }

    public void ReInit(int columnIndex, int rowIndex,bool isBomb)
    {
        StopFalling();
        m_rowIndex = rowIndex;
        m_columnIndex = columnIndex;
        DealWithBombProperties(isBomb);

    }

    private void OnDisable()
    {
        HexManager.SwipeSuccesAction -= SuccesSwipe;
    }


    void InitMarkerObjects()
    {
        for(int i = 0; i < markerObjects.Length; i++)
            markerObjects[i].Init(m_markerManager,(i==0), this, m_rowCount,m_columnCount, m_hexManager, m_mainList);
    }

    void MarkerObjectActivation()
    {
        for (int i = 0; i < markerObjects.Length; i++)
            markerObjects[i].DecideIsActive();
    }

    void DealWithBombProperties(bool isBomb)
    {
        bombCountDownText.transform.parent.gameObject.SetActive(isBomb);
        m_animator.enabled = isBomb;

        if (!isBomb)
        {
            HexManager.SwipeSuccesAction -= SuccesSwipe;
            bombCountDown = 0;
        }
        else
        {
            HexManager.SwipeSuccesAction += SuccesSwipe;
            bombCountDown = UnityEngine.Random.Range(5, 10);
        }

        bombCountDownText.text = bombCountDown.ToString();

    }


    void SuccesSwipe()
    {

        if (bombCountDown-- == 1)
            GameOver();
        else
            bombCountDownText.text = bombCountDown.ToString();

    }


    internal void LogNeighbours(List<List<HexObject>> m_mainList)
    {
        List<HexObject> neighbourList = new List<HexObject>();
        FindNeighbours(neighbourList);
       
        foreach (var neighbour in neighbourList)
        {
            if(neighbour!=null)
            Debug.Log(gameObject.name + " : " + neighbour.gameObject.name);
        }

            
    }



    public List<HexObject> FindNeighboursAndCheckForExpolation()
    {
       List<HexObject> neighbourList = new List<HexObject>();
       FindNeighbours(neighbourList);
     return  CheckForExpolation(neighbourList);
     
    }

    List<HexObject> CheckForExpolation(List<HexObject> neighbourList)
    {
        List<HexObject> objectsToDestroy = new List<HexObject>();
        for (int j = 0; j < 6; j++)
        {
            nextIndex = (j + 1) % 6;
            if (neighbourList[j] != null && neighbourList[nextIndex] != null)
            {
                if ((m_hexColor == neighbourList[j].HexColor) && (m_hexColor == neighbourList[nextIndex].HexColor))
                {
                    objectsToDestroy.Add(neighbourList[j]);
                    objectsToDestroy.Add(neighbourList[nextIndex]);
                    objectsToDestroy.Add(this);

                    neighbourList.Clear();
                    return objectsToDestroy;
                }
             
            }
           
        }

            neighbourList.Clear();
            return null;
    }

    //Explode when Falling
    public void StopFalling()
    {
        LeanTween.cancel(this.gameObject);
        StopAllCoroutines();
        isFalling = false;

    }


    bool IsNull( Vector2 index)
    {
        return ColumnIndex + index.x >= m_columnCount || ColumnIndex + index.x < 0 || RowIndex + index.y >= m_rowCount || RowIndex + index.y < 0;
    }

    void DecideIndexList(out Vector2[] indexList)
    {
        if (IsOdd)
            indexList = GameConstants.odd_neighbourIndexes;
        else
            indexList = GameConstants.even_neighbourIndexes;

    }

    public void FindNeighbours( List<HexObject> neighbourList ) {

        DecideIndexList(out Vector2[] indexList);

        for (int i = 0; i < indexList.Length; i++)
        {
            if (!IsNull(indexList[i]) && m_mainList[ColumnIndex + (int)indexList[i].x][RowIndex + (int)indexList[i].y]!=null)
                neighbourList.Add(m_mainList[ColumnIndex + (int)indexList[i].x][RowIndex + (int)indexList[i].y]);
            else
                neighbourList.Add(null);
        }


    }

    public void Fall()
    {
        Debug.Log(this.gameObject +"  " + " FALLED");
        m_rowIndex--;

        StartCoroutine(FallCoroutine());

    }

    IEnumerator FallCoroutine()
    {
        while (isFalling)
            yield return null;

        isFalling = true;

         LeanTween.move(gameObject, new Vector3(transform.position.x, transform.position.y - 0.71f), 0.3f);

         yield return new WaitForSeconds(0.3f);

        MarkerObjectActivation();

        m_hexManager.FillHexesToDestroyList(this);
        m_hexManager.Explode();
        
        isFalling = false;


    }



    public int RowIndex { get => m_rowIndex; set => m_rowIndex = value; }
    public int ColumnIndex { get => m_columnIndex; set => m_columnIndex = value; }
    public bool IsOdd { get => m_columnIndex%2!=0; }

    public  Color HexColor {

        get
        {
            return m_hexColor;
        }

        set
        {
            m_hexColor = value;
            GetComponent<SpriteRenderer>().color = m_hexColor;
        }
    }

}
