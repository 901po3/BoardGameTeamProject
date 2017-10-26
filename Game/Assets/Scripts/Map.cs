using System.Collections;
using UnityEngine;

public class Map : MonoBehaviour 
{
    private static Map m_Instance; //singleton
    public int typeNum = 4; //max number of tile kinds
    public GameObject[,,] map; //Array contains tiles.
    public GameObject TempTile; //tempTile
    public Sprite[] tileTypes; //We are going to use sprite and put it in tempTile.
    public int mapSizeX = 9;
    public int mapSizeY = 7;
    float depth = 0;

    public static Map Instance //use this to call singleton
    {
        get { return m_Instance; }
    }

    private void Awake()
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
        map = new GameObject[mapSizeX, mapSizeY, 2];

        for (int x = 0; x < mapSizeX; x++)
        {
            for (int y = 0; y < mapSizeY; y++)
            {
                Vector2 tpos = new Vector2(x, y);
                tpos = General.Instance.twoDToIso(tpos);

                map[x, y, 0] = (GameObject)Instantiate(TempTile, new Vector3(tpos.x, tpos.y, depth), Quaternion.identity);
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
                map[x, y, 0].GetComponent<SpriteRenderer>().sprite = tileTypes[rand];
            }
        }
    }
}