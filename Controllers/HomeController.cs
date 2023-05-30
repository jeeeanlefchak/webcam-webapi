using AForge.Video.DirectShow;
using AForge.Video;
using Microsoft.AspNetCore.Mvc;
using System.Drawing;
using System.Drawing.Imaging;

namespace webcam.Controllers
{
    [ApiController]
    public class HomeController : Controller
    {

        public static VideoCaptureDevice videoSource;
        public static Bitmap _video;

        [HttpGet("ligar")]
        public async Task<IActionResult> ligar(int secondes = 60)
        {
            var videoSources = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            if (videoSources != null && videoSources.Count > 0)
            {
                videoSource = new VideoCaptureDevice(videoSources[0].MonikerString);
                videoSource.NewFrame += VideoSource_NewFrame;
            }
            if (videoSource.IsRunning == false)
                videoSource.Start();

            return Ok(videoSource);
        }

        [HttpGet("")]
        public async Task<IActionResult> getVideo()
        {
            if(videoSource == null || videoSource.IsRunning == false)
            {
                await this.ligar();
            }
            byte[] imageByteData = Convert.FromBase64String(BitmapToBase64(_video));

            return File(imageByteData, "image/jpeg");
        }

        [HttpGet("desligar")]
        public async Task<IActionResult> Desligar()
        {
            videoSource.Stop();
            return Ok(true);
        }


        private void VideoSource_NewFrame(object sender, AForge.Video.NewFrameEventArgs eventArgs)
        {
            _video = (Bitmap)eventArgs.Frame.Clone();
        }

        private string BitmapToBase64(Bitmap bImage)
        {
            if (bImage == null) return "";
            System.IO.MemoryStream ms = new MemoryStream();
            bImage.Save(ms, ImageFormat.Jpeg);
            byte[] byteImage = ms.ToArray();
            return Convert.ToBase64String(byteImage);
        }

    }
}
