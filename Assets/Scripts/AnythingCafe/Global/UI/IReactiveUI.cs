using Cysharp.Threading.Tasks;

public interface IReactiveUI
{
    UniTask Open();

    UniTask Close();
}
