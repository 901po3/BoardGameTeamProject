using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UnitControl : MonoBehaviour
{
    public Sprite[] unitSpriteArray;
    int unitNum; // The total number of units
    int selectedUnit; // The selected units number
    GameObject[] unitArray; // An array storing all 8 units
    int mouseOverX; // X position of mouse (used for tile position)
    int mouseOverY; // y position of mouse (used for tile position)
    float offsetY;
    int turn = 1;
    public int tileType; //Type of tile (Fire,Ice,Rock,Sand)
    public bool isFirstClick = false; //check if the player selected unit
    public bool isSecondClick = false; //check if player selects new tile and if it matches unit type
    public GameObject UnitPrefab;
    bool move = false;
    public float speed = 1000;
    public int prevX, prevY;
    bool resulfOfFistClick = false;
    // Use this for initialization
    void Start()
    {
        prevX = prevY = -1;
        offsetY = Map.Instance.tileSize / 1.2f;
        initUnit();
    }

    // Update is called once per frame
    void Update()
    {
        bool check = true;
        if (Input.GetMouseButtonDown(0) && check)
        {
            check = false;

            if (!isFirstClick)
                FirstClick();
            else if (isFirstClick && !isSecondClick)
                CheckTile();
        }

        if (Input.GetMouseButtonUp(0))
        {
            check = true;
        }

    }

    void initUnit() // Generate initial unit
    {
        int distance = 2;
        int startPointX = 1;
        int indexX = 0;
        int indexY = 0; 
        unitNum = 8; //# of Player units
        unitArray = new GameObject[unitNum];

        for (int i = 0; i < unitNum; i++)
        {
            unitArray[i] = Instantiate(UnitPrefab, new Vector3(0, 0, -10), Quaternion.identity);
            unitArray[i].GetComponent<SpriteRenderer>().sprite = unitSpriteArray[i];

            switch(i)
            {
                case 0:
                case 4:
                    unitArray[i].GetComponent<unit>().type = 0;
                    break;
                case 1:
                case 5:
                    unitArray[i].GetComponent<unit>().type = 1;
                    break;
                case 2:
                case 6:
                    unitArray[i].GetComponent<unit>().type = 2;
                    break;
                case 3:
                case 7:
                    unitArray[i].GetComponent<unit>().type = 3;
                    break;
            }
            if (i < unitNum / 2)
            {
                indexX = unitArray[i].GetComponent<unit>().indexX = startPointX;
                indexY = unitArray[i].GetComponent<unit>().indexY = 0;
                startPointX += distance;
                if (i == unitNum / 2 - 1)
                    startPointX = 1;
            }
            if (i >= unitNum / 2)
            {
                indexX = unitArray[i].GetComponent<unit>().indexX = startPointX;
                indexY = unitArray[i].GetComponent<unit>().indexY = Map.Instance.mapSizeX - 1;
                startPointX += distance;
            }
            unitArray[i].transform.position =
                new Vector3(Map.Instance.map[indexX, indexY].transform.position.x,
                Map.Instance.map[indexX, indexY].transform.position.y + offsetY,
                Map.Instance.map[indexX, indexY].transform.position.z - 0.1f);

        }
    }

    public bool SelectUnit(int turn) //returns true if unit is player 
    {
        Vector2 unitRay = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D unitHit = Physics2D.Raycast(unitRay, Vector2.zero);

        if (unitHit.collider.tag != "Unit")
        {
            mouseOverY = mouseOverX = -1;
            return false;
        }

        if (unitHit.collider.tag == "Unit") //If unit selected
        {
            unitHit.transform.gameObject.GetComponent<unit>().selected = true;

            for (int i = 0; i < unitNum; i++)
            {
                if (unitArray[i].GetComponent<unit>().selected)
                {
                    unitHit.transform.gameObject.GetComponent<unit>().selected = false;
                    Debug.Log(i + " Unit is clicked");
                    selectedUnit = i;
                    return true;
                }
            }
        }
        return false;
    }

    private void CheckTile() // check tile type to compare to player type
    {
        Vector2 cubeRay = Camera.main.ScreenToWorldPoint(Input.mousePosition); // gets position of mouse and stores it in a Vector2 
        RaycastHit2D cubeHit = Physics2D.Raycast(cubeRay, Vector2.zero); // Uses a Raycast cast from mouse position to the scene??? I think LOL
        int playerXIndex = unitArray[selectedUnit].GetComponent<unit>().indexX; //stores the x position of the unit
        int PlayerYIndex = unitArray[selectedUnit].GetComponent<unit>().indexY; //stores the y position of the unit

        if (cubeHit.transform.tag == "Map")  //checks if the object hit has a map tag
        {
            Vector3 mosPos = cubeHit.transform.position; //returns the position of Gameobject hit by Raycast

            mouseOverX = cubeHit.transform.GetComponent<IsoTile>().indexX; //gets the indexX from IsoTile script attached to tile prefab for directory
            mouseOverY = cubeHit.transform.GetComponent<IsoTile>().indexY; //gets the indexY from IsoTile script attached to tile prefab for directory
        }
        else if (cubeHit.transform.tag == "Unit")
        {
            FirstClick();
            return;
        }
        else //no tile clicked
        {
            mouseOverX = mouseOverX = -1;
        }

        tileType = Map.Instance.GetType(mouseOverX, mouseOverY); //returns the x,y position of the tile type

        if ((playerXIndex + 1 == mouseOverX && PlayerYIndex == mouseOverY) ||    //Checks if the tile selected is directly beside the unit
            (playerXIndex - 1 == mouseOverX && PlayerYIndex == mouseOverY) ||
            (playerXIndex == mouseOverX && PlayerYIndex + 1 == mouseOverY) ||
            (playerXIndex == mouseOverX && PlayerYIndex - 1 == mouseOverY))
        {
            if (tileType == unitArray[selectedUnit].GetComponent<unit>().type)  //should be added after randomization map is done.
            {
                 if (prevX == mouseOverX && prevY == mouseOverY)
                 {
                     isSecondClick = true;
                     MoveUnit();
                     Debug.Log("Tile is clicked second times");
                     return;
                 }
                 prevX = mouseOverX;
                 prevY = mouseOverY;
                 Debug.Log("Tile is clicked");
                 return;
            }
        }
        isSecondClick = false;
    }

    private void MoveUnit() //translate unit position and change Sprite
    {
        GameObject tilePos = Map.Instance.map[mouseOverX, mouseOverY];
        float destPosX = tilePos.transform.position.x;
        float destPosY = tilePos.transform.position.y;
        float destPosZ = tilePos.transform.position.z;

        int curIndexX = unitArray[selectedUnit].GetComponent<unit>().indexX = mouseOverX;
        int curIndexY = unitArray[selectedUnit].GetComponent<unit>().indexY = mouseOverY;

        unitArray[selectedUnit].transform.position =
                new Vector3(destPosX, destPosY + offsetY, destPosZ - 0.1f);
        isFirstClick = false;
        isSecondClick = false;

        if(turn == 1)
            turn = 2;
        else
            turn = 1;
    }

    private void FirstClick() //firstClick returns the unit number and turns firstClick to true to allow secondClick
    {
         resulfOfFistClick = SelectUnit(turn);
    }

    public void UnitButtonPush()
    {
        if(resulfOfFistClick)
        {
            isFirstClick = true;
        }
    }
}
