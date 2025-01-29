using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;


public class GameManager : PersistentSingleton<GameManager>
{
    private List<GameObject> _validateIntegrityObjects; // TODO:将列表序列化，显示在Inspector面板中，方便编辑

    private async void Start()
    {
        try
        {
            await PrepareGame();
            await ValidateIntegrity();
            await ReadyTitleScene();

#pragma warning disable CS1998 // 异步方法缺少 "await" 运算符，将以同步方式运行
            static async Task ValidateIntegrity()
            {
                // TODO: 检测游戏资源完整性
            }

            static async Task PrepareGame()
            {
                // TODO: 准备游戏资源,初始化游戏
            }

            static async Task ReadyTitleScene()
            {
                // TODO：加载Title场景
                // var component = GameObject.FindWithTag("Scene Handler").GetComponent<ISceneHandler>();
                // if (component == null) return;
                // await component.OnSceneLoad();
            }
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            Application.Quit();
        }
    }


}
