public interface IHasDataTemplate<in T> where T : IDataTemplate, new()
{
    void Init(T param);
}