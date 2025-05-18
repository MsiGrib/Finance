using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
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

        [JsonIgnore]
        public virtual MainBoardDTO MainBoard { get; set; }

        public virtual ICollection<PlotDTO> Plots { get; set; } = new List<PlotDTO>();
    }
}
