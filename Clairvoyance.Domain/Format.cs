namespace Clairvoyance.Domain;

/// <summary>
/// A play format.
/// </summary>
public class Format
{
    private static readonly Format _None;
    public static Format None { get { return _None; } }
    private static readonly Format _Commander;
    public static Format Commander { get { return _Commander; } }
    private static readonly Format _Legacy;
    public static Format Legacy { get { return _Legacy; } }
    private static readonly Format _Modern;
    public static Format Modern { get { return _Modern; } }
    private static readonly Format _Standard;
    public static Format Standard { get { return _Standard; } }

    static Format()
    {
        _None = new Format();
        _Commander = new Format();
        _Legacy = new Format();
        _Modern = new Format();
        _Standard = new Format();
    }
}