using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Screenshot_OCR
{
    public partial class OverlayForm : Form
    {
        // Biến lưu ảnh gốc toàn màn hình
        public Bitmap OriginalScreenshot { get; set; }
        // Biến lưu kết quả cắt được để trả về cho Form1
        public Bitmap CroppedImage { get; private set; }

        private Point startPoint; // Điểm bắt đầu kéo chuột
        private Rectangle selectionRect; // Hình chữ nhật vùng chọn
        private bool isSelecting = false; // Cờ kiểm tra đang kéo hay không

        public OverlayForm()
        {
            InitializeComponent();
            // Cấu hình Form phủ kín màn hình
            this.FormBorderStyle = FormBorderStyle.None; // Bỏ viền
            this.WindowState = FormWindowState.Maximized; // Phóng to hết cỡ
            this.DoubleBuffered = true; // Chống giật lag khi vẽ
            this.Cursor = Cursors.Cross; // Đổi chuột thành hình chữ thập
            this.ShowInTaskbar = false;
        }

        // Khi Form hiện lên, gán ảnh chụp màn hình vào nền
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            this.BackgroundImage = OriginalScreenshot;
        }

        // 1. Bắt đầu ấn chuột xuống
        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isSelecting = true;
                startPoint = e.Location;
            }
        }

        // 2. Di chuột để vẽ hình chữ nhật
        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (isSelecting)
            {
                // Tính toán hình chữ nhật dựa trên vị trí chuột
                int x = Math.Min(startPoint.X, e.X);
                int y = Math.Min(startPoint.Y, e.Y);
                int width = Math.Abs(startPoint.X - e.X);
                int height = Math.Abs(startPoint.Y - e.Y);
                selectionRect = new Rectangle(x, y, width, height);

                this.Invalidate(); // Vẽ lại Form (gọi hàm OnPaint)
            }
        }

        // 3. Nhả chuột ra -> Cắt ảnh
        protected override void OnMouseUp(MouseEventArgs e)
        {
            if (isSelecting)
            {
                isSelecting = false;
                // Nếu vùng chọn có kích thước hợp lý (lớn hơn 0)
                if (selectionRect.Width > 0 && selectionRect.Height > 0)
                {
                    // Cắt ảnh từ ảnh gốc
                    CroppedImage = new Bitmap(selectionRect.Width, selectionRect.Height);
                    using (Graphics g = Graphics.FromImage(CroppedImage))
                    {
                        g.DrawImage(OriginalScreenshot,
                            new Rectangle(0, 0, CroppedImage.Width, CroppedImage.Height),
                            selectionRect,
                            GraphicsUnit.Pixel);
                    }
                    this.DialogResult = DialogResult.OK; // Báo OK
                }
                else
                {
                    this.DialogResult = DialogResult.Cancel; // Báo hủy nếu click nhầm
                }
                this.Close(); // Đóng form phủ
            }
        }

        // Vẽ hình chữ nhật màu đỏ lên màn hình
        protected override void OnPaint(PaintEventArgs e)
        {
            using (Pen pen = new Pen(Color.Red, 2))
            {
                if (selectionRect != null && selectionRect.Width > 0)
                {
                    e.Graphics.DrawRectangle(pen, selectionRect);

                    // (Optional) Vẽ lớp phủ mờ xung quanh vùng chọn cho đẹp (giống Zalo)
                    // Vùng trên
                    e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(100, 0, 0, 0)), 0, 0, this.Width, selectionRect.Y);
                    // Vùng dưới
                    e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(100, 0, 0, 0)), 0, selectionRect.Bottom, this.Width, this.Height - selectionRect.Bottom);
                    // Vùng trái
                    e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(100, 0, 0, 0)), 0, selectionRect.Y, selectionRect.X, selectionRect.Height);
                    // Vùng phải
                    e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(100, 0, 0, 0)), selectionRect.Right, selectionRect.Y, this.Width - selectionRect.Right, selectionRect.Height);
                }
                else
                {
                    // Khi chưa chọn gì thì phủ mờ toàn bộ
                    e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(100, 0, 0, 0)), this.ClientRectangle);
                }
            }
        }

        // Bấm ESC để hủy
        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.DialogResult = DialogResult.Cancel;
                this.Close();
            }
            base.OnKeyDown(e);
        }
    }
}
