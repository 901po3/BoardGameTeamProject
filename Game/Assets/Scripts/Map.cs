using System.Collections;
using UnityEngine;

/*I made 3d array and told why i used it, but as i already mentioned this method will cause memory lick.
  Since we are using small map, it won't cause big trouble, but still it's not good coding style.
  And there is better and simple way to replace this method.
  We can simply make a structure for unit that has variables store coordinate of map they are currently standing on.
  Since we only have 8 players, we can reduce memory usage significantly.
  I won't fix any code, it's up to you what code you are gona implement for this game.
  */

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