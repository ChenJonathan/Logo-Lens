using UnityEngine;

public class SmoothHover : MonoBehaviour
{
    public Card Card;
    public float Scale;
    
    private float target = 0;

    public void Update()
    {
        target = 0;
    }

    public void FixedUpdate()
    {
        if(Card.Busy != 0)
            target = 0;

        Vector3 localPosition = transform.localPosition;
        localPosition.z = Mathf.Lerp(localPosition.z, target, 0.1f);
        transform.localPosition = localPosition;
    }

    public void Hover()
    {
        if(Card.Busy == 0 && transform.localPosition.z > -0.1f)
        {
            Vector3 localPosition = transform.localPosition;
            localPosition.z = -Scale * 1.25f;
            transform.localPosition = localPosition;
        }
        
        target = -Scale;
    }
}