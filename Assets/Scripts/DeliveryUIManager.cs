using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeliveryUIManager : MonoBehaviour
{
    [Header("UI ���")]
    public Text statusText;
    public Text messageText;
    public Slider batterySlider;
    public Image batteryFill;

    [Header("���� ������Ʈ")]
    public DeliveryDriver driver;

    // Start is called before the first frame update
    void Start()
    {
        if(driver != null)
        {
            //Event ����
            driver.driverEvents.OnMoneyChanged.AddListener(UpdateMoney);
            driver.driverEvents.OnBatteryChanged.AddListener(UpdateBattery);
            driver.driverEvents.OnDeliverycountChanged.AddListener(UpdateDeliveryCount);
            driver.driverEvents.OnMoveStarted.AddListener(OnmoveStarted);
            driver.driverEvents.OnMoveStoped.AddListener(OnMovStoped);
            driver.driverEvents.OnLowBattery.AddListener(OnLowBattery);
            driver.driverEvents.OnLowBatteryEmpty.AddListener(OnBatteryEmpty);
            driver.driverEvents.OnDeliveryCompleted.AddListener(OnDeliveryCompleted);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(statusText != null && driver != null)
        {
            statusText.text = driver.GetStatueText();
        }
    }

    void ShowMessage(string message, Color color)
    {
        if(messageText != null)
        {
            messageText.text = message;
            messageText.color = color;
            StartCoroutine(ClearMessageAgterDelay(2f));
        }
    }

    IEnumerator ClearMessageAgterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if(messageText != null)
        {
            messageText.text = "";
        }
    }
    void UpdateMoney(float money)
    {
        ShowMessage($"�� : {money} ��", Color.green);
    }
    void UpdateBattery(float battery)
    {
        if(batterySlider != null)
        {
            if (battery > 50f)
                batteryFill.color = Color.green;
            else if (battery > 20f)
                batteryFill.color = Color.yellow;
            else
                batteryFill.color = Color.red;
        }
    }

    void UpdateDeliveryCount(int count)
    {
        ShowMessage($"���� �Ϸ� : {count}��", Color.blue);
    }

    void OnmoveStarted()
    {
        ShowMessage("�̵� ����", Color.cyan);
    }

    void OnMovStoped()
    {
        ShowMessage("�̵� ����", Color.gray);
    }

    void OnLowBattery()
    {
        ShowMessage("���͸� ����!", Color.red);
    }

    void OnBatteryEmpty()
    {
        ShowMessage("���͸� ����", Color.red);
    }

    void OnDeliveryCompleted()
    {
        ShowMessage("��� �Ϸ�!", Color.green);
    }

    void UpdateUI()
    {
        if(driver != null)
        {
            UpdateMoney(driver.currentMoney);
            UpdateBattery(driver.batteryLevel);
            UpdateDeliveryCount(driver.deliveryCount);
        }
    }

    private void OnDestroy()
    {
        if (driver != null)
        {
            //Event ���� ����
            driver.driverEvents.OnMoneyChanged.AddListener(UpdateMoney);
            driver.driverEvents.OnBatteryChanged.AddListener(UpdateBattery);
            driver.driverEvents.OnDeliverycountChanged.AddListener(UpdateDeliveryCount);
            driver.driverEvents.OnMoveStarted.AddListener(OnmoveStarted);
            driver.driverEvents.OnMoveStoped.AddListener(OnMovStoped);
            driver.driverEvents.OnLowBattery.AddListener(OnLowBattery);
            driver.driverEvents.OnLowBatteryEmpty.AddListener(OnBatteryEmpty);
            driver.driverEvents.OnDeliveryCompleted.AddListener(OnDeliveryCompleted);
        }
    }

}
