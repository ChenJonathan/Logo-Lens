using System;

public static class Util
{

	public static string FormatDate(DateTime date)
    {
        return (date.Month + "").PadLeft(2, '0') + "/" + (date.Day + "").PadLeft(2, '0') + "/" + date.Year;
    }

    public static string FormatTime(DateTime time)
    {
        return (time.Hour + "").PadLeft(2, '0') + ":" + (time.Minute + "").PadLeft(2, '0');
    }
}