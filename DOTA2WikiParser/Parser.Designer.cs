namespace DOTA2WikiParser
{
    partial class ParserForm
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            this.metroTabControl1 = new MetroFramework.Controls.MetroTabControl();
            this.treasuresTabPage = new MetroFramework.Controls.MetroTabPage();
            this.treasuresUrlGrid = new MetroFramework.Controls.MetroGrid();
            this.setsTabPage = new MetroFramework.Controls.MetroTabPage();
            this.itemsTabPage = new MetroFramework.Controls.MetroTabPage();
            this.id = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.treasureName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.treasureRare = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.treasureCost = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.treasureImage = new System.Windows.Forms.DataGridViewImageColumn();
            this.url = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.parserBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.metroTabControl1.SuspendLayout();
            this.treasuresTabPage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.treasuresUrlGrid)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.parserBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // metroTabControl1
            // 
            this.metroTabControl1.Controls.Add(this.treasuresTabPage);
            this.metroTabControl1.Controls.Add(this.setsTabPage);
            this.metroTabControl1.Controls.Add(this.itemsTabPage);
            this.metroTabControl1.Location = new System.Drawing.Point(23, 83);
            this.metroTabControl1.Name = "metroTabControl1";
            this.metroTabControl1.SelectedIndex = 0;
            this.metroTabControl1.Size = new System.Drawing.Size(1260, 691);
            this.metroTabControl1.TabIndex = 0;
            this.metroTabControl1.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.metroTabControl1.UseSelectable = true;
            // 
            // treasuresTabPage
            // 
            this.treasuresTabPage.Controls.Add(this.treasuresUrlGrid);
            this.treasuresTabPage.HorizontalScrollbarBarColor = true;
            this.treasuresTabPage.HorizontalScrollbarHighlightOnWheel = false;
            this.treasuresTabPage.HorizontalScrollbarSize = 10;
            this.treasuresTabPage.Location = new System.Drawing.Point(4, 38);
            this.treasuresTabPage.Name = "treasuresTabPage";
            this.treasuresTabPage.Size = new System.Drawing.Size(1252, 649);
            this.treasuresTabPage.TabIndex = 1;
            this.treasuresTabPage.Text = "Сокровищницы";
            this.treasuresTabPage.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.treasuresTabPage.VerticalScrollbarBarColor = true;
            this.treasuresTabPage.VerticalScrollbarHighlightOnWheel = false;
            this.treasuresTabPage.VerticalScrollbarSize = 10;
            // 
            // treasuresUrlGrid
            // 
            this.treasuresUrlGrid.AllowUserToResizeRows = false;
            this.treasuresUrlGrid.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(17)))), ((int)(((byte)(17)))), ((int)(((byte)(17)))));
            this.treasuresUrlGrid.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.treasuresUrlGrid.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.treasuresUrlGrid.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(174)))), ((int)(((byte)(219)))));
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(17)))), ((int)(((byte)(17)))), ((int)(((byte)(17)))));
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(198)))), ((int)(((byte)(247)))));
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(17)))), ((int)(((byte)(17)))), ((int)(((byte)(17)))));
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.treasuresUrlGrid.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.treasuresUrlGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.treasuresUrlGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.id,
            this.treasureName,
            this.treasureRare,
            this.treasureCost,
            this.treasureImage,
            this.url});
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(17)))), ((int)(((byte)(17)))), ((int)(((byte)(17)))));
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(109)))), ((int)(((byte)(109)))), ((int)(((byte)(109)))));
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(198)))), ((int)(((byte)(247)))));
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(17)))), ((int)(((byte)(17)))), ((int)(((byte)(17)))));
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.treasuresUrlGrid.DefaultCellStyle = dataGridViewCellStyle2;
            this.treasuresUrlGrid.EnableHeadersVisualStyles = false;
            this.treasuresUrlGrid.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.treasuresUrlGrid.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(17)))), ((int)(((byte)(17)))), ((int)(((byte)(17)))));
            this.treasuresUrlGrid.Location = new System.Drawing.Point(3, 3);
            this.treasuresUrlGrid.Name = "treasuresUrlGrid";
            this.treasuresUrlGrid.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(174)))), ((int)(((byte)(219)))));
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            dataGridViewCellStyle3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(17)))), ((int)(((byte)(17)))), ((int)(((byte)(17)))));
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(198)))), ((int)(((byte)(247)))));
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(17)))), ((int)(((byte)(17)))), ((int)(((byte)(17)))));
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.treasuresUrlGrid.RowHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this.treasuresUrlGrid.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.treasuresUrlGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.treasuresUrlGrid.Size = new System.Drawing.Size(1240, 639);
            this.treasuresUrlGrid.TabIndex = 3;
            this.treasuresUrlGrid.Theme = MetroFramework.MetroThemeStyle.Dark;
            // 
            // setsTabPage
            // 
            this.setsTabPage.HorizontalScrollbarBarColor = true;
            this.setsTabPage.HorizontalScrollbarHighlightOnWheel = false;
            this.setsTabPage.HorizontalScrollbarSize = 10;
            this.setsTabPage.Location = new System.Drawing.Point(4, 38);
            this.setsTabPage.Name = "setsTabPage";
            this.setsTabPage.Size = new System.Drawing.Size(972, 498);
            this.setsTabPage.TabIndex = 2;
            this.setsTabPage.Text = "Сеты";
            this.setsTabPage.VerticalScrollbarBarColor = true;
            this.setsTabPage.VerticalScrollbarHighlightOnWheel = false;
            this.setsTabPage.VerticalScrollbarSize = 10;
            // 
            // itemsTabPage
            // 
            this.itemsTabPage.HorizontalScrollbarBarColor = true;
            this.itemsTabPage.HorizontalScrollbarHighlightOnWheel = false;
            this.itemsTabPage.HorizontalScrollbarSize = 10;
            this.itemsTabPage.Location = new System.Drawing.Point(4, 38);
            this.itemsTabPage.Name = "itemsTabPage";
            this.itemsTabPage.Size = new System.Drawing.Size(972, 498);
            this.itemsTabPage.TabIndex = 3;
            this.itemsTabPage.Text = "Итемы";
            this.itemsTabPage.VerticalScrollbarBarColor = true;
            this.itemsTabPage.VerticalScrollbarHighlightOnWheel = false;
            this.itemsTabPage.VerticalScrollbarSize = 10;
            // 
            // id
            // 
            this.id.HeaderText = "id";
            this.id.Name = "id";
            this.id.Width = 30;
            // 
            // treasureName
            // 
            this.treasureName.HeaderText = "Name";
            this.treasureName.Name = "treasureName";
            // 
            // treasureRare
            // 
            this.treasureRare.HeaderText = "Rare";
            this.treasureRare.Name = "treasureRare";
            // 
            // treasureCost
            // 
            this.treasureCost.HeaderText = "Cost";
            this.treasureCost.Name = "treasureCost";
            // 
            // treasureImage
            // 
            this.treasureImage.HeaderText = "Image";
            this.treasureImage.Name = "treasureImage";
            // 
            // url
            // 
            this.url.HeaderText = "URL";
            this.url.Name = "url";
            this.url.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.url.Width = 300;
            // 
            // parserBindingSource
            // 
            this.parserBindingSource.DataSource = typeof(DOTA2WikiParser.ParserForm);
            // 
            // ParserForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1306, 786);
            this.Controls.Add(this.metroTabControl1);
            this.Name = "ParserForm";
            this.Text = "Парсер сокровищниц, итемов, сетов";
            this.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.Load += new System.EventHandler(this.Form1_Load);
            this.metroTabControl1.ResumeLayout(false);
            this.treasuresTabPage.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.treasuresUrlGrid)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.parserBindingSource)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private MetroFramework.Controls.MetroTabControl metroTabControl1;
        private MetroFramework.Controls.MetroTabPage treasuresTabPage;
        private MetroFramework.Controls.MetroTabPage setsTabPage;
        private MetroFramework.Controls.MetroTabPage itemsTabPage;
        private System.Windows.Forms.BindingSource parserBindingSource;
        private MetroFramework.Controls.MetroGrid treasuresUrlGrid;
        private System.Windows.Forms.DataGridViewTextBoxColumn id;
        private System.Windows.Forms.DataGridViewTextBoxColumn treasureName;
        private System.Windows.Forms.DataGridViewTextBoxColumn treasureRare;
        private System.Windows.Forms.DataGridViewTextBoxColumn treasureCost;
        private System.Windows.Forms.DataGridViewImageColumn treasureImage;
        private System.Windows.Forms.DataGridViewTextBoxColumn url;
    }
}

