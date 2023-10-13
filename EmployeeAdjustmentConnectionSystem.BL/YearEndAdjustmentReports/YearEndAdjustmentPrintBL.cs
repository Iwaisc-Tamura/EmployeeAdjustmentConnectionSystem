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
                    string key = arrayData[0];                  //管理番号
                    string tblType = "D01";              //TBL区分

                    DataRow row = new DataTable().NewRow();
                    DataSet dataSet = new DataSet();
                    DataTable dt = new DataTable();
                    using (DbManager dm = new DbManager())
                    {
                        //目標管理基本データを取得
                        row = GetBasicDataRow_Haiguu(key, dm, tblType);
                        ////目標管理承認データを取得
                        //dataSet = SelfDeclareCommonBL.GetSignData(dm, int.Parse(key));
                        //目標管理詳細データを取得
                        dt = GetDataTable_Haiguu(key, dm, tblType);
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
        private DataRow GetBasicDataRow_Haiguu(string keyVal, DbManager dm, string tblType)
        {
            try
            {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");

                string sql = ""; //クエリ生成

                sql = "SELECT * FROM TE120基礎控除申告書Data WHERE 社員番号 = @key ";

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
        private DataTable GetDataTable_Haiguu(string keyVal, DbManager dm, string tblType)
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
                    + "	    ,基礎控除.個人番号相違確認区分 "
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
                    + "     ,基礎控除.所得金額調整控除申告書_扶養親族等同上区分 "
                    + "     ,基礎控除.所得金額調整控除申告書_扶養親族等住所 "
                    + "     ,扶養親族等続柄名.続柄名称 AS 所得金額調整控除申告書_扶養親族等続柄名称 "
                    + "     ,基礎控除.所得金額調整控除申告書_扶養親族等所得金額 "
                    + "     ,基礎控除.所得金額調整控除申告書_特別障害者該当事実 "
                    + "     ,事業所名.税務署名 AS 税務署長 "
                    + "     ,事業所名.事業主名 AS 給与支払者名称 "
                    + "     ,事業所名.所在地 AS 給与支払者所在地 "
                    + " FROM TE120基礎控除申告書Data AS 基礎控除"
                    + "  LEFT JOIN TM911続柄名Master AS 配偶者続柄名 ON 基礎控除.配偶者控除申告書_続柄 = 配偶者続柄名.続柄番号 "
                    + "  LEFT JOIN TM911続柄名Master AS 扶養親族等続柄名 ON 基礎控除.所得金額調整控除申告書_扶養親族等続柄 = 扶養親族等続柄名.続柄番号 "
                    + "  LEFT JOIN TM912事業所名Master AS 事業所名 ON LEFT(基礎控除.所属番号,1) = 事業所名.事業所番号 "
                    + " WHERE 基礎控除.社員番号 = @key ";

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
                    string key = arrayData[0];                  //管理番号
                    string tblType = "D01";              //TBL区分

                    DataRow row = new DataTable().NewRow();
                    DataSet dataSet = new DataSet();
                    DataTable dt = new DataTable();
                    using (DbManager dm = new DbManager())
                    {
                        //目標管理基本データを取得
                        row = GetBasicDataRow_Huyou(key, dm, tblType);
                        ////目標管理承認データを取得
                        //dataSet = SelfDeclareCommonBL.GetSignData(dm, int.Parse(key));
                        //目標管理詳細データを取得
                        dt = GetDataTable_Huyou(key, dm, tblType);
                    }

                    //帳票を出力
                    //職掌番号により自己申告書の分岐を行う
                    var HuyouDeclareReport = new HuyouDeclareReport();

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
        private DataRow GetBasicDataRow_Huyou(string keyVal, DbManager dm, string tblType)
        {
            try
            {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");

                string sql = ""; //クエリ生成

                sql = "SELECT * FROM TE120基礎控除申告書Data WHERE 社員番号 = @key ";

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
        private DataTable GetDataTable_Huyou(string keyVal, DbManager dm, string tblType)
        {
            try
            {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");

                string sql = "";
                sql = " SELECT "
                    + "     社員番号 as test "
                    + "     ,氏名_姓 as 氏名_姓 "
                    + " FROM TE120基礎控除申告書Data"
                    + " WHERE 社員番号 = @key ";

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
                    string key = arrayData[0];                  //管理番号
                    string tblType = "D01";              //TBL区分

                    DataRow row = new DataTable().NewRow();
                    DataSet dataSet = new DataSet();
                    DataTable dt = new DataTable();
                    using (DbManager dm = new DbManager())
                    {
                        //基本データを取得
                        row = GetBasicDataRow_Hoken(key, dm, tblType);
                        //データを取得
                        dt = GetDataTable_Hoken(key, dm, tblType);
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
        private DataRow GetBasicDataRow_Hoken(string keyVal, DbManager dm, string tblType)
        {
            try
            {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");

                string sql = ""; //クエリ生成

                sql = "SELECT * FROM TE120基礎控除申告書Data WHERE 社員番号 = @key ";

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
        private DataTable GetDataTable_Hoken(string keyVal, DbManager dm, string tblType)
        {
            try
            {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");

                string sql = "";
                sql = " SELECT "
                    + "     社員番号 as 社員番号 "
                    + "     ,対象年度 as 対象年度 "
                    + "     ,氏名_姓 as 氏名 "
                    + " FROM TE120基礎控除申告書Data"
                    + " WHERE 社員番号 = @key ";

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
