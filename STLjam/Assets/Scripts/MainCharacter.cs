using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(GameObject))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]

public class MainCharacter : MonoBehaviour
{
    public float hor_speed = 1.0f;
    public float ver_speed = 1.0f;

    public int lateral_num = 5;

    public GameObject lateral;
    public GameObject lateralParent;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float x = Input.GetAxis("Horizontal") * hor_speed * Time.deltaTime;
        float y = -1 * ver_speed * Time.deltaTime;

        Vector3 movement = new Vector3(x, y, 0);
        transform.Translate(movement);
        
        if(Input.GetKeyDown(KeyCode.Space))
        {
            GenerateLateral();
        }
    }

    void GenerateLateral()
    {
        //Fixme: refactor this
        GameObject l = Instantiate(lateral, transform);
        LateralRoot lr = l.GetComponent<LateralRoot>();
        lr.dir = new Vector2(-1.0f, -2.0f);

        GameObject l1 = Instantiate(lateral, transform);
        LateralRoot lr1 = l1.GetComponent<LateralRoot>();
        lr1.dir = new Vector2(1.0f, -2.0f);

        l.transform.parent = lateralParent.transform;
        l1.transform.parent = lateralParent.transform;
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        //collide with obstacle, 8 is hardcoded layer number
        if (col.gameObject.layer == 7)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        Debug.Log("OnCollisionEnter2D");
    }
}
