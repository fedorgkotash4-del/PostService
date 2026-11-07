namespace PostService.App;
partial class PostingEditor
{
    protected override void Dispose(bool disposing)
    {
        if (disposing) { _closing.Cancel(); _errors?.Dispose(); _closing.Dispose(); }
        base.Dispose(disposing);
    }
    private void InitializeComponent()
    {
        SuspendLayout();
        AutoScaleDimensions=new SizeF(96,96); AutoScaleMode=AutoScaleMode.Dpi;
        Font=new Font("Segoe UI",10); ClientSize=new Size(1040,710);
        MinimumSize=new Size(1056,749); MaximumSize=new Size(1056,749);
        StartPosition=FormStartPosition.CenterParent; MaximizeBox=false; MinimizeBox=false;
        Text="Відправлення"; BackColor=Color.White;
        _errors=new ErrorProvider { ContainerControl=this, BlinkStyle=ErrorBlinkStyle.NeverBlink };
        var heading=new Label { Text="Дані поштового відправлення", Font=new Font("Segoe UI",18,FontStyle.Bold),
            Location=new Point(24,18), Size=new Size(950,40) }; Controls.Add(heading);
        _summary=new Label { Text="* — обов’язкове поле. Числа можна вводити з комою або крапкою.",
            Location=new Point(24,65), Size=new Size(990,48) }; Controls.Add(_summary);
        string[] keys={"From","To","Content","DeliveryType","Weight","Width","Height","Depth","Value","Description","Price"};
        string[] titles={"Відправник *","Одержувач *","Вміст *","Тип доставки *","Вага, кг *","Ширина, см *","Висота, см *","Глибина, см *","Оголошена цінність, грн","Опис","Ціна доставки, грн"};
        for (int i=0;i<keys.Length;i++)
        {
            int y=118+i*44;
            Controls.Add(new Label { Text=titles[i], Location=new Point(24,y+5), Size=new Size(220,30) });
            if (keys[i]=="DeliveryType") {
                _delivery=new ComboBox { Name="DeliveryType", AccessibleName="Тип доставки", DropDownStyle=ComboBoxStyle.DropDownList,
                    Location=new Point(250,y), Size=new Size(390,32), TabIndex=i };
                foreach(var type in Enum.GetValues<PostService.CommonTypes.DeliveryType>()) _delivery.Items.Add(PostingInputValidator.DeliveryName(type));
                _delivery.SelectedIndex=0; Controls.Add(_delivery); continue;
            }
            var input=new TextBox { Name=keys[i], AccessibleName=titles[i], Location=new Point(250,y), Size=new Size(390,32), TabIndex=i };
            var error=new Label { ForeColor=Color.Firebrick, Location=new Point(675,y+5), Size=new Size(340,34) };
            input.TextChanged += (_, _) => { _errors.SetError(input, ""); error.Text=""; };
            _fields.Add(keys[i],input); _messages.Add(keys[i],error); Controls.Add(input); Controls.Add(error);
        }
        _save=new Button { Text="Зберегти", Name="btnSave", Location=new Point(728,642), Size=new Size(135,40),
            BackColor=Color.FromArgb(35,99,180), ForeColor=Color.White, FlatStyle=FlatStyle.Flat, TabIndex=11 };
        _save.Click+=SaveRecord;
        _cancel=new Button { Text="Скасувати", Name="btnCancel", DialogResult=DialogResult.Cancel,
            Location=new Point(877,642), Size=new Size(135,40), TabIndex=12 };
        Controls.Add(_save); Controls.Add(_cancel); AcceptButton=_save; CancelButton=_cancel;
        ResumeLayout(false); PerformLayout();
    }
}
