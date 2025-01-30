using Cysharp.Threading.Tasks;

public interface ICommonComponent
{
    UniTask Show(float duration);

    UniTask Hide(float duration);
}
