using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class LootedItemShow : MonoBehaviour
{
    [SerializeField] 
    private float _speed = 5f;

    [SerializeField] 
    private float _fadeSpeed = 0.5f;

    [SerializeField] 
    private TextMeshProUGUI _showText;

    [SerializeField] 
    private Image _showIcon;

    private RectTransform _rectTransform;
    private CanvasGroup _canvasGroup;
    private bool _isMoving = true;

    public void SetTextAndIcon(string text, Sprite sprite)
    {
        _showText.SetText(text);
        _showIcon.sprite = sprite;
    }

    void Start()
    {
        _rectTransform = GetComponent<RectTransform>();
        _canvasGroup = GetComponent<CanvasGroup>();
    }

    void Update()
    {
        if (_isMoving)
        {
            MoveAndFade();
        }
    }

    void MoveAndFade()
    {
        _rectTransform.anchoredPosition += Vector2.up * _speed * Time.deltaTime;

        _canvasGroup.alpha -= _fadeSpeed * Time.deltaTime;

        if (_rectTransform.anchoredPosition.y > Screen.height)
        {
            _isMoving = false;
            Destroy(gameObject);
        }
    }
}
