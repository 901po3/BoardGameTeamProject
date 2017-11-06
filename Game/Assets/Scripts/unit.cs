using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class unit : MonoBehaviour   {

    public int type; //double check
    public int posX; //character position
    public int posY;
    Vector2 size;
    Vector2 offSet;
    public Sprite[] image; //character image
    private Vector2 mouseOver;
    public  bool selected = false; //if character is selected 

    private void Awake()
    {
        size = GetComponent<SpriteRenderer>().sprite.rect.size;
        offSet = new Vector2(size.x / 2, size.y);
    }
    
}
