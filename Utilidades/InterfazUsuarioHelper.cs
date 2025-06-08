using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;

namespace Cliente_AdoptMe.Utilidades
{
    public static class InterfazUsuarioHelper
    {
        public static void ReiniciarBordesTextBox(IEnumerable<TextBox> textBoxes, Brush color)
        {
            foreach (TextBox textBox in textBoxes)
            {
                textBox.BorderBrush = color;
            }
        }

        public static void ReiniciarBordesPasswordBox(IEnumerable<PasswordBox> passwordBoxes, Brush color)
        {
            foreach (PasswordBox passwordBox in passwordBoxes)
            {
                passwordBox.BorderBrush = color;
            }
        }
    }
}
