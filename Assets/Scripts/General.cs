using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Point { public int x, y; }

public class General : MonoBehaviour {

    private static General m_Instance;
    public enum TileType { FIRE = 0, WATER, ROCK, GRASS, NOTHING = -1};
    public enum Direction { UP, DOWN, RIGHT, LEFT};

    private void Awake()
    {
        m_Instance = this;
    }

    public int GetOpponentIndex(int turn, int currentIndex)
    {
        if (turn == 1)
            return currentIndex + 4;
        else if (turn == 2)
            return currentIndex - 4;
        return -1;
    }

    public void GetFromAndToIndex(int turn, int unitNum, ref int from, ref int to) // get i and maxValue for the forloop.
    {
        if (turn == 1)
        {
            from = 0;
            to = unitNum / 2;
        }
        else if (turn == 2)
        {
            from = unitNum / 2;
            to = unitNum;
        }
    }
   
    public static General Instance
    {
        get { return m_Instance; }
    }

    private void OnDestroy()
    {
        m_Instance = null;
    }

    public Vector2 twoDToIso(Vector2 pos) //funtion that change 2d array to isometric version
    {
        pos.x = pos.x / 2 - pos.y / 2;
        pos.y = pos.x / 2 + pos.y / 2;
        return pos;
    }
}
