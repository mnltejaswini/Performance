using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace ConsoleApplication
{
    public class Program
    {
        private static readonly Random _rand = new Random();

        public static void Main(string[] args)
        {
            var output = args.Length > 1 ? args[0] : "largepage.html";
            var root = new XElement("html", CreateHead(), CreateBody());
            using (var fs = File.OpenWrite(output))
            {
                var xdoc = new XDocument(root);
                xdoc.Save(fs);
            }
        }

        private static object CreateBody()
        {
            var body = new XElement("body");

            for (var i = 0; i < 50; ++i)
            {
                var div = new XElement("div",
                    CreateLargeTable(10, 10),
                    CreateHugeList(50));
                div.SetAttributeValue("class", $"div-{i}");
                body.Add(div);
            }

            return body;
        }

        private static object CreateHead()
        {
            var meta = new XElement("meta");
            meta.SetAttributeValue("charset", "utf-8");
            var head = new XElement("head", meta, new XElement("title", GetLargeString(100)));
            head.SetAttributeValue("attr1", GetLargeString(200));
            head.SetAttributeValue("attr2", GetLargeString(200));
            head.SetAttributeValue("attr3", GetLargeString(200));

            return head;
        }

        private static string GetLargeString(int length)
        {
            var builder = new StringBuilder();

            while (builder.Length < length)
            {
                builder.Append(Guid.NewGuid().ToString());
            }

            builder.Length = length;
            return builder.ToString();
        }

        private static XElement CreateLargeTable(int row, int column)
        {
            return new XElement("table",
                new XElement("tr", Enumerable.Range(0, column)
                                             .Select(i => new XElement("th", $"column {i}"))),
                Enumerable.Range(0, row)
                          .Select(i => new XElement("td",
                                                    Enumerable.Range(0, column)
                                                              .Select(j => new XElement("th", Guid.NewGuid().ToString())))));
        }

        private static XElement CreateHugeList(int count)
        {
            return new XElement("list",
                new XElement("ul", Enumerable.Range(0, count).Select(i => new XElement("li", $"item {i}")),
                new XElement("ol", Enumerable.Range(0, count).Select(i => new XElement("li", $"item {i}")))));
        }
    }
}
