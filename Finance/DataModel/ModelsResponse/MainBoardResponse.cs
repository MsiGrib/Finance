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
        public List<Pair<int, TableDTO>> Tables { get; set; }
    }
}
