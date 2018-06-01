using DevExpress.LookAndFeel;
using KargoTakip.Helper;
using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Printing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KargoTakip.View
{
    public partial class adminPanel : Form
    {
        private SqlConnection con = new SqlConnection("Data Source=FARUK;Initial Catalog=KargoTakip;User Id=root;Password=03102593");
        private SqlDataAdapter adapter;
        private SqlCommand cmd;
        private DataTable dt;
        private businessModel business = new businessModel();
        private employeeModel employee = new employeeModel();
        private orderModel order = new orderModel();
        private helper helper = new helper();
        //Variables
        public adminPanel()
        {
            InitializeComponent();
        }
        private async Task<Boolean> addEmployee()
        {
            try
            {
                adapter = new SqlDataAdapter("SELECT * FROM employee WHERE username='"+employee.username+"' and isDelete='No'",con);
                dt = new DataTable();
                adapter.Fill(dt);
                if(dt.Rows.Count>0)
                {
                    return false;
                }
                else
                {
                    int companyId = entryForm.login_id;
                    await con.OpenAsync();
                    cmd = new SqlCommand("INSERT INTO employee(businessId, nameSurname, username, password, address, phone, isDelete) VALUES('"+companyId+"','"+employee.nameSurname+"','"+employee.username+"','"+employee.password+"','"+employee.address+"','"+employee.phoneNumber+"','No')",con);
                    await cmd.ExecuteNonQueryAsync();
                    employee.password = null;
                    return true;
                }
            }
            catch(Exception)
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
        } // çalışan ekleme
        private async Task<Boolean> deleteEmployee() // çalışan silme
        {
            try
            {
                await con.OpenAsync();
                cmd = new SqlCommand("UPDATE employee SET isDelete='Yes' Where username='"+employee.username+"'",con);
                await cmd.ExecuteNonQueryAsync();
                return true;
            }catch(Exception)
            {
                return false;
            }
            finally
            {
                if(con.State == ConnectionState.Open)
                {
                    con.Close();
                    employee.username = null;
                }
            }
        }
        private async Task<Boolean> updateEmployee()
        {
            try
            {
                adapter = new SqlDataAdapter("SELECT password FROM employee WHERE businessId='"+entryForm.login_id+"' and username='"+employee.username+"' and isDelete='No'",con);
                dt = new DataTable();
                adapter.Fill(dt);
                if(dt.Rows.Count>0)
                {
                    string pass = dt.Rows[0][0].ToString();
                    await con.OpenAsync();
                    cmd = new SqlCommand("UPDATE employee SET address='" + employee.address + "', phone='" + employee.phoneNumber + "', password='" + pass + "' WHERE username='" + employee.username + "'", con);
                    await cmd.ExecuteNonQueryAsync();
                    employee.password = null;
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch(Exception)
            {
                return false;
            }
            finally
            {
                if(con.State == ConnectionState.Open)
                {
                    con.Close();
                    employee.address = employee.phoneNumber = employee.password = employee.username = null;
                }
            }
        } // Çalışan adres ve telefon güncelleme
        private async Task<Boolean> setNewEmployeePass() //çalışana yeni şifre betimleme
        {
            try
            {
                await con.OpenAsync();
                cmd = new SqlCommand("UPDATE employee SET password='"+employee.password+"' WHERE isDelete='No' and username='"+employee.username+"'",con);
                await cmd.ExecuteNonQueryAsync();
                return true;
            }catch(Exception )
            {
                return false;
            }
            finally
            {
                if(con.State == ConnectionState.Open)
                {
                    con.Close();
                    employee.password = employee.username = null;
                }
            }
        }
        private async Task<Boolean> addNewOrder()
        {
            try
            {
                //Şirket İsmini Al
                adapter = new SqlDataAdapter("SELECT companyName FROM company WHERE id='"+entryForm.login_id+"'",con);
                dt = new DataTable();
                adapter.Fill(dt);
                order.businessName = dt.Rows[0][0].ToString();
                //TC Kontrol
                adapter = new SqlDataAdapter("SELECT * FROM customers WHERE tc='"+order.deliveryTC+"'",con);
                dt = new DataTable();
                adapter.Fill(dt);
                if(dt.Rows.Count>0)
                {
                    //Çalışan Kontrol
                    adapter = new SqlDataAdapter("SELECT * FROM employee WHERE username='"+order.orderEmployeeName+"'",con);
                    dt = new DataTable();
                    adapter.Fill(dt);
                    if(dt.Rows.Count>0)
                    {
                        //Sipariş Numarası Kontrol
                        adapter = new SqlDataAdapter("SELECT * FROM orders WHERE orderId='"+order.orderId+"'",con);
                        dt = new DataTable();
                        adapter.Fill(dt);
                        if(dt.Rows.Count>0)
                        {
                            return false;
                        }
                        else
                        {
                            await con.OpenAsync();
                            cmd = new SqlCommand("INSERT INTO orders(orderId, businessName, employeeName, orderState, deliveryTC, isDelete) VALUES('" + order.orderId + "','" + order.businessName + "','" + order.orderEmployeeName + "','" + order.orderState + "','" + order.deliveryTC + "','No')", con);
                            await cmd.ExecuteNonQueryAsync();
                            return true;
                            //Mail Deneme
                            //string fromMail = "";
                            //string fromMailPass = "";
                            //string toMail = "";
                            //adapter = new SqlDataAdapter("SELECT companyMail,mailPassword FROM company WHERE id='"+entryForm.login_id+"'",con);
                            //dt = new DataTable();
                            //adapter.Fill(dt);
                            //fromMail = dt.Rows[0][0].ToString();
                            //fromMailPass = dt.Rows[0][1].ToString();
                            //adapter = new SqlDataAdapter("SELECT mailAddress FROM customers WHERE tc='"+order.deliveryTC+"'",con);
                            //dt = new DataTable();
                            //adapter.Fill(dt);
                            //toMail = dt.Rows[0][0].ToString();
                            //var message = new MailMessage(fromMail,toMail);
                            //message.Subject = order.businessName + " Sisteme Sipariş Ekledi!";
                            //message.Body = order.businessName + " Sisteme sizinle ilgili sipariş ekledi!\nSipariş Takip Kodunuz:" + order.orderId;
                            //using (SmtpClient client = new SmtpClient("smtp.gmail.com", 587))
                            //{
                            //    client.UseDefaultCredentials = true;
                            //    client.Credentials = new NetworkCredential(fromMail,fromMailPass);
                            //    client.EnableSsl = true;
                            //    client.Send(message);
                            //}
                        }
                    }
                    else
                    {
                        return false;
                    }
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
                order.orderId = order.businessName = order.orderEmployeeName = order.orderState = order.deliveryTC = null;
            }
        } // Sipariş Ekleme Mail Gönderme Çalışmadı Üstüne Bakılacak
        private async Task getCustomersAddress()
        {
            try
            {
                adapter = new SqlDataAdapter("SELECT nameSurname,address FROM customers WHERE tc='"+order.deliveryTC+"'",con);
                dt = new DataTable();
                adapter.Fill(dt);
                MessageBox.Show(dt.Rows[0][1].ToString(),dt.Rows[0][0].ToString(),MessageBoxButtons.OK);
                GC.SuppressFinalize(dt);
            }catch(Exception ex)
            {
                MessageBox.Show(ex.Message,"HATA",MessageBoxButtons.OK,MessageBoxIcon.Error);
            }
            finally
            {
                order.deliveryTC = "";
            }
        } // Siparişi Verilecek Kişinin Adresini Alma
        private async Task<Boolean> deleteOrder() // Sipariş Silme
        {
            try
            {
                await con.OpenAsync();
                cmd = new SqlCommand("UPDATE orders SET isDelete='Yes' WHERE orderId='"+order.orderId+"'",con);
                await cmd.ExecuteNonQueryAsync();
                order.orderId = null;
                return true;
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
        private async Task<Boolean> updateOrder()
        {
            try
            {
                await con.OpenAsync();
                cmd = new SqlCommand("UPDATE orders SET orderState='"+order.orderState+"' WHERE orderId='"+order.orderId+"'",con);
                await cmd.ExecuteNonQueryAsync();
                order.orderState = order.orderId = null;
                return true;
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
        private async Task<DataTable> getCustomer()
        {
            adapter = new SqlDataAdapter("SELECT nameSurname,username,address,phone FROM employee WHERE businessId='" + entryForm.login_id + "' and isDelete='No'", con);
            dt = new DataTable();
            adapter.Fill(dt);
            return dt;
        }
        private async Task<DataTable> getOrder()
        {
            adapter = new SqlDataAdapter("SELECT orderId,employeeName,orderState,deliveryTC FROM orders WHERE businessName=(SELECT companyName FROM company WHERE id='" + entryForm.login_id + "') and isDelete='No'", con);
            dt = new DataTable();
            adapter.Fill(dt);
            return dt;
        }
        private async Task<DataTable> getCustomers()
        {
            adapter = new SqlDataAdapter("SELECT DISTINCT nameSurname,username,tc,address,phone FROM dbo.customers WHERE tc=(SELECT DISTINCT deliveryTC FROM dbo.orders WHERE businessName=(SELECT DISTINCT companyName FROM company WHERE id='" + entryForm.login_id + "') and isDelete='No')", con);
            dt = new DataTable();
            adapter.Fill(dt);
            return dt;
        }
        private async Task<DataTable> getFeedBacks()
        {
            adapter = new SqlDataAdapter("SELECT orderId,comment FROM feedbacks WHERE companyId='" + entryForm.login_id + "'", con);
            dt = new DataTable();
            adapter.Fill(dt);
            return dt;
        }
        private async void getAllData()
        {
            try
            {

                dataGridView1.DataSource = await Task.Run(getCustomer);
                dataGridView1.Columns[0].HeaderText = "Ad Soyad";
                dataGridView1.Columns[1].HeaderText = "Kullanıcı Adı";
                dataGridView1.Columns[2].HeaderText = "Adresi";
                dataGridView1.Columns[3].HeaderText = "Telefon Numarası";
                dataGridView2.DataSource = await Task.Run(getOrder);
                dataGridView2.Columns[0].HeaderText = "Sipariş Numarası";
                dataGridView2.Columns[1].HeaderText = "Teslimatçı Adı";
                dataGridView2.Columns[2].HeaderText = "Sipariş Durum";
                dataGridView2.Columns[3].HeaderText = "Alıcı T.C No";
               dataGridView3.DataSource = await Task.Run(getCustomer);
                dataGridView4.DataSource = await Task.Run(getFeedBacks);
                dataGridView4.Columns[0].HeaderText = "Sipariş No";
                dataGridView4.Columns[1].HeaderText = "Geri Bildirim";
            }catch(Exception ex)
            {
                MessageBox.Show(ex.Message,"HATA",MessageBoxButtons.OK,MessageBoxIcon.Error);
            }
        } // Bütün veri çekme işlemleri
        private void adminPanel_Load(object sender, EventArgs e)
        {
            getAllData();
            employeeName.Focus();
            dataGridView1.MultiSelect = false;
            dataGridView2.MultiSelect = false;
            dataGridView3.MultiSelect = false;
            dataGridView4.MultiSelect = false;
        }
        private void simpleButton1_Click(object sender, EventArgs e) // Çalışan Temizle Paneli
        {
            employeeName.Text = employeeUsername.Text = employeePassword.Text = employeeAddress.Text = employeePhone.Text = employeeNewPass.Text = employeeNewPassRp.Text= "";
            employee.nameSurname = employee.username = employee.password = employee.address = employee.phoneNumber = "";
        }
        private async void barButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e) // Çalışan Ekleme
        {
            if(employeeName.Text != ""&&employeeUsername.Text!=""&&employeePassword.Text!=""&&employeeAddress.Text!=""&&employeePhone.MaskFull==true)
            {
                barButtonItem2.Enabled = false;
                employee.nameSurname = employeeName.Text;
                employee.username = employeeUsername.Text;
                employee.password = helper.crypto(employeePassword.Text);
                employee.address = employeeAddress.Text;
                employee.phoneNumber = employeePhone.Text;
                bool isOk = false;
                isOk = await Task.Run(addEmployee);
                if(isOk == true)
                {
                    MessageBox.Show("Çalışan Başarıyla Eklendi!");
                }
                else
                {
                    MessageBox.Show("Çalışan Eklenemedi!");
                }
                employeeName.Text = employeeUsername.Text = employeePassword.Text = employeeAddress.Text = employeePhone.Text = "";
                barButtonItem2.Enabled = true;
            }
            else
            {
                MessageBox.Show("Lütfen ilgili yerleri doldurunuz!","HATA",MessageBoxButtons.OK,MessageBoxIcon.Error);
            }
        }
        private void dataGridView1_RowHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e) // Çalışanlar Listesinden Çalışan Seçme
        {
            employeeName.Text = dataGridView1.CurrentRow.Cells[0].Value.ToString();
            employeeUsername.Text = dataGridView1.CurrentRow.Cells[1].Value.ToString();
            employeeAddress.Text = dataGridView1.CurrentRow.Cells[2].Value.ToString();
            employeePhone.Text = dataGridView1.CurrentRow.Cells[3].Value.ToString();
            employee.nameSurname = employeeName.Text;
            employee.username = employeeUsername.Text;
            employee.address = employeeAddress.Text;
            employee.phoneNumber = employeePhone.Text;
        }
        private async void barButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e) // Çalışan Silme Button
        {
            barButtonItem3.Enabled = false;
            DialogResult result = MessageBox.Show(employee.nameSurname+" Adlı çalışanı silmek istediğinize emin misinz?","EMİN MİSİNİZ?",MessageBoxButtons.YesNo,MessageBoxIcon.Question);
            if(result == DialogResult.Yes)
            {
                bool isOk = false;
                isOk = await Task.Run(deleteEmployee);
                if(isOk == true)
                {
                    MessageBox.Show("Çalışan Başarıyla Silindi!");
                }
                else
                {
                    MessageBox.Show("Çalışan Silinemedi");
                }
            }
            barButtonItem3.Enabled = true;

        }
        private async void barButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e) //Çalışan Güncelle Button
        {
            barButtonItem4.Enabled = false;
            if(employee.nameSurname!=null)
            {
                DialogResult result = MessageBox.Show(employee.nameSurname + " Adlı çalışanın bilgilerini güncellemek istiyor musunuz?", "EMİN MİSİNİZ?", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    if (employeeNewPass.Text != "" && employeeNewPassRp.Text != "" && employeeUsername.Text != "")
                    {
                        if (employeeNewPass.Text == employeeNewPassRp.Text)
                        {
                            employee.username = employeeUsername.Text;
                            employee.password = helper.crypto(employeeNewPass.Text);
                            bool isOk = false;
                            isOk = await Task.Run(setNewEmployeePass);
                            if (isOk == true)
                            {
                                MessageBox.Show("Çalışan Şifresi Güncellendi!");
                            }
                            else
                            {
                                MessageBox.Show("Çalışan Şifresi Güncellenemedi");
                            }
                        }
                        else
                        {
                            MessageBox.Show("Şifreler birbiriyle uyuşmuyor!", "HATA", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }else if(employeeNewPass.Text==""||employeeNewPassRp.Text=="")
                    {
                        employee.address = employeeAddress.Text;
                        employee.phoneNumber = employeePhone.Text;
                        employee.password = helper.crypto(employeePassword.Text);
                        bool isOk = false;
                        isOk = await Task.Run(updateEmployee);
                        if(isOk == true)
                        {
                            MessageBox.Show("Çalışan Güncellendi!");
                        }
                        else
                        {
                            MessageBox.Show("Çalışan Güncellenemedi");
                        }
                    }                  
                }             
            }
            else
            {
                MessageBox.Show("Lütfen listenizden güncellemek istediğiniz çalışana tıklayınız","HATA",MessageBoxButtons.OK,MessageBoxIcon.Error);
            }
            barButtonItem4.Enabled = true;
        }
        private void barButtonItem9_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e) //Çalışan Çıktısı Alma
        {
            if(printPreviewEmployee.ShowDialog() == DialogResult.OK)
            {
                printEmployee.Print();
            }
        }
        private void printDocument1_PrintPage(object sender, PrintPageEventArgs e) // Çıktı Alma Metodu PrintDocument
        {
            adapter = new SqlDataAdapter("SELECT nameSurname,username,address,phone FROM employee WHERE businessId='"+entryForm.login_id+"' and isDelete='No'",con);
            dt = new DataTable();
            adapter.Fill(dt);
            Font font = new Font("Verdana",12,FontStyle.Bold);
            SolidBrush solidBrush = new SolidBrush(Color.Black);
            SolidBrush solid = new SolidBrush(Color.Red);
            ArrayList employee = new ArrayList();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                string print = "Ad:" + dt.Rows[i][0].ToString() + "\nKullanıcı Adı:" + dt.Rows[i][1].ToString() + "\nAdresi:" + dt.Rows[i][2].ToString() + "\nTel:" + dt.Rows[i][3].ToString() + "\n";
                employee.Add(print);
            }
            int y = 10;
            for (int i = 0; i < employee.Count; i++)
            {

                e.Graphics.DrawString(employee[i].ToString() + "\n", font, solidBrush, 0, y);
                y += 100;
            }
            e.Graphics.DrawString("Çalışan Sayısı:" + dt.Rows.Count, font, solid, 645, 5);
            GC.SuppressFinalize(employee);
        }
        private async void barButtonItem5_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e) // Sipariş Kayıt Metodu
        {
            barButtonItem5.Enabled = false;
            if(xtraTabControl1.SelectedTabPageIndex == 1)
            {
                if(orderId.Text != ""&&orderEmployee.Text!=""&&orderState.Text!=""&&orderTC.Text!="")
                {
                    order.orderId = orderId.Text;
                    order.orderEmployeeName = orderEmployee.Text;
                    order.orderState = orderState.Text;
                    order.deliveryTC = orderTC.Text;
                    bool isOk = false;
                    isOk = await Task.Run(addNewOrder);
                    if(isOk == true)
                    {
                        MessageBox.Show("Sipariş Başarıyla Eklendi!");
                    }
                    else
                    {
                        MessageBox.Show("Sipariş Eklenemedi!");
                    }
                    orderId.Text = orderEmployee.Text = orderTC.Text = "";
                    orderState.SelectedIndex = -1;
                }
            }
            else
            {
                MessageBox.Show("Lütfen siparişler sayfasına gidip ilgili yerleri doldurunuz!","HATA",MessageBoxButtons.OK,MessageBoxIcon.Error);
            }
            barButtonItem5.Enabled = true;
        }
        private void simpleButton2_Click(object sender, EventArgs e) // Sipariş Temizleme Bölümü
        {

        }
        private void barButtonItem13_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e) // Tüm Verileri Yenile
        {
            getAllData();
        }
        private void dataGridView2_RowHeaderMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)// Çift Tıklamada Adres Bilgiler Getirilsin
        {
            order.deliveryTC = dataGridView2.Rows[e.RowIndex].Cells[3].Value.ToString();
            Task.Run(getCustomersAddress);
        }
        private void dataGridView2_RowHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e) // Siparişler Mouse Tıklandığında
        {
            orderId.Text = dataGridView2.Rows[e.RowIndex].Cells[0].Value.ToString();
            orderEmployee.Text = dataGridView2.Rows[e.RowIndex].Cells[1].Value.ToString();
            for(int i=0;i<orderState.Items.Count;i++)
            {
                if (orderState.Items[i].ToString() == dataGridView2.Rows[e.RowIndex].Cells[2].Value.ToString())
                {
                    orderState.Text = orderState.Items[i].ToString();            
                }
            }
            orderTC.Text = dataGridView2.Rows[e.RowIndex].Cells[3].Value.ToString();
            order.orderId = dataGridView2.Rows[e.RowIndex].Cells[0].Value.ToString();
        }
        private void simpleButton2_Click_1(object sender, EventArgs e) // Siparişler Bölümü TextBox Temizleme
        {
            orderId.Text = orderEmployee.Text = orderTC.Text = "";
            orderState.SelectedIndex = -1;
        }
        private async void barButtonItem6_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e) // Sipariş Silme
        {
            barButtonItem6.Enabled = false;
            if(xtraTabControl1.SelectedTabPageIndex == 1)
            {
                if (order.orderId != null)
                {
                    DialogResult result = MessageBox.Show(order.orderId+" Kodlu siparişi sistemden silmek istiyor musunuz?","Emin Misiniz?",MessageBoxButtons.YesNo,MessageBoxIcon.Question);
                    if(result == DialogResult.Yes)
                    {
                        bool isOk = false;
                        isOk = await Task.Run(deleteOrder);
                        if(isOk == true)
                        {
                            MessageBox.Show("Sipariş Başarıyla Silindi!");
                        }
                        else
                        {
                            MessageBox.Show("Sipariş Silinemedi!");
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Lütfen silinecek siparişi seçiniz!", "HATA", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Lütfen siparişler sayfasına giriniz!","HATA",MessageBoxButtons.OK,MessageBoxIcon.Error);
            }
            barButtonItem6.Enabled = true;

        }
        private async void barButtonItem8_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e) // Sipariş Güncelleme
        {
            barButtonItem8.Enabled = false;
            if(xtraTabControl1.SelectedTabPageIndex == 1)
            {
                if(order.orderId!=null&&order.orderState!=null)
                {
                    bool isOk = false;
                    isOk = await Task.Run(updateOrder);
                    if(isOk == true)
                    {
                        MessageBox.Show("Sipariş Durumu Başarıyla Güncellendi!");
                    }
                    else
                    {
                        MessageBox.Show("Sipariş Durumu Güncellenemedi!");
                    }
                }
            }
            else
            {
                MessageBox.Show("Lütfen siparişler sayfasına giriniz!", "HATA", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            barButtonItem8.Enabled = true;

        }
        private void orderState_SelectedIndexChanged(object sender, EventArgs e) // Sipariş ComboBox Değer Değişim
        {
            order.orderState = orderState.Text;
        }     
        private void barButtonItem10_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e) // Sipariş Çıktı Alma Butonu
        {
            if(printPreviewOrder.ShowDialog() == DialogResult.OK)
            {
                printOrder.Print();
            }
        }
        private void printOrder_PrintPage(object sender, PrintPageEventArgs e)
        {
            adapter = new SqlDataAdapter("SELECT orderId,employeeName,orderState,deliveryTC FROM orders WHERE businessName=(SELECT companyName FROM company WHERE id='"+entryForm.login_id+"') and isDelete='No'",con);
            dt = new DataTable();
            adapter.Fill(dt);
            Font font = new Font("Verdana", 12, FontStyle.Bold);
            SolidBrush solidBrush = new SolidBrush(Color.Black);
            SolidBrush solid = new SolidBrush(Color.Red);
            ArrayList order = new ArrayList();
            for(int i=0;i<dt.Rows.Count;i++)
            {
                string print = "Sipariş No:" + dt.Rows[i][0].ToString() + "\nTeslimatçı:" + dt.Rows[i][1].ToString() + "\nSipariş Durumu:" + dt.Rows[i][2].ToString() + "\nAlıcı T.C No:" + dt.Rows[i][3].ToString();
                order.Add(print);
            }
            int y = 20;
            for(int i=0;i<order.Count;i++)
            {
                e.Graphics.DrawString(order[i].ToString()+"\n",font,solidBrush,0,y);
                y += 100;
            }
            e.Graphics.DrawString("Tarih:" +DateTime.Now.ToShortDateString() , font, solid, 650, 0);
            e.Graphics.DrawString("Sipariş Sayısı:" + dt.Rows.Count, font, solid, 650, 20);
            GC.SuppressFinalize(order);
            GC.SuppressFinalize(dt);
        }

        private void barButtonItem15_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            DialogResult result = MessageBox.Show("Oturumu kapatmak istediğinize emin misiniz?","EMİN MİSİNİZ?",MessageBoxButtons.YesNo,MessageBoxIcon.Question);
            if(result == DialogResult.Yes)
            {
                Form x = new entryForm();
                GC.Collect();
                GC.WaitForPendingFinalizers();
                x.Show();
                this.Hide();
            }
        }

        private void adminPanel_FormClosing(object sender, FormClosingEventArgs e)
        {
            //Form x = new entryForm();
            //GC.Collect();
            //GC.WaitForPendingFinalizers();
            //x.Show();
            Application.Exit();
        }

        private void barToggleSwitchItem1_CheckedChanged(object sender, DevExpress.XtraBars.ItemClickEventArgs e) //Karanlık Tema Aktifleştirme
        {
            //            UserLookAndFeel.Default.SetSkinStyle("DevExpress Dark Style");
            if(UserLookAndFeel.Default.SkinName == "DevExpress Style")
            {
                UserLookAndFeel.Default.SetSkinStyle("DevExpress Dark Style");
            }
            else
            {
                UserLookAndFeel.Default.SetSkinStyle("DevExpress Style");
            }
        }

        private void xtraTabControl1_SelectedPageChanged(object sender, DevExpress.XtraTab.TabPageChangedEventArgs e)
        {
            if(xtraTabControl1.SelectedTabPageIndex == 1) // Form Load veya harici bir metodda çalışmadığı için buraya yazıldı!
            {
                for (int i = 0; i < dataGridView2.Rows.Count; i++)
                {
                    DataGridViewCellStyle style = new DataGridViewCellStyle();
                    if (dataGridView2.Rows[i].Cells[2].Value.ToString() == "Siparisiniz Teslim Edildi !")
                    {
                        style.BackColor = Color.Green;
                    }
                    else
                    {
                        style.BackColor = Color.Red;
                    }
                    dataGridView2.Rows[i].DefaultCellStyle = style;
                }
            }
        }
    }
}
