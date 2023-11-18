using Hotels.Data;
using Hotels.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using MimeKit;
using MailKit.Net.Smtp;
using static System.Net.Mime.MediaTypeNames;

namespace Hotels.Controllers
{
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;

        }
        public async Task<string> SendEmail()
        {
            var Message = new MimeMessage();
            Message.From.Add(new MailboxAddress("Test Message", "saharalgryan@gmail.com"));
            Message.To.Add(MailboxAddress.Parse("saharalgryan@gmail.com"));
            Message.Subject = ("Test Email From My Project in Asp.net Core MVC");
            Message.Body = new TextPart("Plain")
            {
                Text = "Welcome In My App"
            };

            using (var client = new SmtpClient())
            {
                try
                {
                    client.Connect("smtp.gmail.com", 587);
                    client.Authenticate("saharalgryan@gmail.com", "krhkhpjoqoiaqhbb");
                    await client.SendAsync(Message);
                    client.Disconnect(true);
                }
                catch(Exception e) { return e.Message.ToString();
                }
            }
            return "Ok";
        }
        public IActionResult Delete(int id)
        {
            var hotelDel = _context.hotel.SingleOrDefault(x => x.id == id);
            if (hotelDel != null)
            {
                _context.hotel.Remove(hotelDel);
                _context.SaveChanges();
                TempData["Del"] = "Ok";
            }

            return RedirectToAction("Index");
        }

        public IActionResult CreateNewRooms(Rooms rooms)
        {
            _context.rooms.Add(rooms);
            _context.SaveChanges();
            return RedirectToAction("Rooms");
        }
        [HttpPost]
        public IActionResult Index(string city)
        {
            var hotel = _context.hotel.Where(x => x.City.Equals(city));
            return View(hotel);
        }

   //     public IActionResult RoomDetails()
   //     {
			//_context.rooms.Add(RoomDetails);
			//_context.SaveChanges();
			//return RedirectToAction("RoomDetails")

			//return View();
       // }
        public IActionResult Rooms()
        {

            var hotel = _context.hotel.ToList();
            ViewBag.hotel = hotel;
			//ViewBag.currentuser = Request.Cookies["userName"];
			ViewBag.currentuser = HttpContext.Session.GetString("UserName");
			var rooms = _context.rooms.ToList();

            return View(rooms);
        }
        public IActionResult Update(Hotel hotel)
        {
            if (ModelState.IsValid)
            {
                _context.hotel.Update(hotel);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }

            return View("Edit");


        }
        public IActionResult Edit(int id)
        {
            var hoteledit = _context.hotel.SingleOrDefault(x => x.id == id);

            return View(hoteledit);
        }

        //krhkhpjoqoiaqhbb
        [Authorize]
        public IActionResult Index()
        {
            var currentuser = HttpContext.User.Identity.Name;
            ViewBag.currentuser= currentuser;
            //CookieOptions option = new CookieOptions();
            //option.Expires = DateTime.Now.AddMinutes(20);
            //Response.Cookies.Append("UserName", currentuser, option);
            HttpContext.Session.SetString("UserName", currentuser); 
            var hotel = _context.hotel.ToList();
            return View(hotel);
        }
        //ModelState.IsValid
        public IActionResult CreateNewHotel(Hotel hotels)
        {
            if (ModelState.IsValid)
            {
                _context.hotel.Add(hotels);
                _context.SaveChanges();
                return RedirectToAction("Index");

            }
            var hotel = _context.hotel.ToList();
            return View("index", hotel);

        }
    }
}
