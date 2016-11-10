using System;
using System.Linq;

namespace RIAT2
{
	class Program
	{
		static void Main(string[] args)
		{
			var connection = new HttpConnection(new JsonSerialize(), "127.0.0.1", int.Parse(Console.ReadLine()));

			connection.Ping();
			Input input = connection.GetInputData();
			connection.WriteAnswer(CreateOutput(input));
		}

		private static Output CreateOutput(Input input)
		{
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
