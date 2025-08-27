


// ��������ڴ洢��Ϸ��Ҫ��ɫ�����ݣ�����ײ�model�Ľ���


using System;
using System.Collections.Generic;
using UnityEngine;

public class CharacterData 
{
    public const int MaxVar = 100;//����ֵ�����

    public string Name { get; set; } = "����";//��ɫ����//Ŀǰд��Ӳ����todo

    public int Full { get; private set; } = MaxVar;//����ֵ��ʼֵ��

    public int San { get; private set; } = MaxVar;//����
    public int Clean { get; private set; } = MaxVar;//���

    public HealthState HealthState { get; private set; } = HealthState.Normal;//����״̬
    public int Love { get; private set; } = 0;//�øж�

    public HashSet<string> PersonalityTags { get; private set; }

    public DateTime FirstStartTime { get; private set; }

    public event Action<string, int> OnCharacterStateChanged;// ��ɫ״̬�仯�¼�������Ϊ״̬���ƺͱ仯ֵ


    // ĿǰӲ���룬д����һ����ɫ���Ը��ǩ
    public CharacterData()
    {
        FirstStartTime = DateTime.Now; // ��¼��һ�ο�ʼ��Ϸ��ʱ��
        PersonalityTags = new HashSet<string> { "mature", "calm" }; 
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
                    ChangeLove(effect.Value * quantity);
                    break;
                default:
                    return OperationResult.Fail("��ƷЧ������δ֪���ͣ�������ƷЧ����");
            }
        }
        return OperationResult.Complete();
    }

    // TODO:ʹ����Ʒ����ʾ�Ի���

    public OperationResult ApplyEffect(EffectType type, int quantity)
    {
        switch (type)
        {
            case EffectType.Full:
                ChangeFull(quantity);
                break;
            case EffectType.San:
                ChangeSan(quantity);
                break;
            case EffectType.Clean:
                ChangeClean(quantity);
                break;
            case EffectType.Love:
                ChangeLove(quantity);
                break;
            default:
                return OperationResult.Fail("δ֪Ч�����ͣ����鵱ǰ�¼�");
        }
        return OperationResult.Complete();
    }


    public void ChangeFull(int delta)
    {
        Full = Mathf.Clamp(Full + delta, 0, 100);
        OnCharacterStateChanged?.Invoke("Full", Full);
    }

    public void ChangeClean(int delta)
    {
        Clean = Mathf.Clamp(Clean + delta, 0, 100);
        OnCharacterStateChanged?.Invoke("Clean", Clean);
    }

    public void ChangeSan(int delta)
    {
        San = Mathf.Clamp(San + delta, 0, 100);
        OnCharacterStateChanged?.Invoke("San", San);
    }

    public void ChangeLove(int delta)
    {
        Love = Mathf.Max(0, Love + delta); // Love����С��0
        OnCharacterStateChanged?.Invoke("Love", Love);
    }


}
