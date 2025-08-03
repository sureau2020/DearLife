


// ��������ڴ洢��Ϸ��Ҫ��ɫ�����ݣ�����ײ�model�Ľ���


using System;
using System.Collections.Generic;
using UnityEngine;

public class CharacterData 
{
    private const int MaxVar = 100;//����ֵ�����
    public int Full { get; private set; } = MaxVar;//����ֵ��ʼֵ��

    public int San { get; private set; } = MaxVar;//����
    public int Clean { get; private set; } = MaxVar;//���

    public HealthState HealthState { get; private set; } = HealthState.Normal;//����״̬
    public int Love { get; private set; } = 0;//�øж�

    public List<PersonalityType> Personalities { get; private set; } = new List<PersonalityType>();//�Ը��б�

    public DateTime FirstStartTime { get; private set; }

    public CharacterData()
    {
        FirstStartTime = DateTime.Now; // ��¼��һ�ο�ʼ��Ϸ��ʱ��
    }


    // ��������������ʹ����Ʒ��Ӧ��Ч�������ز������
    public OperationResult ApplyItemEffect(string itemId, int quantity) {
        ItemData item = ItemDataBase.GetItemById(itemId);
        if (item == null)
        {
            return OperationResult.Fail("��Ʒ�����ڡ�������Ʒ���ݿ��𻵣�����������ԭ���ݿ�json�ļ���");
        }

        foreach (var effect in item.Effect)
        {
            switch (effect.Key)
            {
                case EffectType.Full:
                    ChangeFull(effect.Value * quantity);
                    break;
                case EffectType.San:
                    ChangeSan(effect.Value * quantity);
                    break;
                case EffectType.Clean:
                    ChangeClean(effect.Value * quantity);
                    break;
                case EffectType.Love:
                    Love += effect.Value * quantity;
                    break;
                default:
                    return OperationResult.Fail("��ƷЧ������δ֪���ͣ�������ƷЧ����");
            }
        }
        return OperationResult.Complete();
    }

    // TODO:ʹ����Ʒ����ʾ�Ի���


    public void ChangeFull(int delta)
    {
        Full = Mathf.Clamp(Full + delta, 0, 100);
    }

    public void ChangeClean(int delta)
    {
        Clean = Mathf.Clamp(Clean + delta, 0, 100);
    }

    public void ChangeSan(int delta)
    {
        San = Mathf.Clamp(San + delta, 0, 100);
    }



}
