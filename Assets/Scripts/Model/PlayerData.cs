


// ��������ڴ洢��ң���Ϸ��Ҫ��ɫ�������ݣ�������ҵײ�model�Ľ���


using System.Collections.Generic;

public class PlayerData 
{
    private const int MAXVAR = 100;//����ֵ�����
    public int Full { get; private set; } = MAXVAR;//����ֵ��ʼֵ��

    public int San { get; private set; } = MAXVAR;//����
    public int Clean { get; private set; } = MAXVAR;//���
    public int Love { get; private set; } = 0;//�øж�
    public int Money { get; private set; } = 0;//��Ǯ

    public List<PersonalityType> personalities { get; private set; } = new List<PersonalityType>();//�Ը��б�

}
