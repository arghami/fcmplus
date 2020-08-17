namespace math.utils
{
    public class NumParser
    {

        public static int parseInt(string s)
        {
            int ret;
            int.TryParse(s, out ret);
            return ret;
        }

        public static double parseDouble(string s)
        {
            double ret;
            double.TryParse(s, out ret);
            return ret;
        }
    }
}
