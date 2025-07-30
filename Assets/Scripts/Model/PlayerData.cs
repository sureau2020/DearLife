using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// ������ʾ��ң�ʹ�����������û������ݣ���������ҵײ�model�Ľ���
public class PlayerData 
{
    public const int MaxBagCapacity = 30; // �����������30�ͬ��Ʒ
    public int Money { get; private set; } = 0;//��Ǯ

    public List<ItemEntryData> Items { get; private set; } = new List<ItemEntryData>();//��������ɶ��Ʒ



    // ���������������Ʒ��ûǮ���أ�����������û����ͬ��Ʒ���ء�(����ʧ��OperationResult)
    // �ɹ����Ǯ�������Ʒ��������û����ͬ��Ʒ���½�һ��еĻ�������1��������list��λ�����سɹ�OperationResult
    public OperationResult BuyItem(int cost, ItemData item, int quantity) {
        if (!IsMoneyEnough(cost * quantity)) { 
            return OperationResult.Fail("��Ҳ��㡣");
        }
        ItemEntryData isInBag = GetTheItemInBag(item.Id);
        if (!IsBagHasSpace() && isInBag == null)
        {
            return OperationResult.Fail("����������");
        }
        Money -= cost; 
        if( isInBag == null)
        {
            // ������û�������Ʒ���������Ŀ
            ItemEntryData newItem = new(item.Id, quantity);
            Items.Insert(0, newItem);
        }
        else
        {
            isInBag.Count+= quantity;
            Items.Remove(isInBag);
            Items.Insert(0, isInBag);
        }
        return OperationResult.Complete();
    }


    // �������п�λ��
    public bool IsBagHasSpace()
    {
        return Items.Count <= MaxBagCapacity;
    }



    // ��鱳�����Ƿ�����ͬ��Ʒ,�еĻ����ظ���Ʒ��Ŀ��û���򷵻�null
    public ItemEntryData GetTheItemInBag(string itemId)
    {
        foreach (var entry in Items)
        {
            if (entry.ItemID == itemId)
            {
                return entry;
            }
        }
        return null;
    }

    public bool IsMoneyEnough(int cost)
    {
        return Money >= cost;
    }




}
