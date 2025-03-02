public interface ICondition
{
    bool IsMet();

    string GetFailureReason();
}
