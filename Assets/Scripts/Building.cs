using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Events;

public class Building : MonoBehaviour
{
    [Header("건물 정보")]
    public BuildingType BuildingType;
    public string buildingName = "건물";

    [System.Serializable]

    public class BuildingEvents
    {
        public UnityEvent<string> OnDriverEntered;
        public UnityEvent<string> OnDriverExited;
        public UnityEvent<BuildingType> OnServiceUsed;
    }

    public BuildingEvents buildingEvents;

    private DeliveryOrderSystem orderSystem;

    void HandleDriverService(DeliveryDriver deliver)
    {
        switch (BuildingType)
        {
            case BuildingType.Restautant:
               if(orderSystem != null)
                {
                    orderSystem.OnDriverEnteredRestaurant(this);
                }
                break;
            case BuildingType.Coustomer:
                if (orderSystem != null)
                {
                    orderSystem.OnDriverEnteredRestaurant(this);
                }
                else
                {
                    deliver.CompleteDelivery();
                }
                break;
            case BuildingType.ChargingStation:
                deliver.ChargeBattery();
                break;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        DeliveryDriver dirver = other.GetComponent<DeliveryDriver>();
        if(dirver != null)
        {
            buildingEvents.OnDriverEntered?.Invoke(buildingName);
            HandleDriverService(dirver);
        }
    }

    private void OnTriggerExit(Collider other)
    {
       DeliveryDriver driver = other.GetComponent<DeliveryDriver>();
        if(driver != null)
        {
            buildingEvents.OnDriverExited?.Invoke(buildingName);
            Debug.Log($"{buildingName} 을 떠났습니다.");
        }
    }

    void Start()
    {
        SetupBuilding();
        orderSystem = FindObjectOfType<DeliveryOrderSystem>();
        CreateNameTag();
    }

    void SetupBuilding()
    {
        Renderer renderer = GetComponent<Renderer>();
        if(renderer != null)
        {
            Material mat = renderer.material;
            switch (BuildingType) {
                case BuildingType.Restautant:
                    mat.color = Color.red;
                    buildingName = "음식점";
                    break;
                case BuildingType.Coustomer:
                    mat.color = Color.green;
                    buildingName = "고객 집";
                    break;
                case BuildingType.ChargingStation:
                    mat.color = Color.yellow;
                    buildingName = "충전소";
                    break;
            }
        }
        Collider col = GetComponent<Collider>();
        if(col != null) { col.isTrigger = true; }
    }

    void CreateNameTag()
    {
        // 건물 위에 이름표 생성
        GameObject nameTag = new GameObject("NameTage");
        nameTag.transform.SetParent(transform);
        nameTag.transform.localPosition = Vector3.up * 1.5f;

        TextMesh textMesh  = nameTag.AddComponent<TextMesh>();
        textMesh.text = buildingName;
        textMesh.characterSize = 0.2f;
        textMesh.anchor = TextAnchor.MiddleCenter;
        textMesh.color = Color.white;
        textMesh.fontSize = 20;

        nameTag.AddComponent<Bildboard>();
    }


}
