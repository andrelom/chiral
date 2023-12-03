namespace Chiral.Core.Utilities;

public static class CancellationTokenUtility
{
    public static CancellationTokenSource CreateSource(CancellationToken token = default)
    {
        if (token != default)
        {
            return CancellationTokenSource.CreateLinkedTokenSource(token);
        }

        return new CancellationTokenSource();
    }

    public static CancellationTokenSource CreateTimeoutSource(TimeSpan time, CancellationToken token = default)
    {
        var source = CreateSource(token);

        source.CancelAfter(time);

        return source;
    }
}
