using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace HexagonalWpf
{
    internal class FileList
    {
        public List<string> GetList { get; }

        public FileList(string fName, string ext)
        {
            GetList = new List<string>(Directory.GetFiles(fName ?? string.Empty, $"*{ext}").Select(itm => new CustomFileName(itm)).ToList().OrderBy(x => x.number).Select(y => y.Name));
        }

    }

    internal class CustomFileName
    {
        public string Name { get; set; }
        public int number { get; set; }

        public CustomFileName(string fName)
        {
            Name = fName;
            var t = new Regex(@"\d+").Matches(fName);
            number = t.Count > 0 ? int.Parse(t[t.Count - 1].Value) : 9999;
        }

    }
}
