using System.Collections;
using UnityEngine;


public class Map : MonoBehaviour 
{
    private static Map m_Instance; //singleton
    public int typeNum = 4; //max number of tile kinds
    public GameObject[,] map; //Array contains tiles.
    public GameObject TempTile; //tempTile
    public Sprite[] tileTypes; //We are going to use sprite and put it in tempTile.
    public Sprite[] backgroundSprite;
    public Sprite pondSprite;
    public int mapSizeX;
    public int mapSizeY;
    float depth = 0;
    public float tileSize;

    public GameObject gafas;
    
    public static Map Instance //use this to call singleton
    {
        get { return m_Instance; }
    }
    
    private void Start()
    {
        m_Instance = this;
        GenerateMapVisual();
        GenerateBackground();
    }

    private void Update()
    {
      // RandomizeMap();
    }

    private void OnDestroy()
    {
        m_Instance = null;
    }

    void GenerateMapVisual()
    {
        map = new GameObject[mapSizeY, mapSizeX];
        float x, y;
        x = 0;
        y = 0;
        for (int  i= 0; i < mapSizeX; i++)
        {
            for (int j = 0; j < mapSizeY; j++)
            {
                x = i * tileSize;
                y = j * tileSize;
                Vector2 tpos = new Vector2(y, x);
                tpos = General.Instance.twoDToIso(tpos);

                map[j, i] = (GameObject)Instantiate(TempTile, new Vector3(tpos.x, tpos.y + Random.Range(-0.12f, 0.12f), depth), Quaternion.identity);
                if (i != 0 && i != mapSizeX - 1)
                {
                    int rand = Random.Range(0, typeNum);
                    map[j, i].GetComponent<SpriteRenderer>().sprite = tileTypes[rand];
                }
                else
                    map[j, i].GetComponent<SpriteRenderer>().sprite = pondSprite;
                map[j, i].transform.localScale = new Vector3(tileSize, tileSize, tileSize);
                map[j, i].GetComponent<IsoTile>().indexX = j;
                map[j, i].GetComponent<IsoTile>().indexY = i;
                depth += 0.1f;
            }
        }
    }

    void GenerateBackground()
    { 
        GameObject gob;
        float x, y;
        float temp;
        for (int k = 1; k < 20; k++)
        {
            temp = depth = -20 + k * 5;
            x = 0;
            y = 0;
            for (int i = 0; i < mapSizeX + 2; i++)
            {
                for (int j = 0; j < mapSizeY + 2; j++)
                {
                    if (j == 0 || i == 0 || ((j == mapSizeY + 1 || i == mapSizeX + 1) && k == 1))
                    {
                        x = i * tileSize - tileSize * k - 0.3f;
                        y = j * tileSize - (tileSize + 0.1f) * k - 0.45f;
                        Vector2 tpos = new Vector2(y, x);
                        tpos = General.Instance.twoDToIso(tpos);

                        if (j == 0 || i == 0)
                            temp -= 256;
                        gob = Instantiate(TempTile, new Vector3(tpos.x + Random.Range(-0.3f, 0.3f), tpos.y + Random.Range(-0.12f, 0.12f), temp), Quaternion.identity);

                        int rand = Random.Range(0, 3);
                        gob.GetComponent<SpriteRenderer>().sprite = backgroundSprite[rand];
                        gob.transform.localScale = new Vector3(tileSize, tileSize, tileSize);

                        temp = depth += 0.1f + 5;
                    }

                }
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