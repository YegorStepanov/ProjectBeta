public static class ObjectUtil
{
    public static void Swap<T1>(ref T1 left, ref T1 right)
    {
        (left, right) = (right, left);
    }
}