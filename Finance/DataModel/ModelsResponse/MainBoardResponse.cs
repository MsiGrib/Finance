using DataModel.DataBase;
using DataModel.DataStructures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModel.ModelsResponse
{
    public class MainBoardResponse
    {
        public List<Tuple<int, TableView>> MainBoards { get; set; }
    }

    public class TableView
    {
        public string Name { get; set; }
        public string SubName { get; set; }
        public string Currency { get; set; }
        public string ImageBase64 { get; set; }
        public List<PlotDTO> Plots { get; set; }
    }
}
