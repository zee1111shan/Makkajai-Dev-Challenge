using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class Testing : MonoBehaviour
{
    //Grid grid;
    public GameObject blackTile;
    public GameObject whiteTile;
    public TextMeshProUGUI startText;  //Text Attached to start btn
    private GameObject[,] arrayGM=new GameObject[9,9]; //array holding all the positions
    JsonDeserializer jsonDeserializer; //reference to jsondeserialzer
    float x=-2f; //initial x position for tile 
    float y = 2f; //initial y position for tile
    bool invoking; //bool variable is invoking or not

    private void Awake()
    {
        jsonDeserializer = FindObjectOfType<JsonDeserializer>();
    }
    private void Start()
    {
        
        CreateArray();
        Debug.Log(arrayGM.GetLength(0)+" "+arrayGM.GetLength(1)) ;
        //Invoke("ChangeColor", 2f);

    }

    private void Update()
    {
        if (!invoking)
        {
            if (Input.GetMouseButtonDown(0)) //Input on the array 
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);

                if (hit.collider != null)
                {
                    Debug.Log("CLICKED " + hit.transform.gameObject.tag);
                    IndexOfObject(hit.collider.gameObject);
                }
            }
        }
        
    }

    //create black 2d array
    void CreateArray()
    {
        for (int i = 0; i <= 8; i++)
        {
            for (int j = 0; j <= 8; j++)
            {
                if (arrayGM[i, j] == null)
                {
                    arrayGM[i, j] = Instantiate(blackTile, new Vector3(x, y, 0), Quaternion.identity);
                    
                }
                else
                {
                    GameObject g = Instantiate(blackTile, new Vector3(x, y, 0), Quaternion.identity);
                    Destroy(arrayGM[i, j]);
                    arrayGM[i, j] = g;
                }
                x += .5f;
                if (j == 8)
                {
                    y -= .5f;
                    x = -2f;
                }

            }

        }
        x = -2f;
        y = 2f;
    }

    //change the color of the tile by x and y position
    void ChangeColor(int x,int y)
    {
        if (arrayGM[x, y].transform.tag == "BlackTile")
        {
            GameObject g = Instantiate(whiteTile, arrayGM[x, y].transform.position, Quaternion.identity);
            Destroy(arrayGM[x, y]);
            arrayGM[x, y] = g;
        }

        else
        {
            GameObject g = Instantiate(blackTile, arrayGM[x, y].transform.position, Quaternion.identity);
            Destroy(arrayGM[x, y]);
            arrayGM[x, y] = g;
        }
            
    }

    //finds the index of the gameobject in the array
    void IndexOfObject(GameObject go)
    {
        for (int i = 0; i <= 8; i++)
        {
            for (int j = 0; j <= 8; j++)
            {
                if (arrayGM[i,j] == go)
                {
                    Debug.Log(i + ", "+j);
                    ChangeColor(i, j);
                }

            }

        }
    }
    
    //called when start button pressed
    public void StartBtn()
    {
        if (invoking == false)
        {
            startText.text = "Stop";
            InvokeRepeating("Generation", 1f, 1f);
            invoking = true;
        }
        else
        {
            startText.text = "Start";
            CancelInvoke();
            invoking = false;
        }
            
    }

    //if something is selected from the dropdown this function is called
    public void PreDefinedPatternSelector()
    {
        ClearPattern();
        foreach (Position p in jsonDeserializer.positionCollection.positions)
        {
            ChangeColor(p.x, p.y);
        }
    }

    //clear button functionn
    public void ClearPattern()
    {
        CreateArray();
        
    }

    //Iterate over the each generation with the rules and update the array accordingly
    void Generation()
    {
        
        int blackcounter = 0;
        int whitecounter = 0;
        List<Position> newList = new List<Position>(); //temp list to store the positon of the game object in array which has to be changed in the next generation.
        for(int i = 0; i <= 8; i++)
        {
            for(int j = 0; j <= 8; j++)
            {
                var results = AdjacentElements(arrayGM, i, j); //temp array to store the adjacent elements
                foreach(var result in results)
                {
                    //check tag of gameobject and increment the respective counter
                    if (result.transform.gameObject.tag == "BlackTile")
                        blackcounter += 1;
                    else if(result.transform.tag=="WhiteTile")
                        whitecounter += 1;
                }
                //rules for black tile
                if (arrayGM[i, j].tag == "BlackTile" && whitecounter == 3)
                {
                    Position p = new Position();
                    p.x = i;
                    p.y = j;
                    newList.Add(p);
                }
                //rules for white tile
                else if (arrayGM[i, j].tag == "WhiteTile" && (whitecounter < 2||whitecounter>3))
                {
                    Position p = new Position();
                    p.x = i;
                    p.y = j;
                    newList.Add(p);
                }
                //reset the counters
                blackcounter = 0;
                whitecounter = 0;
            }
            
        }
        //change the color of apropriate objects according to the above rules
        foreach(Position p in newList)
        {
            ChangeColor(p.x, p.y);
        }
    }

    //returns adjacent gameobject for the selected gameobject
    public static IEnumerable<T> AdjacentElements<T>(T[,] arr, int row, int column)
    {
        int rows = arr.GetLength(0);
        int columns = arr.GetLength(1);

        for (int j = row - 1; j <= row + 1; j++)
            for (int i = column - 1; i <= column + 1; i++)
                if (i >= 0 && j >= 0 && i < columns && j < rows && !(j == row && i == column))
                    yield return arr[j, i];
    }
}