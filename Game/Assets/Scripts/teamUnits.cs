using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class teamUnits : MonoBehaviour {

    public unit[] unitArray;
    public int team;

    private void Start()
    {
        int unitNum = Map.Instance.typeNum;
        unitArray = new unit[unitNum];
        for (int i = 0; i < unitNum; i++)
            unitArray[i].team = team;
    }
}
