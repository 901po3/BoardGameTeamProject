using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UnitControl : MonoBehaviour
{

    int unitNum; // The total number of units
    int selectedUnit; // The selected units number
    GameObject[] unitArray; // An array storing all 8 units
    int mouseOverX; // X position of mouse (used for tile position)
    int mouseOverY; // y position of mouse (used for tile position)
    int playerOverX;
    int playerOverY;
    int unitPosX = 0;
    int unitPosY = 0;
    int turn = 1; // 0=Player 1, 1=Player 2
    int tileType; //Type of tile (Fire,Ice,Rock,Sand)
    bool isFirstClick = false; //check if the player selected unit
    bool isSecondClick = false; //check if player selects new tile and if it matches unit type
    public GameObject UnitPrefab;


    // Use this for initialization
    void Start() {
        initUnit();
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
                new Vector3( Map.Instance.map[indexX, indexY].transform.position.x +0.11f,
                Map.Instance.map[indexX, indexY].transform.position.y +0.9f, -10);

        }
    }

    // Update is called once per frame
    void Update() {
        bool check = false;
        if (Input.GetMouseButtonDown(0))
        {
            //selectTile();
            if (!check)
            {
                if(!isFirstClick)
                    FirstClick();
                if (isFirstClick)
                    SecondClick();
            }
              
            check = true;      
        }
 
        if(Input.GetMouseButtonUp(0))
        {
            check = false;
        }

    }


    public void selectTile() //update position of tile
    {

        Vector2 cubeRay = Camera.main.ScreenToWorldPoint(Input.mousePosition); // gets position of mouse and stores it in a Vector2 
        RaycastHit2D cubeHit = Physics2D.Raycast(cubeRay, Vector2.zero); // Uses a Raycast cast from mouse position to the scene??? I think LOL
            
        if (cubeHit.transform.tag == "Map")  //checks if the object hit has a map tag
        {

            Vector3 mosPos = cubeHit.transform.position; //returns the position of Gameobject hit by Raycast

            playerOverX = cubeHit.transform.GetComponent<IsoTile>().indexX; //gets the indexX from IsoTile script attached to tile prefab for directory
            playerOverY = cubeHit.transform.GetComponent<IsoTile>().indexY; //gets the indexY from IsoTile script attached to tile prefab for directory
          
            
            Debug.Log(mosPos); //for testing
            Debug.Log(mouseOverX); //for testing
            Debug.Log(mouseOverY); //for testing
            
        }
        else //no tile clicked
        {
            mouseOverX = mouseOverY = -1;
        }
    }

    public bool SelectUnit(int turn) //returns true if unit is player 
    {
        Vector2 unitRay = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D unitHit = Physics2D.Raycast(unitRay, Vector2.zero);

        if (unitHit.collider.tag != "Unit")
        {
            mouseOverY = mouseOverX = -1;
            isFirstClick = false; // assigns false if the if nothing detected by Raycast
            return false;
        }


        int from; //0-3 is player 1 
        int to; //4-7 is player 2 
        if (turn == 1) 
        {
            from = 0;
            to = 4;
        }
        else
        {
            from = 4;
            to = 8;
        }
   
        if (unitHit.collider.tag == "Unit") //If unit selected
        {
        unitHit.transform.gameObject.GetComponent<unit>().selected = true; 

            for (int i = from; i < to; i++)
            {
                if (unitArray[i].GetComponent<unit>().selected)
                {
                    unitHit.transform.gameObject.GetComponent<unit>().selected = false;
                    Debug.Log(i + " Unit is clicked"); 
                    selectedUnit = i;
                    isFirstClick = true;
                    return true;
                }
            }
        }

        isFirstClick = false; // assigns false if the if nothing detected by Raycast
        return false;
    }

    private void CheckTile() // check tile type to compare to player type
    {

        float playerXIndex = unitArray[selectedUnit].GetComponent<unit>().indexX; //stores the x position of the unit
        float PlayerYIndex = unitArray[selectedUnit].GetComponent<unit>().indexY; //stores the y position of the unit


        tileType = Map.Instance.GetType(playerOverX, playerOverY); //returns the x,y position of the tile type

        //if(SelectUnit(turn))
        //    FirstClick();

        if ((playerXIndex + 1 == mouseOverX && PlayerYIndex == mouseOverY) ||    //Checks if the tile selected is directly beside the unit
            (playerXIndex - 1 == mouseOverX && PlayerYIndex == mouseOverY) ||
            (playerXIndex == mouseOverX && PlayerYIndex + 1 == mouseOverY) ||
            (playerXIndex == mouseOverX && PlayerYIndex - 1 == mouseOverY))      
        {
            if (tileType == unitArray[selectedUnit].GetComponent<unit>().type) 
            {
                isSecondClick = true;
                Debug.Log("Tile is clicked");
                return;
            }
        }
        isFirstClick = false;
        isSecondClick = false;
    }

    private void MoveUnit() //translate unit position and change Sprite
    {
        
    }

    private void FirstClick() //firstClick returns the unit number and turns firstClick to true to allow secondClick
    {
        isFirstClick = true;
        SelectUnit(turn);
    }
    private void SecondClick() //SecondClick returns the selected tile type and position
    {
        if (!isFirstClick) return;

        selectTile();
        CheckTile();
    }
    private void ThirdClick() //moves unit to new position
    {
        if (!isSecondClick) return;
        MoveUnit();
    }
}
