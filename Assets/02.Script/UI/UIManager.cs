using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;

/// <summary>
/// UIManager
/// - 여러 UXML 화면(페이지)을 한 번에 준비해두고, 필요할 때 ON/OFF로 전환하는 허브.
/// - 장점: 전환이 빠르고(미리 메모리에 올라와 있음), 코드에서 간단히 Open/Close/Toggle 호출만 하면 됨.
/// - 단점: 모든 페이지를 미리 붙이므로 메모리 사용량 증가. (규모 커지면 일부는 지연 로드로 바꾸는 게 좋음)
/// </summary>
public class UIManager : MonoBehaviour
{
    /// <summary>
    /// 싱글톤 인스턴스
    /// - 씬 전역에서 접근이 쉬워짐.
    /// - UI는 보통 전역 허브가 편해서 싱글톤을 자주 씀.
    /// </summary>
    public static UIManager Instance { get; private set; }

    /// <summary>
    /// 하나의 UI 페이지(화면) 단위를 표현
    /// - key: 어떤 화면인지 구분하는 열거형(예: HUD, Pause, Loading)
    /// - uxml: 해당 화면의 UXML 에셋(VisualTreeAsset)
    /// </summary>
    [System.Serializable]
    public class UIPage
    {
        public UIKey key;               // 어떤 화면인지 식별 (enum 권장: 오타 방지/자동완성/스위치문 유리)
        public VisualTreeAsset uxml;    // 그 화면의 UXML(원본 템플릿). CloneTree()로 인스턴스화해서 씀.
    }

    [Header("References")]
    public UIDocument uiDocument;       // 씬에 1개 존재하는 UI 루트. 여기의 rootVisualElement 아래에 모든 페이지를 붙인다.
    public List<UIPage> pages = new();  // 인스펙터에서 등록할 페이지 목록. (필수 구성요소)

    /// <summary>
    /// 실제로 씬에 붙여진 페이지 인스턴스를 보관하는 맵
    /// - key: UIKey(화면 식별자)
    /// - value: 해당 화면의 루트 VisualElement(인스턴스)
    /// </summary>
    private Dictionary<UIKey, VisualElement> map = new();

    private void Awake()
    {
        // 싱글톤 보장: 이미 존재하면 자기 자신 파괴 (중복 생성 방지)
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        // 씬 전환 시에도 살아남게 함. (UI 전역 허브이므로 유지가 편함)
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        // UIDocument가 비어있다면 같은 오브젝트에서 찾아서 참조
        if (uiDocument == null) uiDocument = GetComponent<UIDocument>();

        // 실제 UI 트리의 루트. 모든 페이지를 이 밑에 추가(Add)한다.
        var root = uiDocument.rootVisualElement;

        // 1) 등록된 모든 UXML을 한 번씩 인스턴스화(CloneTree)해서 트리에 붙인다.
        // 2) 처음에는 보이지 않게 display=None 처리한다. (필요할 때만 켜기)
        // 3) 전체 화면 UI라면 StretchToParentSize()로 패널 크기에 맞춘다.
        foreach (var p in pages)
        {
            if (p.uxml == null) { Debug.LogWarning($"UXML 없음: {p.key}"); continue; }

            var ve = p.uxml.CloneTree();          // 템플릿(UXML) → 인스턴스(VisualElement) 생성
            ve.style.display = DisplayStyle.None;  // 초기에 숨김 처리(전환 시 보이게 함)
            ve.StretchToParentSize();              // 화면 꽉 채우기(Full-screen UI에 적합)
            root.Add(ve);                          // 루트에 추가 → 이제 씬에서 렌더링 대상

            map[p.key] = ve;                       // 나중에 빠르게 접근하기 위해 딕셔너리에 보관
        }

        // GameState 변경 이벤트 구독
        // - 게임 흐름(플레이/일시정지/로딩 등)에 따라 자동으로 특정 UI를 켜거나 끄고 싶을 때 편리
        GameStateManager.Instance.OnGameStateChanged += HandleGameState;
    }

