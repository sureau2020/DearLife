


// ��������ڴ洢��Ϸ��Ҫ��ɫ�����ݣ�����ײ�model�Ľ���


using System;
using System.Collections.Generic;

public class CharacterData 
{
    private const int MAXVAR = 100;//����ֵ�����
    public int Full { get; private set; } = MAXVAR;//����ֵ��ʼֵ��

    public int San { get; private set; } = MAXVAR;//����
    public int Clean { get; private set; } = MAXVAR;//���
    public int Love { get; private set; } = 0;//�øж�

    public List<PersonalityType> personalities { get; private set; } = new List<PersonalityType>();//�Ը��б�

    public DateTime FirstStartTime { get; private set; }

}
