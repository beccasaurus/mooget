using System;

public class Dog {
	public string Name { get; set; }
	
	public void Bark() {
		Console.WriteLine("Woof! My name is {0}", Name);
	}
}