    private void OnDestroy()
    {
        // 이벤트 구독 해제(중요): 씬이 바뀌거나 오브젝트가 파괴될 때, 유령 참조로 인한 콜백 호출 방지
        if (GameStateManager.Instance != null)
            GameStateManager.Instance.OnGameStateChanged -= HandleGameState;
    }

    /// <summary>
    /// 게임 상태가 바뀔 때 자동으로 호출되는 핸들러
    /// - 여기서는 대표 예시 두 가지만 처리(Pause/Loading).
    /// - 프로젝트가 커지면 상태→UI목록 매핑 테이블로 일반화하는 걸 추천.
    /// </summary>
    private void HandleGameState(GameState state)
    {
        // 예: Paused면 Pause UI ON, 아니면 OFF
        SetActive(UIKey.Pause, state == GameState.Paused);

        // 예: Loading이면 Loading UI ON, 아니면 OFF
        SetActive(UIKey.Loading, state == GameState.Loading);
    }

    // --- 외부에서 쓰는 공개 메서드들 ---

    /// <summary>
    /// 지정한 페이지를 연다(보이게 만든다).
    /// 내부적으로 SetActive(key, true) 호출.
    /// </summary>
    public void Open(UIKey key) => SetActive(key, true);

    /// <summary>
    /// 지정한 페이지를 닫는다(숨긴다).
    /// 내부적으로 SetActive(key, false) 호출.
    /// </summary>
    public void Close(UIKey key) => SetActive(key, false);

    /// <summary>
    /// 현재 상태와 반대로 전환한다.
    /// - display가 None이면 열고, 그렇지 않으면 닫는다.
    /// - 메뉴 토글(예: ESC로 Pause 열고 닫기)에 유용.
    /// </summary>
    public void Toggle(UIKey key)
    {
        if (!map.TryGetValue(key, out var ve)) return;

        // resolvedStyle: 레이아웃이 계산된 시점의 최종 값에 접근할 때 사용.
        // 여기서는 현재 display가 None인지 확인해서 반전 로직에 활용.
        bool willOpen = ve.resolvedStyle.display == DisplayStyle.None;
        SetActive(key, willOpen);
    }

    /// <summary>
    /// 특정 페이지 안에서 이름으로 자식 요소를 찾는다.
    /// - 버튼, 라벨 같은 컨트롤을 안전하게 가져오는 용도.
    /// - 페이지마다 트리가 분리되어 있어 name 충돌 걱정이 줄어듦.
    /// </summary>
    public T Q<T>(UIKey key, string name) where T : VisualElement
    {
        if (!map.TryGetValue(key, out var ve)) return null;
        return ve.Q<T>(name); // UI Toolkit의 쿼리 API. name은 UXML의 name 속성.
    }

    /// <summary>
    /// 내부 전환 핵심: display를 Flex/None으로 바꿔서 보이기/숨기기.
    /// - 활성화 시 BringToFront()로 시각적 우선순위를 올려, 겹칠 때 최상단에 오게 함.
    /// - 애니메이션/트랜지션을 쓰고 싶다면, 여기서 클래스 토글(AddToClassList)로 전환하는 패턴도 좋음.
    /// </summary>
    private void SetActive(UIKey key, bool active)
    {
        if (!map.TryGetValue(key, out var ve)) return;

        ve.style.display = active ? DisplayStyle.Flex : DisplayStyle.None;

        if (active)
        {
            // 동일 레벨에서 여러 페이지가 겹칠 때, 열리는 페이지를 최상단으로
            ve.BringToFront();

            // (선택) 전환 효과를 쓰고 싶다면:
            // ve.AddToClassList("ui-show"); ve.RemoveFromClassList("ui-hide");
            // → USS에서 .ui-show/.ui-hide에 transition/opacity 정의
        }
    }
}
