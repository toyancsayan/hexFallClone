using System.Collections.Generic;
using UnityEngine;

public class MarkerObject : MonoBehaviour
{

    MarkerManager m_markerManager;
    HexObject m_parentHex;
    HexManager m_hexmanager;

    bool m_isUp;
    bool m_isOdd;
    bool isActive;

    List<HexObject> selectedHexObject = new List<HexObject>();
    List<HexObject> neighbourList = new List<HexObject>();
    List<List<HexObject>> m_mainList;

    int m_rowCount;
    int m_columnCount;

    int parentHexRowIndex;
    int parentHexColumnIndex;

    public void Init(MarkerManager markerManager, bool isUp,HexObject parentHex,int rowCount,int columnCount, HexManager hexManager, List<List<HexObject>> mainList)
    {
        m_markerManager = markerManager;
        m_isUp = isUp;
        m_parentHex = parentHex;
        m_mainList = mainList;
        m_hexmanager = hexManager;
        m_rowCount = rowCount;
        m_columnCount = columnCount;

        m_isUp = isUp;

        HexManager.SwipeSuccesAction += DeSelect;
        DecideIsActive();
    }

    private void OnDisable()
    {
        HexManager.SwipeSuccesAction -= DeSelect;
    }
    public void DecideIsActive()
    {
        parentHexColumnIndex = m_parentHex.ColumnIndex;
        parentHexRowIndex = m_parentHex.RowIndex;

        m_isOdd = m_parentHex.IsOdd;

        isActive = true;
        

        if ((parentHexRowIndex == m_rowCount - 1 && !m_isOdd) ||
            (parentHexColumnIndex == m_columnCount - 1) || 
            (parentHexRowIndex == 0 && m_isOdd && !m_isUp) || 
            (parentHexRowIndex == m_rowCount - 1 && m_isOdd && m_isUp))
            isActive = false;

        gameObject.SetActive(isActive);

    }



    public void Selected()
    {
        m_markerManager.Selected(this);
        selectedHexObject.Add(m_parentHex);


        m_parentHex.FindNeighbours(neighbourList);


        //If isUP select first and second neighbour
        //else select second and third neighbour

        int startInt = 0;
        if (!m_isUp)
             startInt = 1;


        for (int i = startInt; i < startInt+2; i++)
            selectedHexObject.Add(neighbourList[i]);


        m_hexmanager.SelectedGroupList(selectedHexObject);
        selectedHexObject.Clear();
        neighbourList.Clear();

    }

    void DeSelect()
    {
        m_hexmanager.DeselectGroupList();

    }


    public bool IsUp { get => m_isUp; }
}
