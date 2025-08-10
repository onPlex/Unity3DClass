using UnityEngine;

// 이 스크립트는 게임이 시작될 때 설정된 위치에 여러 개의 오브젝트를 생성하는 기능을 합니다.
public class Spawner : MonoBehaviour
{
    // 생성할 대상 프리팹을 에디터에서 할당합니다.
    public GameObject entityToSpawn;

    // SpawnManagerScriptableObject 에셋을 참조합니다.
    // 이 에셋에는 프리팹 이름, 생성 개수, 위치 정보(spawn points)가 저장되어 있습니다.
    public SpawnManagerScriptableObject spawnManagerValues;

    // 생성되는 오브젝트 이름에 붙일 일련번호입니다.
    int instanceNumber = 1;

    // 게임 시작 시 자동으로 호출되는 함수입니다.
    void Start()
    {
        // 오브젝트들을 생성합니다.
        SpawnEntities();
    }

    // 오브젝트를 순차적으로 생성하는 함수입니다.
    void SpawnEntities()
    {
        // 현재 사용할 spawn point의 인덱스를 저장하는 변수입니다.
        int currentSpawnPointIndex = 0;

        // ScriptableObject에서 설정한 생성 개수만큼 반복합니다.
        for (int i = 0; i < spawnManagerValues.numberOfPrefabsToCreate; i++)
        {
            // 지정된 위치(spawn point)에 프리팹을 하나 생성합니다.
            GameObject currentEntity = Instantiate(
                entityToSpawn,
                spawnManagerValues.spawnPoints[currentSpawnPointIndex],
                Quaternion.identity // 회전값은 기본값으로 설정 (0,0,0)
            );

            // 생성된 오브젝트의 이름을 ScriptableObject에 지정된 이름 + 고유 번호로 설정합니다.
            // 예: "Enemy1", "Enemy2", "Enemy3" ...
            currentEntity.name = spawnManagerValues.prefabName + instanceNumber;

            // 다음 spawn point 인덱스로 이동합니다.
            // 인덱스가 배열의 길이를 넘으면 처음으로 되돌아갑니다 (순환 구조).
            currentSpawnPointIndex = (currentSpawnPointIndex + 1) % spawnManagerValues.spawnPoints.Length;

            // 고유 번호 증가
            instanceNumber++;
        }
    }
}
