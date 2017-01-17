using UnityEngine;
using System.Collections;

public class CardButton : MonoBehaviour
{
    public Card Card;
    public GameObject LeftArrow;
    public GameObject RightArrow;
    public bool IncreaseTimeRange;

    public void ModifyRange()
    {
        if(Card.Busy == 0)
        {
            if(IncreaseTimeRange)
            {
                if(Card.Range != Card.TimeRange.Month)
                    Card.ViewTimeRange(Card.Range + 1);
            }
            else
            {
                if (Card.Range != Card.TimeRange.Day)
                    Card.ViewTimeRange(Card.Range - 1);
            }
        }

        LeftArrow.SetActive(Card.Range != Card.TimeRange.Month);
        RightArrow.SetActive(Card.Range != Card.TimeRange.Day);
    }
}
