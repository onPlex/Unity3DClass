using UnityEngine;
using System;

/// <summary>
/// 게임의 전체 상태를 관리하는 클래스입니다.
/// - 싱글톤 패턴으로 전역에서 접근 가능
/// - 상태 변경 시 이벤트 발생
/// </summary>
public class GameStateManager : MonoBehaviour
{
    /// <summary>
    /// 싱글톤 인스턴스 (다른 스크립트에서 접근 가능)
    /// </summary>
    public static GameStateManager Instance { get; private set; }

    /// <summary>
    /// 현재 게임 상태를 저장하는 변수
    /// </summary>
    private GameState currentState;

    /// <summary>
    /// 현재 상태를 외부에서 읽기 전용으로 접근할 수 있는 프로퍼티
    /// </summary>
    public GameState CurrentState => currentState;

    /// <summary>
    /// 상태가 변경될 때 호출되는 이벤트
    /// - 다른 컴포넌트들이 상태 변경을 구독해서 반응할 수 있음
    /// </summary>
    public event Action<GameState> OnGameStateChanged;

    /// <summary>
    /// 객체가 생성될 때 호출됨. 싱글톤 인스턴스를 설정하고, 중복 생성을 처리합니다.
    /// </summary>
    private void Awake()
    {
        // 이미 인스턴스가 존재하면 자신을 파괴
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }

        // 싱글톤 인스턴스 설정
        Instance = this;

        // 씬 전환 시에도 파괴되지 않도록 설정
        DontDestroyOnLoad(gameObject);

        // 기본 상태 설정 (예: 탐색 모드)
        currentState = GameState.FreeRoam;
    }

    /// <summary>
    /// 상태를 새로운 상태로 변경하는 함수
    /// </summary>
    /// <param name="newState">변경할 새로운 상태</param>
    public void SetState(GameState newState)
    {
        // 이미 같은 상태라면 변경하지 않음
        if (currentState == newState)
            return;

        // 상태 변경
        currentState = newState;
        Debug.Log($"[GameStateManager] 상태 변경: {newState}");

        // 구독자들에게 상태 변경 알림
        OnGameStateChanged?.Invoke(newState);
    }

    /// <summary>
    /// 현재 상태가 특정 상태인지 확인하는 함수
    /// </summary>
    /// <param name="state">확인할 상태</param>
    /// <returns>맞으면 true, 아니면 false</returns>
    public bool Is(GameState state) => currentState == state;
}
