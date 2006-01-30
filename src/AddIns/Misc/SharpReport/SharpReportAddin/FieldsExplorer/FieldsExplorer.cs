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
using System.IO;
using System.Data;
using System.Drawing;
using System.Resources;
using System.Reflection;
using System.Windows.Forms;
using System.ComponentModel;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;


using SharpReport;
using SharpReportCore;

/// <summary>
/// This Pad shows the Available Fields from a report and is used to handel sorting /grouping
/// </summary>
/// <remarks>
/// 	created by - Forstmeier Peter
/// 	created on - 12.06.2005 18:17:46
/// </remarks>
/// 


namespace SharpReportAddin {
	public class FieldsExplorer : TreeView, IPadContent {
		Panel contentPanel = new Panel();
		
		private SectionTreeNode nodeAvailableFields;
		private SectionTreeNode nodeSorting;
		private SectionTreeNode nodeGrouping;
		private TreeNode nodeFunction;
		private TreeNode nodeParams;
		
		private ReportModel reportModel;
		private bool isFilled ;
		
		#region Publics
		
		///<summary>
		/// Clear the selected Section
		/// </summary>
		public void ClearNodeSection () {
			System.Console.WriteLine("ClearNodeSection");
			if (this.SelectedNode is SectionTreeNode) {
				if (this.SelectedNode.Nodes.Count > 0) {
					this.SelectedNode.Nodes.Clear();
					NotifyReportView();
				}
			}
		}
		
		/// <summary>
		/// Remove the selected Node from Sorting or Grouping Collection
		/// </summary>
		public void ClearSelectedNode() {
			if (this.SelectedNode != null) {
				TreeNode parent = this.SelectedNode.Parent;
				this.SelectedNode.Remove();
				this.SelectedNode = parent;
				NotifyReportView();
			}
		}
		
		/// <summary>
		/// Toggle the SortDirection
		/// </summary>
		public void ToogleSortDirection () {
			if (this.SelectedNode is ColumnsTreeNode) {
				ColumnsTreeNode cn = (ColumnsTreeNode)this.SelectedNode;
				if (cn.SortDirection ==  ListSortDirection.Ascending) {
					cn.SortDirection = ListSortDirection.Descending;
					cn.ImageIndex = 5;
					cn.SelectedImageIndex = 5;
				} else {
					cn.SortDirection = ListSortDirection.Ascending;
					cn.ImageIndex = 4;
					cn.SelectedImageIndex = 4;
				}
				this.NotifyReportView();
			}
		}

		#endregion
		
		#region TreeView Events
		
		void TreeViewItemDrag (object sender,ItemDragEventArgs e) {
			if (e.Item is ColumnsTreeNode) {
				ColumnsTreeNode node = (ColumnsTreeNode)e.Item;
				this.SelectedNode = node;
				if (node != null) {
					this.DoDragDrop(node.DragDropDataObject,
					                DragDropEffects.Copy | DragDropEffects.Scroll);
				}
			}
		}
		
		
		void TreeViewDragOver (object sender,DragEventArgs e) {
			TreeNode node  = this.GetNodeAt(PointToClient(new Point (e.X,e.Y)));
			node.EnsureVisible();
			if (node.Nodes.Count > 0) {
				node.Expand();
			}
			if(e.Data.GetDataPresent("SharpReportAddin.ColumnsTreeNode", false)){
				//If we are in the AvailableFields Section we can't drop
				if (node is SectionTreeNode){
					e.Effect = DragDropEffects.Copy | DragDropEffects.Scroll;
				} else {
					e.Effect = DragDropEffects.None;
				}
			} else {
				e.Effect = DragDropEffects.None;
			}
		}
		
		
		void TreeViewDragDrop (object sender,DragEventArgs e) {
			if(e.Data.GetDataPresent("SharpReportAddin.ColumnsTreeNode", false)){
				
				Point pt = ((TreeView)sender).PointToClient(new Point(e.X, e.Y));
				SectionTreeNode node = (SectionTreeNode)((TreeView)sender).GetNodeAt(pt);

				if (node != null) {
					
					ColumnsTreeNode t = (ColumnsTreeNode)e.Data.GetData("SharpReportAddin.ColumnsTreeNode", true);
					ColumnsTreeNode dest = new ColumnsTreeNode (t.Text);

					// Useless to add a node twice
					if (!CheckForExist (node,dest)) {
						dest.SortDirection = ListSortDirection.Ascending;
						dest.ImageIndex = 4;
						dest.SelectedImageIndex = 4;
						this.SelectedNode = (TreeNode)dest;
						CheckNode (dest);
						node.Nodes.Add(dest);
						NotifyReportView();
						this.OnViewSaving(this,EventArgs.Empty);
					}
				}
			}
		}
		
		
		private void Fill () {
			this.Nodes.Clear();
			InitImageList();
			BuildNodes();
			this.FillTree();
			this.ExpandAll();
			isFilled = true;
		}
		
