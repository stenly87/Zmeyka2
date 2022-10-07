using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Zmeyka2.Элементы
{
    class Snake : GameEntity
    {
        public Snake(int size)
        {
            Rectangle rect = new Rectangle();
            rect.Width = size;
            rect.Height = size;
            rect.Fill = Brushes.Blue;
            UIElement = rect;
        }
        public bool IsHead { get; set; }

    }
}
