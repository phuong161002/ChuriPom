using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;

namespace Puzzle.UI
{
    public abstract class MonoAnimation : MonoBehaviour
    {
        [SerializeField] private float delay;
        [field: SerializeField] public Type execution { get; private set; }
        [SerializeField] protected bool independentUpdate;
        [Tooltip("if ease == INTERNAL_Custom using curve to set ease")]
        [SerializeField] protected Ease ease;
        [SerializeField] protected AnimationCurve curve;
        private bool initialize;

        private void Awake()
        {
            if (!initialize)
            {
                Initialize();
            }
        }

        protected virtual void Initialize()
        {
            initialize = true;
        }

        public enum Type
        {
            None,
            OnShowPopUp,
            OnHidePopup,
            OnStart,
            OnEnable,
        }

        protected virtual void Start()
        {
            if (execution == Type.OnStart)
                GetTweenDelayAndAnimation().Play();
        }

        protected virtual void OnEnable()
        {
            if (execution == Type.OnEnable)
                GetTweenDelayAndAnimation().Play();
        }


        public Tween GetTweenDelayAndAnimation()
        {
            if (!initialize) Initialize();
            if (ease == Ease.INTERNAL_Custom)
                return DOTween.Sequence().AppendCallback(OnBeforeBegin).AppendInterval(delay)
                    .Append(GetTweenAnimation().SetEase(curve));
            return DOTween.Sequence().AppendCallback(OnBeforeBegin).AppendInterval(delay)
                .Append(GetTweenAnimation().SetEase(ease)).SetUpdate(independentUpdate);
        }

        protected abstract Tween GetTweenAnimation();
        protected abstract void OnBeforeBegin();
    }
}