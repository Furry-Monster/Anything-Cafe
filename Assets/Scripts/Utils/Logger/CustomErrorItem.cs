// 注:该版本Unity对应的C#版本，无法使用struct构造函数，故改用record代替，对性能有一定影响。
public record CustomErrorItem
{
    public ErrorSeverity Severity;
    public ErrorCode Code;

    public CustomErrorItem(ErrorSeverity severity, ErrorCode code)
    {
        Severity = severity;
        Code = code;
    }
}
