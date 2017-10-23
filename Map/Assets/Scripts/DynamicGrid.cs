using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class DynamicGrid : MonoBehaviour 
{
    
    public GameObject Tile; //this is the tile prefab
    public Transform grid; //grid
    public GameObject[,] TileArray; //the array for the tiles its multi
    public Sprite[] ImgArray; //the array for the images so you can grab all of them not multi
    private int rowSize; 
    private int columnSize; 

       
    void Start()
    {
        //GenerateGrid standard 4x4

        rowSize = 4;
        columnSize = 4;

        TileArray = new GameObject[columnSize, rowSize];/* initializes array for tiles */
        for (int i = 0; i < columnSize; i++)
        {
            for (int j = 0; j < rowSize; j++) 
            {
                TileArray[i, j] = (GameObject)Instantiate(Tile, new Vector3(i-(columnSize/2), j - (rowSize / 2), 0), new Quaternion(0,0,0,0)); 
                TileArray[i, j].transform.SetParent(transform);
                TileArray[i, j].GetComponent<SpriteRenderer>().sprite = ImgArray[i];  //getting the imgs from sprite renderer and putting them in the array
            //pre randomization pictures are in colomns
            }
        }

        //swaping tiles randomly
        
        for (int i = 0; i < columnSize; i++)
        {
            for (int j = 0; j < rowSize; j++)
            {
                Swap(ref TileArray[i,j], ref TileArray[(i + Random.Range(0, columnSize - i)), (j + Random.Range(0, rowSize - j))]);
            }
        }

        transform.Rotate(new Vector3(55, 0, 45)); //how you change the "view" can test in window first!
        transform.localScale = new Vector3(3, 3, 1); //size of tiles
    }

    void Swap(ref GameObject a, ref GameObject b)
    {
        Vector3 t = a.transform.position;
        a.transform.position = b.transform.position;
        b.transform.position = t;
    }

}