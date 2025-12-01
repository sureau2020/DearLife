using System.Collections;
using UnityEngine;

public class CoroutineRunner : MonoBehaviour
{
    private static CoroutineRunner _instance;

    public static CoroutineRunner Instance
    {
        get
        {
            if (_instance == null)
            {
                // 创建 GameObject 并挂载 CoroutineRunner
                var go = new GameObject("CoroutineRunner");
                GameObject.DontDestroyOnLoad(go);
                _instance = go.AddComponent<CoroutineRunner>();
            }
            return _instance;
        }
    }

    // 不返回 Coroutine，直接启动协程即可
    public static void Run(IEnumerator routine)
    {
        Instance.StartCoroutine(routine);
    }
}
