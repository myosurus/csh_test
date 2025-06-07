using System.Windows.Media;

namespace TWpfApp.Models
{
    public class ImageItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public byte[] Data { get; set; }
        public ImageSource ImageSource { get; set; }
    }
}

