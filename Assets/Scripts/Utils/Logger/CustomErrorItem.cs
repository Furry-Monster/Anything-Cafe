// ע:�ð汾Unity��Ӧ��C#�汾���޷�ʹ��struct���캯�����ʸ���record���棬��������һ��Ӱ�졣
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
