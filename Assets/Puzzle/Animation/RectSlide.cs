using DG.Tweening;
using UnityEngine;

namespace Puzzle.UI
{
    [RequireComponent(typeof(RectTransform))]
    public class RectSlide : MonoAnimation
    {
        [SerializeField] private Position position;
        [SerializeField] private float duration;
        private Vector2 start, end;
        private RectTransform rect;

        protected override void Initialize()
        {
            base.Initialize();
            rect = GetComponent<RectTransform>();
            end = position.GetEnd(rect);
            start = position.GetStart(rect);
        }

        protected override Tween GetTweenAnimation() => rect.DOAnchorPos(end, duration);

        protected override void OnBeforeBegin()
        {
            rect.anchoredPosition = start;
        }
    }
}