using Prototype;

GameSystem gs = new GameSystem();
gs.Run(new NormalActor(1, new Gan()), new FlyActor(1, new Gan()), new WaterActor(1));
