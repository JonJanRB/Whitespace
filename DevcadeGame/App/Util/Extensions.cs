using Microsoft.Xna.Framework;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Whitespace.App.Util
{
    public static class Extensions
    {
        public static void ZoomToWorldPoint(this OrthographicCamera cam, Vector2 point, float zoomAmount, float easing, Vector2 xBounds)
        {
            RectangleF bounds = cam.BoundingRectangle;
            cam.BoundedMove((point - cam.Center) * easing, xBounds);
            cam.ZoomIn((zoomAmount - cam.Zoom) * easing);
        }

        public static void BoundedMove(this OrthographicCamera cam, Vector2 moveAmount, Vector2 xBounds)
        {
            if (cam.Position.X < xBounds.X)
            {
                moveAmount.X += xBounds.X - cam.Position.X;
            }
            else
            {
                float rightBound = cam.BoundingRectangle.Width + cam.Position.X;
                if (rightBound > xBounds.Y)
                    moveAmount.X -= rightBound - xBounds.Y;
            }

            cam.Move(moveAmount);
        }
    }
}
