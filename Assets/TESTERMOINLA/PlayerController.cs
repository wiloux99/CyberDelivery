using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Rigidbody rb;
    [SerializeField]float speed;
    [SerializeField] float jumpForce;
    [Range(0f,1f)]
    [SerializeField] float smoothRotation;
    private float SmoothVel;

    [SerializeField] Transform cam;

    Camera MainCamera;

    private void Awake()
    {
        
    }


    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
    }



    void Update()
    {
        Move();

        
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
            Debug.Log("PD = " + PlayerDirection);
            Debug.Log("MV = " + Movement);


             rb.AddForce(new Vector3(Movement.x * speed * T, 0, Movement.z * speed * T), ForceMode.Force);
        }

       



    }

    void Jump()
    {
        if (Input.GetKey("Space")) { rb.AddForce(new Vector3(0, jumpForce, 0), ForceMode.Impulse); }
    }
}
