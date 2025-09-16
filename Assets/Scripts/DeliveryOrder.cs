using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// ������ ��� �ֹ�
[System.Serializable]
public class DeliveryOrder : MonoBehaviour
{
    public int orderId;
    public string restaurantName;
    public string customerName;
    public Building restaurantBuilding;
    public Building custmerBuilding;
    public float orderTime;
    public float timeLimit;
    public float reward;
    public OrderState state;

    public DeliveryOrder(int id, Building restaurant, Building customer, float rewardAmount)
    {
        orderId = id;
        restaurantBuilding = restaurant;
        custmerBuilding = customer;
        restaurantName = restaurant.buildingName;
        customerName = customer.buildingName;
        orderTime = Time.deltaTime;
        timeLimit = Random.Range(60f, 120f);           // 1~2 ������
        reward = rewardAmount;
        state = OrderState.WaitingPickup;
    }

    public float GetRemainingTime()
    {
        return Mathf.Max(0f, timeLimit - (Time.time - orderTime));              // ���� �ð� ����
    }

    public bool IsExpired()
    {
        return GetRemainingTime() <= 0;                                        // �ֹ� �Ҹ�
    }
}
