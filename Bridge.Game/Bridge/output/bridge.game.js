﻿(function (globals) {
    "use strict";

    Bridge.define('Bridge.Game.App', {
        statics: {
            bounds: null,
            FPS: 30,
            TARGET_MS: 33,
            manager: null,
            generator: null,
            gameOver: false,
            config: {
                init: function () {
                    this.bounds = new Bridge.Lib.Rectangle(0, 0, 4000, 4000);
                    Bridge.ready(this.main);
                }
            },
            main: function () {
                Bridge.get(Bridge.Game.App).initialize();
                Bridge.get(Bridge.Game.App).addSystems();
                Bridge.get(Bridge.Game.App).generateLevel();
                Bridge.get(Bridge.Game.App).subscribeEvents();
            },
            initialize: function () {
                Bridge.get(Bridge.Game.App).manager = new Ecs.Core.Manager();
                Bridge.get(Bridge.Game.App).generator = new Ecs.EntitySystem.Generator(Bridge.get(Bridge.Game.App).manager);
            },
            addSystems: function () {
                Bridge.get(Bridge.Game.App).manager.addSystem(new Ecs.EntitySystem.RenderSystem());
                Bridge.get(Bridge.Game.App).manager.addSystem(new Ecs.EntitySystem.InputSystem());
                Bridge.get(Bridge.Game.App).manager.addSystem(new Ecs.EntitySystem.IntentSystem());
                Bridge.get(Bridge.Game.App).manager.addSystem(new Ecs.EntitySystem.MovementSystem());
                Bridge.get(Bridge.Game.App).manager.addSystem(new Ecs.EntitySystem.GarbageSystem());
                Bridge.get(Bridge.Game.App).manager.addSystem(new Ecs.EntitySystem.WeaponSystem());
                Bridge.get(Bridge.Game.App).manager.addSystem(new Ecs.EntitySystem.CollisionSystem());
                Bridge.get(Bridge.Game.App).manager.addSystem(new Ecs.EntitySystem.SteeringSystem());
                Bridge.get(Bridge.Game.App).manager.addSystem(new Ecs.EntitySystem.FacingSystem());
                Bridge.get(Bridge.Game.App).manager.addSystem(new Ecs.EntitySystem.AiSystem());
                Bridge.get(Bridge.Game.App).manager.addSystem(new Ecs.EntitySystem.CameraSystem());
                Bridge.get(Bridge.Game.App).manager.addSystem(new Ecs.EntitySystem.StatsSystem());
            },
            generateLevel: function () {
                Bridge.get(Bridge.Game.App).generator.setupBounds(Bridge.get(Bridge.Game.App).bounds);
                Bridge.get(Bridge.Game.App).generator.generateLevel();
                Bridge.get(Bridge.Game.App).manager.bindHero(Bridge.get(Bridge.Game.App).generator.getHeroId());
            },
            subscribeEvents: function () {
                window.setInterval(Bridge.get(Bridge.Game.App).update, Bridge.get(Bridge.Game.App).TARGET_MS);
            },
            update: function () {
                if (!Bridge.get(Bridge.Game.App).manager.entityExists(Bridge.get(Bridge.Game.App).generator.getHeroId())) {
                    Bridge.get(Bridge.Game.App).gameOver = true;
                }
    
                if (!Bridge.get(Bridge.Game.App).gameOver) {
                    Bridge.get(Bridge.Game.App).manager.update(0.0333333351);
                }
            }
        }
    });
    
    Bridge.init();
})(this);
