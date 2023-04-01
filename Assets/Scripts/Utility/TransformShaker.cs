using UnityEngine;

public class TransformShaker : MonoBehaviour
{
    private float intensity;

    private Vector3 shake, origin;

    private float time;

    private float shakeSpeed;

    void Awake()
    {
        origin = transform.localPosition;
    }

    void Update()
    {
        if (time > 0)
        {
            time -= shakeSpeed * Time.deltaTime;
            shake = origin + ((Random.insideUnitSphere * intensity) * time);
            transform.localPosition = shake;
        }
        else
        {
            transform.localPosition = origin;
            intensity = 0;
        }
    }

    public void Shake(float speed, float duration)
    {
        intensity += speed * .02f;
        intensity = Mathf.Clamp(intensity, 0, .14f);
        shakeSpeed = speed;
        time = duration;
    }
}