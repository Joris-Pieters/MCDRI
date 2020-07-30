using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.Xml.Serialization;
using System.IO;
using MC.Design;
using MC.Testing;

namespace MC.IO
{
    [Serializable]
    public class SerializableImageBox : SerializableTestElement
    {
        public ImageMode ImageMode { get; set; }
        public string Image { get; set; }

        public SerializableImageBox() : base ()
        {
        }

        public SerializableImageBox(ImageBox imageBox) : base (imageBox)
        {
            ImageMode = imageBox.ImageMode;
            MemoryStream ms = new MemoryStream();
            if (imageBox.Image != null)
            {
                imageBox.Image.Save(ms, ImageFormat.Png);
                Image = Convert.ToBase64String(ms.ToArray());
            }
        }

        public override TestElement Deserialize(Page page, bool isCopy = false)
        {
            if (isCopy)
            {
                Name = page.ParentTest.GetFreeName(Name);
            }
            ImageBox imageBox = new ImageBox(Name, page, GridPosition, GridSize);
            imageBox.Border = Border;
            imageBox.ImageMode = ImageMode;
            if (Image != null)
            {
                byte[] imageBytes = Convert.FromBase64String(Image);
                MemoryStream ms = new MemoryStream(imageBytes, 0, imageBytes.Length);
                ms.Write(imageBytes, 0, imageBytes.Length);
                imageBox.Image = System.Drawing.Image.FromStream(ms, true);
            }
            return imageBox;
        }
    }
}
