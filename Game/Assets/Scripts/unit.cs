using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class unit : MonoBehaviour {

    public int team;
    public int type;
    Vector3 position;
    
    public void setPosition(int x, int y)
    {
        position = new Vector3(x, y, 1);
    }
}
