

// ������ʾ��Ϸ�е���Ʒ���ݣ���������Ʒ�ײ�model�Ľ�����
using System.Collections.Generic;

public class ItemData
{
    public string Name { get; private set; } 
    public string Description { get; private set; }

    public int Price { get; private set; }

    public bool IsConsumable { get; private set; } // �Ƿ�������Ʒ,��������Ʒ��ֻ����һ��

    public Dictionary<EffectType, int> Effect { get; private set; }
}
