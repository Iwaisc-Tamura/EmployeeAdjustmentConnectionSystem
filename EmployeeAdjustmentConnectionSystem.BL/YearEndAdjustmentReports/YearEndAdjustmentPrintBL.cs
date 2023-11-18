using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using EmployeeAdjustmentConnectionSystem.COM.Util.Config;
using System.Data;
using EmployeeAdjustmentConnectionSystem.COM.Util.Database;
using System.IO;
using System.Web;
using EmployeeAdjustmentConnectionSystem.BL.YearEndAdjustmentReports;
using System.Globalization;
using EmployeeAdjustmentConnectionSystem.COM.Util.Convert;
using EmployeeAdjustmentConnectionSystem.BL.Common;
using EmployeeAdjustmentConnectionSystem.COM.Util.Zip;
using System.Data.SqlClient;
using EmployeeAdjustmentConnectionSystem.COM.Models;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;
using EmployeeAdjustmentConnectionSystem.BL.SelfDeclareSearch.Reports;
//using EmployeeAdjustmentConnectionSystem.BL.SelfDeclareRegister;

namespace EmployeeAdjustmentConnectionSystem.BL.YearEndAdjustmentReports {
    /// <summary>
    /// 目標管理印刷
    /// </summary>
    public class YearEndAdjustmentPrintBL {
        /// <summary>
        /// 帳票作成ディレクトリフルパス
        /// </summary>
        private readonly string TempDir;
        /// <summary>
        /// フォルダ削除基準日数
        /// </summary>
        private readonly int R_P;
        /// <summary>
        /// TBL区分(現在)
        /// </summary>
        private const string TBL_TYPE_G = "G";
        /// <summary>
        /// TBL区分(履歴)
        /// </summary>
        private const string TBL_TYPE_R = "R";

        /// <summary>
        /// ロガー
        /// </summary>
        private static NLog.Logger nlog = NLog.LogManager.GetCurrentClassLogger();

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public YearEndAdjustmentPrintBL(string fullPath) {
            Configuration config = WebConfig.GetConfigFile();
            ////帳票作成ディレクトリを取得
            //TempDir = config.AppSettings.Settings["DOWNLOAD_TEMP_DIR_O"].Value;

            ////末尾がpathの区切り文字かしらべ、違っていたら追加する。
            //if(!(TempDir.EndsWith(Path.DirectorySeparatorChar.ToString()))) {
            //    TempDir = TempDir + Path.DirectorySeparatorChar;
            //}

            TempDir = fullPath;

            //帳票作成ディレクトリ内保存日数を取得(int変換に失敗した場合は3とする。)
            if(!int.TryParse(config.AppSettings.Settings["RETENTIO_PERIOD"].Value, out R_P)) { R_P = 3; };
        }

