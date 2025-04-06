using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModel.DataBase
{
    public class TableDTO : IEntity<long>
    {
        public long Id { get; set; }
        public required string Name { get; set; }
        public string SubName { get; set; }
        public required string Currency { get; set; }
        public string ImagePath { get; set; }

        public virtual MainBoardDTO MainBoard { get; set; }

        public required long PlotId { get; set; }
        public virtual PlotDTO Plot { get; set; }
    }
}
