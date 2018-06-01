using KargoTakip.Helper;
using KargoTakip.Properties;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KargoTakip.View
{
    public partial class entryForm : Form
    {
        private SqlConnection con = new SqlConnection("Data Source=FARUK;Initial Catalog=KargoTakip;User Id=root;Password=03102593");
        private SqlDataAdapter adapter;
        private SqlCommand cmd;
        private DataTable dt;
        private businessModel business = new businessModel();
        private helper helper = new helper();
        private customerModel customer = new customerModel();
        public static int login_id;
        public static string employeeName = "";
        //Variables
        private async Task<Boolean> addBusines()
        {
            try
            {
                adapter = new SqlDataAdapter("SELECT * FROM company WHERE companyName='" + business.bsName + "' and isDelete='No' and bossName='"+business.bossName+"'", con);
                dt = new DataTable();
                adapter.Fill(dt);
                garbageCollector();
                GC.SuppressFinalize(dt);
                if (dt.Rows.Count > 0)
                {
                    return false;
                }
                else
                {
                    await con.OpenAsync();
                    cmd = new SqlCommand("INSERT INTO company(companyName, bossName, bossPassword, secQuestion, secAnswer, companyMail, mailPassword,isDelete) VALUES(@company,@bossName,@bossPassword,@secQuestion,@secAnswer,@companyMail,@mailPassword,'No')", con);
                    cmd.Parameters.AddWithValue("@company", business.bsName);
                    cmd.Parameters.AddWithValue("@bossName", business.bossName);
                    cmd.Parameters.AddWithValue("@bossPassword", business.bossPass);
                    cmd.Parameters.AddWithValue("@secQuestion", business.secQuestion);
                    cmd.Parameters.AddWithValue("@secAnswer", business.secAnswer);
                    cmd.Parameters.AddWithValue("@companyMail", business.mailAddress);
                    cmd.Parameters.AddWithValue("@mailPassword", business.mailPassword);
                    await cmd.ExecuteNonQueryAsync();
                    return true;
                }
                
            }
            catch (Exception )
            {
                return false;
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                }
            }
        } // Firma Ekleme 
        private async Task<Boolean> addCustomer()
        {
            try
            {
                adapter = new SqlDataAdapter("SELECT * FROM customers WHERE username='"+customer.username+"' and tc='"+customer.tcNo+"'",con);
                dt = new DataTable();
                adapter.Fill(dt);
                garbageCollector();
                GC.SuppressFinalize(dt);
                if (dt.Rows.Count>0)
                {
                    return false;
                }
                else
                {
                    await con.OpenAsync();
                    cmd = new SqlCommand("INSERT INTO customers(nameSurname, username, password, tc, address, phone, mailAddress, isDelete) VALUES(@name,@username,@password,@tc,@address,@phone,@mailAdd,@isDelete)",con);
                    cmd.Parameters.AddWithValue("@name",customer.nameSurname);
                    cmd.Parameters.AddWithValue("@username",customer.username);
                    cmd.Parameters.AddWithValue("@password",customer.password);
                    cmd.Parameters.AddWithValue("@tc",customer.tcNo);
                    cmd.Parameters.AddWithValue("@address",customer.address);
                    cmd.Parameters.AddWithValue("@phone",customer.phoneNumber);
                    cmd.Parameters.AddWithValue("@mailAdd",customer.mailAddress);
                    cmd.Parameters.AddWithValue("@isDelete","No");
                    await cmd.ExecuteNonQueryAsync();
                    return true;
                }
                
            }catch(Exception)
            {
                return false;
            }
            finally
            {
                if(con.State == ConnectionState.Open)
                {
                    con.Close();
                }
            }
        } // Müşteri Ekleme
        private async Task loginBusiness() //Main Thread Üzerinde bulunan verilere erişim olmadığından çalışmayacak
        {
            //Task async giriş paneli
            //try
            //{
            //    await con.OpenAsync();
            //    adapter = new SqlDataAdapter("SELECT * FROM company WHERE bossName='" + user.username + "' and bossPassword='" + helper.crypto(user.password) + "'", con);
            //    dt = new DataTable();
            //    adapter.Fill(dt);
            //    garbageCollector();
            //    if (dt.Rows.Count > 0)
            //    {
            //        Form x = new adminPanel();
            //        this.Hide();
            //        x.Show();
            //    }
            //    else
            //    {
            //        MessageBox.Show("Kullanıcı Adı veya Şifre Hatalı!", "HATA", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //    }
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show(ex.Message, "HATA", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //}
            //finally
            //{
            //    if (con.State == ConnectionState.Open)
            //    {
            //        con.Close();
            //    }
            //}
        }
        private async Task<Boolean> setNewPassCompany() // Şirket Yöneticisi İçin Yeni Şifre Belirleme
        {
            try
            {
                adapter = new SqlDataAdapter("SELECT * FROM company WHERE bossName='"+business.bossName+"' and secAnswer='"+business.secAnswer+"'",con);
                dt = new DataTable();
                adapter.Fill(dt);
                garbageCollector();
                GC.SuppressFinalize(dt);
                if (dt.Rows.Count>0)
                {
                    await con.OpenAsync();
                    cmd = new SqlCommand("UPDATE company SET bossPassword='"+business.bossPass+"' WHERE bossName='"+business.bossName+"'",con);
                    await cmd.ExecuteNonQueryAsync();
                    return true;
                }
                else
                {
                    return false;
                }
            }catch(Exception)
            {
                return false;
            }
            finally
            {
                if(con.State == ConnectionState.Open)
                {
                    con.Close();
                }
            }
        }
        private async Task<Boolean> setNewPassCustomer()
        {
            try
            {
                adapter = new SqlDataAdapter("SELECT * FROM customers WHERE username='"+customer.username+"' and tc='"+customer.tcNo+"'",con);
                dt = new DataTable();
                adapter.Fill(dt);
                garbageCollector();
                GC.SuppressFinalize(dt);
                if (dt.Rows.Count>0)
                {
                    await con.OpenAsync();
                    cmd = new SqlCommand("UPDATE customers SET password='"+customer.password+"' WHERE username='"+customer.username+"' and tc='"+customer.tcNo+"'",con);
                    await cmd.ExecuteNonQueryAsync();
                    return true;
                }
                else
                {
                    return false;
                }
            }catch(Exception)
            {
                return false;
            }
            finally
            {
                if(con.State == ConnectionState.Open)
                {
                    con.Close();
                }
            }
        }
        public entryForm()
        {
            InitializeComponent();
        }
        private void entryForm_Load(object sender, EventArgs e)
        {
            txtUsername.Focus(); // İlgili textBox Odaklanıyor
            txtUsername.Text = Settings.Default["username"].ToString();
            txtPassword.Text = Settings.Default["password"].ToString();
        }
        private void tabPane1_SelectedPageIndexChanged(object sender, EventArgs e)
        {
            //İlgili Metod Çağırılıyor
            garbageCollector();
        } // Her Sayfa değiştiğinde Garbage Collector Çağırılsın
        private void garbageCollector()
        {
            //Garbage Collector Modülü Çalışıyor!
            GC.Collect();
            GC.WaitForPendingFinalizers();
        } // Bir takım bellek işlemleri yapıyor
        private async void simpleButton3_Click(object sender, EventArgs e)
        {
            //Şirket Kayıt
            if(bsTxtName.Text!=""&&bsBossName.Text!=""&&bsBossPass.Text!=""&&bsSecQuestion.Text!=""&&bsSecAnswer.Text!=""&&bsEmailAddress.Text!=""&&bsEmailAddressCbx.Text!=""&&bsEmailPassword.Text!="")
            {
                business.bsName = bsTxtName.Text;
                business.bossName = bsBossName.Text;
                business.bossPass = helper.crypto(bsBossPass.Text);
                business.secQuestion = bsSecQuestion.Text;
                business.secAnswer = bsSecAnswer.Text;
                business.mailAddress = bsEmailAddress.Text + bsEmailAddressCbx.Text;
                business.mailPassword = bsEmailPassword.Text;
                bool isOk = false;
                isOk = await Task.Run(addBusines);
                if(isOk == true)
                {
                    MessageBox.Show("Firma Başarıyla Oluşturuldu!","BAŞARILI",MessageBoxButtons.OK);
                }
                else
                {
                    MessageBox.Show("Firma Oluşturulamadı!","HATA",MessageBoxButtons.OK,MessageBoxIcon.Error);
                }
                bsTxtName.Text = bsBossName.Text = bsBossPass.Text = bsSecQuestion.Text = bsSecAnswer.Text = bsEmailAddress.Text = bsEmailAddressCbx.Text = bsEmailPassword.Text = "";
                tabPanel1.SelectedPageIndex = 0;

            }
            else
            {
                MessageBox.Show("Eksik Yerleri Lütfen Doldurunuz!","HATA",MessageBoxButtons.OK,MessageBoxIcon.Error);
            }
        } // Şirket Kayıt
        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            //Şifremi Göster Firma İçin
            if(bsBossPass.UseSystemPasswordChar == true)
            {
                bsBossPass.UseSystemPasswordChar = false;
            }
            else
            {
                bsBossPass.UseSystemPasswordChar = true;
            }
        } // Şifremi Göster Firma İçin
        private void simpleButton2_Click(object sender, EventArgs e)
        {
            //Şirket Girişi
            if(txtUsername.Text!=""&&txtPassword.Text!="")
            {
                try
                {
                    con.Open();
                    adapter = new SqlDataAdapter("SELECT * FROM company WHERE bossName='" + txtUsername.Text + "' and bossPassword='" + helper.crypto(txtPassword.Text) + "' and isDelete='No'", con);
                    dt = new DataTable();
                    adapter.Fill(dt);
                    garbageCollector();
                    if (dt.Rows.Count > 0)
                    {
                        login_id = Convert.ToInt32(dt.Rows[0][0].ToString());
                        Form x = new adminPanel();
                        this.Hide();
                        x.Show();
                    }
                    else
                    {
                        MessageBox.Show("Kullanıcı Adı veya Şifre Hatalı!", "HATA", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "HATA", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    if (con.State == ConnectionState.Open)
                    {
                        con.Close();
                    }
                }
            }
            else
            {
                MessageBox.Show("Eksik Yerleri Doldurunuz!", "HATA", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        } // Firma Giriş
        private async void simpleButton4_Click(object sender, EventArgs e)
        {
            if(customerName.Text != ""&&customerUsername.Text!=""&&customerPassword.Text!=""&&customerTC.Text!=""&&customerAddress.Text!=""&&customerPhone.MaskFull!=false&&customerMail.Text!=""&&customerMailCbx.Text!=""&&customerTC.TextLength==11)
            {
                customer.nameSurname = customerName.Text;
                customer.username = customerUsername.Text;
                customer.password = helper.crypto(customerPassword.Text);
                customer.tcNo = customerTC.Text;
                customer.address = customerAddress.Text;
                customer.phoneNumber = customerPhone.Text;
                customer.mailAddress = customerMail.Text + customerMailCbx.Text;
                bool isOk = false;
                isOk = await Task.Run(addCustomer);
                if(isOk == true)
                {
                    MessageBox.Show("Kullanıcı Başarıyla Oluşturuldu!","BAŞARILI",MessageBoxButtons.OK);
                }
                else
                {
                    MessageBox.Show("Kullanıcı Oluşturulamadı!","HATA",MessageBoxButtons.OK,MessageBoxIcon.Error);
                }
                customerName.Text = customerUsername.Text = customerPassword.Text = customerTC.Text = customerAddress.Text = customerPhone.Text = customerMail.Text = customerMailCbx.Text = "";
            }
            else
            {
                MessageBox.Show("İlgili Yerleri Doldurunuz","HATA",MessageBoxButtons.OK,MessageBoxIcon.Error);
            }
        } // Müşteri Üye
        private void simpleButton5_Click(object sender, EventArgs e) // Çalışan Girişi
        {
            if(txtUsername.Text!=""&&txtPassword.Text!="")
            {
                try
                {
                    adapter = new SqlDataAdapter("SELECT * FROM employee WHERE username='"+txtUsername.Text+"' and password='"+helper.crypto(txtPassword.Text)+"'",con);
                    dt = new DataTable();
                    adapter.Fill(dt);
                    if(dt.Rows.Count>0)
                    {
                        login_id = Convert.ToInt32(dt.Rows[0][0].ToString());
                        employeeName = txtUsername.Text;
                        Form x = new employeeForm();
                        GC.Collect();
                        GC.WaitForPendingFinalizers();
                        GC.SuppressFinalize(dt);
                        x.Show();
                        this.Hide();
                    }
                    else
                    {
                        MessageBox.Show("Kullanıcı adı veya şifre yanlış!","HATA",MessageBoxButtons.OK,MessageBoxIcon.Error);
                    }
                }catch(Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            else
            {
                MessageBox.Show("Lütfen ilgili yerleri doldurunuz!","HATA",MessageBoxButtons.OK,MessageBoxIcon.Error);
            }
        }
        private void checkBox3_CheckedChanged(object sender, EventArgs e)//Kullanıcı Ekle Şifremi Göster
        {
            if(customerPassword.UseSystemPasswordChar == true)
            {
                customerPassword.UseSystemPasswordChar = false;
            }
            else
            {
                customerPassword.UseSystemPasswordChar = true;
            }
        }
        private void simpleButton1_Click(object sender, EventArgs e) // Müşteri Giriş
        {
            if(txtUsername.Text!=""&&txtPassword.Text!="")
            {
                try
                {
                    dt = new DataTable();
                    adapter = new SqlDataAdapter("SELECT * FROM customers WHERE username='"+txtUsername.Text+"' and password='"+helper.crypto(txtPassword.Text)+"' and isDelete='No'",con);
                    adapter.Fill(dt);
                    garbageCollector();
                    if(dt.Rows.Count>0)
                    {
                        login_id = Convert.ToInt32(dt.Rows[0][0].ToString());
                        GC.SuppressFinalize(dt);
                        Form x = new customerForm();
                        x.Show();
                        this.Hide();
                    }
                    else
                    {
                        MessageBox.Show("Kullanıcı Adı veya Şifre Hatalı","HATA",MessageBoxButtons.OK,MessageBoxIcon.Error);
                    }
                    GC.SuppressFinalize(dt);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                
            }
            else
            {
                MessageBox.Show("Lütfen Kullanıcı Adı ve Şifrenizi Eksiksiz Giriniz!","HATA",MessageBoxButtons.OK,MessageBoxIcon.Error);
            }
        }
        private void checkBox4_CheckedChanged(object sender, EventArgs e) // Beni Hatırla
        {
            Settings.Default["username"] = txtUsername.Text;
            Settings.Default["password"] = txtPassword.Text;
            Settings.Default.Save();
            MessageBox.Show("Verileriniz Kaydedildi!", "BAŞARILI", MessageBoxButtons.OK);
        }
        private void checkBox1_CheckedChanged(object sender, EventArgs e) // Giriş Şifremi Göster
        {
            if(txtPassword.UseSystemPasswordChar == true)
            {
                txtPassword.UseSystemPasswordChar = false;
            }
            else
            {
                txtPassword.UseSystemPasswordChar = true;
            }
        }
        private async void simpleButton6_Click(object sender, EventArgs e) // Şifreyi Yenile Şirketler için
        {
            if(forgetBossName.Text!=""&&forgetSecQuestion.Text!=""&&forgetNewPass.Text!=""&&forgetNewPassRp.Text!="")
            {
                if(forgetNewPass.Text == forgetNewPassRp.Text)
                {
                    business.bossName = forgetBossName.Text;
                    business.secAnswer = forgetSecQuestion.Text;
                    business.bossPass = helper.crypto(forgetNewPassRp.Text);
                    bool isOk =false;
                    isOk = await Task.Run(setNewPassCompany);
                    if(isOk == true)
                    {
                        MessageBox.Show("Şifre Başarıyla Değiştirildi!","BAŞARILI",MessageBoxButtons.OK);
                    }
                    else
                    {
                        MessageBox.Show("Şifre Değiştirilemedi!","BAŞARISIZ",MessageBoxButtons.OK,MessageBoxIcon.Error);
                    }
                    forgetBossName.Text = forgetSecQuestion.Text = forgetNewPass.Text = forgetNewPassRp.Text = "";
                    tabPanel1.SelectedPageIndex = 0;
                }
                else
                {
                    MessageBox.Show("Şifreler Birbiriyle Uyuşmuyor!","HATA",MessageBoxButtons.OK,MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Lütfen Gerekli Bilgileri Doldurunuz!","HATA",MessageBoxButtons.OK,MessageBoxIcon.Error);
            }
        }
        private async void simpleButton7_Click(object sender, EventArgs e) // Şifreyi Yenile Müşteri
        {
            if(forgetCustomerName.Text!=""&&forgetCustomerTc.Text!=""&&forgetCustomerPass.Text!=""&&forgetCustomerPassRp.Text!="")
            {
                if(forgetCustomerPass.Text == forgetCustomerPassRp.Text)
                {
                    customer.username = forgetCustomerName.Text;
                    customer.tcNo = forgetCustomerTc.Text;
                    customer.password = helper.crypto(forgetCustomerPass.Text);
                    bool isOk = false;
                    isOk = await Task.Run(setNewPassCustomer);
                    if(isOk == true)
                    {
                        MessageBox.Show("Şifre Başarıyla Değiştirildi!","BAŞARILI",MessageBoxButtons.OK);
                    }
                    else
                    {
                        MessageBox.Show("Şifre Değiştirilemedi!","HATA",MessageBoxButtons.OK,MessageBoxIcon.Error);
                    }
                    forgetCustomerName.Text = forgetCustomerTc.Text = forgetCustomerPass.Text = forgetCustomerPassRp.Text = "";
                    tabPanel1.SelectedPageIndex = 0;
                }
                else
                {
                    MessageBox.Show("Şifreler birbirleriyle uyuşmuyor!","HATA",MessageBoxButtons.OK,MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Lütfen ilgili verileri eksiksiz doldurunuz!","HATA",MessageBoxButtons.OK,MessageBoxIcon.Error);
            }
        }

        private void entryForm_FormClosing(object sender, FormClosingEventArgs e) // Uygulama Kapatılacak
        {
            Application.Exit();
        }
    }
}
