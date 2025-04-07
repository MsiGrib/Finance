using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModel.DataBase
{
    public class MainBoardDTO : IEntity<long>
    {
        public long Id { get; set; }
        public required int OrderLevel { get; set; }

        public required long TableId { get; set; }
        public virtual TableDTO Table { get; set; }
    }
}
