using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Background : MonoBehaviour {

    public Material[] skyboxArray;

    private void Start()
    {
       RenderSettings.skybox = skyboxArray[Random.Range(0, 4)];
    }
}
