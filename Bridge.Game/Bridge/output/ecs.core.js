(function (globals) {
    "use strict";

    Bridge.define('Ecs.Core.Component', {
        dependencies: null,
        constructor: function () {
            this.dependencies = new Bridge.List$1(String)();
        },
        onCreate: function (entity) {
        },
        addDependency: function (T) {
            this.dependencies.add(Bridge.getTypeName(T));
        },
        canBeAddedToEntity: function (entity) {
            return true;
            //return dependencies.All(
            //    dependency => entity.HasComponent(dependency));
        },
        hasDependency: function (componentType) {
            return this.dependencies.contains(componentType);
        },
        getLackingDependency: function (entity) {
            var lacking = Bridge.Linq.Enumerable.from(this.dependencies).first(function (dependency) {
                return !entity.hasComponent(dependency);
            });
    
            return "The component " + Bridge.getTypeName(this) + " has a missing dependency: " + lacking;
        }
    });
    
    Bridge.define('Ecs.Core.Entity', {
        components: null,
        config: {
            properties: {
                Id: 0
            }
        },
        constructor: function (id) {
            this.setId(id);
            this.components = new Bridge.Dictionary$2(String,Ecs.Core.Component)();
        },
        addComponent: function (component) {
            this.components.set(Bridge.getTypeName(component), component);
        },
        removeComponent: function (T) {
            this.components.remove(Bridge.getTypeName(T));
        },
        getComponent: function (T) {
            return Bridge.cast(this.components.get(Bridge.getTypeName(T)), T);
        },
        hasComponent: function (componentType) {
            return this.components.containsKey(componentType);
        },
        hasComponent$1: function (T) {
            return this.components.containsKey(Bridge.getTypeName(T));
        },
        getDependentComponents: function (component) {
            var result = Bridge.Linq.Enumerable.from(this.components.getValues()).where(function (other) {
                return other.hasDependency(Bridge.getTypeName(component));
            });
    
            return result.toList(Ecs.Core.Component);
        }
    });
    
    Bridge.define('Ecs.Core.Manager', {
        entities: null,
        systems: null,
        toDelete: null,
        currentId: 0,
        constructor: function () {
            this.entities = new Bridge.Dictionary$2(Bridge.Int,Ecs.Core.Entity)();
            this.systems = new Bridge.Dictionary$2(String,Ecs.Core.System)();
            this.toDelete = new Bridge.List$1(Bridge.Int)();
        },
        getCount: function () {
            return this.entities.getCount();
        },
        addAndGetEntity: function () {
            var entity = new Ecs.Core.Entity(this.currentId++);
            this.entities.set(entity.getId(), entity);
            return entity;
        },
        deleteEntity: function (id) {
            this.toDelete.add(id);
        },
        getEntityById: function (id) {
            return this.entities.get(id);
        },
        entityExists: function (id) {
            return this.entities.containsKey(id);
        },
        addComponentToEntity: function (entity, component) {
            var $t;
            if (!component.canBeAddedToEntity(entity)) {
                throw new Bridge.ArgumentException(component.getLackingDependency(entity));
            }
    
            entity.addComponent(component);
            component.onCreate(entity);
    
            var dependingOn = entity.getDependentComponents(component);
            $t = Bridge.getEnumerator(dependingOn);
            while ($t.moveNext()) {
                var depending = $t.getCurrent();
                depending.onCreate(entity);
            }
    
            this.updateEntityRegistration(entity);
        },
        removeComponentFromEntity: function (T, entity) {
            entity.removeComponent(T);
            this.updateEntityRegistration(entity);
        },
        updateEntityRegistration: function (entity) {
            var $t;
            $t = Bridge.getEnumerator(this.systems.getValues());
            while ($t.moveNext()) {
                var system = $t.getCurrent();
                system.updateEntityRegistration(entity);
            }
        },
        flush: function () {
            var $t, $t1;
            $t = Bridge.getEnumerator(this.toDelete);
            while ($t.moveNext()) {
                var id = $t.getCurrent();
                if (!this.entityExists(id)) {
                    continue;
                }
    
                $t1 = Bridge.getEnumerator(this.systems.getValues());
                while ($t1.moveNext()) {
                    var system = $t1.getCurrent();
                    system.deleteEntity(id);
                }
    
                this.entities.remove(id);
            }
            this.toDelete.clear();
        },
        addSystem: function (system) {
            this.systems.set(Bridge.getTypeName(system), system);
            system.bindManager(this);
        },
        getSystem: function (T) {
            return Bridge.cast(this.systems.get(Bridge.getTypeName(T)), T);
        },
        update: function (deltaTime) {
            var $t;
            $t = Bridge.getEnumerator(this.systems.getValues());
            while ($t.moveNext()) {
                var system = $t.getCurrent();
                system.updateAll(deltaTime);
            }
            this.flush();
        },
        bindHero: function (heroId) {
            var $t;
            $t = Bridge.getEnumerator(this.systems.getValues());
            while ($t.moveNext()) {
                var system = $t.getCurrent();
                system.bindHero(heroId);
            }
        }
    });
    
    Bridge.define('Ecs.Core.System', {
        registeredEntityIds: null,
        requiredComponents: null,
        manager: null,
        heroId: 0,
        constructor: function () {
            this.registeredEntityIds = new Bridge.Lib.HashSet$2(Bridge.Int,Bridge.Int)();
            this.requiredComponents = new Bridge.List$1(String)();
        },
        getEntities: function () {
            var result = Bridge.Linq.Enumerable.from(this.registeredEntityIds).where(Bridge.fn.bind(this, $_.Ecs.Core.System.f1)).select(Bridge.fn.bind(this, $_.Ecs.Core.System.f2));
    
            return result.toList(Ecs.Core.Entity);
        },
        getHero: function () {
            return this.manager.getEntityById(this.heroId);
        },
        bindHero: function (heroId) {
            this.heroId = heroId;
        },
        bindManager: function (manager) {
            this.manager = manager;
        },
        updateAll: function (deltaTime) {
            var $t;
            $t = Bridge.getEnumerator(this.getEntities());
            while ($t.moveNext()) {
                var entity = $t.getCurrent();
                this.update(entity, deltaTime);
            }
        },
        addRequiredComponent: function (T) {
            this.requiredComponents.add(Bridge.getTypeName(T));
        },
        updateEntityRegistration: function (entity) {
            var matches = this.matches(entity);
            if (this.registeredEntityIds.contains(entity.getId())) {
                if (!matches) {
                    this.registeredEntityIds.remove(entity.getId());
                }
            }
            else  {
                if (matches) {
                    this.registeredEntityIds.add(entity.getId());
                }
            }
        },
        deleteEntity: function (id) {
            if (this.registeredEntityIds.contains(id)) {
                this.registeredEntityIds.remove(id);
            }
        },
        matches: function (entity) {
            var $t;
            $t = Bridge.getEnumerator(this.requiredComponents);
            while ($t.moveNext()) {
                var required = $t.getCurrent();
                if (!entity.hasComponent(required)) {
                    return false;
                }
            }
            return true;
        }
    });
    
    var $_ = {};
    
    Bridge.ns("Ecs.Core.System", $_)
    
    Bridge.apply($_.Ecs.Core.System, {
        f1: function (id) {
            return this.manager.entityExists(id);
        },
        f2: function (id) {
            return this.manager.getEntityById(id);
        }
    });
    
    Bridge.init();
})(this);
