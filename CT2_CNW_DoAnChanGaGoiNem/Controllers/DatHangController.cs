using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CT2_CNW_DoAnChanGaGoiNem.Models;
namespace CT2_CNW_DoAnChanGaGoiNem.Controllers
{
    public class DatHangController : Controller
    {
        DataClasses1DataContext db = new DataClasses1DataContext();
        //
        // GET: /DatHang/
        public ActionResult ThemMatHang(string masp, FormCollection c)
        {
            if (Session["kh"] == null)
            {
                return RedirectToAction("Signin", "KhachHang");
            }
            else
            {
                SANPHAM sptt = db.SANPHAMs.Where(sp => sp.MASANPHAM == masp).Single();
                double gia;
                if (sptt.GIAM == null) {
                    gia = sptt.GIAGOC ?? default(double);
                }else{
                    gia = sptt.GIABAN ?? default(double);
                }
                string kt = c["rdoKichThuoc"];
                if (kt == null) {
                    kt = "Không có";
                }
                int sl = int.Parse(c["txtQuantity"]);
                KHACHHANG k = Session["kh"] as KHACHHANG;
                List<CART_ITEM> gh = db.CART_ITEMs.Where(s => s.USERNAME == k.USERNAME).ToList();
                ShopCart shop = new ShopCart();
                if (gh != null)
                {
                    shop.list = gh;
                }
                CART_ITEM cc = db.CART_ITEMs.Where(ct => ct.MASANPHAM == masp && ct.USERNAME == k.USERNAME && ct.KICHTHUOC == kt).FirstOrDefault();
                if (cc == null)
                {
                    shop.Them(masp, k.USERNAME, sptt.TENSANPHAM, kt, gia, sptt.HINHMINHHOA, sl);
                }
                else {
                    cc.SOLUONG += sl;
                    db.SubmitChanges();
                }
                return Redirect(HttpContext.Request.UrlReferrer.AbsoluteUri);
            }
        }

        public ActionResult GioHang() {
            if (Session["kh"] == null)
            {
                return RedirectToAction("Signin", "KhachHang");
            }
            KHACHHANG kh = Session["kh"] as KHACHHANG;
            List<CART_ITEM> listCart = db.CART_ITEMs.Where(s => s.USERNAME == kh.USERNAME && s.TRANGTHAI == 0).ToList();
            ShopCart gh = new ShopCart(listCart);
            bool hasCart = db.CART_ITEMs.Where(k => k.USERNAME == kh.USERNAME).Count() > 0;
            if (!hasCart) {
                ViewBag.cart = "Giỏ Hàng Rỗng !";
            }
            return View(gh);
        }
        [HttpPost]
        public ActionResult EditQuantity(FormCollection c) {
            if (Session["kh"] == null)
            {
                return RedirectToAction("Signin", "KhachHang");
            }
            else {
                KHACHHANG kh = Session["kh"] as KHACHHANG;
                string[] quantities = c.GetValues("txtQuantity");
                List<CART_ITEM> listitem = db.CART_ITEMs.Where(s => s.USERNAME == kh.USERNAME && s.TRANGTHAI == 0).ToList();
                for (int i = 0; i < listitem.Count(); i++) {
                    listitem[i].SOLUONG = Convert.ToInt32(quantities[i]);
                    listitem[i].THANHTIEN = long.Parse((listitem[i].SOLUONG * listitem[i].GIA).ToString());
                    db.SubmitChanges();
                }
                ShopCart shop = new ShopCart(listitem);
                Session["gh"] = shop;
                return Redirect(HttpContext.Request.UrlReferrer.AbsoluteUri);
            }
        }

        //

