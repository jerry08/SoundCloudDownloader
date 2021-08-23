using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoundCloudDownloader.Models
{
    public class Query
    {
        public QueryKind Kind { get; }

        public string Value { get; }

        public Query(QueryKind kind, string value)
        {
            Kind = kind;
            Value = value;
        }
    }
}
