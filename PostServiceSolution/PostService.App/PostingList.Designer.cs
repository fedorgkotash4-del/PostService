#nullable disable
namespace PostService.App;

partial class PostingList
{
    private System.ComponentModel.IContainer components;

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            ReleaseResources();
            components?.Dispose();
        }
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container();
        gridPostings = new DataGridView();
        btnDelete = new Button();
        btnRefresh = new Button();
        lblStatus = new Label();
        colId = new DataGridViewTextBoxColumn();
        colFrom = new DataGridViewTextBoxColumn();
        colTo = new DataGridViewTextBoxColumn();
        colContent = new DataGridViewTextBoxColumn();
        colDeliveryType = new DataGridViewTextBoxColumn();
        colWeight = new DataGridViewTextBoxColumn();
        colWidth = new DataGridViewTextBoxColumn();
        colHeight = new DataGridViewTextBoxColumn();
        colDepth = new DataGridViewTextBoxColumn();
        colValue = new DataGridViewTextBoxColumn();
        colPrice = new DataGridViewTextBoxColumn();
        colDescription = new DataGridViewTextBoxColumn();
        colCreatedAt = new DataGridViewTextBoxColumn();
        ((System.ComponentModel.ISupportInitialize)gridPostings).BeginInit();
        SuspendLayout();
        // gridPostings
        gridPostings.AllowUserToAddRows = false;
        gridPostings.AllowUserToDeleteRows = false;
        gridPostings.AutoGenerateColumns = false;
        gridPostings.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        gridPostings.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        gridPostings.BackgroundColor = SystemColors.Window;
        gridPostings.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        gridPostings.Columns.AddRange(new DataGridViewColumn[]
        {
            colId,
            colFrom,
            colTo,
            colContent,
            colDeliveryType,
            colWeight,
            colWidth,
            colHeight,
            colDepth,
            colValue,
            colPrice,
            colDescription,
            colCreatedAt
        });
        gridPostings.Location = new Point(16, 115);
        gridPostings.MultiSelect = false;
        gridPostings.Name = "gridPostings";
        gridPostings.ReadOnly = true;
        gridPostings.RowHeadersVisible = false;
        gridPostings.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        gridPostings.Size = new Size(1248, 430);
        gridPostings.TabIndex = 0;
        // colId
        colId.DataPropertyName = "Id";
        colId.HeaderText = "Id";
        colId.Name = "colId";
        colId.ReadOnly = true;
        colId.FillWeight = 45F;
        colId.Visible = false;
        // colFrom
        colFrom.DataPropertyName = "From";
        colFrom.HeaderText = "Відправник";
        colFrom.Name = "colFrom";
        colFrom.ReadOnly = true;
        colFrom.FillWeight = 105F;
        colFrom.Visible = true;
        // colTo
        colTo.DataPropertyName = "To";
        colTo.HeaderText = "Одержувач";
        colTo.Name = "colTo";
        colTo.ReadOnly = true;
        colTo.FillWeight = 105F;
        colTo.Visible = true;
        // colContent
        colContent.DataPropertyName = "Content";
        colContent.HeaderText = "Вміст";
        colContent.Name = "colContent";
        colContent.ReadOnly = true;
        colContent.FillWeight = 130F;
        colContent.Visible = true;
        // colDeliveryType
        colDeliveryType.DataPropertyName = "DeliveryType";
        colDeliveryType.HeaderText = "Тип доставки";
        colDeliveryType.Name = "colDeliveryType";
        colDeliveryType.ReadOnly = true;
        colDeliveryType.FillWeight = 110F;
        colDeliveryType.Visible = true;
        // colWeight
        colWeight.DataPropertyName = "Weight";
        colWeight.HeaderText = "Вага, кг";
        colWeight.Name = "colWeight";
        colWeight.ReadOnly = true;
        colWeight.FillWeight = 65F;
        colWeight.Visible = true;
        colWeight.DefaultCellStyle.Format = "0.##";
        // colWidth
        colWidth.DataPropertyName = "Width";
        colWidth.HeaderText = "Ширина";
        colWidth.Name = "colWidth";
        colWidth.ReadOnly = true;
        colWidth.FillWeight = 65F;
        colWidth.Visible = true;
        colWidth.DefaultCellStyle.Format = "0.##";
        // colHeight
        colHeight.DataPropertyName = "Height";
        colHeight.HeaderText = "Висота";
        colHeight.Name = "colHeight";
        colHeight.ReadOnly = true;
        colHeight.FillWeight = 65F;
        colHeight.Visible = true;
        colHeight.DefaultCellStyle.Format = "0.##";
        // colDepth
        colDepth.DataPropertyName = "Depth";
        colDepth.HeaderText = "Глибина";
        colDepth.Name = "colDepth";
        colDepth.ReadOnly = true;
        colDepth.FillWeight = 65F;
        colDepth.Visible = true;
        colDepth.DefaultCellStyle.Format = "0.##";
        // colValue
        colValue.DataPropertyName = "Value";
        colValue.HeaderText = "Цінність, грн";
        colValue.Name = "colValue";
        colValue.ReadOnly = true;
        colValue.FillWeight = 100F;
        colValue.Visible = true;
        colValue.DefaultCellStyle.Format = "0.##";
        // colPrice
        colPrice.DataPropertyName = "Price";
        colPrice.HeaderText = "Ціна, грн";
        colPrice.Name = "colPrice";
        colPrice.ReadOnly = true;
        colPrice.FillWeight = 80F;
        colPrice.Visible = true;
        colPrice.DefaultCellStyle.Format = "0.##";
        // colDescription
        colDescription.DataPropertyName = "Description";
        colDescription.HeaderText = "Опис";
        colDescription.Name = "colDescription";
        colDescription.ReadOnly = true;
        colDescription.FillWeight = 125F;
        colDescription.Visible = true;
        // colCreatedAt
        colCreatedAt.DataPropertyName = "CreatedAt";
        colCreatedAt.HeaderText = "Створено";
        colCreatedAt.Name = "colCreatedAt";
        colCreatedAt.ReadOnly = true;
        colCreatedAt.FillWeight = 120F;
        colCreatedAt.Visible = false;
        // btnRefresh
        btnRefresh.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        btnRefresh.Location = new Point(1004, 16);
        btnRefresh.Name = "btnRefresh";
        btnRefresh.Size = new Size(122, 34);
        btnRefresh.TabIndex = 1;
        btnRefresh.Text = "Оновити";
        btnRefresh.UseVisualStyleBackColor = true;
        btnRefresh.Click += btnRefresh_Click;
        // btnDelete
        btnDelete.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        btnDelete.Location = new Point(1142, 16);
        btnDelete.Name = "btnDelete";
        btnDelete.Size = new Size(122, 34);
        btnDelete.TabIndex = 2;
        btnDelete.Text = "Видалити";
        btnDelete.UseVisualStyleBackColor = true;
        btnDelete.Click += btnDelete_Click;
        // lblStatus
        lblStatus.AutoSize = true;
        lblStatus.Location = new Point(16, 77);
        lblStatus.Name = "lblStatus";
        lblStatus.Size = new Size(130, 19);
        lblStatus.TabIndex = 3;
        lblStatus.Text = "Відправлень: 0";
        // PostingList
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(1280, 565);
        Controls.Add(new Label { Text="Облік поштових відправлень", Location=new Point(16,18), Size=new Size(470,35), Font=new Font("Segoe UI",16,FontStyle.Bold) });
        btnCreate = new Button { Text="Нове відправлення", Name="btnCreate", Location=new Point(520,16), Size=new Size(200,34), BackColor=Color.FromArgb(35,99,180), ForeColor=Color.White, FlatStyle=FlatStyle.Flat, Anchor=AnchorStyles.Top|AnchorStyles.Right };
        btnEdit = new Button { Text="Редагувати", Name="btnEdit", Location=new Point(736,16), Size=new Size(150,34), Anchor=AnchorStyles.Top|AnchorStyles.Right };
        btnCreate.Click += btnCreate_Click; btnEdit.Click += btnEdit_Click;
        Controls.Add(btnCreate); Controls.Add(btnEdit);
        Controls.Add(gridPostings);
        Controls.Add(btnDelete);
        Controls.Add(btnRefresh);
        Controls.Add(lblStatus);
        MinimumSize = new Size(1100, 400);
        Name = "PostingList";
        StartPosition = FormStartPosition.CenterScreen;
        Text = "Поштові відправлення";
        Load += PostingList_Load;
        ((System.ComponentModel.ISupportInitialize)gridPostings).EndInit();
        ResumeLayout(false);
        PerformLayout();
    }

    private DataGridView gridPostings;
    private Button btnCreate;
    private Button btnEdit;
    private Button btnDelete;
    private Button btnRefresh;
    private Label lblStatus;
    private DataGridViewTextBoxColumn colId;
    private DataGridViewTextBoxColumn colFrom;
    private DataGridViewTextBoxColumn colTo;
    private DataGridViewTextBoxColumn colContent;
    private DataGridViewTextBoxColumn colDeliveryType;
    private DataGridViewTextBoxColumn colWeight;
    private DataGridViewTextBoxColumn colWidth;
    private DataGridViewTextBoxColumn colHeight;
    private DataGridViewTextBoxColumn colDepth;
    private DataGridViewTextBoxColumn colValue;
    private DataGridViewTextBoxColumn colPrice;
    private DataGridViewTextBoxColumn colDescription;
    private DataGridViewTextBoxColumn colCreatedAt;
}
