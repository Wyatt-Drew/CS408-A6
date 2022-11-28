using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class Movement : MonoBehaviour
{
    public float speed;
    public float rotationSpeed;
    private float ySpeed = -5f;
    private CharacterController characterController;
    public GameObject focus;
    public GameObject sack;
    public Animator animator;

    [SerializeField]
    CinemachineVirtualCamera m_MainCamera;
    // Start is called before the first frame update
    void Start()
    {
       // animator = GetComponent<Animator>();
        characterController = GetComponent<CharacterController>();
        //originalStepOffset = characterController.stepOffset;
    }

    // Update is called once per frame
    void Update()
    {
        float leftRight = Input.GetAxis("Horizontal");
        float upDown = Input.GetAxis("Vertical");
        Vector3 moveDirection = new Vector3(leftRight, 0, upDown);
        
        float magnitude = Mathf.Clamp01(moveDirection.magnitude) * speed;
        moveDirection.Normalize();

        //adjust the movement direction to the angle of the camera.
        moveDirection = Quaternion.Euler(0, m_MainCamera.transform.localEulerAngles.y, 0) * moveDirection;

        Vector3 velocity = moveDirection * magnitude;
        //move the camera with character
        m_MainCamera.transform.position += (velocity * Time.deltaTime);
        velocity.y = ySpeed;
        characterController.Move(velocity * Time.deltaTime);
        //sack.transform.eularAngles.x = 180;

        if (moveDirection != Vector3.zero)
        {
            animator.SetBool("isMoving", true);
            Quaternion toRotation = Quaternion.LookRotation(moveDirection, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);
            sack.transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);
        }
        else
        {
            animator.SetBool("isMoving", false);
        }

        if (Input.mouseScrollDelta.y != 0f)
        {
            CinemachineComponentBase componentBase = m_MainCamera.GetCinemachineComponent(CinemachineCore.Stage.Body);
            if (componentBase is CinemachineFramingTransposer)
            {
                (componentBase as CinemachineFramingTransposer).m_CameraDistance = (componentBase as CinemachineFramingTransposer).m_CameraDistance + Input.mouseScrollDelta.y * Time.deltaTime * 100f; // your value
            }
        }
        getInput();
    }
    void getInput()
    {
        foreach (char c in Input.inputString.ToLower())
        {
            switch (c)
            {
                case 'e'://rotate camera right
                    {
                        m_MainCamera.transform.RotateAround(transform.position, Vector3.up, -700 * Time.deltaTime);
                        break;
                    }
                case 'q'://rotate camera left
                    {
                        m_MainCamera.transform.RotateAround(transform.position, Vector3.up, 700 * Time.deltaTime);
                        break;
                    }
                case 'r'://move camera up
                    {
                        Vector3 up = new Vector3(0, 10, 0);
                        focus.transform.position += up;
                        break;
                    }
                case 'f'://move camera down
                    {
                        Vector3 down = new Vector3(0, -10, 0);
                        focus.transform.position += down;
                        break;
                    }
                case 't'://rotate camera up
                    {
                        m_MainCamera.transform.RotateAround(transform.position, Vector3.right, 700 * Time.deltaTime);
                        break;
                    }
                case 'g'://rotate camera down
                    {
                        m_MainCamera.transform.RotateAround(transform.position, Vector3.right, -700 * Time.deltaTime);
                        break;
                    }
            }
        }

    }
}
