using System;
using UnityEngine;

public class PoolObject : MonoBehaviour, IPoolable<PoolObject>
{
    private Action<PoolObject> returnToPool;

    private void OnDisable()
    {
        ReturnToPool();
    }

    public void Initialize(Action<PoolObject> returnAction)
    {
        returnToPool = returnAction;
    }

    public void ReturnToPool()
    {
        returnToPool?.Invoke(this);
    }
}
