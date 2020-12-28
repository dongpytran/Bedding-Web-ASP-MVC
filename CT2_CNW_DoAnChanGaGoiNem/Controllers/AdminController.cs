using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CT2_CNW_DoAnChanGaGoiNem.Models;
using System.IO;

namespace CT2_CNW_DoAnChanGaGoiNem.Controllers
{
    public class AdminController : Controller
    {
        //
        // GET: /Admin/

        DataClasses1DataContext db = new DataClasses1DataContext();
        public ActionResult Dashboard()
        {
            return View();
        }

        public ActionResult Product() {
            return View();
        }
        IEnumerable<SANPHAM> getAll() {
            return db.SANPHAMs.ToList<SANPHAM>();
        }
        public ActionResult Products() {
            List<SANPHAM> lsp = db.SANPHAMs.ToList();
            return View(lsp);
        }
        [HttpGet]
        public ActionResult AddOrEdit(string masp) {
            SANPHAM sp = new SANPHAM();
            //KichThuoc
            List<string> ktt = db.NEM_KICHTHUOCs.Select(ss => ss.KICHTHUOC).Distinct().ToList();
            ViewBag.kt = ktt;

            //DropDown LOAI, TEN THUONG HIEU
            List<LOAI> l = db.LOAIs.ToList();
            List<THUONGHIEU> th = db.THUONGHIEUs.ToList();
            ViewBag.loai = l;
            ViewBag.th = th;
            if (masp != null) {
                sp = db.SANPHAMs.Where(s => s.MASANPHAM == masp).FirstOrDefault<SANPHAM>();
                ViewBag.img = sp.HINHMINHHOA;
            }
            return View(sp);
        }
        [HttpPost]
        public ActionResult AddOrEdit(string masp, FormCollection c, HttpPostedFileBase fileupload)
        {


                /*if (fileupload != null)
                {
                    if (masp == null)
                    {
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
                    }
                    else
                    {
                        string ma = masp.Trim(new Char[] { ' ', ',', '*', '.' });
                        SANPHAM sp = db.SANPHAMs.Where(sq=>sq.MASANPHAM == ma).SingleOrDefault();
                        sp.TENSANPHAM = c["TENSANPHAM"];
                        db.SubmitChanges();
                    }
                }
                else {
                    if (masp != null) {
                        string ma = masp.Trim(new Char[] { ' ', ',', '*', '.' });
                        SANPHAM ssp = db.SANPHAMs.Where(sw => sw.MASANPHAM == ma).SingleOrDefault();
                        ssp.TENSANPHAM = c["TENSANPHAM"];
                        ssp.MALOAI = c["MALOAI"];
                        ssp.MATHUONGHIEU = c["THUONGHIEU"];
                        ssp.GIAGOC = double.Parse(c["GIAGOC"]);
                        string now = DateTime.Now.ToShortDateString();
                        ssp.NGAYTAO = Convert.ToDateTime(now);
                        ssp.MOI = Convert.ToInt16(c["MOI"]);
                        ssp.GIAM = double.Parse(c["GIAM"]);
                        db.SubmitChanges();
                        
                    }
                }

                return RedirectToAction("Product", "Admin", new { success = true, message = "Success" });
                 * */
            if (masp != null)
            {
                string ma = masp.Trim(new Char[] { ' ', ',', '*', '.' });
                SANPHAM ssp = db.SANPHAMs.Where(sw => sw.MASANPHAM == ma).SingleOrDefault();
                ssp.TENSANPHAM = c["TENSANPHAM"];
                db.SubmitChanges();
            }
            else {
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
            }
            return RedirectToAction("Product", "Admin");
        }
    }
}