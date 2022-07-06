using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class TestPresenter : MonoBehaviour
{
    [SerializeField] private Button TestButton;
    [SerializeField] private TMP_Text TestText;
    private int testNumber = 1;
    [Inject] private readonly ITestController TestController;

    private void Awake()
    {
        TestButton.onClick.AddListener(UpdateText);
    }

    private void UpdateText()
    {
        testNumber = TestController.IncreaseNumber(testNumber);
        TestText.text = testNumber.ToString();
    }
}
