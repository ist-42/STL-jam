using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;



public class GameController : MonoBehaviour
{
    private static GameController _instance;
    public GameObject treePrefab;
    public GameObject treeParent;
    public GameObject seedPrefab;
    public GameObject seedParent;
    
    public CircleCollider2D detect;
    public CircleCollider2D detect1;
    public LayerMask standable;
    public LayerMask unpassable;
    
    public TMP_Text SeedText;

    public int seedNum;

    public Volume v;
    public ColorAdjustments caj;
    public GameObject wormHoldMask;
    public SpriteRenderer wormHoleSp;
    public float timeToShowWormHole;
    public float timeToTravelThroughWormHole;
    
    public Camera cam0;
    public Camera cam1;

    private Camera _activeCam, _hiddenCam;

    //0:none, 1:prepare 2: done prepare 3: swap
    public int swapStatus = 0;

    public TMP_Text winText;
    
    

    //https://www.youtube.com/watch?v=dBsmaSJhUsc
    public void swapCam()
    {
        _activeCam.targetTexture = _hiddenCam.targetTexture;
        _hiddenCam.targetTexture = null;
        (_activeCam, _hiddenCam) = (_hiddenCam, _activeCam);
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

    private ContactFilter2D cf2d;
    private Collider2D[] tcd2d;
    
    private void Awake()
    {
        _instance = this;
        standable = LayerMask.GetMask("ground") | LayerMask.GetMask("seed") | LayerMask.GetMask("tree") | LayerMask.GetMask("newground") | LayerMask.GetMask("newobstacle") | LayerMask.GetMask("obstacle");
        unpassable = LayerMask.GetMask("ground") | LayerMask.GetMask("tree") | LayerMask.GetMask("newground") | LayerMask.GetMask("newobstacle") | LayerMask.GetMask("obstacle");
        var rt = new RenderTexture(Screen.width, Screen.height, 32);
        _activeCam = cam0;
        _hiddenCam = cam1;
        Shader.SetGlobalTexture("_camrt", rt);
        _hiddenCam.targetTexture = rt;
        cf2d.layerMask = unpassable;
        tcd2d = new Collider2D[1];
    }

    // Start is called before the first frame update
    void Start()
    {
        seedNum = 1;
        wormHoleSp.color = new Color(1,1,1,0);
        v.profile.TryGet(out caj);
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown(buttonName:"Fire2") || Input.GetButtonDown("Fire1"))
        {
            swapWorld();
        }
    }

    void prepareSwapWorld()
    {
        if (swapStatus==0)
        {
            Time.timeScale = 0;
            swapStatus = 1;
            StartCoroutine("showWormHole");
        } else if (swapStatus == 2)
        {
            swapStatus = 1;
            StartCoroutine("hideWormHole");
        }
    }

    void startSwapWorld()
    {
        if (swapStatus != 2)
            return;
        
        swapStatus = 3;
        currentWorldNum = 1 - currentWorldNum;
        StartCoroutine("passWormHole");
    }

    IEnumerator showWormHole()
    {
        Color c = wormHoleSp.color;
        while (c.a < 1.0f)
        {
            c.a += 1.0f /(60.0f * timeToShowWormHole);
            caj.saturation.value -= 90.0f /(60.0f * timeToShowWormHole);
            wormHoleSp.color = c;
            yield return new WaitForEndOfFrame();
        }

        swapStatus = 2;
        caj.saturation.value = -70;
    }
    
    IEnumerator hideWormHole()
    {
        Color c = wormHoleSp.color;
        while (c.a > 0.01f)
        {
            c.a -= 1.0f /(60.0f * timeToShowWormHole);
            caj.saturation.value += 90.0f /(60.0f * timeToShowWormHole);
            wormHoleSp.color = c;
            yield return new WaitForEndOfFrame();
        }  
        swapStatus = 0;
        Time.timeScale = 1;
        caj.saturation.value = 20;
    }
    
    IEnumerator passWormHole()
    {
        Time.timeScale = 0.7f;
        while (wormHoldMask.transform.localScale.x < 7f)
        {
            wormHoldMask.transform.localScale += 7.0f /(60.0f * timeToTravelThroughWormHole) * Vector3.one;
            caj.saturation.value += 90.0f /(60.0f * timeToTravelThroughWormHole);
            Time.timeScale += 0.3f /(60.0f * timeToTravelThroughWormHole);
            yield return new WaitForEndOfFrame();
        }
        
        
        caj.saturation.value = 20;
        Color c = wormHoleSp.color;
        c.a = 0;
        wormHoleSp.color = c;
        doSwapWorld();
        wormHoldMask.transform.localScale = Vector3.one;
        swapStatus = 0;
        Time.timeScale = 1;
    }

    void swapWorld()
    {
        if (Input.GetButtonDown("Fire1") && swapStatus == 2)
        {
            if (detect.OverlapCollider(cf2d, tcd2d)==0 && detect1.OverlapCollider(cf2d, tcd2d)==0)
            {
                startSwapWorld();
            }
        }
        else if (Input.GetButtonDown("Fire2") && (swapStatus == 0 || swapStatus == 2))
        {
            prepareSwapWorld();
        } 
    }

    void doSwapWorld()
    {
        switch (currentWorldNum)
        {
            case(0):
                player.transform.Translate(new Vector3(0,1000,0));
                break;
            case (1):
                player.transform.Translate(new Vector3(0,-1000,0));
                break;
        }
        
        swapCam();
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

        Vector3 offset = currentWorldNum == 0 ? Vector3.zero : Vector3.up * 1000.0f;
        GameObject seed = Instantiate(seedPrefab, c.gameObject.transform.position + offset + Vector3.up * 0.1f, Quaternion.identity);
        seed.transform.parent = seedParent.transform;
        seedNum--;
        updateSeedText(seedNum);
    }
    
    public void seedGrow(Seed seed)
    {
        seed.planted = true;
        Vector3 offset = Vector3.down * 1000.0f;
        GameObject tree = Instantiate(treePrefab, seed.gameObject.transform.position + offset,Quaternion.identity);
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
