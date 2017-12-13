using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI; // ????

public class UnitControl :  MonoBehaviour
{
    public float turnSwitchSpeed = 1f; // switch speed of turns
    private static UnitControl m_Instance; //singlton
    public Sprite[] unitSpriteArray;
    public int unitNum; // The total number of units
    public int selectedUnit; // The selected units number
    public GameObject[] unitArray; // An array storing all 8 units
    Point mouseOver; 
    public int turn;
    public bool isUnitSelected = false; //check if the player selected unit
    public bool isTileSelected = false; //check if player selects new tile and if it matches unit type
    public GameObject UnitPrefab;
    public Point prevPoint;
    bool resulfOfFistClick = false;
    bool isUnitReady = false;
    bool isColorChanged = false;
    Point[] unitStartPoints;
    Color[] color;
    bool check = true;
    bool onlyOnce = true;
    public Button backButton;  //turn to private 
    public GameObject selectedPosition; //position arrow. Make it private

    
    // Use this for initialization
    public static UnitControl Instance
    {
        get { return m_Instance; }
    }

    private void Awake()
    {
        m_Instance = this;
    }

    void Start()
    {
        prevPoint.x = -1;
        prevPoint.y = -1;
    }

    private void OnDestroy()
    {
        m_Instance = null;
    }

    // Update is called once per frame
    void Update()
    {
        if (Map.Instance.isMapReady && !isUnitReady)
        {
            if (onlyOnce)
            {
                initUnit();
                Map.Instance.RandomizeMap();
                onlyOnce = false;
            }
            ChangeColor(0, 1, 0.01f);
        }

        if (Input.GetMouseButtonDown(0) && check)
        {
            check = false;
            if (!isUnitSelected)
                SelectUnit();
            else if (isUnitSelected && !isTileSelected && resulfOfFistClick)
                CheckTile();
        }

        if (Input.GetMouseButtonUp(0))
        {
            check = true;
        }

    }

    void ChangeColor(float from, float to, float speed)
    {
       if(!isColorChanged)
        {
            color = new Color[unitNum];
            
            for (int i = 0; i < unitNum; i++)
            {
                color[i] = unitArray[i].GetComponent<SpriteRenderer>().color;
                color[i].a = from;
            }
            isColorChanged = true;
        }

        for (int i = 0; i < unitNum; i++)
        {
            if (color[i].a >= to - (speed * 1.1f))
            {
                color[i].a = to;
                if (i == unitNum - 1)
                {
                    isUnitReady = true;
                    return;
                }
                else
                    continue;
            }
            unitArray[i].GetComponent<SpriteRenderer>().color = color[i];
            color[i].a += speed;
        }
    }

    void initUnit() // Generate initial unit
    {
        int distance = 2;
        int startPointX = 1;
        Point posPoint;
        posPoint.x = 0;
        posPoint.y = 0;
        unitNum = 8; //# of Player units
        unitArray = new GameObject[unitNum];
        unitStartPoints = new Point[unitNum];

        for (int i = 0; i < unitNum; i++)
        {
            unitArray[i] = Instantiate(UnitPrefab, new Vector3(0, 0, -10), Quaternion.identity);
            unitArray[i].GetComponent<SpriteRenderer>().sprite = unitSpriteArray[i];

            switch (i)
            {
                case 0:
                case 4:
                    unitArray[i].GetComponent<unit>().SetType(General.TileType.FIRE);
                    break;
                case 1:
                case 5:
                    unitArray[i].GetComponent<unit>().SetType(General.TileType.WATER);
                    break;
                case 2:
                case 6:
                    unitArray[i].GetComponent<unit>().SetType(General.TileType.ROCK);
                    break;
                case 3:
                case 7:
                    unitArray[i].GetComponent<unit>().SetType(General.TileType.GRASS);
                    break;
            }
            if (i < unitNum / 2)
            {
                posPoint.x = unitArray[i].GetComponent<unit>().index.x = startPointX;
                posPoint.y = unitArray[i].GetComponent<unit>().index.y = 0;
                startPointX += distance;
                if (i == unitNum / 2 - 1)
                    startPointX = 1;
            }
            if (i >= unitNum / 2)
            {
                posPoint.x = unitArray[i].GetComponent<unit>().index.x = startPointX;
                posPoint.y = unitArray[i].GetComponent<unit>().index.y = Map.Instance.mapSizeX - 1;
                startPointX += distance;
            }
            unitStartPoints[i].x = posPoint.x;
            unitStartPoints[i].y = posPoint.y;
            

            unitArray[i].transform.position =
                new Vector3(Map.Instance.map[posPoint.x, posPoint.y].transform.position.x + Map.Instance.offset.x,
                Map.Instance.map[posPoint.x, posPoint.y].transform.position.y + Map.Instance.offset.y,
                Map.Instance.map[posPoint.x, posPoint.y].transform.position.z - 0.1f);
        }
    }

