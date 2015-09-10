﻿using System;
using System.Text;
using System.Windows.Forms;

namespace Test
{
    public partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();
		}

		private void Form1_Load( object sender, EventArgs e )
		{
		}

        private void button1_Click( object sender, EventArgs e )
		{
            this.comboBox1.DropDownHeight = 80;

			for ( char i = 'a'; i < 'z'; i++ )
			{
                StringBuilder sb = new StringBuilder();
                for ( int j = 0; j < 8; j++ )
                {
                    sb.Append( i );
                }
				this.comboBox1.Items.Add( sb.ToString() );
				this.autoCompleteTextBox1.Items.Add( sb.ToString() );
			}

			this.comboBox1.DroppedDown = true;
            
            this.autoCompleteTextBox1.DropList();

            this.autoCompleteTextBox1.Focus();
        }
		
	}
}
