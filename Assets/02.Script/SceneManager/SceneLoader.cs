using System;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 씬을 애드티티브(Additive) 방식으로 로드하고,
/// 싱글톤 패턴으로 전역에서 접근 가능한 SceneLoader 클래스입니다.
/// </summary>
public class SceneLoader : MonoBehaviour
{
    /// <summary>
    /// SceneLoader의 싱글톤 인스턴스 (전역)
    /// </summary>
    public static SceneLoader Instance { get; private set; }

    /// <summary>
    /// 객체가 생성될 때 호출됩니다. 싱글톤 인스턴스를 설정하고, 중복 생성을 처리합니다.
    /// </summary>
    private void Awake()
    {
        // 이미 인스턴스가 존재하면 자신을 파괴하고 종료
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }

        // 현재 인스턴스를 싱글톤으로 설정
        Instance = this;

        // 씬 전환 시에도 파괴되지 않도록 설정
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// 씬 이름을 받아 Additive 방식으로 로드합니다.
    /// 이미 로드되어 있다면 중복 호출하지 않습니다.
    /// </summary>
    /// <param name="sceneName">로드할 씬 이름</param>
    /// <param name="onSceneLoaded">씬 로드 완료 후 실행할 콜백</param>
    public void LoadSceneAdditive(string sceneName, Action onSceneLoaded)
    {
        // 이미 해당 씬이 로드되어 있다면, 씬을 다시 로드하지 않고 콜백만 실행
        if (SceneManager.GetSceneByName(sceneName).isLoaded)
        {
            Debug.Log($"Scene {sceneName} already loaded.");
            onSceneLoaded?.Invoke(); // null 체크 후 콜백 실행
            return;
        }

        // Additive 방식으로 씬을 비동기 로드 시작
        AsyncOperation loadOp = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

        // 씬 로드가 완료되면 등록된 이벤트 콜백 실행
        loadOp.completed += (AsyncOperation op) =>
        {
            Debug.Log($"Scene {sceneName} loaded.");
            onSceneLoaded?.Invoke(); // 로드 완료 후 콜백 실행
        };
    }
}