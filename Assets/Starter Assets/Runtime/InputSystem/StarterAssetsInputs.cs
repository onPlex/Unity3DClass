using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace StarterAssets
{
    /// <summary>
    /// Unity의 새로운 Input System을 사용하는 입력 관리 클래스
    /// 
    /// [Unity Input System의 특징]
    /// - 기존 Input.GetKey() 방식과 달리 이벤트 기반으로 동작
    /// - 키보드, 게임패드, 모바일 등 다양한 입력 장치를 자동으로 지원
    /// - Input Action Asset에서 입력 매핑을 시각적으로 설정 가능
    /// - 런타임에 입력 장치 변경 시 자동으로 적응
    /// 
    /// [동작 원리]
    /// 1. Input Action Asset에서 입력 액션 정의
    /// 2. C# 스크립트에서 On[ActionName] 메서드로 입력 감지
    /// 3. 입력 값이 변경될 때마다 자동으로 해당 메서드 호출
    /// 4. 입력 값을 public 변수에 저장하여 다른 컴포넌트에서 사용
    /// </summary>
    public class StarterAssetsInputs : MonoBehaviour
    {
        [Header("Character Input Values")]
        [Tooltip("캐릭터의 이동 방향 (WASD 키 입력)")]
        public Vector2 move;         // Vector2: X축(좌우), Y축(앞뒤) 이동 입력
        
        [Tooltip("카메라 회전 방향 (마우스 움직임)")]
        public Vector2 look;         // Vector2: X축(좌우 회전), Y축(상하 회전) 입력
        
        [Tooltip("점프 입력 (스페이스바)")]
        public bool jump;            // bool: 점프 키가 눌렸는지 여부
        
        [Tooltip("달리기 입력 (Shift 키)")]
        public bool sprint;          // bool: 달리기 키가 눌렸는지 여부
        
        [Tooltip("일시정지 입력 (P 키)")]
        public bool pause;           // bool: 일시정지 키가 눌렸는지 여부

        [Header("Movement Settings")]
        [Tooltip("아날로그 이동 활성화 (게임패드 스틱 지원)")]
        public bool analogMovement;  // true: 부드러운 이동, false: 디지털 이동

        [Header("Mouse Cursor Settings")]
        [Tooltip("마우스 커서를 화면 중앙에 고정 (FPS 게임 스타일)")]
        public bool cursorLocked = true;     // true: 커서 고정, false: 커서 자유 이동
        
        [Tooltip("마우스 움직임을 카메라 회전에 사용할지 여부")]
        public bool cursorInputForLook = true;   // true: 마우스로 카메라 회전, false: 마우스 비활성화

#if ENABLE_INPUT_SYSTEM
        /// <summary>
        /// Unity Input System의 Move 액션에서 호출되는 메서드
        /// 
        /// [동작 과정]
        /// 1. Input Action Asset에서 Move 액션이 정의됨
        /// 2. WASD 키나 게임패드 스틱 입력 시 자동으로 호출
        /// 3. value.Get<Vector2>()로 입력 값을 Vector2로 변환
        /// 4. 입력 값을 move 변수에 저장하여 다른 컴포넌트에서 사용
        /// 
        /// [InputValue의 특징]
        /// - Unity Input System에서 제공하는 입력 값 래퍼
        /// - Get<T>() 메서드로 원하는 타입으로 변환 가능
        /// - isPressed, wasPressedThisFrame 등 다양한 입력 상태 제공
        /// </summary>
        /// <param name="value">Input System에서 전달하는 입력 값</param>
        public void OnMove(InputValue value)
        {
            MoveInput(value.Get<Vector2>());
        }

        /// <summary>
        /// Unity Input System의 Look 액션에서 호출되는 메서드
        /// 
        /// [마우스 입력 처리]
        /// - 마우스 움직임을 카메라 회전으로 변환
        /// - cursorInputForLook이 false면 카메라 회전 비활성화
        /// - 게임패드의 오른쪽 스틱으로도 동작 가능
        /// </summary>
        /// <param name="value">마우스/게임패드 스틱 입력 값</param>
        public void OnLook(InputValue value)
        {
            if(cursorInputForLook)
            {
                LookInput(value.Get<Vector2>());
            }
        }

        /// <summary>
        /// Unity Input System의 Jump 액션에서 호출되는 메서드
        /// 
        /// [점프 입력 처리]
        /// - 스페이스바나 게임패드 버튼 입력 시 호출
        /// - value.isPressed로 키가 눌렸는지 여부 확인
        /// - true: 키를 누름, false: 키를 뗌
        /// </summary>
        /// <param name="value">점프 입력 값 (눌림/뗌 상태)</param>
        public void OnJump(InputValue value)
        {
            JumpInput(value.isPressed);
        }

        /// <summary>
        /// Unity Input System의 Sprint 액션에서 호출되는 메서드
        /// 
        /// [달리기 입력 처리]
        /// - Shift 키나 게임패드 버튼 입력 시 호출
        /// - value.isPressed로 키 상태 확인
        /// - true: 달리기, false: 걷기
        /// </summary>
        /// <param name="value">달리기 입력 값</param>
        public void OnSprint(InputValue value)
        {
            SprintInput(value.isPressed);
        }
        
        /// <summary>
        /// Unity Input System의 Pause 액션에서 호출되는 메서드
        /// 
        /// [일시정지 입력 처리]
        /// - ESC 키나 게임패드 버튼 입력 시 호출
        /// - value.isPressed로 키 상태 확인
        /// - true: 일시정지 키를 누름, false: 키를 뗌
        /// 
        /// [게임 상태와의 연동]
        /// - GameStateManager에서 이 값을 모니터링하여 일시정지 상태 전환
        /// - UIManager를 통해 일시정지 UI 표시
        /// - Time.timeScale을 0으로 설정하여 게임 시간 정지
        /// </summary>
        /// <param name="value">일시정지 입력 값 (눌림/뗌 상태)</param>
        public void OnPause(InputValue value)
        {
            PauseInput(value.isPressed);
        }
#endif

        /// <summary>
        /// 이동 입력을 처리하는 메서드
        /// 
        /// [Vector2 입력 값의 의미]
        /// - X축: -1(왼쪽) ~ 0(중앙) ~ +1(오른쪽)
        /// - Y축: -1(뒤) ~ 0(중앙) ~ +1(앞)
        /// - 정규화된 벡터로 방향만 표현, 크기는 항상 1 이하
        /// 
        /// [사용 예시]
        /// - move = (0, 1): 앞으로 이동
        /// - move = (1, 0): 오른쪽으로 이동
        /// - move = (0.7, 0.7): 대각선 이동 (정규화됨)
        /// </summary>
        /// <param name="newMoveDirection">새로운 이동 방향 (Vector2)</param>
        public void MoveInput(Vector2 newMoveDirection)
        {
            move = newMoveDirection;
        } 

        /// <summary>
        /// 카메라 회전 입력을 처리하는 메서드
        /// 
        /// [마우스 입력 처리]
        /// - 마우스 움직임을 카메라 회전으로 변환
        /// - X축: 좌우 회전 (Yaw)
        /// - Y축: 상하 회전 (Pitch)
        /// 
        /// [게임패드 스틱 지원]
        /// - 오른쪽 스틱으로도 카메라 회전 가능
        /// - analogMovement와 연동하여 부드러운 회전 지원
        /// </summary>
        /// <param name="newLookDirection">새로운 카메라 회전 방향</param>
        public void LookInput(Vector2 newLookDirection)
        {
            look = newLookDirection;
        }

        /// <summary>
        /// 점프 입력을 처리하는 메서드
        /// 
        /// [점프 시스템]
        /// - true: 점프 시작 (키를 누름)
        /// - false: 점프 종료 (키를 뗌)
        /// 
        /// [ThirdPersonController와의 연동]
        /// - 이 변수를 ThirdPersonController에서 읽어서 점프 처리
        /// - 점프 높이, 점프 타이머 등은 ThirdPersonController에서 관리
        /// </summary>
        /// <param name="newJumpState">새로운 점프 상태</param>
        public void JumpInput(bool newJumpState)
        {
            jump = newJumpState;
        }

        /// <summary>
        /// 달리기 입력을 처리하는 메서드
        /// 
        /// [달리기 시스템]
        /// - true: 달리기 모드 (빠른 속도)
        /// - false: 걷기 모드 (일반 속도)
        /// 
        /// [ThirdPersonController와의 연동]
        /// - MoveSpeed vs SprintSpeed로 속도 차이 적용
        /// - 애니메이션 블렌딩에도 영향을 줌
        /// </summary>
        /// <param name="newSprintState">새로운 달리기 상태</param>
        public void SprintInput(bool newSprintState)
        {
            sprint = newSprintState;
        }
        
        /// <summary>
        /// 일시정지 입력을 처리하는 메서드
        /// 
        /// [일시정지 시스템]
        /// - true: 일시정지 요청 (ESC 키를 누름)
        /// - false: 일시정지 해제 요청 (ESC 키를 뗌)
        /// 
        /// [입력 처리 방식]
        /// - 단일 키 입력으로 토글 방식 동작
        /// - GameController에서 이 값을 모니터링하여 GameState 변경
        /// - 일시정지 상태에서는 모든 게임 로직 정지 (Time.timeScale = 0)
        /// 
        /// [주의사항]
        /// - 입력 후 자동으로 false로 리셋되어야 함 (GameController에서 처리)
        /// - 일시정지 중에도 ESC 키로 일시정지 해제 가능
        /// </summary>
        /// <param name="newPauseState">새로운 일시정지 상태</param>
        public void PauseInput(bool newPauseState)
        {
            pause = newPauseState;
        }
        
        /// <summary>
        /// 애플리케이션 포커스 변경 시 호출되는 메서드
        /// 
        /// [포커스 변경 시나리오]
        /// - 게임 창을 클릭하여 활성화할 때
        /// - Alt+Tab으로 다른 프로그램으로 전환할 때
        /// - 게임 창을 최소화했다가 복원할 때
        /// 
        /// [커서 상태 관리]
        /// - 게임 중: 커서 숨김 및 고정 (FPS 스타일)
        /// - 게임 외: 커서 표시 및 자유 이동 (메뉴 조작)
        /// </summary>
        /// <param name="hasFocus">게임 창이 포커스를 받았는지 여부</param>
        private void OnApplicationFocus(bool hasFocus)
        {
            SetCursorState(cursorLocked);
        }

        /// <summary>
        /// 마우스 커서의 상태를 설정하는 메서드
        /// 
        /// [커서 상태 옵션]
        /// - CursorLockMode.Locked: 커서를 화면 중앙에 고정
        /// - CursorLockMode.None: 커서를 자유롭게 이동 가능
        /// 
        /// [게임플레이에서의 의미]
        /// - Locked: FPS 게임처럼 마우스로 시점 조작
        /// - None: 메뉴 조작이나 UI 인터랙션
        /// 
        /// [주의사항]
        /// - cursorLocked = true일 때도 ESC 키로 일시정지 시 커서 해제 필요
        /// - 게임 상태에 따라 동적으로 커서 상태 변경 가능
        /// </summary>
        /// <param name="newState">새로운 커서 상태</param>
        private void SetCursorState(bool newState)
        {
            Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
        }
    }
}