using UnityEngine;
using UnityEngine.EventSystems;

public class PointerAnimator : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Animator _animator;

    public void OnPointerEnter(PointerEventData eventData)
    {
        _animator.SetBool("State", true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _animator.SetBool("State", false);
    }
}
