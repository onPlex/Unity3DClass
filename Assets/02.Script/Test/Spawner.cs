using UnityEngine;

/// <summary>
/// 이 스크립트는 설정된 스폰 포인트에서 지정된 개수만큼 게임오브젝트를 생성하는 스포너입니다.
/// </summary>
public class Spawner : MonoBehaviour
{
    // 생성할 모든 게임오브젝트의 프리팹을 할당합니다.
    public GameObject entityToSpawn;

    // SpawnManagerScriptableObject 에셋을 할당합니다.
    // 이 에셋에는 프리팹 이름, 생성 개수, 위치 정보(spawn points)가 설정되어 있습니다.
    public SpawnManagerScriptableObject spawnManagerValues;

    // 생성되는 게임오브젝트 이름에 붙일 일련번호입니다.
    int instanceNumber = 1;

    // 게임 시작 시 자동으로 호출되는 함수입니다.
    void Start()
    {
        // 게임오브젝트들을 생성합니다.
        SpawnEntities();
    }

    // 게임오브젝트를 설정된 위치에 생성하는 함수입니다.
    void SpawnEntities()
    {
        // 현재 사용할 spawn point의 인덱스를 추적합니다.
        int currentSpawnPointIndex = 0;

        // ScriptableObject에서 설정된 생성 개수만큼 반복합니다.
        for (int i = 0; i < spawnManagerValues.numberOfPrefabsToCreate; i++)
        {
            // 설정된 위치(spawn point)에서 프리팹을 하나 생성합니다.
            GameObject currentEntity = Instantiate(
                entityToSpawn,
                spawnManagerValues.spawnPoints[currentSpawnPointIndex],
                Quaternion.identity // 회전값을 기본값으로 설정 (0,0,0)
            );

            // 생성된 게임오브젝트의 이름을 ScriptableObject의 프리팹 이름 + 일련 번호로 설정합니다.
            // 예: "Enemy1", "Enemy2", "Enemy3" ...
            currentEntity.name = spawnManagerValues.prefabName + instanceNumber;

            // 다음 spawn point 인덱스로 이동합니다.
            // 인덱스가 배열의 끝에 도달하면 처음으로 돌아갑니다 (순환 방식).
            currentSpawnPointIndex = (currentSpawnPointIndex + 1) % spawnManagerValues.spawnPoints.Length;

            // 일련 번호 증가
            instanceNumber++;
        }
    }
}
