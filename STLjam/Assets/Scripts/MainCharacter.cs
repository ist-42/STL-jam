using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(GameObject))]
public class MainCharacter : MonoBehaviour
{
    public float hor_speed = 1.0f;
    public float ver_speed = 1.0f;

    public int lateral_num = 5;

    public GameObject lateral;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
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

    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if(col.gameObject.layer == 7)
        {
            //collide with obstacle, 8 is hardcoded layer number
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        Debug.Log("OnCollisionEnter2D");
    }
}
