# 포폴일지
- 1일차 (5/23)
    - 만든것
        1. 기본 바닥
      2. 캐릭터 오브젝트
      3. 캐릭터 스크립트
          > 캐릭터 상태 enum, 최고속도, 가속도 선언\
          > 디버깅 및 속도 확인 위한 OnDrawGizmos() 적용.\
          > 입력에 따른 가속도 적용 (Mathf.MoveTowards() 이용)
      
    - 문제점
        1. 캐릭터 스크립트를 만들다가, 구버전 입력으로 좌우 이동까지는 성공했으나, 적절히 멈추는 구현을 rigidbody2d의 linear damping으로 처리하려 했으나 떨어지는 속도까지 마찰걸리는 것처럼 천천히 떨어지는 문제점 발생.
    - 해결
      1. (2일차 해결) 점프 한것까지 천천히 떨어질 수는 없으므로, 브레이크 가속(음수)를 따로 선언하여 입력이 없으면 멈추는 식으로 해결해야 할 것으로 보임.
    - 알게된 것
        1. if문을 여러 번 사용하지 않더라도, Mathf.MoveTowards()를 이용하면 한줄로 해결이 가능하다.\
           가속도를 이용하여 목표 속도에 도달하도록 구현하는 코드를 적을 때,
           >```csharp
           >if(velocity.x<목표속도){
           >velocity.x += 가속도 * Time.fixedDeltaTime;}
           처럼 작성하면, 목표속도보다 실제 속도가 더 커지는 경우가 발생할 수 있다. 따라서, 아래와 같이 작성하면 그런 오류를 방지할 수 있다.
           >```csharp
           >Mathf.MoveTowards(현재속도, 목표속도, 가속도 * Time.fixedDeltaTime)
        2. GetComponent\<T>를 자꾸 Start()에다 쓰려고 하는데, Awake()가 관행상 더 알맞은 위치다.
            > 스크립트 실행 시 Awake() -> OnEnable() -> Start() 순으로 실행되므로, Start()에서 GetComponent\<T>를 쓰면 다른 스크립트에서 Start()보다 먼저 실행되는 경우 NullReferenceException이 발생할 수 있다.
        3. OnDrawGizmos()에서 rigidbody2d등을 이용 할 때, 스크립트에서 GetComponent\<T>한 것 때문에 에디터 상에서 얘 널이라고 자꾸 징징대면, if(rb == null) return;으로 조용하게 만들어 줄 수 있다.

- 2일차(5/24)
    - 만든 것
        1. 캐릭터 스크립트 업데이트
            > 대쉬 및 점프 상태 구현\
            > 대쉬 최고속도, 대쉬 가속도, 브레이크 가속도 선언\
            > 바라보던 방향(좌/우) 유지 구현

    - 문제점
        1. idle상태에서 점프가 안되는 문제점 발생
        1. 점프 상태가 유지되지 않는 문제점 발생
        1. 점프가 입력하는 대로 실행되지 않는 문제점 발생
        1. 최초 실행 시 점프가 마구마구 실행되는 문제점 발생
        1. 낙하 속도 조절을 위해 Gravity Scale을 조정하였더니 좌우 이동속도까지 줄어들어 거의 움직이지 못하는 문제점 발생
        1. (미결)가끔가다가 에디터에서 테스트하려고 하면, player 오브젝트가 destroy되었는데 자꾸 접근을 시도한다면서 에러 띄우고 입력이 안들어가는데, 현재는 에디터를 끄고 키는 것으로 해결 중이나(에디터 자체 버그로 판단) 정확한 원인을 파악하지 못했다.
    - 해결
        - 이전 문제 해결 
            1. linear damping이 좌우이동, 점프, 낙하 속도에 모두 영향을 끼치는 관계로 breakAcc를 따로 선언하여 다른 이동속도 관리와 마찬가지로 Mathf.MoveTowards()를 사용하여 감속하게끔 변경
        - 오늘 문제 해결
            1. 이는 좌우 이동중에만 점프 상태로 들어갈 수 있게끔 코드를 작성해서 발생한 문제. 다만, 점프 키를 입력받는 조건문에서 점프 상태 전체를 관리하면 다른 상태와의 연동에 문제가 생기는 관계로, 땅에 착지해 있는 상태를 나타내는 isGrounded를 따로 선언하여 isGrounded만 false로 바꾸고, 상태 변경은 각 상태에서 파생될 수 있게끔 변경하여 해결.
            1. Update()에서 호출되는 Update_State()의 첫 구문이 상태를 계속 덮어쓰고 있었음.
            1. 최초 작성 시 점프 입력을 Update_State()에서 받는게 아닌 FixedUpdate_Jump()에서 받게끔 하는 실수를 하여 이후 해결함.
            1. FixedUpdate_Jump()에서 canJump가 false면 아예 return하게끔 하게 했었는데, canJump를 true로 선언하는 바람에 문제가 발생했었음.
            1. 조사 결과 바닥 오브젝트의 Rigidbody2D의 Physics Material의 None상태의 friction(마찰력)이 0.4로 설정되어 있어 무거워짐에 따라 마찰력이 크게 작용되어 좌우이동도 어려워지는 상황이 발생했었음. 2D-physics matrial2D에서 friction을 0으로 설정, 적용하여 해결함.
    - 알게된 것
        - 기존 플랫포머에서는 나처럼 gravity scale이 이동에 영향을 주지 않게끔 바닥의 마찰을 보통 0으로 설정하고, 감속 등의 속도 조절은 스크립트 단계에서 해결한다고 함.
        - 현재 점프 상태에서는 좌우 이동방향을 조절할 수 없는 현상이 있다. 이게 맞는 상태이지만, '게임적'인 표현을 위해 공중에서의 가속을 추가하여 공중에서도 어느정도 이동방향을 조절할 수 있게끔 변경할 예정. 생각해보면 마리오에서도 공중에서 방향전환이 가능하다. 단, 이 가속은 지면에서의 가속보다 적어야 한다.
        - 점프 상태를 제어하는 isGrounded변수명이 isLanded보다 관용적인 표현이라고 한다. 정확히는 isLanded는 착지 '이벤트'를 관리하는 변수명으로 주로 사용된다고 한다.
