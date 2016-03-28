(function (globals) {
    "use strict";

    Bridge.define('Ecs.EntitySystem.AiSystem', {
        inherits: [Ecs.Core.System],
        constructor: function () {
            Ecs.Core.System.prototype.$constructor.call(this);
    
            this.addRequiredComponent(Ecs.EntitySystem.Intelligence);
            this.addRequiredComponent(Ecs.EntitySystem.Weapon);
            this.addRequiredComponent(Ecs.EntitySystem.Intent);
            this.addRequiredComponent(Ecs.EntitySystem.Movement);
            this.addRequiredComponent(Ecs.EntitySystem.Location);
        },
        update: function (entity, deltaTime) {
            var intelligence = entity.getComponent(Ecs.EntitySystem.Intelligence);
            var weapon = Bridge.get(Ecs.EntitySystem.EntityUtil).getWeapon(entity, this.manager);
            var intent = entity.getComponent(Ecs.EntitySystem.Intent);
            var velocity = entity.getComponent(Ecs.EntitySystem.Movement).lastMoved;
            var position = entity.getComponent(Ecs.EntitySystem.Location).position;
            var heroPosition = this.getHero().getComponent(Ecs.EntitySystem.Location).position;
    
            var distance = (Bridge.Lib.Vector2.op_Subtraction(position, heroPosition)).getLength();
            if (intelligence.followingId === -1) {
                if (distance < Bridge.get(Ecs.EntitySystem.Intelligence).SIGHT_RANGE) {
                    intelligence.followingId = this.getHero().getId();
                }
            }
            else  {
                if (intelligence.followingId === this.getHero().getId()) {
                    if (distance > Bridge.get(Ecs.EntitySystem.Intelligence).MAX_SIGHT_RANGE) {
                        intelligence.followingId = -1;
                    }
                }
            }
    
            if (distance < Bridge.get(Ecs.EntitySystem.Intelligence).SIGHT_RANGE) {
                if (weapon.currentTime > weapon.fireRate) {
                    intent.queue.add(new Ecs.EntitySystem.AttackIntent(heroPosition));
                }
            }
        },
        deleteEntity: function (id) {
            var $t;
            $t = Bridge.getEnumerator(this.getEntities());
            while ($t.moveNext()) {
                var entity = $t.getCurrent();
                var intelligence = entity.getComponent(Ecs.EntitySystem.Intelligence);
                if (intelligence.followingId === id) {
                    intelligence.followingId = -1;
                }
            }
    
            Ecs.Core.System.prototype.deleteEntity.call(this, id);
        }
    });
    
    Bridge.define('Ecs.EntitySystem.IIntent');
    
    Bridge.define('Ecs.EntitySystem.WeaponTemplate', {
        fireRate: 0,
        attack: null,
        attackTemplate: null,
        minDamage: 0,
        maxDamage: 0,
        range: 0
    });
    
    Bridge.define('Ecs.EntitySystem.ICollisionEffect');
    
    Bridge.define('Ecs.EntitySystem.EnemyTemplate', {
        bounds: null,
        size: 0,
        color: null,
        speed: 0,
        grantedXp: 0,
        health: 0,
        followingId: -1,
        constructor: function (bounds) {
            this.bounds = bounds;
        },
        createEntity: function (manager) {
            var location = Bridge.get(Ecs.EntitySystem.VectorUtil).getVectorInBounds(this.bounds);
            var enemy = manager.addAndGetEntity();
            manager.addComponentToEntity(enemy, new Ecs.EntitySystem.Location(location));
            manager.addComponentToEntity(enemy, new Ecs.EntitySystem.Shape(3, this.size, this.color));
            manager.addComponentToEntity(enemy, new Ecs.EntitySystem.Rotation());
            manager.addComponentToEntity(enemy, new Ecs.EntitySystem.Intent());
            manager.addComponentToEntity(enemy, new Ecs.EntitySystem.Steering(enemy));
            manager.addComponentToEntity(enemy, new Ecs.EntitySystem.Movement(this.speed, 10));
            manager.addComponentToEntity(enemy, new Ecs.EntitySystem.Faction("bad"));
            manager.addComponentToEntity(enemy, new Ecs.EntitySystem.Weapon(new Ecs.EntitySystem.BasicWeapon()));
            manager.addComponentToEntity(enemy, new Ecs.EntitySystem.Intelligence(this.followingId));
            manager.addComponentToEntity(enemy, new Ecs.EntitySystem.Health(this.health));
            manager.addComponentToEntity(enemy, new Ecs.EntitySystem.Death(this.grantedXp));
    
            return enemy.getId();
        }
    });
    
    Bridge.define('Ecs.EntitySystem.Camera', {
        inherits: [Ecs.Core.Component],
        viewport: null,
        constructor: function (width, height) {
            Ecs.Core.Component.prototype.$constructor.call(this);
    
            this.viewport = new Bridge.Lib.Rectangle(0, 0, width, height);
        },
        setPosition: function (position) {
            this.viewport.x = Bridge.Int.trunc(position.x) - Bridge.Int.div(this.viewport.width, 2);
            this.viewport.y = Bridge.Int.trunc(position.y) - Bridge.Int.div(this.viewport.height, 2);
        }
    });
    
    Bridge.define('Ecs.EntitySystem.CameraSystem', {
        inherits: [Ecs.Core.System],
        constructor: function () {
            Ecs.Core.System.prototype.$constructor.call(this);
    
            this.addRequiredComponent(Ecs.EntitySystem.Camera);
            this.addRequiredComponent(Ecs.EntitySystem.Location);
        },
        update: function (entity, deltaTime) {
            var camera = entity.getComponent(Ecs.EntitySystem.Camera);
            var position = entity.getComponent(Ecs.EntitySystem.Location).position;
            camera.setPosition(position);
        }
    });
    
    Bridge.define('Ecs.EntitySystem.CollisionEffect', {
        inherits: [Ecs.Core.Component],
        effects: null,
        constructor: function (effects) {
            Ecs.Core.Component.prototype.$constructor.call(this);
    
            if (effects === void 0) { effects = []; }
            this.effects = Bridge.Linq.Enumerable.from(effects).toList(Ecs.EntitySystem.ICollisionEffect);
        },
        containsEffect: function (collisionName) {
            var $t;
            $t = Bridge.getEnumerator(this.effects);
            while ($t.moveNext()) {
                var effect = $t.getCurrent();
                if (Bridge.getTypeName(effect) === collisionName) {
                    return true;
                }
            }
            return false;
        }
    });
    
    Bridge.define('Ecs.EntitySystem.CollisionSystem', {
        inherits: [Ecs.Core.System],
        partition: null,
        checkedPairs: null,
        constructor: function () {
            Ecs.Core.System.prototype.$constructor.call(this);
    
            this.addRequiredComponent(Ecs.EntitySystem.Shape);
            this.addRequiredComponent(Ecs.EntitySystem.Location);
        },
        getHeroViewport: function () {
            return this.getHero().getComponent(Ecs.EntitySystem.Camera).viewport;
        },
        updateAll: function (deltaTime) {
            var $t;
            this.checkedPairs = new Bridge.Lib.HashSet$2(String,String)();
            this.partition = new Ecs.EntitySystem.QuadTree(0, this.getHeroViewport());
            $t = Bridge.getEnumerator(this.getEntities());
            while ($t.moveNext()) {
                var entity = $t.getCurrent();
                this.partition.insert(entity);
            }
            Ecs.Core.System.prototype.updateAll.call(this, deltaTime);
        },
        update: function (entity, deltaTime) {
            var $t;
            var entityBounds = Bridge.get(Ecs.EntitySystem.CollisionUtil).getBounds(entity);
            var couldCollide = this.partition.couldCollide(entityBounds);
    
            $t = Bridge.getEnumerator(couldCollide);
            while ($t.moveNext()) {
                var other = $t.getCurrent();
                if (entity.getId() === other.getId()) {
                    continue;
                }
    
                var pair1 = this.getPairString(entity, other);
                var pair2 = this.getPairString(other, entity);
    
                if (this.checkedPairs.contains(pair1) || this.checkedPairs.contains(pair2)) {
                    continue;
                }
    
                this.checkCollsion(entity, other);
            }
        },
        checkCollsion: function (a, b) {
            var $t, $t1;
            if (!Bridge.get(Ecs.EntitySystem.CollisionUtil).couldCollide(a, b)) {
                return;
            }
    
            if (!Bridge.get(Ecs.EntitySystem.CollisionUtil).collide(a, b)) {
                return;
            }
    
            this.checkedPairs.add(this.getPairString(a, b));
    
            if (a.hasComponent$1(Ecs.EntitySystem.CollisionEffect)) {
                var effects = a.getComponent(Ecs.EntitySystem.CollisionEffect);
                $t = Bridge.getEnumerator(effects.effects);
                while ($t.moveNext()) {
                    var effect = $t.getCurrent();
                    if (effect.affectsEntity(a, b)) {
                        effect.resolveEffect(a, b, this.manager);
                    }
                }
            }
    
            if (b.hasComponent$1(Ecs.EntitySystem.CollisionEffect)) {
                var effects1 = b.getComponent(Ecs.EntitySystem.CollisionEffect);
                $t1 = Bridge.getEnumerator(effects1.effects);
                while ($t1.moveNext()) {
                    var effect1 = $t1.getCurrent();
                    if (effect1.affectsEntity(b, a)) {
                        effect1.resolveEffect(b, a, this.manager);
                    }
                }
            }
    
        },
        getPairString: function (a, b) {
            return a.getId() + "_" + b.getId();
        }
    });
    
    Bridge.define('Ecs.EntitySystem.CollisionUtil', {
        statics: {
            getBounds: function (entity) {
                var position = entity.getComponent(Ecs.EntitySystem.Location).position;
                var radius = entity.getComponent(Ecs.EntitySystem.Shape).radius;
    
                return new Bridge.Lib.Rectangle(Bridge.Int.trunc(position.x) - radius, Bridge.Int.trunc(position.y) - radius, radius * 2, radius * 2);
            },
            couldCollide: function (a, b) {
                return Bridge.get(Ecs.EntitySystem.CollisionUtil).getBounds(a).intersectsWith(Bridge.get(Ecs.EntitySystem.CollisionUtil).getBounds(b));
            },
            collide: function (mover, blocker) {
                var normal = { v : Bridge.get(Bridge.Lib.Vector2).getZero() };
                return Bridge.get(Ecs.EntitySystem.CollisionUtil).collide$1(mover, blocker, normal);
            },
            collide$1: function (mover, blocker, normal) {
                var moverShape = mover.getComponent(Ecs.EntitySystem.Shape);
                var blockerShape = blocker.getComponent(Ecs.EntitySystem.Shape);
    
                if (moverShape.getIsCircle() && blockerShape.getIsCircle()) {
                    return Bridge.get(Ecs.EntitySystem.CollisionUtil).circleCollidesWithCircle(mover, blocker, normal);
                }
                else  {
                    if (moverShape.getIsCircle()) {
                        return Bridge.get(Ecs.EntitySystem.CollisionUtil).circleCollidesWithPolygon(mover, blocker, normal);
                    }
                    else  {
                        if (blockerShape.getIsCircle()) {
                            var result = Bridge.get(Ecs.EntitySystem.CollisionUtil).circleCollidesWithPolygon(blocker, mover, normal);
                            normal.v = Bridge.Lib.Vector2.op_Multiply(normal.v, -1);
                            return result;
                        }
                        else  {
                            return Bridge.get(Ecs.EntitySystem.CollisionUtil).polygonCollidesWithPolygon(mover, blocker, normal);
                        }
                    }
                }
            },
            getNormal: function (mover, blocker) {
                var normal = { v : Bridge.get(Bridge.Lib.Vector2).getZero() };
                Bridge.get(Ecs.EntitySystem.CollisionUtil).collide$1(mover, blocker, normal);
                return normal.v;
            },
            circleCollidesWithCircle: function (mover, blocker, normal) {
                var moverPos = mover.getComponent(Ecs.EntitySystem.Location).position;
                var blockerPos = blocker.getComponent(Ecs.EntitySystem.Location).position;
                var moverRadius = mover.getComponent(Ecs.EntitySystem.Shape).radius;
                var blockerRadius = blocker.getComponent(Ecs.EntitySystem.Shape).radius;
    
                var between = Bridge.Lib.Vector2.op_Subtraction(moverPos, blockerPos);
                var distance = between.getLength();
    
                if (distance > moverRadius + blockerRadius) {
                    return false;
                }
    
                normal.v = Bridge.get(Ecs.EntitySystem.VectorUtil).rotateVector(between, Math.PI / 2);
    
                return true;
            },
            circleCollidesWithPolygon: function (circle, polygon, normal) {
                var circleShape = circle.getComponent(Ecs.EntitySystem.Shape);
                var polygonShape = polygon.getComponent(Ecs.EntitySystem.Shape);
                var circlePosition = circle.getComponent(Ecs.EntitySystem.Location).position;
                var polygonPosition = polygon.getComponent(Ecs.EntitySystem.Location).position;
    
                var angle = Bridge.get(Ecs.EntitySystem.EntityUtil).getRotation(polygon);
    
                normal.v = Bridge.get(Bridge.Lib.Vector2).getZero();
                var minDistance = Number.MAX_VALUE;
                for (var i = 0; i < polygonShape.sides; i++) {
                    var p1 = Bridge.Lib.Vector2.op_Addition(polygonShape.getVertex(i, angle), polygonPosition);
                    var p2 = Bridge.Lib.Vector2.op_Addition(polygonShape.getVertex(i + 1, angle), polygonPosition);
    
                    if (Bridge.get(Ecs.EntitySystem.CollisionUtil).lineIntersectsCircle(p1, p2, circlePosition, circleShape.radius)) {
                        var lineCenter = Bridge.get(Ecs.EntitySystem.VectorUtil).getMidpoint(p1, p2);
                        var distance = (Bridge.Lib.Vector2.op_Subtraction(circlePosition, lineCenter)).getLength();
    
                        if (distance < minDistance) {
                            minDistance = distance;
                            var dx = p2.x - p1.x;
                            var dy = p2.y - p1.y;
                            normal.v = new Bridge.Lib.Vector2(dy, -dx);
                        }
                    }
                }
    
                return Bridge.Lib.Vector2.op_Inequality(normal.v, Bridge.get(Bridge.Lib.Vector2).getZero());
            },
            polygonCollidesWithPolygon: function (mover, blocker, normal) {
                var moverShape = mover.getComponent(Ecs.EntitySystem.Shape);
                var blockerShape = blocker.getComponent(Ecs.EntitySystem.Shape);
                var moverPosition = mover.getComponent(Ecs.EntitySystem.Location).position;
                var blockerPosition = blocker.getComponent(Ecs.EntitySystem.Location).position;
                var moverAngle = Bridge.get(Ecs.EntitySystem.EntityUtil).getRotation(mover);
                var blockerAngle = Bridge.get(Ecs.EntitySystem.EntityUtil).getRotation(blocker);
    
    
                normal.v = new Bridge.Lib.Vector2(0, 0);
                var minDistance = Number.MAX_VALUE;
                for (var i = 0; i < moverShape.sides; i++) {
                    var a1 = Bridge.Lib.Vector2.op_Addition(moverShape.getVertex(i, moverAngle), moverPosition);
                    var a2 = Bridge.Lib.Vector2.op_Addition(moverShape.getVertex(i + 1, moverAngle), moverPosition);
    
                    for (var j = 0; j < blockerShape.sides; j++) {
                        var b1 = Bridge.Lib.Vector2.op_Addition(blockerShape.getVertex(j, blockerAngle), blockerPosition);
                        var b2 = Bridge.Lib.Vector2.op_Addition(blockerShape.getVertex(j + 1, blockerAngle), blockerPosition);
    
                        if (Bridge.get(Ecs.EntitySystem.CollisionUtil).linesIntersect(a1, a2, b1, b2)) {
                            var lineCenter = Bridge.get(Ecs.EntitySystem.VectorUtil).getMidpoint(b1, b2);
                            var distance = (Bridge.Lib.Vector2.op_Subtraction(moverPosition, lineCenter)).getLength();
    
                            if (distance < minDistance) {
                                minDistance = distance;
                                var dx = b2.x - b1.x;
                                var dy = b2.y - b1.y;
                                normal.v = new Bridge.Lib.Vector2(dy, -dx);
                            }
                        }
                    }
                }
    
                return Bridge.Lib.Vector2.op_Inequality(normal.v, Bridge.get(Bridge.Lib.Vector2).getZero());
            },
            linesIntersect: function (a1, a2, b1, b2) {
                var cmp = Bridge.Lib.Vector2.op_Subtraction(b1, a1);
                var r = Bridge.Lib.Vector2.op_Subtraction(a2, a1);
                var s = Bridge.Lib.Vector2.op_Subtraction(b2, b1);
    
                var cmpxr = cmp.x * r.y - cmp.y * r.x;
                var cmpxs = cmp.x * s.y - cmp.y * s.x;
                var rxs = r.x * s.y - r.y * s.x;
    
                if (Math.abs(cmpxr) < 0.01) {
                    return ((b1.x - a1.x < 0) !== (b1.x - a2.x < 0)) || ((b1.y - a1.y < 0) !== (b1.y - a2.y < 0));
    
                }
    
                if (Math.abs(rxs) < 0.01) {
                    return false;
                }
    
                var rxsr = 1.0 / rxs;
                var t = cmpxs * rxsr;
                var u = cmpxr * rxsr;
    
                return (t >= 0) && (t <= 1) && (u >= 0) && (u <= 1);
            },
            lineIntersectsCircle: function (p1, p2, center, radius) {
                var d = Bridge.Lib.Vector2.op_Subtraction(p2, p1);
                var f = Bridge.Lib.Vector2.op_Subtraction(p1, center);
    
                var a = Bridge.get(Bridge.Lib.Vector2).dot(d, d);
                var b = 2 * Bridge.get(Bridge.Lib.Vector2).dot(f, d);
                var c = Bridge.get(Bridge.Lib.Vector2).dot(f, f) - radius * radius;
    
                var discriminant = b * b - 4 * a * c;
                if (discriminant < 0) {
                    return false;
                }
    
                discriminant = Math.sqrt(discriminant);
    
                var t1 = (-b - discriminant) / (2 * a);
                var t2 = (-b + discriminant) / (2 * a);
    
                if (t1 >= 0 && t1 <= 1) {
                    return true;
                }
    
                if (t2 >= 0 && t2 <= 1) {
                    return true;
                }
    
                return false;
            }
        }
    });
    
    Bridge.define('Ecs.EntitySystem.Death', {
        inherits: [Ecs.Core.Component],
        grantedXp: 0,
        constructor: function (grantedXp) {
            Ecs.Core.Component.prototype.$constructor.call(this);
    
            this.grantedXp = grantedXp;
        }
    });
    
    Bridge.define('Ecs.EntitySystem.EnemyFactory', {
        statics: {
            generateLoneEnemy: function (bounds) {
                var enemies = new Bridge.List$1(Ecs.EntitySystem.EnemyTemplate)();
                var frequencies = new Bridge.List$1(Number)();
    
                Bridge.get(Ecs.EntitySystem.EnemyFactory).getSmallEnemies(bounds, enemies, frequencies);
    
                Bridge.get(Ecs.EntitySystem.EnemyFactory).normalize(frequencies);
    
                var random = Bridge.get(Ecs.EntitySystem.Util).rng.nextDouble();
    
                var template = enemies.getItem(0);
                for (var i = 0; i < frequencies.getCount(); i++) {
                    if (random > frequencies.getItem(i)) {
                        template = enemies.getItem(i);
                        break;
                    }
                }
    
                return template;
            },
            generateBoss: function (bounds) {
                var bosses = new Bridge.List$1(Ecs.EntitySystem.BossTemplate)();
                var frequencies = new Bridge.List$1(Number)();
    
                Bridge.get(Ecs.EntitySystem.EnemyFactory).getBosses(bounds, bosses, frequencies);
    
                Bridge.get(Ecs.EntitySystem.EnemyFactory).normalize(frequencies);
    
                var random = Bridge.get(Ecs.EntitySystem.Util).rng.nextDouble();
    
                var template = bosses.getItem(0);
                for (var i = 0; i < frequencies.getCount(); i++) {
                    if (random > frequencies.getItem(i)) {
                        template = bosses.getItem(i);
                        break;
                    }
                }
    
                return template;
            },
            getSmallEnemies: function (bounds, enemies, frequencies) {
                var enemies2 = Bridge.merge(new Bridge.List$1(Ecs.EntitySystem.EnemyTemplate)(), [
                    [new Ecs.EntitySystem.BlueEnemy(bounds)],
                    [new Ecs.EntitySystem.GreenEnemy(bounds)],
                    [new Ecs.EntitySystem.OrangeEnemy(bounds)]
                ] );
    
                var frequencies2 = Bridge.merge(new Bridge.List$1(Number)(), [
                    [Bridge.get(Ecs.EntitySystem.BlueEnemy).FREQUENCY],
                    [Bridge.get(Ecs.EntitySystem.GreenEnemy).FREQUENCY],
                    [Bridge.get(Ecs.EntitySystem.OrangeEnemy).FREQUENCY]
                ] );
    
                enemies.addRange(enemies2);
                frequencies.addRange(frequencies2);
            },
            getBosses: function (bounds, bosses, frequencies) {
                var bosses2 = Bridge.merge(new Bridge.List$1(Ecs.EntitySystem.BossTemplate)(), [
                    [new Ecs.EntitySystem.PurpleBoss(bounds)]
                ] );
    
                var frequencies2 = Bridge.merge(new Bridge.List$1(Number)(), [
                    [Bridge.get(Ecs.EntitySystem.PurpleBoss).FREQUENCY]
                ] );
    
                bosses.addRange(bosses2);
                frequencies.addRange(frequencies2);
            },
            normalize: function (frequencies) {
                var $t;
                var total = 0;
    
                $t = Bridge.getEnumerator(frequencies);
                while ($t.moveNext()) {
                    var f = $t.getCurrent();
                    total += f;
                }
    
                for (var i = 0; i < frequencies.getCount(); i++) {
                    frequencies.setItem(i, frequencies.getItem(i)/total);
                }
            }
        }
    });
    
    Bridge.define('Ecs.EntitySystem.EntityUtil', {
        statics: {
            getRotation: function (entity) {
                var rotation = 0;
                if (entity.hasComponent$1(Ecs.EntitySystem.Rotation)) {
                    rotation = entity.getComponent(Ecs.EntitySystem.Rotation).getAngle();
                }
    
                return rotation;
            },
            getWeapon: function (entity, manager) {
                if (entity.hasComponent$1(Ecs.EntitySystem.Weapon)) {
                    return entity.getComponent(Ecs.EntitySystem.Weapon);
                }
    
                var weaponId = entity.getComponent(Ecs.EntitySystem.Inventory).weaponId;
                var weaponEntity = manager.getEntityById(weaponId);
                return weaponEntity.getComponent(Ecs.EntitySystem.Weapon);
            },
            adjustCamera: function (entity) {
                if (entity.hasComponent$1(Ecs.EntitySystem.Camera)) {
                    var camera = entity.getComponent(Ecs.EntitySystem.Camera);
                    camera.setPosition(entity.getComponent(Ecs.EntitySystem.Location).position);
                }
            }
        }
    });
    
    Bridge.define('Ecs.EntitySystem.Experience', {
        inherits: [Ecs.Core.Component],
        statics: {
            XP_GROWTH_BASE: 1.5,
            INITIAL_LEVEL_XP: 200
        },
        level: 0,
        xp: 0,
        toNextLevel: 0,
        constructor: function () {
            Ecs.Core.Component.prototype.$constructor.call(this);
    
            this.xp = 0;
            this.toNextLevel = Bridge.get(Ecs.EntitySystem.Experience).INITIAL_LEVEL_XP;
            this.level = 1;
        },
        levelUp: function () {
            this.level++;
            var prevLevelXp = this.toNextLevel;
            this.toNextLevel = Bridge.Int.trunc((Bridge.get(Ecs.EntitySystem.Experience).INITIAL_LEVEL_XP + Math.pow(Bridge.get(Ecs.EntitySystem.Experience).XP_GROWTH_BASE, this.level)));
            this.xp = 0;
        }
    });
    
    Bridge.define('Ecs.EntitySystem.FacingSystem', {
        inherits: [Ecs.Core.System],
        constructor: function () {
            Ecs.Core.System.prototype.$constructor.call(this);
    
            this.addRequiredComponent(Ecs.EntitySystem.Rotation);
            this.addRequiredComponent(Ecs.EntitySystem.Movement);
        },
        update: function (entity, deltaTime) {
            var rotation = entity.getComponent(Ecs.EntitySystem.Rotation);
            var velocity = entity.getComponent(Ecs.EntitySystem.Movement).velocity;
    
            var up = new Bridge.Lib.Vector2(0, -1);
            var angle = Bridge.get(Ecs.EntitySystem.VectorUtil).angleBetween(up, velocity);
            rotation.setAngle(angle);
        }
    });
    
    Bridge.define('Ecs.EntitySystem.Faction', {
        inherits: [Ecs.Core.Component],
        group: null,
        constructor: function (group) {
            Ecs.Core.Component.prototype.$constructor.call(this);
    
            this.group = group;
        }
    });
    
    Bridge.define('Ecs.EntitySystem.GarbageSystem', {
        inherits: [Ecs.Core.System],
        constructor: function () {
            Ecs.Core.System.prototype.$constructor.call(this);
    
            this.addRequiredComponent(Ecs.EntitySystem.Lifetime);
        },
        update: function (entity, deltaTime) {
            var lifetime = entity.getComponent(Ecs.EntitySystem.Lifetime);
            lifetime.timeLeft -= deltaTime;
    
            if (lifetime.timeLeft <= 0) {
                this.manager.deleteEntity(entity.getId());
            }
        }
    });
    
    Bridge.define('Ecs.EntitySystem.Generator', {
        manager: null,
        heroId: 0,
        bounds: null,
        constructor: function (manager) {
            this.manager = manager;
        },
        setupHero: function () {
    
        },
        setupBounds: function (bounds) {
            this.bounds = bounds;
        },
        generateLevel: function () {
            this.generateHero();
            this.generateObstacles();
            this.generateEnemies();
        },
        generateHero: function () {
            var hero = this.manager.addAndGetEntity();
            this.manager.addComponentToEntity(hero, new Ecs.EntitySystem.Location(new Bridge.Lib.Vector2(100, 100)));
            this.manager.addComponentToEntity(hero, new Ecs.EntitySystem.Shape(0, 30, Bridge.get(Bridge.Lib.Color).getRed()));
            this.manager.addComponentToEntity(hero, new Ecs.EntitySystem.Intent());
            this.manager.addComponentToEntity(hero, new Ecs.EntitySystem.Movement(200));
            this.manager.addComponentToEntity(hero, new Ecs.EntitySystem.Camera(800, 600));
            this.manager.addComponentToEntity(hero, new Ecs.EntitySystem.Weapon(new Ecs.EntitySystem.BasicWand()));
            this.manager.addComponentToEntity(hero, new Ecs.EntitySystem.Faction("good"));
            this.manager.addComponentToEntity(hero, new Ecs.EntitySystem.Health(300));
            this.manager.addComponentToEntity(hero, new Ecs.EntitySystem.Stats());
            this.manager.addComponentToEntity(hero, new Ecs.EntitySystem.Experience());
    
            this.heroId = hero.getId();
        },
        generateObstacles: function () {
            var obstacle = this.manager.addAndGetEntity();
            this.manager.addComponentToEntity(obstacle, new Ecs.EntitySystem.Location(new Bridge.Lib.Vector2(300, 300)));
            this.manager.addComponentToEntity(obstacle, new Ecs.EntitySystem.Shape(5, 50, Bridge.get(Bridge.Lib.Color).getGray()));
            this.manager.addComponentToEntity(obstacle, new Ecs.EntitySystem.CollisionEffect([new Ecs.EntitySystem.BlockEffect()]));
            this.manager.addComponentToEntity(obstacle, new Ecs.EntitySystem.Rotation());
        },
        generateEnemies: function () {
            this.generateBosses(1);
            this.generateLoneEnemies(50);
        },
        generateBosses: function (amount) {
            var bossTemplate = Bridge.get(Ecs.EntitySystem.EnemyFactory).generateBoss(this.bounds);
    
            var bossId = bossTemplate.createBoss(this.manager);
        },
        generateLoneEnemies: function (amount) {
            for (var i = 0; i < amount; i++) {
                var template = Bridge.get(Ecs.EntitySystem.EnemyFactory).generateLoneEnemy(this.bounds);
                template.createEntity(this.manager);
            }
        },
        getHeroId: function () {
            return this.heroId;
        }
    });
    
    Bridge.define('Ecs.EntitySystem.Health', {
        inherits: [Ecs.Core.Component],
        maxHp: 0,
        hp: 0,
        constructor: function (maxHp) {
            Ecs.Core.Component.prototype.$constructor.call(this);
    
            this.maxHp = maxHp;
            this.hp = maxHp;
        }
    });
    
    Bridge.define('Ecs.EntitySystem.IAttack');
    
    Bridge.define('Ecs.EntitySystem.InputSystem', {
        inherits: [Ecs.Core.System],
        keysDown: null,
        mouseDown: null,
        constructor: function () {
            Ecs.Core.System.prototype.$constructor.call(this);
    
            this.keysDown = new Bridge.Lib.HashSet$1(Bridge.Lib.Key)();
            this.mouseDown = new Bridge.Lib.Vector2(-1, -1);
    
            document.onkeydown = Bridge.fn.bind(this, this.keyDown);
            document.onkeyup = Bridge.fn.bind(this, this.keyUp);
            document.onmousedown = Bridge.fn.bind(this, this.mouseDown$1);
            document.onmousemove = Bridge.fn.bind(this, this.mouseMove);
            document.onmouseup = Bridge.fn.bind(this, this.mouseUp);
    
            this.addRequiredComponent(Ecs.EntitySystem.Camera);
            this.addRequiredComponent(Ecs.EntitySystem.Intent);
        },
        getHeroViewport: function () {
            return this.getHero().getComponent(Ecs.EntitySystem.Camera).viewport;
        },
        update: function (entity, deltaTime) {
            this.updateAttack(entity);
            this.updateMovement(entity);
        },
        updateAttack: function (entity) {
            if (!this.isMouseDown()) {
                return;
            }
    
            var target = Bridge.Lib.Vector2.op_Addition(this.mouseDown, new Bridge.Lib.Vector2(this.getHeroViewport().x, this.getHeroViewport().y));
    
            var intent = entity.getComponent(Ecs.EntitySystem.Intent);
            intent.queue.add(new Ecs.EntitySystem.AttackIntent(target));
        },
        updateMovement: function (entity) {
            var intent = entity.getComponent(Ecs.EntitySystem.Intent);
    
            var movement = Bridge.get(Bridge.Lib.Vector2).getZero();
            if (this.keysDown.contains(Bridge.get(Bridge.Lib.Key).getW())) {
                movement.y--;
            }
            if (this.keysDown.contains(Bridge.get(Bridge.Lib.Key).getA())) {
                movement.x--;
            }
            if (this.keysDown.contains(Bridge.get(Bridge.Lib.Key).getS())) {
                movement.y++;
            }
            if (this.keysDown.contains(Bridge.get(Bridge.Lib.Key).getD())) {
                movement.x++;
            }
    
            if (Bridge.Lib.Vector2.op_Inequality(movement, Bridge.get(Bridge.Lib.Vector2).getZero())) {
                intent.queue.add(new Ecs.EntitySystem.MoveIntent(movement));
            }
            else  {
                intent.queue.add(new Ecs.EntitySystem.StopIntent());
            }
        },
        isMouseDown: function () {
            return this.mouseDown.x >= 0 && this.mouseDown.y >= 0;
        },
        keyDown: function (e) {
            this.keysDown.add(new Bridge.Lib.Key(e.keyCode));
        },
        keyUp: function (e) {
            this.keysDown.remove(new Bridge.Lib.Key(e.keyCode));
        },
        mouseDown$1: function (e) {
            this.mouseDown = new Bridge.Lib.Vector2(e.clientX, e.clientY);
        },
        mouseUp: function (e) {
            this.mouseDown = new Bridge.Lib.Vector2(-1, -1);
        },
        mouseMove: function (e) {
            if (this.isMouseDown()) {
                this.mouseDown = new Bridge.Lib.Vector2(e.clientX, e.clientY);
            }
        },
        getMousePosition: function (e) {
            var canvas = document.getElementById("canvas");
    
            var clientRect = canvas.getBoundingClientRect();
            return new Bridge.Lib.Vector2(Bridge.cast((e.clientX - clientRect.left), Number), Bridge.cast((e.clientY - clientRect.top), Number));
        }
    });
    
    Bridge.define('Ecs.EntitySystem.Intelligence', {
        inherits: [Ecs.Core.Component],
        statics: {
            SIGHT_RANGE: 500,
            MAX_SIGHT_RANGE: 1000
        },
        followingId: 0,
        constructor: function (followingId) {
            Ecs.Core.Component.prototype.$constructor.call(this);
    
            if (followingId === void 0) { followingId = -1; }
            this.followingId = followingId;
        }
    });
    
    Bridge.define('Ecs.EntitySystem.Intent', {
        inherits: [Ecs.Core.Component],
        queue: null,
        constructor: function () {
            Ecs.Core.Component.prototype.$constructor.call(this);
    
            this.queue = new Bridge.List$1(Ecs.EntitySystem.IIntent)();
        }
    });
    
    Bridge.define('Ecs.EntitySystem.IntentSystem', {
        inherits: [Ecs.Core.System],
        constructor: function () {
            Ecs.Core.System.prototype.$constructor.call(this);
    
            this.addRequiredComponent(Ecs.EntitySystem.Intent);
        },
        update: function (entity, deltaTime) {
            var $t;
            var intent = entity.getComponent(Ecs.EntitySystem.Intent);
    
            $t = Bridge.getEnumerator(intent.queue);
            while ($t.moveNext()) {
                var queued = $t.getCurrent();
                queued.doIntent(entity, this.manager);
            }
    
            intent.queue.clear();
        }
    });
    
    Bridge.define('Ecs.EntitySystem.Inventory', {
        inherits: [Ecs.Core.Component],
        weaponId: 0,
        constructor: function () {
            Ecs.Core.Component.prototype.$constructor.call(this);
    
            this.weaponId = -1;
        }
    });
    
    Bridge.define('Ecs.EntitySystem.Lifetime', {
        inherits: [Ecs.Core.Component],
        timeLeft: 0,
        constructor: function (timeLeft) {
            Ecs.Core.Component.prototype.$constructor.call(this);
    
            this.timeLeft = timeLeft;
        }
    });
    
    Bridge.define('Ecs.EntitySystem.Location', {
        inherits: [Ecs.Core.Component],
        position: null,
        constructor: function (position) {
            Ecs.Core.Component.prototype.$constructor.call(this);
    
            this.position = position;
        }
    });
    
    Bridge.define('Ecs.EntitySystem.Movement', {
        inherits: [Ecs.Core.Component],
        velocity: null,
        lastMoved: null,
        maxSpeed: 0,
        maxForce: 0,
        constructor: function (maxSpeed, maxForce, velocity) {
            Ecs.Core.Component.prototype.$constructor.call(this);
    
            if (maxForce === void 0) { maxForce = -1; }
            if (velocity === void 0) { velocity = null; }
            this.maxSpeed = maxSpeed;
            this.maxForce = maxForce;
            this.lastMoved = Bridge.get(Bridge.Lib.Vector2).getZero();
    
            this.velocity = (!Bridge.hasValue(velocity)) ? Bridge.get(Bridge.Lib.Vector2).getZero() : velocity;
        },
        getDoesAccelerate: function () {
            return this.maxForce >= 0;
        },
        move: function (force) {
            if (this.getDoesAccelerate()) {
                this.velocity = Bridge.Lib.Vector2.op_Addition(this.velocity, Bridge.get(Ecs.EntitySystem.VectorUtil).trunctate(force, this.maxForce));
                this.velocity = Bridge.get(Ecs.EntitySystem.VectorUtil).trunctate(this.velocity, this.maxSpeed);
            }
            else  {
                force.normalize();
                this.velocity = Bridge.Lib.Vector2.op_Multiply(force, this.maxSpeed);
            }
        },
        stop: function () {
            this.velocity = Bridge.get(Bridge.Lib.Vector2).getZero();
        }
    });
    
    Bridge.define('Ecs.EntitySystem.MovementSystem', {
        inherits: [Ecs.Core.System],
        constructor: function () {
            Ecs.Core.System.prototype.$constructor.call(this);
    
            this.addRequiredComponent(Ecs.EntitySystem.Movement);
            this.addRequiredComponent(Ecs.EntitySystem.Location);
        },
        update: function (entity, deltaTime) {
            var movement = entity.getComponent(Ecs.EntitySystem.Movement);
            var location = entity.getComponent(Ecs.EntitySystem.Location);
    
            var moved = Bridge.Lib.Vector2.op_Multiply(movement.velocity, deltaTime);
            location.position = Bridge.Lib.Vector2.op_Addition(location.position, moved);
    
            movement.lastMoved = moved;
        }
    });
    
    Bridge.define('Ecs.EntitySystem.Owner', {
        inherits: [Ecs.Core.Component],
        ownerId: 0,
        constructor: function (ownerId) {
            Ecs.Core.Component.prototype.$constructor.call(this);
    
            this.ownerId = ownerId;
        }
    });
    
    Bridge.define('Ecs.EntitySystem.QuadTree', {
        statics: {
            MAX_LEVELS: 10,
            MAX_ENTITIES: 5
        },
        level: 0,
        bounds: null,
        children: null,
        entities: null,
        constructor: function (level, bounds) {
            this.level = level;
            this.bounds = bounds;
            this.children = Bridge.Array.init(4, null);
    
            for (var i = 0; i < 4; i++) {
                this.children[i] = null;
            }
    
            this.entities = new Bridge.List$1(Ecs.Core.Entity)();
        },
        getIsSplit: function () {
            return Bridge.hasValue(this.children[0]);
        },
        insert: function (entity) {
            var $t;
            var entityBounds = Bridge.get(Ecs.EntitySystem.CollisionUtil).getBounds(entity);
            if (this.level === 0 && !entityBounds.intersectsWith(this.bounds)) {
                return;
            }
    
            if (this.getIsSplit()) {
                var index = this.getIndex(entityBounds);
    
                if (index !== -1) {
                    this.children[index].insert(entity);
                    return;
                }
            }
    
            this.entities.add(entity);
    
            if (this.level < Bridge.get(Ecs.EntitySystem.QuadTree).MAX_LEVELS && this.entities.getCount() > Bridge.get(Ecs.EntitySystem.QuadTree).MAX_ENTITIES) {
                if (!this.getIsSplit()) {
                    this.split();
                }
    
                var newEntities = new Bridge.List$1(Ecs.Core.Entity)();
    
                $t = Bridge.getEnumerator(this.entities);
                while ($t.moveNext()) {
                    var other = $t.getCurrent();
                    var otherBounds = Bridge.get(Ecs.EntitySystem.CollisionUtil).getBounds(other);
                    var index1 = this.getIndex(otherBounds);
    
                    if (index1 !== -1) {
                        this.children[index1].insert(other);
                    }
                    else  {
                        newEntities.add(other);
                    }
                }
                this.entities = newEntities;
            }
        },
        total: function () {
            var total = this.entities.getCount();
    
            if (this.getIsSplit()) {
                for (var i = 0; i < 4; i++) {
                    total += this.children[i].total();
                }
            }
            return total;
        },
        couldCollide: function (entityBounds) {
            var could = new Bridge.List$1(Ecs.Core.Entity)();
            this.couldCollide$1(could, entityBounds);
            return could;
        },
        couldCollide$1: function (could, entityBounds) {
            if (!this.bounds.intersectsWith(entityBounds)) {
                return;
            }
    
            if (this.getIsSplit()) {
                var index = this.getIndex(entityBounds);
    
                if (index !== -1) {
                    this.children[index].couldCollide$1(could, entityBounds);
                }
                else  {
                    for (var i = 0; i < 4; i++) {
                        this.children[i].couldCollide$1(could, entityBounds);
                    }
                }
            }
    
            could.addRange(this.entities);
        },
        getIndex: function (bounds) {
            var childBounds = this.getChildBounds();
    
            for (var i = 0; i < 4; i++) {
                if (childBounds[i].contains(bounds)) {
                    return i;
                }
            }
            return -1;
        },
        split: function () {
            var childBounds = this.getChildBounds();
    
            for (var i = 0; i < 4; i++) {
                this.children[i] = new Ecs.EntitySystem.QuadTree(this.level + 1, childBounds[i]);
            }
        },
        getChildBounds: function () {
            var x = this.bounds.x;
            var y = this.bounds.y;
            var width = Bridge.Int.div(this.bounds.width, 2);
            var height = Bridge.Int.div(this.bounds.height, 2);
            var centerX = Bridge.Int.div((this.bounds.getLeft() + this.bounds.getRight()), 2);
            var centerY = Bridge.Int.div((this.bounds.getTop() + this.bounds.getBottom()), 2);
    
            return [new Bridge.Lib.Rectangle(x, y, width, height), new Bridge.Lib.Rectangle(centerX, y, width, height), new Bridge.Lib.Rectangle(x, centerY, width, height), new Bridge.Lib.Rectangle(centerX, centerY, width, height)];
        }
    });
    
    Bridge.define('Ecs.EntitySystem.RenderSystem', {
        inherits: [Ecs.Core.System],
        statics: {
            WIDTH: 800,
            HEIGHT: 600,
            HUD_HEIGHT: 80,
            HUD_Y: 520
        },
        context: null,
        constructor: function () {
            Ecs.Core.System.prototype.$constructor.call(this);
    
            this.addRequiredComponent(Ecs.EntitySystem.Shape);
            this.addRequiredComponent(Ecs.EntitySystem.Location);
    
            var canvas = document.getElementById("canvas");
            this.context = canvas.getContext("2d");
        },
        getHeroViewport: function () {
            return this.getHero().getComponent(Ecs.EntitySystem.Camera).viewport;
        },
        updateAll: function (deltaTime) {
            this.context.clearRect(0, 0, Bridge.get(Ecs.EntitySystem.RenderSystem).WIDTH, Bridge.get(Ecs.EntitySystem.RenderSystem).HEIGHT);
            this.context.fillStyle = "black";
            this.context.fillRect(0, 0, Bridge.get(Ecs.EntitySystem.RenderSystem).WIDTH, Bridge.get(Ecs.EntitySystem.RenderSystem).HEIGHT);
    
            this.context.translate(-this.getHeroViewport().x, -this.getHeroViewport().y);
            Ecs.Core.System.prototype.updateAll.call(this, deltaTime);
            this.context.translate(this.getHeroViewport().x, this.getHeroViewport().y);
    
            this.renderHud();
        },
        update: function (entity, deltaTime) {
            var bounds = Bridge.get(Ecs.EntitySystem.CollisionUtil).getBounds(entity);
            if (!bounds.intersectsWith(this.getHeroViewport())) {
                return;
            }
    
            var position = entity.getComponent(Ecs.EntitySystem.Location).position;
            var shape = entity.getComponent(Ecs.EntitySystem.Shape);
            var angle = Bridge.get(Ecs.EntitySystem.EntityUtil).getRotation(entity);
    
    
            this.context.translate(position.x, position.y);
            this.context.fillStyle = shape.color.getColorName();
    
            if (shape.getIsCircle()) {
                this.renderCircle(shape);
            }
            else  {
                this.renderPolygon(shape, angle);
            }
    
            if (entity.hasComponent$1(Ecs.EntitySystem.Health)) {
                this.renderHealth$1(entity);
            }
    
            this.context.translate(-position.x, -position.y);
        },
        renderPolygon: function (shape, angle) {
            this.context.beginPath();
    
            this.moveTo(shape.getVertex(0, angle));
            for (var i = 1; i < shape.sides + 1; i++) {
                this.lineTo(shape.getVertex(i, angle));
            }
            this.context.fill();
            this.context.closePath();
        },
        renderCircle: function (shape) {
            this.context.beginPath();
    
            this.context.arc(0, 0, shape.radius, 0, Math.PI * 2);
            this.context.fill();
    
            this.context.closePath();
        },
        renderHealth$1: function (entity) {
            var health = entity.getComponent(Ecs.EntitySystem.Health);
            var ratio = 1.0 * health.hp / health.maxHp;
            var shape = entity.getComponent(Ecs.EntitySystem.Shape);
    
            var bar = new Bridge.Lib.Rectangle(-shape.radius, -(shape.radius + 5), shape.radius * 2, 5);
    
            this.drawRectangle(bar, Bridge.get(Bridge.Lib.Color).getGray());
    
            bar.width = Bridge.Int.trunc((bar.width * ratio));
            this.drawRectangle(bar, Bridge.get(Bridge.Lib.Color).getRed());
        },
        renderHealth: function () {
            var width = 150;
            var height = 20;
            var x = 20;
            var y = 530;
    
            var health = this.getHero().getComponent(Ecs.EntitySystem.Health);
    
            var healthBar = new Bridge.Lib.Rectangle(x, y, width, height);
            var healthRatio = 1.0 * health.hp / health.maxHp;
            healthBar.width = Bridge.Int.trunc((healthBar.width * healthRatio));
    
            this.context.fillStyle = "red";
            this.drawRectangle(healthBar, Bridge.get(Bridge.Lib.Color).getRed());
            healthBar.width = width;
            this.outlineRectangle(healthBar, Bridge.get(Bridge.Lib.Color).getBlack());
        },
        renderHud: function () {
            this.drawRectangle(new Bridge.Lib.Rectangle(0, Bridge.get(Ecs.EntitySystem.RenderSystem).HUD_Y, Bridge.get(Ecs.EntitySystem.RenderSystem).WIDTH, Bridge.get(Ecs.EntitySystem.RenderSystem).HUD_HEIGHT), Bridge.get(Bridge.Lib.Color).getGray());
    
            this.renderHealth();
            this.renderExperience();
            this.renderStats();
        },
        renderExperience: function () {
            var width = 150;
            var height = 20;
            var x = 20;
            var y = 570;
    
            var experience = this.getHero().getComponent(Ecs.EntitySystem.Experience);
    
            var xpBar = new Bridge.Lib.Rectangle(x, y, width, height);
            var xpRatio = 1.0 * experience.xp / experience.toNextLevel;
            xpBar.width = Bridge.Int.trunc((xpBar.width * xpRatio));
    
            this.drawRectangle(xpBar, Bridge.get(Bridge.Lib.Color).getGreen());
            xpBar.width = width;
            this.outlineRectangle(xpBar, Bridge.get(Bridge.Lib.Color).getBlack());
        },
        renderStats: function () {
            //var stats = Hero.GetComponent<Stats>();
    
            //int x = 300;
    
            //string display = $"ATT: {stats.Attack} ({stats.BaseAtt} + {stats.ModAtt})\n" +
            //                 $"DEF: {stats.Defense} ({stats.BaseDef} + {stats.ModDef})\n" +
            //                 $"VIT: {stats.Vitality} ({stats.BaseVit} + {stats.ModVit})";
    
    
            //x += 130;
            //display = $"SPD: {stats.Speed} ({stats.BaseSpd} + {stats.ModSpd})\n" +
            //          $"DEX: {stats.Dexterity} ({stats.BaseDex} + {stats.ModDex})\n" +
            //          $"WIS: {stats.Wisdom} ({stats.BaseWis} + {stats.ModWis})";
    
            //textRenderer.RenderText(display, new Vector2(x, y));
        },
        moveTo: function (vector) {
            this.context.moveTo(vector.x, vector.y);
        },
        lineTo: function (vector) {
            this.context.lineTo(vector.x, vector.y);
        },
        drawRectangle: function (rect, color) {
            this.context.fillStyle = color.getColorName();
            this.context.fillRect(rect.x, rect.y, rect.width, rect.height);
        },
        outlineRectangle: function (rect, color) {
            this.context.strokeStyle = color.getColorName();
            this.context.strokeRect(rect.x, rect.y, rect.width, rect.height);
        }
    });
    
    Bridge.define('Ecs.EntitySystem.Rotation', {
        inherits: [Ecs.Core.Component],
        angle: 0,
        baseAngle: 0,
        constructor: function () {
            Ecs.Core.Component.prototype.$constructor.call(this);
    
            this.angle = 0;
            this.baseAngle = 0;
            this.addDependency(Ecs.EntitySystem.Shape);
        },
        getAngle: function () {
            return this.angle;
        },
        setAngle: function (value) {
            this.angle = value + this.baseAngle;
        },
        onCreate: function (entity) {
            var shape = entity.getComponent(Ecs.EntitySystem.Shape);
    
            var side = Bridge.Lib.Vector2.op_Subtraction(shape.getVertex(0), shape.getVertex(1));
            var horizontal = Bridge.get(Bridge.Lib.Vector2).getUnitX();
    
            this.baseAngle = Bridge.get(Ecs.EntitySystem.VectorUtil).angleBetween(horizontal, side);
            this.baseAngle += Bridge.cast(Math.PI, Number);
            this.setAngle(0);
        }
    });
    
    Bridge.define('Ecs.EntitySystem.Shape', {
        inherits: [Ecs.Core.Component],
        sides: 0,
        radius: 0,
        color: null,
        constructor: function (sides, radius, color) {
            Ecs.Core.Component.prototype.$constructor.call(this);
    
            this.sides = sides === 0 ? 360 : sides;
            this.radius = radius;
            this.color = color;
        },
        getIsCircle: function () {
            return this.sides === 360;
        },
        getVertex: function (index, angleOffset) {
            if (angleOffset === void 0) { angleOffset = 0.0; }
            var angle = Math.PI * 2 * index / this.sides;
            angle += angleOffset;
            var x = Bridge.cast((this.radius * Math.cos(angle)), Number);
            var y = Bridge.cast((this.radius * Math.sin(angle)), Number);
            return new Bridge.Lib.Vector2(x, y);
        }
    });
    
    Bridge.define('Ecs.EntitySystem.Stats', {
        inherits: [Ecs.Core.Component],
        baseAtt: 0,
        baseDef: 0,
        baseSpd: 0,
        baseDex: 0,
        baseVit: 0,
        baseWis: 0,
        modAtt: 0,
        modDef: 0,
        modSpd: 0,
        modDex: 0,
        modVit: 0,
        modWis: 0,
        constructor: function (att, def, spd, dex, vit, wis) {
            Ecs.Core.Component.prototype.$constructor.call(this);
    
            if (att === void 0) { att = 10; }
            if (def === void 0) { def = 10; }
            if (spd === void 0) { spd = 10; }
            if (dex === void 0) { dex = 10; }
            if (vit === void 0) { vit = 10; }
            if (wis === void 0) { wis = 10; }
            this.baseAtt = att;
            this.baseDef = def;
            this.baseSpd = spd;
            this.baseDex = dex;
            this.baseVit = vit;
            this.baseWis = wis;
        },
        getAttack: function () {
            return this.baseAtt + this.modAtt;
        },
        getDefense: function () {
            return this.baseDef + this.modDef;
        },
        getSpeed: function () {
            return this.baseSpd + this.modSpd;
        },
        getDexterity: function () {
            return this.baseDex + this.modDex;
        },
        getVitality: function () {
            return this.baseVit + this.modVit;
        },
        getWisdom: function () {
            return this.baseWis + this.modWis;
        }
    });
    
    Bridge.define('Ecs.EntitySystem.StatsSystem', {
        inherits: [Ecs.Core.System],
        constructor: function () {
            Ecs.Core.System.prototype.$constructor.call(this);
    
            this.addRequiredComponent(Ecs.EntitySystem.Stats);
        },
        update: function (entity, deltaTime) {
            var stats = entity.getComponent(Ecs.EntitySystem.Stats);
    
            //Set movement speed
            var tileSize = 50;
            var baseSpeed = 200.0;
            var speedModifier = 3.7335;
    
            var speed = baseSpeed + stats.getSpeed() * speedModifier;
            entity.getComponent(Ecs.EntitySystem.Movement).maxSpeed = speed;
    
            //Set fire rate
            var baseAttackSpeed = 1.5;
            var attackSpeedModifier = 0.0867;
    
            var attackSpeed = baseAttackSpeed + stats.getDexterity() * attackSpeedModifier;
            Bridge.get(Ecs.EntitySystem.EntityUtil).getWeapon(entity, this.manager).fireRate = 1 / attackSpeed;
        }
    });
    
    Bridge.define('Ecs.EntitySystem.Steering', {
        inherits: [Ecs.Core.Component],
        statics: {
            SLOW_RADIUS: 20,
            DECISION_FREQUENCY: 0.5,
            WANDER_RADIUS: 300
        },
        owner: null,
        steer: null,
        wanderDecision: null,
        currentDecisionTime: 0,
        constructor: function (owner) {
            Ecs.Core.Component.prototype.$constructor.call(this);
    
            this.owner = owner;
            this.steer = Bridge.get(Bridge.Lib.Vector2).getZero();
            this.wanderDecision = Bridge.get(Bridge.Lib.Vector2).getZero();
            this.currentDecisionTime = Bridge.cast((Bridge.get(Ecs.EntitySystem.Util).rng.nextDouble() * Bridge.get(Ecs.EntitySystem.Steering).DECISION_FREQUENCY), Number);
        },
        getPosition: function () {
            return this.owner.getComponent(Ecs.EntitySystem.Location).position;
        },
        getVelocity: function () {
            return this.owner.getComponent(Ecs.EntitySystem.Movement).velocity;
        },
        getMaxSpeed: function () {
            return this.owner.getComponent(Ecs.EntitySystem.Movement).maxSpeed;
        },
        update: function (deltaTime, heroPosition) {
            this.currentDecisionTime += deltaTime;
    
            if (this.currentDecisionTime >= Bridge.get(Ecs.EntitySystem.Steering).DECISION_FREQUENCY) {
                this.currentDecisionTime = 0;
                this.wanderDecision = Bridge.get(Ecs.EntitySystem.VectorUtil).getVectorInRange(heroPosition, Bridge.get(Ecs.EntitySystem.Steering).WANDER_RADIUS);
            }
        },
        begin: function () {
            this.steer = Bridge.get(Bridge.Lib.Vector2).getZero();
        },
        getSteeringForce: function () {
            return this.steer;
        },
        seek: function (target, scale) {
            if (scale === void 0) { scale = 1.0; }
            this.steer = Bridge.Lib.Vector2.op_Addition(this.steer, Bridge.Lib.Vector2.op_Multiply(this.getSeek(target), scale));
        },
        wander: function (scale) {
            if (scale === void 0) { scale = 1.0; }
            this.steer = Bridge.Lib.Vector2.op_Addition(this.steer, Bridge.Lib.Vector2.op_Multiply(this.getSeek(this.wanderDecision), scale));
        },
        getSeek: function (target) {
            var desired = Bridge.Lib.Vector2.op_Subtraction(target, this.getPosition());
            var distance = desired.getLength();
            desired.normalize();
    
            if (distance <= Bridge.get(Ecs.EntitySystem.Steering).SLOW_RADIUS) {
                desired = Bridge.Lib.Vector2.op_Multiply(desired, distance / Bridge.get(Ecs.EntitySystem.Steering).SLOW_RADIUS);
            }
            desired = Bridge.Lib.Vector2.op_Multiply(desired, this.getMaxSpeed());
    
            return Bridge.Lib.Vector2.op_Subtraction(desired, this.getVelocity());
        }
    });
    
    Bridge.define('Ecs.EntitySystem.SteeringSystem', {
        inherits: [Ecs.Core.System],
        constructor: function () {
            Ecs.Core.System.prototype.$constructor.call(this);
    
            this.addRequiredComponent(Ecs.EntitySystem.Steering);
            this.addRequiredComponent(Ecs.EntitySystem.Intelligence);
            this.addRequiredComponent(Ecs.EntitySystem.Location);
            this.addRequiredComponent(Ecs.EntitySystem.Intent);
        },
        getHeroPosition: function () {
            return this.getHero().getComponent(Ecs.EntitySystem.Location).position;
        },
        update: function (entity, deltaTime) {
            var steering = entity.getComponent(Ecs.EntitySystem.Steering);
            var intelligence = entity.getComponent(Ecs.EntitySystem.Intelligence);
            var position = entity.getComponent(Ecs.EntitySystem.Location).position;
            var intent = entity.getComponent(Ecs.EntitySystem.Intent);
    
            var wanderAround;
            if (intelligence.followingId !== -1) {
                var following = this.manager.getEntityById(intelligence.followingId);
                wanderAround = following.getComponent(Ecs.EntitySystem.Location).position;
            }
            else  {
                wanderAround = position;
            }
    
            steering.update(deltaTime, wanderAround);
            steering.begin();
            steering.wander();
    
            var force = steering.getSteeringForce();
            intent.queue.add(new Ecs.EntitySystem.MoveIntent(force));
        }
    });
    
    Bridge.define('Ecs.EntitySystem.Util', {
        statics: {
            rng: null,
            config: {
                init: function () {
                    this.rng = new Bridge.Lib.Random();
                }
            },
            valueInRange: function (T, value, min, max) {
                return Bridge.compare(value, min) >= 0 && Bridge.compare(value, max) <= 0;
            }
        }
    });
    
    Bridge.define('Ecs.EntitySystem.VectorUtil', {
        statics: {
            trunctate: function (vector, max) {
                if (vector.getLength() > max) {
                    vector.normalize();
                    vector = Bridge.Lib.Vector2.op_Multiply(vector, max);
                }
                return vector;
            },
            rotateVector: function (vector, angle) {
                var newX = Bridge.cast((vector.x * Math.cos(angle) - vector.y * Math.sin(angle)), Number);
                var newY = Bridge.cast((vector.x * Math.sin(angle) + vector.y * Math.cos(angle)), Number);
    
                return new Bridge.Lib.Vector2(newX, newY);
            },
            angleBetween: function (a, b) {
                var angleA = Bridge.cast(Math.atan2(a.y, a.x), Number);
                var angleB = Bridge.cast(Math.atan2(b.y, b.x), Number);
    
                return angleB - angleA;
            },
            getMidpoint: function (p1, p2) {
                var x = (p2.x - p1.x) / 2.0;
                var y = (p2.y - p1.y) / 2.0;
                return new Bridge.Lib.Vector2(x, y);
            },
            getVectorInBounds: function (bounds) {
                var x = Bridge.cast((Bridge.get(Ecs.EntitySystem.Util).rng.nextDouble() * bounds.width + bounds.getLeft()), Number);
                var y = Bridge.cast((Bridge.get(Ecs.EntitySystem.Util).rng.nextDouble() * bounds.height + bounds.getTop()), Number);
                return new Bridge.Lib.Vector2(x, y);
            },
            getVectorInRange: function (vector, range) {
                var result = Bridge.get(Bridge.Lib.Vector2).getUnitX();
                var length = Bridge.cast((Bridge.get(Ecs.EntitySystem.Util).rng.nextDouble() * range), Number);
                result = Bridge.Lib.Vector2.op_Multiply(result, length);
    
                var angle = Bridge.cast((Bridge.get(Ecs.EntitySystem.Util).rng.nextDouble() * Math.PI * 2), Number);
                result = Bridge.get(Ecs.EntitySystem.VectorUtil).rotateVector(result, angle);
    
                return Bridge.Lib.Vector2.op_Addition(result, vector);
            }
        }
    });
    
    Bridge.define('Ecs.EntitySystem.Weapon', {
        inherits: [Ecs.Core.Component],
        attack: null,
        fireRate: 0,
        currentTime: 0,
        minDamage: 0,
        maxDamage: 0,
        constructor: function (template) {
            Ecs.Core.Component.prototype.$constructor.call(this);
    
            this.attack = template.attack;
            this.fireRate = template.fireRate;
            this.minDamage = template.minDamage;
            this.maxDamage = template.maxDamage;
    
            this.currentTime = Bridge.get(Ecs.EntitySystem.Util).rng.nextDouble() * this.fireRate;
        },
        getDamage: function () {
            return Bridge.get(Ecs.EntitySystem.Util).rng.next$1(this.minDamage, this.maxDamage);
        }
    });
    
    Bridge.define('Ecs.EntitySystem.WeaponSystem', {
        inherits: [Ecs.Core.System],
        constructor: function () {
            Ecs.Core.System.prototype.$constructor.call(this);
    
            this.addRequiredComponent(Ecs.EntitySystem.Weapon);
        },
        update: function (entity, deltaTime) {
            entity.getComponent(Ecs.EntitySystem.Weapon).currentTime += deltaTime;
        }
    });
    
    Bridge.define('Ecs.EntitySystem.AttackIntent', {
        inherits: [Ecs.EntitySystem.IIntent],
        target: null,
        constructor: function (target) {
            if (target === void 0) { target = null; }
            this.target = (!Bridge.hasValue(target)) ? Bridge.get(Bridge.Lib.Vector2).getZero() : target;
        },
        doIntent: function (entity, manager) {
            var weapon = Bridge.get(Ecs.EntitySystem.EntityUtil).getWeapon(entity, manager);
    
            if (weapon.currentTime < weapon.fireRate) {
                return;
            }
    
            weapon.currentTime = 0;
            weapon.attack.doAttack(entity, this.target, manager);
        }
    });
    
    Bridge.define('Ecs.EntitySystem.BasicWand', {
        inherits: [Ecs.EntitySystem.WeaponTemplate],
        constructor: function () {
            Ecs.EntitySystem.WeaponTemplate.prototype.$constructor.call(this);
    
            this.fireRate = 0.2;
            this.attack = new Ecs.EntitySystem.ProjectileAttack(500, 800);
            this.minDamage = 30;
            this.maxDamage = 40;
        }
    });
    
    Bridge.define('Ecs.EntitySystem.BasicWeapon', {
        inherits: [Ecs.EntitySystem.WeaponTemplate],
        constructor: function () {
            Ecs.EntitySystem.WeaponTemplate.prototype.$constructor.call(this);
    
            this.fireRate = 2.0;
            this.attack = new Ecs.EntitySystem.ProjectileAttack(500, 600);
            this.minDamage = 10;
            this.maxDamage = 15;
        }
    });
    
    Bridge.define('Ecs.EntitySystem.BlockEffect', {
        inherits: [Ecs.EntitySystem.ICollisionEffect],
        affectsEntity: function (entity, affected) {
            return affected.hasComponent$1(Ecs.EntitySystem.Movement);
        },
        resolveEffect: function (entity, affected, manager) {
            var normal = Bridge.get(Ecs.EntitySystem.CollisionUtil).getNormal(affected, entity);
            this.handleBlockingCollision(affected, entity, normal);
        },
        handleBlockingCollision: function (mover, blocker, normal) {
            normal.normalize();
            var correction = this.adjustCorrectionVector(mover, blocker, normal);
            mover.getComponent(Ecs.EntitySystem.Location).position = Bridge.Lib.Vector2.op_Addition(mover.getComponent(Ecs.EntitySystem.Location).position, correction);
            Bridge.get(Ecs.EntitySystem.EntityUtil).adjustCamera(mover);
        },
        adjustCorrectionVector: function (mover, blocker, correction) {
            var min = 0;
            var max = mover.getComponent(Ecs.EntitySystem.Movement).lastMoved.getLength() * 2;
    
            var minResolution = 1;
            while ((max - min) > minResolution) {
                var mid = (max + min) / 2;
                if (this.collideAfterCorrection(mover, blocker, Bridge.Lib.Vector2.op_Multiply(correction, mid))) {
                    min = mid;
                }
                else  {
                    max = mid;
                }
            }
            return Bridge.Lib.Vector2.op_Multiply(correction, max);
        },
        collideAfterCorrection: function (mover, blocker, correction) {
            var location = mover.getComponent(Ecs.EntitySystem.Location);
            location.position = Bridge.Lib.Vector2.op_Addition(location.position, correction);
    
            var result = Bridge.get(Ecs.EntitySystem.CollisionUtil).collide(mover, blocker);
    
            location.position = Bridge.Lib.Vector2.op_Subtraction(location.position, correction);
            return result;
        }
    });
    
    Bridge.define('Ecs.EntitySystem.BlueEnemy', {
        inherits: [Ecs.EntitySystem.EnemyTemplate],
        statics: {
            FREQUENCY: 1
        },
        constructor: function (bounds) {
            Ecs.EntitySystem.EnemyTemplate.prototype.$constructor.call(this, bounds);
    
            this.size = 20;
            this.color = Bridge.get(Bridge.Lib.Color).getBlue();
            this.speed = 250;
            this.grantedXp = 3;
            this.health = 20;
        }
    });
    
    Bridge.define('Ecs.EntitySystem.BossTemplate', {
        inherits: [Ecs.EntitySystem.EnemyTemplate],
        followerTemplate: null,
        followerAmount: 0,
        constructor: function (bounds) {
            Ecs.EntitySystem.EnemyTemplate.prototype.$constructor.call(this, bounds);
    
    
        },
        createBoss: function (manager) {
            var bossId = this.createEntity(manager);
            var bossPosition = manager.getEntityById(bossId).getComponent(Ecs.EntitySystem.Location).position;
    
            var radius = 200;
            var diameter = 400;
            var followerBounds = new Bridge.Lib.Rectangle(Bridge.Int.trunc((bossPosition.x - radius)), Bridge.Int.trunc((bossPosition.y - radius)), diameter, diameter);
    
    
            for (var i = 0; i < this.followerAmount; i++) {
                this.followerTemplate.bounds = followerBounds;
                this.followerTemplate.followingId = bossId;
                this.followerTemplate.createEntity(manager);
            }
    
    
            return bossId;
        }
    });
    
    Bridge.define('Ecs.EntitySystem.DamageEffect', {
        inherits: [Ecs.EntitySystem.ICollisionEffect],
        damage: 0,
        constructor: function (damage) {
            this.damage = damage;
        },
        affectsEntity: function (entity, affected) {
            if (!affected.hasComponent$1(Ecs.EntitySystem.Faction) || !affected.hasComponent$1(Ecs.EntitySystem.Health)) {
                return false;
            }
    
            var entityFaction = entity.getComponent(Ecs.EntitySystem.Faction).group;
            var affectedFaction = affected.getComponent(Ecs.EntitySystem.Faction).group;
    
            return entityFaction !== affectedFaction;
        },
        resolveEffect: function (entity, affected, manager) {
            var health = affected.getComponent(Ecs.EntitySystem.Health);
    
            health.hp -= this.damage;
            if (health.hp <= 0) {
                var intent = affected.getComponent(Ecs.EntitySystem.Intent);
    
                var ownerId = entity.getComponent(Ecs.EntitySystem.Owner).ownerId;
                var killer = manager.getEntityById(ownerId);
                intent.queue.add(new Ecs.EntitySystem.DeathIntent(killer));
            }
        }
    });
    
    Bridge.define('Ecs.EntitySystem.DeathIntent', {
        inherits: [Ecs.EntitySystem.IIntent],
        killer: null,
        constructor: function (killer) {
            this.killer = killer;
        },
        doIntent: function (entity, manager) {
            if (entity.hasComponent$1(Ecs.EntitySystem.Death)) {
                var death = entity.getComponent(Ecs.EntitySystem.Death);
    
                if (this.killer.hasComponent$1(Ecs.EntitySystem.Experience)) {
                    var experience = this.killer.getComponent(Ecs.EntitySystem.Experience);
                    experience.xp += death.grantedXp;
    
                    if (experience.xp >= experience.toNextLevel) {
                        experience.levelUp();
                    }
                }
            }
            manager.deleteEntity(entity.getId());
        }
    });
    
    Bridge.define('Ecs.EntitySystem.DestroyedEffect', {
        inherits: [Ecs.EntitySystem.ICollisionEffect],
        ignoreId: 0,
        constructor: function (ignoreId) {
            this.ignoreId = ignoreId;
        },
        affectsEntity: function (entity, affected) {
            if (affected.hasComponent$1(Ecs.EntitySystem.CollisionEffect)) {
                var effect = affected.getComponent(Ecs.EntitySystem.CollisionEffect);
                if (effect.containsEffect(Bridge.getTypeName(Ecs.EntitySystem.DestroyedEffect))) {
                    return false;
                }
            }
    
            return affected.getId() !== this.ignoreId;
        },
        resolveEffect: function (entity, affected, manager) {
            manager.deleteEntity(entity.getId());
        }
    });
    
    Bridge.define('Ecs.EntitySystem.GreenEnemy', {
        inherits: [Ecs.EntitySystem.EnemyTemplate],
        statics: {
            FREQUENCY: 0.3
        },
        constructor: function (bounds) {
            Ecs.EntitySystem.EnemyTemplate.prototype.$constructor.call(this, bounds);
    
            this.size = 40;
            this.color = Bridge.get(Bridge.Lib.Color).getGreen();
            this.speed = 300;
            this.grantedXp = 10;
            this.health = 100;
        }
    });
    
    Bridge.define('Ecs.EntitySystem.MoveIntent', {
        inherits: [Ecs.EntitySystem.IIntent],
        force: null,
        constructor: function (force) {
            this.force = force;
        },
        doIntent: function (entity, manager) {
            var movement = entity.getComponent(Ecs.EntitySystem.Movement);
            movement.move(this.force);
        }
    });
    
    Bridge.define('Ecs.EntitySystem.OrangeEnemy', {
        inherits: [Ecs.EntitySystem.EnemyTemplate],
        statics: {
            FREQUENCY: 0.5
        },
        constructor: function (bounds) {
            Ecs.EntitySystem.EnemyTemplate.prototype.$constructor.call(this, bounds);
    
            this.size = 20;
            this.color = Bridge.get(Bridge.Lib.Color).getOrange();
            this.speed = 300;
            this.grantedXp = 10;
            this.health = 50;
        }
    });
    
    Bridge.define('Ecs.EntitySystem.ProjectileAttack', {
        inherits: [Ecs.EntitySystem.IAttack],
        speed: 0,
        lifetime: 0,
        constructor: function (speed, range) {
            this.speed = speed;
            this.lifetime = 1.0 * range / speed;
        },
        doAttack: function (attacker, target, manager) {
            var weapon = Bridge.get(Ecs.EntitySystem.EntityUtil).getWeapon(attacker, manager);
    
            var attackerPosition = attacker.getComponent(Ecs.EntitySystem.Location).position;
            var towardsTarget = Bridge.Lib.Vector2.op_Subtraction(target, attackerPosition);
            towardsTarget.normalize();
            towardsTarget = Bridge.Lib.Vector2.op_Multiply(towardsTarget, this.speed);
    
            var projectile = manager.addAndGetEntity();
            manager.addComponentToEntity(projectile, new Ecs.EntitySystem.Location(attackerPosition));
            manager.addComponentToEntity(projectile, new Ecs.EntitySystem.Movement(this.speed, -1, towardsTarget));
            manager.addComponentToEntity(projectile, new Ecs.EntitySystem.Shape(0, 5, Bridge.get(Bridge.Lib.Color).getRed()));
            manager.addComponentToEntity(projectile, new Ecs.EntitySystem.CollisionEffect([new Ecs.EntitySystem.DamageEffect(weapon.getDamage()), new Ecs.EntitySystem.DestroyedEffect(attacker.getId())]));
            manager.addComponentToEntity(projectile, new Ecs.EntitySystem.Faction(attacker.getComponent(Ecs.EntitySystem.Faction).group));
            manager.addComponentToEntity(projectile, new Ecs.EntitySystem.Lifetime(this.lifetime));
            manager.addComponentToEntity(projectile, new Ecs.EntitySystem.Owner(attacker.getId()));
        }
    });
    
    Bridge.define('Ecs.EntitySystem.StopIntent', {
        inherits: [Ecs.EntitySystem.IIntent],
        doIntent: function (entity, manager) {
            entity.getComponent(Ecs.EntitySystem.Movement).stop();
        }
    });
    
    Bridge.define('Ecs.EntitySystem.PurpleBoss', {
        inherits: [Ecs.EntitySystem.BossTemplate],
        statics: {
            FREQUENCY: 1.0
        },
        constructor: function (bounds) {
            Ecs.EntitySystem.BossTemplate.prototype.$constructor.call(this, bounds);
    
            this.size = 100;
            this.color = Bridge.get(Bridge.Lib.Color).getPurple();
            this.speed = 100;
            this.grantedXp = 50;
            this.health = 500;
    
            this.followerTemplate = new Ecs.EntitySystem.BlueEnemy(bounds);
            this.followerAmount = 20;
        }
    });
    
    Bridge.init();
})(this);
