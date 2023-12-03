namespace Chiral.Network;

internal static class Constants
{
    public static readonly int TcpServerSemaphoreLimit = 256;

    public static readonly TimeSpan TcpServerConnectionTimeout = TimeSpan.FromSeconds(15);
}
