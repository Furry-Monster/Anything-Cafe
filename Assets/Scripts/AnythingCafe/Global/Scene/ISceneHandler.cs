using System.Threading.Tasks;

public interface ISceneHandler
{
    public Task OnSceneLoad();

    public Task OnSceneUnload();
}
