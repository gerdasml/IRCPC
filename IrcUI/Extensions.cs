using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows.Media;
using System.Xml;

namespace IrcUI
{
    public static class Extensions
    {
        public static void AppendLine(this RichTextBox box, string text, string color)
        {
            BrushConverter bc = new BrushConverter();
            TextRange tr = new TextRange(box.Document.ContentEnd, box.Document.ContentEnd);
            tr.Text = text + Environment.NewLine;
            try
            {
                tr.ApplyPropertyValue(TextElement.ForegroundProperty, bc.ConvertFromString(color));
                box.ScrollToEnd();
            }
            catch (FormatException) { }
        }

        public static RichTextBox DeepCopy(this RichTextBox element)

        {
            string shapestring = XamlWriter.Save(element);
            StringReader stringReader = new StringReader(shapestring);
            XmlTextReader xmlTextReader = new XmlTextReader(stringReader);
            RichTextBox DeepCopyobject = (RichTextBox)XamlReader.Load(xmlTextReader);
            return DeepCopyobject;
        }
    }
}
