using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace RIAT2
{
	public class HttpConnection
	{
		private readonly string url;
		private readonly ISerialize serialize;

		public HttpConnection(ISerialize serialize, string domain, int port = 80)
		{
			this.serialize = serialize;

			var index = domain.IndexOf('/');
			url = (index != -1) ? $"http://{domain.Insert(index, $":{port}")}" : $"http://{domain}:{port}";
		}

		public byte[] SendRequest(RequestType requestType, string methodName, byte[] requestBody)
		{
			while (true)
			{
				var webRequest = (HttpWebRequest)WebRequest.Create($"{url}/{methodName}");
				webRequest.Timeout = 1000;
				webRequest.Method = requestType.ToString();

				if (requestType == RequestType.POST)
				{
					webRequest.ContentLength = requestBody.Length;
					using (var stream = webRequest.GetRequestStream())
						stream.Write(requestBody, 0, requestBody.Length);
				}

				try
				{
					var webResponse = (HttpWebResponse)webRequest.GetResponse();
					using (var streamReader = new StreamReader(webResponse.GetResponseStream(), Encoding.UTF8))
						return Encoding.UTF8.GetBytes(streamReader.ReadToEnd());
				}
				catch (WebException e)
				{
					if (e.Status != WebExceptionStatus.Timeout &&
						e.Status != WebExceptionStatus.ReceiveFailure &&
						e.Status != WebExceptionStatus.NameResolutionFailure)
						throw;
				}
			}
		}

		public void Ping()
		{
			SendRequest(RequestType.GET, "Ping", null);
		}

		public void Create(KeyValue keyValue)
		{
			SendRequest(RequestType.POST, "Create", serialize.Serializing(keyValue));
		}

		public KeyValue Find(string key)
		{
			return serialize.Deserializing<KeyValue>(SendRequest(RequestType.GET, $"Find?key={key}", null));
		}

		public Input GetInputData()
		{
			return serialize.Deserializing<Input>(SendRequest(RequestType.GET, "GetInputData", null));
		}

		public void WriteAnswer(Output output)
		{
			SendRequest(RequestType.POST, "WriteAnswer", serialize.Serializing(output));
		}
	}
}