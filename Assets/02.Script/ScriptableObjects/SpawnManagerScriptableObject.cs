using UnityEngine;

// Unity 에디터에서 ScriptableObject 에셋을 생성할 수 있게 해주는 속성(Attribute)입니다.
// "Create > ScriptableObjects > SpawnManagerScriptableObject" 경로로 생성할 수 있습니다.
[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/SpawnManagerScriptableObject", order = 1)]
public class SpawnManagerScriptableObject : ScriptableObject
{
    // 생성할 프리팹의 이름입니다.
    // 예: "Enemy", "Tree" 등의 이름을 입력하여 Resources 폴더에서 해당 프리팹을 불러올 수 있습니다.
    public string prefabName;

    // 생성할 프리팹의 개수입니다.
    // 예: 5를 입력하면 프리팹이 5개 생성됩니다.
    public int numberOfPrefabsToCreate;

    // 각 프리팹이 생성될 위치를 담고 있는 배열입니다.
    // 예: [ (0,0,0), (1,0,0), (2,0,0) ] 이런 식으로 위치를 지정할 수 있습니다.
    public Vector3[] spawnPoints;
}

