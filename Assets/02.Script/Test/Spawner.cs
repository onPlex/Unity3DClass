using UnityEngine;

// �� ��ũ��Ʈ�� ������ ���۵� �� ������ ��ġ�� ���� ���� ������Ʈ�� �����ϴ� ����� �մϴ�.
public class Spawner : MonoBehaviour
{
    // ������ ��� �������� �����Ϳ��� �Ҵ��մϴ�.
    public GameObject entityToSpawn;

    // SpawnManagerScriptableObject ������ �����մϴ�.
    // �� ���¿��� ������ �̸�, ���� ����, ��ġ ����(spawn points)�� ����Ǿ� �ֽ��ϴ�.
    public SpawnManagerScriptableObject spawnManagerValues;

    // �����Ǵ� ������Ʈ �̸��� ���� �Ϸù�ȣ�Դϴ�.
    int instanceNumber = 1;

    // ���� ���� �� �ڵ����� ȣ��Ǵ� �Լ��Դϴ�.
    void Start()
    {
        // ������Ʈ���� �����մϴ�.
        SpawnEntities();
    }

    // ������Ʈ�� ���������� �����ϴ� �Լ��Դϴ�.
    void SpawnEntities()
    {
        // ���� ����� spawn point�� �ε����� �����ϴ� �����Դϴ�.
        int currentSpawnPointIndex = 0;

        // ScriptableObject���� ������ ���� ������ŭ �ݺ��մϴ�.
        for (int i = 0; i < spawnManagerValues.numberOfPrefabsToCreate; i++)
        {
            // ������ ��ġ(spawn point)�� �������� �ϳ� �����մϴ�.
            GameObject currentEntity = Instantiate(
                entityToSpawn,
                spawnManagerValues.spawnPoints[currentSpawnPointIndex],
                Quaternion.identity // ȸ������ �⺻������ ���� (0,0,0)
            );

            // ������ ������Ʈ�� �̸��� ScriptableObject�� ������ �̸� + ���� ��ȣ�� �����մϴ�.
            // ��: "Enemy1", "Enemy2", "Enemy3" ...
            currentEntity.name = spawnManagerValues.prefabName + instanceNumber;

            // ���� spawn point �ε����� �̵��մϴ�.
            // �ε����� �迭�� ���̸� ������ ó������ �ǵ��ư��ϴ� (��ȯ ����).
            currentSpawnPointIndex = (currentSpawnPointIndex + 1) % spawnManagerValues.spawnPoints.Length;

            // ���� ��ȣ ����
            instanceNumber++;
        }
    }
}
