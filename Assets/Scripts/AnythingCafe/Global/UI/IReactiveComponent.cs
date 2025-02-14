using Cysharp.Threading.Tasks;

/// <summary>
/// UI系统组件接口
/// </summary>
public interface IReactiveComponent
{
    UniTask Open();

    UniTask Close();
}
