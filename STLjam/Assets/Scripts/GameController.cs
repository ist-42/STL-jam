using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class GameController : MonoBehaviour
{
    public GameObject oldWorld;

    public GameObject newWorld;

    public GameObject player;

    //0 = oldWorld, 1 = newWorld
    public int currentWorldNum = 0;
    public int worldCount = 2;

    // Start is called before the first frame update
    void Start()
    {
        updateWorld(currentWorldNum);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.J))
        {
            currentWorldNum = 1 - currentWorldNum;
            updateWorld(currentWorldNum);
        }
    }

    void updateWorld(int worldNum)
    {
        oldWorld.SetActive(false);
        newWorld.SetActive(false);

        switch (worldNum)
        {
            case(0):
                oldWorld.SetActive(true);
                break;
            case (1):
                newWorld.SetActive(true);
                break;
        }
    }
}