        public ActionResult ClearCart(bool confirm) {
            if (Session["kh"] == null) {
                return RedirectToAction("Signin", "KhachHang");
            }
            KHACHHANG kh = Session["kh"] as KHACHHANG;
            if (confirm) {
                List<CART_ITEM> listCart = db.CART_ITEMs.Where(s => s.USERNAME == kh.USERNAME && s.TRANGTHAI == 0).ToList();
                foreach (CART_ITEM c in listCart)
                {
                    db.CART_ITEMs.DeleteOnSubmit(c);
                    
                }
                db.SubmitChanges();
                return Redirect(HttpContext.Request.UrlReferrer.AbsoluteUri);
            }
            return Redirect(HttpContext.Request.UrlReferrer.AbsoluteUri);
        }

        //
        public ActionResult DeleteCartItem(string masp, string kt) {
            if (Session["kh"] == null) {
                return RedirectToAction("Signin", "KhachHang");
            }
            KHACHHANG kh = Session["kh"] as KHACHHANG;
            CART_ITEM c = db.CART_ITEMs.Where(s => s.MASANPHAM == masp && s.USERNAME == kh.USERNAME && s.KICHTHUOC == kt).Single();
            db.CART_ITEMs.DeleteOnSubmit(c);
            db.SubmitChanges();
            return RedirectToAction("GioHang", "DatHang");
        }

        //
        [HttpGet]
        public ActionResult ThanhToan() {
            if(Session["kh"] == null){
                return RedirectToAction("Signin", "KhachHang");
            }
            KHACHHANG kh = Session["kh"] as KHACHHANG;
            List<CART_ITEM> list = db.CART_ITEMs.Where(s => s.USERNAME == kh.USERNAME && s.TRANGTHAI == 0).ToList();
            ShopCart sh = new ShopCart(list);
            return View(sh);
        }

        [HttpPost]
        public ActionResult ThanhToan(FormCollection c) {
            if (Session["kh"] == null) {
                return RedirectToAction("Signin", "KhachHang");
            }
            KHACHHANG kh = Session["kh"] as KHACHHANG;
            bool matchPass = db.KHACHHANGs.Where(k => k.USERNAME == kh.USERNAME && k.MATKHAU.Trim() == c["txtPassword"].ToString().Trim()).Count() > 0;
            if (matchPass == false)
            {
                Session["wrong"] = "Sai mật khẩu !";
                return Redirect(HttpContext.Request.UrlReferrer.AbsoluteUri);
            }
                List<CART_ITEM> list = db.CART_ITEMs.Where(sp=>sp.USERNAME == kh.USERNAME && sp.TRANGTHAI == 0).ToList();
                ShopCart s = new ShopCart(list);
                HOADON hd = new HOADON();
                hd.NGAYTAO = DateTime.Now;
                hd.USERNAME = kh.USERNAME;
                db.HOADONs.InsertOnSubmit(hd);
                db.SubmitChanges();
                foreach (CART_ITEM item in s.list) {
                    CHITIETHOADON ct = new CHITIETHOADON();
                    ct.MAHD = hd.MAHD;
                    ct.MASANPHAM = item.MASANPHAM;
                    ct.SOLUONG = item.SOLUONG;
                    ct.GIA = item.GIA;
                    ct.KICHTHUOC = item.KICHTHUOC;
                    db.CHITIETHOADONs.InsertOnSubmit(ct);
                    db.SubmitChanges();
                }
                foreach (CART_ITEM item in list) {
                    item.TRANGTHAI = 1;
                    db.SubmitChanges();
                }
                List<CART_ITEM> datt = db.CART_ITEMs.Where(tt => tt.USERNAME == kh.USERNAME && tt.TRANGTHAI == 1).ToList();
                foreach (CART_ITEM cit in datt) {
                    db.CART_ITEMs.DeleteOnSubmit(cit);
                    db.SubmitChanges();
                }
                db.SubmitChanges();
                Session["hd"] = "Đặt hàng thành công, quý khách vui lòng kiểm tra lại đơn hàng trong lịch sử giao dịch, xin cám ơn!";
                return RedirectToAction("Index", "KhachHang");
        }
	}
}