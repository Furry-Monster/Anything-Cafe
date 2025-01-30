using Cysharp.Threading.Tasks;
using UnityEngine;

public class CommonUI : MonoBehaviour,ICommonUI
{
    public UniTask Open()
    {
        this.gameObject.SetActive(true);
        return new UniTask();
    }

    public UniTask Close()
    {
        this.gameObject.SetActive(false);
        return new UniTask();
    }
}
