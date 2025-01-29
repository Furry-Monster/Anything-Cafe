using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
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
            static async Task ValidateIntegrity()
            {
                // TODO: �����Ϸ��Դ������
            }

            static async Task PrepareGame()
            {
                // TODO: ׼����Ϸ��Դ,��ʼ����Ϸ
            }

            static async Task ReadyTitleScene()
            {
                // TODO������Title����
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
