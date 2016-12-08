using UnityEngine;

public class SmoothJitter : MonoBehaviour
{
    public float Scale;
    public float Speed;

    public void Update()
    {
        float t = Mathf.Sin(Time.time * Speed);
        transform.localScale = new Vector3(1 + t * Scale, 1 + t * Scale, 1);
    }
}