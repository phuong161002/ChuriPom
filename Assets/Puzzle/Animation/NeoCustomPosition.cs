using UnityEngine;

namespace Puzzle.UI
{
    public class NeoCustomPosition : Position
    {
        [SerializeField] private Vector2 start, end;

        public override Vector2 GetStart(RectTransform rect) => start;

        public override Vector2 GetEnd(RectTransform rect) => end;
    }
}