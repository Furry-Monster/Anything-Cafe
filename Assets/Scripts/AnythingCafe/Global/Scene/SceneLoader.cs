using System;
using UnityEngine;

/// <summary>
/// ����ö��,�ֶ��޸ġ�
/// ��Ϸ���ʱ���ǵ��ڷ����й�ѡ���г���������SceneEnum�е�˳������
/// </summary>
public enum SceneEnum
{
    TitleScene,
    PlayScene,
    ScenarioScene
}

/// <summary>
/// �첽����������
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
