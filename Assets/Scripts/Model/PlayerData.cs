using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// ������ʾ��ң�ʹ�����������û������ݣ���������ҵײ�model�Ľ���
public class PlayerData 
{
    public int Money { get; private set; } = 0;//��Ǯ

    public Dictionary<string, int> Items { get; private set; } = new Dictionary<string, int>();//��������ɶ��Ʒ
}
