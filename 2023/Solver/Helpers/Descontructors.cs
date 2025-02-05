namespace AdventOfCode.Helpers
{
    public static class Deconstructors
    {
        public static void Deconstruct(this string[] array, out string a, out string b)
        {
            a = array[0];
            b = array[1];
        }

        public static void Deconstruct(this string[] array, out string a, out string b, out string c)
        {
            a = array[0];
            b = array[1];
            c = array[2];
        }
    }
}
