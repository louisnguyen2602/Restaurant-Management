﻿using RestaurantManagement.DAO;
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
    public partial class AccountProfile : Form
    {
        private Account loginAccount;
        public Account LoginAccount
        {
            get { return loginAccount; }
            set
            {
                loginAccount = value;
                ChangeAccount(loginAccount);
            }
        }

        public AccountProfile(Account acc)
        {
            InitializeComponent();
            LoginAccount = acc;
        }

        void ChangeAccount(Account acc)
        {
            txbUserName.Text = LoginAccount.UserName;
            txbDisplayName.Text = LoginAccount.DisplayName;
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private event EventHandler<AccountEvent> updateAccount;
        public event EventHandler<AccountEvent> UpdateAccount
        {
            add { updateAccount += value; }
            remove { updateAccount -= value; }
        }

        void UpdateAccountInfo()
        {
            string userName = txbUserName.Text;
            string displayName = txbDisplayName.Text;
            string passEncode = txbPassword.Text;
            string newPassEncode = txbNewPass.Text;
            string retypePass = txbRetypePass.Text;

            if(!newPassEncode.Equals(retypePass))
            {
                MessageBox.Show("Please enter password is correct with new password!","Notification!");
            }
            else
            {
                if(AccountDAO.Instance.UpdateAccount(userName,displayName,passEncode, newPassEncode))
                {
                    MessageBox.Show("Update successful!", "Notification!");
                    if(updateAccount != null)
                    {
                        updateAccount(this,new AccountEvent (AccountDAO.Instance.GetAccountByUserName(userName)));
                    }
                }
                else
                {
                    MessageBox.Show("Please fill correct password!", "Notification!");
                }
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            UpdateAccountInfo();
        }

        Boolean flag;
        int x, y;

        private void panel2_MouseDown(object sender, MouseEventArgs e)
        {
            flag = true;
            x = e.X;
            y = e.Y;
        }

        private void panel2_MouseUp(object sender, MouseEventArgs e)
        {
            flag = false;
        }
        
        private void panel2_MouseMove(object sender, MouseEventArgs e)
        {
            if (flag == true)
            {
                this.SetDesktopLocation(Cursor.Position.X - x, Cursor.Position.Y - y);
            }
        }
    }

    public class AccountEvent:EventArgs
    {
        private Account acc;
        public Account Acc
        {
            get { return acc; }
            set { acc = value; }
        }

        public AccountEvent(Account acc)
        {
            this.Acc = acc;
        }
    }
}
