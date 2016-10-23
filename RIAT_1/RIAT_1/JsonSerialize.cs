using Newtonsoft.Json;
using System.Text;

namespace RIAT_1
{
	public class JsonSerialize : ISerialize
	{
		public byte[] Serializing<T>(T obj)
		{
			return Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(obj));
		}

		public T Deserializing<T>(byte[] bytes)
		{
			return JsonConvert.DeserializeObject<T>(Encoding.UTF8.GetString(bytes));
		}
	}
}