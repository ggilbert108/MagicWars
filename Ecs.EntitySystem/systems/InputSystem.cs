using System.Collections.Generic;
using Bridge.Html5;
using Bridge.Lib;
using Ecs.Core;

namespace Ecs.EntitySystem
{
    public class InputSystem : Core.System
    {
        private HashSet<Key> keysDown;
        private Vector2 mouseDown;

        public InputSystem()
        {
            keysDown = new HashSet<Key>();
            mouseDown = new Vector2(-1, -1);

            Document.OnKeyDown = KeyDown;
            Document.OnKeyUp = KeyUp;
            Document.OnMouseDown = MouseDown;
            Document.OnMouseMove = MouseMove;
            Document.OnMouseUp = MouseUp;

            AddRequiredComponent<Camera>();
            AddRequiredComponent<Intent>();
        }

        protected override void Update(Entity entity, float deltaTime)
        {
            UpdateAttack(entity);
            UpdateMovement(entity);
        }

        private void UpdateAttack(Entity entity)
        {
            if (!IsMouseDown()) return;

            Vector2 target = mouseDown + new Vector2(HeroViewport.X, HeroViewport.Y);

            var intent = entity.GetComponent<Intent>();
            intent.Queue.Add(new AttackIntent(target));
        }

        private void UpdateMovement(Entity entity)
        {
            var intent = entity.GetComponent<Intent>();

            Vector2 movement = Vector2.Zero;
            if (keysDown.Contains(Key.W))
            {
                movement.Y--;
            }
            if (keysDown.Contains(Key.A)) 
            {
                movement.X--;
            }
            if (keysDown.Contains(Key.S))
            {
                movement.Y++;
            }
            if (keysDown.Contains(Key.D))
            {
                movement.X++;
            }

            if (movement != Vector2.Zero)
            {
                intent.Queue.Add(new MoveIntent(movement));
            }
            else
            {
                intent.Queue.Add(new StopIntent());
            }
        }

        private bool IsMouseDown()
        {
            return mouseDown.X >= 0 && mouseDown.Y >= 0;
        }

        private void KeyDown(KeyboardEvent e)
        {
            keysDown.Add(new Key(e.KeyCode));
        }

        private void KeyUp(KeyboardEvent e)
        {
            keysDown.Remove(new Key(e.KeyCode));
        }

        private void MouseDown(MouseEvent e)
        {
            mouseDown = new Vector2(e.ClientX, e.ClientY);
        }

        private void MouseUp(MouseEvent e)
        {
            mouseDown = new Vector2(-1, -1);
        }

        private void MouseMove(MouseEvent e)
        {
            if(IsMouseDown())
                mouseDown = new Vector2(e.ClientX, e.ClientY);
        }

        private Vector2 GetMousePosition(MouseEvent e)
        {
            var canvas = Document.GetElementById<CanvasElement>("canvas");

            var clientRect = canvas.GetBoundingClientRect();
            return new Vector2((float) (e.ClientX - clientRect.Left), (float) (e.ClientY - clientRect.Top));
        }

        private Rectangle HeroViewport
        {
            get { return Hero.GetComponent<Camera>().Viewport; }
        }
    }
}