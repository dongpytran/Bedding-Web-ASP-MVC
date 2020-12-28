using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CT2_CNW_DoAnChanGaGoiNem.Models;
using System.IO;
using PagedList.Mvc;
using PagedList;
using System.Globalization;
using Newtonsoft.Json;
namespace CT2_CNW_DoAnChanGaGoiNem.Controllers
{
    public class AdminMenuController : Controller
    {
        //
        // GET: /AdminMenu/

        DataClasses1DataContext db = new DataClasses1DataContext();
        //get number of week
        public int getWeek(DateTime date) {
            DayOfWeek day = CultureInfo.InvariantCulture.Calendar.GetDayOfWeek(date);
            if (day >= DayOfWeek.Monday && day <= DayOfWeek.Wednesday)
            {
                date = date.AddDays(3);
            }

            // Return the week of our adjusted day
            return CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(date, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
        }
        public ActionResult Dashboard() {
            if (Session["kh"] == null) {
                return RedirectToAction("Signin", "KhachHang");
            }
            KHACHHANG k = Session["kh"] as KHACHHANG;
            if (k.ROLE != 1) {
                return RedirectToAction("Index", "KhachHang");
            }
            //get member
            List<KHACHHANG> listkh = db.KHACHHANGs.Where(kh=>kh.TRANGTHAI != 0 && kh.ROLE != 1).ToList();
            ViewBag.kh = listkh;

            //get so item da ban
            int slItem = 0;
            if (db.CHITIETHOADONs.ToList().Count() > 0) {
                slItem = db.CHITIETHOADONs.Sum(c => c.SOLUONG.HasValue ? c.SOLUONG.Value : 0);
            }
            
            ViewBag.slItem = slItem;
            //get sl trong tuan
            List<HOADON> lhd = db.HOADONs.ToList();
            List<HOADON> lhdWeek = new List<HOADON>();
            List<CHITIETHOADON> lct = new List<CHITIETHOADON>();
            foreach (HOADON h in lhd) {
                if (getWeek(DateTime.Now) == getWeek(h.NGAYTAO)) {
                    lhdWeek.Add(h);
                }
            }
            foreach(HOADON h in lhdWeek){
                List<CHITIETHOADON> ct = db.CHITIETHOADONs.Where(c => c.MAHD == h.MAHD).ToList();
                foreach (CHITIETHOADON cc in ct) {
                    lct.Add(cc);
                }
            }

            ViewBag.itemWeek = lct.Count();

            //get Tong thu nhap $$
            List<CHITIETHOADON> lcthd = db.CHITIETHOADONs.OrderBy(c => c.MAHD).ToList();
            double tong = 0;
            foreach (CHITIETHOADON c in lcthd) {
                tong += ((c.SOLUONG.HasValue ? c.SOLUONG.Value : 0) * (c.GIA.HasValue ? c.GIA.Value : 0));
            }
            ViewBag.tongtien = tong;

            return View(db.CHITIETHOADONs.ToList());
        }
        public ActionResult Index(int? page)
        {
            if (Session["kh"] == null)
            {
                return RedirectToAction("Signin", "KhachHang");
            }
            KHACHHANG k = Session["kh"] as KHACHHANG;
            if (k.ROLE != 1)
            {
                return RedirectToAction("Index", "KhachHang");
            }
            var list = db.SANPHAMs.OrderByDescending(s=>s.NGAYTAO).ToList().ToPagedList(page ?? 1,10);
            return View(list);
        }



        //
        // GET: /AdminMenu/Create
        public ActionResult Create()
        {
            if (Session["kh"] == null)
            {
                return RedirectToAction("Signin", "KhachHang");
            }
            KHACHHANG k = Session["kh"] as KHACHHANG;
            if (k.ROLE != 1)
            {
                return RedirectToAction("Index", "KhachHang");
            }
            SANPHAM sp = new SANPHAM();
            //KichThuoc
            List<string> ktt = db.NEM_KICHTHUOCs.Select(ss => ss.KICHTHUOC).Distinct().ToList();
            ViewBag.kt = ktt;

            //DropDown LOAI, TEN THUONG HIEU
            List<LOAI> l = db.LOAIs.ToList();
            List<THUONGHIEU> th = db.THUONGHIEUs.ToList();
            ViewBag.loai = l;
            ViewBag.th = th;
            
            //Lay list masp da ton tai
            //List<string> listMasp = db.SANPHAMs.Select(swp => swp.MASANPHAM).ToList();
            
            char[] letters = "qwertyuioplkjhgfdsazxcvbnm0123456789".ToCharArray();
            Random r = new Random();
            string randomMasp = "";
            for (int i = 0; i < 5; i++)
            {
                randomMasp += letters[r.Next(0, 36)].ToString();
            }
            ViewBag.masp = randomMasp;
            return View();
        }

        //
        // POST: /AdminMenu/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public ActionResult Create(FormCollection c, HttpPostedFileBase fileupload)
        {
                if (ModelState.IsValid) {
                    // TODO: Add insert logic here
                    SANPHAM s = new SANPHAM();
                    //Insert sanpham
                    s.MASANPHAM = c["MASANPHAM"].Trim(new Char[] { ' ', ',', '*', '.' });
                    s.TENSANPHAM = c["TENSANPHAM"];
                    s.MALOAI = c["MALOAI"];
                    s.MATHUONGHIEU = c["THUONGHIEU"];
                    s.GIAGOC = double.Parse(c["GIAGOC"]);
                    string now = DateTime.Now.ToShortDateString();
                    s.NGAYTAO = Convert.ToDateTime(now);
                    s.MOI = Convert.ToInt16(c["MOI"]);
                    s.GIAM = double.Parse(c["GIAM"]);
                    s.SLTON = Convert.ToInt32(c["SLTON"]);
                    string filename = Path.GetFileNameWithoutExtension(fileupload.FileName);
                    string extension = Path.GetExtension(fileupload.FileName);
                    filename = filename + extension;
                    s.HINHMINHHOA = filename;
                    fileupload.SaveAs(Server.MapPath("~/Content/img/products/" + filename.ToString()));
                    db.SANPHAMs.InsertOnSubmit(s);
                    db.SubmitChanges();
                    //Insert kichthuoc
                    List<string> kichthuoc = new List<string>();
                    if (c["size1"] != null)
                    {
                        kichthuoc.Add(c["size1"].ToString());
                    }
                    if (c["size2"] != null)
                    {
                        kichthuoc.Add(c["size2"].ToString());
                    }
                    if (c["size3"] != null)
                    {
                        kichthuoc.Add(c["size3"].ToString());
                    }
                    if (c["size4"] != null)
                    {
                        kichthuoc.Add(c["size4"].ToString());
                    }
                    foreach (string k in kichthuoc)
                    {
                        NEM_KICHTHUOC kt = new NEM_KICHTHUOC();
                        kt.MASANPHAM = c["MASANPHAM"].Trim(new Char[] { ' ', ',', '*', '.' });
                        kt.KICHTHUOC = k;
                        db.NEM_KICHTHUOCs.InsertOnSubmit(kt);
                        db.SubmitChanges();
                    }
                    return RedirectToAction("Index");
                }
                return View();
        }

        //
        // GET: /AdminMenu/Edit/5
        public ActionResult Edit(string masp)
        {
            if (Session["kh"] == null)
            {
                return RedirectToAction("Signin", "KhachHang");
            }
            KHACHHANG k = Session["kh"] as KHACHHANG;
            if (k.ROLE != 1)
            {
                return RedirectToAction("Index", "KhachHang");
            }
            if (masp == null) {
                return RedirectToAction("Index", "AdminMenu");
            }
            SANPHAM sp = new SANPHAM();
            //KichThuoc
            List<string> ktt = db.NEM_KICHTHUOCs.Select(ss => ss.KICHTHUOC).Distinct().ToList();
            

            //DropDown LOAI, TEN THUONG HIEU
            List<LOAI> l = db.LOAIs.ToList();
            List<THUONGHIEU> th = db.THUONGHIEUs.ToList();
            ViewBag.loai = l;
            ViewBag.th = th;
            var spp = db.SANPHAMs.Single(s => s.MASANPHAM == masp);
            // GET THE KICHTHUOC OF SANPHAM
            List<string> kttheoSp = (from kt in db.NEM_KICHTHUOCs
                                     where kt.MASANPHAM == masp
                                     select kt.KICHTHUOC).ToList();
            //get kt chuwa chon
            var chuaChon = ktt.Except(kttheoSp).ToList();
            ViewBag.kttheoMasp = chuaChon;
            ViewBag.kt = kttheoSp;
            return View(spp);
        }

        //
        // POST: /AdminMenu/Edit/5
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        [HttpPost]
        public ActionResult Edit(string masp, SANPHAM collection, FormCollection c, HttpPostedFileBase fileupload)
        {
                // TODO: Add update logic here
            SANPHAM sp = db.SANPHAMs.Where(s => s.MASANPHAM == masp).Single();
                sp.TENSANPHAM = c["TENSANPHAM"];
                sp.MALOAI = c["MALOAI"];
                sp.MATHUONGHIEU = c["THUONGHIEU.MATHUONGHIEU"];
                sp.GIAGOC = double.Parse(c["GIAGOC"]);
                string now = DateTime.Now.ToShortDateString();
                sp.NGAYTAO = Convert.ToDateTime(now);
                sp.MOI = Convert.ToInt16(c["MOI"]);
                sp.GIAM = double.Parse(c["GIAM"]);
                sp.SLTON = Convert.ToInt32(c["SLTON"]);
                if (fileupload != null) {
                    string filename = Path.GetFileNameWithoutExtension(fileupload.FileName);
                    string extension = Path.GetExtension(fileupload.FileName);
                    filename = filename + extension;
                    sp.HINHMINHHOA = filename;
                    fileupload.SaveAs(Server.MapPath("~/Content/img/products/" + filename.ToString()));
                }
                //Insert kichthuoc
                List<string> kichthuoc = new List<string>();
                if (c["size1"] != null)
                {
                    kichthuoc.Add(c["size1"].ToString());
                }
                if (c["size2"] != null)
                {
                    kichthuoc.Add(c["size2"].ToString());
                }
                if (c["size3"] != null)
                {
                    kichthuoc.Add(c["size3"].ToString());
                }
                if (c["size4"] != null)
                {
                    kichthuoc.Add(c["size4"].ToString());
                }
                List<NEM_KICHTHUOC> kt = db.NEM_KICHTHUOCs.Where(kk => kk.MASANPHAM == masp).ToList();
                foreach (NEM_KICHTHUOC nkt in kt) {
                    db.NEM_KICHTHUOCs.DeleteOnSubmit(nkt);
                    db.SubmitChanges();
                }
                foreach (string k in kichthuoc)
                {
                    NEM_KICHTHUOC nktt = new NEM_KICHTHUOC();
                    nktt.MASANPHAM = masp.Trim(new Char[] { ' ', ',', '*', '.' });
                    nktt.KICHTHUOC = k;
                    db.NEM_KICHTHUOCs.InsertOnSubmit(nktt);
                    db.SubmitChanges();
                }
                db.SubmitChanges();
                return RedirectToAction("Index", "AdminMenu");
        }

        //
        // GET: /AdminMenu/Delete/5
        public ActionResult Delete(bool confirm, string masp)
        {
            if (Session["kh"] == null)
            {
                return RedirectToAction("Signin", "KhachHang");
            }
            KHACHHANG kh = Session["kh"] as KHACHHANG;
            if (kh.ROLE != 1)
            {
                return RedirectToAction("Index", "KhachHang");
            }
            if (confirm) {
                try
                {
                    List<NEM_KICHTHUOC> kt = db.NEM_KICHTHUOCs.Where(k => k.MASANPHAM == masp).ToList();
                    foreach (NEM_KICHTHUOC n in kt)
                    {
                        db.NEM_KICHTHUOCs.DeleteOnSubmit(n);
                        db.SubmitChanges();
                    }
                    var del = db.SANPHAMs.Single(s => s.MASANPHAM == masp);
                    db.SANPHAMs.DeleteOnSubmit(del);
                    db.SubmitChanges();
                    Session["xoa"] = true;
                    return RedirectToAction("Index");
                }
                catch
                {
                    return RedirectToAction("Index");
                }
            }
            return RedirectToAction("Index");
        }

        public ActionResult Logout() {
            Session.Remove("kh");
            return RedirectToAction("Index", "KhachHang");
        }
        //END PRODUCTS




        //BEGIN USER
        public ActionResult UserMng() {
            if (Session["kh"] == null)
            {
                return RedirectToAction("Signin", "KhachHang");
            }
            KHACHHANG k = Session["kh"] as KHACHHANG;
            if (k.ROLE != 1)
            {
                return RedirectToAction("Index", "KhachHang");
            }
            return View();
        }
        public JsonResult listUser()
        {

            return Json((from obj in db.KHACHHANGs select new { USERNAME = obj.USERNAME, MATKHAU = obj.MATKHAU, HOTEN = obj.HOTEN, TRANGTHAI = obj.TRANGTHAI }), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Add(KHACHHANG k) {
            int kq = 1;
            try
            {
                db.KHACHHANGs.InsertOnSubmit(k);
                db.SubmitChanges();
            }
            catch {
                kq = 0;
            }
            return Json(kq, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult changeStatus(string USERNAME)
        {
            int result = 1;
            KHACHHANG user = db.KHACHHANGs.Where(k => k.USERNAME == USERNAME).Single();
            try
            {
                if (user.TRANGTHAI == 1)
                {
                    user.TRANGTHAI = 0;
                    db.SubmitChanges();
                }
                else
                {
                    user.TRANGTHAI = 1;
                    db.SubmitChanges();
                }
            }
            catch {
                result = 0;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        //END USER
    }
}
