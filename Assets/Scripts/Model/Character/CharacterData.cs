


// ��������ڴ洢��Ϸ��Ҫ��ɫ�����ݣ�����ײ�model�Ľ���


using System;
using System.Collections.Generic;

public class CharacterData 
{
    private const int MaxVar = 100;//����ֵ�����
    public int Full { get; private set; } = MaxVar;//����ֵ��ʼֵ��

    public int San { get; private set; } = MaxVar;//����
    public int Clean { get; private set; } = MaxVar;//���
    public int Love { get; private set; } = 0;//�øж�

    public List<PersonalityType> Personalities { get; private set; } = new List<PersonalityType>();//�Ը��б�

    public DateTime FirstStartTime { get; private set; }



    public OperationResult ApplyItemEffect(ItemData item, int quantity) {
        if (item == null)
        {
            return OperationResult.Fail("��Ʒ�����ڡ�������Ʒ���ݿ��𻵣�����������ԭ���ݿ�json�ļ���");
        }

        foreach (var effect in item.Effect)
        {
            switch (effect.Key)
            {
                case EffectType.Full:
                    Full = Math.Min(MaxVar, Full + effect.Value * quantity);
                    break;
                case EffectType.San:
                    San = Math.Min(MaxVar, San + effect.Value * quantity);
                    break;
                case EffectType.Clean:
                    Clean = Math.Min(MaxVar, Clean + effect.Value * quantity);
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
}
