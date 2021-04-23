using cAlgo.API;

namespace cAlgo.Helpers
{
    public static class ColorParser
    {
        public static Color Parse(string colorString, int alpha = 255)
        {
            var color = colorString[0] == '#' ? Color.FromHex(colorString) : Color.FromName(colorString);

            return Color.FromArgb(alpha, color);
        }
    }
}