﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Collections;

namespace ExtLibrary
{
    /// <summary>
    /// 带下拉列表的自定义搜索文本框
    /// </summary>
	[ToolboxItem( true )]
	public partial class CustomCompleteTextBox : TextBox
    {
        /// <summary>
        /// 监视鼠标滚轮事件
        /// </summary>
        private MouseWheelFilter mouseWheel;

        /// <summary>
        /// 监视鼠标左,中,右键点击事件
        /// </summary>
        private AppClickFilter appClick;

        /// <summary>
        /// 是否手动设置 Text 文本
        /// </summary>
        private bool manualChangeText;

        /// <summary>
        /// 显示候选列表
        /// </summary>
        private ListBox box;
		private ToolStripControlHost host;

        /// <summary>
        /// 下拉控件
        /// </summary>
		private ToolStripDropDownExt drop;

        //--------------------------------------------------------------------------------

		/// <summary>
		/// 获取或设置数据集合
		/// </summary>
		public List<object> Items
		{
			get;
			set;
		}

        /// <summary>
        /// 获取选择的项目
        /// </summary>
        public object SelectedItem
        {
            get;
            internal set;
        }

        /// <summary>
        /// 获取或设置是否自动显示下拉列表
        /// </summary>
        public bool AutoDrop
        {
            get;
            set;
        }

        //--------------------------------------------------------------------------------

		/// <summary>
		/// 构造函数
		/// </summary>
		public CustomCompleteTextBox()
			: base()
		{
			this.InitControl();
		}

        //--------------------------------------------------------------------------------

        /// <summary>
        /// 初始化布局
        /// </summary>
		protected override void InitLayout()
		{
			base.InitLayout();

			this.box.Width = this.Width - 2;
		}

		protected override void OnClick( EventArgs e )
		{
			base.OnClick( e );

            if ( this.AutoDrop )
            {
                this.DropList();
            }
		}

		protected override void OnEnter( EventArgs e )
		{
			base.OnEnter( e );
            this.mouseWheel.Enable = true;

            if ( this.AutoDrop )
            {
                this.DropList();
            }
        }

		protected override void OnLeave( EventArgs e )
		{
			base.OnLeave( e );
            this.mouseWheel.Enable = false;

            if ( this.AutoDrop )
            {
                this.CloseList();
            }
		}

        protected override void OnTextChanged( EventArgs e )
		{
			base.OnTextChanged( e );

            if ( this.AutoDrop && this.manualChangeText )
            {
                this.DropList();
            }
        }

        //--------------------------------------------------------------------------------

        /// <summary>
        /// 显示下拉列表
        /// </summary>
        public void DropList()
        {
            if ( this.Items != null )
            {
                this.box.Items.Clear();

                if ( this.Text == string.Empty )
                {
                    this.box.Items.AddRange( this.Items.ToArray() );
                }
                else
                {
                    List<object> newList = new List<object>();

                    for ( int i = 0; i < this.Items.Count; i++ )
                    {
                        object obj = this.Items[i];

                        if ( obj != null )
                        {
                            if ( obj.ToString().IndexOf( this.Text, StringComparison.OrdinalIgnoreCase ) >= 0 )
                            {
                                newList.Add( obj );
                            }
                        }
                    }

                    this.box.Items.AddRange( newList.ToArray() );
                }

                if ( this.box.Items.Contains( this.Text ) )
                {
                    this.box.SelectedIndex = this.box.Items.IndexOf( this.Text );
                }
            }

            if ( !this.drop.Visible )
            {
                this.drop.Show( this, new Point( -2, this.Height - 1 ) );
            }
        }

        /// <summary>
        /// 关闭下拉列表
        /// </summary>
        public void CloseList()
        {
            this.drop.Close();
        }

        //--------------------------------------------------------------------------------

        /// <summary>
        /// 初始化各参数
        /// </summary>
        private void InitControl()
		{
			this.Items = new List<object>();
            this.AutoDrop = true;
            this.manualChangeText = false;

			this.box = new ListBox();
			this.box.Margin = Padding.Empty;
			this.box.BorderStyle = BorderStyle.None;
			this.box.TabStop = false;
			this.box.SelectionMode = SelectionMode.One;
			this.box.IntegralHeight = false;
			this.box.MouseMove += Box_MouseMove;
			this.box.Click += Box_Click;

			this.host = new ToolStripControlHost( box );
			this.host.Margin = Padding.Empty;
			this.host.Padding = Padding.Empty;
			this.host.AutoSize = false;
            this.host.AutoToolTip = false;

			this.drop = new ToolStripDropDownExt();
			this.drop.AutoClose = false;
			this.drop.Items.Add( host );
			this.drop.Margin = Padding.Empty;
			this.drop.Padding = new Padding( 1 );
			this.drop.ShowItemToolTips = false;
			this.drop.TabStop = false;
			this.drop.Closed += Drop_Closed;
			this.drop.ActiveChange += drop_ActiveChange;

            this.mouseWheel = new MouseWheelFilter( this.box );
            //this.mouseWheel.Enable = false;
            this.appClick = new AppClickFilter( () =>
            {
                if ( this.AutoDrop )
                {
                    this.CloseList();
                }
            }, this, this.drop, this.box );
            Application.AddMessageFilter( this.mouseWheel );
            Application.AddMessageFilter( this.appClick );
        }

        /// <summary>
        /// 下拉列表激活或失去激活状态时引发的事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void drop_ActiveChange( object sender, ActiveChangeEventArgs e )
		{
			if ( this.AutoDrop && !e.Active )
			{
                this.CloseList();
			}
		}

        /// <summary>
        /// 单击下拉选项
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		private void Box_Click( object sender, EventArgs e )
		{
            this.SelectedItem = this.box.SelectedItem;

            if ( this.SelectedItem != null )
            {
                this.manualChangeText = false;
                this.Text = this.SelectedItem.ToString();
            }

            this.CloseList();
        }

        /// <summary>
        /// 关闭下拉列表时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		private void Drop_Closed( object sender, ToolStripDropDownClosedEventArgs e )
		{
			this.SelectAll();
			this.box.SelectedIndex = -1;
		}

        /// <summary>
        /// 鼠标在选项间移动时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		private void Box_MouseMove( object sender, MouseEventArgs e )
		{
			this.box.SelectedIndex = this.box.IndexFromPoint( e.Location );
		}
	}
}
