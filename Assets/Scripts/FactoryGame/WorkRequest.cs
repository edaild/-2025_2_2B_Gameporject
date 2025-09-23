using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 요청서 클랙스
public class WorkRequest
{
    public ProductType productType;
    public int quantity;
    public int reward;

    public WorkRequest(ProductType productType, int quantity, int reward)           // 생성자
    {
        this.productType = productType;
        this.quantity = quantity;
        this.reward = reward;
    }
}
