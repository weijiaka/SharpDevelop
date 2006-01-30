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
	using System.Drawing;
	using System.Drawing.Drawing2D;
	using System.ComponentModel;
	using SharpReportCore;
	
	
	/// <summary>
	/// Baseclass for all Graphical Items	
	/// </summary>
	/// <remarks>
	/// 	created by - Forstmeier Peter
	/// 	created on - 31.08.2005 13:24:59
	/// </remarks>
	public class BaseGraphicItem : BaseReportItem,IItemRenderer {
		
		private int thickness = 1;
		private DashStyle dashStyle = DashStyle.Solid;
		
		public BaseGraphicItem():base() {
		}
		
		
		protected  SizeF MeasureReportItem (SharpReportCore.ReportPageEventArgs rpea,IItemRenderer item) {
			if (item == null) {
				throw new ArgumentNullException("item","BaseGraphicItem");
			}
			
			return new SizeF (item.Size.Width,item.Size.Height);
		}
		
		
		#region property's
		/// <summary>
		/// Line Thickness of graphical Element
		/// </summary>
		
		[Browsable(true),
		 Category("Appearance"),
		 Description("Thickness of Line")]
		
		public int Thickness {
			get {
				return thickness;
			}
			set {
				thickness = value;
				base.NotifyPropertyChanged("FormatString");
			}
		}
		
		[Browsable(true),
		 Category("Appearance"),
		 Description("Linestyle")]
		public DashStyle DashStyle {
			get {
				return dashStyle;
			}
			set {
				dashStyle = value;
				base.NotifyPropertyChanged("FormatString");
			}
		}
		
		#endregion
	
		
		
	}
}
