using EmployeeAdjustmentConnectionSystem.COM.Models;
using EmployeeAdjustmentConnectionSystem.COM.Util.Convert;
using EmployeeAdjustmentConnectionSystem.COM.Util.Database;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using System.Web;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;

namespace EmployeeAdjustmentConnectionSystem.BL.SystemManagement {
    public class TakeOverAmendmentCompanyBL {
        #region メンバ変数
        /// <summary>
        /// ロガー
        /// </summary>
        private static NLog.Logger nlog = NLog.LogManager.GetCurrentClassLogger();

        #endregion



        /// <summary>
        /// エクセルデータ取得
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public ArrayList ExcelOpen(HttpPostedFileWrapper file) {
            try {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");

                ArrayList ret = new ArrayList();

                using (var excel = new ExcelPackage(file.InputStream)) {
                    // シート名を指定
                    var sheet = excel.Workbook.Worksheets["Sheet1"];

                    // アドレスを指定してデータ取得
                    // [1 , 1] = セルA1
                    int x_rows = 1;
                    int y_columns = 1;
                    int y_columns_max = 0;

                    //先頭行を空のセルが見つかるまでループ
                    IList<string> HeadLine = new List<string>();
                    for (y_columns = 1; sheet.Cells[x_rows, y_columns].Text != ""; y_columns++) {
                        HeadLine.Add(sheet.Cells[x_rows, y_columns].Text);
                    }
                    ret.Add(HeadLine);
                    y_columns_max = HeadLine.Count;


                    for (x_rows = 2; sheet.Cells[x_rows, 1].Text != ""; x_rows++) {
                        IList<string> ReadLine = new List<string>();
                        // 先頭行の列数分ループ
                        for (y_columns = 1; y_columns <= y_columns_max; y_columns++) {
                            ReadLine.Add(sheet.Cells[x_rows, y_columns].Text);
                        }
                        ret.Add(ReadLine);
                    }
                }
                return ret;
            } catch (Exception ex) {
                // エラー
                nlog.Error(System.Reflection.MethodBase.GetCurrentMethod().Name + " error " + ex.ToString());
                throw;
            } finally {
                //終了
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " end");
            }
        }

