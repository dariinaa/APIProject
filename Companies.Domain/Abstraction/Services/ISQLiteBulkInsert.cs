using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Companies.Domain.Abstraction.Services
{
    public interface ISQLiteBulkInsert : IDisposable
    {
        bool AllowBulkInsert { get; set; }
        uint CommitMax { get; set; }
        string TableName { get; }
        string ParamDelimiter { get; }

        void AddParameter(string name, DbType dbType);
        Task Flush();
        Task Insert(object[] paramValues);
    }
}