    public void SelectUnit() //returns true if unit is player 
    {
        Vector2 unitRay = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D unitHit = Physics2D.Raycast(unitRay, Vector2.zero);

        if (unitHit.transform.tag == "Unit" && unitHit.transform.GetComponent<unit>().isAlive &&
            unitHit.transform.GetComponent<unit>().touchable) //If unit selected
        {
            int from = 0;
            int to = 0;
            General.Instance.GetFromAndToIndex(turn, unitNum, ref from, ref to);
            if (unitHit.transform.GetComponent<unit>().isAlive == false) return;

            unitHit.transform.GetComponent<unit>().selected = true;
            
            for (int i = from; i < to; i++)
            {
                if (unitArray[i].GetComponent<unit>().selected)
                {
                    unitArray[i].GetComponent<unit>().selected = false;
                    FindObjectOfType<AudioManager>().Play("SELECTTILE");
                    backButton.gameObject.SetActive(true);
                    Debug.Log(i + " Unit is clicked");
                    selectedUnit = i;
                    isUnitSelected = true;
                    OffOtherUnits();
                    if (turn == 1 && selectedUnit < unitNum / 2)
                        resulfOfFistClick = true;
                    else if (turn == 2 && selectedUnit > unitNum / 2 - 1)
                        resulfOfFistClick = true;
                    return;
                }   
            }
        }
    }

    private void CheckTile() // check tile type to compare to player type
    {
        Vector2 cubeRay = Camera.main.ScreenToWorldPoint(Input.mousePosition); // gets position of mouse and stores it in a Vector2 
        RaycastHit2D cubeHit = Physics2D.Raycast(cubeRay, Vector2.zero, Mathf.Infinity, 
            (1 << LayerMask.NameToLayer("Map") | 1 << LayerMask.NameToLayer("Background"))); // Uses a Raycast cast from mouse position to the scene??? I think LOL
        Point playerIndex;
        playerIndex.x = unitArray[selectedUnit].GetComponent<unit>().index.x; //stores the x position of the unit
        playerIndex.y = unitArray[selectedUnit].GetComponent<unit>().index.y; //stores the y position of the unit

        if (cubeHit.transform.tag != "Map")
            return;
        else//checks if the object hit has a map tag
        {
            Vector3 mosPos = cubeHit.transform.position; //returns the position of Gameobject hit by Raycast

            mouseOver.x = cubeHit.transform.GetComponent<IsoTile>().index.x; //gets the indexX from IsoTile script attached to tile prefab for directory
            mouseOver.y = cubeHit.transform.GetComponent<IsoTile>().index.y; //gets the indexY from IsoTile script attached to tile prefab for directory
            Debug.Log("x: " + mouseOver.x + ", y: " + mouseOver.y);
        }

        General.TileType tileType = Map.Instance.GetType(mouseOver); //returns the x,y position of the tile type

        int from = 0;
        int to = 0;
        General.Instance.GetFromAndToIndex(turn, unitNum, ref from, ref to);
        for (int i = from; i < to; i++)
        {
            if (i == selectedUnit) continue;
            if (mouseOver.x == unitArray[i].GetComponent<unit>().index.x && 
                mouseOver.y == unitArray[i].GetComponent<unit>().index.y)
                return;
        }

        if (prevPoint.x != mouseOver.x || prevPoint.y != mouseOver.y)
        {
            prevPoint.x = mouseOver.x;
            prevPoint.y = mouseOver.y;
            FindObjectOfType<AudioManager>().Play("SELECTTILE");
            Instantiate(selectedPosition, new Vector3(mouseOver.x, mouseOver.y, -10f), Quaternion.identity);///????????????????????????????
           
            Debug.Log("Tile is clicked");
            isTileSelected = false;
            return;
        }
        else if (prevPoint.x == mouseOver.x && prevPoint.y == mouseOver.y)
        {
            if ((playerIndex.x + 1 == mouseOver.x && playerIndex.y == mouseOver.y) ||    //Checks if the tile selected is directly beside the unit
            (playerIndex.x - 1 == mouseOver.x && playerIndex.y == mouseOver.y) ||
            (playerIndex.x == mouseOver.x && playerIndex.y + 1 == mouseOver.y) ||
            (playerIndex.x == mouseOver.x && playerIndex.y - 1 == mouseOver.y))
            {                
                if (tileType == unitArray[selectedUnit].GetComponent<unit>().GetType())  //should be added after randomization map is done.
                {
                    isTileSelected = true;
                    MoveUnit(mouseOver);
                    backButton.gameObject.SetActive(false);
                    Debug.Log("Tile is clicked second times");
                    return;
                }
            }
        }   
    }
    
