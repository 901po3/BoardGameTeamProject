using System.Collections;
using UnityEngine;

public class unit : MonoBehaviour   {

    private int type; //double check
    public bool touchable = true;
    public Point index;
    Vector2 size;
    public bool isAlive = true;
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

    public void SetType(General.TileType _type)
    {
        type = (int)_type;
    }
    public General.TileType GetType()
    {
        return (General.TileType)type;
    }
    public int GetTypeInt()
    {
        if (General.TileType.FIRE == (General.TileType)type) return 0;
        else if (General.TileType.WATER == (General.TileType)type) return 1;
        else if (General.TileType.ROCK == (General.TileType)type) return 2;
        else if (General.TileType.GRASS == (General.TileType)type) return 3;
        else return -1;
    }
}
