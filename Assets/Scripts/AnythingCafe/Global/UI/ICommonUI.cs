using Cysharp.Threading.Tasks;

public interface ICommonUI
{
    UniTask Open();

    UniTask Close();
}
