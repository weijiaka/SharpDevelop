//------------------------------------------------------------------------------
// <autogenerated>
//     This code was generated by a tool.
//     Runtime Version: 1.1.4322.2032
//
//     Changes to this file may cause incorrect behavior and will be lost if 
//     the code is regenerated.
// </autogenerated>
//------------------------------------------------------------------------------

namespace SharpReportCore {
	using System;
	using System.ComponentModel;
	using System.Drawing;
	
	/// <summary>
	/// BaseClass for all Functions
	/// </summary>
	/// <remarks>
	/// 	created by - Forstmeier Peter
	/// 	created on - 05.09.2005 22:25:18
	/// </remarks>
	public class BaseFunction : SharpReportCore.BaseTextItem {
		string friendlyName;

		public BaseFunction():base() {
			
		}
		public BaseFunction(string friendlyName)
		{
			this.friendlyName = friendlyName;
		}
		
		#region properties
		
		public virtual string FriendlyName {
			get {
				return friendlyName;
			}
		}
		
		#endregion
	}
}
