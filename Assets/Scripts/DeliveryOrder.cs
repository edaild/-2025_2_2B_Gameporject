using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// 간단한 배달 주문
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
        timeLimit = Random.Range(60f, 120f);           // 1~2 분제한
        reward = rewardAmount;
        state = OrderState.WaitingPickup;
    }

    public float GetRemainingTime()
    {
        return Mathf.Max(0f, timeLimit - (Time.time - orderTime));              // 남은 시간 리턴
    }

    public bool IsExpired()
    {
        return GetRemainingTime() <= 0;                                        // 주문 소멸
    }
}
