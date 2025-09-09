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

    void HandleDriverService(DeliveryDriver delivery)
    {
        switch (BuildingType)
        {
            case BuildingType.Restautant:
                Debug.Log($"{buildingName} 에서 배달 완료");
                break;
            case BuildingType.Coustomer:
                Debug.Log($"{buildingName} 에서 음식을 픽업 했습니다.");
                break;
            case BuildingType.ChargingStation:
                Debug.Log($"{buildingName} 에서 베터리를 충전 했습니다.");
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
}
