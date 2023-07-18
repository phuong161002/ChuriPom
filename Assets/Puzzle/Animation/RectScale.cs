using DG.Tweening;
using UnityEngine;

namespace Puzzle.UI
{
    [RequireComponent(typeof(RectTransform))]
    public class RectScale : MonoAnimation
    {
        private RectTransform rect;
        public Vector3 start, end;
        public float duration;
        
        protected override void Initialize()
        {
            base.Initialize();
            rect = GetComponent<RectTransform>();
        }

        protected override Tween GetTweenAnimation() => rect.DOScale(end, duration);

        protected override void OnBeforeBegin()
        {
            rect.localScale = start;
        }
    }
}