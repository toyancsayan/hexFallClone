using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lean.Touch;
using System;

public class HexManager : MonoBehaviour
{

    List<List<HexObject>> m_mainList;

    List<HexObject> m_selectedGroupList = new List<HexObject>();
    List<List<HexObject>> hexesTodestroy = new List<List<HexObject>>();
    List<HexObject> removedHexObject = new List<HexObject>();


    HexCreator m_hexCreator;

    int m_rowCount;
    int m_columnCount;

    int rowIndexOfExplodingHex;
    int columnIndexOfExplodingHex;

    int tempRowIndex;
    int tempColumnIndex;

    int totalScore;
    int scorePointForBomb;

    public static event Action SwipeSuccesAction = delegate { };
    public static event Action<int> SetTotalScoreAction  = delegate { };

    public void Init(List<List<HexObject>> mainList, HexCreator hexCreator,int columnCount,int rowCount)
    {
        m_mainList = mainList;
        m_rowCount = rowCount;
        m_columnCount = columnCount;
        m_hexCreator = hexCreator;
        scorePointForBomb = GameConstants.scorePointForBomb;

       LeanFingerSwipe.SwipeAction += Swipe;
    }

    private void OnDisable()
    {
        LeanFingerSwipe.SwipeAction -= Swipe;

    }

    void Swipe(bool isRight)
    {
        if (m_selectedGroupList.Count==0)
            return;
     // m_selectedGroupList[0].LogNeighbours(m_mainList);
     StartCoroutine( RotateHexesAndCheckForExploation(m_selectedGroupList,isRight));
    }
    IEnumerator RotateHexesAndCheckForExploation(List<HexObject> hexList,bool isRight)
     {
        for (int j=0;j < 3; j++)
        {
            AssignIndexes(isRight, j, out int nextHexIndex, out int thirdHexIndex);
            RotateHexObjects(hexList,j,nextHexIndex,thirdHexIndex);
            ReAssignMainList(hexList, j, nextHexIndex, thirdHexIndex);
            ReAssignColumnAndRowIndexes(hexList, j, nextHexIndex, thirdHexIndex);
          

            foreach (var hex in hexList)
            FillHexesToDestroyList(hex);

            yield return new WaitForSeconds(0.3f);

            if (Explode())
            {
                SwipeSuccesAction();
                break;
            }
 
        }

    }

    public bool Explode()
    {
        if (hexesTodestroy.Count != 0 && hexesTodestroy[0] != null)
        {
            foreach (List<HexObject> threeObjectList in hexesTodestroy)
                foreach (HexObject hex in threeObjectList)
                {
                    if (removedHexObject.Contains(hex))  //Removed before
                        continue;

                    removedHexObject.Add(hex);
                    m_mainList[hex.ColumnIndex].Remove(hex);

                    rowIndexOfExplodingHex = hex.RowIndex;
                    columnIndexOfExplodingHex = hex.ColumnIndex;

                    m_mainList[columnIndexOfExplodingHex].Add(m_hexCreator.PoolHexObject(hex, DealWithTotalScoreAndCheckForBombCreation()));


                    for (int index = rowIndexOfExplodingHex; index < m_rowCount-1; index++)
                    {
                            if (m_mainList[columnIndexOfExplodingHex][index] != null)
                                m_mainList[columnIndexOfExplodingHex][index].Fall();
               
                    }
               
                }
  
            hexesTodestroy.Clear();
            removedHexObject.Clear();
            return true;
        }
        return false;
    }

    public void FillHexesToDestroyList(HexObject hex)
    {
       List<HexObject> threeObjectListToDestroy = hex.FindNeighboursAndCheckForExpolation();

        if (threeObjectListToDestroy != null)
        {
            hexesTodestroy.Add(threeObjectListToDestroy);
        }

    } 

    private void ReAssignColumnAndRowIndexes(List<HexObject> hexList, int j, int nextHexIndex, int thirdHexIndex)
    {
         tempRowIndex = hexList[j].RowIndex;
         tempColumnIndex = hexList[j].ColumnIndex;

        hexList[j].RowIndex = hexList[nextHexIndex].RowIndex;
        hexList[j].ColumnIndex = hexList[nextHexIndex].ColumnIndex;

        hexList[nextHexIndex].RowIndex = hexList[thirdHexIndex].RowIndex;
        hexList[nextHexIndex].ColumnIndex = hexList[(thirdHexIndex)].ColumnIndex;

        hexList[(thirdHexIndex)].RowIndex = tempRowIndex;
        hexList[(thirdHexIndex)].ColumnIndex = tempColumnIndex;

    }

    private void ReAssignMainList(List<HexObject> hexList, int j, int nextHexIndex, int thirdHexIndex)
    {
        m_mainList[hexList[nextHexIndex].ColumnIndex][hexList[nextHexIndex].RowIndex] = hexList[j];

        m_mainList[hexList[thirdHexIndex].ColumnIndex][hexList[thirdHexIndex].RowIndex] = hexList[nextHexIndex];

        m_mainList[hexList[j].ColumnIndex][hexList[j].RowIndex] = hexList[thirdHexIndex];
    }

    void AssignIndexes(bool isRight, int j, out int nextHexIndex, out int thirdHexIndex)
    {

        if (isRight)
        {
            nextHexIndex = (j + 1) % 3;
            thirdHexIndex = (j + 2) % 3;
        }

        else
        {
            nextHexIndex = (j + 2) % 3;
            thirdHexIndex = (j + 1) % 3;
        }

    }

    void RotateHexObjects(List<HexObject> hexList, int j, int nextHexIndex, int thirdHexIndex)
    {
        LeanTween.move(hexList[j].gameObject, hexList[nextHexIndex].transform.position, 0.3f);
        LeanTween.move(hexList[nextHexIndex].gameObject, hexList[thirdHexIndex].transform.position, 0.3f);
        LeanTween.move(hexList[thirdHexIndex].gameObject, hexList[j].transform.position, 0.3f);

    }
    
    public void DeselectGroupList()
    {
        m_selectedGroupList.Clear();
    }

    public void SelectedGroupList(List<HexObject> selectedGroupList)
    {
        if(m_selectedGroupList!=selectedGroupList)
        m_selectedGroupList.Clear();

        foreach(var item in selectedGroupList)
        m_selectedGroupList.Add(item);
    }

    bool  DealWithTotalScoreAndCheckForBombCreation()
    {
        InceraseTotalScore();
        SetTotalScoreAction(totalScore);
        return CheckForBomb();

    }

    void InceraseTotalScore()
    {
        totalScore += 5;
    }


    bool CheckForBomb()
    {
        return totalScore % scorePointForBomb == 0;
    }
}
