using System;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// ���� �ֵ�Ƽ��(Additive) ������� �ε��ϰ�,
/// �̱��� �������� ���� ������ ������ SceneLoader Ŭ�����Դϴ�.
/// </summary>
public class SceneLoader : MonoBehaviour
{
    /// <summary>
    /// SceneLoader�� ���� �ν��Ͻ� (�̱���)
    /// </summary>
    public static SceneLoader Instance { get; private set; }

    /// <summary>
    /// ��ü�� ������ �� ȣ��˴ϴ�. �̱��� �ν��Ͻ��� �����ϰ�, �ߺ� ���� ó���� �����մϴ�.
    /// </summary>
    private void Awake()
    {
        // �̹� �ν��Ͻ��� �����ϸ� �ڽ��� �ı��ϰ� ����
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }

        // ���� �ν��Ͻ��� �̱������� ����
        Instance = this;

        // �� ��ȯ �ÿ��� �ı����� �ʵ��� ����
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// ������ �̸��� ���� Additive ���� �ε��մϴ�.
    /// �̹� �ε�� ���̶�� �ݹ鸸 ȣ���մϴ�.
    /// </summary>
    /// <param name="sceneName">�ε��� ���� �̸�</param>
    /// <param name="onSceneLoaded">�� �ε� �Ϸ� �� ������ �ݹ�</param>
    public void LoadSceneAdditive(string sceneName, Action onSceneLoaded)
    {
        // �̹� ���� �ε�Ǿ� �ִٸ�, ���� �ٽ� �ε����� �ʰ� �ݹ鸸 ����
        if (SceneManager.GetSceneByName(sceneName).isLoaded)
        {
            Debug.Log($"Scene {sceneName} already loaded.");
            onSceneLoaded?.Invoke(); // null üũ �� �ݹ� ����
            return;
        }

        // Additive ���� �񵿱� �� �ε� ����
        AsyncOperation loadOp = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

        // �� �ε尡 �Ϸ�Ǹ� ������ �̺�Ʈ �ڵ鷯 ���
        loadOp.completed += (AsyncOperation op) =>
        {
            Debug.Log($"Scene {sceneName} loaded.");
            onSceneLoaded?.Invoke(); // �ε� �Ϸ� �� �ݹ� ����
        };
    }
}