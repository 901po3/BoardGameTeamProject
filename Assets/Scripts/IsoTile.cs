using System.Collections;
using UnityEngine;

public class IsoTile : MonoBehaviour {

    public Point index;
    public float dropPosY;
    public float originalY;
    public bool isSet;
    public float speed;

    private void Awake()
    {
        speed = Random.Range(0.15f, 0.26f);
        isSet = false;
        originalY = dropPosY = 0.0f;
    }
}
