using Bridge.Lib;
using Ecs.Core;

namespace Ecs.EntitySystem
{
    public class Camera : Component
    {
        public Rectangle Viewport;

        public Camera(int width, int height)
        {
            Viewport = new Rectangle(0, 0, width, height);
        }

        public void SetPosition(Vector2 position)
        {
            Viewport.X = (int) position.X - Viewport.Width/2;
            Viewport.Y = (int) position.Y - Viewport.Height/2;
        }
    }
}