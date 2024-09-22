namespace CSBDashboardServer.Helpers
{
    public class JSDateHelper
    {
        static readonly long epochTicks = new DateTime(1970, 1, 1, 0, 0, 0, 0).Ticks;
        public static long ToJSTicks(string isoDatetime) { return (Convert.ToDateTime(isoDatetime).Ticks - epochTicks)/ 10000; }
    }
}
