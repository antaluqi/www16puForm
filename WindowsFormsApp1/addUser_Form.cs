using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WWW16pu;
using Global;

namespace WindowsFormsApp1
{
    public partial class addUser_Form : Form
    {
        public addUser_Form()
        {
            InitializeComponent();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void ok_button_Click(object sender, EventArgs e)
        {
            if(username_textBox.Text!=null || password_textBox.Text!=null)
            {
                if (GlobalV.userList.Where(ul => { return ul.Obj.username == username_textBox.Text; }).Count() > 0)
                {
                    MessageBox.Show(username_textBox.Text + "已在列表中");
                    return;
                }
                www16pu user = new www16pu(username_textBox.Text, password_textBox.Text);
                string reStr="";
                if (user.Login(ref reStr))
                {
                    main_Form main_form = (main_Form)this.Owner;
                    main_form.addUserList(user);
                    Close();
                }
                else
                {
                    MessageBox.Show(reStr);
                }
            }
        }

        private void cancel_button_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
