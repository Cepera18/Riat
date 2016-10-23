using System;
using System.Linq;
using System.Text;

namespace RIAT_1 {
	public class Program {
		public static void Main(string[] args) {
			string typeSerialize = Console.ReadLine();
			ISerialize serialize;
			if (typeSerialize == "Json")
				serialize = new JsonSerialize();
			else
				serialize = new XmlSerialize();

			Input input = serialize.Deserializing<Input>(Encoding.UTF8.GetBytes(Console.ReadLine()));
			Output output = CreateOutput(input);
			Console.WriteLine(Encoding.UTF8.GetString(serialize.Serializing(output)));
		}

		private static Output CreateOutput(Input input) {
			Output output = new Output();
			output.SumResult = input.Sums.Sum() * input.K;

			output.MulResult = 1;
			foreach (var x in input.Muls)
				output.MulResult *= x;

			output.SortedInputs = new decimal[input.Sums.Length + input.Muls.Length];
			for (int i = 0; i < input.Sums.Length; i++)
				output.SortedInputs[i] = input.Sums[i];
			for (int j = 0, i = input.Sums.Length; j < input.Muls.Length; j++, i++)
				output.SortedInputs[i] = input.Muls[j];
			Array.Sort(output.SortedInputs);

			return output;
		}
	}
}
