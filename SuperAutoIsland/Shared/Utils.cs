namespace SuperAutoIsland.Shared;

public static class Utils
{
    public static string GenerateRandomId()
    {
        return Guid.NewGuid().ToString("N")[..8];
    }
}