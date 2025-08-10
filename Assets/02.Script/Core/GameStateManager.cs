using UnityEngine;
using System;

/// <summary>
/// 게임의 현재 상태를 관리하는 클래스입니다.
/// - 싱글톤 패턴으로 전역에서 접근 가능
/// - 상태 변경 시 이벤트 발생
/// </summary>
public class GameStateManager : MonoBehaviour
{
    /// <summary>
    /// 싱글톤 인스턴스 (전역 접근을 위한 정적 변수)
    /// </summary>
    public static GameStateManager Instance { get; private set; }

    /// <summary>
    /// 현재 게임 상태를 저장하는 변수
    /// </summary>
    private GameState currentState;

    /// <summary>
    /// 현재 상태를 외부에서 읽기 전용으로 접근 가능하게 하는 프로퍼티
    /// </summary>
    public GameState CurrentState => currentState;

    /// <summary>
    /// 상태가 변경될 때 호출되는 이벤트
    /// - 다른 오브젝트들이 상태 변경을 구독해서 반응할 수 있게 해줌
    /// </summary>
    public event Action<GameState> OnGameStateChanged;

    /// <summary>
    /// 객체가 생성될 때 호출됨. 싱글톤 인스턴스를 설정하고, 중복 방지를 처리합니다.
    /// </summary>
    private void Awake()
    {
        // 이미 인스턴스가 존재할 경우 자신을 제거
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }

        // 싱글톤 인스턴스 지정
        Instance = this;

        // 씬 전환 시에도 파괴되지 않도록 설정
        DontDestroyOnLoad(gameObject);

        // 기본 상태 설정 (예: 탐험 중)
        currentState = GameState.FreeRoam;
    }

    /// <summary>
    /// 새로운 상태로 변경하는 함수
    /// </summary>
    /// <param name="newState">변경할 게임 상태</param>
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
    /// 현재 상태가 지정된 상태와 같은지 확인하는 함수
    /// </summary>
    /// <param name="state">비교할 상태</param>
    /// <returns>같으면 true, 다르면 false</returns>
    public bool Is(GameState state) => currentState == state;
}