    void Attack()
    {
        Point curUnitPos = unitArray[selectedUnit].GetComponent<unit>().index;
        int tempTurn = 1;
        int opponentIndex = General.Instance.GetOpponentIndex(turn, selectedUnit);
        int from = 0;
        int to = 0;
        if (turn == 1) tempTurn = 2;
        else if (turn == 2) tempTurn = 1;

        General.Instance.GetFromAndToIndex(tempTurn, unitNum, ref from, ref to);

        if(curUnitPos.x == unitStartPoints[opponentIndex].x &&
            curUnitPos.y == unitStartPoints[opponentIndex].y) // check enemy pawn
        {
            unitArray[opponentIndex].GetComponent<unit>().isAlive = false;
            unitArray[opponentIndex].GetComponent<unit>().touchable = false;
            unitArray[opponentIndex].SetActive(false);
            //Map.Instance.areTilesOut[unitArray[opponentIndex].GetComponent<unit>().GetTypeInt()] = false;
        }

        for (int i = from; i < to; i++) //check enemy units
        {
            Point opponentUnitPos = unitArray[i].GetComponent<unit>().index;
            if (curUnitPos.x == opponentUnitPos.x && curUnitPos.y == opponentUnitPos.y)
            {
                unitArray[i].GetComponent<unit>().isAlive = false;
                unitArray[i].GetComponent<unit>().touchable = false;
                unitArray[i].SetActive(false);
                //Map.Instance.areTilesOut[unitArray[i].GetComponent<unit>().GetTypeInt()] = false;
                return;
            }
        }
    }

    private void MoveUnit(Point to) //translate unit position and change Sprite
    {
        GameObject tilePos = Map.Instance.map[to.x, to.y];
        float destPosX = tilePos.transform.position.x;
        float destPosY = tilePos.transform.position.y;
        float destPosZ = tilePos.transform.position.z;

        int curIndexX = unitArray[selectedUnit].GetComponent<unit>().index.x = mouseOver.x;
        int curIndexY = unitArray[selectedUnit].GetComponent<unit>().index.y = mouseOver.y;

        unitArray[selectedUnit].transform.position =
                new Vector3(destPosX, destPosY + Map.Instance.offset.y, destPosZ - 0.1f);
                FindObjectOfType<AudioManager>().Play("MOVE");
            
        StartCoroutine("EndTurn");
    }

    IEnumerator EndTurn()
    {
        yield return new WaitForSeconds(turnSwitchSpeed);
        isUnitSelected = false;
        isTileSelected = false;
        resulfOfFistClick = false;
        selectedUnit = 0;
        unitArray[selectedUnit].GetComponent<unit>().selected = false;

        OnOtherUnits();

        if (turn == 1)
        {
            turn = 2;
        }
        else if (turn == 2)
            turn = 1;

        Map.Instance.RandomizeMap();
    }

   
    public void ChangeUnitButtonPush()
    {
        if (isUnitSelected)
        {
            unitArray[selectedUnit].GetComponent<unit>().selected = false;
            isUnitSelected = false;
            isTileSelected = false;
            resulfOfFistClick = false;
            OnOtherUnits();
        }
    }

    void OffOtherUnits()
    {
        for (int i = 0; i < unitNum; i++)
        {
            if (selectedUnit == i || !unitArray[i].GetComponent<unit>().isAlive) continue;

            Color color = unitArray[i].GetComponent<SpriteRenderer>().color;
            unitArray[i].GetComponent<unit>().touchable = false;
            color.a = 0.55f;
            unitArray[i].GetComponent<SpriteRenderer>().color = color;
            
        }
    }

    void OnOtherUnits()
    {
        for (int i = 0; i < unitNum; i++)
        {
            if (unitArray[i].GetComponent<unit>().touchable 
                || !unitArray[i].GetComponent<unit>().isAlive) continue;

            Color color = unitArray[i].GetComponent<SpriteRenderer>().color;
            unitArray[i].GetComponent<unit>().touchable = true;
            color.a = 1;
            unitArray[i].GetComponent<SpriteRenderer>().color = color;
            
        }
    }

    //void AISytem()
    //{
    //    Point destPoint;
    //    destPoint.x = 0;
    //    destPoint.y = 0;
    //    bool toUnit = false;
    //    int from = 0;
    //    int to = 0;
    //    int shortestDisIndex = unitNum + 1;
    //    int shortestDis = Map.Instance.mapSizeX * Map.Instance.mapSizeY + 1;
    //    int prevShortestDis = shortestDis;
    //    int moveIndex = -1;

    //    if (turn == 1)
    //    {
    //        from = 0;
    //        to = unitNum / 2;
    //    }
    //    else if(turn == 2)
    //    {
    //        from = unitNum / 2;
    //        to = unitNum;
    //    }

