using UnityEngine;

/// <summary>
/// 플레이어가 특정 트리거와 충돌했을 때 Additive 방식으로 씬을 로드하고,
/// 로드가 완료되면 입구 게임오브젝트를 활성화하는 트리거입니다.
/// </summary>
public class SceneLoadTrigger : MonoBehaviour
{
    // 로드할 대상 씬 이름
    public string targetSceneName;

    // 로드 완료 후 활성화할 입구 게임오브젝트 (예: 문, 포탈 등)
    public GameObject entranceToActivate;

    // 중복 트리거 방지를 위한 플래그
    private bool triggered = false;

    /// <summary>
    /// 플레이어가 트리거와 충돌했을 때 호출됩니다.
    /// </summary>
    /// <param name="other">트리거와 충돌한 Collider</param>
    private void OnTriggerEnter(Collider other)
    {
        // 이미 한 번 트리거되었다면 종료
        if (triggered) return;

        // 충돌한 객체가 "Player" 태그가 아니면 종료
        if (!other.CompareTag("Player")) return;

        // 트리거 상태 플래그 설정
        triggered = true;

        // SceneLoader의 LoadSceneAdditive 함수 호출
        // - 씬 이름을 받아 로드 완료 후 콜백을 실행
        SceneLoader.Instance.LoadSceneAdditive(targetSceneName, () =>
        {
            Debug.Log("Scene loaded, opening entrance...");

            // 입구 게임오브젝트가 설정되어 있다면 활성화
            if (entranceToActivate != null)
            {
                entranceToActivate.SetActive(true);

                //// Animator가 있다면 "Open" 트리거를 발생 (예: 문 열기 애니메이션)
                Animator anim = entranceToActivate.GetComponent<Animator>();
                if (anim != null)
                {
                    anim.SetTrigger("Open");
                }
            }
        });
    }
}
