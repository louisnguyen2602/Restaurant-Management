using RestaurantManagement.DAO;
using RestaurantManagement.DTO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RestaurantManagement
{
    public partial class Admin : Form
    {
        BindingSource foodList = new BindingSource();
        BindingSource accountList = new BindingSource();
        BindingSource categoryList = new BindingSource();
        BindingSource tableList = new BindingSource();

        public Account loginAccount;

        public Admin()
        {
            InitializeComponent();
            Load();
        }

        #region methods
        new void Load()
        {
            dtgvFood.DataSource = foodList;
            dtgvAccount.DataSource = accountList;
            dtgvCategory.DataSource = categoryList;
            dtgvTable.DataSource = tableList;

            LoadDateTimePickerBill();
            LoadListBillByDate(dtpkFromDate.Value, dtpkToDate.Value);
            LoadListFood();
            LoadListCategory();
            LoadListTable();
            LoadAccount();
            LoadCategoryIntoCombobox(cbFoodCategory);
            AddFoodBinding();
            AddAccountBinding();
            AddCategoryBinding();
            AddTableBinding();
        }

        void AddAccountBinding()
        {
            txbUserName.DataBindings.Add(new Binding("Text", dtgvAccount.DataSource, "UserName", true, DataSourceUpdateMode.Never));
            txbDisplayName.DataBindings.Add(new Binding("Text", dtgvAccount.DataSource, "DisplayName", true, DataSourceUpdateMode.Never));
            nmType.DataBindings.Add(new Binding("Value", dtgvAccount.DataSource, "Type", true, DataSourceUpdateMode.Never));
        }

        void LoadAccount()
        {
            accountList.DataSource = AccountDAO.Instance.GetListAccount();
        }

        void LoadListBillByDate(DateTime checkIn, DateTime checkOut)
        {
           dtgvBill.DataSource = BillDAO.Instance.GetBillListByDate(checkIn, checkOut);
        }

        void LoadDateTimePickerBill()
        {
            DateTime today = DateTime.Now;
            dtpkFromDate.Value = new DateTime(today.Year, today.Month, 1);
            dtpkToDate.Value = dtpkFromDate.Value.AddMonths(1).AddDays(-1);
        }

        void LoadListFood()
        {
            foodList.DataSource = FoodDAO.Instance.GetListFood();
        }

        void LoadListCategory()
        {
            categoryList.DataSource = CategoryDAO.Instance.GetListCategory();
        }

        void LoadListTable()
        {
            tableList.DataSource = TableDAO.Instance.GetListTable();
        }

        void LoadCategoryIntoCombobox(ComboBox cb)
        {
            cb.DataSource = CategoryDAO.Instance.GetListCategory();
            cb.DisplayMember = "Name";
        }

        void AddFoodBinding()
        {
            txbFoodName.DataBindings.Add(new Binding("Text",dtgvFood.DataSource,"Name",true,DataSourceUpdateMode.Never));
            txbFoodID.DataBindings.Add(new Binding("Text", dtgvFood.DataSource, "ID", true, DataSourceUpdateMode.Never));
            nmFoodPrice.DataBindings.Add(new Binding("Value", dtgvFood.DataSource, "Price", true, DataSourceUpdateMode.Never));
        }

        void AddCategoryBinding()
        {
            txbCategoryName.DataBindings.Add(new Binding("Text", dtgvCategory.DataSource, "Name", true, DataSourceUpdateMode.Never));
            txbCategoryID.DataBindings.Add(new Binding("Text", dtgvCategory.DataSource, "ID", true, DataSourceUpdateMode.Never));
        }

        void AddTableBinding()
        {
            txbTableID.DataBindings.Add(new Binding("Text", dtgvTable.DataSource, "ID", true, DataSourceUpdateMode.Never));
            txbTableName.DataBindings.Add(new Binding("Text", dtgvTable.DataSource, "Name", true, DataSourceUpdateMode.Never));
            txbStatus.DataBindings.Add(new Binding("Text", dtgvTable.DataSource, "Status", true, DataSourceUpdateMode.Never));
        }

        List<Food> FindFoodByName(string name)
        {
            List<Food> listFood = FoodDAO.Instance.FindFoodByName(name);

            return listFood;
        }

        void AddAccount(string userName, string displayName, int type)
        {
            if(AccountDAO.Instance.InsertAccount(userName, displayName, type))
            {
                MessageBox.Show("Add Account Successful.","Notification!");
            }
            else
            {
                MessageBox.Show("Error.");
            }
            LoadAccount();
        }

        void EditAccount(string userName, string displayName, int type)
        {
            if (AccountDAO.Instance.UpdateAccount(userName, displayName, type))
            {
                MessageBox.Show("Edit Account Successful.", "Notification!");
            }
            else
            {
                MessageBox.Show("Error.");
            }
            LoadAccount();
            
        }

        void DeleteAccount(string userName)
        {
            if(loginAccount.UserName.Equals(userName))
            {
                MessageBox.Show("Please don't delete this account.", "Notification!");
                return;
            }
            if (AccountDAO.Instance.DeleteAccount(userName))
            {
                MessageBox.Show("Delete Account Successful.", "Notification!");
            }
            else
            {
                MessageBox.Show("Error.");
            }
            LoadAccount();
        }

        void ResetPass(string userName)
        {
            if (AccountDAO.Instance.ResetPassWord(userName))
            {
                MessageBox.Show("Reset PassWord Successful.", "Notification!");
            }
            else
            {
                MessageBox.Show("Error.");
            }
        }
        #endregion

        #region events
        private void btnView_Click(object sender, EventArgs e)
        {
            LoadListBillByDate(dtpkFromDate.Value, dtpkToDate.Value);
        }

        private void btnViewFood_Click(object sender, EventArgs e)
        {
            LoadListFood();
        }

        private void txbFoodID_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (dtgvFood.SelectedCells.Count > 0)
                {
                    int id = (int)dtgvFood.SelectedCells[0].OwningRow.Cells["CategoryID"].Value;

                    Category category = CategoryDAO.Instance.GetCategoryByID(id);

                    cbFoodCategory.SelectedItem = category;

                    int index = -1;
                    int i = 0;
                    foreach (Category item in cbFoodCategory.Items)
                    {
                        if (item.ID == category.ID)
                        {
                            index = i;
                            break;
                        }
                        i++;
                    }
                    cbFoodCategory.SelectedIndex = index;
                }
            }
            catch { }
        }

        private void btnAddFood_Click(object sender, EventArgs e)
        {
            string name = txbFoodName.Text;
            int categoryID = (cbFoodCategory.SelectedItem as Category).ID;
            float price = (float)nmFoodPrice.Value;

            if(FoodDAO.Instance.InsertFood(name,categoryID,price))
            {
                MessageBox.Show("Add Food Successful.", "Notification!");
                LoadListFood();
                if(insertFood != null)
                {
                    insertFood(this, new EventArgs());
                }
            }
            else
            {
                MessageBox.Show("Error.");
            }
        }

        private void btnEditFood_Click(object sender, EventArgs e)
        {
            string name = txbFoodName.Text;
            int categoryID = (cbFoodCategory.SelectedItem as Category).ID;
            float price = (float)nmFoodPrice.Value;
            int id = Convert.ToInt32(txbFoodID.Text);

            if (FoodDAO.Instance.UpdateFood(id,name, categoryID, price))
            {
                MessageBox.Show("Edit Food Successful.", "Notification!");
                LoadListFood();
                if(editFood != null)
                {
                    editFood(this, new EventArgs());
                }
            }
            else
            {
                MessageBox.Show("Error.");
            }
        }

        private void btnDeleteFood_Click(object sender, EventArgs e)
        {
            int id = Convert.ToInt32(txbFoodID.Text);

            if (FoodDAO.Instance.DeleteFood(id))
            {
                MessageBox.Show("Delete Food Successful.", "Notification!");
                LoadListFood();
                if(deleteFood != null)
                {
                    deleteFood(this, new EventArgs());
                }
            }
            else
            {
                MessageBox.Show("Error.");
            }
        }

        private event EventHandler insertFood;
        public event EventHandler InsertFood
        {
            add { insertFood += value; }
            remove { insertFood -= value; }
        }

        private event EventHandler deleteFood;
        public event EventHandler DeleteFood
        {
            add { deleteFood += value; }
            remove { deleteFood -= value; }
        }

        private event EventHandler editFood;
        public event EventHandler EditFood
        {
            add { editFood += value; }
            remove { editFood -= value; }
        }

        private void btnFindFood_Click(object sender, EventArgs e)
        {
            foodList.DataSource = FindFoodByName(txbFindFoodName.Text);
        }
        
        private void btnAddAccount_Click(object sender, EventArgs e)
        {
            string userName = txbUserName.Text;
            string displayName = txbDisplayName.Text;
            int type = (int)nmType.Value;

            AddAccount(userName, displayName, type);
        }

        private void btnEditAccount_Click(object sender, EventArgs e)
        {
            string userName = txbUserName.Text;
            string displayName = txbDisplayName.Text;
            int type = (int)nmType.Value;

            EditAccount(userName, displayName, type);
        }

        private void btnDeleteAccount_Click(object sender, EventArgs e)
        {
            string userName = txbUserName.Text;

            DeleteAccount(userName);
        }

        private void btnResetPassword_Click(object sender, EventArgs e)
        {
            string userName = txbUserName.Text;

            ResetPass(userName);
        }

        private void btnAddTable_Click(object sender, EventArgs e)
        {
            string name = txbTableName.Text;
            string status = txbStatus.Text;

            if (TableDAO.Instance.InsertTable(name, status))
            {
                MessageBox.Show("Add Table Successful.", "Notification!");
                if (insertTable != null)
                {
                    insertTable(this, new EventArgs());
                }
            }
            else
            {
                MessageBox.Show("Error.");
            }
            LoadListTable();
        }

        private void btnEditTable_Click(object sender, EventArgs e)
        {
            string name = txbTableName.Text;
            string status = txbStatus.Text;
            int id = Convert.ToInt32(txbTableID.Text);

            if (TableDAO.Instance.UpdateTable(id,name, status))
            {
                MessageBox.Show("Update Table Successful.", "Notification!");
                if (editTable != null)
                {
                    editTable(this, new EventArgs());
                }
            }
            else
            {
                MessageBox.Show("Error.");
            }
            LoadListTable();
        }

        private void btnDeleteTable_Click(object sender, EventArgs e)
        {
            int id = Convert.ToInt32(txbTableID.Text);

            if (TableDAO.Instance.DeleteTable(id))
            {
                MessageBox.Show("Delete Table Successful.", "Notification!");
                if(deleteTable != null)
                {
                    deleteTable(this, new EventArgs());
                }
            }
            else
            {
                MessageBox.Show("Error.");
            }
            LoadListTable();
        }

        private event EventHandler insertTable;
        public event EventHandler InsertTable
        {
            add { insertTable += value; }
            remove { insertTable -= value; }
        }

        private event EventHandler deleteTable;
        public event EventHandler DeleteTable
        {
            add { deleteTable += value; }
            remove { deleteTable -= value; }
        }

        private event EventHandler editTable;
        public event EventHandler EditTable
        {
            add { editFood += value; }
            remove { editFood -= value; }
        }

        private void btnViewAccount_Click(object sender, EventArgs e)
        {
            LoadAccount();
        }

        private void btnViewCategory_Click(object sender, EventArgs e)
        {
            LoadListCategory();
        }

        private void btnViewTable_Click(object sender, EventArgs e)
        {
            LoadListTable();
        }

        private void btnFirstBillPage_Click(object sender, EventArgs e)
        {
            txbPageBill.Text = "1";
        }

        private void btnLastBillPage_Click(object sender, EventArgs e)
        {
            int sumRecord = BillDAO.Instance.GetNumBillListByDate(dtpkFromDate.Value, dtpkToDate.Value);
            int lastPage = sumRecord / 10;

            if(sumRecord % 10 != 0)
            {
                lastPage++;
            }
            txbPageBill.Text = lastPage.ToString();
        }

        private void btnPrevious_Click(object sender, EventArgs e)
        {
            int page = Convert.ToInt32(txbPageBill.Text);

            if(page > 1)
            {
                page--;
            }
            txbPageBill.Text = page.ToString();
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            int page = Convert.ToInt32(txbPageBill.Text);
            int sumRecord = BillDAO.Instance.GetNumBillListByDate(dtpkFromDate.Value, dtpkToDate.Value);
            if(page < sumRecord)
            {
                page++;
            }
            txbPageBill.Text = page.ToString();
        }

        private void txbPageBill_TextChanged(object sender, EventArgs e)
        {
            dtgvBill.DataSource = BillDAO.Instance.GetBillListByDateAndPage(dtpkFromDate.Value, dtpkToDate.Value, Convert.ToInt32(txbPageBill.Text));
        }
        #endregion

    }
}
