using UnityEngine;
using System;

/// <summary>
/// ������ ���� ���¸� �����ϴ� Ŭ�����Դϴ�.
/// - �̱��� �������� �������� ���� ����
/// - ���� ���� �� �̺�Ʈ �߻�
/// </summary>
public class GameStateManager : MonoBehaviour
{
    /// <summary>
    /// �̱��� �ν��Ͻ� (���� ������ ���� ���� ����)
    /// </summary>
    public static GameStateManager Instance { get; private set; }

    /// <summary>
    /// ���� ���� ���¸� �����ϴ� ����
    /// </summary>
    private GameState currentState;

    /// <summary>
    /// ���� ���¸� �ܺο��� �б� �������� ���� �����ϰ� �ϴ� ������Ƽ
    /// </summary>
    public GameState CurrentState => currentState;

    /// <summary>
    /// ���°� ����� �� ȣ��Ǵ� �̺�Ʈ
    /// - �ٸ� ������Ʈ���� ���� ������ �����ؼ� ������ �� �ְ� ����
    /// </summary>
    public event Action<GameState> OnGameStateChanged;

    /// <summary>
    /// ��ü�� ������ �� ȣ���. �̱��� �ν��Ͻ��� �����ϰ�, �ߺ� ������ ó���մϴ�.
    /// </summary>
    private void Awake()
    {
        // �̹� �ν��Ͻ��� ������ ��� �ڽ��� ����
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }

        // �̱��� �ν��Ͻ� ����
        Instance = this;

        // �� ��ȯ �ÿ��� �ı����� �ʵ��� ����
        DontDestroyOnLoad(gameObject);

        // �⺻ ���� ���� (��: Ž�� ��)
        currentState = GameState.FreeRoam;
    }

    /// <summary>
    /// ���ο� ���·� �����ϴ� �Լ�
    /// </summary>
    /// <param name="newState">������ ���� ����</param>
    public void SetState(GameState newState)
    {
        // �̹� ���� ���¶�� �������� ����
        if (currentState == newState)
            return;

        // ���� ����
        currentState = newState;
        Debug.Log($"[GameStateManager] ���� ����: {newState}");

        // �����ڵ鿡�� ���� ���� �˸�
        OnGameStateChanged?.Invoke(newState);
    }

    /// <summary>
    /// ���� ���°� ������ ���¿� ������ Ȯ���ϴ� �Լ�
    /// </summary>
    /// <param name="state">���� ����</param>
    /// <returns>������ true, �ٸ��� false</returns>
    public bool Is(GameState state) => currentState == state;
}
