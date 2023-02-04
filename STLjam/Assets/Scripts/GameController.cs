using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditorInternal;
using UnityEngine;



public class GameController : MonoBehaviour
{
    private static GameController _instance;
    public GameObject treePrefab;
    public GameObject treeParent;
    public GameObject seedPrefab;
    public GameObject seedParent;

    public LayerMask standable;

    public TMP_Text SeedText;

    public int seedNum;
    private void Awake()
    {
        _instance = this;
        standable = LayerMask.GetMask("ground") | LayerMask.GetMask("seed") | LayerMask.GetMask("tree");
    }

    public static GameController Instance
    {
        get
        {
            if (!_instance)
                Debug.Log("GameController Error: null instance");
            return _instance;
        }
    }
    
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
        seedNum = 1;
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

    public void createSeed(CharacterMovement c)
    {
        if (currentWorldNum == 1)
            return;
        if (seedNum <= 0)
        {
            seedNum = 0;
            return;
        }
        GameObject seed = Instantiate(seedPrefab, c.gameObject.transform.position, Quaternion.identity);
        seed.transform.parent = seedParent.transform;
        seedNum--;
        updateSeedText(seedNum);
    }
    
    public void seedGrow(Seed seed)
    {
        seed.planted = true;
        GameObject tree = Instantiate(treePrefab, seed.gameObject.transform.position,Quaternion.identity);
        tree.transform.parent = treeParent.transform;
        seed.growedTree = tree;
    }

    public void retriveSeed(Seed seed)
    {
        if (currentWorldNum == 1)
            return;
        Destroy(seed.growedTree);
        Destroy(seed.gameObject);
        seedNum++;
        updateSeedText(seedNum);
    }

    private void updateSeedText(int seedNum)
    {
        SeedText.text = $"Seeds: {this.seedNum}";
    }
}
