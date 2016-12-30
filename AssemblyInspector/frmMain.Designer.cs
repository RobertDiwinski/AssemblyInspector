namespace AssemblyInspector
{
   partial class frmMain
   {
      /// <summary>
      /// Erforderliche Designervariable.
      /// </summary>
      private System.ComponentModel.IContainer components = null;

      /// <summary>
      /// Verwendete Ressourcen bereinigen.
      /// </summary>
      /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
      protected override void Dispose(bool disposing)
      {
         if (disposing && (components != null))
         {
            components.Dispose();
         }
         base.Dispose(disposing);
      }

      #region Vom Windows Form-Designer generierter Code

      /// <summary>
      /// Erforderliche Methode für die Designerunterstützung.
      /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
      /// </summary>
      private void InitializeComponent()
      {
         this.components = new System.ComponentModel.Container();
         System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMain));
         this.toolStrip1 = new System.Windows.Forms.ToolStrip();
         this.cmdOpen = new System.Windows.Forms.ToolStripButton();
         this.lbl = new System.Windows.Forms.ToolStripLabel();
         this.tree = new System.Windows.Forms.TreeView();
         this.il = new System.Windows.Forms.ImageList(this.components);
         this.splitContainer1 = new System.Windows.Forms.SplitContainer();
         this.txt = new System.Windows.Forms.TextBox();
         this.toolStrip1.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
         this.splitContainer1.Panel1.SuspendLayout();
         this.splitContainer1.Panel2.SuspendLayout();
         this.splitContainer1.SuspendLayout();
         this.SuspendLayout();
         // 
         // toolStrip1
         // 
         this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
         this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cmdOpen,
            this.lbl});
         this.toolStrip1.Location = new System.Drawing.Point(0, 0);
         this.toolStrip1.Name = "toolStrip1";
         this.toolStrip1.Size = new System.Drawing.Size(1035, 25);
         this.toolStrip1.TabIndex = 1;
         this.toolStrip1.Text = "toolStrip1";
         this.toolStrip1.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.frmMain_PreviewKeyDown);
         // 
         // cmdOpen
         // 
         this.cmdOpen.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
         this.cmdOpen.Image = ((System.Drawing.Image)(resources.GetObject("cmdOpen.Image")));
         this.cmdOpen.ImageTransparentColor = System.Drawing.Color.Magenta;
         this.cmdOpen.Name = "cmdOpen";
         this.cmdOpen.Size = new System.Drawing.Size(23, 22);
         this.cmdOpen.Text = "&Open";
         this.cmdOpen.Click += new System.EventHandler(this.cmdOpen_Click);
         // 
         // lbl
         // 
         this.lbl.Name = "lbl";
         this.lbl.Size = new System.Drawing.Size(0, 22);
         // 
         // tree
         // 
         this.tree.Dock = System.Windows.Forms.DockStyle.Fill;
         this.tree.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
         this.tree.ImageIndex = 0;
         this.tree.ImageList = this.il;
         this.tree.Location = new System.Drawing.Point(0, 0);
         this.tree.Name = "tree";
         this.tree.SelectedImageIndex = 0;
         this.tree.Size = new System.Drawing.Size(342, 672);
         this.tree.TabIndex = 2;
         this.tree.BeforeExpand += new System.Windows.Forms.TreeViewCancelEventHandler(this.tree_BeforeExpand);
         this.tree.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.tree_AfterSelect);
         this.tree.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.frmMain_PreviewKeyDown);
         // 
         // il
         // 
         this.il.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("il.ImageStream")));
         this.il.TransparentColor = System.Drawing.Color.Transparent;
         this.il.Images.SetKeyName(0, "ns");
         this.il.Images.SetKeyName(1, "err");
         this.il.Images.SetKeyName(2, "dll");
         this.il.Images.SetKeyName(3, "prop");
         this.il.Images.SetKeyName(4, "event");
         this.il.Images.SetKeyName(5, "ctor");
         this.il.Images.SetKeyName(6, "interface");
         this.il.Images.SetKeyName(7, "link");
         this.il.Images.SetKeyName(8, "enum");
         this.il.Images.SetKeyName(9, "exe");
         this.il.Images.SetKeyName(10, "field");
         this.il.Images.SetKeyName(11, "gac");
         this.il.Images.SetKeyName(12, "class");
         this.il.Images.SetKeyName(13, "method");
         this.il.Images.SetKeyName(14, "struct");
         // 
         // splitContainer1
         // 
         this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
         this.splitContainer1.Location = new System.Drawing.Point(0, 25);
         this.splitContainer1.Name = "splitContainer1";
         // 
         // splitContainer1.Panel1
         // 
         this.splitContainer1.Panel1.Controls.Add(this.tree);
         // 
         // splitContainer1.Panel2
         // 
         this.splitContainer1.Panel2.Controls.Add(this.txt);
         this.splitContainer1.Size = new System.Drawing.Size(1035, 672);
         this.splitContainer1.SplitterDistance = 342;
         this.splitContainer1.TabIndex = 3;
         this.splitContainer1.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.frmMain_PreviewKeyDown);
         // 
         // txt
         // 
         this.txt.BackColor = System.Drawing.SystemColors.Window;
         this.txt.Dock = System.Windows.Forms.DockStyle.Fill;
         this.txt.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
         this.txt.Location = new System.Drawing.Point(0, 0);
         this.txt.Multiline = true;
         this.txt.Name = "txt";
         this.txt.ReadOnly = true;
         this.txt.ScrollBars = System.Windows.Forms.ScrollBars.Both;
         this.txt.Size = new System.Drawing.Size(689, 672);
         this.txt.TabIndex = 0;
         this.txt.WordWrap = false;
         this.txt.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.frmMain_PreviewKeyDown);
         // 
         // frmMain
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.ClientSize = new System.Drawing.Size(1035, 697);
         this.Controls.Add(this.splitContainer1);
         this.Controls.Add(this.toolStrip1);
         this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
         this.KeyPreview = true;
         this.Name = "frmMain";
         this.Text = "Assembly Inspector";
         this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
         this.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.frmMain_PreviewKeyDown);
         this.toolStrip1.ResumeLayout(false);
         this.toolStrip1.PerformLayout();
         this.splitContainer1.Panel1.ResumeLayout(false);
         this.splitContainer1.Panel2.ResumeLayout(false);
         this.splitContainer1.Panel2.PerformLayout();
         ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
         this.splitContainer1.ResumeLayout(false);
         this.ResumeLayout(false);
         this.PerformLayout();

      }

      #endregion
      private System.Windows.Forms.ToolStrip toolStrip1;
      private System.Windows.Forms.ToolStripButton cmdOpen;
      private System.Windows.Forms.TreeView tree;
      private System.Windows.Forms.SplitContainer splitContainer1;
      private System.Windows.Forms.TextBox txt;
      private System.Windows.Forms.ToolStripLabel lbl;
      private System.Windows.Forms.ImageList il;
   }
}

