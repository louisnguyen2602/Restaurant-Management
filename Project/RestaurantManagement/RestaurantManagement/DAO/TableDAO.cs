using RestaurantManagement.DTO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantManagement.DAO
{
    public class TableDAO
    {
        private static TableDAO instance;
        public static TableDAO Instance
        {
            get
            {
                if (instance == null)
                    instance = new TableDAO();
                return TableDAO.instance;
            }
            private set
            {
                TableDAO.instance = value;
            }
        }

        public static int TableWidth = 117;
        public static int TableHeight = 117;
        private TableDAO()
        { }
        public List<Table> LoadTableList()
        {
            List<Table> tableList = new List<Table>();

            DataTable data = DataProvider.Instance.ExecuteQuery("usp_GetTableList");

            foreach(DataRow item in data.Rows)          // trong danh sách dòng lấy ra từng dòng
            {
                Table table = new Table(item);
                tableList.Add(table);
            }

            return tableList;
        }

        public void SwitchTable(int id1,int id2)
        {
            DataProvider.Instance.ExecuteQuery("usp_SwitchTable @idTable1, @idTable2", new object[] { id1, id2 });
        }

        public List<Table> GetListTable()
        {
            List<Table> list = new List<Table>();

            string query = "select * from FoodTable";

            DataTable data = DataProvider.Instance.ExecuteQuery(query);
            foreach (DataRow item in data.Rows)
            {
                Table Table = new Table(item);
                list.Add(Table);
            }
            return list;
        }

        public bool InsertTable(string name, string status)
        {
            string query = string.Format("insert dbo.FoodTable (Name, Status) values (N'{0}',N'{1}')", name, status);

            int result = DataProvider.Instance.ExecuteNonQuery(query);
            return result > 0;
        }

        public bool UpdateTable(int id, string name, string status)
        {
            string query = string.Format("update dbo.FoodTable set Name = N'{1}', Status = N'{2}' where ID = {0}",id, name, status );

            int result = DataProvider.Instance.ExecuteNonQuery(query);
            return result > 0;
        }

        public bool DeleteTable(int id)
        {
            BillDAO.Instance.DeleteBillByIdTable(id);
            string query = string.Format("Delete FoodTable where id = {0}", id);

            int result = DataProvider.Instance.ExecuteNonQuery(query);
            return result > 0;
        }
    }
}
