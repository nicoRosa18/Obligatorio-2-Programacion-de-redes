using System.Drawing;

namespace Common.FileManagement
{
    public class FileDefaultCoverCreation
    {
        public Bitmap DrawFilledRectangle(int x, int y)
        {
            Bitmap bmp = new Bitmap(x, y);
            PointF firstLocation = new PointF(10f, 10f);
            using (Graphics graph = Graphics.FromImage(bmp))
            {
                using (Font arialFont = new Font("Arial", 10))
                {
                    Rectangle ImageSize = new Rectangle(0, 0, x, y);
                    graph.FillRectangle(Brushes.White, ImageSize);
                    graph.DrawString("COVER NOT FOUND", arialFont, Brushes.Black, firstLocation);
                }
            }
            return bmp;
        }
    }
}