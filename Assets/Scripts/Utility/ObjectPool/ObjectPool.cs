using System.Collections.Generic;
using System;
using UnityEngine;

public class ObjectPool<T> : IPool<T>
    where T : MonoBehaviour, IPoolable<T>
{
    private Action<T> pullObject;
    private Action<T> pushObject;

    private Stack<T> pooledObjects = new Stack<T>();

    private Transform parent;

    private GameObject prefab;

    public int Count => pooledObjects.Count;

    public ObjectPool(Transform parent, GameObject prefab, int spawnCount = 0)
    {
        this.parent = parent;
        this.prefab = prefab;
        Spawn(spawnCount);
    }

    public ObjectPool(Transform parent, GameObject prefab, Action<T> pullObject, Action<T> pushObject, int spawnCount = 0)
    {
        this.parent = parent;
        this.prefab = prefab;
        this.pullObject = pullObject;
        this.pushObject = pushObject;
        Spawn(spawnCount);
    }

    private void Spawn(int spawnCount)
    {
        T t;

        for(int i = 0; i < spawnCount; i++) 
        {
            t = GameObject.Instantiate(prefab).GetComponent<T>();
            pooledObjects.Push(t);
            //t.gameObject.transform.SetParent(parent, true);
            t.gameObject.SetActive(false);
        }
    }

    public T Pull()
    {
        T t;

        if (Count > 0)
        {
            t = pooledObjects.Pop();
        }
        else
        {
            t = GameObject.Instantiate(prefab).GetComponent<T>();
            //t.gameObject.transform.SetParent(parent, true);
        }

        t.gameObject.SetActive(true);
        t.Initialize(Push);

        pullObject?.Invoke(t);

        return t;
    }

    public T Pull(Vector3 position)
    {
        var t = Pull();
        t.transform.position = position;
        return t;
    }

    public T Pull(Vector3 position, Vector3 direction)
    {
        var t = Pull(position);
        t.transform.forward = direction;
        return t;
    }

    public GameObject PullGameObject() => Pull().gameObject;
    public GameObject PullGameObject(Vector3 position) => Pull(position).gameObject;
    public GameObject PullGameObject(Vector3 position, Vector3 direction) => Pull(position, direction).gameObject;

    public void Push(T t)
    {
        pooledObjects.Push(t);
        pushObject?.Invoke(t);
        t.gameObject.SetActive(false);
    }
}