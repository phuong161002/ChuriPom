using UnityEngine;

namespace Puzzle.UI
{
    public abstract class Position : MonoBehaviour
    {
        public abstract Vector2 GetStart(RectTransform rect);
        public abstract Vector2 GetEnd(RectTransform rect);
    }
}