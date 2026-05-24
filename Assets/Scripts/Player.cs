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
        Idle_R, Idle_L,
        Walk_R, Walk_L,
        Run_R, Run_L,
        Jump_R, Jump_L,
        Break_R, Break_L
    }

    [SerializeField, Tooltip("일반이동속도 캡")] private float walkSpd = 5.0f;
    [SerializeField, Tooltip("가속도, 추천 : moveSpeed*0.2")] private float walkAcc = 10.0f;
    [SerializeField, Tooltip("브레이크 속도, 추천 : moveSpeed*3")] private float breakAcc = 15.0f;
    [SerializeField, Tooltip("대쉬속도 캡")] private float dashSpd = 8.5f;
    [SerializeField, Tooltip("대쉬 가속도")] private float dashAcc = 20.0f;
    [SerializeField, Tooltip("점프 힘, rb 중력배율 3에서 10")] private float jumpForce = 10.0f;

    private bool isFacingLeft = false;
    private bool canJump = false;
    private bool isGrounded = false;


    private State state = State.Idle_R;
    private Rigidbody2D rb;
    //private Animator animator;


    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        //animator = GetComponent<Animator>();
    }

    private void Update()
    {
        Update_State();
    }

    private void FixedUpdate()
    {
        FixedUpdate_Move();
        FixedUpdate_Jump();

    }

    private void Update_State()
    {
        if (isFacingLeft)
        {
            state = State.Idle_L;
            if (isGrounded == false)
            {
                state = State.Jump_L;
            }
        }
        else 
        { 
            state = State.Idle_R; 
            if(isGrounded == false)
            {
                state = State.Jump_R;
            }
        }


        if (Input.GetKey(KeyCode.RightArrow))
        {
            if (isGrounded == true)
            {
                state = State.Walk_R;
                //TODO : anim추가
                if (Input.GetKey(KeyCode.Z))
                {
                    state = State.Run_R;
                }
            }
            else if (isGrounded == false)
            {
                state = State.Jump_R;
            }
            isFacingLeft = false;
        }
        else if (Input.GetKey(KeyCode.LeftArrow))
        {
            if (isGrounded == true)
            {
                state = State.Walk_L;
                if (Input.GetKey(KeyCode.Z))
                {
                    state = State.Run_L;
                }
            }
            else if (isGrounded == false)
            {
                state = State.Jump_L;
            }


            isFacingLeft = true;
        }

        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            isGrounded = false;
        }

    }

    private void FixedUpdate_Move()
    {
        Vector2 velocity = rb.linearVelocity;

        //이동 미입력시 브레이크 속도 계산
        if (state == State.Idle_R || state == State.Idle_L)
        {
            if (Mathf.Abs(velocity.x) > 0)
            {
                velocity.x = Mathf.MoveTowards(
                    velocity.x,
                    0,
                    breakAcc * Time.fixedDeltaTime);
            }
        }
        //좌우 걷기 속도 계산
        if (state == State.Walk_R)
        {
            if (velocity.x < walkSpd)
            {
                velocity.x = Mathf.MoveTowards(
                    velocity.x,
                    walkSpd,
                    walkAcc * Time.fixedDeltaTime);
            }
        }
        else if (state == State.Walk_L)
        {
            if (velocity.x > -walkSpd)
            {
                velocity.x = Mathf.MoveTowards(
                    velocity.x,
                    -walkSpd,
                    walkAcc * Time.fixedDeltaTime);
            }
        }
        //좌우 대쉬 속도 계산
        if (state == State.Run_R)
        {
            if (velocity.x < dashSpd)
            {
                velocity.x = Mathf.MoveTowards(
                    velocity.x,
                    dashSpd,
                    dashAcc * Time.fixedDeltaTime);
            }
        }
        else if (state == State.Run_L)
        {
            if (velocity.x > -dashSpd)
            {
                velocity.x = Mathf.MoveTowards(
                    velocity.x,
                    -dashSpd,
                    dashAcc * Time.fixedDeltaTime);
            }
        }
        rb.linearVelocity = velocity; //최종 적용
    }

    private void FixedUpdate_Jump()
    {
        if (canJump == false) return;
        if (state == State.Jump_L || state == State.Jump_R)
        {
            canJump = false;

            Vector2 velocity = rb.linearVelocity;

            velocity.y = jumpForce;
            rb.linearVelocity = velocity;
            print("점프실행완료");
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Ground"))
        {
            canJump = true;
            isGrounded = true;
        }
    }


    private void OnDrawGizmos()
    {
        if (rb == null) return;


        Vector2 velocity = rb.linearVelocity;

        GUIStyle style = GUIStyle.none;
        style.normal.textColor = Color.black;
        style.alignment = TextAnchor.MiddleCenter;

        string str = "속도 : " + velocity.x.ToString();
        str += "\n상태 : " + state.ToString();
        str += "\ncanJump : " + canJump.ToString();
        str += "\nisGrounded : " + isGrounded.ToString();

        Handles.Label((Vector2)transform.position + new Vector2(0, 1.25f), str, style);
    }
}
