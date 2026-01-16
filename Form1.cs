using System;
using System.Drawing;
using System.Windows.Forms;
using Tesseract; 
using System.IO; 
using System.Drawing.Imaging;
using Windows.Media.Ocr;
using Windows.Globalization;

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

        private async void button1_Click(object sender, EventArgs e)
        {
            try
            {
                // 1. Ẩn Form chính
                this.Hide();
                // Dùng Task.Delay thay cho Thread.Sleep để không bị đơ UI
                await System.Threading.Tasks.Task.Delay(200);

                // 2. Chụp màn hình
                Bitmap fullScreen = new Bitmap(Screen.PrimaryScreen.Bounds.Width,
                                               Screen.PrimaryScreen.Bounds.Height);
                using (Graphics g = Graphics.FromImage(fullScreen))
                {
                    g.CopyFromScreen(0, 0, 0, 0, fullScreen.Size);
                }

                // 3. Mở OverlayForm
                using (OverlayForm overlay = new OverlayForm())
                {
                    overlay.OriginalScreenshot = fullScreen;

                    if (overlay.ShowDialog() == DialogResult.OK)
                    {
                        // Lấy ảnh cắt được
                        Bitmap resultImage = overlay.CroppedImage;
                        pictureBox2.Image = resultImage; // Hiện ảnh lên (nhớ check tên pictureBox có đúng là 2 hay 1 nhé)
                        this.Show(); // Hiện lại tool chính ngay

                        // --- BẮT ĐẦU XỬ LÝ OCR (ĐÃ SỬA) ---
                        try
                        {
                            // [QUAN TRỌNG] Lấy đường dẫn gốc của file EXE
                            string exePath = AppContext.BaseDirectory;
                            string tessDataPath = System.IO.Path.Combine(exePath, "tessdata");
                            string x64Path = System.IO.Path.Combine(exePath, "x64");

                            // [KIỂM TRA 1] Check folder Data
                            if (!System.IO.Directory.Exists(tessDataPath))
                            {
                                MessageBox.Show($"Lỗi: Không tìm thấy folder 'tessdata' tại:\n{tessDataPath}");
                                return;
                            }

                            // [KIỂM TRA 2] Check folder Thư viện x64
                            // Đây là nguyên nhân chính gây lỗi TargetInvocationException
                            if (!System.IO.Directory.Exists(x64Path))
                            {
                                MessageBox.Show($"Lỗi: Không tìm thấy folder 'x64' chứa file DLL tại:\n{x64Path}\n\nÔng nhớ copy folder x64 từ bin/Debug sang đây nhé!");
                                return;
                            }

                            // [THỦ THUẬT CAO CẤP] Ép Windows nhận diện folder x64
                            // Dòng này giúp Tesseract tìm thấy file DLL leptonica và tesseract50
                            string pathVar = Environment.GetEnvironmentVariable("PATH");
                            Environment.SetEnvironmentVariable("PATH", x64Path + ";" + pathVar);

                            // Phóng to ảnh để đọc cho nét
                            using (Bitmap scaledImage = ScaleImage(resultImage, 3.0f))
                            {
                                using (var stream = new System.IO.MemoryStream())
                                {
                                    scaledImage.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                                    stream.Position = 0;

                                    using (var img = Tesseract.Pix.LoadFromMemory(stream.ToArray()))
                                    {
                                        // Khởi tạo Engine với đường dẫn đã check kỹ
                                        using (var engine = new Tesseract.TesseractEngine(tessDataPath, "vie", Tesseract.EngineMode.Default))
                                        {
                                            engine.SetVariable("preserve_interword_spaces", "1");

                                            using (var page = engine.Process(img))
                                            {
                                                string text = page.GetText();

                                                // Kiểm tra nếu không đọc được gì
                                                if (string.IsNullOrWhiteSpace(text))
                                                    richTextBox1.Text = "Không đọc được chữ nào (hoặc ảnh mờ quá).";
                                                else
                                                    richTextBox1.Text = text;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            // Bắt lỗi chi tiết nhất có thể
                            string rootCause = ex.InnerException != null ? ex.InnerException.Message : "Không có Inner Exception";
                            MessageBox.Show($"Lỗi OCR:\n- Message: {ex.Message}\n- Root Cause: {rootCause}\n\n*Khả năng cao là thiếu file DLL trong folder x64*", "Lỗi Tesseract");
                        }
                    }
                    else
                    {
                        this.Show(); // Hiện lại nếu hủy
                    }
                }
            }
            catch (Exception ex)
            {
                this.Show();
                MessageBox.Show("Lỗi chụp màn hình: " + ex.Message);
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
