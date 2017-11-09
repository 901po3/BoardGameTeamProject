using System.Collections;
using UnityEngine;


public class Map : MonoBehaviour 
{
    private static Map m_Instance; //singleton
    public int typeNum = 4; //max number of tile kinds
    public GameObject[,] map; //Array contains tiles.
    public GameObject TempTile; //tempTile
    public Sprite[] tileTypes; //We are going to use sprite and put it in tempTile.
    public int mapSizeX;
    public int mapSizeY;
    float depth = 0;

    
    public static Map Instance //use this to call singleton
    {
        get { return m_Instance; }
    }
    
    private void Start()
    {
        m_Instance = this;
        GenerateMapVisual();
    }

    private void Update()
    {
        RandomizeMap();
    }

    private void OnDestroy()
    {
        m_Instance = null;
    }

    void GenerateMapVisual()
    {
        map = new GameObject[mapSizeY, mapSizeX];

        for (int  i= 0; i < mapSizeX; i++)
        {
            for (int j = 0; j < mapSizeY; j++)
            {
                Vector2 tpos = new Vector2(j, i);
                tpos = General.Instance.twoDToIso(tpos);

                map[j, i] = (GameObject)Instantiate(TempTile, new Vector3(tpos.x, tpos.y, depth), Quaternion.identity);
                map[j, i].GetComponent<IsoTile>().indexX = j;
                map[j, i].GetComponent<IsoTile>().indexY = i;
                depth += 0.1f;
            }
        }
    }

    public int GetType(int x, int y) 
    {
        Sprite tempSprite = map[x, y].GetComponent<SpriteRenderer>().sprite;
        if (tileTypes[0] == tempSprite)
            return 0;
        else if (tileTypes[1] == tempSprite)
            return 1;
        else if (tileTypes[2] == tempSprite)
            return 2;
        else
            return 3;
    }

    void RandomizeMap()
    {
        for (int x = 0; x < mapSizeY; x++)
        {
            for (int y = 1; y < mapSizeX - 1; y++)
            {
                int rand = Random.Range(0, typeNum);
                map[x, y].GetComponent<SpriteRenderer>().sprite = tileTypes[rand];
            }
        }
    }
} 