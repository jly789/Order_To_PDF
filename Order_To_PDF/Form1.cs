using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Rectangle = iTextSharp.text.Rectangle;






namespace WindowsFormsApp17
{
    public partial class Form1 : Form
    {
        private SqlConnection sqlConn = null;
        private string connstr = "SERVER=" + ConfigurationManager.AppSettings["IP"] + "," + ConfigurationManager.AppSettings["PORT"] + ";" +
            "DATABASE=" + ConfigurationManager.AppSettings["DBNAME"] + ";UID=" + ConfigurationManager.AppSettings["USERID"] + ";PASSWORD=" + ConfigurationManager.AppSettings["USERPASSWORD"];


        public Form1()
        {
            InitializeComponent();

        }


        /// <summary>
        /// 데이터베이스연결
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn1_Click(object sender, EventArgs e)
        {
            try
            {
                sqlConn = new SqlConnection(connstr);
                //데이터베이스
                sqlConn.Open();
                MessageBox.Show("데이터베이스연결성공");
                Console.WriteLine("[알림]데이터베이스연결성공");
            }
            catch (Exception ex)
            {
                //알림창으로 에러내용
                MessageBox.Show("에러발생내용" + ex.ToString());
                Console.WriteLine("[오류]오류내용" + ex.ToString());
            }
        }

        /// <summary>
        /// 데이터베이스연결해제
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn2_Click(object sender, EventArgs e)
        {
            try
            {
                //데이터베이스와 연결을 끊기
                //null이아닌 것은  sqlConn = new SqlConnection(connstr);이 실행됬음.
                if (sqlConn != null)
                {
                    sqlConn.Close();
                    MessageBox.Show("데이터베이스연결을 끊음");
                    Console.WriteLine("[알림]데이터베이스연결끊김");
                }
            }
            catch (Exception ex)
            {
                //알림창으로 에러내용
                MessageBox.Show("에러발생내용" + ex.ToString());
                Console.WriteLine("[오류]오류내용" + ex.ToString());
            }

        }

        // 데이터베이스에서 BOOKS 테이블 데이터를 조회하는 메서드
        public DataTable GetBooksData()
        {
            DataSet ds = new DataSet();

            using (SqlConnection conn = new SqlConnection(connstr))
            {
                conn.Open();
                string sql = "SELECT * FROM BOOKS";
                SqlDataAdapter adapter = new SqlDataAdapter(sql, conn);
                adapter.Fill(ds, "BOOKS");
            }

            return ds.Tables[0];
        }


        private void button4_Click(object sender, EventArgs e)
        {
            //데이터를 조회해와서 저장하는 곳
            DataSet ds = new DataSet();

            using (SqlConnection conn = new SqlConnection(connstr))
            {
                conn.Open();
                //연결이 안되있으면 자동적으로 연결을하고 연결을끊어서
                string sql = "SELECT * FROM BOOKS";
                SqlDataAdapter adapter = new SqlDataAdapter(sql, conn);
                adapter.Fill(ds, "BOOKS");


            }

            //conn.Clos();자동적으로 호출
            //데이터베이스와 연결을 자동적으로 끊어줍니다.

            dataGridView1.DataSource = ds.Tables[0];

            Create_PDF(dataGridView1);
        }


        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            //선택한행의 책번호
            textBox1.Text = dataGridView1.Rows[e.RowIndex].Cells["BOOKNO"].Value.ToString();
            textBox2.Text = dataGridView1.Rows[e.RowIndex].Cells["NAME"].Value.ToString();
            textBox4.Text = dataGridView1.Rows[e.RowIndex].Cells["BOOKNO"].Value.ToString();

        }

