using System;
using Dogs;

public class Program {
	public static void Main(string[] args) {
		if (args.Length != 1) {
			Console.WriteLine("Usage: bark Rover");
		} else
			new Dog { Name = args[0] }.Bark();
	}
}
