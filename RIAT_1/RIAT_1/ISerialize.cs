namespace RIAT_1
{
	public interface ISerialize
	{
		byte[] Serializing<T>(T obj);
		T Deserializing<T>(byte[] bytes);
	}
}