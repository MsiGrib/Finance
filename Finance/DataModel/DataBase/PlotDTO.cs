using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModel.DataBase
{
    public class PlotDTO : IEntity<long>
    {
        public long Id { get; set; }
        public required DateTime Date { get; set; }
        public required decimal Price { get; set; }

        public virtual TableDTO Table { get; set; }
    }
}
