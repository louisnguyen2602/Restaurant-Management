﻿using RestaurantManagement.DTO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantManagement.DAO
{
    public class BillDAO
    {
        private static BillDAO instance;

        public static BillDAO Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new BillDAO();
                }
                return BillDAO.instance;
            }
            private set
            {
                instance = value;
            }
        }

        private BillDAO()
        { }

        /// <summary>
        /// Success bill ID
        /// Fail -1
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public int GetUncheckBillIDByTableID(int id)
        {
            DataTable data = DataProvider.Instance.ExecuteQuery("select * from dbo.Bill where idTable = " + id +" and status = 0");

            if (data.Rows.Count>0)          //số trường trả về > 0
            {
                Bill bill = new Bill(data.Rows[0]);
                return bill.ID;             // lấy id của bill
            }
            return -1;                      // id = -1 là ko có j hết
        }

        public void InsertBill(int id)
        {
            DataProvider.Instance.ExecuteNonQuery("exec usp_InsertBill @idTable", new object[] { id });
        }

        public int GetMaxIDBill()
        {
            try
            {
                return (int)DataProvider.Instance.ExecuteScalar("select max(id) from dbo.Bill");
            }
            catch
            {
                return -1;
            }
        }

        public void CheckOut(int id, int discount,float totalPrice)
        {
            string query = "update dbo.Bill set dateCheckOut = GETDATE(), status = 1, " + " discount = " + discount + ", totalPrice = " + totalPrice + " where id = " + id;
            DataProvider.Instance.ExecuteNonQuery(query);
        }

        public DataTable GetBillListByDate(DateTime checkIn,DateTime checkOut)
        {
            return DataProvider.Instance.ExecuteQuery("exec usp_GetListBillByDate @checkIn, @checkOut", new object[] { checkIn, checkOut });
        }

        public DataTable GetBillListByDateAndPage(DateTime checkIn, DateTime checkOut, int pageNum)
        {
            return DataProvider.Instance.ExecuteQuery("exec usp_GetListBillByDateAndPage @checkIn, @checkOut, @page", new object[] { checkIn, checkOut, pageNum });
        }

        public int GetNumBillListByDate(DateTime checkIn, DateTime checkOut)
        {
            return (int)DataProvider.Instance.ExecuteScalar("exec usp_GetNumBillByDate @checkIn, @checkOut", new object[] { checkIn, checkOut });
        }

        public void DeleteBillByIdTable(int id)
        {
            DataProvider.Instance.ExecuteQuery("delete dbo.Bill where idTable =  " + id);
        }
    }
}
