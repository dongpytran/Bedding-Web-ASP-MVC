using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList.Mvc;
using PagedList;
using CT2_CNW_DoAnChanGaGoiNem.Models;
namespace CT2_CNW_DoAnChanGaGoiNem.Controllers
{
    public class ProductController : Controller
    {
        DataClasses1DataContext db = new DataClasses1DataContext();
        //
        // GET: /Product/
        public ActionResult Category(string sale, string searchBy, string maloai, string math, int? page)
        {
            List<THUONGHIEU> lth = db.THUONGHIEUs.ToList();
            ViewBag.th = lth;
            List<LOAI> ll = db.LOAIs.ToList();
            ViewBag.loai = ll;
            List<string> lkt = db.NEM_KICHTHUOCs.Select(n=>n.KICHTHUOC).Distinct().ToList();
            ViewBag.kt = lkt;
            if (Session["kh"] != null)
            {
                KHACHHANG k = Session["kh"] as KHACHHANG;
                List<SANPHAM> listSpThich = (from s in db.SANPHAMs
                                             join t in db.SPTHICHes
                                             on s.MASANPHAM equals t.MASANPHAM
                                             where t.USERNAME == k.USERNAME
                                             select s).ToList();
                ViewBag.thich = listSpThich;
            }
            if (Session["search"] != null) {
                string search = Session["search"].ToString();
                ViewBag.count = db.SANPHAMs.Where(s => s.TENSANPHAM.Contains(search)).ToList();
                return View(db.SANPHAMs.Where(s => s.TENSANPHAM.Contains(search)).ToList().ToPagedList(page ?? 1, 12));
            }
            
            if (sale == "true") {
                return View(db.SANPHAMs.Where(s => s.GIAM != 0).OrderByDescending(sp=>sp.GIAM).ToList().ToPagedList(page ?? 1, 12));
            }
            if (searchBy != null) {
                List<NEM_KICHTHUOC> lnkt = db.NEM_KICHTHUOCs.Where(n => n.KICHTHUOC == searchBy).ToList();
                List<SANPHAM> lsp = new List<SANPHAM>();
                foreach (NEM_KICHTHUOC nk in lnkt) {
                    SANPHAM s = db.SANPHAMs.Where(sp => sp.MASANPHAM == nk.MASANPHAM).Single();
                    lsp.Add(s);
                }
                return View(lsp.ToPagedList(page ?? 1, 12));
            }
            if (maloai != null && math == null)
            {
                return View(db.SANPHAMs.Where(s => s.MALOAI == maloai).ToList().ToPagedList(page ?? 1,12));
            }
            else if (maloai != null && math != null) {
                return View(db.SANPHAMs.Where(s => s.MALOAI == maloai && s.MATHUONGHIEU == math).ToList().ToPagedList(page ?? 1, 12));
            }
            else
            {
                return View(db.SANPHAMs.OrderByDescending(s=>s.NGAYTAO).ToList().ToPagedList(page ?? 1, 12));
            }
        }

        public ActionResult CategoriesOp()
        {
            return PartialView();
        }
        public ActionResult ProductDetail(string masp)
        {
            if (masp == null) {
               return RedirectToAction("Index", "KhachHang");
            }
            SANPHAM s = db.SANPHAMs.Where(sp => sp.MASANPHAM == masp).Single();
            THUONGHIEU t = db.THUONGHIEUs.Where(th => th.MATHUONGHIEU == s.MATHUONGHIEU).Single();
            List<NEM_KICHTHUOC> kth = db.NEM_KICHTHUOCs.Where(kt => kt.MASANPHAM == masp).ToList();
            ViewBag.kichthuoc = kth;
            ViewBag.thuonghieu = t;
            if (Session["kh"] != null)
            {
                KHACHHANG k = Session["kh"] as KHACHHANG;
                List<SANPHAM> thich = new List<SANPHAM>();
                var data = from sp in db.SANPHAMs
                           join th in db.SPTHICHes
                           on sp.MASANPHAM equals th.MASANPHAM
                           where th.USERNAME == k.USERNAME
                           select sp;
                foreach (var item in data)
                {
                    SANPHAM ss = item as SANPHAM;
                    thich.Add(ss);
                }
                ViewBag.thich = thich;
            }
            List<SANPHAM> related = db.SANPHAMs.Where(sp => sp.MALOAI == s.MALOAI && sp.MASANPHAM != s.MASANPHAM).Take(4).ToList();
            ViewBag.related = related;
            //lay desc
            NEM_DACDIEM ldd = db.NEM_DACDIEMs.Where(dd => dd.MASANPHAM == masp).SingleOrDefault();
            if (ldd != null)
            {
                ViewBag.dd = ldd;
            }
            return View(s);
        }
        public ActionResult AddLoveProduct(string masp) {
            if(Session["kh"] == null){
                return RedirectToAction("Signin", "KhachHang");
            }
            KHACHHANG k = Session["kh"] as KHACHHANG;
            SPTHICH s = new SPTHICH();
            bool check = db.SPTHICHes.Where(sp => sp.MASANPHAM == masp && sp.USERNAME == k.USERNAME).Count() > 0;
            if (check)
            {
                SPTHICH sp = db.SPTHICHes.Where(ss => ss.MASANPHAM == masp && ss.USERNAME == k.USERNAME).Single();
                db.SPTHICHes.DeleteOnSubmit(sp);
                db.SubmitChanges();
                return Redirect(Request.UrlReferrer.ToString());
            }
            else {
                s.MASANPHAM = masp;
                s.USERNAME = k.USERNAME;
                db.SPTHICHes.InsertOnSubmit(s);
                db.SubmitChanges();
                return Redirect(HttpContext.Request.UrlReferrer.AbsoluteUri);
            }
        }
        [HttpPost]
        public ActionResult Search(FormCollection c) {
            string search = c["txtSearch"];
            Session["search"] = search;
            return RedirectToAction("Category", "Product");
        }
	}
}