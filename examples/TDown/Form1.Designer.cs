
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
            btn = new AntdUI.Button();
            txt_uri = new AntdUI.Input();
            textBox2 = new AntdUI.InputNumber();
            label1 = new AntdUI.Label();
            btn_suspend = new AntdUI.Button();
            btn_resume = new AntdUI.Button();
            btn_stop = new AntdUI.Button();
            label_prog = new AntdUI.Label();
            txt_max = new AntdUI.Label();
            txt_value = new AntdUI.Label();
            label_time = new AntdUI.Label();
            txt_speed = new AntdUI.Label();
            txt_state = new AntdUI.Badge();
            panel_url = new System.Windows.Forms.TableLayoutPanel();
            progress = new AntdUI.Progress();
            panel_info = new System.Windows.Forms.TableLayoutPanel();
            label_state = new AntdUI.Label();
            txt_prog = new AntdUI.Label();
            label_speed = new AntdUI.Label();
            txt_time = new AntdUI.Label();
            label_start_time = new AntdUI.Label();
            txt_start_time = new AntdUI.Label();
            txt_end_time = new AntdUI.Label();
            label_end_time = new AntdUI.Label();
            label_max = new AntdUI.Label();
            label_value = new AntdUI.Label();
            panel_btns = new System.Windows.Forms.TableLayoutPanel();
            panel_url.SuspendLayout();
            progress.SuspendLayout();
            panel_info.SuspendLayout();
            panel_btns.SuspendLayout();
            SuspendLayout();
            // 
            // btn
            // 
            btn.Dock = System.Windows.Forms.DockStyle.Fill;
            btn.Location = new System.Drawing.Point(739, 3);
            btn.Name = "btn";
            btn.Size = new System.Drawing.Size(80, 38);
            btn.TabIndex = 0;
            btn.Text = "下载";
            btn.Type = AntdUI.TTypeMini.Primary;
            btn.Click += btn_Click;
            // 
            // txt_uri
            // 
            txt_uri.Back = System.Drawing.Color.Transparent;
            txt_uri.BackColor = System.Drawing.Color.Transparent;
            txt_uri.BorderWidth = 0F;
            txt_uri.Dock = System.Windows.Forms.DockStyle.Fill;
            txt_uri.Fore = System.Drawing.Color.White;
            txt_uri.ForeColor = System.Drawing.Color.White;
            txt_uri.Location = new System.Drawing.Point(0, 0);
            txt_uri.Name = "txt_uri";
            txt_uri.Size = new System.Drawing.Size(730, 38);
            txt_uri.TabIndex = 1;
            txt_uri.Text = "https://github.com/Haku-Men/HttpLib.git";
            // 
            // textBox2
            // 
            textBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            textBox2.Location = new System.Drawing.Point(103, 3);
            textBox2.Name = "textBox2";
            textBox2.Size = new System.Drawing.Size(476, 38);
            textBox2.TabIndex = 2;
            textBox2.Text = "2";
            textBox2.Value = new decimal(new int[] { 2, 0, 0, 0 });
            // 
            // label1
            // 
            label1.Dock = System.Windows.Forms.DockStyle.Fill;
            label1.Location = new System.Drawing.Point(0, 0);
            label1.Margin = new System.Windows.Forms.Padding(0);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(100, 44);
            label1.TabIndex = 0;
            label1.Text = "核心数：";
            label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // btn_suspend
            // 
            btn_suspend.Dock = System.Windows.Forms.DockStyle.Fill;
            btn_suspend.Enabled = false;
            btn_suspend.Location = new System.Drawing.Point(585, 3);
            btn_suspend.Name = "btn_suspend";
            btn_suspend.Size = new System.Drawing.Size(74, 38);
            btn_suspend.TabIndex = 3;
            btn_suspend.Text = "暂停";
            btn_suspend.Type = AntdUI.TTypeMini.Warn;
            btn_suspend.Click += btn_suspend_Click;
            // 
            // btn_resume
            // 
            btn_resume.Dock = System.Windows.Forms.DockStyle.Fill;
            btn_resume.Enabled = false;
            btn_resume.Location = new System.Drawing.Point(665, 3);
            btn_resume.Name = "btn_resume";
            btn_resume.Size = new System.Drawing.Size(74, 38);
            btn_resume.TabIndex = 3;
            btn_resume.Text = "继续";
            btn_resume.Type = AntdUI.TTypeMini.Success;
            btn_resume.Click += btn_resume_Click;
            // 
            // btn_stop
            // 
            btn_stop.Dock = System.Windows.Forms.DockStyle.Fill;
            btn_stop.Enabled = false;
            btn_stop.Location = new System.Drawing.Point(745, 3);
            btn_stop.Name = "btn_stop";
            btn_stop.Size = new System.Drawing.Size(74, 38);
            btn_stop.TabIndex = 3;
            btn_stop.Text = "终止";
            btn_stop.Type = AntdUI.TTypeMini.Error;
            btn_stop.Click += btn_stop_Click;
            // 
            // label_prog
            // 
            label_prog.Dock = System.Windows.Forms.DockStyle.Fill;
            label_prog.Location = new System.Drawing.Point(411, 0);
            label_prog.Margin = new System.Windows.Forms.Padding(0);
            label_prog.Name = "label_prog";
            label_prog.Size = new System.Drawing.Size(140, 42);
            label_prog.TabIndex = 0;
            label_prog.Text = "下载进度：";
            label_prog.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txt_max
            // 
            txt_max.Dock = System.Windows.Forms.DockStyle.Fill;
            txt_max.Location = new System.Drawing.Point(140, 126);
            txt_max.Margin = new System.Windows.Forms.Padding(0);
            txt_max.Name = "txt_max";
            txt_max.Size = new System.Drawing.Size(271, 43);
            txt_max.TabIndex = 0;
            // 
            // txt_value
            // 
            txt_value.Dock = System.Windows.Forms.DockStyle.Fill;
            txt_value.Location = new System.Drawing.Point(551, 126);
            txt_value.Margin = new System.Windows.Forms.Padding(0);
            txt_value.Name = "txt_value";
            txt_value.Size = new System.Drawing.Size(271, 43);
            txt_value.TabIndex = 0;
            // 
            // label_time
            // 
            label_time.Dock = System.Windows.Forms.DockStyle.Fill;
            label_time.Location = new System.Drawing.Point(411, 42);
            label_time.Margin = new System.Windows.Forms.Padding(0);
            label_time.Name = "label_time";
            label_time.Size = new System.Drawing.Size(140, 42);
            label_time.TabIndex = 0;
            label_time.Text = "剩余时间：";
            label_time.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txt_speed
            // 
            txt_speed.Dock = System.Windows.Forms.DockStyle.Fill;
            txt_speed.Location = new System.Drawing.Point(140, 42);
            txt_speed.Margin = new System.Windows.Forms.Padding(0);
            txt_speed.Name = "txt_speed";
            txt_speed.Size = new System.Drawing.Size(271, 42);
            txt_speed.TabIndex = 0;
            // 
            // txt_state
            // 
            txt_state.Dock = System.Windows.Forms.DockStyle.Fill;
            txt_state.Location = new System.Drawing.Point(140, 0);
            txt_state.Margin = new System.Windows.Forms.Padding(0);
            txt_state.Name = "txt_state";
            txt_state.Size = new System.Drawing.Size(271, 42);
            txt_state.TabIndex = 0;
            txt_state.Text = "待下载";
            // 
            // panel_url
            // 
            panel_url.ColumnCount = 2;
            panel_url.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            panel_url.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 86F));
            panel_url.Controls.Add(progress, 0, 0);
            panel_url.Controls.Add(btn, 1, 0);
            panel_url.Dock = System.Windows.Forms.DockStyle.Top;
            panel_url.Location = new System.Drawing.Point(0, 0);
            panel_url.Margin = new System.Windows.Forms.Padding(4);
            panel_url.Name = "panel_url";
            panel_url.RowCount = 1;
            panel_url.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            panel_url.Size = new System.Drawing.Size(822, 44);
            panel_url.TabIndex = 0;
            // 
            // progress
            // 
            progress.Back = System.Drawing.Color.Gray;
            progress.Controls.Add(txt_uri);
            progress.Dock = System.Windows.Forms.DockStyle.Fill;
            progress.Location = new System.Drawing.Point(3, 3);
            progress.Name = "progress";
            progress.Shape = AntdUI.TShape.Default;
            progress.Size = new System.Drawing.Size(730, 38);
            progress.TabIndex = 0;
            // 
            // panel_info
            // 
            panel_info.ColumnCount = 4;
            panel_info.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 140F));
            panel_info.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            panel_info.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 140F));
            panel_info.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            panel_info.Controls.Add(txt_state, 1, 0);
            panel_info.Controls.Add(label_state, 0, 0);
            panel_info.Controls.Add(label_prog, 2, 0);
            panel_info.Controls.Add(txt_prog, 3, 0);
            panel_info.Controls.Add(label_speed, 0, 1);
            panel_info.Controls.Add(txt_speed, 1, 1);
            panel_info.Controls.Add(label_time, 2, 1);
            panel_info.Controls.Add(txt_time, 3, 1);
            panel_info.Controls.Add(label_start_time, 0, 2);
            panel_info.Controls.Add(txt_start_time, 1, 2);
            panel_info.Controls.Add(txt_end_time, 3, 2);
            panel_info.Controls.Add(label_end_time, 2, 2);
            panel_info.Controls.Add(label_max, 0, 3);
            panel_info.Controls.Add(txt_max, 1, 3);
            panel_info.Controls.Add(label_value, 2, 3);
            panel_info.Controls.Add(txt_value, 3, 3);
            panel_info.Dock = System.Windows.Forms.DockStyle.Fill;
            panel_info.Location = new System.Drawing.Point(0, 88);
            panel_info.Name = "panel_info";
            panel_info.RowCount = 4;
            panel_info.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            panel_info.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            panel_info.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            panel_info.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            panel_info.Size = new System.Drawing.Size(822, 169);
            panel_info.TabIndex = 0;
            // 
            // label_state
            // 
            label_state.Dock = System.Windows.Forms.DockStyle.Fill;
            label_state.Location = new System.Drawing.Point(0, 0);
            label_state.Margin = new System.Windows.Forms.Padding(0);
            label_state.Name = "label_state";
            label_state.Size = new System.Drawing.Size(140, 42);
            label_state.TabIndex = 0;
            label_state.Text = "状态：";
            label_state.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txt_prog
            // 
            txt_prog.Dock = System.Windows.Forms.DockStyle.Fill;
            txt_prog.Font = new System.Drawing.Font("Microsoft YaHei UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            txt_prog.Location = new System.Drawing.Point(551, 0);
            txt_prog.Margin = new System.Windows.Forms.Padding(0);
            txt_prog.Name = "txt_prog";
            txt_prog.Size = new System.Drawing.Size(271, 42);
            txt_prog.TabIndex = 0;
            // 
            // label_speed
            // 
            label_speed.Dock = System.Windows.Forms.DockStyle.Fill;
            label_speed.Location = new System.Drawing.Point(0, 42);
            label_speed.Margin = new System.Windows.Forms.Padding(0);
            label_speed.Name = "label_speed";
            label_speed.Size = new System.Drawing.Size(140, 42);
            label_speed.TabIndex = 0;
            label_speed.Text = "下载速度：";
            label_speed.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txt_time
            // 
            txt_time.Dock = System.Windows.Forms.DockStyle.Fill;
            txt_time.Location = new System.Drawing.Point(551, 42);
            txt_time.Margin = new System.Windows.Forms.Padding(0);
            txt_time.Name = "txt_time";
            txt_time.Size = new System.Drawing.Size(271, 42);
            txt_time.TabIndex = 0;
            // 
            // label_start_time
            // 
            label_start_time.Dock = System.Windows.Forms.DockStyle.Fill;
            label_start_time.Location = new System.Drawing.Point(0, 84);
            label_start_time.Margin = new System.Windows.Forms.Padding(0);
            label_start_time.Name = "label_start_time";
            label_start_time.Size = new System.Drawing.Size(140, 42);
            label_start_time.TabIndex = 0;
            label_start_time.Text = "开始时间：";
            label_start_time.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txt_start_time
            // 
            txt_start_time.Dock = System.Windows.Forms.DockStyle.Fill;
            txt_start_time.Location = new System.Drawing.Point(140, 84);
            txt_start_time.Margin = new System.Windows.Forms.Padding(0);
            txt_start_time.Name = "txt_start_time";
            txt_start_time.Size = new System.Drawing.Size(271, 42);
            txt_start_time.TabIndex = 0;
            // 
            // txt_end_time
            // 
            txt_end_time.Dock = System.Windows.Forms.DockStyle.Fill;
            txt_end_time.Location = new System.Drawing.Point(551, 84);
            txt_end_time.Margin = new System.Windows.Forms.Padding(0);
            txt_end_time.Name = "txt_end_time";
            txt_end_time.Size = new System.Drawing.Size(271, 42);
            txt_end_time.TabIndex = 0;
            // 
            // label_end_time
            // 
            label_end_time.Dock = System.Windows.Forms.DockStyle.Fill;
            label_end_time.Location = new System.Drawing.Point(411, 84);
            label_end_time.Margin = new System.Windows.Forms.Padding(0);
            label_end_time.Name = "label_end_time";
            label_end_time.Size = new System.Drawing.Size(140, 42);
            label_end_time.TabIndex = 0;
            label_end_time.Text = "完成时间：";
            label_end_time.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label_max
            // 
            label_max.Dock = System.Windows.Forms.DockStyle.Fill;
            label_max.Location = new System.Drawing.Point(0, 126);
            label_max.Margin = new System.Windows.Forms.Padding(0);
            label_max.Name = "label_max";
            label_max.Size = new System.Drawing.Size(140, 43);
            label_max.TabIndex = 0;
            label_max.Text = "Max：";
            label_max.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label_value
            // 
            label_value.Dock = System.Windows.Forms.DockStyle.Fill;
            label_value.Location = new System.Drawing.Point(411, 126);
            label_value.Margin = new System.Windows.Forms.Padding(0);
            label_value.Name = "label_value";
            label_value.Size = new System.Drawing.Size(140, 43);
            label_value.TabIndex = 0;
            label_value.Text = "Value：";
            label_value.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // panel_btns
            // 
            panel_btns.ColumnCount = 5;
            panel_btns.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            panel_btns.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            panel_btns.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 80F));
            panel_btns.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 80F));
            panel_btns.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 80F));
            panel_btns.Controls.Add(textBox2, 1, 0);
            panel_btns.Controls.Add(btn_stop, 4, 0);
            panel_btns.Controls.Add(label1, 0, 0);
            panel_btns.Controls.Add(btn_resume, 3, 0);
            panel_btns.Controls.Add(btn_suspend, 2, 0);
            panel_btns.Dock = System.Windows.Forms.DockStyle.Top;
            panel_btns.Location = new System.Drawing.Point(0, 44);
            panel_btns.Name = "panel_btns";
            panel_btns.RowCount = 1;
            panel_btns.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            panel_btns.Size = new System.Drawing.Size(822, 44);
            panel_btns.TabIndex = 1;
            // 
            // Form1
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(10F, 21F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(822, 257);
            Controls.Add(panel_info);
            Controls.Add(panel_btns);
            Controls.Add(panel_url);
            Font = new System.Drawing.Font("Microsoft YaHei UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            MinimumSize = new System.Drawing.Size(838, 296);
            Name = "Form1";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            Text = "下载文件DEMO";
            panel_url.ResumeLayout(false);
            progress.ResumeLayout(false);
            panel_info.ResumeLayout(false);
            panel_btns.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private AntdUI.Button btn;
        private AntdUI.Input txt_uri;
        private AntdUI.InputNumber textBox2;
        private AntdUI.Label label1;
        private AntdUI.Button btn_suspend;
        private AntdUI.Button btn_resume;
        private AntdUI.Button btn_stop;
        private AntdUI.Label label_prog;
        private AntdUI.Label txt_max;
        private AntdUI.Label txt_value;
        private AntdUI.Label label_time;
        private AntdUI.Label txt_speed;
        private AntdUI.Badge txt_state;
        private System.Windows.Forms.TableLayoutPanel panel_url;
        private AntdUI.Progress progress;
        private System.Windows.Forms.TableLayoutPanel panel_info;
        private AntdUI.Label label_state;
        private AntdUI.Label txt_prog;
        private AntdUI.Label txt_time;
        private AntdUI.Label label_speed;
        private AntdUI.Label label_max;
        private AntdUI.Label label_value;
        private System.Windows.Forms.TableLayoutPanel panel_btns;
        private AntdUI.Label label_start_time;
        private AntdUI.Label txt_start_time;
        private AntdUI.Label txt_end_time;
        private AntdUI.Label label_end_time;
    }
}
