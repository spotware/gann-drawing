namespace cAlgo.Helpers
{
    public static class DoubleToStringNonScientificExtension
    {
        public static string ToNonScientificString(this double value, int digits = 339)
        {
            return value.ToString("0." + new string('#', digits));
        }
    }
}