using System;

namespace SaNi.Spriter
{

	/// <summary>
	/// An Exception which will be thrown if a Spriter specific issue happens at runtime.
	/// @author Trixt0r
	/// 
	/// </summary>
	public class SpriterException : Exception
	{
		public SpriterException(string message) : base(message)
		{
		}
	}

}