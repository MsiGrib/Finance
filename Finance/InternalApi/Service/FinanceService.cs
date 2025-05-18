using DataModel.DataBase;
using DataModel.DataStructures;
using InternalApi.EntityGateWay;
using Microsoft.AspNetCore.Components.Forms;
using System.Net.Http;
using System.Threading.Tasks;

namespace InternalApi.Service
{
    public class FinanceService
    {
        private readonly MainBoardRepository _mainBoardRepository;
        private readonly IWebHostEnvironment _env;

        public FinanceService(IRepository<MainBoardDTO, long> mainBoardRepository, IWebHostEnvironment env)
        {
            _mainBoardRepository = (MainBoardRepository)mainBoardRepository;
            _env = env;
        }

        public async Task<List<Pair<int, TableDTO>>> GetFinanceViewsAsync()
        {
            var result = new List<Pair<int, TableDTO>>();
            var mainBoards = await _mainBoardRepository.GetAllAsync();

            foreach (var item in mainBoards)
            {
                var tmp = new Pair<int, TableDTO>();
                tmp.First = item.OrderLevel;
                tmp.Second = item.Table;

                result.Add(tmp);
            }

            return result;
        }

        public async Task<string> ConvertImageToBase64(string imagePath)
        {
            var fullPath = Path.Combine(_env.WebRootPath, imagePath.TrimStart('/'));
            if (!File.Exists(fullPath))
                throw new FileNotFoundException("Image not found", fullPath);

            var bytes = await File.ReadAllBytesAsync(fullPath);
            return $"data:image/svg+xml;base64,{Convert.ToBase64String(bytes)}";
        }
    }
}
