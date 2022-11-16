// See https://aka.ms/new-console-template for more information
Console.WriteLine("Booting...");
var game = new Mygame(Mygame.SizeX, Mygame.SizeY, "Dodge em All");
game.Run();
Console.WriteLine("Stopping...");