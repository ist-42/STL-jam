using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class CharacterMovement : MonoBehaviour
{
    public float horGroundMaxSpeed = 6.0f;
    public float horGroundAcc = 60.0f;
    public float horGroundSlow = 20.0f;

    //if horizontal spped exceeds this limit on air, the player won't be able to accelerate further
    public float horAirMaxSpeed = 4.0f;
    public float horAirAcc = 6.0f;
    public float horAirSlow = 0.5f;
    

    public float jumpForce = 10.0f;

    private Rigidbody2D _rb;
    private bool _onGround = false;
    private int _jumpPoint = 1;

    // Start is called before the first frame update
    void Start()
    {
        _rb = gameObject.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        float hor = Input.GetAxisRaw("Horizontal");

        if(_onGround)
        {
            if (Mathf.Abs(hor) > 0.001f)
            {
                _rb.AddForce(new Vector2(hor, 0.0f) * horGroundAcc * Time.deltaTime, ForceMode2D.Impulse);
            } else if (Mathf.Abs(_rb.velocity.x) > 0.01f)
            {
                _rb.AddForce(new Vector2(1.0f * -Mathf.Sign(_rb.velocity.x), 0.0f) * horGroundSlow * Time.deltaTime, ForceMode2D.Impulse);
            }
        } else
        {
            if (Mathf.Abs(hor) > 0.001f && (Mathf.Abs(_rb.velocity.x) < horAirMaxSpeed || (_rb.velocity.x * hor < 0.001f)))
            {
                _rb.AddForce(new Vector2(hor, 0.0f) * horAirAcc * Time.deltaTime, ForceMode2D.Impulse);
            }
            else if (Mathf.Abs(_rb.velocity.x) > 0.01f)
            {
                _rb.AddForce(new Vector2(1.0f * -Mathf.Sign(_rb.velocity.x), 0.0f) * horAirSlow * Time.deltaTime, ForceMode2D.Impulse);
            }
        }


        //max speed limit
        if (Mathf.Abs(_rb.velocity.x) >= horGroundMaxSpeed)
        {
            _rb.velocity = new Vector2(Mathf.Sign(_rb.velocity.x) * horGroundMaxSpeed, _rb.velocity.y);
        }

        if (!_onGround && Mathf.Abs(_rb.velocity.x) >= horAirMaxSpeed)
        {
            _rb.AddForce(new Vector2(1.0f * -Mathf.Sign(_rb.velocity.x), 0.0f) * horAirSlow * Time.deltaTime, ForceMode2D.Impulse);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if(_jumpPoint <= 0)
            {
                _jumpPoint = 0;
                return;
            }
            _jumpPoint--;
            _rb.AddForce(new Vector2(0.0f, jumpForce), ForceMode2D.Impulse);
        }
    }


    void FixedUpdate()
    {
        //dead zone
        if (Mathf.Abs(_rb.velocity.x) < .2)
        {
            _rb.velocity = new Vector2(0, _rb.velocity.y);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //10 is the layernumber of ground
        if(collision.gameObject.layer == 10)
        {
            _onGround = true;
            _jumpPoint = 1;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 10)
        {
            _onGround = false;
            _jumpPoint = 0;
        }
    }
}