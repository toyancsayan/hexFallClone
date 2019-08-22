using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class HexCreator : MonoBehaviour
{
    [SerializeField]
    HexManager hexmanager;
    [SerializeField]
    LevelProperties_SO levelProperties;
    [SerializeField]
    MarkerManager markerManager;
    [SerializeField]
    GameObject marker;

    private float highestRowPos;
    private float correctedYofset = 0;

    private int columnCount;
    private int rowCount;

    List<List<HexObject>> mainList= new List<List<HexObject>>();

    private Vector3 startPosition;
    private Vector3 tempStartPosition;

    void Start()
    {
        AssignColumnAndRowCount();
        SetPositionOfStartPoint();
        StartCoroutine(CreateHexes());
        hexmanager.Init(mainList,this,columnCount,rowCount);
        markerManager.Init(marker);
    }


   void AssignColumnAndRowCount()
    {
        columnCount = levelProperties.ColumnCount;
        rowCount = levelProperties.RowCount;
    }



    //Assigns first Hexes start position according to row and column count to set grid on middle of the screen.
    void SetPositionOfStartPoint()
    {
        float x = GameConstants.aCoefficientForX * columnCount + GameConstants.bCoefficientForX;
        float y = GameConstants.aCoefficientForY * rowCount + GameConstants.bCoefficientForY-GameConstants.yOffset;

        startPosition = new Vector3(x, y, 0);
    }


   //Different Yoffset for odd and even columns.
    void DecideCorrectedYOffset()
    {
        if (correctedYofset==0)
            correctedYofset = GameConstants.yOffsetCorrection;
        else
            correctedYofset = 0;
    }
    


    IEnumerator CreateHexes()
    {
        for (int i = 0; i < columnCount; i++)
        {
            DecideCorrectedYOffset();

            tempStartPosition = new Vector3(startPosition.x + GameConstants.xOffset * i, startPosition.y + correctedYofset);

            List<HexObject> tempList = new List<HexObject>();

            for (int j = 0; j < rowCount; j++)
            {
                tempStartPosition = new Vector3(tempStartPosition.x , tempStartPosition.y + GameConstants.yOffset);

                tempList.Add(CreateStartedHexObject(tempStartPosition, i, j));

                GenerateNonExplodingColorAtStart(j, tempList);

                yield return new WaitForSeconds(0.03f);

            }
            mainList.Add(tempList);
          
        }
        AssignHighestRowPos();
    }

    //Store HighestRowPosition to Decide Created New Hex Position.
    void AssignHighestRowPos()
    {
        highestRowPos = mainList[0].Last().gameObject.transform.localPosition.y;
    }


    private void GenerateNonExplodingColorAtStart(int j, List<HexObject> tempList)
    {
        if (j != 0)
        {
            do
            {
                tempList[j].HexColor = levelProperties.GenerateRandomColor();
            } while (tempList[j - 1].HexColor == tempList[j].HexColor);
        }
        else
            tempList[j].HexColor = levelProperties.GenerateRandomColor();
    }

    private HexObject CreateStartedHexObject(Vector3 tempStartPosition, int columnIndex, int rowIndex)
    {
        GameObject newhex = GameObject.Instantiate(levelProperties.HexPrefab, transform);
        newhex.transform.localPosition = tempStartPosition;

       newhex.name = columnIndex + ":" + rowIndex;

        HexObject hexObject = newhex.GetComponent<HexObject>();

        hexObject.InitAtStart(columnIndex, rowIndex, hexmanager, markerManager, columnCount,rowCount, mainList);

        return hexObject;

    }
     
    //Different method from Start because of optimization.
    void DecidePooledCorrectedYOffset(bool isOdd)
    {
        correctedYofset = 0;

        if (isOdd)
            correctedYofset = -GameConstants.yOffsetCorrection;

    }

    public HexObject PoolHexObject(HexObject hexObject,bool isBomb)
    {
        DecidePooledCorrectedYOffset(hexObject.IsOdd);

        hexObject.ReInit(hexObject.ColumnIndex, rowCount - 1, isBomb);
        hexObject.HexColor = levelProperties.GenerateRandomColor();
        hexObject.transform.localPosition = new Vector3(hexObject.transform.localPosition.x, highestRowPos + correctedYofset);


        return hexObject;
    }

}
