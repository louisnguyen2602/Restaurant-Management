using RestaurantManagement.DAO;
using RestaurantManagement.DTO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RestaurantManagement
{
    public partial class TableManager : Form
    {
        private Account loginAccount;           //Constructor
        public Account LoginAccount
        {
            get { return loginAccount; }
            set
            {
                loginAccount = value;
                ChangeAccount(loginAccount.Type);
            }
        }

        public TableManager(Account acc)
        {
            InitializeComponent();

            this.LoginAccount = acc;
            LoadTable();
            LoadCategory();
            LoadComboboxTable(cbSwitchTable);
        }

        #region Method
        void ChangeAccount(int type)
        {
            admintoolStripMenuItem.Enabled = type == 1;
            informationToolStripMenuItem.Text += " (" + LoginAccount.DisplayName +")";
        }

        void LoadCategory()
        {
            List<Category> listCategory = CategoryDAO.Instance.GetListCategory();
            cbCategory.DataSource = listCategory;
            cbCategory.DisplayMember = "Name";
        }

        void LoadFoodListByCategoryID(int id)
        {
            List<Food> listFood = FoodDAO.Instance.GetFoodByCategoryID(id);
            cbFood.DataSource = listFood;
            cbFood.DisplayMember = "Name";
        }

        void LoadTable()
        {
            flpTable.Controls.Clear();
            List<Table> tableList = TableDAO.Instance.LoadTableList();

            foreach (Table item in tableList)
            {
                Button btn = new Button()
                {
                    Width = TableDAO.TableWidth,
                    Height = TableDAO.TableHeight
                };

                //String status = item.Status != "NULL" ? item.Status : "Empty";
                btn.Text = item.Name + Environment.NewLine + item.Status;
                btn.Click += Btn_Click;
                btn.Tag = item;

                switch(item.Status)
                {
                    case "NULL":
                        btn.BackColor = Color.LightSkyBlue;
                        break;
                    default:
                        btn.BackColor = Color.IndianRed;
                        break;
                }
                flpTable.Controls.Add(btn);
            }
        }

        void ShowBill(int id)
        {
            lsvBill.Items.Clear();
            List<MenuFood> listBillinfor = MenuDAO.Instance.GetListMenuByTable(id);

            float totalPrice = 0;
            foreach(MenuFood item in listBillinfor)
            {
                ListViewItem lsvItem = new ListViewItem(item.FoodName.ToString());
                lsvItem.SubItems.Add(item.Count.ToString());
                lsvItem.SubItems.Add(item.Price.ToString());
                lsvItem.SubItems.Add(item.TotalPrice.ToString());

                totalPrice += item.TotalPrice;
                lsvBill.Items.Add(lsvItem);
            }

            CultureInfo culture = new CultureInfo("vi-VN");
            //Thread.CurrentThread.CurrentCulture = culture;
            txbTotalPrice.Text = totalPrice.ToString("c",culture);
        }

        void LoadComboboxTable(ComboBox cb)
        {
            cb.DataSource = TableDAO.Instance.LoadTableList();
            cb.DisplayMember = "Name"; 
        }

        void CheckOut()
        {
            Table table = lsvBill.Tag as Table;

            if (table == null)
            {
                MessageBox.Show("Please Choose Table.");
                return;
            }

            int idBill = BillDAO.Instance.GetUncheckBillIDByTableID(table.ID);
            int discount = (int)nmDisCount.Value;

            string pr = txbTotalPrice.Text.Split(',')[0].Replace(".", "");
            double totalPrice = Convert.ToDouble(pr);
            double finalTotalPrice = totalPrice - (totalPrice / 100) * discount;

            if (idBill != -1)
            {
                if (MessageBox.Show(string.Format("Are you sure to check out the bill of {0}\nTotalPrice: {1}\nDiscount: {2}\nFinalPrice: {3} ", table.Name, totalPrice, discount, finalTotalPrice), "Notification", MessageBoxButtons.OKCancel) == System.Windows.Forms.DialogResult.OK)
                {
                    BillDAO.Instance.CheckOut(idBill, discount, (float)finalTotalPrice);
                    ShowBill(table.ID);

                    LoadTable();
                }
            }
        }

        void AddFood()
        {
            Table table = lsvBill.Tag as Table;

            if (table == null)
            {
                MessageBox.Show("Please Choose Table.");
                return;
            }

            int idBill = BillDAO.Instance.GetUncheckBillIDByTableID(table.ID);
            int foodID = (cbFood.SelectedItem as Food).ID;
            int count = (int)nmFoodCount.Value;

            if (idBill == -1)        //TH1: bill ko tồn tại
            {
                BillDAO.Instance.InsertBill(table.ID);
                BillInfoDAO.Instance.InsertBillInfo(BillDAO.Instance.GetMaxIDBill(), foodID, count);
            }
            else                    //TH2: bill đã tồn tại
            {
                BillInfoDAO.Instance.InsertBillInfo(idBill, foodID, count);
            }
            ShowBill(table.ID);
            LoadTable();
        }

        void SwitchTabble()
        {
            Table table = lsvBill.Tag as Table;
            if (table == null)
            {
                MessageBox.Show("Please Choose Table.");
                return;
            }
            int id1 = (table).ID;
            int id2 = (cbSwitchTable.SelectedItem as Table).ID;
            if (MessageBox.Show(string.Format("Do you really want to switch {0} to {1}", (lsvBill.Tag as Table).Name, (cbSwitchTable.SelectedItem as Table).Name), "Notification!", MessageBoxButtons.OKCancel) == System.Windows.Forms.DialogResult.OK)
            {
                TableDAO.Instance.SwitchTable(id1, id2);
                LoadTable();
            }
        }
        #endregion

        #region Events
        void Btn_Click(object sender, EventArgs e)
        {
            int tableID = ((sender as Button).Tag as Table).ID;
            lsvBill.Tag = (sender as Button).Tag;
            ShowBill(tableID); 
        }

        private void personalInformationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AccountProfile f = new AccountProfile(LoginAccount);
            f.UpdateAccount += f_UpdateAccount;
            f.ShowDialog();
        }

        private void logOutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void f_UpdateAccount(object sender, AccountEvent e)
        {
            informationToolStripMenuItem.Text = "Information (" + e.Acc.DisplayName + ")";
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Admin f = new Admin();
            f.loginAccount = LoginAccount;
            f.InsertFood += f_InsertFood;
            f.DeleteFood += f_DeleteFood;
            f.EditFood += f_EditFood;
            f.InsertTable += f_InsertTable;
            f.DeleteTable += f_DeleteTable;
            f.EditTable += f_EditTable;

            f.ShowDialog();
        }

        void f_EditTable(object sender, EventArgs e)
        {
            LoadTable();
        }

        void f_DeleteTable(object sender, EventArgs e)
        {
            LoadTable();
        }

        void f_InsertTable(object sender, EventArgs e)
        {
            LoadTable();
        }

        void f_EditFood(object sender, EventArgs e)
        {
            LoadFoodListByCategoryID((cbCategory.SelectedItem as Category).ID);
            if (lsvBill.Tag != null)
            {
                ShowBill((lsvBill.Tag as Table).ID);
            }
        }

        void f_DeleteFood(object sender, EventArgs e)
        {
            LoadFoodListByCategoryID((cbCategory.SelectedItem as Category).ID);
            if (lsvBill.Tag != null)
            {
                ShowBill((lsvBill.Tag as Table).ID);
            }
            LoadTable();
        }

        void f_InsertFood(object sender, EventArgs e)
        {
            LoadFoodListByCategoryID((cbCategory.SelectedItem as Category).ID);
            if (lsvBill.Tag != null)
            {
                ShowBill((lsvBill.Tag as Table).ID);
            }
        }

        private void cbCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            int id = 0;
            ComboBox cb = sender as ComboBox;
            if (cb.SelectedItem == null)
            { return; }
            Category selected = cb.SelectedItem as Category;
            id = selected.ID;
            LoadFoodListByCategoryID(id);
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            AddFood();
        }

        private void btnCheckOut_Click(object sender, EventArgs e)
        {
            CheckOut();
        }

        private void btnSwitchTable_Click(object sender, EventArgs e)
        {
            SwitchTabble();
        }

        private void checkOutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CheckOut();
        }

        private void addFoodToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddFood();
        }

        private void switchTableToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SwitchTabble();
        }
        #endregion
    }
}
