using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Expandable
{
    internal class TableParser
    {
        private string data;
        private readonly List<string> columns = new List<string>();
        private readonly List<List<string>> rows = new List<List<string>>();

        public TableParser(string data)
        {
            this.data = data;
        }

        public IEnumerable<string> Columns
        {
            get { return columns; }
        }

        public IEnumerable<IEnumerable<string>> Rows
        {
            get { return rows; }
        }

        public void Parse()
        {
            String header = null;
            using (var reader = new StringReader(data))
            {
                while (reader.Peek() != -1)
                {
                    var line = reader.ReadLine();
                    if (!String.IsNullOrWhiteSpace(line))
                    {
                        if (header == null)
                        {
                            header = line;
                            columns.AddRange(header.Split('|').Select(c => c.Trim()));
                        }
                        else
                        {
                            var rowColumns = line.Split('|');
                            rows.Add(new List<string>(rowColumns));
                        }
                    }
                }
            }
        }
    }
}
