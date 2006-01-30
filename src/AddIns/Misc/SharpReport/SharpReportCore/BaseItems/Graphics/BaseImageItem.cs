//------------------------------------------------------------------------------
// <autogenerated>
//     This code was generated by a tool.
//     Runtime Version: 1.1.4322.2032
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </autogenerated>
//------------------------------------------------------------------------------

using System;
using System.Globalization;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms;
/// <summary>
/// Handles the drawing of Images like Bitmap's and so on
/// </summary>
/// <remarks>
/// 	created by - Forstmeier Peter
/// 	created on - 04.10.2005 11:17:29
/// </remarks>

namespace SharpReportCore {
	public class BaseImageItem : SharpReportCore.BaseGraphicItem {
		
		/// <summary>
		/// Default constructor - initializes all fields to default values
		/// </summary>
		string fileName;
		Image image;
		bool scaleImageToSize;
		
		public BaseImageItem():base() {
		}
		
		private void LoadImage (string fName) {
			if (fName == "") {
				throw new ArgumentException("BaseImageItem:FileName");
			}
			try {
				this.image = null;
				this.image = Image.FromFile (fName);
				if (image == null) {
					string str = String.Format(CultureInfo.InvariantCulture,
					                           "Unable to Load {0}",fName);
					throw new ApplicationException(str);
				}
			} catch (Exception) {
				throw;
			}
			
		}
		#region overrides
		
		public override void Render(ReportPageEventArgs rpea) {
			base.Render(rpea);
			Graphics g = rpea.PrintPageEventArgs.Graphics;
			if (this.image != null) {
				if (this.scaleImageToSize) {
					g.DrawImageUnscaled(image,0,0);
					rpea.LocationAfterDraw = new PointF (this.Location.X + this.image.Width,
					                                  this.Location.Y + this.image.Height);
				} else {
					SizeF measureSize = base.MeasureReportItem (rpea,this);
					RectangleF rect =  base.DrawingRectangle (rpea,measureSize);
					g.DrawImage(image,
					            rect);
					rpea.LocationAfterDraw = new PointF (this.Location.X + rect.Width,
					                                  this.Location.Y + rect.Height);
				}
			}
		}
		
		public override void Dispose() {
			base.Dispose();
			this.image = null;
		}
		
		public override string ToString() {
			return "BaseImageItem";
		}
		#endregion
		
		
		
		#region properties
		
//		[EditorAttribute(typeof(System.Windows.Forms.Design.f.Design.FileNameEditor), typeof(System.Drawing.Design.UITypeEditor))]
		
		public virtual string FileName {
			get {
				return fileName;
			}
			set {
				fileName = value;
				this.image = null;
				LoadImage (fileName);
				base.NotifyPropertyChanged("FileName");
			}
		}
		
		/// <summary>
		/// The Image loaded from a File
		/// </summary>
		public Image Image {
			get {
				return image;
			}
		}
		
		///<summary>
		/// enlarge / Shrink the Controls Size
		/// </summary>
		public bool ScaleImageToSize {
			get {
				return scaleImageToSize;
			}
			set {
				scaleImageToSize = value;
				base.NotifyPropertyChanged("ScaleImageToSize");
			}
		}
		
		#endregion
		
		
		
	}
}
