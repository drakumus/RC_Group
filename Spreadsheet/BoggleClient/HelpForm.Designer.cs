﻿namespace BoggleClient
{
    partial class HelpForm
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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(141, 17);
            this.label1.TabIndex = 0;
            this.label1.Text = "Steps to play boggle:";
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(12, 50);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(568, 38);
            this.label2.TabIndex = 1;
            this.label2.Text = "1) Enter a name and server into the fields next to the respective labels. Once th" +
    "is is done press the connect button. ";
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(12, 107);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(568, 38);
            this.label3.TabIndex = 2;
            this.label3.Text = "2) Enter a desired time limit and click join to join a game.";
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(12, 211);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(568, 38);
            this.label4.TabIndex = 3;
            this.label4.Text = "3) Once Connected to a game your name and a player\'s name will be presented below" +
    " the text box for join game.";
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(12, 154);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(565, 40);
            this.label5.TabIndex = 4;
            this.label5.Text = "Note: At any point during the previous two steps (while available) you can click " +
    "cancel to cancel the server request.";
            // 
            // label6
            // 
            this.label6.Location = new System.Drawing.Point(12, 286);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(568, 52);
            this.label6.TabIndex = 5;
            this.label6.Text = "4) The timer will start counting down immediately. The player must add words to t" +
    "he text box by typing them in and pressing the \"Enter\" button being displayed ne" +
    "xt to the text box.";
            // 
            // label7
            // 
            this.label7.Location = new System.Drawing.Point(12, 370);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(568, 52);
            this.label7.TabIndex = 6;
            this.label7.Text = "5) Once the timer runs out a score will be displayed and the user can attempt to " +
    "join a new server or game.";
            // 
            // label8
            // 
            this.label8.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label8.Location = new System.Drawing.Point(-1, 198);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(588, 2);
            this.label8.TabIndex = 40;
            this.label8.Text = " ";
            // 
            // label9
            // 
            this.label9.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label9.Location = new System.Drawing.Point(-1, 145);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(588, 2);
            this.label9.TabIndex = 41;
            this.label9.Text = " ";
            // 
            // HelpForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(586, 430);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "HelpForm";
            this.Text = "Help";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
    }
}