- 3일차(5/25)
    - 만든 것
        1. 플레이어 스크립트 업데이트
            1. enum State가 각 상태별 L,R을 가지고 있던 기존 구조에서 State는 각 상태만, 방향은 int direction(우=1, 좌=-1, 미입력 = 0)을 별도로 선언하여 관리
                > 이하 FixedUpdate_Move 등의 메서드에서 L,R별 조건 분기를 나누지 않고 direction을 곱하여 자연스럽게 방향까지 지정할 수 있도록 구조 변경
            1. 점프 상태 중 이동방향 변경을 위해 jumpWalkAcc, jumpDashAcc, isDashing(bool) 추가
                > 점프 상태 중에서도 z키 입력에 따라 점프이동가속, 점프대쉬가속을 분리
            1. 위의 변경점을 반영한 기존 메서드 리팩터링
    - 문제점
        1. FixedUpdate_Jump()안에 점프 상태에서 좌우 이동방향 변경을 위한 코드를 작성했더니, Update_State()에서 점프 상태를 받는 조건문이 GetKeyDown으로 작성되어 상태 자체는 업데이트되나 실제로 반영되지 않는 문제점 발생.
        1. 같은 코드가 좌,우 방향에 따라 과도하게 반복되는 문제점 발생.
        1. Update_State() 내부에서 조건문 안의 내용이 과도하게 반복되고, 이에 따라 가독성이 현저하게 떨어지는 문제점 발생.
    - 해결
        1. 책임 분리를 위해 FixedUpdate_Jump()에서는 점프 구현 까지만 담당, 점프 상태에서의 이동방향 변경은 FixedUpdate_InAirMove()를 새로 정의하여 담당하도록 작성.
        1. 기존 State enum에서 각 상태별 좌우 방향까지 모두 담당하던 구조를 State에서는 상태만 담당, 방향은 direction(int, 우=1, 좌=-1, 미입력=0)으로 별도 선언하는 구조로 변경하여 코드 반복을 줄이고 구현 메서드에서 굳이 방향을 조건문으로 지정하지 않아도 *direction하여 방향까지 반영할 수 있도록 변경.
        1. 메서드 전반부에서는 필요한 각 bool 변수를 관리하고, 후반부에서는 관리된 변수들을 토대로 State를 결정하는 식으로 변경. 방향에 대해서는 Update_Direction()을 새로 정의하여 방향에 대해서만 관리하도록 변경하여 가독성 개선.
    - TODO
        1. 코요테타임 추가
