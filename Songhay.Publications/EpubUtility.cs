using System.IO;
using System.Text;
using System.Xml.Linq;

namespace Songhay.Publications
{
    /// <summary>
    /// Shared routines for EPUB pipeline automation.
    /// </summary>
    public static class EpubUtility
    {
        /// <summary>
        /// Gets <see cref="UTF8Encoding"/>
        /// instanitated with <c>encoderShouldEmitUTF8Identifier: true</c>.
        /// </summary>
        /// <returns></returns>
        public static Encoding GetUnicodeWithBomEncoding()
        {
            return new UTF8Encoding(encoderShouldEmitUTF8Identifier: true);
        }

        /// <summary>
        /// Saves the specified <see cref="XDocument"/>
        /// with <see cref="Stream"/> encoding
        /// from <see cref="GetUnicodeWithBomEncoding"/>.
        /// </summary>
        /// <param name="document"></param>
        /// <param name="path"></param>
        public static void SaveAsUnicodeWithBom(XDocument document, string path)
        {
            var encoding = GetUnicodeWithBomEncoding();
            using (var stream = new StreamWriter(path, append: false, encoding: encoding))
            {
                document.Save(stream);
            }
        }
    }
}
