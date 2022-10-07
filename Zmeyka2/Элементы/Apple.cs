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
    class Apple : GameEntity
    {
        public Apple(int size)
        {
            Rectangle rect = new Rectangle();
            rect.Width = size;
            rect.Height = size;
            rect.Fill = Brushes.Red;
            UIElement = rect;
        }

        public override bool Equals(object obj)
        {
            Apple apple = obj as Apple;
            if(apple != null)
            {
                return X == apple.X && Y == apple.Y;
            }
            else
            {
                return false;
            }

        }
    }
}
