public interface IPoolable<T>
{
    void Initialize(System.Action<T> returnAction);
    void ReturnToPool();
}