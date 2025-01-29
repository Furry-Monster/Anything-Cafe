using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : PersistentSingleton<GameManager>
{
    private List<GameObject> _validateIntegrityObjects; // TODO:���б����л�����ʾ��Inspector����У�����༭

    private async void Start()
    {
        try
        {
            await PrepareGame();
            await ValidateIntegrity();
            await ReadyTitleScene();

#pragma warning disable CS1998 // �첽����ȱ�� "await" �����������ͬ����ʽ����
            static async UniTask ValidateIntegrity()
            {
                // TODO: �����Ϸ��Դ������
            }

            static async UniTask PrepareGame()
            {
                // TODO: ׼����Ϸ��Դ,��ʼ����Ϸ
                SoundManager.Instance.ToString();
            }

            static async UniTask ReadyTitleScene()
            {
                // TODO������Title����
                // var component = GameObject.FindWithTag("Scene Handler").GetComponent<ISceneHandler>();
                // if (component == null) return;
                // await component.OnSceneLoad();
            }
        }
        catch (Exception ex)
        {
            if (ex is CustomErrorException customErrorException)
            {
                // ͨ��UIManager��UI������ʾ������Ϣ������ʾ�˳�
                throw ex;
            }
            // ͨ��UIManager��UI������ʾ������Ϣ������ʾ�˳�
            throw new CustomErrorException(ex.Message, new CustomErrorItem(ErrorSeverity.ForceQuit, ErrorCode.GameInitFailed));
        }
    }


}
