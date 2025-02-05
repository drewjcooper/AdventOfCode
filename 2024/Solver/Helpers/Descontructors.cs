namespace AdventOfCode.Helpers
{
    public static class Deconstructors
    {
        public static void Deconstruct<T>(this T[] array, out T a, out T b)
        {
            a = array[0];
            b = array[1];
        }

        public static void Deconstruct<T>(this T[] array, out T a, out T b, out T c)
        {
            a = array[0];
            b = array[1];
            c = array[2];
        }
    }
}
