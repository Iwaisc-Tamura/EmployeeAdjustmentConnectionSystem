using OfficeOpenXml;
using OfficeOpenXml.Style;
using EmployeeAdjustmentConnectionSystem.COM.Models;
using EmployeeAdjustmentConnectionSystem.COM.Util.Config;
using EmployeeAdjustmentConnectionSystem.COM.Util.Convert;
using EmployeeAdjustmentConnectionSystem.COM.Util.Database;
using EmployeeAdjustmentConnectionSystem.COM.Util.Zip;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace EmployeeAdjustmentConnectionSystem.BL.SystemManagement
{
    public class ExcelOutputBL
    {
        readonly string TempDir;            //帳票作成ディレクトリ
        readonly int intRP;
        /// <summary>
        /// ロガー
        /// </summary>
        private static NLog.Logger nlog = NLog.LogManager.GetCurrentClassLogger();
        
        #region コンストラクタ
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ExcelOutputBL(string fullPath) {
            //帳票作成ディレクトリを取得
            Configuration config = WebConfig.GetConfigFile();
            TempDir = fullPath;

            //帳票作成ディレクトリ内保存日数を取得(int変換に失敗した場合は3とする。)
            if(!int.TryParse(config.AppSettings.Settings["RETENTIO_PERIOD"].Value, out intRP)) { intRP = 3; };
        }
        #endregion

        /// <summary>
        /// Excel出力
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public string ExcelOutput(SystemManagementModels model)
        {
            
            try {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");
                
                //現在日時を取得
                DateTime NowDate = DateTime.Now;
                string strUserCode = ((EmployeeAdjustmentConnectionSystem.COM.Entity.Session.LoginUser)
                    (HttpContext.Current.Session["LoginUser"])).UserCode.ToString();
                string strDepartmentNo = ((EmployeeAdjustmentConnectionSystem.COM.Entity.Session.LoginUser)
                    (HttpContext.Current.Session["LoginUser"])).DepartmentNo.ToString();

                //帳票作成フォルダを用意
                string strWorkFolder = "";
                strWorkFolder = TempDir + NowDate.ToString("yyyyMMdd") + Path.DirectorySeparatorChar +
                    strUserCode + NowDate.ToString("yyyyMMddHHmmss") + Path.DirectorySeparatorChar;
                System.IO.Directory.CreateDirectory(strWorkFolder);

                //作成フォルダ内ファイル一覧を取得
                foreach (string file in System.IO.Directory.GetDirectories(TempDir, "*"))
                {
                    DateTime oldDirDate = new DateTime();
                    oldDirDate = System.IO.Directory.GetCreationTime(file); //作成日時を取得

                    //削除基準日より古いフォルダを削除
                    if (oldDirDate < NowDate.AddDays(-(intRP)))
                    {
                        DeleteDirectory(file);
                    }
                }

                //zip作成フォルダとzipファイル名を用意を用意
                string strZipFolder = TempDir + NowDate.ToString("yyyyMMdd") + Path.DirectorySeparatorChar;
                string strZipName = strUserCode + NowDate.ToString("yyyyMMddHHmmss") + ".zip";
                //return用path文字列を用意
                string zipReturnPath = NowDate.ToString("yyyyMMdd") + Path.DirectorySeparatorChar + strZipName;
                

                //excelファイルの作成
                //ファイル名作成
                //[対象年度]_[対象タイトル]_[対象支社]_[出力日時].xlsx
                string strTitleName = "";
                switch (model.OutputTarget) { //出力対象
                    case "Objective" : // 目標管理
                        strTitleName = "目標管理決裁権限一覧";
                        break;
                    case "Skill" : // 職能判定
                        strTitleName = "職能判定決裁権限一覧";
                        break;
                    // 2018-99-99 iwai-tamura upd str ------
                    case "Self" : // 自己申告書
                        strTitleName = "自己申告書決裁権限一覧";
                        break;
                    case "Career" : // キャリアシート
                        strTitleName = "キャリアシート決裁権限一覧";
                        break;
                    // 2018-99-99 iwai-tamura upd end ------
                }

                string strBranchName = "";
                switch (model.OutputBranch){
                    case "*":
                        strBranchName = "全社対象";
                        break;
                    case "1":
                        strBranchName = "本社";
                        break;
                    case "2":
                        strBranchName = "東京支社";
                        break;
                    case "3":
                        strBranchName = "関東支社";
                        break;
                    case "7":
                        strBranchName = "大阪支社";
                        break;
                    case "8":
                        strBranchName = "名古屋支社";
                        break;
                    case "9":
                        strBranchName = "福岡支社";
                        break;
                    default:
                        strBranchName = "";
                        break;
                }

                string fileName = model.OutputYear + "年度_" + strTitleName + "_" + strBranchName + "_" + NowDate.ToString("yyyyMMddHHmmss") + ".xlsx";

                //データ取得
                DataTable dt = new DataTable();
                dt = GetOutputData(model);

                // EPPlus使用版
                var outputFile = new FileInfo(strWorkFolder + fileName);
                if (outputFile.Exists) {
                    outputFile.Delete();
                }
                using (var excel = new ExcelPackage(outputFile))
                {
                    // シート追加
                    var sheet = excel.Workbook.Worksheets.Add("Sheet1");

                    int x = 1;
                    int y = 1;
                    foreach (DataColumn col in dt.Columns) {
                        // セル取得
                        // 1,1 = A1セル
                        var cell = sheet.Cells[x, y];
                        // セルに値設定
                        cell.Value = col.ColumnName;
                        // そのままだとフォントが英語圏のフォントなので調整
                        cell.Style.Font.SetFromFont(new Font("MS Gothic", 10, FontStyle.Regular));
                        // 次のセルへ
                        y++;
                    }

                    //次の行頭へ
                    x++;
                    y = 1;

                    foreach (DataRow dr in dt.Rows) {
                        foreach (object trg in dr.ItemArray) {
                            var cell = sheet.Cells[x, y];
                            cell.Value = trg.ToString();
                            cell.Style.Font.SetFromFont(new Font("MS Gothic", 10, FontStyle.Regular));
                            y++;
                        }
                        x++;
                        y = 1;
                    }

                    // 保存
                    excel.Save();
                }

                //作成したファイルに読み取り専用プロパティを設定
                //ネット経由でダウンロードされたファイルを保護されたビューで開くために必要
                FileAttributes fas = File.GetAttributes(strWorkFolder + fileName);
                fas = fas | FileAttributes.ReadOnly;
                File.SetAttributes(strWorkFolder + fileName, fas);

                //課題--pdf複数出力で、どれか１つにエラーがあってもダウンロードできてしまう。
                //圧縮
                string strZipFullPath = "";
                var compress = new Compress();
                strZipFullPath = compress.CreateZipFile(strZipName, strZipFolder, strWorkFolder);

                //zipファイルパスをセット
                return zipReturnPath;


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
        /// 出力データ取得
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public DataTable GetOutputData(SystemManagementModels model) {
            try {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");

                string year = model.OutputYear;                     // 出力年
                string target = model.OutputTarget;                 // 出力対象
                string Branch = model.OutputBranch;                 // 対象支社


                //対象事業所
                string OfficeNum = ((EmployeeAdjustmentConnectionSystem.COM.Entity.Session.LoginUser)
                    (HttpContext.Current.Session["LoginUser"])).SYS管理決裁権限出力.ToString();

                string Office = string.Format("   AND LEFT(所属番号,1) = '{0}'", OfficeNum);   // 対象事業所
                if (OfficeNum == "K") 
                    Office = (Branch == "*" ? "" : string.Format("   AND LEFT(所属番号,1) = '{0}'", Branch));   // 対象事業所
                
                
                // 列名をExcel1行目に出力
                string sql = "";                                //実行するクエリを保持
                
                switch (target) { //出力対象
                    case "Objective" : // 目標管理
                        // 2018-03-20 iwai-tamura upd str ------
                        sql = " SELECT "
                            + " 社員番号 "
                            + " ,氏名 "
                            + " ,所属番号 "
                            + " ,所属名 "
                            + " ,役職番号 "
                            + " ,役職名 "
                            + " ,身分ｺｰﾄﾞ "
                            + " ,身分名 "
                            + " ,資格番号 "
                            + " ,資格名 "
                            + " ,委嘱業種 "
                            + " ,本人異動区分 "
                            + " ,目標面談者 "
                            + " ,目標面談者氏名 "
                            + " ,目標面談者所属番号 "
                            + " ,目標面談者役職番号 "
                            + " ,目標面談者異動区分 "
                            + " ,目標面談上位者 "
                            + " ,目標面談上位者氏名 "
                            + " ,目標面談上位者所属番号 "
                            + " ,目標面談上位者役職番号 "
                            + " ,目標面談上位者異動区分 "
                            + " ,目標面談部長支店長 "
                            + " ,目標面談部長支店長氏名 "
                            + " ,目標面談部長支店長所属番号 "
                            + " ,目標面談部長支店長役職番号 "
                            + " ,目標面談部長支店長異動区分 "
                            + " ,達成評価者 "
                            + " ,達成評価者氏名 "
                            + " ,達成評価者所属番号 "
                            + " ,達成評価者役職番号 "
                            + " ,達成評価者異動区分 "
                            + " ,達成評価上位者 "
                            + " ,達成評価上位者氏名 "
                            + " ,達成評価上位者所属番号 "
                            + " ,達成評価上位者役職番号 "
                            + " ,達成評価上位者異動区分 "
                            + " ,達成評価部長支店長 "
                            + " ,達成評価部長支店長氏名 "
                            + " ,達成評価部長支店長所属番号 "
                            + " ,達成評価部長支店長役職番号 "
                            + " ,達成評価部長支店長異動区分 "
                            + " ,人事担当課長 "
                            + " ,人事担当課長氏名 "
                            + " ,人事担当課長所属番号 "
                            + " ,人事担当課長役職番号 "
                            + " ,人事担当課長異動区分 "
                            + " ,総務部長 "
                            + " ,総務部長氏名 "
                            + " ,総務部長所属番号 "
                            + " ,総務部長役職番号 "
                            + " ,総務部長異動区分 "
                            + " ,支社長担当役員 "
                            + " ,支社長担当役員氏名 "
                            + " ,支社長担当役員所属番号 "
                            + " ,支社長担当役員役職番号 "
                            + " ,支社長担当役員異動区分 "
                            + " FROM SD_VT目標管理決裁権限ExcelData "
                            + " WHERE 1 = 1 "
                            + Office
                            + string.Format("   AND 年度 = {0}", year)
                            + " ORDER BY 所属番号,役職番号,社員番号";
                            
                        //sql = " SELECT * FROM SD_VT目標管理決裁権限ExcelData"
                        //    + " WHERE 1 = 1 "
                        //    + Office
                        //    + " ORDER BY 所属番号,役職番号,社員番号";
                        // 2018-03-20 iwai-tamura upd end ------
                        break;
                    case "Skill" : // 職能判定
                        // 2018-03-20 iwai-tamura upd str ------
                        sql = " SELECT "
                            + " 社員番号 "
                            + " ,氏名 "
                            + " ,所属番号 "
                            + " ,所属名 "
                            + " ,役職番号 "
                            + " ,役職名 "
                            + " ,身分ｺｰﾄﾞ "
                            + " ,身分名 "
                            + " ,資格番号 "
                            + " ,資格名 "
                            + " ,委嘱業種 "
                            + " ,本人異動区分 "
                            + " ,一次判定 "
                            + " ,一次判定氏名 "
                            + " ,一次判定所属番号 "
                            + " ,一次判定役職番号 "
                            + " ,一次判定異動区分 "
                            + " ,二次判定 "
                            + " ,二次判定氏名 "
                            + " ,二次判定所属番号 "
                            + " ,二次判定役職番号 "
                            + " ,二次判定異動区分 "
                            + " ,部門調整 "
                            + " ,部門調整氏名 "
                            + " ,部門調整所属番号 "
                            + " ,部門調整役職番号 "
                            + " ,部門調整異動区分 "
                            + " ,支社調整 "
                            + " ,支社調整氏名 "
                            + " ,支社調整所属番号 "
                            + " ,支社調整役職番号 "
                            + " ,支社調整異動区分 "
                            + " FROM SD_VT職能判定決裁権限ExcelData "
                            + " WHERE 1 = 1 "
                            + Office
                            + string.Format("   AND 年度 = {0}", year)
                            + " ORDER BY 所属番号,役職番号,社員番号";
                        //sql = " SELECT * FROM SD_VT職能判定決裁権限ExcelData"
                        //    + " WHERE 1 = 1 "
                        //    + Office
                        //    + " ORDER BY 所属番号,役職番号,社員番号";
                        // 2018-03-20 iwai-tamura upd end ------
                        break;

                    
                    // 2018-99-99 iwai-tamura upd str ------
                    case "Self" : // 自己申告書
                        sql = " SELECT "
                            + " 社員番号 "
                            + " ,氏名 "
                            + " ,所属番号 "
                            + " ,所属名 "
                            + " ,役職番号 "
                            + " ,役職名 "
                            + " ,身分ｺｰﾄﾞ "
                            + " ,身分名 "
                            + " ,資格番号 "
                            + " ,資格名 "
                            + " ,委嘱業種 "
                            + " ,本人異動区分 "
                            + " ,上司社員 "
                            + " ,上司社員氏名 "
                            + " ,上司社員所属番号 "
                            + " ,上司社員役職番号 "
                            + " ,上司社員異動区分 "
                            + " ,人事担当部長 "
                            + " ,人事担当部長氏名 "
                            + " ,人事担当部長所属番号 "
                            + " ,人事担当部長役職番号 "
                            + " ,人事担当部長異動区分 "
                            + " FROM SD_VT自己申告書決裁権限ExcelData "
                            + " WHERE 1 = 1 "
                            + Office
                            + string.Format("   AND 年度 = {0}", year)
                            + " ORDER BY 所属番号,役職番号,社員番号";
                        break;

                    case "Career" : // キャリアシート
                        sql = " SELECT "
                            + " 社員番号 "
                            + " ,氏名 "
                            + " ,所属番号 "
                            + " ,所属名 "
                            + " ,役職番号 "
                            + " ,役職名 "
                            + " ,身分ｺｰﾄﾞ "
                            + " ,身分名 "
                            + " ,資格番号 "
                            + " ,資格名 "
                            + " ,委嘱業種 "
                            + " ,本人異動区分 "
                            + " ,所属部署副長 "
                            + " ,所属部署副長氏名 "
                            + " ,所属部署副長所属番号 "
                            + " ,所属部署副長役職番号 "
                            + " ,所属部署副長異動区分 "
                            + " ,所属部署課長 "
                            + " ,所属部署課長氏名 "
                            + " ,所属部署課長所属番号 "
                            + " ,所属部署課長役職番号 "
                            + " ,所属部署課長異動区分 "
                            + " ,所属部署部長 "
                            + " ,所属部署部長氏名 "
                            + " ,所属部署部長所属番号 "
                            + " ,所属部署部長役職番号 "
                            + " ,所属部署部長異動区分 "
                            + " ,支社総務部副長 "
                            + " ,支社総務部副長氏名 "
                            + " ,支社総務部副長所属番号 "
                            + " ,支社総務部副長役職番号 "
                            + " ,支社総務部副長異動区分 "
                            + " ,支社総務部課長 "
                            + " ,支社総務部課長氏名 "
                            + " ,支社総務部課長所属番号 "
                            + " ,支社総務部課長役職番号 "
                            + " ,支社総務部課長異動区分 "
                            + " ,支社総務部部長 "
                            + " ,支社総務部部長氏名 "
                            + " ,支社総務部部長所属番号 "
                            + " ,支社総務部部長役職番号 "
                            + " ,支社総務部部長異動区分 "
                            + " ,本社人事部副長 "
                            + " ,本社人事部副長氏名 "
                            + " ,本社人事部副長所属番号 "
                            + " ,本社人事部副長役職番号 "
                            + " ,本社人事部副長異動区分 "
                            + " ,本社人事部課長 "
                            + " ,本社人事部課長氏名 "
                            + " ,本社人事部課長所属番号 "
                            + " ,本社人事部課長役職番号 "
                            + " ,本社人事部課長異動区分 "
                            + " ,本社人事部部長 "
                            + " ,本社人事部部長氏名 "
                            + " ,本社人事部部長所属番号 "
                            + " ,本社人事部部長役職番号 "
                            + " ,本社人事部部長異動区分 "
                            + " FROM SD_VTCareerSheet決裁権限ExcelData "
                            + " WHERE 1 = 1 "
                            + Office
                            + string.Format("   AND 年度 = {0}", year)
                            + " ORDER BY 所属番号,役職番号,社員番号";
                        break;
                    
                    // 2018-99-99 iwai-tamura upd end ------

                
                }

                using (DbManager dm = new DbManager())
                using (IDbCommand cmd = dm.CreateCommand(sql))
                using (DataSet ds = new DataSet()) {
                    //DbHelper.AddDbParameter(cmd, "@year", DbType.String);
                    //var parameters = cmd.Parameters.Cast<IDbDataParameter>().ToArray<IDbDataParameter>();
                    //parameters[0].Value = DataConv.IfNull(year);
                    IDataAdapter da = dm.CreateSqlDataAdapter(cmd);
                    da.Fill(ds);        // データセットに設定する
                    return ds.Tables[0];
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
        /// 読み取り専用属性を付けられたファイルを含むディレクトリの削除
        /// </summary>
        /// <param name="dir"></param>
        private void DeleteDirectory(string dir)
        {
            //DirectoryInfoオブジェクトの作成
            DirectoryInfo di = new DirectoryInfo(dir);

            //フォルダ以下のすべてのファイル、フォルダの属性を削除
            RemoveReadonlyAttribute(di);

            //フォルダを根こそぎ削除
            di.Delete(true);
        }

        /// <summary>
        /// 読み取り専用属性の解除
        /// </summary>
        /// <param name="dirInfo"></param>
        private void RemoveReadonlyAttribute(DirectoryInfo dirInfo)
        {
            //基のフォルダの属性を変更
            if ((dirInfo.Attributes & FileAttributes.ReadOnly) ==
                FileAttributes.ReadOnly)
                dirInfo.Attributes = FileAttributes.Normal;
            //フォルダ内のすべてのファイルの属性を変更
            foreach (FileInfo fi in dirInfo.GetFiles())
            {
                if ((fi.Attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                {
                    fi.Attributes = FileAttributes.Normal;
                }
            }
            //サブフォルダの属性を回帰的に変更
            foreach (DirectoryInfo di in dirInfo.GetDirectories())
            {
                RemoveReadonlyAttribute(di);
            }
        }
    }
}
