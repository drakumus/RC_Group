namespace BoggleClient
{
    partial class NameForm
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
            this.statusLabel = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.confirmButton = new System.Windows.Forms.Button();
            this.NameBox = new System.Windows.Forms.TextBox();
            this.NameTextLabel = new System.Windows.Forms.Label();
            this.ServerBox = new System.Windows.Forms.TextBox();
            this.ServerTextLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // statusLabel
            // 
            this.statusLabel.AutoSize = true;
            this.statusLabel.Location = new System.Drawing.Point(65, 114);
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(51, 17);
            this.statusLabel.TabIndex = 13;
            this.statusLabel.Text = "waiting";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 114);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(56, 17);
            this.label1.TabIndex = 12;
            this.label1.Text = "Status: ";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(153, 80);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 11;
            this.button1.Text = "Cancel";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // confirmButton
            // 
            this.confirmButton.Location = new System.Drawing.Point(68, 80);
            this.confirmButton.Name = "confirmButton";
            this.confirmButton.Size = new System.Drawing.Size(75, 23);
            this.confirmButton.TabIndex = 10;
            this.confirmButton.Text = "Confirm";
            this.confirmButton.UseVisualStyleBackColor = true;
            // 
            // NameBox
            // 
            this.NameBox.Location = new System.Drawing.Point(68, 43);
            this.NameBox.Name = "NameBox";
            this.NameBox.Size = new System.Drawing.Size(201, 22);
            this.NameBox.TabIndex = 9;
            // 
            // NameTextLabel
            // 
            this.NameTextLabel.AutoSize = true;
            this.NameTextLabel.Location = new System.Drawing.Point(9, 46);
            this.NameTextLabel.Name = "NameTextLabel";
            this.NameTextLabel.Size = new System.Drawing.Size(49, 17);
            this.NameTextLabel.TabIndex = 8;
            this.NameTextLabel.Text = "Name:";
            // 
            // ServerBox
            // 
            this.ServerBox.Location = new System.Drawing.Point(68, 15);
            this.ServerBox.Name = "ServerBox";
            this.ServerBox.Size = new System.Drawing.Size(201, 22);
            this.ServerBox.TabIndex = 15;
            // 
            // ServerTextLabel
            // 
            this.ServerTextLabel.AutoSize = true;
            this.ServerTextLabel.Location = new System.Drawing.Point(7, 18);
            this.ServerTextLabel.Name = "ServerTextLabel";
            this.ServerTextLabel.Size = new System.Drawing.Size(58, 17);
            this.ServerTextLabel.TabIndex = 14;
            this.ServerTextLabel.Text = "Server: ";
            // 
            // NameForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(282, 147);
            this.Controls.Add(this.ServerBox);
            this.Controls.Add(this.ServerTextLabel);
            this.Controls.Add(this.statusLabel);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.confirmButton);
            this.Controls.Add(this.NameBox);
            this.Controls.Add(this.NameTextLabel);
            this.Name = "NameForm";
            this.Text = "Connection Form";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label statusLabel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button confirmButton;
        private System.Windows.Forms.TextBox NameBox;
        private System.Windows.Forms.Label NameTextLabel;
        private System.Windows.Forms.TextBox ServerBox;
        private System.Windows.Forms.Label ServerTextLabel;
    }
}