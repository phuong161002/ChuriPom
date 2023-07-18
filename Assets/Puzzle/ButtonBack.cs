using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Puzzle.UI
{
    [RequireComponent(typeof(Button))]
    public class ButtonBack : MonoBehaviour
    {
        private void Awake()
        {
            GetComponent<Button>().onClick.AddListener(OnClick);
        }
        
        private static void OnClick()
        {
            if (PopUp.Stack.Count > 0)
            {
                var popup = PopUp.Stack.Pop();
                Hub.Hide(popup).Play();
            }
        }
    }
}