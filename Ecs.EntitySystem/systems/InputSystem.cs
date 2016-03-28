using System.Collections.Generic;
using System.Drawing;
using Ecs.Core;
using OpenTK;
using OpenTK.Input;

namespace Ecs.EntitySystem
{
    public class InputSystem : Core.System
    {
        private HashSet<Key> keysDown;
        private Vector2 mouseDown;

        public InputSystem(GameWindow game)
        {
            keysDown = new HashSet<Key>();
            mouseDown = new Vector2(-1, -1);

            game.KeyDown += KeyDown;
            game.KeyUp += KeyUp;
            game.MouseDown += MouseDown;
            game.MouseUp += MouseUp;
            game.MouseMove += MouseMove;

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

        private void KeyDown(object sender, KeyboardKeyEventArgs e)
        {
            keysDown.Add(e.Key);
        }

        private void KeyUp(object sender, KeyboardKeyEventArgs e)
        {
            keysDown.Remove(e.Key);
        }

        private void MouseDown(object sender, MouseEventArgs e)
        {
            mouseDown = new Vector2(e.X, e.Y);
        }

        private void MouseUp(object sender, MouseEventArgs e)
        {
            mouseDown = new Vector2(-1, -1);
        }

        private void MouseMove(object sender, MouseMoveEventArgs e)
        {
            if(IsMouseDown())
                mouseDown = new Vector2(e.X, e.Y);
        }

        private Rectangle HeroViewport
        {
            get { return Hero.GetComponent<Camera>().Viewport; }
        }
    }
}