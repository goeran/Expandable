using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Expandable.Internal
{
    internal class TableParser
    {
        private string data;
        private readonly HashSet<string> columns = new HashSet<string>(); 
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

        public bool HasColumn(string name)
        {
            return columns.Contains(name.ToLower());
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
                            foreach (var colName in header.Split('|').Select(c => c.Trim()))
                            {
                                columns.Add(colName.ToLower());
                            }
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
