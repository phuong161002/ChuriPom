using System;
using UnityEngine;

namespace Puzzle.UI
{
    public class NeoOnScreenPosition : Position
    {
        [SerializeField] private Type start, end;

        public enum Type
        {
            Current,
            Top,
            Bottom,
            Left,
            Right,
            TopLeft,
            TopRight,
            BottomLeft,
            BottomRight,
        }

        public override Vector2 GetStart(RectTransform rect) => Get(start, rect.anchoredPosition);
        public override Vector2 GetEnd(RectTransform rect) => Get(end, rect.anchoredPosition);

        private Vector2 Get(Type type, Vector2 position)
        {
            return type switch
            {
                Type.Current => position,
                Type.Top => new Vector2(position.x, position.y + Screen.height),
                Type.Bottom => new Vector2(position.x, position.y - Screen.height),
                Type.Left => new Vector2(position.x - Screen.width, position.y),
                Type.Right => new Vector2(position.x + Screen.width, position.y),
                Type.TopLeft => new Vector2(position.x - Screen.width, position.y + Screen.height),
                Type.TopRight => new Vector2(position.x + Screen.width, position.y + Screen.height),
                Type.BottomLeft => new Vector2(position.x - Screen.width, position.y - Screen.height),
                Type.BottomRight => new Vector2(position.x + Screen.width, position.y - Screen.height),
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}