using UnityEditor;
using UnityEngine;


/// <summary>
/// 플레이어블 캐릭터
/// 좌우 이동, 대쉬, 점프(1단), 착지대시?
/// 키입력, 착지 콜라이더 인식
/// 기본이동속도, 대쉬속도, 점프높이
/// 기본이동속도 캡, 가속도, 대쉬이동속도 캡(=기본이동속도*배수, 따라서 캡 수치가 아닌 배수)
/// 인풋매니저 사용법을 아직 익히지 못했으므로 구버전으로 입력 받음
/// 
/// 기본 이동은 moveSpeed만큼 바로 움직이는 게 아니라, 가속하다가 moveSpeed에 도달하면 moveSpeed 항속
/// </summary>
public class Player : MonoBehaviour
{
    private enum State
    {
        Idle,
        Walk,
        Dash,
        Jump
    }

    [SerializeField, Tooltip("일반이동속도 캡")] private float walkSpd = 5.0f;
    [SerializeField, Tooltip("가속도, 추천 : moveSpeed*0.2")] private float walkAcc = 10.0f;
    [SerializeField, Tooltip("브레이크 속도, 추천 : moveSpeed*3")] private float breakAcc = 15.0f;
    [SerializeField, Tooltip("대쉬속도 캡")] private float dashSpd = 8.5f;
    [SerializeField, Tooltip("대쉬 가속도")] private float dashAcc = 20.0f;

    [SerializeField, Tooltip("점프 힘, rb 중력배율 3에서 10")] private float jumpForce = 10.0f;
    [SerializeField, Tooltip("점프시 좌우 이동힘, walkAcc보다 작게")] private float jumpWalkAcc = 8.5f;
    [SerializeField, Tooltip("점프시 좌우 이동속도 캡, walkSpd보다 작게")] private float jumpDashAcc = 17.5f;


    [SerializeField, Tooltip("방향, 우 = 1, 좌 = -1")] private int direction = 1;

    //private bool isFacingLeft = false;
    private bool canJump = false;
    private bool isGrounded = false;
    private bool isDashing = false;


    private State state = State.Idle;
    private Rigidbody2D rb;
    //private Animator animator;


    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        //animator = GetComponent<Animator>();
    }

    private void Update()
    {
        Update_Direction();
        Update_State();
    }

    private void FixedUpdate()
    {
        FixedUpdate_Move();
        FixedUpdate_Jump();
        FixedUpdate_InAirMove();
    }

    private void Update_Direction()
    {
        if (Input.GetKey(KeyCode.RightArrow))
        {
            direction = 1;
        }
        else if (Input.GetKey(KeyCode.LeftArrow))
        {
            direction = -1;
        }
        else
        {
            direction = 0;
        }
    }

    private void Update_State()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            isGrounded = false;
            //state = State.Jump;
        }

        if (Input.GetKey(KeyCode.Z))
        {
            isDashing = true;
        }
        else isDashing = false;

        if(isGrounded == false)
        {
            state = State.Jump;
        }
        else if (direction != 0)
        {
            if (isDashing == false)
            {
                state = State.Walk;
            }
            else
            {
                state = State.Dash;
            }
        }
        else
        {
            state = State.Idle;
        }
    }

    private void FixedUpdate_Move()
    {
        Vector2 velocity = rb.linearVelocity;

        //이동 미입력시 브레이크 속도 계산
        if (state == State.Idle)
        {
            velocity.x = Mathf.MoveTowards(
                    velocity.x,
                    0,
                    breakAcc * Time.fixedDeltaTime);
        }
        //좌우 걷기 속도 계산
        if(state == State.Walk)
        {
            velocity.x = Mathf.MoveTowards(
                velocity.x,
                direction * walkSpd,
                walkAcc * Time.fixedDeltaTime);
        }
        //좌우 대쉬 속도 계산
        if(state == State.Dash)
        {
            velocity.x = Mathf.MoveTowards(
                velocity.x,
                direction * dashSpd,
                dashAcc * Time.fixedDeltaTime);
        }
        rb.linearVelocity = velocity; //최종 적용
    }

    private void FixedUpdate_Jump()
    {
        if (canJump == false) return;

        if (state == State.Jump)
        {
            canJump = false;

            Vector2 velocity = rb.linearVelocity;

            velocity.y = jumpForce;


            rb.linearVelocity = velocity;
            print("점프실행완료");
        }
        
    }

    private void FixedUpdate_InAirMove()
    {
        Vector2 velocity = rb.linearVelocity;
        if(state == State.Jump && isDashing == false)
        {
            velocity.x = Mathf.MoveTowards(
                velocity.x,
                direction *walkSpd,
                jumpWalkAcc * Time.fixedDeltaTime);
        }else if (state == State.Jump && isDashing == true)
        {
            velocity.x  = Mathf.MoveTowards(
                velocity.x,
                direction * dashSpd,
                jumpDashAcc * Time.fixedDeltaTime);
        }
        rb.linearVelocity = velocity;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Ground"))
        {
            canJump = true;
            isGrounded = true;
        }
    }
    //TODO : int direction 리팩터링 종료시 isFacingLeft정리할것

    private void OnDrawGizmos()
    {
        if (rb == null) return;


        Vector2 velocity = rb.linearVelocity;

        GUIStyle style = GUIStyle.none;
        style.normal.textColor = Color.black;
        style.alignment = TextAnchor.MiddleCenter;

        string dir="중립";
        if (direction == 1) dir = "오른쪽";
        else if (direction == -1) dir = "왼쪽";
        else if (direction == 0) dir = "중립";

        string str = "속도 : " + velocity.x.ToString();
        str += "\n상태 : " + state.ToString() +  dir;
        str += "\ncanJump : " + canJump.ToString();
        str += "\nisGrounded : " + isGrounded.ToString();

        Handles.Label((Vector2)transform.position + new Vector2(0, 1.25f), str, style);
    }
}
