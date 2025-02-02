using Cysharp.Threading.Tasks;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 异步场景加载器
/// </summary>
public class SceneLoader
{
    private readonly SceneEnum _sceneEnum;

    public SceneLoader(SceneEnum sceneEnum) => _sceneEnum = sceneEnum;

    public async UniTask LoadScene()
    {
        try
        {
            Scene activeScene;
            var sceneHandlerList1 =
               GameObject.FindGameObjectsWithTag("SceneHandler")
                    .Where(sceneHandlerObject => sceneHandlerObject.TryGetComponent(out ISceneHandler _))
                    .Select(sceneHandler => sceneHandler.GetComponent<ISceneHandler>())
                    .ToList();
            if (sceneHandlerList1.Count > 1)
            {
                activeScene = SceneManager.GetActiveScene();
                Debug.LogError($"[SceneLoader] The scene \"{activeScene.name}\" has more than 1 SceneHandler before load");
            }
            if (sceneHandlerList1.Count > 0)
            {
                await sceneHandlerList1.First().OnSceneUnload();
            }
            else
            {
                // TODO: 不需要卸载场景，直接加载Loading的UI（记得在OnSceneUnload里写加载LoadingUI的逻辑）
            }

            await SceneManager.LoadSceneAsync(_sceneEnum.ToString());

            var sceneHandlerList2 =
                GameObject.FindGameObjectsWithTag("SceneHandler")
                    .Where(sceneHandlerObject => sceneHandlerObject.TryGetComponent(out ISceneHandler _))
                    .Select(sceneHandler => sceneHandler.GetComponent<ISceneHandler>())
                    .ToList();
            if (sceneHandlerList2.Count > 1)
            {
                activeScene = SceneManager.GetActiveScene();
                Debug.LogError($"[SceneLoader] The scene \"{activeScene.name}\" has more than 1 SceneHandler after load");
            }
            if (sceneHandlerList2.Count > 0)
            {
                await sceneHandlerList2.First().OnSceneLoad();
            }
            else
            {
                // TODO: 不需要加载场景，直接关闭Loading的UI（记得在OnSceneLoad里写清除LoadingUI的逻辑）
            }
        }
        catch (Exception ex)
        {
            throw ex is CustomErrorException
                ? ex : new CustomErrorException(
                    $"[SceneLoader] Scene {_sceneEnum.ToString()} load error.{ex.Message}",
                    new CustomErrorItem(ErrorSeverity.Error, ErrorCode.SceneCantLoad));
        }
    }
}
