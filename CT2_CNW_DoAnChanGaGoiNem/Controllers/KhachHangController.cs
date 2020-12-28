using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CT2_CNW_DoAnChanGaGoiNem.Models;
using System.IO;
namespace CT2_CNW_DoAnChanGaGoiNem.Controllers
{
    public class KhachHangController : Controller
    {
        DataClasses1DataContext db = new DataClasses1DataContext();
        //
        // GET: /KhachHang/
        public ActionResult Index()
        {
            //Lay loai san pham
            List<LOAI> listLoai = db.LOAIs.ToList();
            ViewBag.categories = listLoai;

            //Lay cac san pham trend
            var trend = db.SPTHICHes.Select(s => s.MASANPHAM).Distinct().Take(3).ToList();
            ViewBag.trend = trend;

            //Lay sp thich cua user
            if (Session["kh"] != null) {
                KHACHHANG k = Session["kh"] as KHACHHANG;
                List<SANPHAM> listSpThich = (from s in db.SANPHAMs
                                             join t in db.SPTHICHes
                                             on s.MASANPHAM equals t.MASANPHAM
                                             where t.USERNAME == k.USERNAME
                                             select s).ToList();
                ViewBag.thich = listSpThich;
            }

            //Lay 8 san pham moi nhat
            List<SANPHAM> listSp = db.SANPHAMs.Where(s => s.MOI == 1).OrderByDescending(sp=>sp.NGAYTAO).Take(8).ToList();

            //Lay SP GIAM GIA
            List<SANPHAM> lspgg = db.SANPHAMs.Where(sp => sp.GIAM != 0).OrderByDescending(sp => sp.GIAM).Take(3).ToList();
            ViewBag.spGiamGia = lspgg;

            //lAY Gio Hang chua thanh toan
            
            if (Session["kh"] != null)
            {
                KHACHHANG kh = Session["kh"] as KHACHHANG;
                List<CART_ITEM> cart = db.CART_ITEMs.Where(s => s.USERNAME == kh.USERNAME && s.TRANGTHAI == 0).ToList();
                ShopCart shop = new ShopCart(cart);
                Session["gh"] = shop;
            }
            return View(listSp);
        }
        //

        [HttpGet]
        public ActionResult Signin() {
            if (Session["kh"] != null) {
                KHACHHANG kh = Session["kh"] as KHACHHANG;
                KHACHHANG k = db.KHACHHANGs.FirstOrDefault(kk => kk.USERNAME == kh.USERNAME);
                if (k.ROLE == 1) {
                    return RedirectToAction("Dashboard", "AdminMenu");
                }
                return RedirectToAction("Index", "KhachHang");
            }
            return View();
        }
        [HttpPost]
        public ActionResult Signin(FormCollection c)
        {
            if (Session["kh"] != null) {
                return RedirectToAction("Index", "KhachHang");
            }
            KHACHHANG kh = db.KHACHHANGs.FirstOrDefault(k => k.USERNAME == c["txtUsername"] && k.MATKHAU == c["txtPassword"]);
            if (kh != null)
            {
                if (kh.ROLE == 1)
                {
                    Session["kh"] = kh;
                    return RedirectToAction("Dashboard", "AdminMenu");
                }
                Session["kh"] = kh;
                return RedirectToAction("Index", "KhachHang");
            }
            else {
                Session["tb"] = "Tài khoản hoặc mật khẩu không đúng !";
                return RedirectToAction("Signin", "KhachHang");
            }
        }
        //
        [HttpGet]
        public ActionResult Signup() {
            return View();
        }
        [HttpPost]
        public ActionResult Signup(FormCollection c)
        {
            bool check = db.KHACHHANGs.Where(k => k.USERNAME == c["txtUsername"]).Count() > 0;
            if (check)
            {
                ViewBag.hasUser = "Tài khoản này đã tồn tại";
                return View();
            }
            else
            {
                KHACHHANG k = new KHACHHANG();
                k.USERNAME = c["txtUsername"];
                k.HOTEN = c["txtHo"] + " " + c["txtTen"];
                k.SDT = c["txtPhone"];
                k.EMAIL = c["txtEmail"];
                k.DIACHI = c["txtAddress"];
                k.QUEQUAN = c["txtQueQuan"];
                k.THANHPHO = c["txtCity"];
                k.MATKHAU = c["txtPassword"];
                string url = "default-avatar.jpg";
                k.HINH = url;
                db.KHACHHANGs.InsertOnSubmit(k);
                db.SubmitChanges();
                Session["tb"] = "Đăng ký tài khoản thành công !";
                Session["kh"] = k;
                return RedirectToAction("Index", "KhachHang");
            }
        }
        public ActionResult KhachHangNow() {
            KHACHHANG k = Session["kh"] as KHACHHANG;
            //List<SPTHICH> listSpThich = db.SPTHICHes.Where(s => s.USERNAME == k.USERNAME).ToList();
            List<SPTHICH> thich = new List<SPTHICH>();
            
            if (k != null)
            {
                thich = db.SPTHICHes.Where(s => s.USERNAME == k.USERNAME).ToList();
                List<CART_ITEM> gh = db.CART_ITEMs.Where(s => s.USERNAME == k.USERNAME && s.TRANGTHAI == 0).ToList();
                ShopCart sh = new ShopCart(gh);
                ViewBag.hang = sh;
            }
            
            ViewBag.thich = thich;
            return PartialView(k);
        }

