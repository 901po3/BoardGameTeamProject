using System.Collections;
using UnityEngine;

public class Map : MonoBehaviour
{
    private static Map m_Instance; //singleton
    public int typeNum = 4; //max number of tile kinds
    public GameObject[,] map; //Array contains tiles.
    public GameObject TempTile; //tempTile
    public Sprite[] tileTypes; //We are going to use sprite and put it in tempTile.
    public Sprite pawnSprite;
    public Vector2 offset;
    public int mapSizeX;
    public int mapSizeY;
    float depth = 0;
    public float tileSize;
    public bool[] areTilesOut;
    public float heightDrop;
    public bool isMapReady = false;
    public bool isPawnReadyToDrop = false;
    public float tileDropSpeed;

    public GameObject gafas;

    public static Map Instance //use this to call singleton
    {
        get { return m_Instance; }
    }

    private void Awake()
    {
        m_Instance = this;
        offset.x = 0;
        offset.y = tileSize / 1.2f + 0.05f;
    }

    private void Start()
    {
        GenerateMap();
        //GenerateBackground();
    }

    private void Update()
    {
        DropTiles();
        DropPondTiles();
    }

    private void OnDestroy()
    {
        m_Instance = null;
    }

    void GenerateMap()
    {
        map = new GameObject[mapSizeY, mapSizeX];
        areTilesOut = new bool[typeNum];
        for (int i = 0; i < typeNum; i++) areTilesOut[i] = true;
        float x, y;
        x = 0;
        y = 0;
        for (int i = 0; i < mapSizeX; i++)
        {
            for (int j = 0; j < mapSizeY; j++)
            {
                x = i * tileSize;
                y = j * tileSize;
                Vector2 tpos = new Vector2(y, x);
                tpos = General.Instance.twoDToIso(tpos);

                float originalY = tpos.y + Random.Range(-0.12f, 0.12f);
                float dropPosY = tpos.y + heightDrop;

                map[j, i] = (GameObject)Instantiate(TempTile, new Vector3(tpos.x, dropPosY, depth), Quaternion.identity);
                if (i != 0 && i != mapSizeX - 1)
                    map[j, i].GetComponent<SpriteRenderer>().sprite = tileTypes[4];
                else
                    map[j, i].GetComponent<SpriteRenderer>().sprite = pawnSprite;
                map[j, i].transform.localScale = new Vector3(tileSize, tileSize, tileSize);
                map[j, i].GetComponent<IsoTile>().index.x = j;
                map[j, i].GetComponent<IsoTile>().index.y = i;
                map[j, i].GetComponent<IsoTile>().originalY = originalY;
                map[j, i].GetComponent<IsoTile>().dropPosY = dropPosY;
                depth += 0.1f;
            }
        }
        depth = 0f;
    }

    void DropTiles()
    {
        if (!isPawnReadyToDrop && !isMapReady)
        {

            float originalY;
            float dropPosY;

            for (int i = 1; i < mapSizeX - 1; i++)
            {
                for (int j = 0; j < mapSizeY; j++)
                {
                    dropPosY = map[j, i].GetComponent<IsoTile>().dropPosY;
                    originalY = map[j, i].GetComponent<IsoTile>().originalY;

                    if (dropPosY - originalY < 0.3f)
                    {
                        dropPosY = originalY;
                        map[j, i].transform.position =
                        new Vector3(map[j, i].transform.position.x, dropPosY, map[j, i].transform.position.z);
                        if (j == mapSizeY - 1 && i == mapSizeX - 2)
                        {
                            map[j, i].GetComponent<IsoTile>().isSet = true;
                            if (map[j, i].GetComponent<IsoTile>().isSet)
                            {
                                isPawnReadyToDrop = true;
                                return;
                            }
                        }
                        continue;
                    }

                    dropPosY -= map[j, i].GetComponent<IsoTile>().speed;
                    map[j, i].GetComponent<IsoTile>().dropPosY = dropPosY;

                    map[j, i].transform.position =
                        new Vector3(map[j, i].transform.position.x, dropPosY, map[j, i].transform.position.z);
                }
            }
        }
    }

    void DropPondTiles()
    {
        if (isPawnReadyToDrop)
        {
            float originalY;
            float dropPosY;
            int i = 0;
            while (i < mapSizeX)
            {
                for (int j = 0; j < mapSizeY; j++)
                {
                    dropPosY = map[j, i].GetComponent<IsoTile>().dropPosY;
                    originalY = map[j, i].GetComponent<IsoTile>().originalY;

                    if (dropPosY - originalY < 0.4f)
                    {
                        dropPosY = originalY;
                        map[j, i].transform.position =
                        new Vector3(map[j, i].transform.position.x, dropPosY, map[j, i].transform.position.z);
                        if (j == mapSizeY - 1 && i == mapSizeX - 1)
                        {
                            map[j, i].GetComponent<IsoTile>().isSet = true;
                            if (map[j, i].GetComponent<IsoTile>().isSet)
                            {
                                StartCoroutine(SetBoolean(isMapReady));
                            }
                        }
                        continue;
                    }
                    dropPosY -= map[j, i].GetComponent<IsoTile>().speed;
                    map[j, i].GetComponent<IsoTile>().dropPosY = dropPosY;

                    map[j, i].transform.position =
                        new Vector3(map[j, i].transform.position.x, dropPosY, map[j, i].transform.position.z);
                }
                i += mapSizeX - 1;
            }
        }
        isPawnReadyToDrop = false;
    }

