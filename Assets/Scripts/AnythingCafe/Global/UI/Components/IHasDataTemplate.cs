public interface IHasDataTemplate<in T> where T : class, new()
{
    void Init(T param);
}