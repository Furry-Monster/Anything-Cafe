using Cysharp.Threading.Tasks;

public interface IReactiveComponent
{
    UniTask Open();

    UniTask Close();
}
