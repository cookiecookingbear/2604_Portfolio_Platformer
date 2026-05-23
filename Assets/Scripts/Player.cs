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

    [SerializeField, Range(4.0f, 10.0f)] private float walkSpd = 5.0f;
    [SerializeField, Tooltip("가속도, 추천 : moveSpeed*0.2")] private float accelerate = 1.0f;
    [SerializeField, Tooltip("브레이크 속도, 추천 : moveSpeed*3")] private float breakSpd = 15.0f;
    


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
    }

    private void Update_State()
    {
        state = State.Idle_R;

        if (Input.GetKey(KeyCode.RightArrow))
        {
            state = State.Walk_R;
            print("우로걸어");
            //TODO : anim추가
        }else if (Input.GetKey(KeyCode.LeftArrow))
        {
            state = State.Walk_L;
            print("좌로걸어");
        }

    }

    private void FixedUpdate_Move()
    {
        Vector2 velocity = rb.linearVelocity;

        if(state == State.Walk_R)
        {
            if(velocity.x < walkSpd)
            {
                velocity.x = Mathf.MoveTowards(
                    velocity.x,
                    walkSpd,
                    accelerate * Time.fixedDeltaTime);
            }
        }else if (state == State.Walk_L)
        {
            if (velocity.x > -walkSpd)
            {
                velocity.x = Mathf.MoveTowards(
                    velocity.x,
                    -walkSpd,
                    accelerate * Time.fixedDeltaTime);
            }
        }
            rb.linearVelocity = velocity;
    }

    


    private void OnDrawGizmos()
    {
        if (rb == null) return;


        Vector2 velocity = rb.linearVelocity;

        GUIStyle style = GUIStyle.none;
        style.normal.textColor = Color.black;
        style.alignment = TextAnchor.MiddleCenter;

        string str = velocity.x.ToString();

        Handles.Label(transform.position, str, style);
    }
}
