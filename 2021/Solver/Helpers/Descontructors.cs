namespace AdventOfCode2021.Helpers
{
    public static class Deconstructors
    {
        public static void Deconstruct(this string[] array, out string a, out string b)
        {
            a = array[0];
            b = array[1];
        }
    }
}
