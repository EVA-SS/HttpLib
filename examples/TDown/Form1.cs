using HttpLib;
using System;
using System.Windows.Forms;

namespace TDown
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        HttpDown down;
        private void btn_Click(object sender, EventArgs e)
        {
            progress.Maximum = progress.Value = 0;
            txt_state.Text = txt_speed.Text = txt_time.Text = txt_max.Text = txt_value.Text = txt_start_time.Text = txt_end_time.Text = null;
            int maxThreads = Environment.ProcessorCount;
            string downUrl = txt_uri.Text;
            if (int.TryParse(textBox2.Text, out int taskCount))
            {
                DateTime start_time = DateTime.Now;
                txt_start_time.Text = start_time.ToString("HH:mm:ss");
                down = Http.Get(downUrl).redirect().downLoad(Program.BasePath);
                btn.Enabled = btn_resume.Enabled = false;
                btn_suspend.Enabled = btn_stop.Enabled = true;
                down.ValueChange(t =>
                {
                    txt_max.Invoke(new Action(() =>
                    {
                        if (down.MaxValue > t)
                        {
                            progress.Value = (int)t;
                            txt_prog.Text = Math.Round(t / down.MaxValue * 100.0, 1) + "%";
                        }
                        txt_value.Text = ByteUnit(t);
                    }));
                });
                down.MaxValueChange(t =>
                {
                    if (t > 0)
                    {
                        txt_max.Invoke(new Action(() =>
                        {
                            progress.Maximum = (int)t;
                            txt_prog.Text = Math.Round(down.Value / t * 100.0, 1) + "%";
                            txt_max.Text = ByteUnit(t);
                        }));
                    }
                    else
                    {
                        txt_max.Invoke(new Action(() =>
                        {
                            txt_prog.Text = "∞";
                            txt_max.Text = "未知";
                        }));
                    }
                });
                down.StateChange((t, err) =>
                {
                    txt_state.Invoke(new Action(() =>
                    {
                        switch (t)
                        {
                            case HttpDown.DownState.Complete:
                                txt_state.Text = "完成 " + err;
                                break;
                            case HttpDown.DownState.Downloading:
                                txt_state.Text = "下载中";
                                break;
                            case HttpDown.DownState.Fail:
                                txt_state.Text = "异常 " + err;
                                break;
                            case HttpDown.DownState.Stop:
                                txt_state.Text = "已停止 " + err;
                                break;
                        }
                    }));
                });
                down.TimeChange(t =>
                {
                    txt_time.Invoke(new Action(() =>
                    {
                        txt_time.Text = t;
                    }));
                });
                down.SpeedChange(t =>
                {
                    txt_speed.Invoke(new Action(() =>
                    {
                        txt_start_time.Text = start_time.ToString("HH:mm:ss") + " | 耗时 " + Math.Round((DateTime.Now - start_time).TotalSeconds) + "秒";
                        txt_speed.Text = ByteUnit(t);
                    }));
                });
                down.Go(taskCount).ContinueWith((action =>
                {
                    DateTime end_time = DateTime.Now;
                    Invoke(new Action(() =>
                    {
                        txt_start_time.Text = start_time.ToString("HH:mm:ss");
                        txt_end_time.Text = end_time.ToString("HH:mm:ss") + " | 耗时 " + Math.Round((end_time - start_time).TotalSeconds) + "秒";
                        btn_stop.Enabled = btn_suspend.Enabled = btn_resume.Enabled = false;
                        btn.Enabled = true;
                    }));
                    System.Diagnostics.Debug.WriteLine("保存至：" + action.Result);
                    if (action.Result != null)
                        MessageBox.Show("保存至：" + action.Result);
                }));
            }
        }

        private void btn_suspend_Click(object sender, EventArgs e)
        {
            btn_suspend.Enabled = false;
            btn_resume.Enabled = true;
            down.Suspend();
        }

        private void btn_resume_Click(object sender, EventArgs e)
        {
            btn_resume.Enabled = false;
            btn_suspend.Enabled = true;
            down.Resume();
        }

        private void btn_stop_Click(object sender, EventArgs e)
        {
            down.Dispose();
            btn_stop.Enabled = btn_suspend.Enabled = btn_resume.Enabled = false;
            btn.Enabled = true;
        }

        #region 转换

        public static string ByteUnit(long val, int d = 1, string nul = "0B")
        {
            return ByteUnit(val * 1.0, d, nul);
        }
        public static string ByteUnit(double val, int d = 1, string nul = "0B")
        {
            if (val == 0) return nul;
            var _val = val;
            int unit = 0;
            while (_val > 1024)
            {
                _val /= 1024;
                unit++;
                if (unit > 5)
                {
                    break;
                }
            }
            return Math.Round(_val, d) + CountSizeUnit(unit);
        }

        static string CountSizeUnit(int val)
        {
            switch (val)
            {
                case 4: return "T";
                case 3: return "G";
                case 2: return "M";
                case 1: return "K";
                case 5: return "P";
                case 6: return "E";
                //case 7: return "Z";
                //case 8: return "Y";
                default: return "B";
            }
        }

        #endregion
    }
}
