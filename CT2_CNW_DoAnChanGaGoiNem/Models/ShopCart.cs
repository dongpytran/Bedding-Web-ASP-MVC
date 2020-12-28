using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CT2_CNW_DoAnChanGaGoiNem.Models;
namespace CT2_CNW_DoAnChanGaGoiNem.Models
{
    public class ShopCart
    {
        DataClasses1DataContext db = new DataClasses1DataContext();
        public List<CART_ITEM> list;
        public ShopCart() {
            list = new List<CART_ITEM>();
        }
        public ShopCart(List<CART_ITEM> listGh) {
            list = listGh;
        }
        public int SoMatHang()
        {
            if (list == null)
                return 0;
            return list.Count();
        }

        public int TongSLHang()
        {
            if (list == null)
                return 0;
            return list.Sum(s => s.SOLUONG);
        }

        public double TongThanhTien()
        {
            if (list == null)
                return 0;
            return list.Sum(s => s.THANHTIEN);
        }

        public int Them(string masp, string username, string tensp, string kt, double gia, string hinh, int sl)
        {
            CART_ITEM sp = list.SingleOrDefault(s => s.MASANPHAM == masp && s.KICHTHUOC == kt);
            bool hasitem = db.CART_ITEMs.Where(s => s.MASANPHAM == masp && s.USERNAME == username && s.KICHTHUOC == kt).Count() > 0;
            if (sp == null)
            {
                CART_ITEM item = new CART_ITEM();
                item.MASANPHAM = masp;
                item.USERNAME = username;
                item.SOLUONG = sl;
                item.TRANGTHAI = 0;
                item.GIA = gia;
                item.TENSANPHAM = tensp;
                item.HINH = hinh;
                item.THANHTIEN = long.Parse((item.SOLUONG * item.GIA).ToString());
                if (item == null)
                    return -1;
                if (kt != null)
                {
                    item.KICHTHUOC = kt;
                }
                else {
                    item.KICHTHUOC = "Không có";
                }
                list.Add(item);
                
                db.CART_ITEMs.InsertOnSubmit(item);
                db.SubmitChanges();
            }
            else
            {
                sp.SOLUONG++;
                sp.THANHTIEN = long.Parse((sp.SOLUONG * sp.GIA).ToString());
                CART_ITEM itemhas = db.CART_ITEMs.Where(s => s.MASANPHAM == masp && username == s.USERNAME && s.KICHTHUOC == kt).Single();
                itemhas.SOLUONG++;
                itemhas.THANHTIEN = long.Parse((itemhas.SOLUONG * itemhas.GIA).ToString());
                db.SubmitChanges();
            }
            return 1;
        }
    }
}