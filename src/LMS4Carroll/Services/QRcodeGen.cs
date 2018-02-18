using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ZXing;

namespace LMS4Carroll.Services
{
    public class QRcodeGen
    {
        /*public void CreateQRcode(string input)
        {
            var writer = new IBarcodeWriter();
            writer.Format = BarcodeFormat.QR_CODE;
            var result = writer.Write(input);
            var barcodeBitmap = new Bitmap(result);
            barcodeBitmap.Save
                (context.Response.OutputStream, ImageFormat.Jpeg);
            context.Response.ContentType = "image/jpeg";
            context.Response.End();
        }

        public void ReadQRcode(string input)
        {
            var reader = new BarcodeReader();
            //Saving the uploaded image and reading from it var fileName =
            Path.Combine(Request.MapPath("~/Imgs"), "QRImage.jpg");
            fileUpload.SaveAs(fileName);
            var result = reader.Decode(new Bitmap(fileName));
            Response.Write(result.Text);
        }*/
    }
}
