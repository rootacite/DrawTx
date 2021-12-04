using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using System.Drawing;
using Color = System.Drawing.Color;
using Pen = System.Drawing.Pen;
using System;
using Brushes = System.Drawing.Brushes;
using FontFamily = System.Drawing.FontFamily;

namespace DrawTx
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private Bitmap buffer = null;
        private WriteableBitmap wb = null;
        private  Graphics Graphics = null;

        List<PointF> pList = new List<PointF>();

        float x_limit = 35f;
        float y_limit = 300f;

        float x_min = 0;
        float y_min = 295f;

        /// <summary>
        /// 
        /// </summary>
        /// 

        private Bitmap cb = null;
        private WriteableBitmap cwb = null;
        private Graphics cg = null;

        public MainWindow()
        {
            InitializeComponent();

            Loaded += (e, v) =>
            {
                //   for(int i = 1; i <= 1280; i++)
                //   {
                pList.Add(new PointF(0, 295.2f));
                pList.Add(new PointF(5, 295.85f));
                pList.Add(new PointF(10, 296.05f));
                pList.Add(new PointF(15, 296.75f));
                pList.Add(new PointF(20, 297.25f));
                pList.Add(new PointF(25, 297.85f));
                pList.Add(new PointF(30, 298.5f));

                //   }
               
                wb = new WriteableBitmap(1280, 720, 96, 96, PixelFormats.Pbgra32, null);
                buffer = new Bitmap(1280, 720, wb.BackBufferStride, System.Drawing.Imaging.PixelFormat.Format32bppPArgb, wb.BackBuffer);
                Graphics = Graphics.FromImage(buffer);

                cwb = new WriteableBitmap(1280, 720, 96, 96, PixelFormats.Pbgra32, null);
                cb = new Bitmap(1280, 720, cwb.BackBufferStride, System.Drawing.Imaging.PixelFormat.Format32bppPArgb, cwb.BackBuffer);
                cg = Graphics.FromImage(cb);

                Plant.Source = wb;
                control.Source = cwb;

                wb.Lock();
                Graphics.Clear(Color.Transparent);

                List<PointF> Lpm = new List<PointF>();
                foreach (var i in pList)
                {
                    var ps = CanvToPlant(i);
                    Lpm.Add(new PointF(ps.X, 720 - ps.Y));
                }

                Graphics.DrawCurve(new Pen(Color.Red, 1.5f), Lpm.ToArray());

                Graphics.DrawLine(new Pen(Color.Blue, 4f), 1, 0, 1, 720);
                Graphics.DrawLine(new Pen(Color.Blue, 4f), 1, 720, 1280, 720);

                wb.AddDirtyRect(new Int32Rect(0, 0, 1280, 720));
                wb.Unlock();

            };


        }

        PointF CanvToPlant(PointF c)
        {
            float x_length = x_limit - x_min;
            float x_rate = (c.X - x_min) / x_length;

            float y_length = y_limit - y_min;
            float y_rate = (c.Y - y_min) / y_length;

            return new PointF(1280f * x_rate, 720f * y_rate);
        }

        float distance(PointF p1,PointF p2)
        {
            return (float)Math.Sqrt((p1.X - p2.X) * (p1.X - p2.X) + (p1.Y - p2.Y) * (p1.Y - p2.Y));
        }

        private void Window_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            var p = e.GetPosition(Sysopt);
            cwb.Lock();

            cg.Clear(Color.Transparent);
            using (SolidBrush sb = new SolidBrush(Color.Green))
                cg.FillEllipse(sb, new Rectangle((int)(p.X - 3), (int)(p.Y - 3), 6, 6));
            cg.DrawLine(new Pen(Color.Blue), (float)0, (float)p.Y, (float)p.X, (float)p.Y);
            cg.DrawLine(new Pen(Color.Blue), (float)p.X, (float)720, (float)p.X, (float)p.Y);

            Dictionary<float, PointF> vcp = new Dictionary<float, PointF>();

            foreach(var i in pList)
            {
                PointF op = CanvToPlant(i);
                op.Y = 720 - op.Y;
                vcp.Add(distance(op, new PointF((float)p.X, (float)p.Y)), i);
            }

            List<float> vas = new List<float>();
            foreach(var i in vcp)
            {
                vas.Add(i.Key);

                PointF cop = CanvToPlant(i.Value);
                cop.Y = 720 - cop.Y;
                using (SolidBrush sb = new SolidBrush(Color.Green))
                    cg.FillEllipse(sb, new Rectangle((int)(cop.X - 5), (int)(cop.Y - 5), 10, 10));
            }
            vas.Sort();
            Title = vcp[vas[0]].ToString();

            


            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Near;
            sf.LineAlignment = StringAlignment.Center;
            using (SolidBrush sb = new SolidBrush(Color.Black)) foreach (var i in pList)
                {
                    PointF op = CanvToPlant(i);
                    op.Y = 720 - op.Y;
                    cg.DrawString(i.X.ToString()+ ","+i.Y.ToString(), new System.Drawing.Font(new FontFamily("宋体"), 8, System.Drawing.FontStyle.Regular), sb, new Rectangle((int)(op.X - 160), (int)(op.Y - 20), 320, 40), sf);

                }
            //一定要用Rectangel的重构函数，



            cwb.AddDirtyRect(new Int32Rect(0, 0, 1280, 720));
            cwb.Unlock();
        }
    }
}
