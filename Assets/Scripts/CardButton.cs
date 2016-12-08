using UnityEngine;
using System.Collections;

public class CardButton : MonoBehaviour
{
    public Card Card;
    public bool IncreaseTimeRange;

    public void ModifyRange()
    {
        if(Card.Busy == 0)
        {
            if(IncreaseTimeRange)
            {
                if(Card.Range != Card.TimeRange.Year)
                    Card.SetTimeRange(Card.Range + 1);
            }
            else
            {
                if(Card.Range != Card.TimeRange.Day)
                    Card.SetTimeRange(Card.Range - 1);
            }
        }
    }
}