        //Log out
        public ActionResult Logout() {
            Session.Clear();
            return RedirectToAction("Index", "KhachHang");
        }

        public ActionResult SanPhamThich() {
            if (Session["kh"] == null)
            {
                return RedirectToAction("Signin", "KhachHang");
            }
            KHACHHANG k = Session["kh"] as KHACHHANG;
            List<SANPHAM> thich = new List<SANPHAM>();
            List<string> listmspthich = new List<string>();
            List<string> listmsprelated = new List<string>();
            var data = from s in db.SANPHAMs
                       join t in db.SPTHICHes
                       on s.MASANPHAM equals t.MASANPHAM
                       where t.USERNAME == k.USERNAME
                       select s;
            foreach (var item in data) {
                SANPHAM s = item as SANPHAM;
                thich.Add(s);
            }
            foreach (SANPHAM sq in thich) {
                listmspthich.Add(sq.MASANPHAM);
            }
            //Related
            List<SANPHAM> related = db.SANPHAMs.ToList();
            int randomRecord = new Random().Next() % related.Count(); //To make sure its valid index in list
            List<SANPHAM> qData = related.Skip(randomRecord).Take(8).ToList();
            foreach (SANPHAM qs in qData) {
                listmsprelated.Add(qs.MASANPHAM);
            }
            IEnumerable<string> relatedSp = listmsprelated.Except(listmspthich).ToList();

            List<SANPHAM> asd = new List<SANPHAM>();
            foreach (string sq in relatedSp) {
                SANPHAM s = db.SANPHAMs.Where(sw => sw.MASANPHAM == sq).Single();
                asd.Add(s);
            }
            ViewBag.related = asd;
            return View(thich);
        }

        //
        public ActionResult ThongTin(string username) {
            
            if (Session["kh"] == null)
            {
                return RedirectToAction("Signin", "KhachHang");
            }
            else
            {
                KHACHHANG k = Session["kh"] as KHACHHANG;
                List<CHITIETHOADON> data = (from c in db.CHITIETHOADONs
                            join d in db.HOADONs
                            on c.MAHD equals d.MAHD
                            where d.USERNAME == k.USERNAME
                            select c).ToList();
                List<HOADON> datahd = db.HOADONs.Where(c => c.USERNAME == k.USERNAME).ToList();
                ViewBag.cthd = data;
                ViewBag.lhd = datahd;
                return View(k);
            }
        }

        public ActionResult UpdateThongTin(FormCollection c, HttpPostedFileBase fileupload) {
            if (Session["kh"] == null) {
                return RedirectToAction("Signup", "KhachHang");
            }
            KHACHHANG kh = Session["kh"] as KHACHHANG;
            KHACHHANG k = db.KHACHHANGs.FirstOrDefault(kkk => kkk.USERNAME == kh.USERNAME);
            k.HOTEN = c["txtHo"] + " " + c["txtTen"];
            k.SDT = c["txtPhone"];
            k.EMAIL = c["txtEmail"];
            k.DIACHI = c["txtAddress"];
            k.QUEQUAN = c["txtQueQuan"];
            k.THANHPHO = c["txtCity"];
            string filename = Path.GetFileNameWithoutExtension(fileupload.FileName);
            string extension = Path.GetExtension(fileupload.FileName);
            filename = filename + extension;
            fileupload.SaveAs(Server.MapPath("~/Content/img/customers/" + filename.ToString()));
            k.HINH = filename;
            db.SubmitChanges();
            Session["kh"] = k;
            return RedirectToAction("ThongTin", "KhachHang");
        }
	}
}