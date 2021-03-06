using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    Rigidbody rb;
    public float speed;
    public float maxSpeed = 50;
    public float jumpForce;
    [Range(0f, 1f)]
    public float smoothRotation;
    private float SmoothVel;
    public Transform cam;

    public Vector2 direction;

    //public bool isGrounded;

    public LayerMask layer = 3;

    public Transform targetPos1;

    public Text velText;
    public Text isGroundedText;

    private void Awake()
    {

    }


    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Locked;

    }



    void Update()
    {




        InputCheck();



    }

    private void FixedUpdate()
    {
        Move(direction);
        Jump();
        IsGrounded();
        if (velText != null)
            DebugText();
    }

    void InputCheck()
    {
        direction.x = Input.GetAxisRaw("Horizontal");
        direction.y = Input.GetAxisRaw("Vertical");
        if (Input.GetKeyDown("space") && IsGrounded()) { needJump = true; }
    }

    void Move(Vector2 direction)
    {



        float T = Time.deltaTime;


        Vector3 Movement = new Vector3(direction.x, 0, direction.y).normalized;




        if (Movement.magnitude >= 0.1f && Input.GetMouseButton(1) == false)
        {
            float PlayerAngle = Mathf.Atan2(Movement.x, Movement.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float SmoothAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, PlayerAngle, ref SmoothVel, smoothRotation);
            transform.rotation = Quaternion.Euler(0f, SmoothAngle, 0f);

            Vector3 PlayerDirection = Quaternion.Euler(0f, PlayerAngle, 0f) * Vector3.forward;
            //Debug.Log("PD = " + PlayerDirection);
            //Debug.Log("MV = " + Movement);


            rb.AddForce(new Vector3(PlayerDirection.x * speed * T, 0, PlayerDirection.z * speed * T), ForceMode.Force);

            if (rb.velocity.magnitude > maxSpeed)
            {
                rb.velocity = rb.velocity.normalized * maxSpeed;
            }
        }

        if (Movement.magnitude <= 0 && IsGrounded() == true)
        {
            if (rb.velocity.magnitude >= 0.1f)
            {
                rb.velocity = rb.velocity * 0.95f;
            }
            else if (rb.velocity.magnitude < 0.1)
            {
                rb.velocity = rb.velocity * 0f;
            }

        }

        //if (Input.GetMouseButton(1))
        //{
        //    float PlayerAngle = Mathf.Rad2Deg + cam.eulerAngles.y;
        //    float SmoothAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, PlayerAngle, ref SmoothVel, smoothRotation);
        //    transform.rotation = Quaternion.Euler(0f, SmoothAngle, 0f);


        //}

    }

    public bool needJump;

    void Jump()
    {

        if (needJump)
        {
            needJump = false;
            rb.AddForce(new Vector3(0, jumpForce, 0), ForceMode.Impulse);
        }

    }

    bool IsGrounded()
    {
        RaycastHit hit;

        return Physics.Raycast(targetPos1.position, Vector3.down, out hit, 0.2f);

    }

    void DebugText()
    {

        velText.text = "Speed = " + rb.velocity.magnitude.ToString("0F");

        if (IsGrounded() == true)
        {
            isGroundedText.text = "IsGrounded? = True";
        }
        else
        {
            isGroundedText.text = "IsGrounded? = False";
        }
    }






}
