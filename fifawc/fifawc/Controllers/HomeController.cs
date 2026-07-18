using System.Text.Json;
using fifawc.Models;
using Microsoft.AspNetCore.Mvc;

namespace fifawc.Controllers
{
    public class HomeController : Controller
    {
        private readonly string filePath = Path.Combine(
            Directory.GetCurrentDirectory(),
            "Data",
            "players.json");

        // ??c JSON
        private List<Player> LoadPlayers()
        {
            if (!System.IO.File.Exists(filePath))
                return new List<Player>();

            var json = System.IO.File.ReadAllText(filePath);

            if (string.IsNullOrWhiteSpace(json))
                return new List<Player>();

            return JsonSerializer.Deserialize<List<Player>>(json) ?? new List<Player>();
        }

        // Ghi JSON
        private void SavePlayers(List<Player> players)
        {
            var json = JsonSerializer.Serialize(players, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            System.IO.File.WriteAllText(filePath, json);
        }

        // Trang ch?
        public IActionResult Index()
        {
            return View(LoadPlayers());
        }

        // Qu?n lý
        public IActionResult Admin()
        {
            return View(LoadPlayers());
        }

        // Chi ti?t
        public IActionResult Details(int id)
        {
            var players = LoadPlayers();

            var player = players.FirstOrDefault(x => x.Id == id);

            if (player == null)
                return NotFound();

            return View(player);
        }

        public IActionResult TopScores()
        {
            var players = LoadPlayers()
                            .OrderByDescending(x => x.Goals)
                            .ThenByDescending(x => x.OVR)
                            .ToList();

            return View(players);
        }

        // ================= CREATE =================

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Player player, IFormFile? fileAnh)
        {
            var players = LoadPlayers();

            if (fileAnh != null && fileAnh.Length > 0)
            {
                string folder = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "wwwroot",
                    "images");

                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(folder);

                string fileName = Guid.NewGuid() + Path.GetExtension(fileAnh.FileName);

                string path = Path.Combine(folder, fileName);

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    fileAnh.CopyTo(stream);
                }

                player.Image = fileName;
            }

            player.Id = players.Count == 0
                ? 1
                : players.Max(x => x.Id) + 1;

            players.Add(player);

            SavePlayers(players);

            return RedirectToAction("Admin");
        }

        // ================= EDIT =================

        public IActionResult Edit(int id)
        {
            var players = LoadPlayers();

            var player = players.FirstOrDefault(x => x.Id == id);

            if (player == null)
                return NotFound();

            return View(player);
        }

        [HttpPost]
        [HttpPost]
        public IActionResult Edit(Player player, IFormFile? fileAnh)
        {
            var players = LoadPlayers();

            var p = players.FirstOrDefault(x => x.Id == player.Id);

            if (p == null)
                return NotFound();

            p.Name = player.Name;
            p.Position = player.Position;
            p.OVR = player.OVR;
            p.PAC = player.PAC;
            p.SHO = player.SHO;
            p.PAS = player.PAS;
            p.DRI = player.DRI;
            p.DEF = player.DEF;
            p.PHY = player.PHY;
            p.Vision = player.Vision;
            p.Finishing = player.Finishing;
            p.Acceleration = player.Acceleration;
            p.LongPass = player.LongPass;
            p.Positioning = player.Positioning;
            p.Control = player.Control;
            p.Goals = player.Goals;
            p.Matches = player.Matches;

            // N?u ch?n ?nh m?i
            if (fileAnh != null && fileAnh.Length > 0)
            {
                string folder = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "wwwroot",
                    "images");

                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(folder);

                string fileName = Guid.NewGuid() + Path.GetExtension(fileAnh.FileName);

                string path = Path.Combine(folder, fileName);

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    fileAnh.CopyTo(stream);
                }

                p.Image = fileName;
            }

            SavePlayers(players);

            return RedirectToAction("Admin");
        }

        // ================= DELETE =================

        public IActionResult Delete(int id)
        {
            var players = LoadPlayers();

            var player = players.FirstOrDefault(x => x.Id == id);

            if (player != null)
            {
                players.Remove(player);

                SavePlayers(players);
            }

            return RedirectToAction("Admin");
        }
    }
}