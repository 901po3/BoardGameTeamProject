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
    int turn; // 0=Player 1, 1=Player 2
    int tileType; //Type of tile (Fire,Ice,Rock,Sand)
    bool isFirstClick = false; //check if the player selected unit
    bool isSecondClick = false; //check if player selects new tile and if it matches unit type
    public GameObject UnitPrefab;


    // Use this for initialization
    void Start() {
        unitNum = 8; //# of Player units
        unitArray = new GameObject[unitNum]; 
        for (int i = 0; i < unitNum; i++)
        {
            unitArray[i] = (GameObject)Instantiate(UnitPrefab, new Vector3(0,0,-5), Quaternion.identity);
        }
    }

    // Update is called once per frame
    void Update() {
        if (Input.GetMouseButtonDown(0))
        {
            FirstClick();
            UpdateMouseOver();
        }

    }


    public void UpdateMouseOver() //update position of tile
    {

        Vector2 cubeRay = Camera.main.ScreenToWorldPoint(Input.mousePosition); // gets position of mouse and stores it in a Vector2 
        RaycastHit2D cubeHit = Physics2D.Raycast(cubeRay, Vector2.zero); // Uses a Raycast cast from mouse position to the scene??? I think LOL
            
        if (cubeHit.transform.tag == "Map")  //checks if the object hit has a map tag
        {

            Vector3 mosPos = cubeHit.transform.position; //returns the position of Gameobject hit by Raycast

            mouseOverX = cubeHit.transform.GetComponent<IsoTile>().indexX; //gets the indexX from IsoTile script attached to tile prefab for directory
            mouseOverY = cubeHit.transform.GetComponent<IsoTile>().indexY; //gets the indexY from IsoTile script attached to tile prefab for directory
          
            
            Debug.Log(mosPos); //for testing
            Debug.Log(mouseOverX); //for testing
            Debug.Log(mouseOverY); //for testing
            
        }
        else //no tile clicked
        {
            mouseOverX = -1;
            mouseOverY = -1;
        }
    }

    public bool SelectUnit(int turn) //returns true if unit is player 
    {
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

        RaycastHit hit; //declare Raycast for tracking click position.. MUST FIX
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 25.0f)) //if statement calculates if the clicked object is a unit and returns unit number.. MUST FIX
        {
            if (hit.collider.tag == "Unit") //If unit selected
            {
                hit.transform.gameObject.GetComponent<unit>().selected = true; 

                for (int i = from; i < to; i++)
                {
                    if (unitArray[i].GetComponent<unit>().selected)
                    {
                        Debug.Log("Unit is clicked");
                        selectedUnit = i;
                        return true;
                    }
                }
            }
            
        }
        isFirstClick = false; // assigns false if the if nothing detected by Raycast
        return false;
    }

    private void CheckTile() // check tile type to compare to player type
    {
        int playerXPos = unitArray[selectedUnit].GetComponent<unit>().posX; //stores the x position of the unit
        int PlayerYPos = unitArray[selectedUnit].GetComponent<unit>().posY; //stores the y position of the unit

        tileType = Map.Instance.GetType(mouseOverX, mouseOverY); //returns the x,y position of the tile type

        if(SelectUnit(turn))
            FirstClick();

        if ((playerXPos + 1 == mouseOverX && PlayerYPos == mouseOverY) ||    //Checks if the tile selected is directly beside the unit
            (playerXPos - 1 == mouseOverX && PlayerYPos == mouseOverY) ||
            (playerXPos == mouseOverX && PlayerYPos + 1 == mouseOverY) ||
            (playerXPos == mouseOverX && PlayerYPos - 1 == mouseOverY))      
        {
            if (tileType == unitArray[selectedUnit].GetComponent<unit>().type) 
            {
                isSecondClick = true;
                Debug.Log("Tile is clicked");
                return;
            }
        }

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

        UpdateMouseOver();
        CheckTile();
    }
    private void ThirdClick() //moves unit to new position
    {
        if (!isSecondClick) return;
        MoveUnit();
    }
}
