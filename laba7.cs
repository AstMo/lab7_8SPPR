using NetTopologySuite.Geometries;
using OSMLSGlobalLibrary.Map;
using OSMLSGlobalLibrary.Modules;
using System;
using System.Collections;
using System.Collections.Generic;

namespace laba7
{
    public class Laba7 : OSMLSModule
    {
        Controller controller;
        protected override void Initialize()
        {
            controller = new Controller();
            // Создание координат полигона.
            var polygonCoordinates = new Coordinate[] {
                 new Coordinate(4817367, 6144314),
                 new Coordinate(4817367, 6267072),
                 new Coordinate(4950673, 6267072),
                 new Coordinate(4950673, 6144314),
                 new Coordinate(4817367, 6144314)
            };
            // Создание стандартного полигона по ранее созданным координатам.
            Territory territory = new Territory(new LinearRing(polygonCoordinates));
            Dog dog = new Dog(controller.generateCoordinate(territory));
            Human human = new Human(controller.generateCoordinate(territory));
            Enemy enemy = new Enemy(controller.generateCoordinate(territory));
            territory.dogs.Add(dog);
            territory.human = human;
            territory.enemys.Add(enemy);
            controller.territories.Add(territory);

            MapObjects.Add(territory);
            MapObjects.Add(dog);
            MapObjects.Add(enemy);
            MapObjects.Add(human);


        }


        public override void Update(long elapsedMilliseconds)
        {

            foreach (var ter in controller.territories)
            {
                var rand = controller.random.Next(0, 20);

                if (ter.getEnemyCoordinate() == null)
                {
                    if (rand == 2 || rand == 4 || rand == 8)
                    {
                        ter.play();
                    }
                    else if (rand % 2 == 1)
                    {
                        ter.walk(controller.generateCoordinate(ter));
                        Console.WriteLine("walk");
                    }
                    else if (rand == 0)
                    {
                        ter.enemyMove();
                    }
                }
                else
                {
                    ter.findEnemy(ter.getEnemyCoordinate());
                    ter.enemyMove();
                    Console.WriteLine("enemy");
                }
            }
        }
    }


    class Territory : Polygon
    {

        public List<Dog> dogs = new List<Dog>();
        public Human human;
        public List<Enemy> enemys = new List<Enemy>();

        public Territory(LinearRing linearRing) : base(linearRing)
        {
        }

        public Coordinate getEnemyCoordinate()
        {
            foreach (var it in enemys)
            {
                if ((int)Coordinates[0].X < it.Coordinate.X && it.Coordinate.X < (int)Coordinates[2].X
                    && (int)Coordinates[0].Y < it.Coordinate.Y && it.Coordinate.Y < (int)Coordinates[2].Y)
                {
                    return it.Coordinate;
                }
            }
            return null;
        }

        public void play()
        {
            foreach (var dog in dogs)
            {
                dog.moveTo(human.Coordinate);
            }
        }

        public void walk(Coordinate coordinate)
        {
            foreach (var dog in dogs)
            {
                dog.moveTo(coordinate);
            }
        }

        public void findEnemy(Coordinate coordinate)
        {
            foreach (var dog in dogs)
            {
                dog.moveTo(coordinate);
            }
        }

        public void enemyMove()
        {
            foreach (var enemy in enemys)
            {
                bool run = false;
                double absX;
                double absY;
                foreach (var dog in dogs)
                {
                    absX = Math.Abs(enemy.X - dog.X);
                    absY = Math.Abs(enemy.Y - dog.Y);
                    if (absX < enemy.speed * 3 && absY < enemy.speed)
                    {
                        enemy.runFromDog(dog.Coordinate);
                        run = true;
                    }
                    if (!run)
                    {
                        enemy.moveTo(human.Coordinate);
                    }
                }
            }
        }
    }

    [CustomStyle(
       @"new ol.style.Style({
            image: new ol.style.Circle({
                opacity: 1.0,
                scale: 1.0,
                radius: 3,
                fill: new ol.style.Fill({
                    color: 'rgba(255, 0, 0, 1)'
                }),
                stroke: new ol.style.Stroke({
                    color: 'rgba(0, 0, 0, 0.4)',
                    width: 1
                }),
            })
        });
        ")]
    class Enemy : Point
    {
        public int speed = 1100;


        public Enemy(Coordinate coordinate) : base(coordinate)
        {
        }

        public void moveTo(Coordinate coordinates)
        {
            if (X < coordinates.X)
            {
                X += speed;

            }
            if (X > coordinates.X)
            {
                X -= speed;


            }
            if (Y < coordinates.Y)
            {
                Y += speed;

            }
            if (Y > coordinates.Y)
            {
                Y -= speed;
            }
        }

        public void runFromDog(Coordinate dogCoord)
        {
            if (X > dogCoord.X)
            {
                X += speed;

            }
            if (X < dogCoord.X)
            {
                X -= speed;
            }
            if (Y > dogCoord.Y)
            {
                Y += speed;

            }
            if (Y < dogCoord.Y)
            {
                Y -= speed;
            }
        }
    }

    [CustomStyle(
      @"new ol.style.Style({
            image: new ol.style.Circle({
                opacity: 1.0,
                scale: 1.0,
                radius: 3,
                fill: new ol.style.Fill({
                    color: 'rgba(0, 0, 255, 1)'
                }),
                stroke: new ol.style.Stroke({
                    color: 'rgba(0, 0, 0, 0.4)',
                    width: 1
                }),
            })
        });
        ")]
    class Dog : Point
    {
        public int speed = 1000;

        public Dog(Coordinate coordinate) : base(coordinate)
        {
        }

        public void moveTo(Coordinate coordinates)
        {
            if (X < coordinates.X)
            {
                X += speed;

            }
            if (X > coordinates.X)
            {
                X -= speed;


            }
            if (Y < coordinates.Y)
            {
                Y += speed;

            }
            if (Y > coordinates.Y)
            {
                Y -= speed;
            }
        }

    }

    [CustomStyle(
     @"new ol.style.Style({
            image: new ol.style.Circle({
                opacity: 1.0,
                scale: 1.0,
                radius: 3,
                fill: new ol.style.Fill({
                    color: 'rgba(0, 128, 0, 1)'
                }),
                stroke: new ol.style.Stroke({
                    color: 'rgba(0, 0, 0, 0.4)',
                    width: 1
                }),
            })
        });
        ")]
    class Human : Point
    {

        public Human(Coordinate coordinate) : base(coordinate)
        {
        }

    }

    class Controller
    {
        public List<Territory> territories = new List<Territory>();
        public Random random = new Random();

        public Coordinate generateCoordinate(Territory territory)
        {
            Coordinate[] cord = territory.Coordinates;
            var answer = new Coordinate(random.Next((int)cord[0].X, (int)cord[2].X), random.Next((int)cord[0].Y, (int)cord[2].Y));
            return answer;
        }

    }
}

