using Cysharp.Threading.Tasks;

public interface ICommonUI
{
    UniTask Show(float duration);

    UniTask Hide(float duration);
}
