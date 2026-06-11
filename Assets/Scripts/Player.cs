using Unity.Collections;
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
        Jump,
        Exhausted
    }

    [SerializeField, Tooltip("일반이동속도 캡")] private float walkSpd = 5.0f;
    [SerializeField, Tooltip("가속도, 추천 : moveSpeed*0.2")] private float walkAcc = 10.0f;
    [SerializeField, Tooltip("브레이크 속도, 추천 : moveSpeed*3")] private float breakAcc = 15.0f;
    [SerializeField, Tooltip("대쉬속도 캡")] private float dashSpd = 8.5f;
    [SerializeField, Tooltip("대쉬 가속도")] private float dashAcc = 20.0f;

    [SerializeField, Tooltip("점프 힘, rb 중력배율 3에서 12.5")] private float jumpForce = 12.5f;
    [SerializeField, Tooltip("점프시 좌우 이동힘, walkAcc보다 작게")] private float jumpWalkAcc = 8.5f;
    [SerializeField, Tooltip("점프시 좌우 대쉬속도 캡, dashSpd보다 작게")] private float jumpDashAcc = 17.5f;

    [SerializeField, Tooltip("방향, 우 = 1, 좌 = -1")] private float direction = 0;
    private int facingDirection = 1;

    public float Direction => direction;
    public int FacingDirection => facingDirection;


    [Header("OverlapBox 관련 변수 3종")]
    [SerializeField, Tooltip("캐릭터 발밑 게임오브젝트")] private Transform groundCheck;
    [SerializeField, Tooltip("검출할 레이어 마스크")] private LayerMask groundMask;
    [SerializeField, Tooltip("검사할 박스 사이즈")] private Vector2 size = new Vector2(0.3f, 0.5f);

    [Header("Coyote Time 관련 변수")]
    [SerializeField, Tooltip("코요테타임, 난이도때문에 굉장히 작게")] private float coyoteTime = 0.1f;
    [SerializeField, Tooltip("코요테타이머, Update안에서 coyoteTime받아서 차감될 변수")] private float coyoteTimer = 0.0f;
    [SerializeField,ReadOnly] private bool coyoteTimeActivated = false;

    [SerializeField, Tooltip("대시게이지")] private float dashGauge = 1.0f;
    [SerializeField, Tooltip("대시 소모량")] private float dashConsume = 0.1f;
    [SerializeField, Tooltip("대시 회복량")] private float dashRecovery = 0.15f;
    [SerializeField, Tooltip("탈진시 대시 회복량")] private float exhaustRecovery = 0.33f;

    private float barrelKnockback = 7.5f;
    private bool dashRequested = false;
    private bool isDashing = false;
    private bool isExhausted = false;
    public float DashGauge => dashGauge;



    //private bool isFacingLeft = false;
    private bool canJump = false;
    //canJump는 "계산 결과"로서 지금 당장 점프가 가능한 상황인지 최종 판별해주는 bool임을 생각할것
    private bool isGrounded = false;

    private int jumpCount = 0;
    private bool jumpRequested = false;
    private float jumpRequestTime = 0.15f;
    private float jumpRequestTimer = 0.0f;


    private State state = State.Idle;
    private Rigidbody2D rb;
    //private Animator animator;

    private float yPos;
    public float YPos => yPos;



    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        //animator = GetComponent<Animator>();
    }

    private void Update()
    {
        Update_Direction();
        Update_JumpRequest();
        Update_CheckGrounded();
        Update_UsingDash();
        Update_RecoveringDash();
        Update_CoyoteTime();
        Update_State();
        Update_Y_Position();
        //Update_RecoveringExhaust();
    }

    private void FixedUpdate()
    {
        FixedUpdate_Move();
        FixedUpdate_Jump();
        FixedUpdate_InAirMove();
    }

    private void Update_Direction()
    {
        if (isExhausted == true) return;

        direction = Input.GetAxisRaw("Horizontal");

        if(direction > 0)
        {
            facingDirection = 1;
        }else if ( direction < 0)
        {
            facingDirection = -1;
        }
    }

    private void Update_State()
    {
        if (isExhausted == true) return;

        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            jumpRequested = true;
            jumpRequestTimer = jumpRequestTime;
        }

        if (Input.GetKey(KeyCode.Z))
        {
            dashRequested = true;
        }
        else dashRequested = false;

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
        //탈진시 속도 0 적용
        if (state == State.Exhausted && isGrounded == true)
        {
            velocity.x = 0;
        }

        rb.linearVelocity = velocity; //최종 적용
    }

    private void FixedUpdate_Jump()
    {
        if (canJump == false ) return;
        if (jumpRequested == false) return;
        if(isExhausted == true) return;
        
        ConsumeJump();

        Vector2 velocity = rb.linearVelocity;

        velocity.y = jumpForce;

        rb.linearVelocity = velocity;

        jumpRequested = false;
    }

    private void FixedUpdate_InAirMove()
    {
        if (state != State.Jump) return;

        Vector2 velocity = rb.linearVelocity;

        if(isDashing == false)
        {
            velocity.x = Mathf.MoveTowards(
                velocity.x,
                direction *walkSpd,
                jumpWalkAcc * Time.fixedDeltaTime);
        }else if (isDashing == true)
        {
            velocity.x  = Mathf.MoveTowards(
                velocity.x,
                direction * dashSpd,
                jumpDashAcc * Time.fixedDeltaTime);
        }
        rb.linearVelocity = velocity;
    }

    private void ConsumeJump()
    {
        jumpCount++;
        coyoteTimeActivated = false;
        coyoteTimer = 0.0f;
        if (jumpCount >= 2)
        {
            canJump = false;
        }
    }

    private void Update_CoyoteTime()
    {
        if (coyoteTimeActivated == false) return;
        
        coyoteTimer -= Time.deltaTime;

        if (coyoteTimer < 0)
        {
            coyoteTimer = 0;
            canJump = false;
        }

    }

    private void CoyoteTimeActivate()
    {
        if (coyoteTimeActivated == false)
        {
            coyoteTimer = coyoteTime;
            coyoteTimeActivated = true;
        }
    }

    private void Update_CheckGrounded()
    {
        if (groundCheck is null) return;

        Collider2D hit = Physics2D.OverlapBox(
            groundCheck.position,
            size,
            0f,
            groundMask);

        isGrounded = hit != null;
        jumpCount = hit != null ? 0 : jumpCount;

        if(canJump == false && hit != null)
        {
            canJump = true;
            //그냥 canJump = hit!=null해버리면 발이 땅에서 떨어지는 순간에 canJump가 false되어 2단점프 구현이 안됨.
        }

        if (coyoteTimeActivated == true &&
            hit != null) 
        {
            coyoteTimeActivated = false;
            //CTA 회복타이밍을 canJump회복타이밍과 동일하게 해버리면, CTA는 반드시 2단점프 후 착지를 하는 상황이 되어야만 회복됨.
            //1단점프후 착지시에는 canJump의 변동이 없다.
        }
        if (coyoteTimeActivated == false 
            && hit == null
            && jumpCount ==0)
        {
            CoyoteTimeActivate();
        }
    }
    
    private void Update_JumpRequest()
    {
        if(jumpRequested == false) return;
        //if(canJump == true) return;

        jumpRequestTimer -= Time.deltaTime;

        if(jumpRequestTimer <= 0)
        {
            jumpRequested = false;
            jumpRequestTimer = 0.0f;
        }
    }
    
    private void Update_UsingDash()
    {
        if (dashRequested == false) return;
        if (isExhausted == true) return;
        if (direction == 0) return;

        isDashing = true;

        dashGauge = Mathf.MoveTowards(
            dashGauge,
            0,
            dashConsume * Time.deltaTime);

        if(dashGauge <= 0)
        {
            Exhausted();
        }
        
    }
    private void Update_RecoveringDash()
    {
        if (dashRequested == true && direction != 0) return;
        if (dashGauge >= 1) return;

        isDashing = false;

        if(isExhausted == false)
        {
            dashGauge = Mathf.MoveTowards(
            dashGauge,
            1,
            dashRecovery * Time.deltaTime);
        }
        else if(isExhausted == true)
        {
            dashGauge = Mathf.MoveTowards(
                        dashGauge,
                        1,
                        exhaustRecovery * Time.deltaTime);
            direction = 0;

            if (dashGauge >= 1)
            {
                isExhausted = false;
            }
        }
    }

    private void Update_Y_Position()
    {
        yPos = transform.position.y;
    }

    private void Exhausted()
    {
        isExhausted = true;
        dashRequested = false;
        isDashing = false;
        state = State.Exhausted;
    }

    //배럴(Barrel)에서 사용되는 메서드, 충돌시 플레이어 튕겨냄
    public void HitbyBarrel(Vector2 normal)
    {
        print("배럴에맞음");

        Vector2 knockback = normal + Vector2.up;
        rb.AddForce(knockback * barrelKnockback, ForceMode2D.Impulse);
    }

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
        str += "\njumpCount : " + jumpCount;

        Handles.Label((Vector2)transform.position + new Vector2(0, 1.25f), str, style);
    }
}
