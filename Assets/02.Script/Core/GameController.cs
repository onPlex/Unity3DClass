using UnityEngine;
using UnityEngine.InputSystem; // ✅ 새 Input System을 사용하기 위한 네임스페이스

/// <summary>
/// GameController (학생용 단순 버전)
/// - 역할: "Pause" 입력이 들어오면 게임 상태를 일시정지/해제로 토글
/// - 전제: 씬에 GameStateManager가 1개 존재해야 합니다.
/// - 사용법:
///   1) Project Settings > Input System > Project-Wide Actions 에 액션 에셋이 연결되어 있어야 함
///   2) 그 에셋에 "Pause" 액션이 있어야 함 (예: UI/Pause 또는 Player/Pause)
///   3) 이 스크립트를 아무 GameObject에 붙이고, 인스펙터의 Pause Action 슬롯에 해당 액션을 드래그해서 연결
/// </summary>
public class GameController : MonoBehaviour
{
    [Header("Input")]
    [Tooltip("Project-Wide Actions에서 만든 'Pause' 액션을 드래그해서 연결하세요. (예: UI/Pause 또는 Player/Pause)")]
    public InputActionReference pauseAction;
    // 👆 InputActionReference
    // - 인스펙터에서 액션(에셋 안의 특정 액션)을 끌어다 넣는 '레퍼런스' 타입
    // - 코드에서 직접 경로를 찾을 필요 없이, 드래그만 하면 연결됨
    // - 액션은 Project-Wide Actions에 들어있는 걸 쓰는 걸 권장

    [Header("Cursor")]
    [Tooltip("일시정지 시 커서를 보여주고, 재개 시 숨깁니다.")]
    public bool controlCursor = true;
    
    [Header("Pause Settings")]
    [Tooltip("일시정지 시 게임 시간을 멈출지 여부 (Time.timeScale 제어)")]
    public bool controlTimeScale = true;

    [Tooltip("일시정지 시 적용할 Time.timeScale 값 (0이면 완전 정지)")]
    [Range(0f, 1f)] public float pausedTimeScale = 0f;

    [Tooltip("플레이어 입력을 관리하는 PlayerInput (없으면 자동으로 검색 시도)")]
    public PlayerInput playerInput;

    [Tooltip("플레이 중 사용할 액션맵 이름 (예: 'Player')")]
    public string gameplayActionMapName = "Player";

    [Tooltip("일시정지 중 사용할 액션맵 이름 (예: 'UI')")]
    public string uiActionMapName = "UI";

    [Tooltip("일시정지 시 UI 액션맵으로 전환하고, 해제 시 게임플레이 액션맵으로 복귀")]
    public bool switchActionMapOnPause = true;

    [Tooltip("일시정지 동안 비활성화할 이동/조작 컴포넌트들 (선택)")]
    public MonoBehaviour[] movementComponentsToDisable;
    // 👆 UI 조작을 위해 일시정지 중에는 커서를 보이게 하고,
    //    게임 재개 시에는 커서를 숨기고 잠그는 옵션

    private void OnEnable()
    {
        // 1) 인스펙터에서 Pause 액션을 안 넣었거나 비어 있다면 경고 출력 후 기능 중단
        if (pauseAction == null || pauseAction.action == null)
        {
            Debug.LogWarning("[GameController] Pause 액션이 연결되지 않았습니다. 인스펙터에서 Pause 액션을 드래그하세요.");
            return;
        }

        // PlayerInput 자동 검색 (선택)
        if (playerInput == null)
        {
            playerInput = FindFirstObjectByType<PlayerInput>(FindObjectsInactive.Include);
        }

        // 2) 액션을 Enable 해야 입력 이벤트가 발생함
        pauseAction.action.Enable();

        // 3) performed는 "해당 액션이 수행되었을 때" 한 번 불리는 이벤트
        //    예: 버튼을 눌렀을 때, 키를 눌렀을 때 등
        pauseAction.action.performed += OnPause;
    }