        //2023-99-99 iwai-tamura test-str ------
        /// <summary>
        /// メインの処理
        /// </summary>
        /// <param name="selPrint"></param>
        /// <returns></returns>
        public string PrintHaiguuDeclare(string[] selPrint)
        {
            try
            {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");

                string workFolder = "";
                DateTime nowDate = DateTime.Now;      //現在日時を取得
                string userCode = ((EmployeeAdjustmentConnectionSystem.COM.Entity.Session.LoginUser)
                    (HttpContext.Current.Session["LoginUser"])).UserCode.ToString();

                //帳票作成フォルダを用意
                workFolder = TempDir + nowDate.ToString("yyyyMMdd") + Path.DirectorySeparatorChar +
                    userCode + nowDate.ToString("yyyyMMddHHmmss") + Path.DirectorySeparatorChar;

                //作成フォルダ内ファイル一覧を取得
                System.IO.Directory.CreateDirectory(workFolder);
                foreach (string file in System.IO.Directory.GetDirectories(TempDir, "*"))
                {
                    DateTime oldDirDate = new DateTime();
                    oldDirDate = System.IO.Directory.GetCreationTime(file); //作成日時を取得

                    //削除基準日より古いフォルダを削除
                    if (oldDirDate < nowDate.AddDays(-(R_P)))
                    {
                        //2017-03-31 sbc-sagara upd str 一括Excel出力で作成した読み取り専用属性を付けたファイルを削除するため
                        //System.IO.Directory.Delete(file, true);
                        DeleteDirectory(file);
                        //2017-03-31 sbc-sagara upd end 一括Excel出力で作成した読み取り専用属性を付けたファイルを削除するため
                    }
                }

                //zip作成フォルダとzipファイル名を用意を用意
                string zipFolder = TempDir + nowDate.ToString("yyyyMMdd") + Path.DirectorySeparatorChar;
                string zipName = userCode + nowDate.ToString("yyyyMMddHHmmss") + ".zip";
                //return用path文字列を用意
                string zipReturnPath = nowDate.ToString("yyyyMMdd") + Path.DirectorySeparatorChar + zipName;

                //目標管理番号ごとにデータを取得
                foreach (string KeyValue in selPrint)
                {
                    //管理番号とTBL区分に分割
                    string[] arrayData = KeyValue.Split(',');
                    string year = arrayData[0];                  //対象年度
                    string key = arrayData[1];                  //社員番号

                    DataRow row = new DataTable().NewRow();
                    DataSet dataSet = new DataSet();
                    DataTable dt = new DataTable();
                    using (DbManager dm = new DbManager())
                    {
                        //目標管理基本データを取得
                        row = GetBasicDataRow_Haiguu(key, dm, year);
                        ////目標管理承認データを取得
                        //dataSet = SelfDeclareCommonBL.GetSignData(dm, int.Parse(key));
                        //目標管理詳細データを取得
                        dt = GetDataTable_Haiguu(key, dm, year);
                    }

                    //帳票を出力
                    //職掌番号により自己申告書の分岐を行う
                    var HaiguuDeclareReport = new HaiguuDeclareReport();

                    try
                    {
                        //ファイル名作成
                        //2017-05-18 iwai-tamura upd str -----
                        string fileName = row["対象年度"].ToString()
                            + "_" + row["所属番号"].ToString()
                            + "_" + row["社員番号"].ToString()
                            + "_" + nowDate.ToString("yyyyMMddHHmmss")
                            + ".pdf";
                        //string fileName = row["社員番号"].ToString()
                        //    + "_" + row["年度"].ToString() + "_"  + row["所属番号"].ToString() + "_" + nowDate.ToString("yyyyMMddHHmmss") + ".pdf";
                        //2017-05-18 iwai-tamura upd end -----

                        HaiguuDeclareReport.SetDataSource(dt);       //目標管理詳細をセット
                        HaiguuDeclareReport.Refresh();

                        //目標管理基本パラメーターを設定
                        HaiguuDeclareBaseSetting(ref HaiguuDeclareReport, row);

                        ////目標管理承認パラメーターを設定
                        //SelfDeclareDApprovalSetting(ref crystalReportD, dataSet.Tables[0]);

                        HaiguuDeclareReport.PrintOptions.PaperOrientation = CrystalDecisions.Shared.PaperOrientation.Landscape;

                        //pdf出力
                        HaiguuDeclareReport.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, workFolder + fileName);

                    }
                    catch (Exception ex)
                    {
                        //　TODO:エラー処理検討中
                        throw ex;
                        //throw;
                    }
                    finally
                    {
                        HaiguuDeclareReport.Dispose();
                    };
                }

                // TODO:pdf複数出力で、どれか１つにエラーがあってもダウンロードできる状況
                //圧縮
                var compress = new Compress();
                string zipFullPath = compress.CreateZipFile(zipName, zipFolder, workFolder);

                //return用zipファイルパスをセット
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
        /// 基本(ヘッダー)を取得
        /// </summary>
        /// <param name="keyVal">社員番号</param>
        /// <returns>目標管理基本情報のDataRow</returns>
        private DataRow GetBasicDataRow_Haiguu(string keyVal, DbManager dm, string year)
        {
            try
            {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");

                string sql = ""; //クエリ生成

                sql = "SELECT * FROM TE120基礎控除申告書Data WHERE 社員番号 = @key ";
                sql += " AND 対象年度 = " + year ;
                DataTable dt = new DataTable();
                DataSet dataSet = new DataSet();
                using (IDbCommand cmd = dm.CreateCommand(sql))
                {
                    DbHelper.AddDbParameter(cmd, "@key", DbType.String);
                    //パラメータ設定
                    var parameters = cmd.Parameters.Cast<IDbDataParameter>().ToArray<IDbDataParameter>();
                    parameters[0].Value = keyVal;
                    //クエリ実行
                    IDataAdapter da = dm.CreateSqlDataAdapter(cmd);
                    da.Fill(dataSet);
                }

                //実行結果確認
                if (dataSet.Tables[0].Rows.Count == 0)
                {
                    //0件字はエラー?
                }
                return dataSet.Tables[0].Rows[0];
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
        /// 配偶者控除データ
        /// </summary>
        /// <param name="keyVal"></param>
        /// <param name="pattern"></param>
        /// <returns></returns>
        private DataTable GetDataTable_Haiguu(string keyVal, DbManager dm, string year)
        {
            try
            {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");

                string sql = "";
                sql = " SELECT "
                    + "     基礎控除.対象年度 "
                    + "     ,基礎控除.社員番号 "
                    + "     ,基礎控除.本人確定区分 "
                    + "	    ,基礎控除.管理者確定区分 "
                    //2023-11-16 iwai-terao upd str ------
                    //+ "	    ,基礎控除.個人番号相違確認区分 "
                    + "     ,Isnull(基礎控除.個人番号相違確認区分,'0') AS 個人番号相違確認区分 "
                    //2023-11-16 iwai-terao upd end ------
                    + "	    ,基礎控除.所属番号 "
                    + "     ,基礎控除.氏名_姓 + ' ' + 基礎控除.氏名_名 AS 氏名 "
                    + "     ,基礎控除.Kana_姓 + ' ' + 基礎控除.Kana_名 AS Kana "
                    + "     ,基礎控除.住所01 "
                    + "     ,基礎控除.Sequence番号 "
                    + "     ,基礎控除.基礎控除申告書_給与所得_収入金額 "
                    + "     ,基礎控除.基礎控除申告書_給与所得_所得金額 "
                    + "     ,基礎控除.基礎控除申告書_他_所得金額 "
                    + "     ,基礎控除.基礎控除申告書_合計所得金額見積額 "
                    + "     ,基礎控除.基礎控除申告書_控除額計算判定 "
                    + "     ,基礎控除.基礎控除申告書_控除額計算区分 "
                    + "     ,基礎控除.基礎控除申告書_基礎控除額 "
                    + "     ,基礎控除.配偶者控除申告書_氏名_姓 + ' ' + 基礎控除.配偶者控除申告書_氏名_名 as 配偶者控除申告書_氏名 "
                    + "     ,基礎控除.配偶者控除申告書_Kana_姓 + ' ' + 基礎控除.配偶者控除申告書_Kana_名 as 配偶者控除申告書_Kana "
                    + "     ,配偶者続柄名.続柄名称 AS 配偶者控除申告書_続柄名称 "
                    + "     ,基礎控除.配偶者控除申告書_生年月日 "
                    + "     ,LEFT(基礎控除.配偶者控除申告書_生年月日,4) AS 配偶者控除申告書_生年月日年 "
                    + "     ,SUBSTRING(基礎控除.配偶者控除申告書_生年月日,5,2) AS 配偶者控除申告書_生年月日月 "
                    + "     ,SUBSTRING(基礎控除.配偶者控除申告書_生年月日,7,2) AS 配偶者控除申告書_生年月日日 "
                    + "     ,基礎控除.配偶者控除申告書_住所 "
                    + "     ,基礎控除.配偶者控除申告書_非居住者 "
                    + "     ,基礎控除.配偶者控除申告書_給与所得_収入金額 "
                    + "     ,基礎控除.配偶者控除申告書_給与所得_所得金額 "
                    + "     ,基礎控除.配偶者控除申告書_他_所得金額 "
                    + "     ,基礎控除.配偶者控除申告書_合計所得金額見積額 "
                    + "     ,Isnull(基礎控除.配偶者控除申告書_控除額計算判定,'0') AS 配偶者控除申告書_控除額計算判定 "
                    + "     ,基礎控除.配偶者控除申告書_控除額計算区分 "
                    + "     ,基礎控除.配偶者控除申告書_配偶者控除額 "
                    + "     ,基礎控除.配偶者控除申告書_配偶者特別控除額 "
                    + "     ,Isnull(基礎控除.所得金額調整控除申告書_要件区分,'0') AS 所得金額調整控除申告書_要件区分 "
                    + "     ,基礎控除.所得金額調整控除申告書_扶養親族等氏名_姓 + ' ' + 基礎控除.所得金額調整控除申告書_扶養親族等氏名_名 AS 所得金額調整控除申告書_扶養親族等氏名 "
                    + "     ,基礎控除.所得金額調整控除申告書_扶養親族等Kana_姓 + ' ' + 基礎控除.所得金額調整控除申告書_扶養親族等Kana_名 AS 所得金額調整控除申告書_扶養親族等Kana "
                    + "     ,基礎控除.所得金額調整控除申告書_扶養親族等生年月日 "
                    + "     ,LEFT(基礎控除.所得金額調整控除申告書_扶養親族等生年月日,4) AS 所得金額調整控除申告書_扶養親族等表示生年月日年 "
                    + "     ,SUBSTRING(基礎控除.所得金額調整控除申告書_扶養親族等生年月日,5,2) AS 所得金額調整控除申告書_扶養親族等表示生年月日月 "
                    + "     ,SUBSTRING(基礎控除.所得金額調整控除申告書_扶養親族等生年月日,7,2) AS 所得金額調整控除申告書_扶養親族等表示生年月日日 "
                    + "     ,基礎控除.所得金額調整控除申告書_扶養親族等同上区分 "
                    + "     ,基礎控除.所得金額調整控除申告書_扶養親族等住所 "
                    + "     ,扶養親族等続柄名.続柄名称 AS 所得金額調整控除申告書_扶養親族等続柄名称 "
                    + "     ,基礎控除.所得金額調整控除申告書_扶養親族等所得金額 "
                    //2023-11-16 iwai-terao upd str ------
                    //+ "     ,基礎控除.所得金額調整控除申告書_特別障害者該当事実 "
                    + "     ,Isnull(基礎控除.所得金額調整控除申告書_特別障害者該当事実,'0') AS 所得金額調整控除申告書_特別障害者該当事実 "
                    //2023-11-16 iwai-terao upd end ------
                    + "     ,事業所名.税務署名 AS 税務署長 "
                    + "     ,事業所名.会社名 AS 給与支払者名称 "
                    + "     ,事業所名.所在地 AS 給与支払者所在地 "

                    //2023-11-16 iwai-terao upd str ------
                    + "     ,'3' AS 法人個人番号01 "
                    + "     ,'0' AS 法人個人番号02 "
                    + "     ,'1' AS 法人個人番号03 "
                    + "     ,'0' AS 法人個人番号04 "
                    + "     ,'0' AS 法人個人番号05 "
                    + "     ,'0' AS 法人個人番号06 "
                    + "     ,'1' AS 法人個人番号07 "
                    + "     ,'0' AS 法人個人番号08 "
                    + "     ,'3' AS 法人個人番号09 "
                    + "     ,'3' AS 法人個人番号10 "
                    + "     ,'3' AS 法人個人番号11 "
                    + "     ,'7' AS 法人個人番号12 "
                    + "     ,'5' AS 法人個人番号13 "
                    //2023-11-16 iwai-terao upd end ------
                    + " FROM TE120基礎控除申告書Data AS 基礎控除"
                    + "  LEFT JOIN TM911続柄名Master AS 配偶者続柄名 ON 基礎控除.配偶者控除申告書_続柄 = 配偶者続柄名.続柄番号 "
                    + "  LEFT JOIN TM911続柄名Master AS 扶養親族等続柄名 ON 基礎控除.所得金額調整控除申告書_扶養親族等続柄 = 扶養親族等続柄名.続柄番号 "
                    + "  LEFT JOIN TM912事業所名Master AS 事業所名 ON CASE WHEN LEFT(基礎控除.所属番号, 1) IN ('2', '3') THEN '1' ELSE LEFT(基礎控除.所属番号, 1) END = 事業所名.事業所番号 "
                    + " WHERE 基礎控除.社員番号 = @key "
                    + "   AND 基礎控除.対象年度 = " + year ;

                DataTable dt = new DataTable();
                DataSet dataSet = new DataSet();
                using (IDbCommand cmd = dm.CreateCommand(sql))
                {
                    DbHelper.AddDbParameter(cmd, "@key", DbType.String);
                    //パラメータ設定
                    var parameters = cmd.Parameters.Cast<IDbDataParameter>().ToArray<IDbDataParameter>();
                    parameters[0].Value = keyVal;
                    //クエリ実行
                    IDataAdapter da = dm.CreateSqlDataAdapter(cmd);
                    da.Fill(dataSet);
                }

                return dataSet.Tables[0];

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
        /// 目標基本を帳票にセット
        /// </summary>
        /// <param name="cr">クリスタルレポート</param>
        /// <param name="dr">目標管理基本DataRow</param>
        /// <returns>セット後のクリスタルレポート</returns>
        private void HaiguuDeclareBaseSetting(ref HaiguuDeclareReport cr, DataRow dr)
        {
            try
            {
                //開始
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");
                //datetimeに変換できたら和暦、だめだったら空をreturn
                Func<string, string, string> wareki = delegate (string val, string format) {
                    DateTime dt = new DateTime();
                    return DateTime.TryParse(val, out dt) ? DataConv.Date2Jcal(dt, format) : "";
                };

                //2020-04-07 iwai-tamura upd str ------
                Func<string, string, string> nengappi = delegate (string val, string format) {
                    DateTime dt = new DateTime();
                    return DateTime.TryParse(val, out dt) ? dt.ToString(format) : "";
                };
                //2020-04-07 iwai-tamura upd end ------     

                //年齢表示用
                Func<string, string> ageView = (val1) => {
                    if (!string.IsNullOrEmpty(val1))
                    {
                        return val1 + "歳";
                    }
                    return null;
                };

                //チェックボックス表示用
                Func<string, string> checkView = (val1) => {
                    if (!string.IsNullOrEmpty(val1))
                    {
                        return (val1 == "1") ? "☑" : "□";
                    }
                    return "□";
                };

                //cr.SetParameterValue("現在年月日", nengappi(dr["年度"].ToString() + "/12/31", "yyyy年M月d日現在"));
                //cr.SetParameterValue("タイトル年度", nengappi(dr["年度"].ToString() + "/01/01", "yyyy年度"));

                //cr.SetParameterValue("氏名", DataConv.IfNull(dr["氏名"].ToString(), ""));
                //cr.SetParameterValue("社員番号", DataConv.IfNull(dr["社員番号"].ToString(), ""));
                //cr.SetParameterValue("所属", DataConv.IfNull(dr["所属名"].ToString(), ""));
                //cr.SetParameterValue("役職名", DataConv.IfNull(dr["役職名"].ToString(), ""));
                //cr.SetParameterValue("資格", DataConv.IfNull(dr["資格名"].ToString(), ""));


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
        //2023-99-99 iwai-tamura test-end ------

        //2023-99-99 iwai-terao test-str 扶養控除ボタン------
        /// <summary>
        /// メインの処理
        /// </summary>
        /// <param name="selPrint"></param>
        /// <returns></returns>
        public string PrintHuyouDeclare(string[] selPrint)
        {
            try
            {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");

                string workFolder = "";
                DateTime nowDate = DateTime.Now;      //現在日時を取得
                string userCode = ((EmployeeAdjustmentConnectionSystem.COM.Entity.Session.LoginUser)
                    (HttpContext.Current.Session["LoginUser"])).UserCode.ToString();

                //帳票作成フォルダを用意
                workFolder = TempDir + nowDate.ToString("yyyyMMdd") + Path.DirectorySeparatorChar +
                    userCode + nowDate.ToString("yyyyMMddHHmmss") + Path.DirectorySeparatorChar;

                //作成フォルダ内ファイル一覧を取得
                System.IO.Directory.CreateDirectory(workFolder);
                foreach (string file in System.IO.Directory.GetDirectories(TempDir, "*"))
                {
                    DateTime oldDirDate = new DateTime();
                    oldDirDate = System.IO.Directory.GetCreationTime(file); //作成日時を取得

                    //削除基準日より古いフォルダを削除
                    if (oldDirDate < nowDate.AddDays(-(R_P)))
                    {
                        //2017-03-31 sbc-sagara upd str 一括Excel出力で作成した読み取り専用属性を付けたファイルを削除するため
                        //System.IO.Directory.Delete(file, true);
                        DeleteDirectory(file);
                        //2017-03-31 sbc-sagara upd end 一括Excel出力で作成した読み取り専用属性を付けたファイルを削除するため
                    }
                }

                //zip作成フォルダとzipファイル名を用意を用意
                string zipFolder = TempDir + nowDate.ToString("yyyyMMdd") + Path.DirectorySeparatorChar;
                string zipName = userCode + nowDate.ToString("yyyyMMddHHmmss") + ".zip";
                //return用path文字列を用意
                string zipReturnPath = nowDate.ToString("yyyyMMdd") + Path.DirectorySeparatorChar + zipName;

                //目標管理番号ごとにデータを取得
                foreach (string KeyValue in selPrint)
                {
                    //管理番号とTBL区分に分割
                    string[] arrayData = KeyValue.Split(',');
                    string year = arrayData[0];                  //対象年度
                    string key = arrayData[1];                  //社員番号

                    DataRow row = new DataTable().NewRow();
                    DataSet dataSet = new DataSet();
                    DataTable dt = new DataTable();
                    using (DbManager dm = new DbManager())
                    {
                        //扶養控除基本データを取得
                        row = GetBasicDataRow_Huyou(key, dm, year);
                        //扶養控除詳細データを取得
                        dt = GetDataTable_Huyou(key, dm, year);
                    }

                    //帳票を出力
                    //職掌番号により自己申告書の分岐を行う
                    var HuyouDeclareReport = new HuyouDeclareReport();

                    try
                    {
                        //ファイル名作成
                        string fileName = row["対象年度"].ToString()
                            + "_" + row["所属番号"].ToString()
                            + "_" + row["社員番号"].ToString()
                            + "_" + nowDate.ToString("yyyyMMddHHmmss")
                            + ".pdf";

                        HuyouDeclareReport.SetDataSource(dt);       //目標管理詳細をセット
                        HuyouDeclareReport.Refresh();

                        //目標管理基本パラメーターを設定
                        HuyouDeclareBaseSetting(ref HuyouDeclareReport, row);

                        ////目標管理承認パラメーターを設定
                        //SelfDeclareDApprovalSetting(ref crystalReportD, dataSet.Tables[0]);

                        HuyouDeclareReport.PrintOptions.PaperOrientation = CrystalDecisions.Shared.PaperOrientation.Landscape;

                        //pdf出力
                        HuyouDeclareReport.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, workFolder + fileName);

                    }
                    catch (Exception ex)
                    {
                        //　TODO:エラー処理検討中
                        throw ex;
                        //throw;
                    }
                    finally
                    {
                        HuyouDeclareReport.Dispose();
                    };
                }

                // TODO:pdf複数出力で、どれか１つにエラーがあってもダウンロードできる状況
                //圧縮
                var compress = new Compress();
                string zipFullPath = compress.CreateZipFile(zipName, zipFolder, workFolder);

                //return用zipファイルパスをセット
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
        /// 基本(ヘッダー)を取得
        /// </summary>
        /// <param name="keyVal">社員番号</param>
        /// <returns>目標管理基本情報のDataRow</returns>
        private DataRow GetBasicDataRow_Huyou(string keyVal, DbManager dm, string year)
        {
            try
            {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");

                string sql = ""; //クエリ生成

                sql = "SELECT * FROM TE100扶養控除申告書Data WHERE 社員番号 = @key ";
                sql += " AND 対象年度 = " + year ;

                DataTable dt = new DataTable();
                DataSet dataSet = new DataSet();
                using (IDbCommand cmd = dm.CreateCommand(sql))
                {
                    DbHelper.AddDbParameter(cmd, "@key", DbType.String);
                    //パラメータ設定
                    var parameters = cmd.Parameters.Cast<IDbDataParameter>().ToArray<IDbDataParameter>();
                    parameters[0].Value = keyVal;
                    //クエリ実行
                    IDataAdapter da = dm.CreateSqlDataAdapter(cmd);
                    da.Fill(dataSet);
                }

                //実行結果確認
                if (dataSet.Tables[0].Rows.Count == 0)
                {
                    //0件字はエラー?
                }
                return dataSet.Tables[0].Rows[0];
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
        /// 扶養控除データ
        /// </summary>
        /// <param name="keyVal"></param>
        /// <param name="pattern"></param>
        /// <returns></returns>
        private DataTable GetDataTable_Huyou(string keyVal, DbManager dm, string year)
        {
            try
            {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");

                string sql = "";
                sql = " SELECT "
                    + "     扶養控除.対象年度 "
                    + "     ,扶養控除.社員番号 "
                    + "     ,扶養控除.本人確定区分 "
                    + "     ,扶養控除.管理者確定区分 "
                    + "     ,扶養控除.個人番号相違確認区分 "
                    + "     ,扶養控除.所属番号 "
                    + "     ,扶養控除.氏名_姓 + ' ' + 扶養控除.氏名_名 AS 氏名 "
                    + "     ,扶養控除.Kana_姓 + ' ' + 扶養控除.Kana_名 AS Kana "
                    + "     ,扶養控除.生年月日 "
                    + "     ,LEFT(扶養控除.生年月日,4) AS 生年月日年 "
                    + "     ,SUBSTRING(扶養控除.生年月日,5,2) AS 生年月日月 "
                    + "     ,SUBSTRING(扶養控除.生年月日,7,2) AS 生年月日日 "
                    + "     ,扶養控除.世帯主氏名_姓 + ' ' + 扶養控除.世帯主氏名_名 AS 世帯主氏名 "
                    + "     ,世帯主続柄名.続柄名称 AS 世帯主続柄名称 "
                    + "     ,扶養控除.郵便番号_前 "
                    + "     ,扶養控除.郵便番号_後 "
                    + "     ,扶養控除.住所01 "
                    + "     ,ISNULL(扶養控除.配偶者有無,'0') AS 配偶者有無 "
                    + "     ,扶養控除.Sequence番号 "
                    + "     ,扶養控除.源泉控除対象配偶者対象外区分 "
                    + "     ,扶養控除.源泉控除対象配偶者氏名_姓 + ' ' + 扶養控除.源泉控除対象配偶者氏名_名 AS 源泉控除対象配偶者氏名 "
                    + "     ,扶養控除.源泉控除対象配偶者Kana_姓 + ' ' + 扶養控除.源泉控除対象配偶者Kana_名 AS 源泉控除対象配偶者Kana "
                    + "     ,扶養控除.源泉控除対象配偶者生年月日 "
                    + "     ,LEFT(扶養控除.源泉控除対象配偶者生年月日,4) AS 源泉控除対象配偶者生年月日年 "
                    + "     ,SUBSTRING(扶養控除.源泉控除対象配偶者生年月日,5,2) AS 源泉控除対象配偶者生年月日月 "
                    + "     ,SUBSTRING(扶養控除.源泉控除対象配偶者生年月日,7,2) AS 源泉控除対象配偶者生年月日日 "
                    + "     ,扶養控除.源泉控除対象配偶者所得見積額 "
                    //2023-11-18 iwai-terao upd str ------
                    //+ "     ,扶養控除.源泉控除対象配偶者非居住者 "
                    + "     ,ISNULL(扶養控除.源泉控除対象配偶者非居住者,'0') AS 源泉控除対象配偶者非居住者 "
                    //2023-11-18 iwai-terao upd end ------
                    + "     ,扶養控除.源泉控除対象配偶者住所 "
                    + "     ,扶養控除.源泉控除対象配偶者異動月日 "
                    + "     ,扶養控除.源泉控除対象配偶者事由 "
                    + "     ,扶養控除.控除対象扶養親族01_対象外区分 "
                    + "     ,扶養控除.控除対象扶養親族01_氏名_姓 + ' ' + 扶養控除.控除対象扶養親族01_氏名_名 AS 控除対象扶養親族01_氏名 "
                    + "     ,扶養控除.控除対象扶養親族01_Kana_姓 + ' ' + 扶養控除.控除対象扶養親族01_Kana_名 AS 控除対象扶養親族01_Kana "
                    + "     ,扶養親族01続柄.続柄名称 AS 控除対象扶養親族01_続柄名称 "
                    + "     ,扶養控除.控除対象扶養親族01_生年月日 "
                    + "     ,LEFT(扶養控除.控除対象扶養親族01_生年月日,4) AS 控除対象扶養親族01_生年月日年 "
                    + "     ,SUBSTRING(扶養控除.控除対象扶養親族01_生年月日,5,2) AS 控除対象扶養親族01_生年月日月 "
                    + "     ,SUBSTRING(扶養控除.控除対象扶養親族01_生年月日,7,2) AS 控除対象扶養親族01_生年月日日 "
                    + "     ,ISNULL(扶養控除.控除対象扶養親族01_老人扶養親族区分,'0') AS 控除対象扶養親族01_老人扶養親族区分 "
                    + "     ,ISNULL(扶養控除.控除対象扶養親族01_特定扶養親族区分,'0') AS 控除対象扶養親族01_特定扶養親族区分 "
                    + "     ,扶養控除.控除対象扶養親族01_所得見積額 "
                    //2023-11-18 iwai-terao upd str ------
                    //+ "     ,扶養控除.控除対象扶養親族01_非居住者 "
                    + "     ,ISNULL(扶養控除.控除対象扶養親族01_非居住者,'0') AS 控除対象扶養親族01_非居住者 "
                    //2023-11-18 iwai-terao upd end ------
                    + "     ,扶養控除.控除対象扶養親族01_住所 "
                    + "     ,扶養控除.控除対象扶養親族01_異動月日 "
                    + "     ,扶養控除.控除対象扶養親族01_事由 "
                    + "     ,扶養控除.控除対象扶養親族02_対象外区分 "
                    + "     ,扶養控除.控除対象扶養親族02_氏名_姓 + ' ' + 扶養控除.控除対象扶養親族02_氏名_名 AS 控除対象扶養親族02_氏名 "
                    + "     ,扶養控除.控除対象扶養親族02_Kana_姓 + ' ' + 扶養控除.控除対象扶養親族02_Kana_名 AS 控除対象扶養親族02_Kana "
                    + "     ,扶養親族02続柄.続柄名称 AS 控除対象扶養親族02_続柄名称 "
                    + "     ,扶養控除.控除対象扶養親族02_生年月日 "
                    + "     ,LEFT(扶養控除.控除対象扶養親族02_生年月日,4) AS 控除対象扶養親族02_生年月日年 "
                    + "     ,SUBSTRING(扶養控除.控除対象扶養親族02_生年月日,5,2) AS 控除対象扶養親族02_生年月日月 "
                    + "     ,SUBSTRING(扶養控除.控除対象扶養親族02_生年月日,7,2) AS 控除対象扶養親族02_生年月日日 "
                    + "     ,ISNULL(扶養控除.控除対象扶養親族02_老人扶養親族区分,'0') AS 控除対象扶養親族02_老人扶養親族区分 "
                    + "     ,ISNULL(扶養控除.控除対象扶養親族02_特定扶養親族区分,'0') AS 控除対象扶養親族02_特定扶養親族区分 "
                    + "     ,扶養控除.控除対象扶養親族02_所得見積額 "
                    //2023-11-18 iwai-terao upd str ------
                    //+ "     ,扶養控除.控除対象扶養親族02_非居住者 "
                    + "     ,ISNULL(扶養控除.控除対象扶養親族02_非居住者,'0') AS 控除対象扶養親族02_非居住者 "
                    //2023-11-18 iwai-terao upd end ------
                    + "     ,扶養控除.控除対象扶養親族02_住所 "
                    + "     ,扶養控除.控除対象扶養親族02_異動月日 "
                    + "     ,扶養控除.控除対象扶養親族02_事由 "
                    + "     ,扶養控除.控除対象扶養親族03_対象外区分 "
                    + "     ,扶養控除.控除対象扶養親族03_氏名_姓 + ' ' + 扶養控除.控除対象扶養親族03_氏名_名 AS 控除対象扶養親族03_氏名 "
                    + "     ,扶養控除.控除対象扶養親族03_Kana_姓 + ' ' + 扶養控除.控除対象扶養親族03_Kana_名 AS 控除対象扶養親族03_Kana "
                    + "     ,扶養親族03続柄.続柄名称 AS 控除対象扶養親族03_続柄名称 "
                    + "     ,扶養控除.控除対象扶養親族03_生年月日 "
                    + "     ,LEFT(扶養控除.控除対象扶養親族03_生年月日,4) AS 控除対象扶養親族03_生年月日年 "
                    + "     ,SUBSTRING(扶養控除.控除対象扶養親族03_生年月日,5,2) AS 控除対象扶養親族03_生年月日月 "
                    + "     ,SUBSTRING(扶養控除.控除対象扶養親族03_生年月日,7,2) AS 控除対象扶養親族03_生年月日日 "
                    + "     ,ISNULL(扶養控除.控除対象扶養親族03_老人扶養親族区分,'0') AS 控除対象扶養親族03_老人扶養親族区分 "
                    + "     ,ISNULL(扶養控除.控除対象扶養親族03_特定扶養親族区分,'0') AS 控除対象扶養親族03_特定扶養親族区分 "
                    + "     ,扶養控除.控除対象扶養親族03_所得見積額 "
                    //2023-11-18 iwai-terao upd str ------
                    //+ "     ,扶養控除.控除対象扶養親族03_非居住者 "
                    + "     ,ISNULL(扶養控除.控除対象扶養親族03_非居住者,'0') AS 控除対象扶養親族03_非居住者 "
                    //2023-11-18 iwai-terao upd end ------
                    + "     ,扶養控除.控除対象扶養親族03_住所 "
                    + "     ,扶養控除.控除対象扶養親族03_異動月日 "
                    + "     ,扶養控除.控除対象扶養親族03_事由 "
                    + "     ,扶養控除.控除対象扶養親族04_対象外区分 "
                    + "     ,扶養控除.控除対象扶養親族04_氏名_姓 + ' ' + 扶養控除.控除対象扶養親族04_氏名_名 AS 控除対象扶養親族04_氏名 "
                    + "     ,扶養控除.控除対象扶養親族04_Kana_姓 + ' ' + 扶養控除.控除対象扶養親族04_Kana_名 AS 控除対象扶養親族04_Kana "
                    + "     ,扶養親族04続柄.続柄名称 AS 控除対象扶養親族04_続柄名称 "
                    + "     ,扶養控除.控除対象扶養親族04_生年月日 "
                    + "     ,LEFT(扶養控除.控除対象扶養親族04_生年月日,4) AS 控除対象扶養親族04_生年月日年 "
                    + "     ,SUBSTRING(扶養控除.控除対象扶養親族04_生年月日,5,2) AS 控除対象扶養親族04_生年月日月 "
                    + "     ,SUBSTRING(扶養控除.控除対象扶養親族04_生年月日,7,2) AS 控除対象扶養親族04_生年月日日 "
                    + "     ,ISNULL(扶養控除.控除対象扶養親族04_老人扶養親族区分,'0') AS 控除対象扶養親族04_老人扶養親族区分 "
                    + "     ,ISNULL(扶養控除.控除対象扶養親族04_特定扶養親族区分,'0') AS 控除対象扶養親族04_特定扶養親族区分 "
                    + "     ,扶養控除.控除対象扶養親族04_所得見積額 "
                    //2023-11-18 iwai-terao upd str ------
                    //+ "     ,扶養控除.控除対象扶養親族04_非居住者 "
                    + "     ,ISNULL(扶養控除.控除対象扶養親族04_非居住者,'0') AS 控除対象扶養親族04_非居住者 "
                    //2023-11-18 iwai-terao upd end ------
                    + "     ,扶養控除.控除対象扶養親族04_住所 "
                    + "     ,扶養控除.控除対象扶養親族04_異動月日 "
                    + "     ,扶養控除.控除対象扶養親族04_事由 "
                    + "     ,ISNULL(扶養控除.障害者,'0') AS 障害者 "
                    + "     ,扶養控除.一般障害_本人 "
                    + "     ,扶養控除.一般障害_同一生計配偶者 "
                    + "     ,扶養控除.一般障害_扶養親族 "
                    + "     ,扶養控除.一般障害_扶養親族人数 "
                    + "     ,扶養控除.特別障害者_本人 "
                    + "     ,扶養控除.特別障害者_同一生計配偶者 "
                    + "     ,扶養控除.特別障害者_扶養親族 "
                    + "     ,扶養控除.特別障害者_扶養親族人数 "
                    + "     ,扶養控除.同居特別障害者_同一生計配偶者 "
                    + "     ,扶養控除.同居特別障害者_扶養親族 "
                    + "     ,扶養控除.同居特別障害者_扶養親族人数 "
                    + "     ,ISNULL(扶養控除.寡婦一人親区分,'0') AS 寡婦一人親区分 "
                    + "     ,扶養控除.理由区分 "
                    + "     ,扶養控除.発生年月日 "
                    + "     ,ISNULL(扶養控除.勤労学生,'0') AS 勤労学生 "
                    + "     ,扶養控除.障害異動月日 "
                    + "     ,扶養控除.障害事由 "
                    + "     ,扶養控除.扶養親族16未満01_対象外区分 "
                    + "     ,扶養控除.扶養親族16未満01_氏名_姓 + ' ' + 扶養控除.扶養親族16未満01_氏名_名 AS 扶養親族16未満01_氏名 "
                    + "     ,扶養控除.扶養親族16未満01_Kana_姓 + ' ' + 扶養控除.扶養親族16未満01_Kana_名 AS 扶養親族16未満01_Kana "
                    + "     ,未満01続柄.続柄名称 AS 扶養親族16未満01_続柄名称 "
                    + "     ,扶養控除.扶養親族16未満01_生年月日 "
                    + "     ,LEFT(扶養控除.扶養親族16未満01_生年月日,4) AS 扶養親族16未満01_生年月日年 "
                    + "     ,SUBSTRING(扶養控除.扶養親族16未満01_生年月日,5,2) AS 扶養親族16未満01_生年月日月 "
                    + "     ,SUBSTRING(扶養控除.扶養親族16未満01_生年月日,7,2) AS 扶養親族16未満01_生年月日日 "
                    + "     ,扶養控除.扶養親族16未満01_同上区分 "
                    + "     ,扶養控除.扶養親族16未満01_住所 "
                    + "     ,扶養控除.扶養親族16未満01_国外区分 "
                    + "     ,扶養控除.扶養親族16未満01_所得見積額 "
                    + "     ,扶養控除.扶養親族16未満01_異動月日 "
                    + "     ,扶養控除.扶養親族16未満01_事由 "
                    + "     ,扶養控除.扶養親族16未満02_対象外区分 "
                    + "     ,扶養控除.扶養親族16未満02_氏名_姓 + ' ' + 扶養控除.扶養親族16未満02_氏名_名 AS 扶養親族16未満02_氏名 "
                    + "     ,扶養控除.扶養親族16未満02_Kana_姓 + ' ' + 扶養控除.扶養親族16未満02_Kana_名 AS 扶養親族16未満02_Kana "
                    + "     ,未満02続柄.続柄名称 AS 扶養親族16未満02_続柄名称 "
                    + "     ,扶養控除.扶養親族16未満02_生年月日 "
                    + "     ,LEFT(扶養控除.扶養親族16未満02_生年月日,4) AS 扶養親族16未満02_生年月日年 "
                    + "     ,SUBSTRING(扶養控除.扶養親族16未満02_生年月日,5,2) AS 扶養親族16未満02_生年月日月 "
                    + "     ,SUBSTRING(扶養控除.扶養親族16未満02_生年月日,7,2) AS 扶養親族16未満02_生年月日日 "
                    + "     ,扶養控除.扶養親族16未満02_同上区分 "
                    + "     ,扶養控除.扶養親族16未満02_住所 "
                    + "     ,扶養控除.扶養親族16未満02_国外区分 "
                    + "     ,扶養控除.扶養親族16未満02_所得見積額 "
                    + "     ,扶養控除.扶養親族16未満02_異動月日 "
                    + "     ,扶養控除.扶養親族16未満02_事由 "
                    + "     ,扶養控除.扶養親族16未満03_対象外区分 "
                    + "     ,扶養控除.扶養親族16未満03_氏名_姓 + ' ' + 扶養控除.扶養親族16未満03_氏名_名 AS 扶養親族16未満03_氏名 "
                    + "     ,扶養控除.扶養親族16未満03_Kana_姓 + ' ' + 扶養控除.扶養親族16未満03_Kana_名 AS 扶養親族16未満03_Kana "
                    + "     ,未満03続柄.続柄名称 AS 扶養親族16未満03_続柄名称 "
                    + "     ,扶養控除.扶養親族16未満03_生年月日 "
                    + "     ,扶養控除.扶養親族16未満03_同上区分 "
                    + "     ,扶養控除.扶養親族16未満03_住所 "
                    + "     ,扶養控除.扶養親族16未満03_国外区分 "
                    + "     ,扶養控除.扶養親族16未満03_所得見積額 "
                    + "     ,扶養控除.扶養親族16未満03_異動月日 "
                    + "     ,扶養控除.扶養親族16未満03_事由 "
                    + "     ,扶養控除.扶養親族16未満04_対象外区分 "
                    + "     ,扶養控除.扶養親族16未満04_氏名_姓 + ' ' + 扶養控除.扶養親族16未満04_氏名_名 AS 扶養親族16未満04_氏名 "
                    + "     ,扶養控除.扶養親族16未満04_Kana_姓 + ' ' + 扶養控除.扶養親族16未満04_Kana_名 AS 扶養親族16未満04_Kana "
                    + "     ,未満04続柄.続柄名称 AS 扶養親族16未満04_続柄名称 "
                    + "     ,扶養控除.扶養親族16未満04_生年月日 "
                    + "     ,扶養控除.扶養親族16未満04_同上区分 "
                    + "     ,扶養控除.扶養親族16未満04_住所 "
                    + "     ,扶養控除.扶養親族16未満04_国外区分 "
                    + "     ,扶養控除.扶養親族16未満04_所得見積額 "
                    + "     ,扶養控除.扶養親族16未満04_異動月日 "
                    + "     ,扶養控除.扶養親族16未満04_事由 "
                    + "     ,事業所名.税務署名 AS 税務署長 "
                    + "     ,事業所名.会社名 AS 給与支払者名称 "
                    + "     ,事業所名.所在地 AS 給与支払者所在地 "

                    //2023-11-16 iwai-terao upd str ------
                    + "     ,扶養控除.源泉控除対象配偶者対象外区分 "
                    + "     ,'3' AS 法人個人番号01 "
                    + "     ,'0' AS 法人個人番号02 "
                    + "     ,'1' AS 法人個人番号03 "
                    + "     ,'0' AS 法人個人番号04 "
                    + "     ,'0' AS 法人個人番号05 "
                    + "     ,'0' AS 法人個人番号06 "
                    + "     ,'1' AS 法人個人番号07 "
                    + "     ,'0' AS 法人個人番号08 "
                    + "     ,'3' AS 法人個人番号09 "
                    + "     ,'3' AS 法人個人番号10 "
                    + "     ,'3' AS 法人個人番号11 "
                    + "     ,'7' AS 法人個人番号12 "
                    + "     ,'5' AS 法人個人番号13 "
                    //2023-11-16 iwai-terao upd end ------
                    + " FROM TE100扶養控除申告書Data AS 扶養控除 "
                    + "  LEFT JOIN TM911続柄名Master AS 世帯主続柄名 ON 扶養控除.世帯主続柄 = 世帯主続柄名.続柄番号 "
                    + "  LEFT JOIN TM911続柄名Master AS 扶養親族01続柄 ON 扶養控除.控除対象扶養親族01_続柄 = 扶養親族01続柄.続柄番号 "
                    + "  LEFT JOIN TM911続柄名Master AS 扶養親族02続柄 ON 扶養控除.控除対象扶養親族02_続柄 = 扶養親族02続柄.続柄番号 "
                    + "  LEFT JOIN TM911続柄名Master AS 扶養親族03続柄 ON 扶養控除.控除対象扶養親族03_続柄 = 扶養親族03続柄.続柄番号 "
                    + "  LEFT JOIN TM911続柄名Master AS 扶養親族04続柄 ON 扶養控除.控除対象扶養親族04_続柄 = 扶養親族04続柄.続柄番号 "
                    + "  LEFT JOIN TM911続柄名Master AS 未満01続柄 ON 扶養控除.扶養親族16未満01_続柄 = 未満01続柄.続柄番号 "
                    + "  LEFT JOIN TM911続柄名Master AS 未満02続柄 ON 扶養控除.扶養親族16未満02_続柄 = 未満02続柄.続柄番号 "
                    + "  LEFT JOIN TM911続柄名Master AS 未満03続柄 ON 扶養控除.扶養親族16未満03_続柄 = 未満03続柄.続柄番号 "
                    + "  LEFT JOIN TM911続柄名Master AS 未満04続柄 ON 扶養控除.扶養親族16未満04_続柄 = 未満04続柄.続柄番号 "
                    + "  LEFT JOIN TM912事業所名Master AS 事業所名 ON CASE WHEN LEFT(扶養控除.所属番号, 1) IN ('2', '3') THEN '1' ELSE LEFT(扶養控除.所属番号, 1) END = 事業所名.事業所番号 "
                    + " WHERE 扶養控除.社員番号 = @key "
                    + "   AND 扶養控除.対象年度 = " + year ;

                DataTable dt = new DataTable();
                DataSet dataSet = new DataSet();
                using (IDbCommand cmd = dm.CreateCommand(sql))
                {
                    DbHelper.AddDbParameter(cmd, "@key", DbType.String);
                    //パラメータ設定
                    var parameters = cmd.Parameters.Cast<IDbDataParameter>().ToArray<IDbDataParameter>();
                    parameters[0].Value = keyVal;
                    //クエリ実行
                    IDataAdapter da = dm.CreateSqlDataAdapter(cmd);
                    da.Fill(dataSet);
                }

                return dataSet.Tables[0];

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
        /// 目標基本を帳票にセット
        /// </summary>
        /// <param name="cr">クリスタルレポート</param>
        /// <param name="dr">目標管理基本DataRow</param>
        /// <returns>セット後のクリスタルレポート</returns>
        private void HuyouDeclareBaseSetting(ref HuyouDeclareReport cr, DataRow dr)
        {
            try
            {
                //開始
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");
                //datetimeに変換できたら和暦、だめだったら空をreturn
                Func<string, string, string> wareki = delegate (string val, string format)
                {
                    DateTime dt = new DateTime();
                    return DateTime.TryParse(val, out dt) ? DataConv.Date2Jcal(dt, format) : "";
                };

                //2020-04-07 iwai-tamura upd str ------
                Func<string, string, string> nengappi = delegate (string val, string format)
                {
                    DateTime dt = new DateTime();
                    return DateTime.TryParse(val, out dt) ? dt.ToString(format) : "";
                };
                //2020-04-07 iwai-tamura upd end ------     

                //年齢表示用
                Func<string, string> ageView = (val1) =>
                {
                    if (!string.IsNullOrEmpty(val1))
                    {
                        return val1 + "歳";
                    }
                    return null;
                };

                //チェックボックス表示用
                Func<string, string> checkView = (val1) =>
                {
                    if (!string.IsNullOrEmpty(val1))
                    {
                        return (val1 == "1") ? "☑" : "□";
                    }
                    return "□";
                };

                //cr.SetParameterValue("現在年月日", nengappi(dr["年度"].ToString() + "/12/31", "yyyy年M月d日現在"));
                //cr.SetParameterValue("タイトル年度", nengappi(dr["年度"].ToString() + "/01/01", "yyyy年度"));

                //cr.SetParameterValue("氏名", DataConv.IfNull(dr["氏名"].ToString(), ""));
                //cr.SetParameterValue("社員番号", DataConv.IfNull(dr["社員番号"].ToString(), ""));
                //cr.SetParameterValue("所属", DataConv.IfNull(dr["所属名"].ToString(), ""));
                //cr.SetParameterValue("役職名", DataConv.IfNull(dr["役職名"].ToString(), ""));
                //cr.SetParameterValue("資格", DataConv.IfNull(dr["資格名"].ToString(), ""));


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
        //2023-99-99 iwai-terao test-end ------


        //2023-99-99 iwai-terao test-str 保険料控除ボタン------
        /// <summary>
        /// 保険料控除申告書 出力
        /// </summary>
        /// <param name="selPrint"></param>
        /// <returns></returns>
        public string PrintHokenDeclare(string[] selPrint)
        {
            try
            {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");

                string workFolder = "";
                DateTime nowDate = DateTime.Now;      //現在日時を取得
                string userCode = ((EmployeeAdjustmentConnectionSystem.COM.Entity.Session.LoginUser)
                    (HttpContext.Current.Session["LoginUser"])).UserCode.ToString();

                //帳票作成フォルダを用意
                workFolder = TempDir + nowDate.ToString("yyyyMMdd") + Path.DirectorySeparatorChar +
                    userCode + nowDate.ToString("yyyyMMddHHmmss") + Path.DirectorySeparatorChar;

                //作成フォルダ内ファイル一覧を取得
                System.IO.Directory.CreateDirectory(workFolder);
                foreach (string file in System.IO.Directory.GetDirectories(TempDir, "*"))
                {
                    DateTime oldDirDate = new DateTime();
                    oldDirDate = System.IO.Directory.GetCreationTime(file); //作成日時を取得

                    //削除基準日より古いフォルダを削除
                    if (oldDirDate < nowDate.AddDays(-(R_P)))
                    {
                        DeleteDirectory(file);
                    }
                }

                //zip作成フォルダとzipファイル名を用意を用意
                string zipFolder = TempDir + nowDate.ToString("yyyyMMdd") + Path.DirectorySeparatorChar;
                string zipName = userCode + nowDate.ToString("yyyyMMddHHmmss") + ".zip";
                //return用path文字列を用意
                string zipReturnPath = nowDate.ToString("yyyyMMdd") + Path.DirectorySeparatorChar + zipName;

                //管理番号ごとにデータを取得
                foreach (string KeyValue in selPrint)
                {
                    //管理番号とTBL区分に分割
                    string[] arrayData = KeyValue.Split(',');
                    string year = arrayData[0];                  //対象年度
                    string key = arrayData[1];                  //社員番号

                    DataRow row = new DataTable().NewRow();
                    DataSet dataSet = new DataSet();
                    DataTable dt = new DataTable();
                    using (DbManager dm = new DbManager())
                    {
                        //基本データを取得
                        row = GetBasicDataRow_Hoken(key, dm, year);
                        //データを取得
                        dt = GetDataTable_Hoken(key, dm, year);
                    }

                    //帳票を出力
                    var HokenDeclareReport = new HokenDeclareReport();

                    try
                    {
                        //ファイル名作成
                        string fileName = row["対象年度"].ToString()
                            + "_" + row["所属番号"].ToString()
                            + "_" + row["社員番号"].ToString()
                            + "_" + nowDate.ToString("yyyyMMddHHmmss")
                            + ".pdf";

                        HokenDeclareReport.SetDataSource(dt);       //データをセット
                        HokenDeclareReport.Refresh();

                        //パラメーターを設定
                        HokenDeclareBaseSetting(ref HokenDeclareReport, row);

                        HokenDeclareReport.PrintOptions.PaperOrientation = CrystalDecisions.Shared.PaperOrientation.Landscape;

                        //pdf出力
                        HokenDeclareReport.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, workFolder + fileName);

                    }
                    catch (Exception ex)
                    {
                        //　TODO:エラー処理検討中
                        throw ex;
                        //throw;
                    }
                    finally
                    {
                        HokenDeclareReport.Dispose();
                    };
                }

                // TODO:pdf複数出力で、どれか１つにエラーがあってもダウンロードできる状況
                //圧縮
                var compress = new Compress();
                string zipFullPath = compress.CreateZipFile(zipName, zipFolder, workFolder);

                //return用zipファイルパスをセット
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
        /// 基本(ヘッダー)を取得
        /// </summary>
        /// <param name="keyVal">社員番号</param>
        /// <returns>目標管理基本情報のDataRow</returns>
        private DataRow GetBasicDataRow_Hoken(string keyVal, DbManager dm, string year)
        {
            try
            {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");

                string sql = ""; //クエリ生成

                sql = "SELECT * FROM TE110保険料控除申告書Data WHERE 社員番号 = @key ";
                sql += " AND 対象年度 = " + year ;

                DataTable dt = new DataTable();
                DataSet dataSet = new DataSet();
                using (IDbCommand cmd = dm.CreateCommand(sql))
                {
                    DbHelper.AddDbParameter(cmd, "@key", DbType.String);
                    //パラメータ設定
                    var parameters = cmd.Parameters.Cast<IDbDataParameter>().ToArray<IDbDataParameter>();
                    parameters[0].Value = keyVal;
                    //クエリ実行
                    IDataAdapter da = dm.CreateSqlDataAdapter(cmd);
                    da.Fill(dataSet);
                }

                //実行結果確認
                if (dataSet.Tables[0].Rows.Count == 0)
                {
                    //0件字はエラー?
                }
                return dataSet.Tables[0].Rows[0];
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
        /// 保険料控除データ
        /// </summary>
        /// <param name="keyVal"></param>
        /// <param name="pattern"></param>
        /// <returns></returns>
        private DataTable GetDataTable_Hoken(string keyVal, DbManager dm, string year)
        {
            try
            {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");

                string sql = "";
                sql = " SELECT "
                    + "     保険料控除.対象年度 "
                    + "     ,保険料控除.社員番号 "
                    + "     ,保険料控除.本人確定区分 "
                    + "     ,保険料控除.管理者確定区分 "
                    + "     ,保険料控除.所属番号 "
                    + "     ,保険料控除.氏名_姓 + ' ' + 保険料控除.氏名_名 AS 氏名 "
                    + "     ,保険料控除.Kana_姓 + ' ' + 保険料控除.Kana_名 AS Kana "
                    + "     ,保険料控除.住所01 "
                    + "     ,保険料控除.Sequence番号 "
                    + "     ,事業所名.税務署名 AS 税務署長 "
                    + "     ,事業所名.会社名 AS 給与支払者名称 "
                    + "     ,事業所名.所在地 AS 給与支払者所在地 "
                    + "     ,保険料控除.一般生命保険料01_保険会社等名称 "
                    + "     ,保険料控除.一般生命保険料01_保険等種類 "
                    + "     ,保険料控除.一般生命保険料01_期間 "
                    + "     ,保険料控除.一般生命保険料01_保険等契約者氏名_姓 + ' ' + 保険料控除.一般生命保険料01_保険等契約者氏名_名 AS 一般生命保険料01_保険等契約者氏名 "
                    + "     ,保険料控除.一般生命保険料01_保険金等受取人氏名_姓 + ' ' + 保険料控除.一般生命保険料01_保険金等受取人氏名_名 AS 一般生命保険料01_保険金等受取人氏名 "
                    + "     ,一般生命保険料01続柄名.続柄名称 AS 一般生命保険料01_保険金等受取人続柄名称 "
                    + "     ,ISNULL(保険料控除.一般生命保険料01_新旧,'0') AS  一般生命保険料01_新旧 "
                    + "     ,保険料控除.一般生命保険料01_支払金額 "
                    + "     ,保険料控除.一般生命保険料02_保険会社等名称 "
                    + "     ,保険料控除.一般生命保険料02_保険等種類 "
                    + "     ,保険料控除.一般生命保険料02_期間 "
                    + "     ,保険料控除.一般生命保険料02_保険等契約者氏名_姓 + ' ' + 保険料控除.一般生命保険料02_保険等契約者氏名_名 AS 一般生命保険料02_保険等契約者氏名 "
                    + "     ,保険料控除.一般生命保険料02_保険金等受取人氏名_姓 + ' ' + 保険料控除.一般生命保険料02_保険金等受取人氏名_名 AS 一般生命保険料02_保険金等受取人氏名 "
                    + "     ,一般生命保険料02続柄名.続柄名称 AS 一般生命保険料02_保険金等受取人続柄名称 "
                    + "     ,ISNULL(保険料控除.一般生命保険料02_新旧,'0') AS  一般生命保険料02_新旧 "
                    + "     ,保険料控除.一般生命保険料02_支払金額 "
                    + "     ,保険料控除.一般生命保険料03_保険会社等名称 "
                    + "     ,保険料控除.一般生命保険料03_保険等種類 "
                    + "     ,保険料控除.一般生命保険料03_期間 "
                    + "     ,保険料控除.一般生命保険料03_保険等契約者氏名_姓 + ' ' + 保険料控除.一般生命保険料03_保険等契約者氏名_名 AS 一般生命保険料03_保険等契約者氏名 "
                    + "     ,保険料控除.一般生命保険料03_保険金等受取人氏名_姓 + ' ' + 保険料控除.一般生命保険料03_保険金等受取人氏名_名 AS 一般生命保険料03_保険金等受取人氏名 "
                    + "     ,一般生命保険料03続柄名.続柄名称 AS 一般生命保険料03_保険金等受取人続柄名称 "
                    + "     ,ISNULL(保険料控除.一般生命保険料03_新旧,'0') AS  一般生命保険料03_新旧 "
                    + "     ,保険料控除.一般生命保険料03_支払金額 "
                    + "     ,保険料控除.一般生命保険料04_保険会社等名称 "
                    + "     ,保険料控除.一般生命保険料04_保険等種類 "
                    + "     ,保険料控除.一般生命保険料04_期間 "
                    + "     ,保険料控除.一般生命保険料04_保険等契約者氏名_姓 + ' ' + 保険料控除.一般生命保険料04_保険等契約者氏名_名 AS 一般生命保険料04_保険等契約者氏名 "
                    + "     ,保険料控除.一般生命保険料04_保険金等受取人氏名_姓 + ' ' + 保険料控除.一般生命保険料04_保険金等受取人氏名_名 AS 一般生命保険料04_保険金等受取人氏名 "
                    + "     ,一般生命保険料04続柄名.続柄名称 AS 一般生命保険料04_保険金等受取人続柄名称 "
                    + "     ,ISNULL(保険料控除.一般生命保険料04_新旧,'0') AS  一般生命保険料04_新旧 "
                    + "     ,保険料控除.一般生命保険料04_支払金額 "
                    + "     ,保険料控除.一般生命保険料新保険料合計 "
                    + "     ,保険料控除.一般生命保険料新保険料表計算 "
                    + "     ,保険料控除.一般生命保険料旧保険料合計 "
                    + "     ,保険料控除.一般生命保険料旧保険料表計算 "
                    + "     ,保険料控除.一般生命保険料表合計 "
                    + "     ,保険料控除.一般生命保険料比較 "
                    + "     ,保険料控除.介護医療保険料01_会社等名称 "
                    + "     ,保険料控除.介護医療保険料01_保険等種類 "
                    + "     ,保険料控除.介護医療保険料01_期間 "
                    + "     ,保険料控除.介護医療保険料01_保険等契約者氏名_姓 + ' ' + 保険料控除.介護医療保険料01_保険等契約者氏名_名 AS 介護医療保険料01_保険等契約者氏名 "
                    + "     ,保険料控除.介護医療保険料01_保険金等受取人氏名_姓 + ' ' + 保険料控除.介護医療保険料01_保険金等受取人氏名_名 AS 介護医療保険料01_保険金等受取人氏名 "
                    + "     ,介護医療保険料01続柄名.続柄名称 AS 介護医療保険料01_保険金等受取人続柄名称 "
                    + "     ,保険料控除.介護医療保険料01_支払金額 "
                    + "     ,保険料控除.介護医療保険料02_会社等名称 "
                    + "     ,保険料控除.介護医療保険料02_保険等種類 "
                    + "     ,保険料控除.介護医療保険料02_期間 "
                    + "     ,保険料控除.介護医療保険料02_保険等契約者氏名_姓 + ' ' + 保険料控除.介護医療保険料02_保険等契約者氏名_名 AS 介護医療保険料02_保険等契約者氏名 "
                    + "     ,保険料控除.介護医療保険料02_保険金等受取人氏名_姓 + ' ' + 保険料控除.介護医療保険料02_保険金等受取人氏名_名 AS 介護医療保険料02_保険金等受取人氏名 "
                    + "     ,介護医療保険料02続柄名.続柄名称 AS 介護医療保険料02_保険金等受取人続柄名称 "
                    + "     ,保険料控除.介護医療保険料02_支払金額 "
                    + "     ,保険料控除.介護医療保険料合計 "
                    + "     ,保険料控除.介護医療保険料表計算 "
                    + "     ,保険料控除.個人年金保険料01_会社等名称 "
                    + "     ,保険料控除.個人年金保険料01_保険等種類 "
                    + "     ,保険料控除.個人年金保険料01_期間 "
                    + "     ,保険料控除.個人年金保険料01_保険等契約者氏名_姓 + ' ' + 保険料控除.個人年金保険料01_保険等契約者氏名_名 AS 個人年金保険料01_保険等契約者氏名 "
                    + "     ,保険料控除.個人年金保険料01_保険金等受取人氏名_姓 + ' ' + 保険料控除.個人年金保険料01_保険金等受取人氏名_名 AS 個人年金保険料01_保険金等受取人氏名 "
                    + "     ,保険料控除.個人年金保険料01_支払開始日 "
                    + "     ,個人年金保険料01続柄名.続柄名称 AS 個人年金保険料01_保険金等受取人続柄名称 "
                    + "     ,ISNULL(保険料控除.個人年金保険料01_新旧,'0') AS  個人年金保険料01_新旧 "
                    + "     ,保険料控除.個人年金保険料01_支払金額 "
                    + "     ,保険料控除.個人年金保険料02_会社等名称 "
                    + "     ,保険料控除.個人年金保険料02_保険等種類 "
                    + "     ,保険料控除.個人年金保険料02_期間 "
                    + "     ,保険料控除.個人年金保険料02_保険等契約者氏名_姓 + ' ' + 保険料控除.個人年金保険料02_保険等契約者氏名_名 AS 個人年金保険料02_保険等契約者氏名 "
                    + "     ,保険料控除.個人年金保険料02_保険金等受取人氏名_姓 + ' ' + 保険料控除.個人年金保険料02_保険金等受取人氏名_名 AS 個人年金保険料02_保険金等受取人氏名 "
                    + "     ,保険料控除.個人年金保険料02_支払開始日 "
                    + "     ,個人年金保険料02続柄名.続柄名称 AS 個人年金保険料02_保険金等受取人続柄名称 "
                    + "     ,ISNULL(保険料控除.個人年金保険料02_新旧,'0') AS  個人年金保険料02_新旧 "
                    + "     ,保険料控除.個人年金保険料02_支払金額 "
                    + "     ,保険料控除.個人年金保険料03_会社等名称 "
                    + "     ,保険料控除.個人年金保険料03_保険等種類 "
                    + "     ,保険料控除.個人年金保険料03_期間 "
                    + "     ,保険料控除.個人年金保険料03_保険等契約者氏名_姓 + ' ' + 保険料控除.個人年金保険料03_保険等契約者氏名_名 AS 個人年金保険料03_保険等契約者氏名 "
                    + "     ,保険料控除.個人年金保険料03_保険金等受取人氏名_姓 + ' ' + 保険料控除.個人年金保険料03_保険金等受取人氏名_名 AS 個人年金保険料03_保険金等受取人氏名 "
                    + "     ,保険料控除.個人年金保険料03_支払開始日 "
                    + "     ,個人年金保険料03続柄名.続柄名称 AS 個人年金保険料03_保険金等受取人続柄名称 "
                    + "     ,ISNULL(保険料控除.個人年金保険料03_新旧,'0') AS  個人年金保険料03_新旧 "
                    + "     ,保険料控除.個人年金保険料03_支払金額 "
                    + "     ,保険料控除.個人年金保険料新保険料合計 "
                    + "     ,保険料控除.個人年金保険料新保険料表計算 "
                    + "     ,保険料控除.個人年金保険料旧保険料合計 "
                    + "     ,保険料控除.個人年金保険料旧保険料表計算 "
                    + "     ,保険料控除.個人年金保険料表合計 "
                    + "     ,保険料控除.個人年金保険料比較 "
                    + "     ,保険料控除.生命保険料控除額計 "
                    + "     ,保険料控除.地震保険料控除01_会社等名称 "
                    + "     ,保険料控除.地震保険料控除01_保険等種類 "
                    + "     ,保険料控除.地震保険料控除01_期間 "
                    + "     ,保険料控除.地震保険料控除01_保険等契約者氏名_姓 + ' ' + 保険料控除.地震保険料控除01_保険等契約者氏名_名 AS 地震保険料控除01_保険等契約者氏名 "
                    + "     ,保険料控除.地震保険料控除01_保険等対象氏名_姓 + ' ' + 保険料控除.地震保険料控除01_保険等対象氏名_名 AS 地震保険料控除01_保険等対象氏名 "
                    + "     ,地震保険料控除01続柄名.続柄名称 AS 地震保険料控除01_保険等対象続柄名称 "
                    + "     ,ISNULL(保険料控除.地震保険料控除01_地震旧長期,'0') AS  地震保険料控除01_地震旧長期 "
                    + "     ,保険料控除.地震保険料控除01_支払保険料 "
                    + "     ,保険料控除.地震保険料控除02_会社等名称 "
                    + "     ,保険料控除.地震保険料控除02_保険等種類 "
                    + "     ,保険料控除.地震保険料控除02_期間 "
                    + "     ,保険料控除.地震保険料控除02_保険等契約者氏名_姓 + ' ' + 保険料控除.地震保険料控除02_保険等契約者氏名_名 AS 地震保険料控除02_保険等契約者氏名 "
                    + "     ,保険料控除.地震保険料控除02_保険等対象氏名_姓 + ' ' + 保険料控除.地震保険料控除02_保険等対象氏名_名 AS 地震保険料控除02_保険等対象氏名 "
                    + "     ,地震保険料控除02続柄名.続柄名称 AS 地震保険料控除02_保険等対象続柄名称 "
                    + "     ,ISNULL(保険料控除.地震保険料控除02_地震旧長期,'0') AS  地震保険料控除02_地震旧長期 "
                    + "     ,保険料控除.地震保険料控除02_支払保険料 "
                    + "     ,保険料控除.地震保険料控除地震保険料合計 "
                    + "     ,保険料控除.地震保険料控除旧長期損害保険料合計 "
                    + "     ,保険料控除.地震保険料控除額金額01 "
                    + "     ,保険料控除.地震保険料控除額金額02 "
                    + "     ,保険料控除.地震保険料控除額金額合計 "
                    + "     ,保険料控除.社会保険料控除01_社会保険種類 "
                    + "     ,保険料控除.社会保険料控除01_支払先名称 "
                    + "     ,保険料控除.社会保険料控除01_負担者氏名_姓 + ' ' + 保険料控除.社会保険料控除01_負担者氏名_名 AS 社会保険料控除01_負担者氏名 "
                    + "     ,社会保険料控除01続柄名.続柄名称 AS 社会保険料控除01_負担者続柄名称 "
                    + "     ,保険料控除.社会保険料控除01_支払保険料 "
                    + "     ,保険料控除.社会保険料控除02_社会保険種類 "
                    + "     ,保険料控除.社会保険料控除02_支払先名称 "
                    + "     ,保険料控除.社会保険料控除02_負担者氏名_姓 + ' ' + 保険料控除.社会保険料控除02_負担者氏名_名 AS 社会保険料控除02_負担者氏名 "
                    + "     ,社会保険料控除02続柄名.続柄名称 AS 社会保険料控除02_負担者続柄名称 "
                    + "     ,保険料控除.社会保険料控除02_支払保険料 "
                    + "     ,保険料控除.社会保険料控除合計 "
                    + "     ,保険料控除.共済契約掛金 "
                    + "     ,保険料控除.企業型年金加入者掛金 "
                    + "     ,保険料控除.個人型年金加入者掛金 "
                    + "     ,保険料控除.心身障害者扶養共済制度契約掛金 "
                    + "     ,保険料控除.小規模企業共済等掛金控除合計 "
                    //2023-11-06 iwai-terao upd str ------
                    + "     ,保険料控除.一般生命保険料05_保険会社等名称 "
                    + "     ,保険料控除.一般生命保険料05_保険等種類 "
                    + "     ,保険料控除.一般生命保険料05_期間 "
                    + "     ,保険料控除.一般生命保険料05_保険等契約者氏名_姓 + ' ' + 保険料控除.一般生命保険料05_保険等契約者氏名_名 AS 一般生命保険料05_保険等契約者氏名 "
                    + "     ,保険料控除.一般生命保険料05_保険金等受取人氏名_姓 + ' ' + 保険料控除.一般生命保険料05_保険金等受取人氏名_名 AS 一般生命保険料05_保険金等受取人氏名 "
                    + "     ,一般生命保険料05続柄名.続柄名称 AS 一般生命保険料05_保険金等受取人続柄名称 "
                    + "     ,ISNULL(保険料控除.一般生命保険料05_新旧,'0') AS  一般生命保険料05_新旧 "
                    + "     ,保険料控除.一般生命保険料05_支払金額 "
                    + "     ,保険料控除.一般生命保険料06_保険会社等名称 "
                    + "     ,保険料控除.一般生命保険料06_保険等種類 "
                    + "     ,保険料控除.一般生命保険料06_期間 "
                    + "     ,保険料控除.一般生命保険料06_保険等契約者氏名_姓 + ' ' + 保険料控除.一般生命保険料06_保険等契約者氏名_名 AS 一般生命保険料06_保険等契約者氏名 "
                    + "     ,保険料控除.一般生命保険料06_保険金等受取人氏名_姓 + ' ' + 保険料控除.一般生命保険料06_保険金等受取人氏名_名 AS 一般生命保険料06_保険金等受取人氏名 "
                    + "     ,一般生命保険料06続柄名.続柄名称 AS 一般生命保険料06_保険金等受取人続柄名称 "
                    + "     ,ISNULL(保険料控除.一般生命保険料06_新旧,'0') AS  一般生命保険料06_新旧 "
                    + "     ,保険料控除.一般生命保険料06_支払金額 "
                    + "     ,保険料控除.一般生命保険料07_保険会社等名称 "
                    + "     ,保険料控除.一般生命保険料07_保険等種類 "
                    + "     ,保険料控除.一般生命保険料07_期間 "
                    + "     ,保険料控除.一般生命保険料07_保険等契約者氏名_姓 + ' ' + 保険料控除.一般生命保険料07_保険等契約者氏名_名 AS 一般生命保険料07_保険等契約者氏名 "
                    + "     ,保険料控除.一般生命保険料07_保険金等受取人氏名_姓 + ' ' + 保険料控除.一般生命保険料07_保険金等受取人氏名_名 AS 一般生命保険料07_保険金等受取人氏名 "
                    + "     ,一般生命保険料07続柄名.続柄名称 AS 一般生命保険料07_保険金等受取人続柄名称 "
                    + "     ,ISNULL(保険料控除.一般生命保険料07_新旧,'0') AS  一般生命保険料07_新旧 "
                    + "     ,保険料控除.一般生命保険料07_支払金額 "
                    + "     ,保険料控除.一般生命保険料08_保険会社等名称 "
                    + "     ,保険料控除.一般生命保険料08_保険等種類 "
                    + "     ,保険料控除.一般生命保険料08_期間 "
                    + "     ,保険料控除.一般生命保険料08_保険等契約者氏名_姓 + ' ' + 保険料控除.一般生命保険料08_保険等契約者氏名_名 AS 一般生命保険料08_保険等契約者氏名 "
                    + "     ,保険料控除.一般生命保険料08_保険金等受取人氏名_姓 + ' ' + 保険料控除.一般生命保険料08_保険金等受取人氏名_名 AS 一般生命保険料08_保険金等受取人氏名 "
                    + "     ,一般生命保険料08続柄名.続柄名称 AS 一般生命保険料08_保険金等受取人続柄名称 "
                    + "     ,ISNULL(保険料控除.一般生命保険料08_新旧,'0') AS  一般生命保険料08_新旧 "
                    + "     ,保険料控除.一般生命保険料08_支払金額 "
                    + "     ,保険料控除.介護医療保険料03_会社等名称 "
                    + "     ,保険料控除.介護医療保険料03_保険等種類 "
                    + "     ,保険料控除.介護医療保険料03_期間 "
                    + "     ,保険料控除.介護医療保険料03_保険等契約者氏名_姓 + ' ' + 保険料控除.介護医療保険料03_保険等契約者氏名_名 AS 介護医療保険料03_保険等契約者氏名 "
                    + "     ,保険料控除.介護医療保険料03_保険金等受取人氏名_姓 + ' ' + 保険料控除.介護医療保険料03_保険金等受取人氏名_名 AS 介護医療保険料03_保険金等受取人氏名 "
                    + "     ,介護医療保険料03続柄名.続柄名称 AS 介護医療保険料03_保険金等受取人続柄名称 "
                    + "     ,保険料控除.介護医療保険料03_支払金額 "
                    + "     ,保険料控除.介護医療保険料04_会社等名称 "
                    + "     ,保険料控除.介護医療保険料04_保険等種類 "
                    + "     ,保険料控除.介護医療保険料04_期間 "
                    + "     ,保険料控除.介護医療保険料04_保険等契約者氏名_姓 + ' ' + 保険料控除.介護医療保険料04_保険等契約者氏名_名 AS 介護医療保険料04_保険等契約者氏名 "
                    + "     ,保険料控除.介護医療保険料04_保険金等受取人氏名_姓 + ' ' + 保険料控除.介護医療保険料04_保険金等受取人氏名_名 AS 介護医療保険料04_保険金等受取人氏名 "
                    + "     ,介護医療保険料04続柄名.続柄名称 AS 介護医療保険料04_保険金等受取人続柄名称 "
                    + "     ,保険料控除.介護医療保険料04_支払金額 "
                    + "     ,保険料控除.介護医療保険料05_会社等名称 "
                    + "     ,保険料控除.介護医療保険料05_保険等種類 "
                    + "     ,保険料控除.介護医療保険料05_期間 "
                    + "     ,保険料控除.介護医療保険料05_保険等契約者氏名_姓 + ' ' + 保険料控除.介護医療保険料05_保険等契約者氏名_名 AS 介護医療保険料05_保険等契約者氏名 "
                    + "     ,保険料控除.介護医療保険料05_保険金等受取人氏名_姓 + ' ' + 保険料控除.介護医療保険料05_保険金等受取人氏名_名 AS 介護医療保険料05_保険金等受取人氏名 "
                    + "     ,介護医療保険料05続柄名.続柄名称 AS 介護医療保険料05_保険金等受取人続柄名称 "
                    + "     ,保険料控除.介護医療保険料05_支払金額 "
                    + "     ,保険料控除.介護医療保険料06_会社等名称 "
                    + "     ,保険料控除.介護医療保険料06_保険等種類 "
                    + "     ,保険料控除.介護医療保険料06_期間 "
                    + "     ,保険料控除.介護医療保険料06_保険等契約者氏名_姓 + ' ' + 保険料控除.介護医療保険料06_保険等契約者氏名_名 AS 介護医療保険料06_保険等契約者氏名 "
                    + "     ,保険料控除.介護医療保険料06_保険金等受取人氏名_姓 + ' ' + 保険料控除.介護医療保険料06_保険金等受取人氏名_名 AS 介護医療保険料06_保険金等受取人氏名 "
                    + "     ,介護医療保険料06続柄名.続柄名称 AS 介護医療保険料06_保険金等受取人続柄名称 "
                    + "     ,保険料控除.介護医療保険料06_支払金額 "
                    + "     ,保険料控除.個人年金保険料04_会社等名称 "
                    + "     ,保険料控除.個人年金保険料04_保険等種類 "
                    + "     ,保険料控除.個人年金保険料04_期間 "
                    + "     ,保険料控除.個人年金保険料04_保険等契約者氏名_姓 + ' ' + 保険料控除.個人年金保険料04_保険等契約者氏名_名 AS 個人年金保険料04_保険等契約者氏名 "
                    + "     ,保険料控除.個人年金保険料04_保険金等受取人氏名_姓 + ' ' + 保険料控除.個人年金保険料04_保険金等受取人氏名_名 AS 個人年金保険料04_保険金等受取人氏名 "
                    + "     ,保険料控除.個人年金保険料04_支払開始日 "
                    + "     ,個人年金保険料04続柄名.続柄名称 AS 個人年金保険料04_保険金等受取人続柄名称 "
                    + "     ,ISNULL(保険料控除.個人年金保険料04_新旧,'0') AS  個人年金保険料04_新旧 "
                    + "     ,保険料控除.個人年金保険料04_支払金額 "
                    + "     ,保険料控除.地震保険料控除03_会社等名称 "
                    + "     ,保険料控除.地震保険料控除03_保険等種類 "
                    + "     ,保険料控除.地震保険料控除03_期間 "
                    + "     ,保険料控除.地震保険料控除03_保険等契約者氏名_姓 + ' ' + 保険料控除.地震保険料控除03_保険等契約者氏名_名 AS 地震保険料控除03_保険等契約者氏名 "
                    + "     ,保険料控除.地震保険料控除03_保険等対象氏名_姓 + ' ' + 保険料控除.地震保険料控除03_保険等対象氏名_名 AS 地震保険料控除03_保険等対象氏名 "
                    + "     ,地震保険料控除03続柄名.続柄名称 AS 地震保険料控除03_保険等対象続柄名称 "
                    + "     ,ISNULL(保険料控除.地震保険料控除03_地震旧長期,'0') AS  地震保険料控除03_地震旧長期 "
                    + "     ,保険料控除.地震保険料控除03_支払保険料 "
                    + "     ,保険料控除.地震保険料控除04_会社等名称 "
                    + "     ,保険料控除.地震保険料控除04_保険等種類 "
                    + "     ,保険料控除.地震保険料控除04_期間 "
                    + "     ,保険料控除.地震保険料控除04_保険等契約者氏名_姓 + ' ' + 保険料控除.地震保険料控除04_保険等契約者氏名_名 AS 地震保険料控除04_保険等契約者氏名 "
                    + "     ,保険料控除.地震保険料控除04_保険等対象氏名_姓 + ' ' + 保険料控除.地震保険料控除04_保険等対象氏名_名 AS 地震保険料控除04_保険等対象氏名 "
                    + "     ,地震保険料控除04続柄名.続柄名称 AS 地震保険料控除04_保険等対象続柄名称 "
                    + "     ,ISNULL(保険料控除.地震保険料控除04_地震旧長期,'0') AS  地震保険料控除04_地震旧長期 "
                    + "     ,保険料控除.地震保険料控除04_支払保険料 "
                    + "     ,保険料控除.社会保険料控除03_社会保険種類 "
                    + "     ,保険料控除.社会保険料控除03_支払先名称 "
                    + "     ,保険料控除.社会保険料控除03_負担者氏名_姓 + ' ' + 保険料控除.社会保険料控除03_負担者氏名_名 AS 社会保険料控除03_負担者氏名 "
                    + "     ,社会保険料控除03続柄名.続柄名称 AS 社会保険料控除03_負担者続柄名称 "
                    + "     ,保険料控除.社会保険料控除03_支払保険料 "
                    //2023-11-06 iwai-terao upd end ------

                    //2023-11-16 iwai-terao upd str ------
                    + "     ,ISNULL(一般生命保険料01_HostData判定,'0') AS  一般生命保険料01_HostData判定 "
                    + "     ,ISNULL(一般生命保険料02_HostData判定,'0') AS  一般生命保険料02_HostData判定 "
                    + "     ,ISNULL(一般生命保険料03_HostData判定,'0') AS  一般生命保険料03_HostData判定 "
                    + "     ,ISNULL(一般生命保険料04_HostData判定,'0') AS  一般生命保険料04_HostData判定 "
                    + "     ,ISNULL(一般生命保険料05_HostData判定,'0') AS  一般生命保険料05_HostData判定 "
                    + "     ,ISNULL(一般生命保険料06_HostData判定,'0') AS  一般生命保険料06_HostData判定 "
                    + "     ,ISNULL(一般生命保険料07_HostData判定,'0') AS  一般生命保険料07_HostData判定 "
                    + "     ,ISNULL(一般生命保険料08_HostData判定,'0') AS  一般生命保険料08_HostData判定 "
                    + "     ,ISNULL(介護医療保険料01_HostData判定,'0') AS  介護医療保険料01_HostData判定 "
                    + "     ,ISNULL(介護医療保険料02_HostData判定,'0') AS  介護医療保険料02_HostData判定 "
                    + "     ,ISNULL(介護医療保険料03_HostData判定,'0') AS  介護医療保険料03_HostData判定 "
                    + "     ,ISNULL(介護医療保険料04_HostData判定,'0') AS  介護医療保険料04_HostData判定 "
                    + "     ,ISNULL(介護医療保険料05_HostData判定,'0') AS  介護医療保険料05_HostData判定 "
                    + "     ,ISNULL(介護医療保険料06_HostData判定,'0') AS  介護医療保険料06_HostData判定 "
                    + "     ,ISNULL(個人年金保険料01_HostData判定,'0') AS  個人年金保険料01_HostData判定 "
                    + "     ,ISNULL(個人年金保険料02_HostData判定,'0') AS  個人年金保険料02_HostData判定 "
                    + "     ,ISNULL(個人年金保険料03_HostData判定,'0') AS  個人年金保険料03_HostData判定 "
                    + "     ,ISNULL(個人年金保険料04_HostData判定,'0') AS  個人年金保険料04_HostData判定 "
                    + "     ,ISNULL(地震保険料控除01_HostData判定,'0') AS  地震保険料控除01_HostData判定 "
                    + "     ,ISNULL(地震保険料控除02_HostData判定,'0') AS  地震保険料控除02_HostData判定 "
                    + "     ,ISNULL(地震保険料控除03_HostData判定,'0') AS  地震保険料控除03_HostData判定 "
                    + "     ,ISNULL(地震保険料控除04_HostData判定,'0') AS  地震保険料控除04_HostData判定 "
                    + "     ,'3' AS 法人個人番号01 "
                    + "     ,'0' AS 法人個人番号02 "
                    + "     ,'1' AS 法人個人番号03 "
                    + "     ,'0' AS 法人個人番号04 "
                    + "     ,'0' AS 法人個人番号05 "
                    + "     ,'0' AS 法人個人番号06 "
                    + "     ,'1' AS 法人個人番号07 "
                    + "     ,'0' AS 法人個人番号08 "
                    + "     ,'3' AS 法人個人番号09 "
                    + "     ,'3' AS 法人個人番号10 "
                    + "     ,'3' AS 法人個人番号11 "
                    + "     ,'7' AS 法人個人番号12 "
                    + "     ,'5' AS 法人個人番号13 "
                    //2023-11-16 iwai-terao upd end ------
                    + " FROM TE110保険料控除申告書Data AS 保険料控除 "
                    + "  LEFT JOIN TM912事業所名Master AS 事業所名 ON CASE WHEN LEFT(保険料控除.所属番号, 1) IN ('2', '3') THEN '1' ELSE LEFT(保険料控除.所属番号, 1) END = 事業所名.事業所番号 "
                    + "  LEFT JOIN TM911続柄名Master AS 一般生命保険料01続柄名 ON 保険料控除.一般生命保険料01_保険金等受取人続柄 = 一般生命保険料01続柄名.続柄番号 "
                    + "  LEFT JOIN TM911続柄名Master AS 一般生命保険料02続柄名 ON 保険料控除.一般生命保険料02_保険金等受取人続柄 = 一般生命保険料02続柄名.続柄番号 "
                    + "  LEFT JOIN TM911続柄名Master AS 一般生命保険料03続柄名 ON 保険料控除.一般生命保険料03_保険金等受取人続柄 = 一般生命保険料03続柄名.続柄番号 "
                    + "  LEFT JOIN TM911続柄名Master AS 一般生命保険料04続柄名 ON 保険料控除.一般生命保険料04_保険金等受取人続柄 = 一般生命保険料04続柄名.続柄番号 "
                    + "  LEFT JOIN TM911続柄名Master AS 介護医療保険料01続柄名 ON 保険料控除.介護医療保険料01_保険金等受取人続柄 = 介護医療保険料01続柄名.続柄番号 "
                    + "  LEFT JOIN TM911続柄名Master AS 介護医療保険料02続柄名 ON 保険料控除.介護医療保険料02_保険金等受取人続柄 = 介護医療保険料02続柄名.続柄番号 "
                    + "  LEFT JOIN TM911続柄名Master AS 個人年金保険料01続柄名 ON 保険料控除.個人年金保険料01_保険金等受取人続柄 = 個人年金保険料01続柄名.続柄番号 "
                    + "  LEFT JOIN TM911続柄名Master AS 個人年金保険料02続柄名 ON 保険料控除.個人年金保険料02_保険金等受取人続柄 = 個人年金保険料02続柄名.続柄番号 "
                    + "  LEFT JOIN TM911続柄名Master AS 個人年金保険料03続柄名 ON 保険料控除.個人年金保険料03_保険金等受取人続柄 = 個人年金保険料03続柄名.続柄番号 "
                    + "  LEFT JOIN TM911続柄名Master AS 地震保険料控除01続柄名 ON 保険料控除.地震保険料控除01_保険等対象続柄 = 地震保険料控除01続柄名.続柄番号 "
                    + "  LEFT JOIN TM911続柄名Master AS 地震保険料控除02続柄名 ON 保険料控除.地震保険料控除02_保険等対象続柄 = 地震保険料控除02続柄名.続柄番号 "
                    + "  LEFT JOIN TM911続柄名Master AS 社会保険料控除01続柄名 ON 保険料控除.社会保険料控除01_負担者続柄 = 社会保険料控除01続柄名.続柄番号 "
                    + "  LEFT JOIN TM911続柄名Master AS 社会保険料控除02続柄名 ON 保険料控除.社会保険料控除02_負担者続柄 = 社会保険料控除02続柄名.続柄番号 "
                    //2023-11-06 iwai-terao upd str ------
                    + "  LEFT JOIN TM911続柄名Master AS 一般生命保険料05続柄名 ON 保険料控除.一般生命保険料05_保険金等受取人続柄 = 一般生命保険料05続柄名.続柄番号 "
                    + "  LEFT JOIN TM911続柄名Master AS 一般生命保険料06続柄名 ON 保険料控除.一般生命保険料06_保険金等受取人続柄 = 一般生命保険料06続柄名.続柄番号 "
                    + "  LEFT JOIN TM911続柄名Master AS 一般生命保険料07続柄名 ON 保険料控除.一般生命保険料07_保険金等受取人続柄 = 一般生命保険料07続柄名.続柄番号 "
                    + "  LEFT JOIN TM911続柄名Master AS 一般生命保険料08続柄名 ON 保険料控除.一般生命保険料08_保険金等受取人続柄 = 一般生命保険料08続柄名.続柄番号 "
                    + "  LEFT JOIN TM911続柄名Master AS 介護医療保険料03続柄名 ON 保険料控除.介護医療保険料03_保険金等受取人続柄 = 介護医療保険料03続柄名.続柄番号 "
                    + "  LEFT JOIN TM911続柄名Master AS 介護医療保険料04続柄名 ON 保険料控除.介護医療保険料04_保険金等受取人続柄 = 介護医療保険料04続柄名.続柄番号 "
                    + "  LEFT JOIN TM911続柄名Master AS 介護医療保険料05続柄名 ON 保険料控除.介護医療保険料05_保険金等受取人続柄 = 介護医療保険料05続柄名.続柄番号 "
                    + "  LEFT JOIN TM911続柄名Master AS 介護医療保険料06続柄名 ON 保険料控除.介護医療保険料06_保険金等受取人続柄 = 介護医療保険料06続柄名.続柄番号 "
                    + "  LEFT JOIN TM911続柄名Master AS 個人年金保険料04続柄名 ON 保険料控除.個人年金保険料04_保険金等受取人続柄 = 個人年金保険料04続柄名.続柄番号 "
                    + "  LEFT JOIN TM911続柄名Master AS 地震保険料控除03続柄名 ON 保険料控除.地震保険料控除03_保険等対象続柄 = 地震保険料控除03続柄名.続柄番号 "
                    + "  LEFT JOIN TM911続柄名Master AS 地震保険料控除04続柄名 ON 保険料控除.地震保険料控除04_保険等対象続柄 = 地震保険料控除04続柄名.続柄番号 "
                    + "  LEFT JOIN TM911続柄名Master AS 社会保険料控除03続柄名 ON 保険料控除.社会保険料控除03_負担者続柄 = 社会保険料控除03続柄名.続柄番号 "
                    //2023-11-06 iwai-terao upd end ------
                    + " WHERE 保険料控除.社員番号 = @key "
                    + "   AND 保険料控除.対象年度 = " + year ;

                DataTable dt = new DataTable();
                DataSet dataSet = new DataSet();
                using (IDbCommand cmd = dm.CreateCommand(sql))
                {
                    DbHelper.AddDbParameter(cmd, "@key", DbType.String);
                    //パラメータ設定
                    var parameters = cmd.Parameters.Cast<IDbDataParameter>().ToArray<IDbDataParameter>();
                    parameters[0].Value = keyVal;
                    //クエリ実行
                    IDataAdapter da = dm.CreateSqlDataAdapter(cmd);
                    da.Fill(dataSet);
                }

                return dataSet.Tables[0];

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
        /// 目標基本を帳票にセット
        /// </summary>
        /// <param name="cr">クリスタルレポート</param>
        /// <param name="dr">目標管理基本DataRow</param>
        /// <returns>セット後のクリスタルレポート</returns>
        private void HokenDeclareBaseSetting(ref HokenDeclareReport cr, DataRow dr)
        {
            try
            {
                //開始
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");
                //datetimeに変換できたら和暦、だめだったら空をreturn
                Func<string, string, string> wareki = delegate (string val, string format)
                {
                    DateTime dt = new DateTime();
                    return DateTime.TryParse(val, out dt) ? DataConv.Date2Jcal(dt, format) : "";
                };

                //2020-04-07 iwai-tamura upd str ------
                Func<string, string, string> nengappi = delegate (string val, string format)
                {
                    DateTime dt = new DateTime();
                    return DateTime.TryParse(val, out dt) ? dt.ToString(format) : "";
                };
                //2020-04-07 iwai-tamura upd end ------     

                //年齢表示用
                Func<string, string> ageView = (val1) =>
                {
                    if (!string.IsNullOrEmpty(val1))
                    {
                        return val1 + "歳";
                    }
                    return null;
                };

                //チェックボックス表示用
                Func<string, string> checkView = (val1) =>
                {
                    if (!string.IsNullOrEmpty(val1))
                    {
                        return (val1 == "1") ? "☑" : "□";
                    }
                    return "□";
                };

                //cr.SetParameterValue("現在年月日", nengappi(dr["年度"].ToString() + "/12/31", "yyyy年M月d日現在"));
                //cr.SetParameterValue("タイトル年度", nengappi(dr["年度"].ToString() + "/01/01", "yyyy年度"));

                //cr.SetParameterValue("氏名", DataConv.IfNull(dr["氏名"].ToString(), ""));
                //cr.SetParameterValue("社員番号", DataConv.IfNull(dr["社員番号"].ToString(), ""));
                //cr.SetParameterValue("所属", DataConv.IfNull(dr["所属名"].ToString(), ""));
                //cr.SetParameterValue("役職名", DataConv.IfNull(dr["役職名"].ToString(), ""));
                //cr.SetParameterValue("資格", DataConv.IfNull(dr["資格名"].ToString(), ""));


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
        //2023-99-99 iwai-terao test-end ------



        ///////////// <summary>
        ///////////// メインの処理
        ///////////// </summary>
        ///////////// <param name="selPrint"></param>
        ///////////// <returns></returns>
        //////////public string PrintBatchHuyou(string[] selPrint) {
        //////////    try {
        //////////        //開始
        //////////        nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");

        //////////        string workFolder = "";
        //////////        DateTime nowDate = DateTime.Now;      //現在日時を取得
        //////////        string userCode = ((EmployeeAdjustmentConnectionSystem.COM.Entity.Session.LoginUser)
        //////////            (HttpContext.Current.Session["LoginUser"])).UserCode.ToString();

        //////////        //帳票作成フォルダを用意
        //////////        workFolder = TempDir + nowDate.ToString("yyyyMMdd") + Path.DirectorySeparatorChar +
        //////////            userCode + nowDate.ToString("yyyyMMddHHmmss") + Path.DirectorySeparatorChar;

        //////////        //作成フォルダ内ファイル一覧を取得
        //////////        System.IO.Directory.CreateDirectory(workFolder);
        //////////        foreach(string file in System.IO.Directory.GetDirectories(TempDir, "*")) {
        //////////            DateTime oldDirDate = new DateTime();
        //////////            oldDirDate = System.IO.Directory.GetCreationTime(file); //作成日時を取得

        //////////            //削除基準日より古いフォルダを削除
        //////////            if (oldDirDate < nowDate.AddDays(-(R_P)))
        //////////            {
        //////////                //2017-03-31 sbc-sagara upd str 一括Excel出力で作成した読み取り専用属性を付けたファイルを削除するため
        //////////                //System.IO.Directory.Delete(file, true);
        //////////                DeleteDirectory(file);
        //////////                //2017-03-31 sbc-sagara upd end 一括Excel出力で作成した読み取り専用属性を付けたファイルを削除するため
        //////////            }
        //////////        }

        //////////        //zip作成フォルダとzipファイル名を用意を用意
        //////////        string zipFolder = TempDir + nowDate.ToString("yyyyMMdd") + Path.DirectorySeparatorChar;
        //////////        string zipName = userCode + nowDate.ToString("yyyyMMddHHmmss") + ".zip";
        //////////        //return用path文字列を用意
        //////////        string zipReturnPath = nowDate.ToString("yyyyMMdd") + Path.DirectorySeparatorChar + zipName;

        //////////        //目標管理番号ごとにデータを取得
        //////////        foreach(string KeyValue in selPrint) {
        //////////            //管理番号とTBL区分に分割
        //////////            string[] arrayData = KeyValue.Split(',');
        //////////            string key = arrayData[0];                  //管理番号
        //////////            string strSelfDeclareCode = arrayData[1];   //自己申告書種別Code
        //////////            string DutyNo = arrayData[2];               //職掌番号
        //////////            string tblType = "D1";

        //////////            if (arrayData[3] != "1") {
        //////////                continue;   //A～C表が許可されてない場合は飛ばす
        //////////            }
        //////////            DataRow row = new DataTable().NewRow();
        //////////            DataSet dataSet = new DataSet();
        //////////            DataTable dt = new DataTable();
        //////////            using(DbManager dm = new DbManager()) {
        //////////                //扶養控除基本データを取得
        //////////                row = GetBasicDataRow_Huyou(key, dm, strSelfDeclareCode);
        //////////                ////目標管理承認データを取得
        //////////                //dataSet = SelfDeclareCommonBL.GetSignData(dm, int.Parse(key));
        //////////                //扶養控除詳細データを取得
        //////////                dt = GetDataTable_Huyou(key, dm, tblType);

        //////////                //dt = GetSelfDeclareTable(key, dm, strSelfDeclareCode);
        //////////            }

        //////////            //帳票を出力
        //////////            //職掌番号により自己申告書の分岐を行う
        //////////            var HuyouDeclareReport = new HuyouDeclareReport();

        //////////            try
        //////////            {
        //////////                //ファイル名作成
        //////////                string fileName = row["対象年度"].ToString()
        //////////                    + "_" + row["所属番号"].ToString()
        //////////                    + "_" + row["社員番号"].ToString()
        //////////                    + "_" + nowDate.ToString("yyyyMMddHHmmss")
        //////////                    + ".pdf";

        //////////                HuyouDeclareReport.SetDataSource(dt);       //目標管理詳細をセット
        //////////                HuyouDeclareReport.Refresh();

        //////////                //扶養控除基本パラメーターを設定
        //////////                HuyouDeclareBaseSetting(ref HuyouDeclareReport, row);

        //////////                ////扶養控除承認パラメーターを設定
        //////////                //SelfDeclareDApprovalSetting(ref crystalReportD, dataSet.Tables[0]);

        //////////                HuyouDeclareReport.PrintOptions.PaperOrientation = CrystalDecisions.Shared.PaperOrientation.Landscape;

        //////////                //pdf出力
        //////////                HuyouDeclareReport.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, workFolder + fileName);

        //////////            }



        //////////            //try {
        //////////            //    //ファイル名作成
        //////////            //    string fileName = row["年度"].ToString()
        //////////            //        + "_" + row["所属番号"].ToString()
        //////////            //        + "_" + row["社員番号"].ToString()
        //////////            //        + "_" + nowDate.ToString("yyyyMMddHHmmss")
        //////////            //        + ".pdf";

        //////////            //    switch( strSelfDeclareCode ){
        //////////            //        case  "A11":
        //////////            //            //crystal report作成                    
        //////////            //            crystalReportA11.SetDataSource(dt);       //自己申告書詳細をセット
        //////////            //            crystalReportA11.Refresh();

        //////////            //            //自己申告書基本パラメーターを設定
        //////////            //            SelfDeclareA11BaseSetting(ref crystalReportA11, dt.Rows[0]);

        //////////            //            crystalReportA11.PrintOptions.PaperOrientation = CrystalDecisions.Shared.PaperOrientation.Landscape;

        //////////            //            //pdf出力
        //////////            //            crystalReportA11.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, workFolder + fileName);
        //////////            //            break;

        //////////            //        case  "A12":
        //////////            //            //crystal report作成                    
        //////////            //            crystalReportA12.SetDataSource(dt);       //自己申告書詳細をセット
        //////////            //            crystalReportA12.Refresh();

        //////////            //            //自己申告書基本パラメーターを設定
        //////////            //            SelfDeclareA12BaseSetting(ref crystalReportA12, dt.Rows[0]);

        //////////            //            crystalReportA12.PrintOptions.PaperOrientation = CrystalDecisions.Shared.PaperOrientation.Landscape;

        //////////            //            //pdf出力
        //////////            //            crystalReportA12.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, workFolder + fileName);
        //////////            //            break;

        //////////            //        case  "A13":
        //////////            //            //crystal report作成                    
        //////////            //            crystalReportA13.SetDataSource(dt);       //自己申告書詳細をセット
        //////////            //            crystalReportA13.Refresh();

        //////////            //            //自己申告書基本パラメーターを設定
        //////////            //            SelfDeclareA13BaseSetting(ref crystalReportA13, dt.Rows[0]);

        //////////            //            crystalReportA13.PrintOptions.PaperOrientation = CrystalDecisions.Shared.PaperOrientation.Landscape;

        //////////            //            //pdf出力
        //////////            //            crystalReportA13.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, workFolder + fileName);
        //////////            //            break;

        //////////            //        case  "B11":
        //////////            //            //crystal report作成                    
        //////////            //            crystalReportB11.SetDataSource(dt);       //自己申告書詳細をセット
        //////////            //            crystalReportB11.Refresh();

        //////////            //            //自己申告書基本パラメーターを設定
        //////////            //            SelfDeclareB11BaseSetting(ref crystalReportB11, dt.Rows[0]);

        //////////            //            crystalReportB11.PrintOptions.PaperOrientation = CrystalDecisions.Shared.PaperOrientation.Landscape;

        //////////            //            //pdf出力
        //////////            //            crystalReportB11.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, workFolder + fileName);
        //////////            //            break;

        //////////            //        case  "B12":
        //////////            //            //crystal report作成                    
        //////////            //            crystalReportB12.SetDataSource(dt);       //自己申告書詳細をセット
        //////////            //            crystalReportB12.Refresh();

        //////////            //            //自己申告書基本パラメーターを設定
        //////////            //            SelfDeclareB12BaseSetting(ref crystalReportB12, dt.Rows[0]);

        //////////            //            crystalReportB12.PrintOptions.PaperOrientation = CrystalDecisions.Shared.PaperOrientation.Landscape;

        //////////            //            //pdf出力
        //////////            //            crystalReportB12.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, workFolder + fileName);
        //////////            //            break;

        //////////            //        case  "C11":
        //////////            //            //crystal report作成                    
        //////////            //            crystalReportC11.SetDataSource(dt);       //自己申告書詳細をセット
        //////////            //            crystalReportC11.Refresh();

        //////////            //            //自己申告書基本パラメーターを設定
        //////////            //            SelfDeclareC11BaseSetting(ref crystalReportC11, dt.Rows[0]);

        //////////            //            crystalReportC11.PrintOptions.PaperOrientation = CrystalDecisions.Shared.PaperOrientation.Landscape;

        //////////            //            //pdf出力
        //////////            //            crystalReportC11.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, workFolder + fileName);
        //////////            //            break;

        //////////            //        case  "C12":
        //////////            //            //crystal report作成                    
        //////////            //            crystalReportC12.SetDataSource(dt);       //自己申告書詳細をセット
        //////////            //            crystalReportC12.Refresh();

        //////////            //            //自己申告書基本パラメーターを設定
        //////////            //            SelfDeclareC12BaseSetting(ref crystalReportC12, dt.Rows[0]);

        //////////            //            crystalReportC12.PrintOptions.PaperOrientation = CrystalDecisions.Shared.PaperOrientation.Landscape;

        //////////            //            //pdf出力
        //////////            //            crystalReportC12.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, workFolder + fileName);
        //////////            //            break;

        //////////            //        case  "A20":
        //////////            //            //crystal report作成                    
        //////////            //            crystalReportA20.SetDataSource(dt);       //自己申告書詳細をセット
        //////////            //            crystalReportA20.Refresh();

        //////////            //            //自己申告書基本パラメーターを設定
        //////////            //            SelfDeclareA20BaseSetting(ref crystalReportA20, dt.Rows[0]);

        //////////            //            crystalReportA20.PrintOptions.PaperOrientation = CrystalDecisions.Shared.PaperOrientation.Landscape;

        //////////            //            //pdf出力
        //////////            //            crystalReportA20.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, workFolder + fileName);
        //////////            //            break;

        //////////            //        case  "B20":
        //////////            //            //crystal report作成                    
        //////////            //            crystalReportB20.SetDataSource(dt);       //自己申告書詳細をセット
        //////////            //            crystalReportB20.Refresh();

        //////////            //            //自己申告書基本パラメーターを設定
        //////////            //            SelfDeclareB20BaseSetting(ref crystalReportB20, dt.Rows[0]);

        //////////            //            crystalReportB20.PrintOptions.PaperOrientation = CrystalDecisions.Shared.PaperOrientation.Landscape;

        //////////            //            //pdf出力
        //////////            //            crystalReportB20.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, workFolder + fileName);
        //////////            //            break;

        //////////            //        case  "C20":
        //////////            //            //crystal report作成                    
        //////////            //            crystalReportC20.SetDataSource(dt);       //自己申告書詳細をセット
        //////////            //            crystalReportC20.Refresh();

        //////////            //            //自己申告書基本パラメーターを設定
        //////////            //            SelfDeclareC20BaseSetting(ref crystalReportC20, dt.Rows[0]);

        //////////            //            crystalReportC20.PrintOptions.PaperOrientation = CrystalDecisions.Shared.PaperOrientation.Landscape;

        //////////            //            //pdf出力
        //////////            //            crystalReportC20.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, workFolder + fileName);
        //////////            //            break;


        //////////            //    }
                        
        //////////            catch(Exception ex) {
        //////////                //　TODO:エラー処理検討中
        //////////                throw ex;
        //////////                //throw;
        //////////            } finally {
        //////////                HuyouDeclareReport.Dispose();
        //////////            };
        //////////        }
               
        //////////        // TODO:pdf複数出力で、どれか１つにエラーがあってもダウンロードできる状況
        //////////        //圧縮
        //////////        var compress = new Compress();
        //////////        string zipFullPath = compress.CreateZipFile(zipName, zipFolder, workFolder);

        //////////        //return用zipファイルパスをセット
        //////////        return zipReturnPath;

        //////////    } catch(Exception ex) {
        //////////        // エラー
        //////////        nlog.Error(System.Reflection.MethodBase.GetCurrentMethod().Name + " error " + ex.ToString());
        //////////        throw;
        //////////    } finally {
        //////////        //終了
        //////////        nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " end");
        //////////    }
        //////////}




        









        
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
        // 2017-03-31 sbc-sagara add end 一括Excel出力ボタン追加
    }
}
