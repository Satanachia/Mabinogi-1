using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    [Header("Ground Check")]
    [SerializeField] Transform groundPivot;
    [SerializeField] LayerMask groundMask;
    [SerializeField] float groundRadius;

    [Header("Move")]
    [SerializeField] float moveSpeed;
    [SerializeField] float turnSpeed;
    [SerializeField] float dashSpeed;
    [SerializeField] float dashTime;

    [SerializeField] Animator anim;

    CharacterController controller;
    Camera cam;
    Rigidbody rigid;                            // still not use


    // Input
    float x;                    
    float z;
    float gravity = (-9.81f * 3f);

    Vector3 cameraForward;                  // 카메라가 바라보는 정면 방향 
    Vector3 direction;                      // 캐릭터 이동 방향 
    Vector3 velocity;                       // 속도 vector

    Quaternion rotation;

    bool isStanding;
    bool isAccel;
    bool isWalk;
    bool isRun;
    bool isGround;
    bool isDash = false;
    bool isGuard = false;
    bool isControl = true;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        rigid = GetComponent<Rigidbody>();
        cam = Camera.main;
    }


    void Update()
    {
        GroundCheck();
        SetCameraForwardDirection();

        if (isControl)
            GetInput();

        Move();
        Rotation(direction);

        if (Input.GetKeyDown(KeyCode.Space) && !isStanding && !isDash)
        {
            StartCoroutine(StartDashCoroutine());
        }
        if (Input.GetKeyDown(KeyCode.Space) && isStanding && !isGuard)
        {
            StartCoroutine(StartGuardCoroutine());
        }

        Gravity();
    }


    void GetInput()
    {
        isAccel = Input.GetKey(KeyCode.LeftShift);

        x = Input.GetAxisRaw("Horizontal");
        z = Input.GetAxisRaw("Vertical");

    }
    void SetCameraForwardDirection()
    {
        cameraForward = cam.transform.localRotation * Vector3.forward;
        cameraForward.y = 0;
        cameraForward = cameraForward.normalized;
    }

    void Move()
    {
        float accel = isAccel ? 1.5f : 1.0f;

        // 정면 방향 계산
        direction = cameraForward * z * moveSpeed * accel * Time.deltaTime;
        // 측면 방향 계산
        direction += Quaternion.Euler(0, 90, 0) * cameraForward * x * moveSpeed * accel * Time.deltaTime ;

        isStanding = direction != Vector3.zero ? false : true;

        isWalk = (!isStanding && !isAccel) ? true : false;
        isRun = (!isStanding && isAccel) ? true : false;

        anim.SetBool("IsWalk", isWalk);
        anim.SetBool("IsRun", isRun);

        controller.Move(direction);
    }

    IEnumerator StartGuardCoroutine()
    {
        isControl = false;
        isGuard = true;
        Debug.Log("가드 시작");
        yield return new WaitForSeconds(2f);
        Debug.Log("가드 끝");

        isGuard = false;
        isControl = true;
    }

    IEnumerator StartDashCoroutine()     // 무적 구현해야함 
    {
        isControl = false;
        isDash = true;
        Debug.Log("대쉬 시작");
        

        float startTime = Time.time;
        while(Time.time < startTime + dashTime)
        {
            controller.Move(transform.forward * dashSpeed * Time.deltaTime);
            yield return null;
        }
        
        yield return new WaitForSeconds(0.02f);
        Debug.Log("대쉬 끝");
        isControl = true;
        isDash = false;
    }



    void Rotation(Vector3 dir)
    {
        Vector3 turnDirection = Vector3.RotateTowards(transform.forward, dir, turnSpeed * Time.deltaTime, 0f);
        rotation = Quaternion.LookRotation(turnDirection);
        transform.rotation = rotation;
    }                                           // 특정 방향으로 바로보는 회전 
    void Gravity()
    {
        if (isGround && velocity.y < 0f)
            velocity.y = -5f;

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }                                                       // 중력 
    void GroundCheck()
    {
        isGround = Physics.CheckSphere(groundPivot.position, groundRadius, groundMask);
    }                                                   // ground check


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundPivot.position, groundRadius);
    }
}
