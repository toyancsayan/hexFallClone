using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "LevelProperties", menuName = "ScriptableObjects/LevelProperties", order = 1)]
public class LevelProperties_SO : ScriptableObject
{
    [SerializeField]
    private int rowCount;
    [SerializeField]
    private int columnCount;
    [SerializeField]
    private int maxColorCount;
    [SerializeField]
    List<Color> colorList;
    [SerializeField]
    GameObject hexPrefab;


    //!! SOME VALUES ARE CLAMPED

    public int RowCount { get => Mathf.Clamp(rowCount,5,GameConstants.defaultRowCount); set => rowCount = value; }
    public int ColumnCount { get => Mathf.Clamp(columnCount, 5, GameConstants.defaultColumnCount); set => columnCount = value; }
    public int MaxColorCount { get => maxColorCount; set => maxColorCount = value; }
    public List<Color> ColorList { get => colorList;}
    public GameObject HexPrefab { get => hexPrefab; }

    public Color GenerateRandomColor()
    {
      return colorList[Random.Range(0,  maxColorCount)];
        
    }
}
