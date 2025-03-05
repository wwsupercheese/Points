using System;
using System.Drawing;
using System.Windows.Forms;

namespace WinFormsApp
{
    partial class PointForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            listBox = new ListBox();
            btnCreate = new Button();
            btnSort = new Button();
            btnSerialize = new Button();
            btnDeserialize = new Button();
            SuspendLayout();
            // 
            // listBox
            // 
            listBox.FormattingEnabled = true;
            listBox.ItemHeight = 15;
            listBox.Location = new Point(12, 12);
            listBox.Name = "listBox";
            listBox.Size = new Size(626, 364);
            listBox.TabIndex = 0;
            // 
            // btnCreate
            // 
            btnCreate.Location = new Point(12, 384);
            btnCreate.Name = "btnCreate";
            btnCreate.Size = new Size(152, 54);
            btnCreate.TabIndex = 1;
            btnCreate.Text = "Create";
            btnCreate.UseVisualStyleBackColor = true;
            btnCreate.Click += btnCreate_Click;
            // 
            // btnSort
            // 
            btnSort.Location = new Point(170, 384);
            btnSort.Name = "btnSort";
            btnSort.Size = new Size(152, 54);
            btnSort.TabIndex = 2;
            btnSort.Text = "Sort";
            btnSort.UseVisualStyleBackColor = true;
            btnSort.Click += btnSort_Click;
            // 
            // btnSerialize
            // 
            btnSerialize.Location = new Point(328, 384);
            btnSerialize.Name = "btnSerialize";
            btnSerialize.Size = new Size(152, 54);
            btnSerialize.TabIndex = 3;
            btnSerialize.Text = "Serialize";
            btnSerialize.UseVisualStyleBackColor = true;
            btnSerialize.Click += btnSerialize_Click;
            // 
            // btnDeserialize
            // 
            btnDeserialize.Location = new Point(486, 384);
            btnDeserialize.Name = "btnDeserialize";
            btnDeserialize.Size = new Size(152, 54);
            btnDeserialize.TabIndex = 4;
            btnDeserialize.Text = "Deserialize";
            btnDeserialize.UseVisualStyleBackColor = true;
            btnDeserialize.Click += btnDeserialize_Click;
            // 
            // PointForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(653, 450);
            Controls.Add(btnDeserialize);
            Controls.Add(btnSerialize);
            Controls.Add(btnSort);
            Controls.Add(btnCreate);
            Controls.Add(listBox);
            Name = "PointForm";
            Text = "PointForm";
            ResumeLayout(false);
        }

        #endregion

        private ListBox listBox;
        private Button btnCreate;
        private Button btnSort;
        private Button btnSerialize;
        private Button btnDeserialize;
    }
}
