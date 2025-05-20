namespace Редактор_файловых_иерархических_сртруктур
{
    partial class ChildForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.buttonUndo = new System.Windows.Forms.Button();
            this.buttonRedo = new System.Windows.Forms.Button();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.renameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pasteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // treeView1
            // 
            this.treeView1.BackColor = System.Drawing.SystemColors.Info;
            this.treeView1.ContextMenuStrip = this.contextMenuStrip1;
            this.treeView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeView1.Location = new System.Drawing.Point(0, 0);
            this.treeView1.Name = "treeView1";
            this.treeView1.Size = new System.Drawing.Size(546, 389);
            this.treeView1.TabIndex = 0;
            this.treeView1.AfterLabelEdit += new System.Windows.Forms.NodeLabelEditEventHandler(this.treeView1_AfterLabelEdit);
            this.treeView1.BeforeExpand += new System.Windows.Forms.TreeViewCancelEventHandler(this.treeView1_BeforeExpand);
            this.treeView1.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.treeView1_NodeMouseClick);
            // 
            // buttonUndo
            // 
            this.buttonUndo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonUndo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            this.buttonUndo.Enabled = false;
            this.buttonUndo.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonUndo.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold);
            this.buttonUndo.Location = new System.Drawing.Point(488, 0);
            this.buttonUndo.Name = "buttonUndo";
            this.buttonUndo.Size = new System.Drawing.Size(26, 24);
            this.buttonUndo.TabIndex = 1;
            this.buttonUndo.Text = "<";
            this.buttonUndo.UseVisualStyleBackColor = false;
            this.buttonUndo.Click += new System.EventHandler(this.buttonUndo_Click);
            // 
            // buttonRedo
            // 
            this.buttonRedo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonRedo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            this.buttonRedo.Enabled = false;
            this.buttonRedo.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonRedo.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold);
            this.buttonRedo.Location = new System.Drawing.Point(520, 0);
            this.buttonRedo.Name = "buttonRedo";
            this.buttonRedo.Size = new System.Drawing.Size(26, 24);
            this.buttonRedo.TabIndex = 2;
            this.buttonRedo.Text = ">";
            this.buttonRedo.UseVisualStyleBackColor = false;
            this.buttonRedo.Click += new System.EventHandler(this.buttonRedo_Click);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.renameToolStripMenuItem,
            this.copyToolStripMenuItem,
            this.cutToolStripMenuItem,
            this.pasteToolStripMenuItem,
            this.deleteToolStripMenuItem,
            this.addToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(162, 136);
            // 
            // renameToolStripMenuItem
            // 
            this.renameToolStripMenuItem.Name = "renameToolStripMenuItem";
            this.renameToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.renameToolStripMenuItem.Text = "Переименовать";
            this.renameToolStripMenuItem.Click += new System.EventHandler(this.renameToolStripMenuItem_Click);
            // 
            // copyToolStripMenuItem
            // 
            this.copyToolStripMenuItem.Name = "copyToolStripMenuItem";
            this.copyToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.copyToolStripMenuItem.Text = "Копировать";
            this.copyToolStripMenuItem.Click += new System.EventHandler(this.copyToolStripMenuItem_Click);
            // 
            // cutToolStripMenuItem
            // 
            this.cutToolStripMenuItem.Name = "cutToolStripMenuItem";
            this.cutToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.cutToolStripMenuItem.Text = "Вырезать";
            this.cutToolStripMenuItem.Click += new System.EventHandler(this.cutToolStripMenuItem_Click);
            // 
            // pasteToolStripMenuItem
            // 
            this.pasteToolStripMenuItem.Name = "pasteToolStripMenuItem";
            this.pasteToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.pasteToolStripMenuItem.Text = "Вставить";
            this.pasteToolStripMenuItem.Click += new System.EventHandler(this.pasteToolStripMenuItem_Click);
            // 
            // deleteToolStripMenuItem
            // 
            this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            this.deleteToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.deleteToolStripMenuItem.Text = "Удалить";
            this.deleteToolStripMenuItem.Click += new System.EventHandler(this.deleteToolStripMenuItem_Click);
            // 
            // addToolStripMenuItem
            // 
            this.addToolStripMenuItem.Name = "addToolStripMenuItem";
            this.addToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.addToolStripMenuItem.Text = "Добавить";
            this.addToolStripMenuItem.Click += new System.EventHandler(this.addToolStripMenuItem_Click);
            // 
            // ChildForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Info;
            this.ClientSize = new System.Drawing.Size(546, 389);
            this.Controls.Add(this.buttonRedo);
            this.Controls.Add(this.buttonUndo);
            this.Controls.Add(this.treeView1);
            this.Name = "ChildForm";
            this.Text = "Form2";
            this.Load += new System.EventHandler(this.ChildForm_Load);
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TreeView treeView1;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.Button buttonUndo;
        private System.Windows.Forms.Button buttonRedo;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem renameToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem copyToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem cutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pasteToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addToolStripMenuItem;
    }
}