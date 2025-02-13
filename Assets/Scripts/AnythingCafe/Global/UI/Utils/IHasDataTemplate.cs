public interface IHasDataTemplate<in T> where T : IDataTemplate
{
    void LoadTemplate(T param);
}