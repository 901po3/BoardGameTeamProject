using System.Collections;
using UnityEngine;

enum tileType { FIRE, ICE, ROCK, SAND };

public class Map : MonoBehaviour
{
    const int typeNum = 4;

    Vector2 twoDToIso(Vector2 pos) //funtion that change 2d array to isometric version
    {
        pos.x = pos.x / 2 - pos.y / 2;
        pos.y = pos.x / 2 + pos.y / 2;
        return pos;
    }


    GameObject[,] map;

    public GameObject TempTile; //tempTile
    public Sprite[] tileTypes; //We are going to use sprite and put it in tempTile.

    int mapSizeX = 8; 
    int mapSizeY = 8;
    float depth = 0;

    private void Start()
    {
        GenerateMapVisual();
    }

    private void Update()
    {
        RandomizeMap();
    }

    void GenerateMapVisual()
    {
        map = new GameObject[mapSizeX, mapSizeY];

        for (int x = 0; x < mapSizeX; x++)
        {
            for (int y = 0; y < mapSizeY; y++)
            {
                Vector2 tpos = new Vector2(x, y);
                tpos = twoDToIso(tpos);

                map[x,y] = (GameObject)Instantiate(TempTile, new Vector3(tpos.x, tpos.y, depth), Quaternion.identity);
                depth += 0.1f;
            }

        }
    }

    void RandomizeMap()
    {
        for (int x = 1; x < mapSizeX - 1; x++)
        {
            for (int y = 0; y < mapSizeY; y++)
            {
                int rand = Random.Range(0, typeNum);
                map[x, y].GetComponent<SpriteRenderer>().sprite = tileTypes[rand];
            }
        }
    }
}
