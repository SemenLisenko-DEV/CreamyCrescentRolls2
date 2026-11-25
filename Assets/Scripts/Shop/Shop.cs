using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

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
        public Sprite sprite = null;
        public UnityEvent onBuy;
        [HideInInspector] public bool isPurchased = false;
    }

    public List<Goods> listOfGoods = new();
    [SerializeField] private GameObject _goodsPrefab;
    [SerializeField] private Transform _shopLayout;

    [SerializeField] private GameObject _itemPrefab;
    [SerializeField] private Transform _itemsLayout;

    public void Awake()
    {
        foreach (var goods in listOfGoods)
        {
            GameObject goodsObj = Instantiate(_goodsPrefab, _shopLayout);

            TMP_Text text = goodsObj.GetComponentInChildren<TMP_Text>(true);
            if (text != null)
            {
                text.text = goods.name + '\n' + goods.cost + '$';
            }

            if (goods.sprite != null && goodsObj.TryGetComponent(out Image image))
            {
                image.sprite = goods.sprite;
            }

            if (goodsObj.TryGetComponent(out Button button))
            {
                button.onClick.AddListener(() => Buy(goods.name));
            }

            goods.onBuy.AddListener(() =>
            {
                if (goodsObj.TryGetComponent(out Button button))
                {
                    button.interactable = false;
                }
            }
            );

            goods.onBuy.AddListener(() => AddGoodsItem(goods));
        }
    }

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

    public void AddGoodsItem(Goods goods)
    {
        GameObject itemObj = Instantiate(_itemPrefab, _itemsLayout);

        if (itemObj.TryGetComponent(out Image image) && goods.sprite != null)
        {
            image.sprite = goods.sprite;
        }
        else
        {
            TMP_Text text = itemObj.GetComponentInChildren<TMP_Text>();

            if (text != null)
            {
                text.text = goods.name;
            }
        }
    }
}
