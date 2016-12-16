using System;

public static class Util
{

	public static string FormatDate(DateTime date)
    {
        return (date.Month + "").PadLeft(2, '0') + "/" + (date.Day + "").PadLeft(2, '0') + "/" + date.Year;
    }
}