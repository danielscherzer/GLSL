using System.Collections.Generic;
using System.ComponentModel;

namespace DMS.GLSL.Contracts
{
	internal interface IUserKeywords : INotifyPropertyChanged
	{
		IEnumerable<string> UserKeywordArray1 { get; }
		IEnumerable<string> UserKeywordArray2 { get; }
	}
}