		private bool CheckForExist (SectionTreeNode sec,ColumnsTreeNode col) {
			if (sec.Nodes.Count > 0) {
				for (int i = 0;i < sec.Nodes.Count ;i++ ) {
					if (sec.Nodes[i].Text == col.Text) {
						return true;
					}
				}
			} else {
				return false;
			}
			return false;
		}
		
		
		
		private void TreeMouseDown(object sender, System.Windows.Forms.MouseEventArgs e){
			TreeNode node = this.GetNodeAt(PointToClient(Cursor.Position));
			if (node != null) {
				this.SelectedNode = node;
				CheckNode (node);
				if (e.Button == MouseButtons.Right) {
					if (node is AbstractFieldsNode) {
						AbstractFieldsNode abstrNode = (AbstractFieldsNode)node;
						if (abstrNode.ContextmenuAddinTreePath.Length > 0) {
							ContextMenuStrip ctMen = MenuService.CreateContextMenu (this,abstrNode.ContextmenuAddinTreePath);
							ctMen.Show (this,new Point (e.X,e.Y));
						}
					}
				}
			}
		}
		

		private void CheckNode (TreeNode node) {
			if (node.Parent == nodeSorting) {
				ColumnsTreeNode cn = (ColumnsTreeNode)node;

				if (cn.SortDirection ==  ListSortDirection.Ascending) {
					cn.ImageIndex = 4;
				} else {
					cn.ImageIndex = 5;
				}
			} else if (node.Parent == this.nodeGrouping) {
				ColumnsTreeNode cn = (ColumnsTreeNode)node;
				cn.ImageIndex = 2;
				cn.SelectedImageIndex = 2;
			}
		}
		
		#endregion
		
		private void NotifyReportView() {
			if (this.isFilled) {
				if (WorkbenchSingleton.Workbench.ActiveWorkbenchWindow.ViewContent is SharpReportView) {
					WorkbenchSingleton.Workbench.ActiveWorkbenchWindow.ViewContent.IsDirty = true;
				}
			}
		}
		
		#region PadEvents
		private void OnWindowChange (object sender,EventArgs e) {
			try {
				if (WorkbenchSingleton.Workbench.ActiveWorkbenchWindow == null || WorkbenchSingleton.Workbench.ActiveWorkbenchWindow.ViewContent == null) {
					return;
				}
				WorkbenchSingleton.Workbench.ActiveWorkbenchWindow.ViewContent.Saving -= OnViewSaving;
				WorkbenchSingleton.Workbench.ActiveWorkbenchWindow.ViewContent.Saving += OnViewSaving;
				
				PadDescriptor pad = WorkbenchSingleton.Workbench.GetPad(typeof(FieldsExplorer));
				
				SharpReportView v =
					WorkbenchSingleton.Workbench.ActiveWorkbenchWindow.ActiveViewContent
					as SharpReportView;
				
				if (v != null) {
					this.reportModel = v.ReportManager.BaseDesignControl.ReportModel;
					if (this.reportModel != null) {
						this.Fill();
						WorkbenchSingleton.Workbench.ShowPad(pad);
						pad.BringPadToFront();
					}
					
				} else {
					WorkbenchSingleton.Workbench.WorkbenchLayout.HidePad(pad);
				}
				
			} catch (Exception er) {
				System.Console.WriteLine("\t{0}",er.Message);
			}
		}
		
		private void OnViewSaving (object sender, EventArgs e) {
			if (this.isFilled) {
				UpdateSorting();
				UpdateGrouping();
			}
		}
		
		#endregion
		
		
		#region Build TreeControl
		
		private void UpdateSorting () {
			this.reportModel.ReportSettings.SortColumnCollection.Clear();
			if (this.nodeSorting.Nodes.Count > 0) {
				SortColumn sc;
				AbstractColumn af;
				for (int i = 0;i < this.nodeSorting.Nodes.Count ;i++ ) {
					ColumnsTreeNode cn = (ColumnsTreeNode)this.nodeSorting.Nodes[i];
					af = this.reportModel.ReportSettings.AvailableFieldsCollection.Find(cn.Text);
					if (af != null) {
						sc = new SortColumn (cn.Text,
						                     cn.SortDirection,
						                     af.DataType);
					} else {
						sc = new SortColumn (cn.Text,
						                     cn.SortDirection,
						                     typeof(System.String));
					}
					this.reportModel.ReportSettings.SortColumnCollection.Add(sc);
				}
			}
		}
		
		
		private void UpdateGrouping () {
			this.reportModel.ReportSettings.GroupColumnsCollection.Clear();
			if (this.nodeGrouping.Nodes.Count > 0) {
				GroupColumn gc;
				for (int i = 0;i < this.nodeGrouping.Nodes.Count ;i++ ) {
					ColumnsTreeNode cn = (ColumnsTreeNode)this.nodeGrouping.Nodes[i];
					gc = new GroupColumn (cn.Text,i,cn.SortDirection);
					this.reportModel.ReportSettings.GroupColumnsCollection.Add(gc);
				}
			}
		}
		
