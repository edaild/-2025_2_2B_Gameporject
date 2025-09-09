using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Events;

public class Building : MonoBehaviour
{
    [Header("�ǹ� ����")]
    public BuildingType BuildingType;
    public string buildingName = "�ǹ�";

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
                Debug.Log($"{buildingName} ���� ��� �Ϸ�");
                break;
            case BuildingType.Coustomer:
                Debug.Log($"{buildingName} ���� ������ �Ⱦ� �߽��ϴ�.");
                break;
            case BuildingType.ChargingStation:
                Debug.Log($"{buildingName} ���� ���͸��� ���� �߽��ϴ�.");
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
            Debug.Log($"{buildingName} �� �������ϴ�.");
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
                    buildingName = "������";
                    break;
                case BuildingType.Coustomer:
                    mat.color = Color.green;
                    buildingName = "�� ��";
                    break;
                case BuildingType.ChargingStation:
                    mat.color = Color.yellow;
                    buildingName = "������";
                    break;
            }
        }
        Collider col = GetComponent<Collider>();
        if(col != null) { col.isTrigger = true; }
    }
}
