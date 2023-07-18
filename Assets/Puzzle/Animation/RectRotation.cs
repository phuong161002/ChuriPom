using DG.Tweening;
using UnityEngine;

namespace Puzzle.UI
{
    [RequireComponent(typeof(RectTransform))]
    public class RectRotation : MonoAnimation
    {
        private RectTransform rect;
        public Vector3 start, end;
        public float duration;
        public RotateMode mode;
        
        protected override void Initialize()
        {
            base.Initialize();
            rect = GetComponent<RectTransform>();
        }

        protected override Tween GetTweenAnimation() => rect.DOLocalRotate(end, duration, mode);

        protected override void OnBeforeBegin()
        {
            rect.localRotation = Quaternion.Euler(start);
        }
    }
}