    IEnumerator SetBoolean(bool boo)
    {
        yield return new WaitForSeconds(0.65f);
        isMapReady = true;
    }



    /*void GenerateBackground()
    { 
        GameObject gob;
        float x, y;
        float temp;
        depth = 50f;
        for (int i = 0; i < mapSizeX ; i++)
        {
            for (int j = 0; j < mapSizeY; j++)
            {
                x = i * tileSize;
                y = j * tileSize;
                Vector2 tpos = new Vector2(y, x);
                tpos = General.Instance.twoDToIso(tpos);

                gob = Instantiate(TempTile, new Vector3(tpos.x, tpos.y - 0.75f + Random.Range(-0.12f, 0.12f), depth), Quaternion.identity);

                int rand = Random.Range(0, 3);
                gob.GetComponent<SpriteRenderer>().sprite = backgroundSprite[rand];
                depth += 0.1f;
            }
        }
        depth = 0f;
        for (int k = 1; k < 20; k++)
        {
            temp = depth = -20 + k * 5;
            x = 0;
            y = 0;
            for (int i = 0; i < mapSizeX + 2; i++)
            {
                for (int j = 0; j < mapSizeY + 2; j++)
                {
                    if (j == 0 || i == 0 || ((j == mapSizeY + 1 || i == mapSizeX + 1)) && k == 1)
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
    } */

    public General.TileType GetType(Point p) 
    {
        Sprite tempSprite = map[p.x, p.y].GetComponent<SpriteRenderer>().sprite;
        if (tileTypes[(int)General.TileType.FIRE] == tempSprite)
            return General.TileType.FIRE;
        else if (tileTypes[(int)General.TileType.WATER] == tempSprite)
            return General.TileType.WATER;
        else if (tileTypes[(int)General.TileType.ROCK] == tempSprite)
            return General.TileType.ROCK;
        else if (tileTypes[(int)General.TileType.GRASS] == tempSprite)
            return General.TileType.GRASS;
        else
            return General.TileType.NOTHING;
    }

    public General.TileType GetType(int x, int y)
    {
        Point p;
        p.x = x;
        p.y = y;
        return GetType(p);
    }

    public Sprite GetSpriteOfUnit(General.TileType type)
    {
        if (type == General.TileType.FIRE) //fire
            return tileTypes[(int)General.TileType.FIRE];
        else if (type == General.TileType.WATER)
            return tileTypes[(int)General.TileType.WATER];
        else if (type == General.TileType.ROCK)
            return tileTypes[(int)General.TileType.ROCK];
        else if (type == General.TileType.GRASS)
            return tileTypes[(int)General.TileType.GRASS];
        else
            return null;
    }

    public void RandomizeMap()
    {
        int turn = UnitControl.Instance.turn;
        GameObject[] unitArray = UnitControl.Instance.unitArray;

        int from = 0;
        int to = 0;

        for (int x = 0; x < mapSizeY; x++)
        {
            for (int y = 1; y < mapSizeX - 1; y++)
            {
                int rand = Random.Range(0, typeNum);
                map[x, y].GetComponent<SpriteRenderer>().sprite = tileTypes[rand];
            }
        }

        General.Instance.GetFromAndToIndex(turn, typeNum * 2, ref from, ref to);

        for (int i = from; i < to; i++)
        {
            Point point = unitArray[i].GetComponent<unit>().index;
            General.TileType type = unitArray[i].GetComponent<unit>().GetType();
            Point tempPoint;

            while (true)
            {
                tempPoint = point;
                int rand = Random.Range(0, typeNum);

                switch (rand)
                {
                    case (int)General.Direction.UP:
                        tempPoint.y -= 1;
                        break;
                    case (int)General.Direction.DOWN:
                        tempPoint.y += 1;
                        break;
                    case (int)General.Direction.RIGHT:
                        tempPoint.x += 1;
                        break;
                    case (int)General.Direction.LEFT:
                        tempPoint.x -= 1;
                        break;
                    default:
                        break;
                }

                if (tempPoint.y > 0 && tempPoint.y < mapSizeX -1)
                {
                    if(tempPoint.x >= 0 && tempPoint.x < mapSizeY)
                        break;
                }
            }
            map[tempPoint.x, tempPoint.y].GetComponent<SpriteRenderer>().sprite = GetSpriteOfUnit(type);
        }
    }
} 