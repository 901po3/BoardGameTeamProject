using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class unit : MonoBehaviour   {

    public int type; //double check
    public int indexX;
    public int indexY;
    Vector2 size;
    public float offsetY = 1.5f;
    public float offsetX = 0.2f;
    public  bool selected = false; //if character is selected 

    private void Awake()
    {
        for (int i = 0; i < 4; i++)
        {
            size = GetComponent<SpriteRenderer>().transform.localScale;
            size = new Vector2(size.x * 0.88f, size.y * 0.88f);

            GetComponent<SpriteRenderer>().transform.localScale = size;
        }
    }
    
}
