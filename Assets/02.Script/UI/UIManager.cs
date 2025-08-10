using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;

/// <summary>
/// UIManager
/// - ���� UXML ȭ��(������)�� �� ���� �غ��صΰ�, �ʿ��� �� ON/OFF�� ��ȯ�ϴ� ���.
/// - ����: ��ȯ�� ������(�̸� �޸𸮿� �ö�� ����), �ڵ忡�� ������ Open/Close/Toggle ȣ�⸸ �ϸ� ��.
/// - ����: ��� �������� �̸� ���̹Ƿ� �޸� ��뷮 ����. (�Ը� Ŀ���� �Ϻδ� ���� �ε�� �ٲٴ� �� ����)
/// </summary>
public class UIManager : MonoBehaviour
{
    /// <summary>
    /// �̱��� �ν��Ͻ�
    /// - �� �������� ������ ������.
    /// - UI�� ���� ���� ��갡 ���ؼ� �̱����� ���� ��.
    /// </summary>
    public static UIManager Instance { get; private set; }

    /// <summary>
    /// �ϳ��� UI ������(ȭ��) ������ ǥ��
    /// - key: � ȭ������ �����ϴ� ������(��: HUD, Pause, Loading)
    /// - uxml: �ش� ȭ���� UXML ����(VisualTreeAsset)
    /// </summary>
    [System.Serializable]
    public class UIPage
    {
        public UIKey key;               // � ȭ������ �ĺ� (enum ����: ��Ÿ ����/�ڵ��ϼ�/����ġ�� ����)
        public VisualTreeAsset uxml;    // �� ȭ���� UXML(���� ���ø�). CloneTree()�� �ν��Ͻ�ȭ�ؼ� ��.
    }

    [Header("References")]
    public UIDocument uiDocument;       // ���� 1�� �����ϴ� UI ��Ʈ. ������ rootVisualElement �Ʒ��� ��� �������� ���δ�.
    public List<UIPage> pages = new();  // �ν����Ϳ��� ����� ������ ���. (�ʼ� �������)

    /// <summary>
    /// ������ ���� �ٿ��� ������ �ν��Ͻ��� �����ϴ� ��
    /// - key: UIKey(ȭ�� �ĺ���)
    /// - value: �ش� ȭ���� ��Ʈ VisualElement(�ν��Ͻ�)
    /// </summary>
    private Dictionary<UIKey, VisualElement> map = new();

    private void Awake()
    {
        // �̱��� ����: �̹� �����ϸ� �ڱ� �ڽ� �ı� (�ߺ� ���� ����)
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        // �� ��ȯ �ÿ��� ��Ƴ��� ��. (UI ���� ����̹Ƿ� ������ ����)
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        // UIDocument�� ����ִٸ� ���� ������Ʈ���� ã�Ƽ� ����
        if (uiDocument == null) uiDocument = GetComponent<UIDocument>();

        // ���� UI Ʈ���� ��Ʈ. ��� �������� �� �ؿ� �߰�(Add)�Ѵ�.
        var root = uiDocument.rootVisualElement;

        // 1) ��ϵ� ��� UXML�� �� ���� �ν��Ͻ�ȭ(CloneTree)�ؼ� Ʈ���� ���δ�.
        // 2) ó������ ������ �ʰ� display=None ó���Ѵ�. (�ʿ��� ���� �ѱ�)
        // 3) ��ü ȭ�� UI��� StretchToParentSize()�� �г� ũ�⿡ �����.
        foreach (var p in pages)
        {
            if (p.uxml == null) { Debug.LogWarning($"UXML ����: {p.key}"); continue; }

            var ve = p.uxml.CloneTree();          // ���ø�(UXML) �� �ν��Ͻ�(VisualElement) ����
            ve.style.display = DisplayStyle.None;  // �ʱ⿡ ���� ó��(��ȯ �� ���̰� ��)
            ve.StretchToParentSize();              // ȭ�� �� ä���(Full-screen UI�� ����)
            root.Add(ve);                          // ��Ʈ�� �߰� �� ���� ������ ������ ���

            map[p.key] = ve;                       // ���߿� ������ �����ϱ� ���� ��ųʸ��� ����
        }

        // GameState ���� �̺�Ʈ ����
        // - ���� �帧(�÷���/�Ͻ�����/�ε� ��)�� ���� �ڵ����� Ư�� UI�� �Ѱų� ���� ���� �� ��
        GameStateManager.Instance.OnGameStateChanged += HandleGameState;
    }

