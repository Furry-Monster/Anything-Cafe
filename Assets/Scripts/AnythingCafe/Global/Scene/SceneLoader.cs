using System;
using UnityEngine;

/// <summary>
/// 场景枚举,手动修改。
/// 游戏打包时，记得在发布中勾选所有场景，并按SceneEnum中的顺序排序
/// </summary>
public enum SceneEnum
{
    TitleScene,
    PlayScene,
    ScenarioScene
}

/// <summary>
/// 异步场景加载器
/// </summary>
public class SceneLoader
{
    private SceneEnum _sceneEnum;

    public SceneLoader(SceneEnum sceneEnum) => _sceneEnum = sceneEnum;

    public async void LoadScene(SceneEnum scene)
    {
        try
        {

        }
        catch (Exception ex)
        {

        }
    }
}
