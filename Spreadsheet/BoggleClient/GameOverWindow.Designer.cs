namespace BoggleClient
{
    partial class GameOverWindow
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
            this.player1Box = new System.Windows.Forms.ListBox();
            this.player2Box = new System.Windows.Forms.ListBox();
            this.player2ScoreLabel = new System.Windows.Forms.Label();
            this.player1ScoreLabel = new System.Windows.Forms.Label();
            this.player2Label = new System.Windows.Forms.Label();
            this.player1Label = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // player1Box
            // 
            this.player1Box.FormattingEnabled = true;
            this.player1Box.ItemHeight = 16;
            this.player1Box.Location = new System.Drawing.Point(12, 94);
            this.player1Box.Name = "player1Box";
            this.player1Box.Size = new System.Drawing.Size(156, 148);
            this.player1Box.TabIndex = 0;
            // 
            // player2Box
            // 
            this.player2Box.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.player2Box.FormattingEnabled = true;
            this.player2Box.ItemHeight = 16;
            this.player2Box.Location = new System.Drawing.Point(181, 94);
            this.player2Box.Name = "player2Box";
            this.player2Box.Size = new System.Drawing.Size(156, 148);
            this.player2Box.TabIndex = 1;
            // 
            // player2ScoreLabel
            // 
            this.player2ScoreLabel.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.player2ScoreLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.player2ScoreLabel.Location = new System.Drawing.Point(252, 48);
            this.player2ScoreLabel.Name = "player2ScoreLabel";
            this.player2ScoreLabel.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.player2ScoreLabel.Size = new System.Drawing.Size(20, 20);
            this.player2ScoreLabel.TabIndex = 16;
            this.player2ScoreLabel.Text = "0";
            this.player2ScoreLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // player1ScoreLabel
            // 
            this.player1ScoreLabel.AutoSize = true;
            this.player1ScoreLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.player1ScoreLabel.Location = new System.Drawing.Point(9, 48);
            this.player1ScoreLabel.Name = "player1ScoreLabel";
            this.player1ScoreLabel.Size = new System.Drawing.Size(18, 20);
            this.player1ScoreLabel.TabIndex = 15;
            this.player1ScoreLabel.Text = "0";
            // 
            // player2Label
            // 
            this.player2Label.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.player2Label.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.player2Label.Location = new System.Drawing.Point(186, 9);
            this.player2Label.Name = "player2Label";
            this.player2Label.Size = new System.Drawing.Size(151, 25);
            this.player2Label.TabIndex = 14;
            this.player2Label.Text = "Player2";
            this.player2Label.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // player1Label
            // 
            this.player1Label.AutoSize = true;
            this.player1Label.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.player1Label.Location = new System.Drawing.Point(8, 9);
            this.player1Label.Name = "player1Label";
            this.player1Label.Size = new System.Drawing.Size(85, 25);
            this.player1Label.TabIndex = 13;
            this.player1Label.Text = "Player1";
            // 
            // label4
            // 
            this.label4.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label4.Location = new System.Drawing.Point(-22, 79);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(400, 2);
            this.label4.TabIndex = 40;
            this.label4.Text = " ";
            // 
            // label1
            // 
            this.label1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label1.Location = new System.Drawing.Point(175, -3);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(2, 83);
            this.label1.TabIndex = 41;
            this.label1.Text = " ";
            // 
            // GameOverWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(349, 250);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.player2ScoreLabel);
            this.Controls.Add(this.player1ScoreLabel);
            this.Controls.Add(this.player2Label);
            this.Controls.Add(this.player1Label);
            this.Controls.Add(this.player2Box);
            this.Controls.Add(this.player1Box);
            this.Name = "GameOverWindow";
            this.Text = "GameOverForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox player1Box;
        private System.Windows.Forms.ListBox player2Box;
        private System.Windows.Forms.Label player2ScoreLabel;
        private System.Windows.Forms.Label player1ScoreLabel;
        private System.Windows.Forms.Label player2Label;
        private System.Windows.Forms.Label player1Label;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label1;
    }
}