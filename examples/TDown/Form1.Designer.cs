
namespace TDown
{
    partial class Form1
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.btn = new System.Windows.Forms.Button();
            this.txt_uri = new System.Windows.Forms.TextBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btn_suspend = new System.Windows.Forms.Button();
            this.btn_resume = new System.Windows.Forms.Button();
            this.btn_stop = new System.Windows.Forms.Button();
            this.label_prog = new System.Windows.Forms.Label();
            this.txt_max = new System.Windows.Forms.Label();
            this.txt_value = new System.Windows.Forms.Label();
            this.label_time = new System.Windows.Forms.Label();
            this.txt_speed = new System.Windows.Forms.Label();
            this.txt_state = new System.Windows.Forms.Label();
            this.panel_url = new System.Windows.Forms.TableLayoutPanel();
            this.progress = new System.Windows.Forms.ProgressBar();
            this.panel_info = new System.Windows.Forms.TableLayoutPanel();
            this.label_state = new System.Windows.Forms.Label();
            this.txt_prog = new System.Windows.Forms.Label();
            this.label_speed = new System.Windows.Forms.Label();
            this.txt_time = new System.Windows.Forms.Label();
            this.label_start_time = new System.Windows.Forms.Label();
            this.txt_start_time = new System.Windows.Forms.Label();
            this.txt_end_time = new System.Windows.Forms.Label();
            this.label_end_time = new System.Windows.Forms.Label();
            this.label_max = new System.Windows.Forms.Label();
            this.label_value = new System.Windows.Forms.Label();
            this.panel_btns = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.panel_url.SuspendLayout();
            this.panel_info.SuspendLayout();
            this.panel_btns.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // btn
            // 
            this.btn.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btn.Location = new System.Drawing.Point(740, 5);
            this.btn.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btn.Name = "btn";
            this.btn.Size = new System.Drawing.Size(78, 34);
            this.btn.TabIndex = 0;
            this.btn.Text = "下载";
            this.btn.UseVisualStyleBackColor = true;
            this.btn.Click += new System.EventHandler(this.btn_Click);
            // 
            // txt_uri
            // 
            this.txt_uri.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.txt_uri.Location = new System.Drawing.Point(4, 8);
            this.txt_uri.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txt_uri.Name = "txt_uri";
            this.txt_uri.Size = new System.Drawing.Size(728, 28);
            this.txt_uri.TabIndex = 1;
            this.txt_uri.Text = "https://github.com/Haku-Men/HttpLib.git";
            // 
            // textBox2
            // 
            this.textBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox2.Location = new System.Drawing.Point(114, 5);
            this.textBox2.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(458, 28);
            this.textBox2.TabIndex = 2;
            this.textBox2.Text = "2";
            // 
            // label1
            // 
            this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label1.Location = new System.Drawing.Point(4, 0);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(102, 38);
            this.label1.TabIndex = 0;
            this.label1.Text = "核心数：";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // btn_suspend
            // 
            this.btn_suspend.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btn_suspend.Enabled = false;
            this.btn_suspend.Location = new System.Drawing.Point(586, 5);
            this.btn_suspend.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btn_suspend.Name = "btn_suspend";
            this.btn_suspend.Size = new System.Drawing.Size(72, 34);
            this.btn_suspend.TabIndex = 3;
            this.btn_suspend.Text = "暂停";
            this.btn_suspend.UseVisualStyleBackColor = true;
            this.btn_suspend.Click += new System.EventHandler(this.btn_suspend_Click);
            // 
            // btn_resume
            // 
            this.btn_resume.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btn_resume.Enabled = false;
            this.btn_resume.Location = new System.Drawing.Point(666, 5);
            this.btn_resume.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btn_resume.Name = "btn_resume";
            this.btn_resume.Size = new System.Drawing.Size(72, 34);
            this.btn_resume.TabIndex = 3;
            this.btn_resume.Text = "继续";
            this.btn_resume.UseVisualStyleBackColor = true;
            this.btn_resume.Click += new System.EventHandler(this.btn_resume_Click);
            // 
            // btn_stop
            // 
            this.btn_stop.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btn_stop.Enabled = false;
            this.btn_stop.Location = new System.Drawing.Point(746, 5);
            this.btn_stop.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btn_stop.Name = "btn_stop";
            this.btn_stop.Size = new System.Drawing.Size(72, 34);
            this.btn_stop.TabIndex = 3;
            this.btn_stop.Text = "终止";
            this.btn_stop.UseVisualStyleBackColor = true;
            this.btn_stop.Click += new System.EventHandler(this.btn_stop_Click);
            // 
            // label_prog
            // 
            this.label_prog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label_prog.Location = new System.Drawing.Point(415, 0);
            this.label_prog.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label_prog.Name = "label_prog";
            this.label_prog.Size = new System.Drawing.Size(132, 38);
            this.label_prog.TabIndex = 0;
            this.label_prog.Text = "下载进度：";
            this.label_prog.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txt_max
            // 
            this.txt_max.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txt_max.Location = new System.Drawing.Point(144, 114);
            this.txt_max.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.txt_max.Name = "txt_max";
            this.txt_max.Size = new System.Drawing.Size(263, 39);
            this.txt_max.TabIndex = 0;
            this.txt_max.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txt_value
            // 
            this.txt_value.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txt_value.Location = new System.Drawing.Point(555, 114);
            this.txt_value.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.txt_value.Name = "txt_value";
            this.txt_value.Size = new System.Drawing.Size(263, 39);
            this.txt_value.TabIndex = 0;
            this.txt_value.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label_time
            // 
            this.label_time.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label_time.Location = new System.Drawing.Point(415, 38);
            this.label_time.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label_time.Name = "label_time";
            this.label_time.Size = new System.Drawing.Size(132, 38);
            this.label_time.TabIndex = 0;
            this.label_time.Text = "剩余时间：";
            this.label_time.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txt_speed
            // 
            this.txt_speed.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txt_speed.Location = new System.Drawing.Point(144, 38);
            this.txt_speed.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.txt_speed.Name = "txt_speed";
            this.txt_speed.Size = new System.Drawing.Size(263, 38);
            this.txt_speed.TabIndex = 0;
            this.txt_speed.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txt_state
            // 
            this.txt_state.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txt_state.Font = new System.Drawing.Font("Microsoft YaHei UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.txt_state.Location = new System.Drawing.Point(144, 0);
            this.txt_state.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.txt_state.Name = "txt_state";
            this.txt_state.Size = new System.Drawing.Size(263, 38);
            this.txt_state.TabIndex = 0;
            this.txt_state.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // panel_url
            // 
            this.panel_url.ColumnCount = 2;
            this.panel_url.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.panel_url.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 86F));
            this.panel_url.Controls.Add(this.btn, 1, 0);
            this.panel_url.Controls.Add(this.txt_uri, 0, 0);
            this.panel_url.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel_url.Location = new System.Drawing.Point(0, 0);
            this.panel_url.Margin = new System.Windows.Forms.Padding(4);
            this.panel_url.Name = "panel_url";
            this.panel_url.RowCount = 1;
            this.panel_url.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.panel_url.Size = new System.Drawing.Size(822, 44);
            this.panel_url.TabIndex = 0;
            // 
            // progress
            // 
            this.progress.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.progress.Location = new System.Drawing.Point(0, 241);
            this.progress.Name = "progress";
            this.progress.Size = new System.Drawing.Size(822, 16);
            this.progress.TabIndex = 0;
            // 
            // panel_info
            // 
            this.panel_info.ColumnCount = 4;
            this.panel_info.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 140F));
            this.panel_info.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.panel_info.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 140F));
            this.panel_info.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.panel_info.Controls.Add(this.txt_state, 1, 0);
            this.panel_info.Controls.Add(this.label_state, 0, 0);
            this.panel_info.Controls.Add(this.label_prog, 2, 0);
            this.panel_info.Controls.Add(this.txt_prog, 3, 0);
            this.panel_info.Controls.Add(this.label_speed, 0, 1);
            this.panel_info.Controls.Add(this.txt_speed, 1, 1);
            this.panel_info.Controls.Add(this.label_time, 2, 1);
            this.panel_info.Controls.Add(this.txt_time, 3, 1);
            this.panel_info.Controls.Add(this.label_start_time, 0, 2);
            this.panel_info.Controls.Add(this.txt_start_time, 1, 2);
            this.panel_info.Controls.Add(this.txt_end_time, 3, 2);
            this.panel_info.Controls.Add(this.label_end_time, 2, 2);
            this.panel_info.Controls.Add(this.label_max, 0, 3);
            this.panel_info.Controls.Add(this.txt_max, 1, 3);
            this.panel_info.Controls.Add(this.label_value, 2, 3);
            this.panel_info.Controls.Add(this.txt_value, 3, 3);
            this.panel_info.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel_info.Location = new System.Drawing.Point(0, 88);
            this.panel_info.Name = "panel_info";
            this.panel_info.RowCount = 4;
            this.panel_info.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.panel_info.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.panel_info.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.panel_info.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.panel_info.Size = new System.Drawing.Size(822, 153);
            this.panel_info.TabIndex = 0;
            // 
            // label_state
            // 
            this.label_state.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label_state.Location = new System.Drawing.Point(3, 0);
            this.label_state.Name = "label_state";
            this.label_state.Size = new System.Drawing.Size(134, 38);
            this.label_state.TabIndex = 0;
            this.label_state.Text = "状态：";
            this.label_state.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txt_prog
            // 
            this.txt_prog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txt_prog.Font = new System.Drawing.Font("Microsoft YaHei UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.txt_prog.Location = new System.Drawing.Point(555, 0);
            this.txt_prog.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.txt_prog.Name = "txt_prog";
            this.txt_prog.Size = new System.Drawing.Size(263, 38);
            this.txt_prog.TabIndex = 0;
            this.txt_prog.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label_speed
            // 
            this.label_speed.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label_speed.Location = new System.Drawing.Point(4, 38);
            this.label_speed.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label_speed.Name = "label_speed";
            this.label_speed.Size = new System.Drawing.Size(132, 38);
            this.label_speed.TabIndex = 0;
            this.label_speed.Text = "下载速度：";
            this.label_speed.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txt_time
            // 
            this.txt_time.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txt_time.Location = new System.Drawing.Point(555, 38);
            this.txt_time.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.txt_time.Name = "txt_time";
            this.txt_time.Size = new System.Drawing.Size(263, 38);
            this.txt_time.TabIndex = 0;
            this.txt_time.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label_start_time
            // 
            this.label_start_time.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label_start_time.Location = new System.Drawing.Point(4, 76);
            this.label_start_time.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label_start_time.Name = "label_start_time";
            this.label_start_time.Size = new System.Drawing.Size(132, 38);
            this.label_start_time.TabIndex = 0;
            this.label_start_time.Text = "开始时间：";
            this.label_start_time.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txt_start_time
            // 
            this.txt_start_time.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txt_start_time.Location = new System.Drawing.Point(144, 76);
            this.txt_start_time.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.txt_start_time.Name = "txt_start_time";
            this.txt_start_time.Size = new System.Drawing.Size(263, 38);
            this.txt_start_time.TabIndex = 0;
            this.txt_start_time.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txt_end_time
            // 
            this.txt_end_time.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txt_end_time.Location = new System.Drawing.Point(555, 76);
            this.txt_end_time.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.txt_end_time.Name = "txt_end_time";
            this.txt_end_time.Size = new System.Drawing.Size(263, 38);
            this.txt_end_time.TabIndex = 0;
            this.txt_end_time.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label_end_time
            // 
            this.label_end_time.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label_end_time.Location = new System.Drawing.Point(415, 76);
            this.label_end_time.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label_end_time.Name = "label_end_time";
            this.label_end_time.Size = new System.Drawing.Size(132, 38);
            this.label_end_time.TabIndex = 0;
            this.label_end_time.Text = "完成时间：";
            this.label_end_time.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label_max
            // 
            this.label_max.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label_max.Location = new System.Drawing.Point(4, 114);
            this.label_max.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label_max.Name = "label_max";
            this.label_max.Size = new System.Drawing.Size(132, 39);
            this.label_max.TabIndex = 0;
            this.label_max.Text = "Max：";
            this.label_max.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label_value
            // 
            this.label_value.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label_value.Location = new System.Drawing.Point(415, 114);
            this.label_value.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label_value.Name = "label_value";
            this.label_value.Size = new System.Drawing.Size(132, 39);
            this.label_value.TabIndex = 0;
            this.label_value.Text = "Value：";
            this.label_value.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // panel_btns
            // 
            this.panel_btns.ColumnCount = 4;
            this.panel_btns.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.panel_btns.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 80F));
            this.panel_btns.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 80F));
            this.panel_btns.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 80F));
            this.panel_btns.Controls.Add(this.tableLayoutPanel3, 0, 0);
            this.panel_btns.Controls.Add(this.btn_stop, 3, 0);
            this.panel_btns.Controls.Add(this.btn_resume, 2, 0);
            this.panel_btns.Controls.Add(this.btn_suspend, 1, 0);
            this.panel_btns.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel_btns.Location = new System.Drawing.Point(0, 44);
            this.panel_btns.Name = "panel_btns";
            this.panel_btns.RowCount = 1;
            this.panel_btns.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.panel_btns.Size = new System.Drawing.Size(822, 44);
            this.panel_btns.TabIndex = 1;
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.ColumnCount = 2;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 110F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.Controls.Add(this.textBox2, 1, 0);
            this.tableLayoutPanel3.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel3.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 1;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(576, 38);
            this.tableLayoutPanel3.TabIndex = 9;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 21F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(822, 257);
            this.Controls.Add(this.panel_info);
            this.Controls.Add(this.panel_btns);
            this.Controls.Add(this.progress);
            this.Controls.Add(this.panel_url);
            this.Font = new System.Drawing.Font("Microsoft YaHei UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.MinimumSize = new System.Drawing.Size(838, 296);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "下载文件DEMO";
            this.panel_url.ResumeLayout(false);
            this.panel_url.PerformLayout();
            this.panel_info.ResumeLayout(false);
            this.panel_btns.ResumeLayout(false);
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btn;
        private System.Windows.Forms.TextBox txt_uri;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btn_suspend;
        private System.Windows.Forms.Button btn_resume;
        private System.Windows.Forms.Button btn_stop;
        private System.Windows.Forms.Label label_prog;
        private System.Windows.Forms.Label txt_max;
        private System.Windows.Forms.Label txt_value;
        private System.Windows.Forms.Label label_time;
        private System.Windows.Forms.Label txt_speed;
        private System.Windows.Forms.Label txt_state;
        private System.Windows.Forms.TableLayoutPanel panel_url;
        private System.Windows.Forms.ProgressBar progress;
        private System.Windows.Forms.TableLayoutPanel panel_info;
        private System.Windows.Forms.Label label_state;
        private System.Windows.Forms.Label txt_prog;
        private System.Windows.Forms.Label txt_time;
        private System.Windows.Forms.Label label_speed;
        private System.Windows.Forms.Label label_max;
        private System.Windows.Forms.Label label_value;
        private System.Windows.Forms.TableLayoutPanel panel_btns;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.Label label_start_time;
        private System.Windows.Forms.Label txt_start_time;
        private System.Windows.Forms.Label txt_end_time;
        private System.Windows.Forms.Label label_end_time;
    }
}