		void SetAvailableFields () {
			try {
				int avCount = this.reportModel.ReportSettings.AvailableFieldsCollection.Count;
				for (int i = 0;i < avCount ;i++ ) {
					AbstractColumn af = this.reportModel.ReportSettings.AvailableFieldsCollection[i];
					ColumnsTreeNode n = new ColumnsTreeNode(af.ColumnName);
					n.Tag = this.nodeAvailableFields;
					
					//we don't like ContextMenu here
					n.ContextmenuAddinTreePath = "";
					switch (this.reportModel.ReportSettings.CommandType) {
							case CommandType.Text:{
								n.ImageIndex = 6;
								n.SelectedImageIndex = 6;
								break;
							}
							case CommandType.StoredProcedure: {
								n.ImageIndex = 7;
								n.SelectedImageIndex = 7;
								break;
							}
							default:{
								n.ImageIndex = 6;
								n.SelectedImageIndex = 6;
								break;
							}
					}
					this.nodeAvailableFields.Nodes.Add(n);
				}
			} catch (Exception) {
				throw;
			}
		}
			
		
		
		void SetSortFields(){
			try {
				ColumnsTreeNode node;
				int scCount = this.reportModel.ReportSettings.SortColumnCollection.Count;
				foreach (SortColumn sc in this.reportModel.ReportSettings.SortColumnCollection) {
					node = new ColumnsTreeNode(sc.ColumnName,sc.SortDirection);
					if (node.SortDirection == ListSortDirection.Ascending) {
						node.ImageIndex = 4;
						node.SelectedImageIndex = 4;
					} else {
						node.ImageIndex = 5;
						node.SelectedImageIndex = 5;
					}
					this.nodeSorting.Nodes.Add(node);
				}
			} catch (Exception) {
				
			}
		}
		void SetGroupFields(){
			try {
				ColumnsTreeNode node;
				int gcCount = this.reportModel.ReportSettings.GroupColumnsCollection.Count;
				for (int i = 0;i < gcCount ;i++ ) {
					GroupColumn gc = (GroupColumn)this.reportModel.ReportSettings.GroupColumnsCollection[i];
					node = new ColumnsTreeNode(gc.ColumnName);
					if (node.SortDirection == ListSortDirection.Ascending) {
						node.ImageIndex = 4;
						node.SelectedImageIndex = 4;
					} else {
						node.ImageIndex = 5;
						node.SelectedImageIndex = 5;
					}
					this.nodeGrouping.Nodes.Add(node);
				}
			} catch (Exception) {
				
			}
		}
		
		void SetParamFields (){
			
			ColumnsTreeNode node;
			int parCount = this.reportModel.ReportSettings.SqlParametersCollection.Count;
			if (parCount > 0) {
				for (int i = 0;i < parCount ;i++ ) {
					SqlParameter par = (SqlParameter)this.reportModel.ReportSettings.SqlParametersCollection[i];
					node = new ColumnsTreeNode(par.ParameterName);
					node.Tag = par;
					node.SelectedImageIndex = 9;
					node.ImageIndex = 9;
					this.nodeParams.Nodes.Add (node);
				}
			} 
		}
		
		void SetFunctions(){
			ColumnsTreeNode node;
			foreach (ReportSection section in this.reportModel.SectionCollection) {
				foreach (BaseReportObject item in section.Items) {
					BaseFunction func = item as BaseFunction;
					if (func != null) {
						node = new ColumnsTreeNode(func.Name);
						this.nodeFunction.Nodes.Add(func.FriendlyName);
					}				
				}
			}
		}
		
		private void FillTree () {
			this.BeginUpdate();
			SetAvailableFields();
			SetGroupFields();
			SetSortFields();
			SetParamFields ();
			SetFunctions();
			this.EndUpdate();
		}
		
		private const int folderClosed = 0;
		private const int folderOpen  = 1;
			