    private void OnDestroy()
    {
        // �̺�Ʈ ���� ����(�߿�): ���� �ٲ�ų� ������Ʈ�� �ı��� ��, ���� ������ ���� �ݹ� ȣ�� ����
        if (GameStateManager.Instance != null)
            GameStateManager.Instance.OnGameStateChanged -= HandleGameState;
    }

    /// <summary>
    /// ���� ���°� �ٲ� �� �ڵ����� ȣ��Ǵ� �ڵ鷯
    /// - ���⼭�� ��ǥ ���� �� ������ ó��(Pause/Loading).
    /// - ������Ʈ�� Ŀ���� ���¡�UI��� ���� ���̺�� �Ϲ�ȭ�ϴ� �� ��õ.
    /// </summary>
    private void HandleGameState(GameState state)
    {
        // ��: Paused�� Pause UI ON, �ƴϸ� OFF
        SetActive(UIKey.Pause, state == GameState.Paused);

        // ��: Loading�̸� Loading UI ON, �ƴϸ� OFF
        SetActive(UIKey.Loading, state == GameState.Loading);
    }

    // --- �ܺο��� ���� ���� �޼���� ---

    /// <summary>
    /// ������ �������� ����(���̰� �����).
    /// ���������� SetActive(key, true) ȣ��.
    /// </summary>
    public void Open(UIKey key) => SetActive(key, true);

    /// <summary>
    /// ������ �������� �ݴ´�(�����).
    /// ���������� SetActive(key, false) ȣ��.
    /// </summary>
    public void Close(UIKey key) => SetActive(key, false);

    /// <summary>
    /// ���� ���¿� �ݴ�� ��ȯ�Ѵ�.
    /// - display�� None�̸� ����, �׷��� ������ �ݴ´�.
    /// - �޴� ���(��: ESC�� Pause ���� �ݱ�)�� ����.
    /// </summary>
    public void Toggle(UIKey key)
    {
        if (!map.TryGetValue(key, out var ve)) return;

        // resolvedStyle: ���̾ƿ��� ���� ������ ���� ���� ������ �� ���.
        // ���⼭�� ���� display�� None���� Ȯ���ؼ� ���� ������ Ȱ��.
        bool willOpen = ve.resolvedStyle.display == DisplayStyle.None;
        SetActive(key, willOpen);
    }

    /// <summary>
    /// Ư�� ������ �ȿ��� �̸����� �ڽ� ��Ҹ� ã�´�.
    /// - ��ư, �� ���� ��Ʈ���� �����ϰ� �������� �뵵.
    /// - ���������� Ʈ���� �и��Ǿ� �־� name �浹 ������ �پ��.
    /// </summary>
    public T Q<T>(UIKey key, string name) where T : VisualElement
    {
        if (!map.TryGetValue(key, out var ve)) return null;
        return ve.Q<T>(name); // UI Toolkit�� ���� API. name�� UXML�� name �Ӽ�.
    }

    /// <summary>
    /// ���� ��ȯ �ٽ�: display�� Flex/None���� �ٲ㼭 ���̱�/�����.
    /// - Ȱ��ȭ �� BringToFront()�� �ð��� �켱������ �÷�, ��ĥ �� �ֻ�ܿ� ���� ��.
    /// - �ִϸ��̼�/Ʈ�������� ���� �ʹٸ�, ���⼭ Ŭ���� ���(AddToClassList)�� ��ȯ�ϴ� ���ϵ� ����.
    /// </summary>
    private void SetActive(UIKey key, bool active)
    {
        if (!map.TryGetValue(key, out var ve)) return;

        ve.style.display = active ? DisplayStyle.Flex : DisplayStyle.None;

        if (active)
        {
            // ���� �������� ���� �������� ��ĥ ��, ������ �������� �ֻ������
            ve.BringToFront();

            // (����) ��ȯ ȿ���� ���� �ʹٸ�:
            // ve.AddToClassList("ui-show"); ve.RemoveFromClassList("ui-hide");
            // �� USS���� .ui-show/.ui-hide�� transition/opacity ����
        }
    }
}
