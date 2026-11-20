using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.ComponentModel;

namespace Glassculator
{
    public class RoundedButton : Button
    {
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int CornerRadius { get; set; } = 15;

        public RoundedButton()
        {
            FlatStyle = FlatStyle.Flat;
            FlatAppearance.BorderSize = 0;

            BackColor = Color.FromArgb(250, 250, 250);
            ForeColor = Color.FromArgb(65, 65, 75);

            Font = new Font(
                "Segoe UI",
                16,
                FontStyle.Regular
            );

            Cursor = Cursors.Hand;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics graphics = e.Graphics;
            graphics.Clear(Parent?.BackColor ?? SystemColors.Control);

            graphics.SmoothingMode =
                SmoothingMode.AntiAlias;

            Rectangle rectangle = new Rectangle(
                0, 0, Width - 1, Height - 1
            );

            using (GraphicsPath path = new GraphicsPath())
            {
                int radius = CornerRadius;
                int diameter = radius * 2;

                path.AddArc(
                    rectangle.X,
                    rectangle.Y,
                    diameter,
                    diameter,
                    180, 90
                );

                path.AddArc(
                    rectangle.Right - diameter,
                    rectangle.Y,
                    diameter,
                    diameter,
                    270, 90
                );

                path.AddArc(
                    rectangle.Right - diameter,
                    rectangle.Bottom - diameter,
                    diameter,
                    diameter,
                    0, 90
                );

                path.AddArc(
                    rectangle.X,
                    rectangle.Bottom - diameter,
                    diameter,
                    diameter,
                    90, 90
                );

                path.CloseFigure();

                using (SolidBrush brush =
                    new SolidBrush(BackColor))
                {
                    graphics.FillPath(brush, path);
                }

                using (SolidBrush textBrush =
                    new SolidBrush(ForeColor))
                {
                    StringFormat format = new StringFormat();

                    format.Alignment = StringAlignment.Center;
                    format.LineAlignment = StringAlignment.Center;

                    graphics.DrawString(
                        Text,
                        Font,
                        textBrush,
                        rectangle,
                        format
                    );
                }
            }
        }
    }
}