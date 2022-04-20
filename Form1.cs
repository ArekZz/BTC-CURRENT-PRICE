using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Threading;
using Leaf.xNet;
using System.Drawing.Drawing2D;

namespace BTC
{
    public partial class Form1 : Form
    {
        public void klik(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                this.Invoke((MethodInvoker)delegate { this.Close(); Application.Exit(); });
              
            }
            else//left or middle click
            {
                //do something here
            }
        }
        public class CustomLabel : Label
        {
            public CustomLabel()
            {
                OutlineForeColor = Color.Black;
                OutlineWidth =2;
            }
            public Color OutlineForeColor { get; set; }
            public float OutlineWidth { get; set; }
            protected override void OnPaint(PaintEventArgs e)
            {
                e.Graphics.FillRectangle(new SolidBrush(BackColor), ClientRectangle);
                using (GraphicsPath gp = new GraphicsPath())
                using (Pen outline = new Pen(OutlineForeColor, OutlineWidth)
                { LineJoin = LineJoin.Round })
                using (StringFormat sf = new StringFormat())
                using (Brush foreBrush = new SolidBrush(ForeColor))
                {
                    gp.AddString(Text, Font.FontFamily, (int)Font.Style,
                        Font.Size, ClientRectangle, sf);
                    e.Graphics.ScaleTransform(1.3f, 1.35f);
                    e.Graphics.SmoothingMode = SmoothingMode.None;
                    e.Graphics.DrawPath(outline, gp);
                    e.Graphics.FillPath(foreBrush, gp);
                }
            }
        }




        public double lastprice = 0;
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        private void Form1_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }
        private static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
    private const UInt32 SWP_NOSIZE = 0x0001;
    private const UInt32 SWP_NOMOVE = 0x0002;
    private const UInt32 TOPMOST_FLAGS = SWP_NOMOVE | SWP_NOSIZE;
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);
    public Form1()
        {
            ShowInTaskbar = false;
            SetWindowPos(this.Handle, HWND_TOPMOST, 0, 0, 0, 0, TOPMOST_FLAGS);
            InitializeComponent();
            this.label1.MouseUp += klik;
            Rectangle workingArea = Screen.GetWorkingArea(this);
            this.Location = new Point(workingArea.Right - Size.Width,
                                      workingArea.Bottom - Size.Height);
            this.label1.MouseDown+= Form1_MouseDown;
            void update()
            {
                while (true)
                {
                    try
                    {
                        using (var req = new HttpRequest())
                        {
                            var res = req.Get("https://api.coindesk.com/v1/bpi/currentprice/CNY.json").ToString();
                            var price = res.Substring("rate\":\"", "\",\"description").Split('.')[0].Trim();

                            if (lastprice > double.Parse(price))
                            {


                                label1.ForeColor = Color.Red;
                            }
                            else if (lastprice < double.Parse(price))
                            {


                                label1.ForeColor = Color.LimeGreen;
                            }
                            label1.Invoke((Action)delegate
                            {
                                label1.Text = price + " USD";
                            });
                            lastprice = double.Parse(price);
                        }
                    }
                    catch { }
                    Thread.Sleep(10000);
                }
            }
            new Thread(delegate () { update(); }) { IsBackground = true }.Start();

        }
        protected override void OnPaintBackground(PaintEventArgs e)
        {
            //empty implementation
        }


    }
}
