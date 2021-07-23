using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Rigidbody rb;
    public float speed;
    public float jumpForce;
    [Range(0f,1f)]
    public float smoothRotation;
    private float SmoothVel;
    public Transform cam;

    public bool isGrounded;

    public LayerMask layer = 3;

    public Transform targetPos1;
    public Transform targetPos2;

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
        Move();
        Jump();
        Debug.DrawRay(targetPos1.position, new Vector3(0,-0.2f,0));




    }

    void Move()
    {
        float T = Time.deltaTime;
        float hor = Input.GetAxisRaw("Horizontal");
        float ver = Input.GetAxisRaw("Vertical");

        Vector3 Movement = new Vector3(hor, 0, ver).normalized;




        if(Movement.magnitude >= 0.1f)
        {
            float PlayerAngle = Mathf.Atan2(Movement.x, Movement.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float SmoothAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, PlayerAngle, ref SmoothVel, smoothRotation);
            transform.rotation = Quaternion.Euler(0f, SmoothAngle, 0f);

            Vector3 PlayerDirection = Quaternion.Euler(0f, PlayerAngle, 0f) * Vector3.forward;
            //Debug.Log("PD = " + PlayerDirection);
            //Debug.Log("MV = " + Movement);


             rb.AddForce(new Vector3(PlayerDirection.x * speed * T, 0, PlayerDirection.z * speed * T), ForceMode.Force);
        }

       



    }

    void Jump()
    {

        if (Input.GetKey("space") && isGrounded == true) { rb.AddForce(new Vector3(0, jumpForce, 0), ForceMode.Impulse);}
    }

    void RaycastCheck()
    {

        Ray ray;
        ray = new Ray(targetPos1.position, new Vector3(0, -0.2f, 0));

        Physics.Raycast(ray, -1, 3);

        RaycastHit hit;

        if (Physics.Raycast((ray, out hit ))
        {

        }
    }

   


}
