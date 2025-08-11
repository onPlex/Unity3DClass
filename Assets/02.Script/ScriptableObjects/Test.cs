using UnityEngine;
using System.Collections;

public class Test : MonoBehaviour
{  
    // ✅ 올바른 방법 - UIManager 초기화 완료 후 호출
    void Start()
    {
        StartCoroutine(WaitForUIManager());
    }

    IEnumerator WaitForUIManager()
    {
        yield return new WaitForEndOfFrame();  // UIManager.Start() 완료 대기
        UIManager.Instance.Open(UIKey.Pause);
    }

}
