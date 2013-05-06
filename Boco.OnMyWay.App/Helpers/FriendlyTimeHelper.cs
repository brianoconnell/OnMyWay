namespace Boco.OnMyWay.App.Helpers
{
    using System;

    public static class FriendlyTimeHelper
    {
        public static string TimeSpanToFriendlyTime(TimeSpan timeSpan)
        {
            double delta = Math.Abs(timeSpan.TotalSeconds);

            if (delta < 60) return "about a minute";
            if (delta < 120) return "a couple of minutes";
            if (delta < 300) return "about 5 minutes";
            if (delta < 600) return "less than 10 minutes";
            if (delta < 900) return "about 15 minutes";
            if (delta < 1200) return "about 20 minutes";
            if (delta < 1800) return "about half an hour";
            if (delta < 2700) return "about 40-45 minutes";
            if (delta < 3600) return "about an hour or less";
            if (delta < 5600) return "about an hour and a half";
            if (delta < 7200) return "about two hours";
            if (delta < 9000) return "about 2 and a half hours";

            return "whenever";
        }
    }
}