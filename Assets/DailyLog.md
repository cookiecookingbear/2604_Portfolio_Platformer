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
      1. (미결) 점프 한것까지 천천히 떨어질 수는 없으므로, 브레이크 가속(음수)를 따로 선언하여 입력이 없으면 멈추는 식으로 해결해야 할 것으로 보임.
    - 알게된 것
        1. if문을 여러 번 사용하지 않더라도, Mathf.MoveTowards()를 이용하면 한줄로 해결이 가능하다.
            >Mathf.MoveTowards(현재속도, 목표속도, 가속도 * Time.deltaTime)
        2. GetComponent\<T>를 자꾸 Start()에다 쓰려고 하는데, Awake()가 관행상 더 알맞은 위치다.
            > 스크립트 실행 시 Awake() -> OnEnable() -> Start() 순으로 실행되므로, Start()에서 GetComponent\<T>를 쓰면 다른 스크립트에서 Start()보다 먼저 실행되는 경우 NullReferenceException이 발생할 수 있다.
        3. OnDrawGizmos()에서 rigidbody2d등을 이용 할 때, 스크립트에서 GetComponent\<T>한 것 때문에 에디터 상에서 얘 널이라고 자꾸 징징대면, if(rb == null) return;으로 조용하게 만들어 줄 수 있다.

- 2일차
            