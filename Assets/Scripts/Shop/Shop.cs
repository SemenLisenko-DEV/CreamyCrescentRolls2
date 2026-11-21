using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class Shop : MonoBehaviour
{
    private int _money;
    [SerializeField] private int _moneyPerCorrectAnswer = 20;

    [SerializeField] private TMP_Text _moneyDisplay;

    public int Money
    {
        get 
        { 
            return _money; 
        }
        set 
        { 
            _money = value;
            _moneyDisplay.text = _money.ToString();
        }
    }

    [Serializable]
    public class Goods
    {
        public string name = "";
        public int cost = 0;
        public bool isPurchased = false;
        public UnityEvent onBuy;
    }

    public List<Goods> listOfGoods = new();

    public void Buy(string goodsName)
    {
        Goods goods = listOfGoods.Find((x) => x.name == goodsName);

        if (goods == null)
        {
            Debug.LogWarning($"[Shop] Goods with name: {goodsName} arent exists");
            return;
        }

        if (goods.isPurchased)
        {
            Debug.LogWarning($"[Shop] Goods with name: {goodsName} already purchased");
            return;
        }

        if (Money < goods.cost)
        {
            Debug.LogWarning($"[Shop] Not enough money for goods with name: {goodsName}. Money: {Money}. Cost: {goods.cost}");
            return;
        }

        Money -= goods.cost;
        goods.isPurchased = true;
        goods.onBuy.Invoke();
    }

    [SerializeField] private TMP_Text _moneyAmountField;

    public void EarnMoney(DayRoute.DayStats dayStats)
    {
        Money += dayStats.correctCount * _moneyPerCorrectAnswer;
        _moneyAmountField.text = $"Денег заработано: {dayStats.correctCount * _moneyPerCorrectAnswer}";
    }
}
