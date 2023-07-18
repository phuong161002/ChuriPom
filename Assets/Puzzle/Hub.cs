using System.Collections.Generic;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;

namespace Puzzle.UI
{
    public class Hub : MonoBehaviour
    {
        private static Hub _instance;
        private Dictionary<string, IPopUpContent> popUpContents;
        [SerializeField] private int orderNext;
        [SerializeField] private GameObject eventSystem;

        private void Awake()
        {
            popUpContents = new Dictionary<string, IPopUpContent>();
            _instance = this;
        }

        private void OnDestroy()
        {
            _instance = null;
        }

        public static void Add(string path, IPopUpContent popUpContent)
        {
            if (_instance == null) return;
            if (_instance.popUpContents == null) return;
            if (_instance.popUpContents.ContainsKey(path)) return;
            _instance.popUpContents.Add(path, popUpContent);
        }

        public static void Delete(string path)
        {
            if (_instance == null) return;
            if (_instance.popUpContents == null) return;
            if (!_instance.popUpContents.ContainsKey(path)) return;
            _instance.popUpContents.Remove(path);
        }

        public static T Get<T>(string path) where T : MonoBehaviour, IPopUpContent
        {
            if (_instance.popUpContents.ContainsKey(path))
                return (T)_instance.popUpContents[path];
            var popUp = Instantiate(Resources.Load(path), _instance.transform).GetComponent<T>();
            var canvas = popUp.GetComponent<Canvas>();
            if (canvas != null)
            {
                canvas.overrideSorting = true;
            }
            else
            {
                popUp.AddComponent<Canvas>().overrideSorting = true;
            }
            return popUp;
        }

        public static Tween Show(GameObject gameObject)
        {
            DOTween.Complete(gameObject, true);
            var canvas = gameObject.GetComponent<Canvas>();
            canvas.sortingOrder = ++_instance.orderNext;

            var popUp = gameObject.GetComponent<PopUp>();
            
            if (!popUp)
            {
                return DOTween.Sequence().SetTarget(gameObject)
                    .AppendCallback(() =>
                    {
                        _instance.eventSystem.SetActive(false);
                        gameObject.SetActive(true);
                        _instance.eventSystem.SetActive(true);
                    });
            }

            return DOTween.Sequence().SetTarget(gameObject)
                .AppendCallback(() =>
                {
                    _instance.eventSystem.SetActive(false);
                    gameObject.SetActive(true);
                })
                .Append(popUp.GetTweenOnShow())
                .AppendCallback(() => _instance.eventSystem.SetActive(true));
        }

        public static Tween Hide(GameObject gameObject)
        {
            DOTween.Complete(gameObject, true);

            var canvas = gameObject.GetComponent<Canvas>();
            if (canvas.sortingOrder == _instance.orderNext) _instance.orderNext--;

            var popUp = gameObject.GetComponent<PopUp>();

            if (!popUp)
            {
                return DOTween.Sequence().SetTarget(gameObject)
                    .AppendCallback(() =>
                    {
                        _instance.eventSystem.SetActive(false);
                        gameObject.SetActive(false);
                        _instance.eventSystem.SetActive(true);
                    });
            }

            return DOTween.Sequence().SetTarget(gameObject)
                .AppendCallback(() => _instance.eventSystem.SetActive(false))
                .Append(popUp.GetTweenOnHide())
                .AppendCallback(() =>
                {
                    gameObject.SetActive(false);
                    _instance.eventSystem.SetActive(true);
                });
        }
    }
}