        /// <summary>
        /// CSVデータ取得
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public ArrayList CSVOpen(HttpPostedFileWrapper file) {
            try {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");

                ArrayList ret = new ArrayList();

                // csv等テキストファイル用データ取得
                // StreamReader の新しいインスタンスを生成する
                System.IO.StreamReader cReader = (
                    new System.IO.StreamReader(file.InputStream, System.Text.Encoding.Default)
                );
                // 読み込みできる文字がなくなるまで繰り返す
                while (cReader.Peek() >= 0) {
                    // ファイルを1行ずつ読み込む
                    string stBuffer = cReader.ReadLine();
                    // 読み込んだもの","で分割し追加
                    ret.Add(stBuffer.Split(','));
                }
                // cReader を閉じる (正しくは オブジェクトの破棄を保証する を参照)
                cReader.Close();

                return ret;
            } catch (Exception ex) {
                // エラー
                nlog.Error(System.Reflection.MethodBase.GetCurrentMethod().Name + " error " + ex.ToString());
                throw;
            } finally {
                //終了
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " end");
            }
        }

        /// <summary>
        /// 目標管理 異動による引継ぎ処理ファイルチェック
        /// </summary>
        /// <param name="model"></param>
        /// <param name="ReadData"></param>
        /// <returns></returns>
        public bool ObjectiveTakeOverAmendmentCompanyFileCheck(SystemManagementModels model , ArrayList ReadData) {
            try {
                //開始
                //比較用項目名
                string[] aryFileItemName = new string[] { "社員番号", "引継元所属番号", "引継先所属番号"};
                
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");
                foreach (string[] headCheck in ReadData) {
                    if (headCheck.Length == 3){
                        for (int i = 0; i < headCheck.Length; i++)
                        {
                            if (!headCheck[i].Equals(aryFileItemName[i]))
                            {
                                return false;
                            }
                        }
                        break;
                    }else{
                        return false;
                    }
                }
                return true;
            } catch (Exception ex) {
                // エラー
                nlog.Error(System.Reflection.MethodBase.GetCurrentMethod().Name + " error " + ex.ToString());
                throw;
            } finally {
                //終了
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " end");
            }
        }        
        
        
        /// <summary>
        /// 目標管理 組編による引継ぎ一括処理　データチェック
        /// </summary>
        /// <param name="model"></param>
        /// <param name="ReadData"></param>
        /// <returns></returns>
        public string ObjectiveTakeOverAmendmentCompanyDataCheck(SystemManagementModels model , ArrayList ReadData) {
            try {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");

                string message = "";

                // ヘッダ行削除
                ReadData.RemoveAt(0);
                        
                if (model.TakeOverAmendmentCompanyBulkUploadFile.FileName.EndsWith("xls") || model.TakeOverAmendmentCompanyBulkUploadFile.FileName.EndsWith("xlsx")) {
                    // Excel形式
                    foreach (List<string> data in ReadData) {
                    }
                } else if (model.TakeOverAmendmentCompanyBulkUploadFile.FileName.EndsWith("csv")) {
                    int cntNothing = 0;
                    int cntCommitted = 0;
                    var varNothing = "";
                    var varCommitted = "";
                    foreach (string[] data in ReadData) {
                        //前任者存在チェック
                        switch (ObjectiveConfirm(model.TakeOverAmendmentCompanyBulkYaer,data[0],data[1]))
                        {
                            case "Nothing": //存在しない
                                cntNothing ++;
                                if (cntNothing<6){
                                    varNothing += " 社員番号:" + data[0] + " 所属番号:" + data[1] + "<br>";
                                }
                                break;
                            case "Committed": //確定済み
                                //問題なし
                                break;

                            case "Updated": //更新済み
                                //問題なし
                                break;

                            case "Exist":   //存在する
                                //問題なし
                                break;
                        }

                        //後任者存在チェック
                        switch (ObjectiveConfirm(model.TakeOverAmendmentCompanyBulkYaer,data[0],data[2]))
                        {
                            case "Nothing": //存在しない
                                cntNothing ++;
                                if (cntNothing<6){
                                    varNothing += " 社員番号:" + data[0] + " 所属番号:" + data[2] + "<br>";
                                }
                                break;
                            case "Committed": //確定済み
                                cntCommitted ++;
                                if (cntCommitted<6){
                                    varCommitted += " 社員番号:" + data[0] + " 所属番号:" + data[2] + "<br>";
                                }
                                break;

                            case "Updated": //更新済み
                                //問題なし
                                break;

                            case "Exist":   //存在する
                                //問題なし
                                break;
                        }
                    }

                    if (cntNothing+cntCommitted > 0){
                        message = "データが不正です。確認してください。" + "<br>";
                            
                        if (cntNothing > 0){
                            message += " [データ無]" + "<br>" + varNothing ;

                            //5件まで表示する
                            if (cntNothing > 5){
                                message += "                            etc." + "<br>";
                            }

                        }

                        if (cntCommitted > 0){
                            message += " [確定済データ]" + "<br>" + varCommitted ;

                            //5件まで表示する
                            if (cntCommitted > 5){
                                message += "                            etc." + "<br>";
                            }
                        }
                    }
                }
                return message;
            } catch (Exception ex) {
                // エラー
                nlog.Error(System.Reflection.MethodBase.GetCurrentMethod().Name + " error " + ex.ToString());
                throw;
            } finally {
                //終了
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " end");
            }
        }

        /// <summary>
        /// 存在確認
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public string ObjectiveConfirm(string yaer,string employee,string branch)
        {
            try
            {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");

                string sql = "";                                //実行するクエリを保持

                //対象社員の存在を取得
                sql = "select "
                    + " * "
                    + "from "
                    + " SD_T目標管理基本 "
                    + "where "
                    + "  年度 = " + yaer
                    + "  and 社員番号 = @employee "
                    + "  and 所属番号 = @branch ";

                DataSet dataSet = new DataSet();
                using (DbManager dm = new DbManager())
                using (IDbCommand cmd = dm.CreateCommand(sql))
                {
                    //パラメータ設定
                    DbHelper.AddDbParameter(cmd, "@employee", DbType.String);
                    DbHelper.AddDbParameter(cmd, "@branch", DbType.String);
                    var parameters = cmd.Parameters.Cast<IDbDataParameter>().ToArray<IDbDataParameter>();
                    parameters[0].Value = employee;
                    parameters[1].Value = branch;

                    //クエリ実行
                    IDataAdapter da = dm.CreateSqlDataAdapter(cmd);
                    da.Fill(dataSet);

                    //存在チェック
                    if (dataSet.Tables[0].Rows.Count == 0) {
                        return "Nothing"; //データ無
                    }
                    //確定チェック
                    if (dataSet.Tables[0].Rows[0]["確定フラグ"].ToString()=="1") {
                        return "Committed";
                    }
                    //更新チェック
                    if (dataSet.Tables[0].Rows[0]["登録年月日"].ToString()!=dataSet.Tables[0].Rows[0]["更新年月日"].ToString()) {
                        return "Updated";
                    }

                    //存在する
                    return "Exist";
                }
            }
            catch (Exception ex)
            {
                // エラー
                nlog.Error(System.Reflection.MethodBase.GetCurrentMethod().Name + " error " + ex.ToString());
                throw;
            }
            finally
            {
                //終了
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " end");
            }
        }        
        
        /// <summary>
        /// 目標管理DATA引継処理
        /// </summary>
        /// <param name="model"></param>
        /// <param name="ReadData"></param>
        /// <returns></returns>
        public bool ObjectiveTakeOverAmendmentCompany(SystemManagementModels model) {
            try {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");
                string sql = "SD_SP目標管理DATA組編引継処理";

                using (var scope = new TransactionScope()) {
                    using (DbManager dm = new DbManager())
                    using (IDbCommand cmd = dm.CreateCommand(sql))
                    using (DataSet ds = new DataSet()) {

                        string year = model.TakeOverAmendmentCompanyYaer; // 年度
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("YearParam", SqlDbType.Int));
                        cmd.Parameters.Add(new SqlParameter("EmployeeParam", SqlDbType.NVarChar));
                        cmd.Parameters.Add(new SqlParameter("befBranchParam", SqlDbType.NVarChar));
                        cmd.Parameters.Add(new SqlParameter("aftBranchParam", SqlDbType.NVarChar));
                        var parameters = cmd.Parameters.Cast<IDbDataParameter>().ToArray<IDbDataParameter>();
                        parameters[0].Value = int.Parse(year);                  // 年度
                        parameters[1].Value = DataConv.IfNull(model.TakeOverAmendmentCompanyEmployeeNo);            // 社員番号
                        parameters[2].Value = DataConv.IfNull(model.TakeOverAmendmentCompanyBefDepartment);         // 引継ぎ元所属番号
                        parameters[3].Value = DataConv.IfNull(model.TakeOverAmendmentCompanyAftDepartment);         // 引継ぎ先所属番号
                        cmd.ExecuteNonQuery();
                    }
                    scope.Complete();
                    return true;
                }
            } catch (Exception ex) {
                // エラー
                nlog.Error(System.Reflection.MethodBase.GetCurrentMethod().Name + " error " + ex.ToString());
                throw;
            } finally {
                //終了
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " end");
            }
        }

        /// <summary>
        /// 目標管理DATA引継一括処理
        /// </summary>
        /// <param name="model"></param>
        /// <param name="ReadData"></param>
        /// <returns></returns>
        public bool ObjectiveTakeOverAmendmentCompanyBulk(SystemManagementModels model , ArrayList ReadData) {
            try {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");
                string sql = "SD_SP目標管理DATA組編引継処理";

                using (var scope = new TransactionScope()) {
                    using (DbManager dm = new DbManager())
                    using (IDbCommand cmd = dm.CreateCommand(sql))
                    using (DataSet ds = new DataSet()) {

                        string year = model.TakeOverAmendmentCompanyBulkYaer; // 年度
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("YearParam", SqlDbType.Int));
                        cmd.Parameters.Add(new SqlParameter("EmployeeParam", SqlDbType.NVarChar));
                        cmd.Parameters.Add(new SqlParameter("befBranchParam", SqlDbType.NVarChar));
                        cmd.Parameters.Add(new SqlParameter("aftBranchParam", SqlDbType.NVarChar));

                        if (model.TakeOverAmendmentCompanyBulkUploadFile.FileName.EndsWith("xls") || model.TakeOverAmendmentCompanyBulkUploadFile.FileName.EndsWith("xlsx")) {
                            // Excel形式
                            foreach (List<string> data in ReadData) {
                            }
                        } else if (model.TakeOverAmendmentCompanyBulkUploadFile.FileName.EndsWith("csv")) {
                            foreach (string[] data in ReadData) {
                                var parameters = cmd.Parameters.Cast<IDbDataParameter>().ToArray<IDbDataParameter>();
                                parameters[0].Value = int.Parse(year);                  // 年度
                                parameters[1].Value = DataConv.IfNull(data[0]);            // 社員番号
                                parameters[2].Value = DataConv.IfNull(data[1]);         // 引継ぎ元所属番号
                                parameters[3].Value = DataConv.IfNull(data[2]);         // 引継ぎ先所属番号
                                cmd.ExecuteNonQuery();
                            }
                        }
                    }
                    scope.Complete();
                    return true;
                }
            } catch (Exception ex) {
                // エラー
                nlog.Error(System.Reflection.MethodBase.GetCurrentMethod().Name + " error " + ex.ToString());
                throw;
            } finally {
                //終了
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " end");
            }
        }
    
    }
}
