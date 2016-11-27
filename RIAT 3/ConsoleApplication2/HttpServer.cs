using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace ConsoleApplication2
{
	public class HttpServer
	{
		private readonly ISerialize serialize;
		private readonly string url;
		private HttpListener listener { get; set; }
		private HttpListenerContext context { get; set; }
		private Input input { get; set; }

		public HttpServer(ISerialize serialize, string domain, int port)
		{
			this.serialize = serialize;
			url = $"http://{domain}:{port}/";
		}
		
		public void Listen()
		{
			listener = new HttpListener();
			listener.Prefixes.Add(url);
			listener.Start();
			
			while (listener.IsListening)
			{
				try
				{
					context = listener.GetContext();

					string methodName = context.Request.RawUrl.Split('?')[0].Substring(1);
                    //todo: вызывать методы GetType().GetMethod(methodName) каждый раз очень долго, закешируйте их при инициализации класса
                    GetType().GetMethod(methodName).Invoke(this, new object[0]);
				}
				catch (Exception)
				{
					
				}
			}
		}

		public void Ping()
		{
			context.Response.StatusCode = (int)HttpStatusCode.OK;
			context.Response.OutputStream.Dispose();
		}

		public void PostInputData()
		{
			using (StreamReader stream = new StreamReader(context.Request.InputStream))
				input = serialize.Deserializing<Input>(Encoding.UTF8.GetBytes(stream.ReadToEnd()));
			context.Response.StatusCode = (int)HttpStatusCode.OK;
			context.Response.OutputStream.Dispose();
		}

		public void GetAnswer()
		{
			byte[] outputBytes = serialize.Serializing(CreateOutput(input));
			using (Stream stream = context.Response.OutputStream)
				stream.Write(outputBytes, 0, outputBytes.Length);
		}

		public void Stop()
		{
			listener.Stop();
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