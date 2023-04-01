using UnityEngine;

public class SampleObjectPool : MonoBehaviour
{
    public static ObjectPool<PoolObject> objectPool;

    [SerializeField]
    private GameObject objectPrefab;

    private void Awake()
    {
        objectPool = new ObjectPool<PoolObject>(transform, objectPrefab, CallOnPull, CallOnPush);
    }

    private void CallOnPull(PoolObject poolObject)
    {
        Debug.Log("Pulled " + poolObject.ToString());
    }

    private void CallOnPush(PoolObject poolObject)
    {
        Debug.Log("Pushed " + poolObject.ToString());
    }
}