using System;
using System.Drawing;
using System.Windows.Forms;
using Tesseract; 
using System.IO; 
using System.Drawing.Imaging; 

namespace Screenshot_OCR
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            this.Text = "Tool Trích Xuất Chữ - Lại Thế Vinh";
            this.StartPosition = FormStartPosition.CenterScreen;
        }
        // Hàm phóng to ảnh để tăng độ nét cho Tesseract đọc
        private Bitmap ScaleImage(Bitmap image, float scaleFactor)
        {
            int newWidth = (int)(image.Width * scaleFactor);
            int newHeight = (int)(image.Height * scaleFactor);

            var newImage = new Bitmap(newWidth, newHeight);

            using (Graphics g = Graphics.FromImage(newImage))
            {
                // Chế độ nội suy chất lượng cao (làm mượt ảnh khi phóng to)
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;

                g.DrawImage(image, 0, 0, newWidth, newHeight);
            }

            return newImage;
        }
        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                // 1. Ẩn Form chính đi
                this.Hide();
                System.Threading.Thread.Sleep(200); // Chờ xíu cho nó ẩn hẳn

                // 2. Chụp toàn bộ màn hình trước
                Bitmap fullScreen = new Bitmap(Screen.PrimaryScreen.Bounds.Width,
                                               Screen.PrimaryScreen.Bounds.Height);
                using (Graphics g = Graphics.FromImage(fullScreen))
                {
                    g.CopyFromScreen(0, 0, 0, 0, fullScreen.Size);
                }

                // 3. Mở cái OverlayForm lên để người dùng kéo chuột
                using (OverlayForm overlay = new OverlayForm())
                {
                    overlay.OriginalScreenshot = fullScreen; // Truyền ảnh chụp vào

                    // Chờ người dùng kéo xong và bấm OK
                    if (overlay.ShowDialog() == DialogResult.OK)
                    {
                        // Lấy ảnh đã cắt
                        Bitmap resultImage = overlay.CroppedImage;
                        pictureBox2.Image = resultImage; // Hiện lên PictureBox
                        this.Show(); // Hiện lại tool chính

                        // --- GỌI TESSERACT ĐỂ ĐỌC CHỮ ---
                        try
                        {
                            using (Bitmap scaledImage = ScaleImage(resultImage, 3.0f))
                            {
                                using (var stream = new System.IO.MemoryStream())
                                {
                                    // Lưu ảnh ĐÃ PHÓNG TO vào RAM để đọc
                                    scaledImage.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                                    stream.Position = 0;

                                    using (var img = Tesseract.Pix.LoadFromMemory(stream.ToArray()))
                                    {
                                        // Nhớ dùng "vie" nhé (đảm bảo đã chép file data 'best' vào)
                                        using (var engine = new Tesseract.TesseractEngine(@"./tessdata", "vie", Tesseract.EngineMode.Default))
                                        {
                                            // Mẹo: Set biến này để Tesseract chỉ nhận diện ký tự (bỏ qua nhiễu)
                                            engine.SetVariable("preserve_interword_spaces", "1");

                                            using (var page = engine.Process(img))
                                            {
                                                richTextBox1.Text = page.GetText();
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Lỗi đọc chữ (có thể do chưa tải file data): " + ex.Message);
                        }
                    }
                    else
                    {
                        // Nếu người dùng bấm ESC hoặc không chọn gì
                        this.Show();
                    }
                }
            }
            catch (Exception ex)
            {
                this.Show();
                MessageBox.Show("Lỗi: " + ex.Message);
            }
        }
        //LƯU ẢNH ĐÃ CHỤP
        private void btnImage_Click(object sender, EventArgs e)
        {
            // 1. Kiểm tra xem có ảnh chưa
            if (pictureBox2.Image == null)
            {
                MessageBox.Show("Chưa có ảnh nào để lưu cả sếp ơi!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 2. Tạo hộp thoại Lưu File
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Title = "Lưu ảnh chụp màn hình";
            // Cho phép chọn đuôi PNG hoặc JPG
            sfd.Filter = "PNG Image (*.png)|*.png|JPEG Image (*.jpg)|*.jpg|Bitmap Image (*.bmp)|*.bmp";
            sfd.FileName = "Screenshot_" + DateTime.Now.ToString("yyyyMMdd_HHmmss"); // Tên file mặc định theo giờ

            // 3. Nếu người dùng bấm OK thì lưu
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    // Lưu ảnh vào đường dẫn đã chọn
                    pictureBox2.Image.Save(sfd.FileName);
                    MessageBox.Show("Đã lưu ảnh thành công!", "Thông báo");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi lưu ảnh: " + ex.Message);
                }
            }
        }
        // --- LOGIC LƯU TEXT ---
        private void btnFile_Click(object sender, EventArgs e)
        {
            // 1. Kiểm tra xem có chữ chưa
            if (string.IsNullOrWhiteSpace(richTextBox1.Text))
            {
                MessageBox.Show("Hộp text đang trống trơn, không có gì để lưu!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 2. Tạo hộp thoại Lưu File
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Title = "Lưu văn bản trích xuất";
            sfd.Filter = "Text File (*.txt)|*.txt"; // Chỉ cho lưu file txt
            sfd.FileName = "OCR_Text_" + DateTime.Now.ToString("yyyyMMdd_HHmmss");

            // 3. Nếu người dùng bấm OK thì lưu
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    // Ghi nội dung từ RichTextBox vào file
                    File.WriteAllText(sfd.FileName, richTextBox1.Text);
                    MessageBox.Show("Đã lưu file text thành công!", "Thông báo");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi lưu text: " + ex.Message);
                }
            }
        }
    }
}