- 4일차(5/26)
    - 만든 것
        1. 플레이어 스크립트 업데이트
            1. direction을 Input.GetAxisRaw("Horizontal")로 받게끔 변경
                > 기존엔 구버전 입력을 정확하게 몰라 좌우 입력, 미입력 상태에 따라 direction을 int로 받아 전환되게끔 구현했으나, GetAxisRaw로 받게 되면 키보드뿐만 아니라 다른 입력 방법으로도 조작이 가능함을 알게 되어 변경함.
            1. inAirMove()에서 공통조건인 state==State.Jump를 따로 떼서 최상단에 state !=jump시 return하도록 변경
            1. 2단 점프 구현
                > 이를 위해 jumpCount(int) 선언
            1. 점프 입력 구현을 기존 state를 받아 호출되었던 것을 jumpRequested(bool)을 별도로 선언하여 이를 바탕으로 점프가 호출되게끔 변경
            1. 기타 수치 조정
    - 문제점
        1. 2단점프 구현 중 문제
            1. 내용
                > 2단 점프를 구현하기 위해 jumpCount(int)를 도입하고 FixedUpdate_Jump()에 추가했더니, 한 번의 점프 입력에도 jumpCount가 설정한 한계까지 즉시 오르는 문제점 발생
            1. 원인
                > FixedUpdate_Jump()에서 점프를 실행하는 조건이 state==State.Jump인 것이 문제. 정확히는 Update_State()에서 점프 입력시 state를 Jump로 만들어버리니 착지하기 이전까지 FixedUpdate_Jump()의 구현부가 호출되어 버리는 것이 원인.
                ```csharp
                private void FixedUpdate_Jump(){
                if(canJump == false) return;
                if(state == State.Jump)
                {
                ConsumeJump();
                Vector2 velocity = rb.linearVelocity;
                velocity.y = jumpForce;
                rb.linearVelocity = velocity;
                }
                }
                ```
            1. 해결
                > jumpRequested(bool)과 ConsumeJump()를 추가 선언, 정의하여 점프 입력시 jumpRequested=true가 되고 점프 구현이 false로 전환되게끔 하여 해결. ConsumeJump()에서는 jumpCount와 canJump를 관리한다.
                ```csharp
                private void FixedUpdate_Jump(){
                    if(canJump == false) return;
                    if(jumpRequested == false) return;
                    ConsumeJump();
                    Vector2 velocity = rb.linearVelocity;
                    velocity.y = jumpForce;
                    rb.linearVelocity = velocity;
                    jumpRequested = false;
                    }
                }
                ```
        1. isGrounded명확화
            1. 내용
                > 공중에 있는 플랫폼에서 낙하할 경우, isGrounded가 의도와 다르게 true상태로 지속되는 문제점을 발견. 제작할 맵의 플랫폼이 사선으로 배치될 예정이기 때문에 Circle Collider2D의 경우에는 상관없으나 Box Collider2D가 적용될 경우 OnCollisionEnter2D,OnCollisionExit2D을 이용해서는 isGrounded를 정상적으로 검출 할 수 없는 문제점도 발견.
            1. 원인
                > 현재 코드 로직상 점프를 입력해야만 isGrounded가 false로 변하게 되어 있음. 이를 해결하기 위해 OnCollisionEnter2D,OnCollisionExit2D를 도입하였으나 Collider의 형태에 따라 OnCollisionEnter2D,OnCollisionExit2D이 의도한대로 작동하지 않았다.
            1. 해결
                > 추후 OverlapBox를 코드에 추가하여 캐릭터 오브젝트의 발밑에 플랫폼이 있는지 검출하여 isGrounded를 결정할 예정.
    - 알게된 것
        1. Input.GetAxisRaw("Horizontal")의 반환값이 키보드에서는 -1,0,1을 벗어나지 않는 것을 알고 기존의 코드를 유지하려고 했으나, 입력 장치가 키보드가 아닌 다른 것으로 변경되면 -1과 1 사이의 범위 사이의 값이 입력될 수 있는 가능성을 파악하고 변경하였음. 구체적으로는 조이스틱과 같은 입력 장치의 경우에는 실제로 사이의 값이 입력될 수 있다. 이 경우, 좌, 우 이동시 항상 설정된 최고속도로 움직이지 않을 수 있게 된다.
        1. 게임오브젝트 근처에 무엇이 있는가를 검출하기 위해 주로 RayCast, BoxCast, OverlapBox를 이용한다.
        
    - 작업중인 것
        1. 코요테 타임 구현을 위한 isGrounded 명확화 작업
    - TODO
        1. OverlapBox 도입하여 isGrounded 명확화
        1. Codex에 플레이어 캐릭터에 사용할 스프라이트 제작 요청
1. 5일차