    private void OnDisable()
    {
        // OnEnable에서 등록했던 이벤트는 꼭 해제!
        // (안 하면, 오브젝트가 비활성화되어도 이벤트가 계속 불릴 수 있음)
        if (pauseAction != null && pauseAction.action != null)
        {
            pauseAction.action.performed -= OnPause;
            pauseAction.action.Disable(); // 더 이상 입력을 받지 않게 끔
        }

        // 안전장치: 비활성화 시 게임 시간이 멈춰있지 않도록 복구
        if (controlTimeScale && Time.timeScale != 1f)
        {
            Time.timeScale = 1f;
        }
    }

    /// <summary>
    /// Pause 입력이 들어왔을 때 호출되는 함수
    /// - GameStateManager의 상태를 FreeRoam <-> Paused로 토글
    /// - 커서 처리 옵션(controlCursor)에 따라 표시/숨김
    /// </summary>
    /// <param name="_">입력 콜백 컨텍스트(이번 예제에서는 사용하지 않음)</param>
    private void OnPause(InputAction.CallbackContext _)
    {
        // 현재 일시정지 상태를 확인 후 토글 적용
        bool currentlyPaused = GameStateManager.Instance != null && GameStateManager.Instance.Is(GameState.Paused);
        ApplyPause(!currentlyPaused);
    }

    /// <summary>
    /// 실제 일시정지 적용 로직
    /// - GameState 전환
    /// - Time.timeScale 제어(옵션)
    /// - 액션맵 전환 또는 입력 제한
    /// - 커서 표시/숨김
    /// </summary>
    /// <param name="pause">true: 일시정지, false: 재개</param>
    private void ApplyPause(bool pause)
    {
        // 1) 게임 상태 전환
        if (GameStateManager.Instance != null)
        {
            GameStateManager.Instance.SetState(pause ? GameState.Paused : GameState.FreeRoam);
        }
        else
        {
            Debug.LogWarning("[GameController] GameStateManager가 없어도 일시정지 처리를 진행합니다.");
        }

        // 2) 시간 정지/복구
        if (controlTimeScale)
        {
            Time.timeScale = pause ? pausedTimeScale : 1f;
        }

        // 3) 입력 제한 처리
        RestrictPlayerInput(pause);

        // 4) 커서 처리
        if (controlCursor)
        {
            Cursor.lockState = pause ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible   = pause;
        }
    }

    /// <summary>
    /// 플레이어 입력을 일시정지/재개에 맞게 제한 (쉬운 버전)
    /// - PlayerInput 액션맵 전환 또는 Enable/Disable
    /// - 선택된 이동/조작 컴포넌트 비활성화 처리
    /// </summary>
    private void RestrictPlayerInput(bool pause)
    {
        // PlayerInput이 존재할 경우
        if (playerInput != null)
        {
            // 액션맵 전환을 사용할 때
            if (switchActionMapOnPause)
            {
                // 전환하려는 액션맵 이름 선택
                string mapName = pause ? uiActionMapName : gameplayActionMapName;

                // 액션맵이 존재하면 전환
                if (!string.IsNullOrEmpty(mapName)
                    && playerInput.actions != null
                    && playerInput.actions.FindActionMap(mapName, false) != null)
                {
                    playerInput.SwitchCurrentActionMap(mapName);
                }
                else
                {
                    // 액션맵이 없으면 Enable/Disable로 처리
                    playerInput.enabled = !pause;
                }
            }
            else
            {
                // 전환 기능을 사용하지 않으면 Enable/Disable만 처리
                playerInput.enabled = !pause;
            }
        }

        // 이동/조작 컴포넌트 활성화/비활성화
        if (movementComponentsToDisable != null)
        {
            for (int i = 0; i < movementComponentsToDisable.Length; i++)
            {
                if (movementComponentsToDisable[i] != null)
                    movementComponentsToDisable[i].enabled = !pause;
            }
        }
    }
}