		void BuildNodes() {

			BeginUpdate();
			TreeNode root = new TreeNode(ResourceService.GetString("SharpReport.FieldsExplorer.Title"));		
			nodeAvailableFields = new SectionTreeNode(ResourceService.GetString("SharpReport.FieldsExplorer.AvailableFields"));
			nodeAvailableFields.ImageIndex = folderClosed;
			nodeAvailableFields.SelectedImageIndex = folderOpen;
			// we don't like a ContextMenu here
			nodeAvailableFields.ContextmenuAddinTreePath = "";
			root.Nodes.Add(this.nodeAvailableFields);
			
	
			nodeSorting = new SectionTreeNode (ResourceService.GetString("SharpReport.FieldsExplorer.Sorting"));
			nodeSorting.ImageIndex = folderClosed;
			nodeSorting.SelectedImageIndex = folderOpen;
			root.Nodes.Add(this.nodeSorting);
			
			nodeGrouping = new SectionTreeNode (ResourceService.GetString("SharpReport.FieldsExplorer.Grouping"));
			nodeGrouping.ImageIndex = folderClosed;
			nodeGrouping.SelectedImageIndex = folderOpen;
			root.Nodes.Add(this.nodeGrouping);
			
			nodeFunction = new TreeNode(ResourceService.GetString("SharpReport.FieldsExplorer.Functions"));
			nodeFunction.ImageIndex = folderClosed;
			nodeFunction.SelectedImageIndex = folderOpen;
			root.Nodes.Add(this.nodeFunction);
			
			nodeParams = new TreeNode(ResourceService.GetString("SharpReport.FieldsExplorer.Parameters"));
			nodeParams.ImageIndex = folderClosed;
			nodeParams.SelectedImageIndex = folderOpen;
			
			root.Nodes.Add(this.nodeParams);
			Nodes.Add(root);
			this.EndUpdate();
		}
	
		
		void InitImageList() {
			ImageList imageList = new ImageList();
			imageList.ColorDepth = ColorDepth.Depth32Bit;
			imageList.ImageSize = new System.Drawing.Size(16, 16);
			try {
				imageList.Images.Add(IconService.GetBitmap("Icons.16x16.ClosedFolderBitmap"));
				imageList.Images.Add(IconService.GetBitmap("Icons.16x16.OpenFolderBitmap"));
				imageList.Images.Add(new Bitmap(1, 1));

				imageList.Images.Add(IconService.GetBitmap("Icons.16x16.SelectionArrow"));
				
				ResourceManager rm = new ResourceManager ("SharpReportAddin.Resources.BitmapResources",
				                                          System.Reflection.Assembly.GetExecutingAssembly());
				
				
				imageList.Images.Add ((Bitmap)rm.GetObject("Icons.SharpReport.16x16.Ascending"));
				imageList.Images.Add ((Bitmap)rm.GetObject("Icons.SharpReport.16x16.Descending"));
				
				//Table's or procedure
				imageList.Images.Add(IconService.GetBitmap("Icons.16x16.SharpQuery.Table"));
				imageList.Images.Add(IconService.GetBitmap("Icons.16x16.SharpQuery.Procedure"));
				imageList.Images.Add(IconService.GetBitmap("Icons.16x16.SharpQuery.View"));
				//Parameters
				imageList.Images.Add(IconService.GetBitmap("Icons.16x16.SharpQuery.Column"));
				ImageList = imageList;
			} catch (Exception e) {
				MessageService.ShowError(e);
			}
		}
		#endregion
		
		#region ICSharpCode.SharpDevelop.Gui.IPadContent interface implementation
		public string Title {
			get {
				return ResourceService.GetString("SharpReport.FieldsExplorer.Title");
			}
		}
		
		public string Icon {
			get {
				return "FileIcons.XmlIcon";
			}
		}
		
		public string Category {
			get {
				return String.Empty;
			}
			set {
			}
		}
		
		public string[] Shortcut {
			get {
				return null;
			}
			set {
			}
		}
		
		public System.Windows.Forms.Control Control {
			get {
				return this.contentPanel;
			}
		}
		
		
		public void RedrawContent() {
			this.Invalidate ();
		}
		
		// ********* Own events
		protected virtual void OnTitleChanged(EventArgs e)
		{
			if (TitleChanged != null) {
				TitleChanged(this, e);
			}
		}

		protected virtual void OnIconChanged(EventArgs e)
		{
			if (IconChanged != null) {
				IconChanged(this, e);
			}
		}

		public event EventHandler TitleChanged;
		public event EventHandler IconChanged;
		
		
		#endregion
		
		
		public FieldsExplorer() {

			WorkbenchSingleton.Workbench.ActiveWorkbenchWindowChanged += OnWindowChange;
			
			LabelEdit     = true;
			AllowDrop     = true;
			HideSelection = false;
			Dock          = DockStyle.Fill;
			Scrollable = true;
			LabelEdit = false;
			this.MouseDown += TreeMouseDown;
			this.ItemDrag += TreeViewItemDrag;
			this.DragDrop += TreeViewDragDrop;
			this.DragOver += TreeViewDragOver;
			contentPanel.Controls.Add(this);
		}
	}
}
