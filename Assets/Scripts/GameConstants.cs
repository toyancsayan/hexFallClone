using UnityEngine;



public class GameConstants
{

    public static float xOffset = 0.7f;
    public static float yOffset = 0.73f;
    public static float yOffsetCorrection = 0.4f;
    public static int defaultColumnCount = 8;
    public static int defaultRowCount = 9;
    public static int minColumnCount = 5;
    public static int minRowCount = 5;

    //TO MAKE GRID ON MIDDLE OF THE SCREEN
    public static float aCoefficientForX = -0.4f;
    public static float bCoefficientForX = 0.82f;
    public static float aCoefficientForY = -0.275f;
    public static float bCoefficientForY = 0.745f;



    public static Vector2[] even_neighbourIndexes = { new Vector2(0,1),
                                                     new Vector2(1,1),
                                                     new Vector2(1,0),
                                                     new Vector2(0,-1),
                                                     new Vector2(-1,0),
                                                     new Vector2(-1,1) };

    public static Vector2[] odd_neighbourIndexes = { new Vector2(0,1),
                                                     new Vector2(1,0),
                                                     new Vector2(1,-1),
                                                     new Vector2(0,-1),
                                                     new Vector2(-1,-1),
                                                     new Vector2(-1,0) };

    public static int scorePointForBomb = 50;
}
