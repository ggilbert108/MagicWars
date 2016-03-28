(function (globals) {
    "use strict";

    Bridge.define('Bridge.Lib.Color', {
        statics: {
            getBlack: function () {
                return new Bridge.Lib.Color("black");
            },
            getGray: function () {
                return new Bridge.Lib.Color("gray");
            },
            getGreen: function () {
                return new Bridge.Lib.Color("green");
            },
            getOrange: function () {
                return new Bridge.Lib.Color("orange");
            },
            getRed: function () {
                return new Bridge.Lib.Color("red");
            },
            getBlue: function () {
                return new Bridge.Lib.Color("blue");
            },
            getPurple: function () {
                return new Bridge.Lib.Color("purple");
            }
        },
        config: {
            properties: {
                ColorName: null
            }
        },
        constructor: function (name) {
            this.setColorName(name);
        }
    });
    
    Bridge.define('Bridge.Lib.IHashable');
    
    Bridge.define('Bridge.Lib.HashSet$2', function (T, F) { return {
        inherits: [Bridge.IEnumerable$1(T)],
        dictionary: null,
        constructor: function () {
            this.dictionary = new Bridge.Dictionary$2(T,Bridge.Int)();
        },
        add: function (value) {
            this.dictionary.set(value, 0);
        },
        remove: function (value) {
            this.dictionary.remove(value);
        },
        contains: function (value) {
            return this.dictionary.containsKey(value);
        },
        getEnumerator$1: function () {
            return Bridge.getEnumerator(this.dictionary.getKeys(), "$1");
        },
        getEnumerator: function () {
            return Bridge.getEnumerator(this.dictionary.getKeys(), "$1");
        }
    }; });
    
    Bridge.define('Bridge.Lib.Random', {
        next: function (max) {
            return Bridge.Int.trunc(Math.floor(this.nextDouble() * max));
        },
        next$1: function (min, max) {
            return this.next(max - min) + min;
        },
        nextDouble: function () {
            return Math.random();
        }
    });
    
    Bridge.define('Bridge.Lib.Rectangle', {
        x: 0,
        y: 0,
        width: 0,
        height: 0,
        constructor: function (x, y, width, height) {
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
        },
        getCenter: function () {
            return new Bridge.Lib.Vector2((this.getLeft() + this.getRight()) / 2.0, (this.getTop() + this.getBottom()) / 2.0);
        },
        getTop: function () {
            return this.y;
        },
        getBottom: function () {
            return this.y + this.height;
        },
        getLeft: function () {
            return this.x;
        },
        getRight: function () {
            return this.x + this.width;
        },
        intersectsWith: function (other) {
            var xOverlap = Bridge.get(Bridge.Lib.Util).valueInRange(this.getLeft(), other.getLeft(), other.getRight()) || Bridge.get(Bridge.Lib.Util).valueInRange(other.getLeft(), this.getLeft(), this.getRight());
    
            var yOverlap = Bridge.get(Bridge.Lib.Util).valueInRange(this.getTop(), other.getTop(), other.getBottom()) || Bridge.get(Bridge.Lib.Util).valueInRange(other.getTop(), this.getTop(), this.getBottom());
    
            return xOverlap && yOverlap;
        },
        contains: function (other) {
            return other.getLeft() >= this.getLeft() && other.getRight() <= this.getRight() && other.getTop() >= this.getTop() && other.getBottom() <= this.getBottom();
        }
    });
    
    Bridge.define('Bridge.Lib.Util', {
        statics: {
            valueInRange: function (value, min, max) {
                return Bridge.compare(value, min) >= 0 && Bridge.compare(value, max) <= 0;
            }
        }
    });
    
    Bridge.define('Bridge.Lib.Vector2', {
        statics: {
            getZero: function () {
                return new Bridge.Lib.Vector2(0, 0);
            },
            getUnitX: function () {
                return new Bridge.Lib.Vector2(1, 0);
            },
            getMidpoint: function (p1, p2) {
                return new Bridge.Lib.Vector2((p1.x + p2.x) / 2, (p1.y + p2.y) / 2);
            },
            dot: function (a, b) {
                return a.x * b.x + a.y * b.y;
            },
            angleBetween: function (a, b) {
                var angleA = Math.atan2(a.y, a.x);
                var angleB = Math.atan2(b.y, b.x);
    
                return angleB - angleA;
            },
            op_Addition: function (a, b) {
                return new Bridge.Lib.Vector2(a.x + b.x, a.y + b.y);
            },
            op_Subtraction: function (a, b) {
                return new Bridge.Lib.Vector2(a.x - b.x, a.y - b.y);
            },
            op_Multiply: function (v, k) {
                return new Bridge.Lib.Vector2(v.x * k, v.y * k);
            },
            op_Division: function (v, k) {
                return new Bridge.Lib.Vector2(v.x / k, v.y / k);
            },
            op_UnaryNegation: function (v) {
                return new Bridge.Lib.Vector2(-v.x, -v.y);
            },
            op_Equality: function (a, b) {
                var dx = a.x - b.x;
                var dy = a.y - b.y;
                return (Math.abs(dx) < 0.001) && (Math.abs(dy) < 0.001);
            },
            op_Inequality: function (a, b) {
                return !(Bridge.Lib.Vector2.op_Equality(a, b));
            }
        },
        x: 0,
        y: 0,
        constructor: function (x, y) {
            this.x = x;
            this.y = y;
        },
        getLength: function () {
            return Bridge.cast(Math.sqrt(this.x * this.x + this.y * this.y), Number);
        },
        normalize: function () {
            var length = this.getLength();
            this.x /= length;
            this.y /= length;
        },
        rotate: function (theta) {
            var newX = Bridge.cast((this.x * Math.cos(theta) - this.y * Math.sin(theta)), Number);
            var newY = Bridge.cast((this.x * Math.sin(theta) + this.y * Math.cos(theta)), Number);
            this.x = newX;
            this.y = newY;
        },
        setNaN: function () {
            this.x = Number.NaN;
            this.y = Number.NaN;
        },
        isNaN: function () {
            return (isNaN(this.x) && isNaN(this.y));
        },
        equals$1: function (other) {
            return this.x === other.x && this.y === other.y;
        },
        equals: function (obj) {
            if (null === obj) {
                return false;
            }
            if (this === obj) {
                return true;
            }
            if (Bridge.getType(obj) !== Bridge.getType(this)) {
                return false;
            }
            return this.equals$1(Bridge.cast(obj, Bridge.Lib.Vector2));
        },
        getHashCode: function () {
            return (Bridge.getHashCode(this.x) * 397) ^ Bridge.getHashCode(this.y);
        }
    });
    
    Bridge.define('Bridge.Lib.HashSet$1', function (T) { return {
        inherits: [Bridge.IEnumerable$1(T)],
        dictionary: null,
        constructor: function () {
            this.dictionary = new Bridge.Dictionary$2(Bridge.Int,T)();
        },
        add: function (value) {
            this.dictionary.set(value.getHash(), value);
        },
        remove: function (value) {
            this.dictionary.remove(value.getHash());
        },
        contains: function (value) {
            return this.dictionary.containsKey(value.getHash());
        },
        getEnumerator$1: function () {
            return Bridge.getEnumerator(this.dictionary.getValues(), "$1");
        },
        getEnumerator: function () {
            return Bridge.getEnumerator(this.dictionary.getValues(), "$1");
        }
    }; });
    
    Bridge.define('Bridge.Lib.Key', {
        inherits: [Bridge.Lib.IHashable],
        statics: {
            getA: function () {
                return new Bridge.Lib.Key(65);
            },
            getS: function () {
                return new Bridge.Lib.Key(83);
            },
            getD: function () {
                return new Bridge.Lib.Key(68);
            },
            getW: function () {
                return new Bridge.Lib.Key(87);
            }
        },
        keyCode: 0,
        constructor: function (keyCode) {
            this.keyCode = keyCode;
        },
        getHash: function () {
            return this.keyCode;
        }
    });
    
    Bridge.init();
})(this);
