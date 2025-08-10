using UnityEngine;

/// <summary>
/// �÷��̾ Ư�� Ʈ���ſ� ������ ������ ���� Additive ������� �ε��ϰ�,
/// �ε��� �Ϸ�Ǹ� �Ա��� �����ִ� Ʈ���� ��ũ��Ʈ�Դϴ�.
/// </summary>
public class SceneLoadTrigger : MonoBehaviour
{
    // �ε��� ��� �� �̸�
    public string targetSceneName;

    // �ε� �Ϸ� �� ���� �Ա� ������Ʈ (��, ��Ż ��)
    public GameObject entranceToActivate;

    // �ߺ� Ʈ���� ������ ���� �÷���
    private bool triggered = false;

    /// <summary>
    /// �÷��̾ Ʈ���ſ� �������� �� ȣ��˴ϴ�.
    /// </summary>
    /// <param name="other">Ʈ���ſ� ������ Collider</param>
    private void OnTriggerEnter(Collider other)
    {
        // �̹� �� �� Ʈ���ŵǾ����� ����
        if (triggered) return;

        // �浹�� ��ü�� "Player" �±װ� �ƴϸ� ����
        if (!other.CompareTag("Player")) return;

        // Ʈ���� ���� �÷��� ����
        triggered = true;

        // SceneLoader�� LoadSceneAdditive �Լ� ȣ��
        // - �� �̸��� �ε� �Ϸ� �� ������ �ݹ��� ����
        SceneLoader.Instance.LoadSceneAdditive(targetSceneName, () =>
        {
            Debug.Log("Scene loaded, opening entrance...");

            // �Ա� ������Ʈ�� �����Ǿ� �ִٸ� Ȱ��ȭ
            if (entranceToActivate != null)
            {
                entranceToActivate.SetActive(true);

                //// Animator�� �ִٸ� "Open" Ʈ���� �ߵ� (��: �� ���� �ִϸ��̼�)
                Animator anim = entranceToActivate.GetComponent<Animator>();
                if (anim != null)
                {
                    anim.SetTrigger("Open");
                }
            }
        });
    }
}
