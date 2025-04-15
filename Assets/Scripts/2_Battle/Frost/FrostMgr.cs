using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class FrostMgr : MonoBehaviour
{
    [Button]
    public async void Show()
    {
        await TimerAsync(0.6f, progress =>
        {
            GetComponent<Renderer>().material.SetFloat("_Value", progress);
        });
        _ = TimerAsync(0.6f, progress =>
        {
            transform.GetChild(0).GetComponent<Renderer>().material.SetFloat("_Value", progress);
        });
    }
    [Button]
    public async void Close()
    {
        _ = TimerAsync(0.4f, progress =>
        {
            GetComponent<Renderer>().material.SetFloat("_Value", 1 - progress);
        });
        _ = TimerAsync(0.6f, progress =>
        {
            transform.GetChild(0).GetComponent<Renderer>().material.SetFloat("_Value", 1 - progress);
        });
    }
    public static async Task TimerAsync(float stopTime, Action<float> runAction = null, Action stopAction = null)
    {
        int currentMs = 0;
        int stopMs = (int)(stopTime * 1000);
        while (currentMs <= stopMs)
        {
            //Debug.Log("��ǰ" + (currentMs));
            //�������˲��ֹͣ�����ֱ�ӷ���100%�����򷵻ذٷֱ�
            runAction(stopTime == 0 ? 1 : currentMs * 1f / stopMs);
            currentMs += 50;
            await Task.Delay(50);
        }
        if (stopAction != null)
        {
            stopAction();
        }
        //Debug.Log("������ӡ"+( time - DateTime.Now));
    }
}
