class Data
{
    static public volatile bool IsWebOK = false;
    static public volatile bool IsOpen = true;
#if DEBUG
    public const bool IsDEBUG = true;
#else
    public const bool IsDEBUG = false;
#endif
}