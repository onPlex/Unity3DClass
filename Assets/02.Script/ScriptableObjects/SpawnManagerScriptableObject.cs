using UnityEngine;

// Unity �����Ϳ��� ScriptableObject ������ ������ �� �ְ� ���ִ� �Ӽ�(Attribute)�Դϴ�.
// "Create > ScriptableObjects > SpawnManagerScriptableObject" ��η� ������ �� �ֽ��ϴ�.
[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/SpawnManagerScriptableObject", order = 1)]
public class SpawnManagerScriptableObject : ScriptableObject
{
    // ������ �������� �̸��Դϴ�.
    // ��: "Enemy", "Tree" ���� �̸��� �Է��Ͽ� Resources �������� �ش� �������� �ҷ��� �� �ֽ��ϴ�.
    public string prefabName;

    // ������ �������� �����Դϴ�.
    // ��: 5�� �Է��ϸ� �������� 5�� �����˴ϴ�.
    public int numberOfPrefabsToCreate;

    // �� �������� ������ ��ġ�� ��� �ִ� �迭�Դϴ�.
    // ��: [ (0,0,0), (1,0,0), (2,0,0) ] �̷� ������ ��ġ�� ������ �� �ֽ��ϴ�.
    public Vector3[] spawnPoints;
}

