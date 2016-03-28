using Ecs.Core;

namespace Ecs.EntitySystem
{
    public static class EntityUtil
    {
        public static float GetRotation(Entity entity)
        {
            float rotation = 0;
            if (entity.HasComponent<Rotation>())
            {
                rotation = entity.GetComponent<Rotation>().Angle;
            }

            return rotation;
        }

        public static Weapon GetWeapon(Entity entity, Manager manager)
        {
            if (entity.HasComponent<Weapon>())
            {
                return entity.GetComponent<Weapon>();
            }

            var weaponId = entity.GetComponent<Inventory>().WeaponId;
            Entity weaponEntity = manager.GetEntityById(weaponId);
            return weaponEntity.GetComponent<Weapon>();
        }

        public static void AdjustCamera(Entity entity)
        {
            if (entity.HasComponent<Camera>())
            {
                var camera = entity.GetComponent<Camera>();
                camera.SetPosition(entity.GetComponent<Location>().Position);
            }
        }
    }

}