using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EconomicToolsUI : MonoBehaviour
{
    [Header("������ ������������")]
    public Button printMoneyButton;
    public Button taxesUpButton;
    public Button taxesDownButton;
    public Button subsidiesButton;
    public Button interestUpButton;
    public Button interestDownButton;

    [Header("��������� ����")]
    public TextMeshProUGUI taxRateText;
    public TextMeshProUGUI interestRateText;
    public TextMeshProUGUI moneyText;
    public TextMeshProUGUI inflationText;

    void Start()
    {
        // ����������� ����������� ������
        printMoneyButton.onClick.AddListener(OnPrintMoney);
        taxesUpButton.onClick.AddListener(OnTaxesUp);
        taxesDownButton.onClick.AddListener(OnTaxesDown);
        subsidiesButton.onClick.AddListener(OnSubsidies);
        interestUpButton.onClick.AddListener(OnInterestUp);
        interestDownButton.onClick.AddListener(OnInterestDown);
    }

    void Update()
    {
        // ��������� ��������� ����
        if (GameManager.Instance != null)
        {
            taxRateText.text = $"������: {GameManager.Instance.taxRate:P0}";
            interestRateText.text = $"������: {GameManager.Instance.interestRate:P0}";
            moneyText.text = $"������: {GameManager.Instance.Money}";
            inflationText.text = $"��������: {GameManager.Instance.InflationRate:P0}";
        }
    }

    public void OnPrintMoney()
    {
        GameManager.Instance.PrintMoney(50);
    }

    public void OnTaxesUp()
    {
        GameManager.Instance.AdjustTaxes(true);
    }

    public void OnTaxesDown()
    {
        GameManager.Instance.AdjustTaxes(false);
    }

    public void OnSubsidies()
    {
        GameManager.Instance.GiveSubsidy();
    }

    public void OnInterestUp()
    {
        GameManager.Instance.AdjustInterestRates(true);
    }

    public void OnInterestDown()
    {
        GameManager.Instance.AdjustInterestRates(false);
    }
}