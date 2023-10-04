using EmployeeAdjustmentConnectionSystem.COM.Models;
using EmployeeAdjustmentConnectionSystem.COM.Util.Convert;
using EmployeeAdjustmentConnectionSystem.COM.Util.Database;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using System.Web;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;

namespace EmployeeAdjustmentConnectionSystem.BL.SystemManagement {
    public class UploadBL {
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
        /// 目標管理決裁権限ファイルチェック
        /// </summary>
        /// <param name="model"></param>
        /// <param name="ReadData"></param>
        /// <returns></returns>
        public bool ObjectiveApproverFileCheck(SystemManagementModels model , ArrayList ReadData) {
            try {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");
                foreach (string[] headCheck in ReadData) {
                    if (headCheck.Length == 57){
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
        /// 職能判定決裁権限ファイルチェック
        /// </summary>
        /// <param name="model"></param>
        /// <param name="ReadData"></param>
        /// <returns></returns>
        public bool SkillApproverFileCheck(SystemManagementModels model , ArrayList ReadData) {
            try {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");
                foreach (string[] headCheck in ReadData) {
                    if (headCheck.Length == 32){
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
        /// 目標管理決裁権限データチェック
        /// </summary>
        /// <param name="model"></param>
        /// <param name="ReadData"></param>
        /// <returns></returns>
        public string ObjectiveApproverDataCheck(SystemManagementModels model , ArrayList ReadData) {
            try {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");

                string message = "";

                // ヘッダ行削除
                ReadData.RemoveAt(0);
                        
                if (model.UploadFile.FileName.EndsWith("xls") || model.UploadFile.FileName.EndsWith("xlsx")) {
                    // Excel形式
                    foreach (List<string> data in ReadData) {
                    }
                } else if (model.UploadFile.FileName.EndsWith("csv")) {
                    int cntNothing = 0;
                    int cntCommitted = 0;
                    int intCheck = 0;
                    int i = 1 ;
                    var varNothing = "";
                    var varCommitted = "";
                    foreach (string[] data in ReadData) {
                        i++;
                        //本人所属の数値チェック
                        if (!int.TryParse(data[2],out intCheck)) {
                            return i + "行目付近に不正なデータが存在します。<br>確認してください。";
                        }
                        ////本人存在チェック
                        //switch (ObjectiveConfirm(model.UploadYear,data[0],data[2]))
                        //{
                        //    case "Nothing": //存在しない
                        //        cntNothing ++;
                        //        if (cntNothing<6){
                        //            varNothing += " 社員番号:" + data[0] + " 所属番号:" + data[1] + "<br>";
                        //        }
                        //        break;
                        //    case "Committed": //確定済み
                        //        //問題なし
                        //        break;

                        //    case "Updated": //更新済み
                        //        //問題なし
                        //        break;

                        //    case "Exist":   //存在する
                        //        //問題なし
                        //        break;
                        //}

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
        /// 目標管理　存在確認
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
        /// 職能判定決裁権限データチェック
        /// </summary>
        /// <param name="model"></param>
        /// <param name="ReadData"></param>
        /// <returns></returns>
        public string SkillApproverDataCheck(SystemManagementModels model , ArrayList ReadData) {
            try {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");

                string message = "";

                // ヘッダ行削除
                ReadData.RemoveAt(0);
                        
                if (model.UploadFile.FileName.EndsWith("xls") || model.UploadFile.FileName.EndsWith("xlsx")) {
                    // Excel形式
                    foreach (List<string> data in ReadData) {
                    }
                } else if (model.UploadFile.FileName.EndsWith("csv")) {
                    int cntNothing = 0;
                    int cntCommitted = 0;
                    int intCheck = 0;
                    int i = 1 ;
                    var varNothing = "";
                    var varCommitted = "";
                    foreach (string[] data in ReadData) {
                        i++;
                        //本人所属の数値チェック
                        if (!int.TryParse(data[2],out intCheck)) {
                            return i + "行目付近に不正なデータが存在します。<br>確認してください。";
                        }
                        ////本人存在チェック
                        //switch (SkillConfirm(model.UploadYear,data[0],data[2]))
                        //{
                        //    case "Nothing": //存在しない
                        //        cntNothing ++;
                        //        if (cntNothing<6){
                        //            varNothing += " 社員番号:" + data[0] + " 所属番号:" + data[1] + "<br>";
                        //        }
                        //        break;
                        //    case "Committed": //確定済み
                        //        //問題なし
                        //        break;

                        //    case "Updated": //更新済み
                        //        //問題なし
                        //        break;

                        //    case "Exist":   //存在する
                        //        //問題なし
                        //        break;
                        //}

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
        /// 職能判定　存在確認
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public string SkillConfirm(string yaer,string employee,string branch)
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
                    + " SD_T職能判定基本 "
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
        /// 目標管理決裁権限アップデート
        /// </summary>
        /// <param name="model"></param>
        /// <param name="ReadData"></param>
        /// <returns></returns>
        public bool ObjectiveApproverUpdate(SystemManagementModels model , ArrayList ReadData) {
            try {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");

                string sql = Create_ObjectiveApproverUpdateSql();

                using (var scope = new TransactionScope()) {
                    using (DbManager dm = new DbManager())
                    using (IDbCommand cmd = dm.CreateCommand(sql))
                    using (DataSet ds = new DataSet()) {

                        // ヘッダ行削除
                        //2018-04-11 iwai-tamura upd-str ------
                        //チェック時に削除しているため不要
                        //ReadData.RemoveAt(0);
                        //2018-04-11 iwai-tamura upd-end ------
                        
                        string year = model.UploadYear; // 年度
                        DbHelper.AddDbParameter(cmd, "@year", DbType.String);
                        DbHelper.AddDbParameter(cmd, "@emp", DbType.String);
                        DbHelper.AddDbParameter(cmd, "@branch", DbType.String);
                        DbHelper.AddDbParameter(cmd, "@emp112", DbType.String);
                        DbHelper.AddDbParameter(cmd, "@emp113", DbType.String);
                        DbHelper.AddDbParameter(cmd, "@emp114", DbType.String);
                        DbHelper.AddDbParameter(cmd, "@emp212", DbType.String);
                        DbHelper.AddDbParameter(cmd, "@emp213", DbType.String);
                        DbHelper.AddDbParameter(cmd, "@emp214", DbType.String);
                        DbHelper.AddDbParameter(cmd, "@emp221", DbType.String);
                        DbHelper.AddDbParameter(cmd, "@emp223", DbType.String);
                        DbHelper.AddDbParameter(cmd, "@emp224", DbType.String);

                        if (model.UploadFile.FileName.EndsWith("xls") || model.UploadFile.FileName.EndsWith("xlsx")) {
                            // Excel形式
                            foreach (List<string> data in ReadData) {
                            }
                        } else if (model.UploadFile.FileName.EndsWith("csv")) {
                            foreach (string[] data in ReadData) {
                                var parameters = cmd.Parameters.Cast<IDbDataParameter>().ToArray<IDbDataParameter>();
                                parameters[0].Value = DataConv.IfNull(model.UploadYear);    // 年度
                                parameters[1].Value = DataConv.IfNull(data[0]);             // 社員番号
                                parameters[2].Value = DataConv.IfNull(data[2]);             // 所属番号
                                parameters[3].Value = DataConv.IfNull(data[12]);            // 目標面談者社員番号
                                parameters[4].Value = DataConv.IfNull(data[17]);            // 目標面談者所属番号
                                parameters[5].Value = DataConv.IfNull(data[22]);            // 目標面談者役職番号
                                parameters[6].Value = DataConv.IfNull(data[27]);            // 目標面談者役職番号
                                parameters[7].Value = DataConv.IfNull(data[32]);            // 目標面談者役職番号
                                parameters[8].Value = DataConv.IfNull(data[37]);            // 目標面談者役職番号
                                parameters[9].Value = DataConv.IfNull(data[42]);            // 目標面談者役職番号
                                parameters[10].Value = DataConv.IfNull(data[47]);            // 目標面談者役職番号
                                parameters[11].Value = DataConv.IfNull(data[52]);            // 目標面談者役職番号
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

        /// <summary>
        /// 職能判定決裁権限アップデート
        /// </summary>
        /// <param name="model"></param>
        /// <param name="ReadData"></param>
        /// <returns></returns>
        public bool SkillApproverUpdate(SystemManagementModels model, ArrayList ReadData) {
            try {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");

                string sql = Create_SkillApproverUpdateSql();

                using (var scope = new TransactionScope()) {
                    using (DbManager dm = new DbManager())
                    using (IDbCommand cmd = dm.CreateCommand(sql))
                    using (DataSet ds = new DataSet()) {

                        // ヘッダ行削除
                        //2018-04-11 iwai-tamura upd-str ------
                        //チェック時に削除しているため不要
                        //ReadData.RemoveAt(0);
                        //2018-04-11 iwai-tamura upd-end ------

                        string year = model.UploadYear; // 年度
                        DbHelper.AddDbParameter(cmd, "@year", DbType.String);
                        DbHelper.AddDbParameter(cmd, "@emp", DbType.String);
                        DbHelper.AddDbParameter(cmd, "@branch", DbType.String);
                        DbHelper.AddDbParameter(cmd, "@empPrimary", DbType.String);
                        DbHelper.AddDbParameter(cmd, "@empSecondary", DbType.String);
                        DbHelper.AddDbParameter(cmd, "@empDepartment", DbType.String);
                        DbHelper.AddDbParameter(cmd, "@empBranch", DbType.String);

                        if (model.UploadFile.FileName.EndsWith("xls") || model.UploadFile.FileName.EndsWith("xlsx")) {
                            // Excel形式
                            foreach (List<string> data in ReadData) {
                            }
                        } else if (model.UploadFile.FileName.EndsWith("csv")) {
                            foreach (string[] data in ReadData) {
                                if (data.Length != 32){
                                    continue;
                                }
                                
                                var parameters = cmd.Parameters.Cast<IDbDataParameter>().ToArray<IDbDataParameter>();
                                parameters[0].Value = DataConv.IfNull(model.UploadYear);    // 年度
                                parameters[1].Value = DataConv.IfNull(data[0]);             // 社員番号
                                parameters[2].Value = DataConv.IfNull(data[2]);             // 所属番号
                                parameters[3].Value = DataConv.IfNull(data[12]);            // 一次判定社員番号
                                parameters[4].Value = DataConv.IfNull(data[17]);            // 二次判定社員番号
                                parameters[5].Value = DataConv.IfNull(data[22]);            // 部門調整社員番号
                                parameters[6].Value = DataConv.IfNull(data[27]);            // 支社調整社員番号
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

        /// <summary>
        /// 目標管理決裁権限アップデートSQL文作成
        /// </summary>
        /// <returns></returns>
        public string Create_ObjectiveApproverUpdateSql() {
            try {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");


                string sql = "";
                sql = "UPDATE SD_M目標管理決裁権限"
                    + "   SET 上司社員 = @emp112"
                    + "      ,上司社員所属番号 = 目標面談者.所属番号"
                    + "      ,上司社員役職番号 = 目標面談者.役職番号"
                    + "      ,目標面談者 = @emp112"
                    + "      ,目標面談者所属番号 = 目標面談者.所属番号"
                    + "      ,目標面談者役職番号 = 目標面談者.役職番号"
                    + "      ,目標面談上位者 = @emp113"
                    + "      ,目標面談上位者所属番号 = 目標面談上位者.所属番号"
                    + "      ,目標面談上位者役職番号 = 目標面談上位者.役職番号"
                    + "      ,目標面談部長支店長 = @emp114"
                    + "      ,目標面談部長支店長所属番号 = 目標面談部長支店長.所属番号"
                    + "      ,目標面談部長支店長役職番号 = 目標面談部長支店長.役職番号"
                    + "      ,達成評価者 = @emp212"
                    + "      ,達成評価者所属番号 = 達成評価者.所属番号"
                    + "      ,達成評価者役職番号 = 達成評価者.役職番号"
                    + "      ,達成評価上位者 = @emp213"
                    + "      ,達成評価上位者所属番号 = 達成評価上位者.所属番号"
                    + "      ,達成評価上位者役職番号 = 達成評価上位者.役職番号"
                    + "      ,達成評価部長支店長 = @emp214"
                    + "      ,達成評価部長支店長所属番号 = 達成評価部長支店長.所属番号"
                    + "      ,達成評価部長支店長役職番号 = 達成評価部長支店長.役職番号"
                    + "      ,人事担当課長 = @emp221"
                    + "      ,人事担当課長所属番号 = 人事担当課長.所属番号"
                    + "      ,人事担当課長役職番号 = 人事担当課長.役職番号"
                    + "      ,総務部長 = @emp223"
                    + "      ,総務部長所属番号 = 総務部長.所属番号"
                    + "      ,総務部長役職番号 = 総務部長.役職番号"
                    + "      ,支社長担当役員 = @emp224"
                    + "      ,支社長担当役員所属番号 = 支社長担当役員.所属番号"
                    + "      ,支社長担当役員役職番号 = 支社長担当役員.役職番号"
                    + "      ,作成区分 = LEFT(作成区分,1)+'I'"
                    + "      ,作成日 = FORMAT(GETDATE(), 'yyyyMMdd')"
                    + " FROM SD_M目標管理決裁権限 MAIN "
                    + "     LEFT JOIN SD_M人事Data基本情報 as 目標面談者 on 目標面談者.社員番号 = @emp112"
                    + "     LEFT JOIN SD_M人事Data基本情報 as 目標面談上位者 on 目標面談上位者.社員番号 = @emp113"
                    + "     LEFT JOIN SD_M人事Data基本情報 as 目標面談部長支店長 on 目標面談部長支店長.社員番号 = @emp114"
                    + "     LEFT JOIN SD_M人事Data基本情報 as 達成評価者 on 達成評価者.社員番号 = @emp212"
                    + "     LEFT JOIN SD_M人事Data基本情報 as 達成評価上位者 on 達成評価上位者.社員番号 = @emp213"
                    + "     LEFT JOIN SD_M人事Data基本情報 as 達成評価部長支店長 on 達成評価部長支店長.社員番号 = @emp214"
                    + "     LEFT JOIN SD_M人事Data基本情報 as 人事担当課長 on 人事担当課長.社員番号 = @emp221"
                    + "     LEFT JOIN SD_M人事Data基本情報 as 総務部長 on 総務部長.社員番号 = @emp223"
                    + "     LEFT JOIN SD_M人事Data基本情報 as 支社長担当役員 on 支社長担当役員.社員番号 = @emp224"
                    + " WHERE MAIN.年度 = @year"
                    + "   AND MAIN.所属番号 = @branch"
                    + "   AND MAIN.社員番号 = @emp";

                return sql;
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
        /// 職能判定決裁権限アップデートSQL文作成
        /// </summary>
        /// <returns></returns>
        public string Create_SkillApproverUpdateSql() {
            try {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");


                string sql = "";
                sql = "UPDATE SD_M職能判定決裁権限"
                    + "   SET 上司社員 = @empPrimary"
                    + "      ,上司社員所属番号 = 一次判定.所属番号"
                    + "      ,上司社員役職番号 = 一次判定.役職番号"
                    + "      ,一次判定 = @empPrimary"
                    + "      ,一次判定所属番号 = 一次判定.所属番号"
                    + "      ,一次判定役職番号 = 一次判定.役職番号"
                    + "      ,二次判定 = @empSecondary"
                    + "      ,二次判定所属番号 = 二次判定.所属番号"
                    + "      ,二次判定役職番号 = 二次判定.役職番号"
                    + "      ,部門調整 = @empDepartment"
                    + "      ,部門調整所属番号 = 部門調整.所属番号"
                    + "      ,部門調整役職番号 = 部門調整.役職番号"
                    + "      ,支社調整 = @empBranch"
                    + "      ,支社調整所属番号 = 支社調整.所属番号"
                    + "      ,支社調整役職番号 = 支社調整.役職番号"
                    + "      ,作成区分 = LEFT(作成区分,1)+'I'"
                    + "      ,作成日 = FORMAT(GETDATE(), 'yyyyMMdd')"
                    + " FROM SD_M職能判定決裁権限 MAIN "
                    + "     LEFT JOIN SD_M人事Data基本情報 as 一次判定 on 一次判定.社員番号 = @empPrimary"
                    + "     LEFT JOIN SD_M人事Data基本情報 as 二次判定 on 二次判定.社員番号 = @empSecondary"
                    + "     LEFT JOIN SD_M人事Data基本情報 as 部門調整 on 部門調整.社員番号 = @empDepartment"
                    + "     LEFT JOIN SD_M人事Data基本情報 as 支社調整 on 支社調整.社員番号 = @empBranch"
                    + " WHERE MAIN.年度 = @year"
                    + "   AND MAIN.所属番号 = @branch"
                    + "   AND MAIN.社員番号 = @emp";

                return sql;
            } catch (Exception ex) {
                // エラー
                nlog.Error(System.Reflection.MethodBase.GetCurrentMethod().Name + " error " + ex.ToString());
                throw;
            } finally {
                //終了
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " end");
            }
        }




        //2018-99-99 iwai-tamura add-str ------
        /// <summary>
        /// 自己申告書決裁権限ファイルチェック
        /// </summary>
        /// <param name="model"></param>
        /// <param name="ReadData"></param>
        /// <returns></returns>
        public bool SelfDeclareApproverFileCheck(SystemManagementModels model , ArrayList ReadData) {
            try {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");
                foreach (string[] headCheck in ReadData) {
                    if (headCheck.Length == 22){
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
        /// 自己申告書決裁権限データチェック
        /// </summary>
        /// <param name="model"></param>
        /// <param name="ReadData"></param>
        /// <returns></returns>
        public string SelfDeclareApproverDataCheck(SystemManagementModels model , ArrayList ReadData) {
            try {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");

                string message = "";

                // ヘッダ行削除
                ReadData.RemoveAt(0);
                        
                if (model.UploadFile.FileName.EndsWith("xls") || model.UploadFile.FileName.EndsWith("xlsx")) {
                    // Excel形式
                    foreach (List<string> data in ReadData) {
                    }
                } else if (model.UploadFile.FileName.EndsWith("csv")) {
                    int cntNothing = 0;
                    int cntCommitted = 0;
                    int intCheck = 0;
                    int i = 1 ;
                    var varNothing = "";
                    var varCommitted = "";
                    foreach (string[] data in ReadData) {
                        i++;
                        //本人所属の数値チェック
                        if (!int.TryParse(data[2],out intCheck)) {
                            return i + "行目付近に不正なデータが存在します。<br>確認してください。";
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
        /// 自己申告書　存在確認
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public string SelfDeclareConfirm(string yaer,string employee,string branch)
        {
            try
            {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");

                string sql = "";                                //実行するクエリを保持

                //対象社員の存在を取得
                sql = "select "
                    + " 基本.* "
                    + " ,CASE WHEN (A01.確定フラグ = '1')or(B01.確定フラグ = '1')or(C01.確定フラグ = '1') THEN '1' "
                    + "       ELSE '0' "
                    + "  END 確定フラグ "
                    + "from "
                    + " SD_T自己申告書共通基本Data as 基本"
                    + " left join SD_T自己申告書明細DataA01 as A01 on (基本.年度 = A01.年度 and 基本.管理番号 = A01.管理番号) "
                    + " left join SD_T自己申告書明細DataB01 as B01 on (基本.年度 = B01.年度 and 基本.管理番号 = B01.管理番号) "
                    + " left join SD_T自己申告書明細DataC01 as C01 on (基本.年度 = C01.年度 and 基本.管理番号 = C01.管理番号) "
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
        /// 自己申告書決裁権限アップデート
        /// </summary>
        /// <param name="model"></param>
        /// <param name="ReadData"></param>
        /// <returns></returns>
        public bool SelfDeclareApproverUpdate(SystemManagementModels model , ArrayList ReadData) {
            try {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");

                string sql = Create_SelfDeclareApproverUpdateSql();

                using (var scope = new TransactionScope()) {
                    using (DbManager dm = new DbManager())
                    using (IDbCommand cmd = dm.CreateCommand(sql))
                    using (DataSet ds = new DataSet()) {

                        string year = model.UploadYear; // 年度
                        DbHelper.AddDbParameter(cmd, "@year", DbType.String);
                        DbHelper.AddDbParameter(cmd, "@emp", DbType.String);
                        DbHelper.AddDbParameter(cmd, "@branch", DbType.String);
                        DbHelper.AddDbParameter(cmd, "@emp12", DbType.String);
                        DbHelper.AddDbParameter(cmd, "@emp22", DbType.String);

                        if (model.UploadFile.FileName.EndsWith("xls") || model.UploadFile.FileName.EndsWith("xlsx")) {
                            // Excel形式
                            foreach (List<string> data in ReadData) {
                            }
                        } else if (model.UploadFile.FileName.EndsWith("csv")) {
                            foreach (string[] data in ReadData) {
                                var parameters = cmd.Parameters.Cast<IDbDataParameter>().ToArray<IDbDataParameter>();
                                parameters[0].Value = DataConv.IfNull(model.UploadYear);    // 年度
                                parameters[1].Value = DataConv.IfNull(data[0]);             // 社員番号
                                parameters[2].Value = DataConv.IfNull(data[2]);             // 所属番号
                                parameters[3].Value = DataConv.IfNull(data[12]);            // 上司社員番号
                                parameters[4].Value = DataConv.IfNull(data[17]);            // 人事担当部長所属番号
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

        /// <summary>
        /// 自己申告書決裁権限アップデートSQL文作成
        /// </summary>
        /// <returns></returns>
        public string Create_SelfDeclareApproverUpdateSql() {
            try {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");


                string sql = "";
                sql = "UPDATE SD_M自己申告書決裁権限"
                    + "   SET 上司社員 = @emp12"
                    + "      ,上司社員所属番号 = 上司社員.所属番号"
                    + "      ,上司社員役職番号 = 上司社員.役職番号"
                    + "      ,人事担当部長 = @emp22"
                    + "      ,人事担当部長所属番号 = 人事担当部長.所属番号"
                    + "      ,人事担当部長役職番号 = 人事担当部長.役職番号"
                    + "      ,作成区分 = LEFT(作成区分,1)+'I'"
                    + "      ,作成日 = FORMAT(GETDATE(), 'yyyyMMdd')"
                    + " FROM SD_M自己申告書決裁権限 MAIN "
                    + "     LEFT JOIN SD_M人事Data基本情報 as 上司社員 on 上司社員.社員番号 = @emp12"
                    + "     LEFT JOIN SD_M人事Data基本情報 as 人事担当部長 on 人事担当部長.社員番号 = @emp22"
                    + " WHERE MAIN.年度 = @year"
                    + "   AND MAIN.所属番号 = @branch"
                    + "   AND MAIN.社員番号 = @emp";

                return sql;
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
        /// キャリアシート決裁権限ファイルチェック
        /// </summary>
        /// <param name="model"></param>
        /// <param name="ReadData"></param>
        /// <returns></returns>
        public bool CareerSheetApproverFileCheck(SystemManagementModels model , ArrayList ReadData) {
            try {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");
                foreach (string[] headCheck in ReadData) {
                    if (headCheck.Length == 57){
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
        /// キャリアシート決裁権限データチェック
        /// </summary>
        /// <param name="model"></param>
        /// <param name="ReadData"></param>
        /// <returns></returns>
        public string CareerSheetApproverDataCheck(SystemManagementModels model , ArrayList ReadData) {
            try {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");

                string message = "";

                // ヘッダ行削除
                ReadData.RemoveAt(0);
                        
                if (model.UploadFile.FileName.EndsWith("xls") || model.UploadFile.FileName.EndsWith("xlsx")) {
                    // Excel形式
                    foreach (List<string> data in ReadData) {
                    }
                } else if (model.UploadFile.FileName.EndsWith("csv")) {
                    int cntNothing = 0;
                    int cntCommitted = 0;
                    int intCheck = 0;
                    int i = 1 ;
                    var varNothing = "";
                    var varCommitted = "";
                    foreach (string[] data in ReadData) {
                        i++;
                        //本人所属の数値チェック
                        if (!int.TryParse(data[2],out intCheck)) {
                            return i + "行目付近に不正なデータが存在します。<br>確認してください。";
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
        /// キャリアシート　存在確認
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public string CareerSheetConfirm(string yaer,string employee,string branch)
        {
            try
            {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");

                string sql = "";                                //実行するクエリを保持

                //対象社員の存在を取得
                sql = "select "
                    + " 基本.* "
                    + " ,Career.確定フラグ "
                    + "from "
                    + " SD_T自己申告書共通基本Data as 基本"
                    + " left join SD_TCareerSheet01 as Career on (基本.年度 = Career.年度 and 基本.管理番号 = Career.管理番号) "
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
        /// キャリアシート決裁権限アップデート
        /// </summary>
        /// <param name="model"></param>
        /// <param name="ReadData"></param>
        /// <returns></returns>
        public bool CareerSheetApproverUpdate(SystemManagementModels model , ArrayList ReadData) {
            try {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");

                string sql = Create_CareerSheetApproverUpdateSql();

                using (var scope = new TransactionScope()) {
                    using (DbManager dm = new DbManager())
                    using (IDbCommand cmd = dm.CreateCommand(sql))
                    using (DataSet ds = new DataSet()) {

                        string year = model.UploadYear; // 年度
                        DbHelper.AddDbParameter(cmd, "@year", DbType.String);
                        DbHelper.AddDbParameter(cmd, "@emp", DbType.String);
                        DbHelper.AddDbParameter(cmd, "@branch", DbType.String);
                        DbHelper.AddDbParameter(cmd, "@emp11", DbType.String);
                        DbHelper.AddDbParameter(cmd, "@emp12", DbType.String);
                        DbHelper.AddDbParameter(cmd, "@emp13", DbType.String);
                        DbHelper.AddDbParameter(cmd, "@emp21", DbType.String);
                        DbHelper.AddDbParameter(cmd, "@emp22", DbType.String);
                        DbHelper.AddDbParameter(cmd, "@emp23", DbType.String);
                        DbHelper.AddDbParameter(cmd, "@emp31", DbType.String);
                        DbHelper.AddDbParameter(cmd, "@emp32", DbType.String);
                        DbHelper.AddDbParameter(cmd, "@emp33", DbType.String);

                        if (model.UploadFile.FileName.EndsWith("xls") || model.UploadFile.FileName.EndsWith("xlsx")) {
                            // Excel形式
                            foreach (List<string> data in ReadData) {
                            }
                        } else if (model.UploadFile.FileName.EndsWith("csv")) {
                            foreach (string[] data in ReadData) {
                                var parameters = cmd.Parameters.Cast<IDbDataParameter>().ToArray<IDbDataParameter>();
                                parameters[0].Value = DataConv.IfNull(model.UploadYear);    // 年度
                                parameters[1].Value = DataConv.IfNull(data[0]);             // 社員番号
                                parameters[2].Value = DataConv.IfNull(data[2]);             // 所属番号
                                parameters[3].Value = DataConv.IfNull(data[12]);            // 所属部署副長
                                parameters[4].Value = DataConv.IfNull(data[17]);            // 所属部署課長
                                parameters[5].Value = DataConv.IfNull(data[22]);            // 所属部署部長
                                parameters[6].Value = DataConv.IfNull(data[27]);            // 支社総務部副長
                                parameters[7].Value = DataConv.IfNull(data[32]);            // 支社総務部課長
                                parameters[8].Value = DataConv.IfNull(data[37]);            // 支社総務部部長
                                parameters[9].Value = DataConv.IfNull(data[42]);            // 本社人事部副長
                                parameters[10].Value = DataConv.IfNull(data[47]);            // 本社人事部課長
                                parameters[11].Value = DataConv.IfNull(data[52]);            // 本社人事部部長
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

        /// <summary>
        /// キャリアシート決裁権限アップデートSQL文作成
        /// </summary>
        /// <returns></returns>
        public string Create_CareerSheetApproverUpdateSql() {
            try {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");


                string sql = "";
                sql = "UPDATE SD_MCareerSheet決裁権限"
                    + "   SET 所属部署副長 = @emp11"
                    + "      ,所属部署副長所属番号 = 所属部署副長.所属番号"
                    + "      ,所属部署副長役職番号 = 所属部署副長.役職番号"
                    + "      ,所属部署課長 = @emp12"
                    + "      ,所属部署課長所属番号 = 所属部署課長.所属番号"
                    + "      ,所属部署課長役職番号 = 所属部署課長.役職番号"
                    + "      ,所属部署部長 = @emp13"
                    + "      ,所属部署部長所属番号 = 所属部署部長.所属番号"
                    + "      ,所属部署部長役職番号 = 所属部署部長.役職番号"
                    + "      ,支社総務部副長 = @emp21"
                    + "      ,支社総務部副長所属番号 = 支社総務部副長.所属番号"
                    + "      ,支社総務部副長役職番号 = 支社総務部副長.役職番号"
                    + "      ,支社総務部課長 = @emp22"
                    + "      ,支社総務部課長所属番号 = 支社総務部課長.所属番号"
                    + "      ,支社総務部課長役職番号 = 支社総務部課長.役職番号"
                    + "      ,支社総務部部長 = @emp23"
                    + "      ,支社総務部部長所属番号 = 支社総務部部長.所属番号"
                    + "      ,支社総務部部長役職番号 = 支社総務部部長.役職番号"
                    + "      ,本社人事部副長 = @emp31"
                    + "      ,本社人事部副長所属番号 = 本社人事部副長.所属番号"
                    + "      ,本社人事部副長役職番号 = 本社人事部副長.役職番号"
                    + "      ,本社人事部課長 = @emp32"
                    + "      ,本社人事部課長所属番号 = 本社人事部課長.所属番号"
                    + "      ,本社人事部課長役職番号 = 本社人事部課長.役職番号"
                    + "      ,本社人事部部長 = @emp33"
                    + "      ,本社人事部部長所属番号 = 本社人事部部長.所属番号"
                    + "      ,本社人事部部長役職番号 = 本社人事部部長.役職番号"
                    + "      ,作成区分 = LEFT(作成区分,1)+'I'"
                    + "      ,作成日 = FORMAT(GETDATE(), 'yyyyMMdd')"
                    + " FROM SD_MCareerSheet決裁権限 MAIN "
                    + "     LEFT JOIN SD_M人事Data基本情報 as 所属部署副長 on 所属部署副長.社員番号 = @emp11"
                    + "     LEFT JOIN SD_M人事Data基本情報 as 所属部署課長 on 所属部署課長.社員番号 = @emp12"
                    + "     LEFT JOIN SD_M人事Data基本情報 as 所属部署部長 on 所属部署部長.社員番号 = @emp13"
                    + "     LEFT JOIN SD_M人事Data基本情報 as 支社総務部副長 on 支社総務部副長.社員番号 = @emp21"
                    + "     LEFT JOIN SD_M人事Data基本情報 as 支社総務部課長 on 支社総務部課長.社員番号 = @emp22"
                    + "     LEFT JOIN SD_M人事Data基本情報 as 支社総務部部長 on 支社総務部部長.社員番号 = @emp23"
                    + "     LEFT JOIN SD_M人事Data基本情報 as 本社人事部副長 on 本社人事部副長.社員番号 = @emp31"
                    + "     LEFT JOIN SD_M人事Data基本情報 as 本社人事部課長 on 本社人事部課長.社員番号 = @emp32"
                    + "     LEFT JOIN SD_M人事Data基本情報 as 本社人事部部長 on 本社人事部部長.社員番号 = @emp33"
                    + " WHERE MAIN.年度 = @year"
                    + "   AND MAIN.所属番号 = @branch"
                    + "   AND MAIN.社員番号 = @emp";

                return sql;
            } catch (Exception ex) {
                // エラー
                nlog.Error(System.Reflection.MethodBase.GetCurrentMethod().Name + " error " + ex.ToString());
                throw;
            } finally {
                //終了
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " end");
            }
        }

        //2018-99-99 iwai-tamura add-end ------


    }
}
