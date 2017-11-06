using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum  tileType { FIRE, ICE, ROCK, SAND };

public class General : MonoBehaviour {
    private static General m_Instance;
    
    private void Awake()
    {
        m_Instance = this;
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
