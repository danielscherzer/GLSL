namespace DMS.GLSL
{
	//TODO: Replace enum with builder interface; problem: how to store in dictionary what method to call t4?
	public interface IOnType 
	{
		void Identifier();
		void Keyword();
		void Function();
		void Variable();
		void UserKeyword();
	}
}