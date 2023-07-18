using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Puzzle.UI
{
    [RequireComponent(typeof(Canvas), typeof(GraphicRaycaster))]
    public sealed class PopUp : MonoBehaviour
    {
        public static readonly Stack<GameObject> Stack = new Stack<GameObject>();

        [SerializeField]private MonoAnimation[] onShows, onHides;
        public string path;

        public Tween GetTweenOnShow() => onShows.Aggregate(DOTween.Sequence(),
            (sequence, monoAnimation) => sequence.Join(monoAnimation.GetTweenDelayAndAnimation()));

        public Tween GetTweenOnHide() => onHides.Aggregate(DOTween.Sequence(),
            (sequence, monoAnimation) => sequence.Join(monoAnimation.GetTweenDelayAndAnimation()));

        private void Awake()
        {
            var popUpContent = GetComponent<IPopUpContent>();
            if (popUpContent != null) 
                Hub.Add(path, popUpContent);
        }

        private void OnDestroy()
        {
            Hub.Delete(path);
        }

        public void Resolve()
        {
            var monoAnimations = gameObject.GetComponentsInChildren<MonoAnimation>(true);
            var lsShow = new List<MonoAnimation>();
            var lsHide = new List<MonoAnimation>();
            for (var i = 0; i < monoAnimations.Length; i++)
            {
                switch (monoAnimations[i].execution)
                {
                    case MonoAnimation.Type.OnShowPopUp:
                        lsShow.Add(monoAnimations[i]);
                        break;
                    case MonoAnimation.Type.OnHidePopup:
                        lsHide.Add(monoAnimations[i]);
                        break;
                    case MonoAnimation.Type.None:
                    case MonoAnimation.Type.OnStart:
                    case MonoAnimation.Type.OnEnable:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            onShows = lsShow.ToArray();
            onHides = lsHide.ToArray();
        }
    }
}