        private void button1_Click(object sender, EventArgs e)
        {
            string bookno = textBox1.Text;
            using (SqlConnection conn = new SqlConnection(connstr))
            {
                conn.Open();

                SqlCommand command = new SqlCommand();
                command.Connection = conn;
                command.CommandText = "DELETE FROM BOOKS WHERE BOOKNO = " + bookno;
                command.ExecuteNonQuery();

                button4_Click(null, null);
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {

            string bookno = textBox4.Text;
            string bookname = textBox3.Text;

            using (SqlConnection conn = new SqlConnection(connstr))
            {
                conn.Open();

                SqlCommand command = new SqlCommand();
                command.Connection = conn;
                command.CommandText = "UPDATE BOOKS SET NAME = '" + bookname + "' WHERE BOOKNO = " + bookno;
                command.ExecuteNonQuery();

                button4_Click(null, null);
            }

        }


        private void button3_Click(object sender, EventArgs e) //삽입쿼리
        {


            string bookno = textBox6.Text;
            string bookname = textBox5.Text;
            string bookcode = textBox7.Text;

            using (SqlConnection conn = new SqlConnection(connstr))
            {
                conn.Open();

                SqlCommand command = new SqlCommand("InsertBOOKS", conn); //insertBooks저장프로시저 호출
                command.CommandType = CommandType.StoredProcedure; //실행할 명령의 종류를 지정 ->저장프로시저 지정

                SqlParameter p1 = new SqlParameter("@BOOKNO", SqlDbType.VarChar); //sql쿼리나 저장프로시저에서
                                                                                  //사용하는 파라미터를 나타내는 클래스
                p1.Direction = ParameterDirection.Input;
                p1.Value = bookno;
                command.Parameters.Add(p1);

                SqlParameter p2 = new SqlParameter("@BOOKNAME", SqlDbType.VarChar);

                p2.Direction = ParameterDirection.Input; //sqlparameter는 입력,출력,입출력 매개변수로 사용,input은 데이터를 전달만하는 역할
                p2.Value = bookname;
                command.Parameters.Add(p2);

                SqlParameter p3 = new SqlParameter("@BOOKCODE", SqlDbType.VarChar);
                p3.Direction = ParameterDirection.Input;
                p3.Value = bookcode;
                command.Parameters.Add(p3);

                command.ExecuteNonQuery();

                button4_Click(null, null);


            }
        }


        private void 파일로드_Click(object sender, EventArgs e)
        {
            string pdfFile = Environment.CurrentDirectory + "/재일.pdf";
            axAcroPDF1.LoadFile(pdfFile);
        }


        private void 인쇄설정창_Click(object sender, EventArgs e)
        {
            axAcroPDF1.Print();
        }



        private void 바로출력_Click(object sender, EventArgs e)
        {
            axAcroPDF1.printAllFit(true);
        }


        //PDF 파일 생성

        public void Create_PDF(DataGridView dataGridView)
        {
            // 파일 생성
            FileStream fs = new FileStream("재일.pdf", FileMode.Create, FileAccess.Write, FileShare.None);

            // 문서 형식
            Rectangle size = new Rectangle(PageSize.A4);
            Document doc = new Document(size);

            // PDF 쓰기 설정
            PdfWriter wr = PdfWriter.GetInstance(doc, fs);

            // 헤더와 푸터를 추가하는 이벤트 핸들러 설정 (회사명과 작성자명 전달)
            HeaderFooterEventHelper headerFooter = new HeaderFooterEventHelper("BowooSystem", "YunJaeiL");
            //영어만 설정됨 한글아직안되서 수정해야함
            wr.PageEvent = headerFooter;

            // 문서 열기
            doc.Open();

            // 문서 속성
            doc.AddTitle("BOOKS 데이터 PDF");
            doc.AddAuthor("작성자");
            doc.AddCreationDate();



            // 한글 쓰기 설정 (폰트 경로 지정)
             string font = System.IO.Path.Combine("C:\\Users\\jly78\\AppData\\Local\\Microsoft\\Windows", @"Fonts\HANBatang.ttf");
            BaseFont batangBase = BaseFont.CreateFont(font, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
            var batang = new iTextSharp.text.Font(batangBase, 12F);

            // PDF에 표 추가 (열 개수는 DataGridView 열 수로 설정)
            PdfPTable table = new PdfPTable(dataGridView.ColumnCount);
            table.WidthPercentage = 100; // 테이블이 페이지 너비를 다 채우도록 설정

            // 열 제목 추가 (DataGridView 열 제목을 PDF 테이블에 추가)
            foreach (DataGridViewColumn column in dataGridView.Columns)
            {
                PdfPCell cell = new PdfPCell(new Phrase(column.HeaderText, batang));
                cell.BackgroundColor = BaseColor.LIGHT_GRAY; // 열 제목에 배경 색 추가
                table.AddCell(cell);
            }

            // 데이터 추가 (DataGridView의 모든 행을 PDF 테이블에 추가)
            foreach (DataGridViewRow row in dataGridView.Rows)
            {
                // 마지막 빈 행을 제외
                if (row.IsNewRow) continue;

                foreach (DataGridViewCell cell in row.Cells)
                {
                    table.AddCell(new Phrase(cell.Value?.ToString() ?? "", batang));
                }
            }

            // 문서에 테이블 추가
            doc.Add(table);

            // 문서 닫기
            doc.Close();
            fs.Close();
        }


    }


}
