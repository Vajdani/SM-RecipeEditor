namespace SM_RecipeEditor
{
    partial class NewRecipeFile
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
            tx_name = new TextBox();
            label1 = new Label();
            bt_cancel = new Button();
            bt_ok = new Button();
            SuspendLayout();
            // 
            // tx_name
            // 
            tx_name.Location = new Point(123, 27);
            tx_name.Name = "tx_name";
            tx_name.Size = new Size(128, 23);
            tx_name.TabIndex = 0;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(12, 9);
            label1.Name = "label1";
            label1.Size = new Size(359, 15);
            label1.TabIndex = 1;
            label1.Text = "Enter the name of the recipe file(craftbot/hideout/workbench/etc.)";
            // 
            // bt_cancel
            // 
            bt_cancel.Location = new Point(12, 57);
            bt_cancel.Name = "bt_cancel";
            bt_cancel.Size = new Size(75, 23);
            bt_cancel.TabIndex = 2;
            bt_cancel.Text = "Cancel";
            bt_cancel.UseVisualStyleBackColor = true;
            bt_cancel.Click += OnCancelClick;
            // 
            // bt_ok
            // 
            bt_ok.Location = new Point(296, 57);
            bt_ok.Name = "bt_ok";
            bt_ok.Size = new Size(75, 23);
            bt_ok.TabIndex = 3;
            bt_ok.Text = "OK";
            bt_ok.UseVisualStyleBackColor = true;
            bt_ok.Click += OnOKClick;
            // 
            // NewRecipeFile
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(384, 88);
            Controls.Add(bt_ok);
            Controls.Add(bt_cancel);
            Controls.Add(label1);
            Controls.Add(tx_name);
            Name = "NewRecipeFile";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Create new recipe file";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox tx_name;
        private Label label1;
        private Button bt_cancel;
        private Button bt_ok;
    }
}