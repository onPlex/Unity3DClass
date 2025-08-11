using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;

/// <summary>
/// UIManager
/// - 여러 UXML 화면(페이지)을 한 번에 로드하고, 필요한 화면만 ON/OFF로 전환하는 매니저.
/// - 장점: 화면 전환 시 지연시간(로딩과 메모리 부족 문제 해결), 코드에서 간단히 Open/Close/Toggle 호출만 하면 됨.
/// - 장점: 모든 화면을 미리 로드하므로 메모리 사용량 증가. (하지만 쿠션 효과로 부드러운 전환 가능)
/// </summary>
public class UIManager : MonoBehaviour
{
    /// <summary>
    /// 싱글톤 인스턴스
    /// - 다른 스크립트에서 쉽게 접근 가능.
    /// - UI는 전역적으로 사용되므로 싱글톤이 적합함.
    /// </summary>
    public static UIManager Instance { get; private set; }

    /// <summary>
    /// 하나의 UI 페이지(화면)를 나타냄
    /// - key: 어떤 화면인지 구분하는 식별자(예: HUD, Pause, Loading)
    /// - uxml: 해당 화면의 UXML 파일(VisualTreeAsset)
    /// </summary>
    [System.Serializable]
    public class UIPage
    {
        public UIKey key;               // 어떤 화면인지 구분 (enum 사용: 타입 안전성/코드완성/오타 방지)
        public VisualTreeAsset uxml;    // 해당 화면의 UXML(설정 파일). CloneTree()로 인스턴스화해서 사용.
    }

    [Header("References")]
    public UIDocument uiDocument;       // 씬에 1개만 존재하는 UI 루트. 모든 UI는 rootVisualElement 아래에 자식으로 배치됨.
    public List<UIPage> pages = new();  // 인스펙터에서 설정할 페이지 목록. (필요한 화면들)

    /// <summary>
    /// 페이지 키와 실제 인스턴스된 UI를 매핑하는 딕셔너리
    /// - key: UIKey(화면 구분자)
    /// - value: 해당 화면의 루트 VisualElement(인스턴스)
    /// </summary>
    private Dictionary<UIKey, VisualElement> map = new();

    private void Awake()
    {
        // 싱글톤 설정: 이미 존재하면 자신을 파괴 (중복 방지)
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        // 씬 전환 시에도 파괴되지 않음. (UI는 전역적이므로 계속 유지)
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        // UIDocument가 없으면 자동으로 찾아서 설정
        if (uiDocument == null) uiDocument = GetComponent<UIDocument>();

        // 모든 UI 트리의 루트. 여기에 자식들을 추가(Add)함.
        var root = uiDocument.rootVisualElement;

        // 1) 설정된 모든 UXML을 미리 인스턴스화(CloneTree)해서 트리에 배치함.
        // 2) 초기에는 보이지 않게 display=None 처리함. (필요할 때만 표시)
        // 3) 전체 화면 UI는 StretchToParentSize()로 화면 크기에 맞춤.
        foreach (var p in pages)
        {
            if (p.uxml == null) { Debug.LogWarning($"UXML 누락: {p.key}"); continue; }

            var ve = p.uxml.CloneTree();          // 템플릿(UXML)을 인스턴스(VisualElement)로 변환
            ve.style.display = DisplayStyle.None;  // 초기에는 숨김 처리(전환 시 깜빡임 방지)
            ve.StretchToParentSize();              // 화면 전체 채움(Full-screen UI를 위해)
            root.Add(ve);                          // 루트에 추가해서 계층 구조 완성

            map[p.key] = ve;                       // 키와 인스턴스를 매핑해서 나중에 찾기 쉽게 저장
        }

        // GameState 변경 이벤트 연결
        // - 게임 상태(플레이/일시정지/로딩 등)가 바뀌면 특정 UI를 보이거나 숨김 처리
        GameStateManager.Instance.OnGameStateChanged += HandleGameState;
    }

    private void OnDestroy()
    {
        // 이벤트 연결 해제(중요): 객체가 파괴되거나 씬이 바뀌면, 이벤트 구독이 남아있어서 중복 호출 방지
        if (GameStateManager.Instance != null)
            GameStateManager.Instance.OnGameStateChanged -= HandleGameState;
    }

    /// <summary>
    /// 게임 상태가 바뀔 때 호출되는 콜백
    /// - 여기서는 간단한 상태별 UI 처리(Pause/Loading).
    /// - 다른 스크립트에서도 이벤트를 구독해서 UI 상태를 동기화할 수 있음.
    /// </summary>
    private void HandleGameState(GameState state)
    {
        // 예: Paused면 Pause UI ON, 아니면 OFF
        SetActive(UIKey.Pause, state == GameState.Paused);

        // 예: Loading이면 Loading UI ON, 아니면 OFF
        SetActive(UIKey.Loading, state == GameState.Loading);
    }

    // --- 외부에서 사용할 수 있는 공개 메서드들 ---

    /// <summary>
    /// 특정 페이지를 보이게 함(화면에 표시).
    /// 내부적으로는 SetActive(key, true) 호출.
    /// </summary>
    public void Open(UIKey key) => SetActive(key, true);

    /// <summary>
    /// 특정 페이지를 숨김(화면에서 제거).
    /// 내부적으로는 SetActive(key, false) 호출.
    /// </summary>
    public void Close(UIKey key) => SetActive(key, false);

    /// <summary>
    /// 현재 상태를 반전시킴.
    /// - display가 None이면 보임, 아니면 숨김.
    /// - 메뉴 토글(예: ESC로 Pause 메뉴 열기/닫기)에 유용.
    /// </summary>
    public void Toggle(UIKey key)
    {
        if (!map.TryGetValue(key, out var ve)) return;

        // resolvedStyle: 스타일시트가 적용된 후의 실제 계산된 스타일 값을 가져옴.
        // 여기서는 현재 display가 None인지 확인해서 다음 상태를 결정.
        bool willOpen = ve.resolvedStyle.display == DisplayStyle.None;
        SetActive(key, willOpen);
    }

    /// <summary>
    /// 특정 페이지 안에서 이름으로 자식 요소를 찾음.
    /// - 버튼, 텍스트 등 자식요소들을 찾아서 이벤트 연결할 때 사용.
    /// - 페이지가 트리로 구성되어 있어서 name 매칭으로 찾을 수 있음.
    /// </summary>
    public T Q<T>(UIKey key, string name) where T : VisualElement
    {
        if (!map.TryGetValue(key, out var ve)) return null;
        return ve.Q<T>(name); // UI Toolkit의 내장 API. name은 UXML의 name 속성.
    }

    /// <summary>
    /// 화면 전환 핵심: display를 Flex/None으로 바꿔서 보이기/숨기기.
    /// - 활성화 시 BringToFront()로 최상위로 올림, 다른 UI들 위에 표시.
    /// - 애니메이션/트랜지션을 원한다면, 여기서 클래스 추가(AddToClassList)로 전환 효과 구현.
    /// </summary>
    private void SetActive(UIKey key, bool active)
    {
        if (!map.TryGetValue(key, out var ve)) return;

        ve.style.display = active ? DisplayStyle.Flex : DisplayStyle.None;

        if (active)
        {
            // 활성화된 페이지를 최상위로 올려서 다른 UI들 위에 표시
            ve.BringToFront();

            // (선택사항) 전환 효과를 원한다면:
            // ve.AddToClassList("ui-show"); ve.RemoveFromClassList("ui-hide");
            // 그리고 USS에서 .ui-show/.ui-hide로 transition/opacity 설정
        }
    }
}
