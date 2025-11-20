using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Managers;


namespace Quotation {
    public class Quotation : MonoBehaviour
    {
        [SerializeField]
        TMP_Text _price;
        [SerializeField]
        TMP_Text _model;
        [SerializeField]
        TMP_Text _color;
        [SerializeField]
        TMP_Text _wheels;
        [SerializeField]
        TMP_Text _interior;
        [SerializeField]
        Slider _ballon;
        [SerializeField]
        Slider loanTerm;
        [SerializeField]
        TMP_Text _repayment;

        private void Start()
        {
            _price.text = string.Format(_price.text, GameManager.Instance.SelectedCar.Price);
            _model.text = string.Format(_model.text, GameManager.Instance.SelectedCar.Name);
            _color.text = string.Format(_color.text, GameManager.Instance.SelectedCar.Color);
            _wheels.text = string.Format(_wheels.text, GameManager.Instance.SelectedCar.Wheels);
            _interior.text = string.Format(_interior.text, GameManager.Instance.SelectedCar.Interior);

            _ballon.onValueChanged.AddListener(_ => UpdateRepayment());
            loanTerm.onValueChanged.AddListener(_ => UpdateRepayment());

            UpdateRepayment();
        }
        private void UpdateRepayment()
        {
            float totalPrice = GameManager.Instance.SelectedCar.Price;
            float balloonValue = ((_ballon.value)/100) * totalPrice;
            int loanYears = Mathf.RoundToInt(loanTerm.value);

            float monthly = CalculateMonthlyRepayment(totalPrice, balloonValue, 12.5f, loanYears);
            _repayment.text = $"Monthly: R{monthly:F2}";
        }
        public static float CalculateMonthlyRepayment(float loanAmount, float balloonAmount, float annualInterestRate, int loanTermYears)
        {
            int n = loanTermYears * 12;
            float r = annualInterestRate / 12f / 100f;

            if (r == 0)
            {
                return (loanAmount - balloonAmount) / n;
            }

            float numerator = (loanAmount * r) - (balloonAmount * r / Mathf.Pow(1 + r, n));
            float denominator = 1 - Mathf.Pow(1 + r, -n);

            return numerator / denominator;
        }
    }
}