    //    for (int i = from; i < to; i++)
    //    {
    //        Point curUnitPoint;
    //        curUnitPoint.x = unitArray[i].GetComponent<unit>().index.y;
    //        curUnitPoint.y = unitArray[i].GetComponent<unit>().index.x;

    //        if (!FindPossiblePath(curUnitPoint, ref moveIndex, ref shortestDis, ref toUnit, ref destPoint)) continue;

    //        if (prevShortestDis > shortestDis)
    //        {
    //            prevShortestDis = shortestDis;
    //            moveIndex = i;
    //        }
    //    }

    //    if (moveIndex != -1)
    //    {
    //        selectedUnit = moveIndex;
    //        Debug.Log("destPoint.x: " + destPoint.x + ", destPoint.y: " + destPoint.y);
    //        MoveUnit(destPoint);
    //    }

    //    Debug.Log("moveIndex: " + moveIndex + ", shortestDis: " + shortestDis + ", targeting Unit: " +  toUnit);
    //}

    //bool FindPossiblePath(Point curUnitPoint, ref int shortestDisIndex, ref int shortestDis, ref bool toUnit, ref Point shortestPoint)
    //{
    //    Point possiblePoint;
    //    int curType = Map.Instance.GetType(curUnitPoint) ;
    //    possiblePoint.x = curUnitPoint.x;
    //    possiblePoint.y = curUnitPoint.y;

    //    Debug.Log(curUnitPoint.x +", " + curUnitPoint.y);
    //    if (curType == Map.Instance.GetType(possiblePoint.y, possiblePoint.x + 1))
    //    {
    //        possiblePoint.x = curUnitPoint.x + 1;
    //        FindTheShortestWay(possiblePoint, ref shortestDisIndex, ref shortestDis, ref toUnit, ref shortestPoint);
    //        possiblePoint.x = curUnitPoint.x;
    //        return true;
    //    }
    //    if (curType == Map.Instance.GetType(possiblePoint.y, possiblePoint.x - 1))
    //    {
    //        possiblePoint.x = curUnitPoint.x - 1;
    //        FindTheShortestWay(possiblePoint, ref shortestDisIndex, ref shortestDis, ref toUnit, ref shortestPoint);
    //        possiblePoint.x = curUnitPoint.x;
    //        return true;
    //    }
    //    if (curType == Map.Instance.GetType(possiblePoint.y + 1, possiblePoint.x))
    //    {
    //        possiblePoint.x = curUnitPoint.y + 1;
    //        FindTheShortestWay(possiblePoint, ref shortestDisIndex, ref shortestDis, ref toUnit, ref shortestPoint);
    //        possiblePoint.x = curUnitPoint.y;
    //        return true;
    //    }
    //    if (curType == Map.Instance.GetType(possiblePoint.y - 1, possiblePoint.x))
    //    {
    //        possiblePoint.x = curUnitPoint.y - 1;
    //        FindTheShortestWay(possiblePoint, ref shortestDisIndex, ref shortestDis, ref toUnit, ref shortestPoint);
    //        possiblePoint.x = curUnitPoint.y;
    //        return true;
    //    }
    //    return false;
    //}

    //void FindTheShortestWay(Point curUnit, ref int shortestDisIndex, ref int shortestDis, ref bool toUnit, ref Point shortestPoint)
    //{
    //    int opponentFrom = -1 ;
    //    int opponentTo = -1;
    //    if (turn == 1)
    //    {
    //        opponentFrom  = unitNum / 2;
    //        opponentTo = unitNum;
    //    }
    //    else if (turn == 2)
    //    {
    //        opponentFrom = 0;
    //        opponentTo = unitNum / 2 - 1;
    //    }

    //    for (int i = opponentFrom; i < opponentTo; i++)
    //    {
    //        int DisToUnit = GetDistance(curUnit, unitArray[i].GetComponent<unit>().index);

    //        if (shortestDis > DisToUnit)
    //        {
    //            shortestPoint = curUnit;
    //            shortestDis = DisToUnit;
    //            shortestDisIndex = i;
    //            toUnit = true;
    //        }

    //        if (shortestDis > GetDistance(curUnit, pondPoint[i])) //unit to pond distance
    //        {
    //            shortestPoint = curUnit;
    //            shortestDis = DisToUnit;
    //            shortestDisIndex = -1;
    //            toUnit = false;
    //        }
    //    }
    //}

    //int GetDistance(Point curPoint, Point destPoint)
    //{
    //    int distance = 0;

    //    distance = Mathf.Abs(destPoint.x - curPoint.x) + Mathf.Abs(destPoint.y - curPoint.y);
    //    //Debug.Log("distance: " + distance);
    //    return distance;
    //}

}
