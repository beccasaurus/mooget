using System;

public class MyCommand {
	public static void Main(string[] args) {
		Console.WriteLine("Dog bark:");
		new Dog { Name = "Rover" }.Bark();
	}
}
