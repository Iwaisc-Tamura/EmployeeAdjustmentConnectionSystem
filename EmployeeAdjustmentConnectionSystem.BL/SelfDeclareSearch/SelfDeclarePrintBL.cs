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
using EmployeeAdjustmentConnectionSystem.BL.SelfDeclareSearch.Reports;
using System.Globalization;
using EmployeeAdjustmentConnectionSystem.COM.Util.Convert;
using EmployeeAdjustmentConnectionSystem.BL.Common;
using EmployeeAdjustmentConnectionSystem.COM.Util.Zip;
using System.Data.SqlClient;
using EmployeeAdjustmentConnectionSystem.COM.Models;
//2017-03-31 sbc-sagara add str 一括Excel出力
//using System.Data.OleDb;D:\JS作業用\develop\01.source\SkillDiscriminantSystem\SkillDiscriminantSystem.Bl\SelfDeclareSearch\SelfDeclarePrintBL.cs
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;
using EmployeeAdjustmentConnectionSystem.BL.SelfDeclareRegister;
//2017-03-31 sbc-sagara add end 一括Excel出力

namespace EmployeeAdjustmentConnectionSystem.BL.SelfDeclareSearch {
    /// <summary>
    /// 目標管理印刷
    /// </summary>
    public class SelfDeclareSearchPrintBL {
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
        public SelfDeclareSearchPrintBL(string fullPath) {
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

        /// <summary>
        /// メインの処理
        /// </summary>
        /// <param name="selPrint"></param>
        /// <returns></returns>
        public string PrintAtoC(string[] selPrint) {
            try {
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
                foreach(string file in System.IO.Directory.GetDirectories(TempDir, "*")) {
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
                foreach(string KeyValue in selPrint) {
                    //管理番号とTBL区分に分割
                    string[] arrayData = KeyValue.Split(',');
                    string key = arrayData[0];                  //管理番号
                    string strSelfDeclareCode = arrayData[1];   //自己申告書種別Code
                    string DutyNo = arrayData[2];               //職掌番号

                    if (arrayData[3] != "1") {
                        continue;   //A～C表が許可されてない場合は飛ばす
                    }
                    DataRow row = new DataTable().NewRow();
                    DataSet dataSet = new DataSet();
                    DataTable dt = new DataTable();
                    using(DbManager dm = new DbManager()) {
                        //目標管理基本データを取得
                        row = GetBasicSelfDeclareRow(key, dm, strSelfDeclareCode);
                        ////目標管理承認データを取得
                        dataSet = SelfDeclareCommonBL.GetSignData(dm, int.Parse(key));
                        //目標管理詳細データを取得
                        dt = GetSelfDeclareTable(key, dm, strSelfDeclareCode);
                    }

                    //帳票を出力
                    //職掌番号により自己申告書の分岐を行う

                    //2020-03-31 iwai-tamura upd str -----
                    //var crystalReportA = new SelfDeclareReportA();
                    //var crystalReportB = new SelfDeclareReportB();
                    //var crystalReportC = new SelfDeclareReportC();

                    var crystalReportA11 = new SelfDeclareReportA11();
                    var crystalReportA12 = new SelfDeclareReportA12();
                    var crystalReportA13 = new SelfDeclareReportA13();
                    var crystalReportB11 = new SelfDeclareReportB11();
                    var crystalReportB12 = new SelfDeclareReportB12();
                    var crystalReportC11 = new SelfDeclareReportC11();
                    var crystalReportC12 = new SelfDeclareReportC12();
                    //2021-12-24 iwai-tamura upd str ------
                    var crystalReportA20 = new SelfDeclareReportA20();
                    var crystalReportB20 = new SelfDeclareReportB20();
                    var crystalReportC20 = new SelfDeclareReportC20();
                    //2021-12-24 iwai-tamura upd end ------                    

                    try {
                        //ファイル名作成
                        string fileName = row["年度"].ToString()
                            + "_" + row["所属番号"].ToString()
                            + "_" + row["社員番号"].ToString()
                            + "_" + nowDate.ToString("yyyyMMddHHmmss")
                            + ".pdf";

                        switch( strSelfDeclareCode ){
                            case  "A11":
                                //crystal report作成                    
                                crystalReportA11.SetDataSource(dt);       //自己申告書詳細をセット
                                crystalReportA11.Refresh();

                                //自己申告書基本パラメーターを設定
                                SelfDeclareA11BaseSetting(ref crystalReportA11, dt.Rows[0]);

                                crystalReportA11.PrintOptions.PaperOrientation = CrystalDecisions.Shared.PaperOrientation.Landscape;

                                //pdf出力
                                crystalReportA11.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, workFolder + fileName);
                                break;

                            case  "A12":
                                //crystal report作成                    
                                crystalReportA12.SetDataSource(dt);       //自己申告書詳細をセット
                                crystalReportA12.Refresh();

                                //自己申告書基本パラメーターを設定
                                SelfDeclareA12BaseSetting(ref crystalReportA12, dt.Rows[0]);

                                crystalReportA12.PrintOptions.PaperOrientation = CrystalDecisions.Shared.PaperOrientation.Landscape;

                                //pdf出力
                                crystalReportA12.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, workFolder + fileName);
                                break;

                            case  "A13":
                                //crystal report作成                    
                                crystalReportA13.SetDataSource(dt);       //自己申告書詳細をセット
                                crystalReportA13.Refresh();

                                //自己申告書基本パラメーターを設定
                                SelfDeclareA13BaseSetting(ref crystalReportA13, dt.Rows[0]);

                                crystalReportA13.PrintOptions.PaperOrientation = CrystalDecisions.Shared.PaperOrientation.Landscape;

                                //pdf出力
                                crystalReportA13.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, workFolder + fileName);
                                break;

                            case  "B11":
                                //crystal report作成                    
                                crystalReportB11.SetDataSource(dt);       //自己申告書詳細をセット
                                crystalReportB11.Refresh();

                                //自己申告書基本パラメーターを設定
                                SelfDeclareB11BaseSetting(ref crystalReportB11, dt.Rows[0]);

                                crystalReportB11.PrintOptions.PaperOrientation = CrystalDecisions.Shared.PaperOrientation.Landscape;

                                //pdf出力
                                crystalReportB11.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, workFolder + fileName);
                                break;

                            case  "B12":
                                //crystal report作成                    
                                crystalReportB12.SetDataSource(dt);       //自己申告書詳細をセット
                                crystalReportB12.Refresh();

                                //自己申告書基本パラメーターを設定
                                SelfDeclareB12BaseSetting(ref crystalReportB12, dt.Rows[0]);

                                crystalReportB12.PrintOptions.PaperOrientation = CrystalDecisions.Shared.PaperOrientation.Landscape;

                                //pdf出力
                                crystalReportB12.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, workFolder + fileName);
                                break;

                            case  "C11":
                                //crystal report作成                    
                                crystalReportC11.SetDataSource(dt);       //自己申告書詳細をセット
                                crystalReportC11.Refresh();

                                //自己申告書基本パラメーターを設定
                                SelfDeclareC11BaseSetting(ref crystalReportC11, dt.Rows[0]);

                                crystalReportC11.PrintOptions.PaperOrientation = CrystalDecisions.Shared.PaperOrientation.Landscape;

                                //pdf出力
                                crystalReportC11.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, workFolder + fileName);
                                break;

                            case  "C12":
                                //crystal report作成                    
                                crystalReportC12.SetDataSource(dt);       //自己申告書詳細をセット
                                crystalReportC12.Refresh();

                                //自己申告書基本パラメーターを設定
                                SelfDeclareC12BaseSetting(ref crystalReportC12, dt.Rows[0]);

                                crystalReportC12.PrintOptions.PaperOrientation = CrystalDecisions.Shared.PaperOrientation.Landscape;

                                //pdf出力
                                crystalReportC12.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, workFolder + fileName);
                                break;

                            //2021-12-24 iwai-tamura add-str ------
                            case  "A20":
                                //crystal report作成                    
                                crystalReportA20.SetDataSource(dt);       //自己申告書詳細をセット
                                crystalReportA20.Refresh();

                                //自己申告書基本パラメーターを設定
                                SelfDeclareA20BaseSetting(ref crystalReportA20, dt.Rows[0]);

                                crystalReportA20.PrintOptions.PaperOrientation = CrystalDecisions.Shared.PaperOrientation.Landscape;

                                //pdf出力
                                crystalReportA20.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, workFolder + fileName);
                                break;

                            case  "B20":
                                //crystal report作成                    
                                crystalReportB20.SetDataSource(dt);       //自己申告書詳細をセット
                                crystalReportB20.Refresh();

                                //自己申告書基本パラメーターを設定
                                SelfDeclareB20BaseSetting(ref crystalReportB20, dt.Rows[0]);

                                crystalReportB20.PrintOptions.PaperOrientation = CrystalDecisions.Shared.PaperOrientation.Landscape;

                                //pdf出力
                                crystalReportB20.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, workFolder + fileName);
                                break;

                            case  "C20":
                                //crystal report作成                    
                                crystalReportC20.SetDataSource(dt);       //自己申告書詳細をセット
                                crystalReportC20.Refresh();

                                //自己申告書基本パラメーターを設定
                                SelfDeclareC20BaseSetting(ref crystalReportC20, dt.Rows[0]);

                                crystalReportC20.PrintOptions.PaperOrientation = CrystalDecisions.Shared.PaperOrientation.Landscape;

                                //pdf出力
                                crystalReportC20.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, workFolder + fileName);
                                break;

                            //2021-12-24 iwai-tamura add-end ------

                        }
                        
                    } catch(Exception ex) {
                        //　TODO:エラー処理検討中
                        throw ex;
                        //throw;
                    } finally {
                        crystalReportA11.Dispose();
                        crystalReportA12.Dispose();
                        crystalReportA13.Dispose();
                        crystalReportB11.Dispose();
                        crystalReportB12.Dispose();
                        crystalReportC11.Dispose();
                        crystalReportC12.Dispose();
                        //2021-12-24 iwai-tamura add-str ------
                        crystalReportA20.Dispose();
                        crystalReportB20.Dispose();
                        crystalReportC20.Dispose();
                        //2021-12-24 iwai-tamura add-end ------
                    };
                    
                    //try {
                    //    //ファイル名作成
                    //    //2017-05-18 iwai-tamura upd str -----
                    //    string fileName = row["年度"].ToString()
                    //        + "_" + row["所属番号"].ToString()
                    //        + "_" + row["社員番号"].ToString()
                    //        + "_" + nowDate.ToString("yyyyMMddHHmmss")
                    //        + ".pdf";
                    //    //string fileName = row["社員番号"].ToString()
                    //    //    + "_" + row["年度"].ToString() + "_"  + row["所属番号"].ToString() + "_" + nowDate.ToString("yyyyMMddHHmmss") + ".pdf";
                    //    //2017-05-18 iwai-tamura upd end -----


                    //    switch( DutyNo ){
                    //        case  "30": //専任職
                    //            //crystal report作成                    
                    //            crystalReportA.SetDataSource(dt);       //目標管理詳細をセット
                    //            crystalReportA.Refresh();

                    //            //目標管理基本パラメーターを設定
                    //            SelfDeclareABaseSetting(ref crystalReportA, dt.Rows[0]);

                    //            //目標管理承認パラメーターを設定
                    //            //SelfDeclareAApprovalSetting(ref crystalReportA, dataSet.Tables[0]);

                    //            crystalReportA.PrintOptions.PaperOrientation = CrystalDecisions.Shared.PaperOrientation.Landscape;

                    //            //pdf出力
                    //            crystalReportA.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, workFolder + fileName);
                    //            break;

                    //        case  "20": //総括職
                    //            //crystal report作成                    
                    //            crystalReportB.SetDataSource(dt);       //目標管理詳細をセット
                    //            crystalReportB.Refresh();

                    //            //目標管理基本パラメーターを設定
                    //            SelfDeclareBBaseSetting(ref crystalReportB,  dt.Rows[0]);

                    //            crystalReportB.PrintOptions.PaperOrientation = CrystalDecisions.Shared.PaperOrientation.Landscape;

                    //            //pdf出力
                    //            crystalReportB.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, workFolder + fileName);
                    //            break;

                    //        case  "11": //管理職
                    //        //2019-04-01 iwai-tamura add str ------
                    //        case  "14": //専任職(管理職)
                    //        //2019-04-01 iwai-tamura add end ------
                    //            //crystal report作成                    
                    //            crystalReportC.SetDataSource(dt);       //目標管理詳細をセット
                    //            crystalReportC.Refresh();

                    //            //目標管理基本パラメーターを設定
                    //            SelfDeclareCBaseSetting(ref crystalReportC, dt.Rows[0]);

                    //            crystalReportC.PrintOptions.PaperOrientation = CrystalDecisions.Shared.PaperOrientation.Landscape;

                    //            //pdf出力
                    //            crystalReportC.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, workFolder + fileName);
                    //            break;
                    //    }
                        
                    //} catch(Exception ex) {
                    //    //　TODO:エラー処理検討中
                    //    throw ex;
                    //    //throw;
                    //} finally {
                    //    crystalReportA.Dispose();
                    //    crystalReportB.Dispose();
                    //    crystalReportC.Dispose();
                    //};
                    //2020-03-31 iwai-tamura upd end -----

                }
               
                // TODO:pdf複数出力で、どれか１つにエラーがあってもダウンロードできる状況
                //圧縮
                var compress = new Compress();
                string zipFullPath = compress.CreateZipFile(zipName, zipFolder, workFolder);

                //return用zipファイルパスをセット
                return zipReturnPath;

            } catch(Exception ex) {
                // エラー
                nlog.Error(System.Reflection.MethodBase.GetCurrentMethod().Name + " error " + ex.ToString());
                throw;
            } finally {
                //終了
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " end");
            }
        }


        //2023-11-20 iwai-tamura test-str ------
        /// <summary>
        /// メインの処理
        /// </summary>
        /// <param name="selPrint"></param>
        /// <returns></returns>
        public string PrintTEST(string[] selPrint)
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
                        row = GetBasicTESTRow(key, dm, tblType);
                        ////目標管理承認データを取得
                        //dataSet = SelfDeclareCommonBL.GetSignData(dm, int.Parse(key));
                        //目標管理詳細データを取得
                        dt = GetTESTTable(key, dm, tblType);
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
                        TESTBaseSetting(ref HaiguuDeclareReport, row);

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
        private DataRow GetBasicTESTRow(string keyVal, DbManager dm, string tblType)
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
        private DataTable GetTESTTable(string keyVal, DbManager dm, string tblType)
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
                    + "     ,基礎控除.配偶者控除申告書_控除額計算判定 "
                    + "     ,基礎控除.配偶者控除申告書_控除額計算区分 "
                    + "     ,基礎控除.配偶者控除申告書_配偶者控除額 "
                    + "     ,基礎控除.配偶者控除申告書_配偶者特別控除額 "
                    + "     ,基礎控除.所得金額調整控除申告書_要件区分 "
                    + "     ,基礎控除.所得金額調整控除申告書_扶養親族等氏名_姓 + ' ' + 基礎控除.所得金額調整控除申告書_扶養親族等氏名_名 AS 所得金額調整控除申告書_扶養親族等氏名 "
                    + "     ,基礎控除.所得金額調整控除申告書_扶養親族等Kana_姓 + ' ' + 基礎控除.所得金額調整控除申告書_扶養親族等Kana_名 AS 所得金額調整控除申告書_扶養親族等Kana "
                    + "     ,基礎控除.所得金額調整控除申告書_扶養親族等生年月日 "
                    + "     ,基礎控除.所得金額調整控除申告書_扶養親族等同上区分 "
                    + "     ,基礎控除.所得金額調整控除申告書_扶養親族等住所 "
                    + "     ,扶養親族等続柄名.続柄名称 AS 所得金額調整控除申告書_扶養親族等続柄名称 "
                    + "     ,基礎控除.所得金額調整控除申告書_扶養親族等所得金額 "
                    + "     ,基礎控除.所得金額調整控除申告書_特別障害者該当事実 "
                    + " FROM TE120基礎控除申告書Data AS 基礎控除"
                    + "  LEFT JOIN TM911続柄名Master AS 配偶者続柄名 ON 基礎控除.配偶者控除申告書_続柄 = 配偶者続柄名.続柄番号 "
                    + "  LEFT JOIN TM911続柄名Master AS 扶養親族等続柄名 ON 基礎控除.所得金額調整控除申告書_扶養親族等続柄 = 扶養親族等続柄名.続柄番号 "
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
        private void TESTBaseSetting(ref HaiguuDeclareReport cr, DataRow dr)
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
        //2023-11-20 iwai-tamura test-end ------

        //2023-11-20 iwai-terao test-str 扶養控除ボタン------
        /// <summary>
        /// メインの処理
        /// </summary>
        /// <param name="selPrint"></param>
        /// <returns></returns>
        public string PrintTEST2(string[] selPrint)
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
                        row = GetBasicTESTRow2(key, dm, tblType);
                        ////目標管理承認データを取得
                        //dataSet = SelfDeclareCommonBL.GetSignData(dm, int.Parse(key));
                        //目標管理詳細データを取得
                        dt = GetTESTTable2(key, dm, tblType);
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
                        TESTBaseSetting2(ref HuyouDeclareReport, row);

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
        private DataRow GetBasicTESTRow2(string keyVal, DbManager dm, string tblType)
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
        private DataTable GetTESTTable2(string keyVal, DbManager dm, string tblType)
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
        private void TESTBaseSetting2(ref HuyouDeclareReport cr, DataRow dr)
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
        //2023-11-20 iwai-terao test-end ------


        //2023-11-20 iwai-terao test-str 保険料控除ボタン------
        /// <summary>
        /// メインの処理
        /// </summary>
        /// <param name="selPrint"></param>
        /// <returns></returns>
        public string PrintTEST3(string[] selPrint)
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
                        row = GetBasicTESTRow3(key, dm, tblType);
                        ////目標管理承認データを取得
                        //dataSet = SelfDeclareCommonBL.GetSignData(dm, int.Parse(key));
                        //目標管理詳細データを取得
                        dt = GetTESTTable3(key, dm, tblType);
                    }

                    //帳票を出力
                    //職掌番号により自己申告書の分岐を行う
                    var HokenDeclareReport = new HokenDeclareReport();

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

                        HokenDeclareReport.SetDataSource(dt);       //目標管理詳細をセット
                        HokenDeclareReport.Refresh();

                        //目標管理基本パラメーターを設定
                        TESTBaseSetting3(ref HokenDeclareReport, row);

                        ////目標管理承認パラメーターを設定
                        //SelfDeclareDApprovalSetting(ref crystalReportD, dataSet.Tables[0]);

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
        private DataRow GetBasicTESTRow3(string keyVal, DbManager dm, string tblType)
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
        private DataTable GetTESTTable3(string keyVal, DbManager dm, string tblType)
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
        private void TESTBaseSetting3(ref HokenDeclareReport cr, DataRow dr)
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
        //2023-11-20 iwai-terao test-end ------

        /// <summary>
        /// メインの処理
        /// </summary>
        /// <param name="selPrint"></param>
        /// <returns></returns>
        public string PrintD(string[] selPrint) {
            try {
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
                foreach(string file in System.IO.Directory.GetDirectories(TempDir, "*")) {
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
                foreach(string KeyValue in selPrint) {
                    //管理番号とTBL区分に分割
                    string[] arrayData = KeyValue.Split(',');
                    string key = arrayData[0];                  //管理番号
                    string tblType = "D01";              //TBL区分
                    if (arrayData[4] != "1") {
                        continue;   //D表が許可されてない場合は飛ばす
                    }
                    DataRow row = new DataTable().NewRow();
                    DataSet dataSet = new DataSet();
                    DataTable dt = new DataTable();
                    using(DbManager dm = new DbManager()) {
                        //目標管理基本データを取得
                        row = GetBasicSelfDeclareRow(key, dm, tblType);
                        ////目標管理承認データを取得
                        dataSet = SelfDeclareCommonBL.GetSignData(dm, int.Parse(key));
                        //目標管理詳細データを取得
                        dt = GetSelfDeclareTable(key, dm, tblType);
                    }

                    //帳票を出力
                    //職掌番号により自己申告書の分岐を行う
                    var crystalReportD = new SelfDeclareReportD();

                    try {
                        //ファイル名作成
                        //2017-05-18 iwai-tamura upd str -----
                        string fileName = row["年度"].ToString()
                            + "_" + row["所属番号"].ToString()
                            + "_" + row["社員番号"].ToString()
                            + "_" + nowDate.ToString("yyyyMMddHHmmss")
                            + ".pdf";
                        //string fileName = row["社員番号"].ToString()
                        //    + "_" + row["年度"].ToString() + "_"  + row["所属番号"].ToString() + "_" + nowDate.ToString("yyyyMMddHHmmss") + ".pdf";
                        //2017-05-18 iwai-tamura upd end -----

                        crystalReportD.SetDataSource(dt);       //目標管理詳細をセット
                        crystalReportD.Refresh();

                        //目標管理基本パラメーターを設定
                        SelfDeclareDBaseSetting(ref crystalReportD, row);

                        ////目標管理承認パラメーターを設定
                        //SelfDeclareDApprovalSetting(ref crystalReportD, dataSet.Tables[0]);

                        crystalReportD.PrintOptions.PaperOrientation = CrystalDecisions.Shared.PaperOrientation.Landscape;

                        //pdf出力
                        crystalReportD.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, workFolder + fileName);
                        
                    } catch(Exception ex) {
                        //　TODO:エラー処理検討中
                        throw ex;
                        //throw;
                    } finally {
                        crystalReportD.Dispose();
                    };
                }
               
                // TODO:pdf複数出力で、どれか１つにエラーがあってもダウンロードできる状況
                //圧縮
                var compress = new Compress();
                string zipFullPath = compress.CreateZipFile(zipName, zipFolder, workFolder);

                //return用zipファイルパスをセット
                return zipReturnPath;

            } catch(Exception ex) {
                // エラー
                nlog.Error(System.Reflection.MethodBase.GetCurrentMethod().Name + " error " + ex.ToString());
                throw;
            } finally {
                //終了
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " end");
            }
        }

        /// <summary>
        /// メインの処理(キャリアシート出力)
        /// </summary>
        /// <param name="selPrint"></param>
        /// <returns></returns>
        public string PrintCarrier(string[] selPrint) {
            try {
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
                foreach(string file in System.IO.Directory.GetDirectories(TempDir, "*")) {
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
                foreach(string KeyValue in selPrint) {
                    //管理番号とTBL区分に分割
                    string[] arrayData = KeyValue.Split(',');
                    string key = arrayData[0];                  //管理番号
                    string tblType = "CH01";              //TBL区分

                    if (arrayData[5] != "1") {
                        continue;   //キャリアシートが許可されてない場合は飛ばす
                    }

                    DataRow row = new DataTable().NewRow();
                    DataSet dataSet = new DataSet();
                    DataTable dt = new DataTable();
                    using(DbManager dm = new DbManager()) {
                        //目標管理基本データを取得
                        row = GetBasicSelfDeclareRow(key, dm, tblType);
                        ////目標管理承認データを取得
                        dataSet = SelfDeclareCommonBL.GetSignData(dm, int.Parse(key));
                        //目標管理詳細データを取得
                        dt = GetSelfDeclareTable(key, dm, tblType);
                    }

                    //帳票を出力
                    //職掌番号により自己申告書の分岐を行う
                    var crystalReportCarrier = new CarrierSheetReport();

                    try {
                        //ファイル名作成
                        //2017-05-18 iwai-tamura upd str -----
                        string fileName = row["年度"].ToString()
                            + "_" + row["所属番号"].ToString()
                            + "_" + row["社員番号"].ToString()
                            + "_" + nowDate.ToString("yyyyMMddHHmmss")
                            + ".pdf";
                        //string fileName = row["社員番号"].ToString()
                        //    + "_" + row["年度"].ToString() + "_"  + row["所属番号"].ToString() + "_" + nowDate.ToString("yyyyMMddHHmmss") + ".pdf";
                        //2017-05-18 iwai-tamura upd end -----


                        crystalReportCarrier.SetDataSource(dt);       //目標管理詳細をセット
                        crystalReportCarrier.Refresh();

                        //目標管理基本パラメーターを設定
                        CarrierSheetBaseSetting(ref crystalReportCarrier, dt.Rows[0]);

                        ////目標管理承認パラメーターを設定
                        //CarrierSheetApprovalSetting(ref crystalReportCarrier, dataSet.Tables[0]);

                        crystalReportCarrier.PrintOptions.PaperOrientation = CrystalDecisions.Shared.PaperOrientation.Landscape;

                        //pdf出力
                        crystalReportCarrier.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, workFolder + fileName);

                    } catch(Exception ex) {
                        //　TODO:エラー処理検討中
                        throw ex;
                        //throw;
                    } finally {
                        crystalReportCarrier.Dispose();
                    };
                }
               
                // TODO:pdf複数出力で、どれか１つにエラーがあってもダウンロードできる状況
                //圧縮
                var compress = new Compress();
                string zipFullPath = compress.CreateZipFile(zipName, zipFolder, workFolder);

                //return用zipファイルパスをセット
                return zipReturnPath;

            } catch(Exception ex) {
                // エラー
                nlog.Error(System.Reflection.MethodBase.GetCurrentMethod().Name + " error " + ex.ToString());
                throw;
            } finally {
                //終了
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " end");
            }
        }        

        
        
        // 2017-03-31 sbc-sagara add str 一括Excel出力ボタン追加
        /// <summary>
        /// 一括Excel出力処理
        /// </summary>
        /// <param name="selPrint"></param>
        /// <returns></returns>
        public string PrintXls(string[] selPrint,string tblType)
        {
            try
            {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");

                string workFolder = "";
                DateTime nowDate = DateTime.Now;      //現在日時を取得
                string userCode = ((EmployeeAdjustmentConnectionSystem.COM.Entity.Session.LoginUser)
                    (HttpContext.Current.Session["LoginUser"])).UserCode.ToString();
                string departmentno = ((EmployeeAdjustmentConnectionSystem.COM.Entity.Session.LoginUser)
                    (HttpContext.Current.Session["LoginUser"])).DepartmentNo.ToString();


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

                //excelファイルの作成
                //ファイル名作成
                string fileName = departmentno + "_" + userCode + "_" + nowDate.ToString("yyyyMMddHHmmss") + ".xlsx";

                // EPPlus使用版
                var outputFile = new FileInfo(workFolder + fileName);
                if (outputFile.Exists) {
                    outputFile.Delete();
                }
                using (var excel = new ExcelPackage(outputFile)) {
                    // シート追加
                    var sheet = excel.Workbook.Worksheets.Add("Sheet1");

                    int excel_row = 1;
                    foreach (string KeyValue in selPrint)
                    {
                        int excel_column = 1;
                        //管理番号とTBL区分に分割
                        string[] arrayData = KeyValue.Split(',');
                        string key = arrayData[0];                  //管理番号
                        //string tblType = arrayData[1];              //TBL区分
                        if (tblType == "D01") {
                            if (arrayData[4]!="1"){
                                continue;   //D表が許可されてない場合は飛ばす
                            }

                            //2022-01-31 iwai-tamura upd-str ------
                            //未認証データの場合飛ばす
                            using (DbManager dm = new DbManager())
                            {
                                string sql = ""; //クエリ生成
                                sql = "select distinct "
                                    + "  ms.管理番号"
                                    + " ,ms.年度"
                                    + " ,ms.所属番号"
                                    + " ,ms.社員番号"
                                    + " ,ms.大区分"
                                    + " ,ms.小区分"
                                    + " ,ms.承認社員番号"
                                    + " ,ms.承認日時"
                                    + " from SD_T自己申告書承認情報 ms"
                                    + " where 管理番号= @ManageNo"
                                    + " And 大区分 = 2 and 小区分 = 1 and 承認社員番号 is not null";
                                DataTable Ddt = new DataTable();
                                DataSet DdataSet = new DataSet();
                                using(IDbCommand cmd = new DbManager().CreateCommand(sql)) {
                                    //パラメータ設定
                                    DbHelper.AddDbParameter(cmd, "@ManageNo", DbType.String);
                                    ((IDbDataParameter)cmd.Parameters[0]).Value = int.Parse(key);
                                    //クエリ実行
                                    IDataAdapter da = dm.CreateSqlDataAdapter(cmd);
                                    da.Fill(DdataSet);
                                }
                                //実行結果確認
                                if(DdataSet.Tables[0].Rows.Count == 0) {
                                    continue; 
                                }
                            }
                            //2022-01-31 iwai-tamura upd-end ------
                        }
                        DataRow row = new DataTable().NewRow();
                        DataSet dataSet = new DataSet();
                        DataTable dt = new DataTable();
                        using (DbManager dm = new DbManager())
                        {
                            //目標管理基本データを取得
                            row = GetBasicSelfDeclareRow(key, dm, tblType);
                            //目標管理承認データを取得
                            dataSet = SelfDeclareCommonBL.GetSignData(dm, int.Parse(key));
                            //目標管理詳細データを取得
                            dt = GetSelfDeclareTable(key, dm, tblType);
                        }

                        //目標管理詳細データを設定
                        for (int i = 1; i <= dt.Rows.Count; i++) {
                            foreach (DataColumn column in dt.Columns) {
                                //if (!command.Parameters.Contains("@" + column.ColumnName)) {
                                    //1行目ならヘッダ設定
                                    if (excel_row == 1)
                                    {
                                        var cell_head = sheet.Cells[excel_row, excel_column];
                                        // セルに値設定
                                        cell_head.Value = column.ColumnName + i;
                                        // そのままだとフォントが英語圏のフォントなので調整
                                        cell_head.Style.Font.SetFromFont(new Font("MS Gothic", 10, FontStyle.Regular));
                                    }
                                    // セル取得
                                    var cell = sheet.Cells[excel_row + 1, excel_column];
                                    // セルに値設定
                                    cell.Value = DataConv.IfNull(dt.Rows[i - 1][column.ColumnName].ToString(), "");
                                    // そのままだとフォントが英語圏のフォントなので調整
                                    cell.Style.Font.SetFromFont(new Font("MS Gothic", 10, FontStyle.Regular));
                                    excel_column++;
                                //}
                            }

                            //目標管理承認データを設定
                            foreach (DataTable dataSettable in dataSet.Tables) {
                                string columnname = "";
                                foreach (DataRow dataSetrow in dataSettable.Rows) {
                                    columnname = dataSetrow["大区分"].ToString()  + dataSetrow["小区分"].ToString() + "承認者";
                                    //1行目ならヘッダ設定
                                    if (excel_row == 1)
                                    {
                                        var cell_head = sheet.Cells[excel_row, excel_column];
                                        // セルに値設定
                                        cell_head.Value = columnname;
                                        // そのままだとフォントが英語圏のフォントなので調整
                                        cell_head.Style.Font.SetFromFont(new Font("MS Gothic", 10, FontStyle.Regular));
                                    }
                                    // セル取得
                                    var cell = sheet.Cells[excel_row + 1, excel_column];
                                    // セルに値設定
                                    cell.Value = DataConv.IfNull(dataSetrow["氏名"].ToString(), "");
                                    // そのままだとフォントが英語圏のフォントなので調整
                                    cell.Style.Font.SetFromFont(new Font("MS Gothic", 10, FontStyle.Regular));
                                    excel_column++;
                                }
                            }
                            excel_row++;
                        }                          
                        
                        
                        ////目標管理基本データを設定
                        //foreach (DataColumn rowcolumn in row.Table.Columns)
                        //{
                        //    //1行目ならヘッダ設定
                        //    if (excel_row == 1)
                        //    {
                        //        var cell_head = sheet.Cells[excel_row, excel_column];
                        //        // セルに値設定
                        //        cell_head.Value = rowcolumn.ColumnName;
                        //        // そのままだとフォントが英語圏のフォントなので調整
                        //        cell_head.Style.Font.SetFromFont(new Font("MS Gothic", 10, FontStyle.Regular));
                        //    }
                        //    // セル取得
                        //    var cell = sheet.Cells[excel_row + 1, excel_column];
                        //    // セルに値設定
                        //    cell.Value = DataConv.IfNull(row[rowcolumn.ColumnName].ToString(), "");
                        //    // そのままだとフォントが英語圏のフォントなので調整
                        //    cell.Style.Font.SetFromFont(new Font("MS Gothic", 10, FontStyle.Regular));
                        //    //command.Parameters.Add("@" + rowcolumn.ColumnName, OleDbType.VarWChar);
                        //    //command.Parameters["@" + rowcolumn.ColumnName].Value = DataConv.IfNull(row[rowcolumn.ColumnName].ToString(), "");
                        //    excel_column++;
                        //}



                    }
                    // 保存
                    excel.Save();
                }

                // OleDb使用版 Excelファイルをデータベースソースとして使用し書込 テスト環境で失敗
                //string connString =
                //    "Provider=Microsoft.ACE.OLEDB.10.0;" +
                //    "Data Source=" + workFolder + fileName + ";" +
                //    "Extended Properties=\"Excel 12.0 Xml;HDR=YES\"";
                //using (OleDbConnection connection =
                //    new OleDbConnection(connString))
                //{
                //    try
                //    {
                //        // Excel ブックを作成
                //        connection.Open();

                //        string createQuery = "";
                //        //目標管理番号ごとにデータを取得
                //        foreach (string KeyValue in selPrint)
                //        {
                //            //管理番号とTBL区分に分割
                //            string[] arrayData = KeyValue.Split(',');
                //            string key = arrayData[0];                  //管理番号
                //            string tblType = arrayData[1];              //TBL区分

                //            DataRow row = new DataTable().NewRow();
                //            DataSet dataSet = new DataSet();
                //            DataTable dt = new DataTable();
                //            using (DbManager dm = new DbManager())
                //            {
                //                //目標管理基本データを取得
                //                row = GetBasicSelfDeclareRow(key, dm, tblType);
                //                //目標管理承認データを取得
                //                dataSet = SelfDeclareCommonBL.GetSignData(dm, int.Parse(key), tblType == "R");
                //                //目標管理詳細データを取得
                //                dt = GetSelfDeclareTable(key, dm, tblType);
                //            }

                //            // シート作成
                //            if (string.IsNullOrEmpty(createQuery))
                //            {
                //                createQuery = CreateQuery(row, dataSet, dt);
                //                using (OleDbCommand command = new OleDbCommand(createQuery, connection))
                //                {
                //                    command.ExecuteNonQuery();
                //                }
                //            }
                //            string insertQuery = InsertQuery(row, dataSet, dt);

                //            // Excel ブックにデータを INSERT
                //            using (OleDbCommand command = new OleDbCommand(insertQuery, connection))
                //            {
                //                //目標管理基本データを設定
                //                foreach (DataColumn rowcolumn in row.Table.Columns)
                //                {
                //                    command.Parameters.Add("@" + rowcolumn.ColumnName, OleDbType.VarWChar);
                //                    command.Parameters["@" + rowcolumn.ColumnName].Value = DataConv.IfNull(row[rowcolumn.ColumnName].ToString(), "");
                //                }

                //                //目標管理承認データを設定
                //                foreach (DataTable dataSettable in dataSet.Tables)
                //                {
                //                    string columnname = "";
                //                    foreach (DataRow dataSetrow in dataSettable.Rows)
                //                    {
                //                        columnname = dataSetrow["大区分"].ToString() + dataSetrow["中区分"].ToString() + dataSetrow["小区分"].ToString() + "承認者";
                //                        command.Parameters.Add("@" + columnname, OleDbType.VarWChar);
                //                        command.Parameters["@" + columnname].Value = DataConv.IfNull(dataSetrow["氏名"].ToString(), "");
                //                    }
                //                }
                                
                //                //目標管理詳細データを設定
                //                for (int i = 1; i <= dt.Rows.Count; i++)
                //                {
                //                    foreach (DataColumn column in dt.Columns)
                //                    {
                //                        if (!command.Parameters.Contains("@" + column.ColumnName))
                //                        {
                //                            command.Parameters.Add("@" + column.ColumnName + i, OleDbType.VarWChar);
                //                            command.Parameters["@" + column.ColumnName + i].Value = DataConv.IfNull(dt.Rows[i - 1][column.ColumnName].ToString(), "");
                //                        }
                //                    }
                //                }  
                                
                //                command.ExecuteNonQuery();
                //            }
                //        }
                //    }
                //    catch (Exception ex)
                //    {
                //        throw ex;
                //    }
                //    finally
                //    {
                //        if (connection.State.Equals(ConnectionState.Open))
                //        {
                //            connection.Close();
                //        }
                //    }
                //}
                
                //作成したファイルに読み取り専用プロパティを設定
                //ネット経由でダウンロードされたファイルを保護されたビューで開くために必要
                FileAttributes fas = File.GetAttributes(workFolder + fileName);
                fas = fas | FileAttributes.ReadOnly;
                File.SetAttributes(workFolder + fileName, fas);

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
        // 2017-03-31 sbc-sagara add end 一括Excel出力ボタン追加



        //2020-03-31 iwai-tamura upd str -----
        /// <summary>
        /// 自己申告書データを帳票パラメータに直接セット
        /// </summary>
        /// <param name="cr">クリスタルレポート</param>
        /// <param name="dr">自己申告書DataRow</param>
        /// <returns>セット後のクリスタルレポート</returns>
        private void SelfDeclareA11BaseSetting(ref SelfDeclareReportA11 cr, DataRow dr) {
            try {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");
                //datetimeに変換できたら和暦、だめだったら空をreturn
                Func<string, string, string> wareki = delegate(string val, string format) {
                    DateTime dt = new DateTime();
                    return DateTime.TryParse(val, out dt) ? DataConv.Date2Jcal(dt, format) : "";
                };

                //2020-04-07 iwai-tamura upd str ------
                Func<string, string, string> nengappi = delegate(string val, string format) {
                    DateTime dt = new DateTime();
                    return DateTime.TryParse(val, out dt) ? dt.ToString(format): "";
                };
                //2020-04-07 iwai-tamura upd end ------     

                //年齢表示用
                Func<string, string> ageView = (val1) => {
                    if(!string.IsNullOrEmpty(val1)) {
                        return val1+"歳";
                    }
                    return null;
                };

                //チェックボックス表示用
                Func<string, string> checkView = (val1) => {
                    if(!string.IsNullOrEmpty(val1)) {
                        return (val1 == "1")? "☑": "□";
                    }
                    return "□";
                };

                //2020-04-07 iwai-tamura upd str ------
                //西暦表示に変更
                //2020-12-24 iwai-tamura upd str ------

                //2021-04-05 iwai-tamura upd str ------
                switch (dr["自己申告状態区分"].ToString()) {
	                case "0":
	                case "1":
                        cr.SetParameterValue("現在年月日", nengappi(dr["年度"].ToString() + "/12/31", "yyyy年M月d日現在"));
	                    break;
	                case "2":
	                case "3":
                        cr.SetParameterValue("現在年月日", nengappi((Int32.Parse(dr["年度"].ToString()) + 1 ) + "/3/31", "yyyy年M月d日現在"));
                        
	                    break;
	                default:
	                    //上記以外 ReadOnly
	                    break;
	            }
                //cr.SetParameterValue("現在年月日", nengappi(dr["年度"].ToString() + "/12/31", "yyyy年M月d日現在"));
                //2021-04-05 iwai-tamura upd end ------

                cr.SetParameterValue("タイトル年度", nengappi(dr["年度"].ToString() + "/01/01", "yyyy年度"));
                //cr.SetParameterValue("現在年月日", nengappi(dr["年度"].ToString() + "/03/31", "yyyy年M月d日現在"));
                //cr.SetParameterValue("タイトル年度", nengappi(dr["年度"].ToString() + "/04/01", "yyyy年度"));
                //2020-12-24 iwai-tamura upd end ------
                //cr.SetParameterValue("現在年月日", wareki(dr["年度"].ToString() + "/03/31", "ggy年M月d日現在"));
                //cr.SetParameterValue("タイトル年度", wareki(dr["年度"].ToString() + "/04/01", "ggy年度"));
                //2020-04-07 iwai-tamura upd end ------     
                cr.SetParameterValue("氏名", DataConv.IfNull(dr["氏名"].ToString(), ""));
                cr.SetParameterValue("社員番号", DataConv.IfNull(dr["社員番号"].ToString(), ""));
                cr.SetParameterValue("所属", DataConv.IfNull(dr["所属名"].ToString(), ""));
                cr.SetParameterValue("役職名", DataConv.IfNull(dr["役職名"].ToString(), ""));
                cr.SetParameterValue("資格", DataConv.IfNull(dr["資格名"].ToString(), ""));

                //和暦に変換してセット
                //2020-04-07 iwai-tamura upd str ------
                //西暦表示に変更
                cr.SetParameterValue("入社年月日", nengappi(dr["入社年月日"].ToString().Substring(0,4) + "/" + dr["入社年月日"].ToString().Substring(4,2) + "/" + dr["入社年月日"].ToString().Substring(6,2), "yyyy年M月d日"));
                //cr.SetParameterValue("入社年月日", wareki(dr["入社年月日"].ToString().Substring(0,4) + "/" + dr["入社年月日"].ToString().Substring(4,2) + "/" + dr["入社年月日"].ToString().Substring(6,2), "ggy年M月d日"));
                //2020-04-07 iwai-tamura upd end ------     

                //表示用在籍期間・現職経験
                int intYear = 0;
                int intMonth = 0;
                string strView = "";
                intYear = ((int)dr["在籍月数"]/12);
                intMonth = ((int)dr["在籍月数"]%12);
                if (intYear!=0){
                   strView = intYear.ToString("0年");
                }
                strView = strView + intMonth.ToString("0ヶ月");
                cr.SetParameterValue("当社在籍年数", DataConv.IfNull(strView, ""));    

                strView = "";
                intYear = ((int)dr["現職経験月数"]/12);
                intMonth = ((int)dr["現職経験月数"]%12);
                if (intYear!=0){
                   strView = intYear.ToString("0年");
                }
                strView = strView + intMonth.ToString("0ヶ月");
                cr.SetParameterValue("現職経験年数", DataConv.IfNull(strView, ""));    
                
                //2020-04-07 iwai-tamura upd str ------
                //西暦表示に変更
                cr.SetParameterValue("生年月日", nengappi(dr["生年月日"].ToString().Substring(0,4) + "/" + dr["生年月日"].ToString().Substring(4,2) + "/" + dr["生年月日"].ToString().Substring(6,2), "yyyy年M月d日"));
                //cr.SetParameterValue("生年月日", wareki(dr["生年月日"].ToString().Substring(0,4) + "/" + dr["生年月日"].ToString().Substring(4,2) + "/" + dr["生年月日"].ToString().Substring(6,2), "ggy年M月d日"));
                //2020-04-07 iwai-tamura upd end ------     
                cr.SetParameterValue("年齢", DataConv.IfNull(dr["年齢"].ToString()+"歳", ""));

                //家族扶養同居チェック
                for (int i = 1; i <= 8; i++)
                {
                    cr.SetParameterValue("家族構成同居区分_"+i, DataConv.IfNull(checkView(dr["家族構成同居区分_"+i].ToString()), ""));
                    cr.SetParameterValue("家族構成扶養区分_"+i, DataConv.IfNull(checkView(dr["家族構成扶養区分_"+i].ToString()), ""));
                    cr.SetParameterValue("家族構成年齢_"+i, DataConv.IfNull(ageView(dr["家族構成年齢_"+i].ToString()), ""));
                }

                //2021-03-26 iwai-tamura add-str ------
                //一次入力、二次入力時のグレーアウト処理
                bool bolPrimaryBackColor = true;
                bool bolSecondaryBackColor = true;
	            switch (dr["自己申告状態区分"].ToString()) {
	                case "0":
	                case "1":
                        bolPrimaryBackColor = true;
                        bolSecondaryBackColor = false;
	                    break;
	                case "2":
	                case "3":
                        bolPrimaryBackColor = false;
                        bolSecondaryBackColor = true;
	                    break;
	                default:
	                    //上記以外 ReadOnly
	                    break;
	            }
                cr.ReportDefinition.ReportObjects["背景担当職務1"].ObjectFormat.EnableSuppress = bolPrimaryBackColor;
                cr.ReportDefinition.ReportObjects["背景担当職務2"].ObjectFormat.EnableSuppress = bolSecondaryBackColor;
                cr.ReportDefinition.ReportObjects["背景担当職務3"].ObjectFormat.EnableSuppress = bolSecondaryBackColor;
                cr.ReportDefinition.ReportObjects["背景担当職務4"].ObjectFormat.EnableSuppress = bolSecondaryBackColor;
                cr.ReportDefinition.ReportObjects["背景適性能力開発1"].ObjectFormat.EnableSuppress = bolPrimaryBackColor;
                cr.ReportDefinition.ReportObjects["背景適性能力開発2"].ObjectFormat.EnableSuppress = bolSecondaryBackColor;
                cr.ReportDefinition.ReportObjects["背景職務変更配置換1"].ObjectFormat.EnableSuppress = bolPrimaryBackColor;
                cr.ReportDefinition.ReportObjects["背景職務変更配置換2"].ObjectFormat.EnableSuppress = bolPrimaryBackColor;
                cr.ReportDefinition.ReportObjects["背景上司記入"].ObjectFormat.EnableSuppress = bolSecondaryBackColor;
                //2021-03-26 iwai-tamura add-end ------
            
            } catch(Exception ex) {
                // エラー
                nlog.Error(System.Reflection.MethodBase.GetCurrentMethod().Name + " error " + ex.ToString());
                throw;
            } finally {
                //終了
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " end");
            }
        }

        /// <summary>
        /// 自己申告書データを帳票パラメータに直接セット
        /// </summary>
        /// <param name="cr">クリスタルレポート</param>
        /// <param name="dr">自己申告書DataRow</param>
        /// <returns>セット後のクリスタルレポート</returns>
        private void SelfDeclareA12BaseSetting(ref SelfDeclareReportA12 cr, DataRow dr) {
            try {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");
                //datetimeに変換できたら和暦、だめだったら空をreturn
                Func<string, string, string> wareki = delegate(string val, string format) {
                    DateTime dt = new DateTime();
                    return DateTime.TryParse(val, out dt) ? DataConv.Date2Jcal(dt, format) : "";
                };

                //2020-04-07 iwai-tamura upd str ------
                Func<string, string, string> nengappi = delegate(string val, string format) {
                    DateTime dt = new DateTime();
                    return DateTime.TryParse(val, out dt) ? dt.ToString(format): "";
                };
                //2020-04-07 iwai-tamura upd end ------     

                //年齢表示用
                Func<string, string> ageView = (val1) => {
                    if(!string.IsNullOrEmpty(val1)) {
                        return val1+"歳";
                    }
                    return null;
                };

                //チェックボックス表示用
                Func<string, string> checkView = (val1) => {
                    if(!string.IsNullOrEmpty(val1)) {
                        return (val1 == "1")? "☑": "□";
                    }
                    return "□";
                };

                //2020-04-07 iwai-tamura upd str ------
                //西暦表示に変更
                //2020-12-24 iwai-tamura upd str ------

                //2021-04-05 iwai-tamura upd str ------
                switch (dr["自己申告状態区分"].ToString()) {
	                case "0":
	                case "1":
                        cr.SetParameterValue("現在年月日", nengappi(dr["年度"].ToString() + "/12/31", "yyyy年M月d日現在"));
	                    break;
	                case "2":
	                case "3":
                        cr.SetParameterValue("現在年月日", nengappi((Int32.Parse(dr["年度"].ToString()) + 1 ) + "/3/31", "yyyy年M月d日現在"));
                        
	                    break;
	                default:
	                    //上記以外 ReadOnly
	                    break;
	            }
                //cr.SetParameterValue("現在年月日", nengappi(dr["年度"].ToString() + "/12/31", "yyyy年M月d日現在"));
                //2021-04-05 iwai-tamura upd end ------

                cr.SetParameterValue("タイトル年度", nengappi(dr["年度"].ToString() + "/01/01", "yyyy年度"));
                //cr.SetParameterValue("現在年月日", nengappi(dr["年度"].ToString() + "/03/31", "yyyy年M月d日現在"));
                //cr.SetParameterValue("タイトル年度", nengappi(dr["年度"].ToString() + "/04/01", "yyyy年度"));
                //2020-12-24 iwai-tamura upd end ------
                //cr.SetParameterValue("現在年月日", wareki(dr["年度"].ToString() + "/03/31", "ggy年M月d日現在"));
                //cr.SetParameterValue("タイトル年度", wareki(dr["年度"].ToString() + "/04/01", "ggy年度"));
                //2020-04-07 iwai-tamura upd end ------     
                cr.SetParameterValue("氏名", DataConv.IfNull(dr["氏名"].ToString(), ""));
                cr.SetParameterValue("社員番号", DataConv.IfNull(dr["社員番号"].ToString(), ""));
                cr.SetParameterValue("所属", DataConv.IfNull(dr["所属名"].ToString(), ""));
                cr.SetParameterValue("役職名", DataConv.IfNull(dr["役職名"].ToString(), ""));
                cr.SetParameterValue("資格", DataConv.IfNull(dr["資格名"].ToString(), ""));

                //和暦に変換してセット
                //2020-04-07 iwai-tamura upd str ------
                //西暦表示に変更
                cr.SetParameterValue("入社年月日", nengappi(dr["入社年月日"].ToString().Substring(0,4) + "/" + dr["入社年月日"].ToString().Substring(4,2) + "/" + dr["入社年月日"].ToString().Substring(6,2), "yyyy年M月d日"));
                //cr.SetParameterValue("入社年月日", wareki(dr["入社年月日"].ToString().Substring(0,4) + "/" + dr["入社年月日"].ToString().Substring(4,2) + "/" + dr["入社年月日"].ToString().Substring(6,2), "ggy年M月d日"));
                //2020-04-07 iwai-tamura upd end ------     

                //表示用在籍期間・現職経験
                int intYear = 0;
                int intMonth = 0;
                string strView = "";
                intYear = ((int)dr["在籍月数"]/12);
                intMonth = ((int)dr["在籍月数"]%12);
                if (intYear!=0){
                   strView = intYear.ToString("0年");
                }
                strView = strView + intMonth.ToString("0ヶ月");
                cr.SetParameterValue("当社在籍年数", DataConv.IfNull(strView, ""));    

                strView = "";
                intYear = ((int)dr["現職経験月数"]/12);
                intMonth = ((int)dr["現職経験月数"]%12);
                if (intYear!=0){
                   strView = intYear.ToString("0年");
                }
                strView = strView + intMonth.ToString("0ヶ月");
                cr.SetParameterValue("現職経験年数", DataConv.IfNull(strView, ""));    
                
                //2020-04-07 iwai-tamura upd str ------
                //西暦表示に変更
                cr.SetParameterValue("生年月日", nengappi(dr["生年月日"].ToString().Substring(0,4) + "/" + dr["生年月日"].ToString().Substring(4,2) + "/" + dr["生年月日"].ToString().Substring(6,2), "yyyy年M月d日"));
                //cr.SetParameterValue("生年月日", wareki(dr["生年月日"].ToString().Substring(0,4) + "/" + dr["生年月日"].ToString().Substring(4,2) + "/" + dr["生年月日"].ToString().Substring(6,2), "ggy年M月d日"));
                //2020-04-07 iwai-tamura upd end ------     

                
                cr.SetParameterValue("年齢", DataConv.IfNull(dr["年齢"].ToString()+"歳", ""));

                //家族扶養同居チェック
                for (int i = 1; i <= 8; i++)
                {
                    cr.SetParameterValue("家族構成同居区分_"+i, DataConv.IfNull(checkView(dr["家族構成同居区分_"+i].ToString()), ""));
                    cr.SetParameterValue("家族構成扶養区分_"+i, DataConv.IfNull(checkView(dr["家族構成扶養区分_"+i].ToString()), ""));
                    cr.SetParameterValue("家族構成年齢_"+i, DataConv.IfNull(ageView(dr["家族構成年齢_"+i].ToString()), ""));
                }

                //2021-03-26 iwai-tamura add-str ------
                //一次入力、二次入力時のグレーアウト処理
                bool bolPrimaryBackColor = true;
                bool bolSecondaryBackColor = true;
	            switch (dr["自己申告状態区分"].ToString()) {
	                case "0":
	                case "1":
                        bolPrimaryBackColor = true;
                        bolSecondaryBackColor = false;
	                    break;
	                case "2":
	                case "3":
                        bolPrimaryBackColor = false;
                        bolSecondaryBackColor = true;
	                    break;
	                default:
	                    //上記以外 ReadOnly
	                    break;
	            }
                cr.ReportDefinition.ReportObjects["背景担当職務1"].ObjectFormat.EnableSuppress = bolPrimaryBackColor;
                cr.ReportDefinition.ReportObjects["背景担当職務2"].ObjectFormat.EnableSuppress = bolPrimaryBackColor;
                cr.ReportDefinition.ReportObjects["背景担当職務3"].ObjectFormat.EnableSuppress = bolPrimaryBackColor;
                cr.ReportDefinition.ReportObjects["背景担当職務4"].ObjectFormat.EnableSuppress = bolSecondaryBackColor;
                cr.ReportDefinition.ReportObjects["背景担当職務5"].ObjectFormat.EnableSuppress = bolSecondaryBackColor;
                cr.ReportDefinition.ReportObjects["背景担当職務6"].ObjectFormat.EnableSuppress = bolSecondaryBackColor;
                cr.ReportDefinition.ReportObjects["背景適性能力開発1"].ObjectFormat.EnableSuppress = bolPrimaryBackColor;
                cr.ReportDefinition.ReportObjects["背景適性能力開発2"].ObjectFormat.EnableSuppress = bolPrimaryBackColor;
                cr.ReportDefinition.ReportObjects["背景適性能力開発3"].ObjectFormat.EnableSuppress = bolPrimaryBackColor;
                cr.ReportDefinition.ReportObjects["背景職務変更配置換1"].ObjectFormat.EnableSuppress = bolPrimaryBackColor;
                cr.ReportDefinition.ReportObjects["背景職務変更配置換2"].ObjectFormat.EnableSuppress = bolPrimaryBackColor;
                cr.ReportDefinition.ReportObjects["背景能力開発1"].ObjectFormat.EnableSuppress = bolSecondaryBackColor;
                cr.ReportDefinition.ReportObjects["背景能力開発2"].ObjectFormat.EnableSuppress = bolSecondaryBackColor;
                cr.ReportDefinition.ReportObjects["背景能力開発3"].ObjectFormat.EnableSuppress = bolSecondaryBackColor;
                cr.ReportDefinition.ReportObjects["背景上司記入"].ObjectFormat.EnableSuppress = bolSecondaryBackColor;
                //2021-03-26 iwai-tamura add-end ------
            
            } catch(Exception ex) {
                // エラー
                nlog.Error(System.Reflection.MethodBase.GetCurrentMethod().Name + " error " + ex.ToString());
                throw;
            } finally {
                //終了
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " end");
            }
        }


        /// <summary>
        /// 自己申告書データを帳票パラメータに直接セット
        /// </summary>
        /// <param name="cr">クリスタルレポート</param>
        /// <param name="dr">自己申告書DataRow</param>
        /// <returns>セット後のクリスタルレポート</returns>
        private void SelfDeclareA13BaseSetting(ref SelfDeclareReportA13 cr, DataRow dr) {
            try {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");
                //datetimeに変換できたら和暦、だめだったら空をreturn
                Func<string, string, string> wareki = delegate(string val, string format) {
                    DateTime dt = new DateTime();
                    return DateTime.TryParse(val, out dt) ? DataConv.Date2Jcal(dt, format) : "";
                };

                //2020-04-07 iwai-tamura upd str ------
                Func<string, string, string> nengappi = delegate(string val, string format) {
                    DateTime dt = new DateTime();
                    return DateTime.TryParse(val, out dt) ? dt.ToString(format): "";
                };
                //2020-04-07 iwai-tamura upd end ------     

                //年齢表示用
                Func<string, string> ageView = (val1) => {
                    if(!string.IsNullOrEmpty(val1)) {
                        return val1+"歳";
                    }
                    return null;
                };

                //チェックボックス表示用
                Func<string, string> checkView = (val1) => {
                    if(!string.IsNullOrEmpty(val1)) {
                        return (val1 == "1")? "☑": "□";
                    }
                    return "□";
                };

                //2020-04-07 iwai-tamura upd str ------
                //西暦表示に変更
                //2020-12-24 iwai-tamura upd str ------

                //2021-04-05 iwai-tamura upd str ------
                switch (dr["自己申告状態区分"].ToString()) {
	                case "0":
	                case "1":
                        cr.SetParameterValue("現在年月日", nengappi(dr["年度"].ToString() + "/12/31", "yyyy年M月d日現在"));
	                    break;
	                case "2":
	                case "3":
                        cr.SetParameterValue("現在年月日", nengappi((Int32.Parse(dr["年度"].ToString()) + 1 ) + "/3/31", "yyyy年M月d日現在"));
                        
	                    break;
	                default:
	                    //上記以外 ReadOnly
	                    break;
	            }
                //cr.SetParameterValue("現在年月日", nengappi(dr["年度"].ToString() + "/12/31", "yyyy年M月d日現在"));
                //2021-04-05 iwai-tamura upd end ------

                cr.SetParameterValue("タイトル年度", nengappi(dr["年度"].ToString() + "/01/01", "yyyy年度"));
                //cr.SetParameterValue("現在年月日", nengappi(dr["年度"].ToString() + "/03/31", "yyyy年M月d日現在"));
                //cr.SetParameterValue("タイトル年度", nengappi(dr["年度"].ToString() + "/04/01", "yyyy年度"));
                //2020-12-24 iwai-tamura upd end ------
                //cr.SetParameterValue("現在年月日", wareki(dr["年度"].ToString() + "/03/31", "ggy年M月d日現在"));
                //cr.SetParameterValue("タイトル年度", wareki(dr["年度"].ToString() + "/04/01", "ggy年度"));
                //2020-04-07 iwai-tamura upd end ------     
                cr.SetParameterValue("氏名", DataConv.IfNull(dr["氏名"].ToString(), ""));
                cr.SetParameterValue("社員番号", DataConv.IfNull(dr["社員番号"].ToString(), ""));
                cr.SetParameterValue("所属", DataConv.IfNull(dr["所属名"].ToString(), ""));
                cr.SetParameterValue("役職名", DataConv.IfNull(dr["役職名"].ToString(), ""));
                cr.SetParameterValue("資格", DataConv.IfNull(dr["資格名"].ToString(), ""));

                //和暦に変換してセット
                //2020-04-07 iwai-tamura upd str ------
                //西暦表示に変更
                cr.SetParameterValue("入社年月日", nengappi(dr["入社年月日"].ToString().Substring(0,4) + "/" + dr["入社年月日"].ToString().Substring(4,2) + "/" + dr["入社年月日"].ToString().Substring(6,2), "yyyy年M月d日"));
                //cr.SetParameterValue("入社年月日", wareki(dr["入社年月日"].ToString().Substring(0,4) + "/" + dr["入社年月日"].ToString().Substring(4,2) + "/" + dr["入社年月日"].ToString().Substring(6,2), "ggy年M月d日"));
                //2020-04-07 iwai-tamura upd end ------     

                //表示用在籍期間・現職経験
                int intYear = 0;
                int intMonth = 0;
                string strView = "";
                intYear = ((int)dr["在籍月数"]/12);
                intMonth = ((int)dr["在籍月数"]%12);
                if (intYear!=0){
                   strView = intYear.ToString("0年");
                }
                strView = strView + intMonth.ToString("0ヶ月");
                cr.SetParameterValue("当社在籍年数", DataConv.IfNull(strView, ""));    

                strView = "";
                intYear = ((int)dr["現職経験月数"]/12);
                intMonth = ((int)dr["現職経験月数"]%12);
                if (intYear!=0){
                   strView = intYear.ToString("0年");
                }
                strView = strView + intMonth.ToString("0ヶ月");
                cr.SetParameterValue("現職経験年数", DataConv.IfNull(strView, ""));    
                
                //2020-04-07 iwai-tamura upd str ------
                //西暦表示に変更
                cr.SetParameterValue("生年月日", nengappi(dr["生年月日"].ToString().Substring(0,4) + "/" + dr["生年月日"].ToString().Substring(4,2) + "/" + dr["生年月日"].ToString().Substring(6,2), "yyyy年M月d日"));
                //cr.SetParameterValue("生年月日", wareki(dr["生年月日"].ToString().Substring(0,4) + "/" + dr["生年月日"].ToString().Substring(4,2) + "/" + dr["生年月日"].ToString().Substring(6,2), "ggy年M月d日"));
                //2020-04-07 iwai-tamura upd end ------     
                cr.SetParameterValue("年齢", DataConv.IfNull(dr["年齢"].ToString()+"歳", ""));

                //家族扶養同居チェック
                for (int i = 1; i <= 8; i++)
                {
                    cr.SetParameterValue("家族構成同居区分_"+i, DataConv.IfNull(checkView(dr["家族構成同居区分_"+i].ToString()), ""));
                    cr.SetParameterValue("家族構成扶養区分_"+i, DataConv.IfNull(checkView(dr["家族構成扶養区分_"+i].ToString()), ""));
                    cr.SetParameterValue("家族構成年齢_"+i, DataConv.IfNull(ageView(dr["家族構成年齢_"+i].ToString()), ""));
                }

                //2021-03-26 iwai-tamura add-str ------
                //一次入力、二次入力時のグレーアウト処理
                bool bolPrimaryBackColor = true;
                bool bolSecondaryBackColor = true;
	            switch (dr["自己申告状態区分"].ToString()) {
	                case "0":
	                case "1":
                        bolPrimaryBackColor = true;
                        bolSecondaryBackColor = false;
	                    break;
	                case "2":
	                case "3":
                        bolPrimaryBackColor = false;
                        bolSecondaryBackColor = true;
	                    break;
	                default:
	                    //上記以外 ReadOnly
	                    break;
	            }
                cr.ReportDefinition.ReportObjects["背景担当職務1"].ObjectFormat.EnableSuppress = bolPrimaryBackColor;
                cr.ReportDefinition.ReportObjects["背景担当職務2"].ObjectFormat.EnableSuppress = bolPrimaryBackColor;
                cr.ReportDefinition.ReportObjects["背景担当職務3"].ObjectFormat.EnableSuppress = bolPrimaryBackColor;
                cr.ReportDefinition.ReportObjects["背景担当職務4"].ObjectFormat.EnableSuppress = bolSecondaryBackColor;
                cr.ReportDefinition.ReportObjects["背景担当職務5"].ObjectFormat.EnableSuppress = bolSecondaryBackColor;
                cr.ReportDefinition.ReportObjects["背景担当職務6"].ObjectFormat.EnableSuppress = bolSecondaryBackColor;
                cr.ReportDefinition.ReportObjects["背景適性能力開発1"].ObjectFormat.EnableSuppress = bolPrimaryBackColor;
                cr.ReportDefinition.ReportObjects["背景適性能力開発2"].ObjectFormat.EnableSuppress = bolPrimaryBackColor;
                cr.ReportDefinition.ReportObjects["背景適性能力開発3"].ObjectFormat.EnableSuppress = bolPrimaryBackColor;
                cr.ReportDefinition.ReportObjects["背景職務変更配置換1"].ObjectFormat.EnableSuppress = bolPrimaryBackColor;
                cr.ReportDefinition.ReportObjects["背景職務変更配置換2"].ObjectFormat.EnableSuppress = bolPrimaryBackColor;
                cr.ReportDefinition.ReportObjects["背景能力開発1"].ObjectFormat.EnableSuppress = bolSecondaryBackColor;
                cr.ReportDefinition.ReportObjects["背景能力開発2"].ObjectFormat.EnableSuppress = bolSecondaryBackColor;
                cr.ReportDefinition.ReportObjects["背景能力開発3"].ObjectFormat.EnableSuppress = bolSecondaryBackColor;
                cr.ReportDefinition.ReportObjects["背景上司記入"].ObjectFormat.EnableSuppress = bolSecondaryBackColor;
                //2021-03-26 iwai-tamura add-end ------
            } catch(Exception ex) {
                // エラー
                nlog.Error(System.Reflection.MethodBase.GetCurrentMethod().Name + " error " + ex.ToString());
                throw;
            } finally {
                //終了
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " end");
            }
        }


        //2021-12-24 iwai-tamura add-str ------
        /// <summary>
        /// 自己申告書データを帳票パラメータに直接セット
        /// </summary>
        /// <param name="cr">クリスタルレポート</param>
        /// <param name="dr">自己申告書DataRow</param>
        /// <returns>セット後のクリスタルレポート</returns>
        private void SelfDeclareA20BaseSetting(ref SelfDeclareReportA20 cr, DataRow dr) {
            try {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");
                //datetimeに変換できたら和暦、だめだったら空をreturn
                Func<string, string, string> wareki = delegate(string val, string format) {
                    DateTime dt = new DateTime();
                    return DateTime.TryParse(val, out dt) ? DataConv.Date2Jcal(dt, format) : "";
                };

                Func<string, string, string> nengappi = delegate(string val, string format) {
                    DateTime dt = new DateTime();
                    return DateTime.TryParse(val, out dt) ? dt.ToString(format): "";
                };

                //年齢表示用
                Func<string, string> ageView = (val1) => {
                    if(!string.IsNullOrEmpty(val1)) {
                        return val1+"歳";
                    }
                    return null;
                };

                //チェックボックス表示用
                Func<string, string> checkView = (val1) => {
                    if(!string.IsNullOrEmpty(val1)) {
                        return (val1 == "1")? "☑": "□";
                    }
                    return "□";
                };
                cr.SetParameterValue("現在年月日", nengappi(dr["年度"].ToString() + "/12/31", "yyyy年M月d日現在"));

                cr.SetParameterValue("タイトル年度", nengappi(dr["年度"].ToString() + "/01/01", "yyyy年度"));
                cr.SetParameterValue("氏名", DataConv.IfNull(dr["氏名"].ToString(), ""));
                cr.SetParameterValue("社員番号", DataConv.IfNull(dr["社員番号"].ToString(), ""));
                cr.SetParameterValue("所属", DataConv.IfNull(dr["所属名"].ToString(), ""));
                cr.SetParameterValue("役職名", DataConv.IfNull(dr["役職名"].ToString(), ""));
                cr.SetParameterValue("資格", DataConv.IfNull(dr["資格名"].ToString(), ""));

                //和暦に変換してセット
                //西暦表示に変更
                cr.SetParameterValue("入社年月日", nengappi(dr["入社年月日"].ToString().Substring(0,4) + "/" + dr["入社年月日"].ToString().Substring(4,2) + "/" + dr["入社年月日"].ToString().Substring(6,2), "yyyy年M月d日"));

                //表示用在籍期間・現職経験
                int intYear = 0;
                int intMonth = 0;
                string strView = "";
                intYear = ((int)dr["在籍月数"]/12);
                intMonth = ((int)dr["在籍月数"]%12);
                if (intYear!=0){
                   strView = intYear.ToString("0年");
                }
                strView = strView + intMonth.ToString("0ヶ月");
                cr.SetParameterValue("当社在籍年数", DataConv.IfNull(strView, ""));    

                strView = "";
                intYear = ((int)dr["現職経験月数"]/12);
                intMonth = ((int)dr["現職経験月数"]%12);
                if (intYear!=0){
                   strView = intYear.ToString("0年");
                }
                strView = strView + intMonth.ToString("0ヶ月");
                cr.SetParameterValue("現職経験年数", DataConv.IfNull(strView, ""));    
                
                //西暦表示に変更
                cr.SetParameterValue("生年月日", nengappi(dr["生年月日"].ToString().Substring(0,4) + "/" + dr["生年月日"].ToString().Substring(4,2) + "/" + dr["生年月日"].ToString().Substring(6,2), "yyyy年M月d日"));
                cr.SetParameterValue("年齢", DataConv.IfNull(dr["年齢"].ToString()+"歳", ""));

                //家族扶養同居チェック
                for (int i = 1; i <= 8; i++)
                {
                    cr.SetParameterValue("家族構成同居区分_"+i, DataConv.IfNull(checkView(dr["家族構成同居区分_"+i].ToString()), ""));
                    cr.SetParameterValue("家族構成扶養区分_"+i, DataConv.IfNull(checkView(dr["家族構成扶養区分_"+i].ToString()), ""));
                    cr.SetParameterValue("家族構成年齢_"+i, DataConv.IfNull(ageView(dr["家族構成年齢_"+i].ToString()), ""));
                }

            } catch(Exception ex) {
                // エラー
                nlog.Error(System.Reflection.MethodBase.GetCurrentMethod().Name + " error " + ex.ToString());
                throw;
            } finally {
                //終了
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " end");
            }
        }

        
        /// <summary>
        /// 自己申告書データを帳票パラメータに直接セット
        /// </summary>
        /// <param name="cr">クリスタルレポート</param>
        /// <param name="dr">自己申告書DataRow</param>
        /// <returns>セット後のクリスタルレポート</returns>
        private void SelfDeclareB20BaseSetting(ref SelfDeclareReportB20 cr, DataRow dr) {
            try {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");
                //datetimeに変換できたら和暦、だめだったら空をreturn
                Func<string, string, string> wareki = delegate(string val, string format) {
                    DateTime dt = new DateTime();
                    return DateTime.TryParse(val, out dt) ? DataConv.Date2Jcal(dt, format) : "";
                };

                Func<string, string, string> nengappi = delegate(string val, string format) {
                    DateTime dt = new DateTime();
                    return DateTime.TryParse(val, out dt) ? dt.ToString(format): "";
                };

                //年齢表示用
                Func<string, string> ageView = (val1) => {
                    if(!string.IsNullOrEmpty(val1)) {
                        return val1+"歳";
                    }
                    return null;
                };

                //チェックボックス表示用
                Func<string, string> checkView = (val1) => {
                    if(!string.IsNullOrEmpty(val1)) {
                        return (val1 == "1")? "☑": "□";
                    }
                    return "□";
                };
                cr.SetParameterValue("現在年月日", nengappi(dr["年度"].ToString() + "/12/31", "yyyy年M月d日現在"));

                cr.SetParameterValue("タイトル年度", nengappi(dr["年度"].ToString() + "/01/01", "yyyy年度"));
                cr.SetParameterValue("氏名", DataConv.IfNull(dr["氏名"].ToString(), ""));
                cr.SetParameterValue("社員番号", DataConv.IfNull(dr["社員番号"].ToString(), ""));
                cr.SetParameterValue("所属", DataConv.IfNull(dr["所属名"].ToString(), ""));
                cr.SetParameterValue("役職名", DataConv.IfNull(dr["役職名"].ToString(), ""));
                cr.SetParameterValue("資格", DataConv.IfNull(dr["資格名"].ToString(), ""));

                //和暦に変換してセット
                //西暦表示に変更
                cr.SetParameterValue("入社年月日", nengappi(dr["入社年月日"].ToString().Substring(0,4) + "/" + dr["入社年月日"].ToString().Substring(4,2) + "/" + dr["入社年月日"].ToString().Substring(6,2), "yyyy年M月d日"));

                //表示用在籍期間・現職経験
                int intYear = 0;
                int intMonth = 0;
                string strView = "";
                intYear = ((int)dr["在籍月数"]/12);
                intMonth = ((int)dr["在籍月数"]%12);
                if (intYear!=0){
                   strView = intYear.ToString("0年");
                }
                strView = strView + intMonth.ToString("0ヶ月");
                cr.SetParameterValue("当社在籍年数", DataConv.IfNull(strView, ""));    

                strView = "";
                intYear = ((int)dr["現職経験月数"]/12);
                intMonth = ((int)dr["現職経験月数"]%12);
                if (intYear!=0){
                   strView = intYear.ToString("0年");
                }
                strView = strView + intMonth.ToString("0ヶ月");
                cr.SetParameterValue("現職経験年数", DataConv.IfNull(strView, ""));    
                
                //西暦表示に変更
                cr.SetParameterValue("生年月日", nengappi(dr["生年月日"].ToString().Substring(0,4) + "/" + dr["生年月日"].ToString().Substring(4,2) + "/" + dr["生年月日"].ToString().Substring(6,2), "yyyy年M月d日"));
                cr.SetParameterValue("年齢", DataConv.IfNull(dr["年齢"].ToString()+"歳", ""));

                //家族扶養同居チェック
                for (int i = 1; i <= 8; i++)
                {
                    cr.SetParameterValue("家族構成同居区分_"+i, DataConv.IfNull(checkView(dr["家族構成同居区分_"+i].ToString()), ""));
                    cr.SetParameterValue("家族構成扶養区分_"+i, DataConv.IfNull(checkView(dr["家族構成扶養区分_"+i].ToString()), ""));
                    cr.SetParameterValue("家族構成年齢_"+i, DataConv.IfNull(ageView(dr["家族構成年齢_"+i].ToString()), ""));
                }

            } catch(Exception ex) {
                // エラー
                nlog.Error(System.Reflection.MethodBase.GetCurrentMethod().Name + " error " + ex.ToString());
                throw;
            } finally {
                //終了
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " end");
            }
        }        
        

        
        /// <summary>
        /// 自己申告書データを帳票パラメータに直接セット
        /// </summary>
        /// <param name="cr">クリスタルレポート</param>
        /// <param name="dr">自己申告書DataRow</param>
        /// <returns>セット後のクリスタルレポート</returns>
        private void SelfDeclareC20BaseSetting(ref SelfDeclareReportC20 cr, DataRow dr) {
            try {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");
                //datetimeに変換できたら和暦、だめだったら空をreturn
                Func<string, string, string> wareki = delegate(string val, string format) {
                    DateTime dt = new DateTime();
                    return DateTime.TryParse(val, out dt) ? DataConv.Date2Jcal(dt, format) : "";
                };

                Func<string, string, string> nengappi = delegate(string val, string format) {
                    DateTime dt = new DateTime();
                    return DateTime.TryParse(val, out dt) ? dt.ToString(format): "";
                };

                //年齢表示用
                Func<string, string> ageView = (val1) => {
                    if(!string.IsNullOrEmpty(val1)) {
                        return val1+"歳";
                    }
                    return null;
                };

                //チェックボックス表示用
                Func<string, string> checkView = (val1) => {
                    if(!string.IsNullOrEmpty(val1)) {
                        return (val1 == "1")? "☑": "□";
                    }
                    return "□";
                };
                cr.SetParameterValue("現在年月日", nengappi(dr["年度"].ToString() + "/12/31", "yyyy年M月d日現在"));

                cr.SetParameterValue("タイトル年度", nengappi(dr["年度"].ToString() + "/01/01", "yyyy年度"));
                cr.SetParameterValue("氏名", DataConv.IfNull(dr["氏名"].ToString(), ""));
                cr.SetParameterValue("社員番号", DataConv.IfNull(dr["社員番号"].ToString(), ""));
                cr.SetParameterValue("所属", DataConv.IfNull(dr["所属名"].ToString(), ""));
                cr.SetParameterValue("役職名", DataConv.IfNull(dr["役職名"].ToString(), ""));
                cr.SetParameterValue("資格", DataConv.IfNull(dr["資格名"].ToString(), ""));

                //和暦に変換してセット
                //西暦表示に変更
                cr.SetParameterValue("入社年月日", nengappi(dr["入社年月日"].ToString().Substring(0,4) + "/" + dr["入社年月日"].ToString().Substring(4,2) + "/" + dr["入社年月日"].ToString().Substring(6,2), "yyyy年M月d日"));

                //表示用在籍期間・現職経験
                int intYear = 0;
                int intMonth = 0;
                string strView = "";
                intYear = ((int)dr["在籍月数"]/12);
                intMonth = ((int)dr["在籍月数"]%12);
                if (intYear!=0){
                   strView = intYear.ToString("0年");
                }
                strView = strView + intMonth.ToString("0ヶ月");
                cr.SetParameterValue("当社在籍年数", DataConv.IfNull(strView, ""));    

                strView = "";
                intYear = ((int)dr["現職経験月数"]/12);
                intMonth = ((int)dr["現職経験月数"]%12);
                if (intYear!=0){
                   strView = intYear.ToString("0年");
                }
                strView = strView + intMonth.ToString("0ヶ月");
                cr.SetParameterValue("現職経験年数", DataConv.IfNull(strView, ""));    
                
                //西暦表示に変更
                cr.SetParameterValue("生年月日", nengappi(dr["生年月日"].ToString().Substring(0,4) + "/" + dr["生年月日"].ToString().Substring(4,2) + "/" + dr["生年月日"].ToString().Substring(6,2), "yyyy年M月d日"));
                cr.SetParameterValue("年齢", DataConv.IfNull(dr["年齢"].ToString()+"歳", ""));

                //家族扶養同居チェック
                for (int i = 1; i <= 8; i++)
                {
                    cr.SetParameterValue("家族構成同居区分_"+i, DataConv.IfNull(checkView(dr["家族構成同居区分_"+i].ToString()), ""));
                    cr.SetParameterValue("家族構成扶養区分_"+i, DataConv.IfNull(checkView(dr["家族構成扶養区分_"+i].ToString()), ""));
                    cr.SetParameterValue("家族構成年齢_"+i, DataConv.IfNull(ageView(dr["家族構成年齢_"+i].ToString()), ""));
                }

            } catch(Exception ex) {
                // エラー
                nlog.Error(System.Reflection.MethodBase.GetCurrentMethod().Name + " error " + ex.ToString());
                throw;
            } finally {
                //終了
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " end");
            }
        }
        //2021-12-24 iwai-tamura add-end ------


        /// <summary>
        /// 自己申告書データを帳票パラメータに直接セット
        /// </summary>
        /// <param name="cr">クリスタルレポート</param>
        /// <param name="dr">自己申告書DataRow</param>
        /// <returns>セット後のクリスタルレポート</returns>
        private void SelfDeclareB11BaseSetting(ref SelfDeclareReportB11 cr, DataRow dr) {
            try {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");
                //datetimeに変換できたら和暦、だめだったら空をreturn
                Func<string, string, string> wareki = delegate(string val, string format) {
                    DateTime dt = new DateTime();
                    return DateTime.TryParse(val, out dt) ? DataConv.Date2Jcal(dt, format) : "";
                };

                //2020-04-07 iwai-tamura upd str ------
                Func<string, string, string> nengappi = delegate(string val, string format) {
                    DateTime dt = new DateTime();
                    return DateTime.TryParse(val, out dt) ? dt.ToString(format): "";
                };
                //2020-04-07 iwai-tamura upd end ------     

                //年齢表示用
                Func<string, string> ageView = (val1) => {
                    if(!string.IsNullOrEmpty(val1)) {
                        return val1+"歳";
                    }
                    return null;
                };

                //チェックボックス表示用
                Func<string, string> checkView = (val1) => {
                    if(!string.IsNullOrEmpty(val1)) {
                        return (val1 == "1")? "☑": "□";
                    }
                    return "□";
                };

                //2020-04-07 iwai-tamura upd str ------
                //西暦表示に変更
                //2020-12-24 iwai-tamura upd str ------

                //2021-04-05 iwai-tamura upd str ------
                switch (dr["自己申告状態区分"].ToString()) {
	                case "0":
	                case "1":
                        cr.SetParameterValue("現在年月日", nengappi(dr["年度"].ToString() + "/12/31", "yyyy年M月d日現在"));
	                    break;
	                case "2":
	                case "3":
                        cr.SetParameterValue("現在年月日", nengappi((Int32.Parse(dr["年度"].ToString()) + 1 ) + "/3/31", "yyyy年M月d日現在"));
                        
	                    break;
	                default:
	                    //上記以外 ReadOnly
	                    break;
	            }
                //cr.SetParameterValue("現在年月日", nengappi(dr["年度"].ToString() + "/12/31", "yyyy年M月d日現在"));
                //2021-04-05 iwai-tamura upd end ------

                cr.SetParameterValue("タイトル年度", nengappi(dr["年度"].ToString() + "/01/01", "yyyy年度"));
                //cr.SetParameterValue("現在年月日", nengappi(dr["年度"].ToString() + "/03/31", "yyyy年M月d日現在"));
                //cr.SetParameterValue("タイトル年度", nengappi(dr["年度"].ToString() + "/04/01", "yyyy年度"));
                //2020-12-24 iwai-tamura upd end ------
                //cr.SetParameterValue("現在年月日", wareki(dr["年度"].ToString() + "/03/31", "ggy年M月d日現在"));
                //cr.SetParameterValue("タイトル年度", wareki(dr["年度"].ToString() + "/04/01", "ggy年度"));
                //2020-04-07 iwai-tamura upd end ------     
                cr.SetParameterValue("氏名", DataConv.IfNull(dr["氏名"].ToString(), ""));
                cr.SetParameterValue("社員番号", DataConv.IfNull(dr["社員番号"].ToString(), ""));
                cr.SetParameterValue("所属", DataConv.IfNull(dr["所属名"].ToString(), ""));
                cr.SetParameterValue("役職名", DataConv.IfNull(dr["役職名"].ToString(), ""));
                cr.SetParameterValue("資格", DataConv.IfNull(dr["資格名"].ToString(), ""));

                //和暦に変換してセット
                //2020-04-07 iwai-tamura upd str ------
                //西暦表示に変更
                cr.SetParameterValue("入社年月日", nengappi(dr["入社年月日"].ToString().Substring(0,4) + "/" + dr["入社年月日"].ToString().Substring(4,2) + "/" + dr["入社年月日"].ToString().Substring(6,2), "yyyy年M月d日"));
                //cr.SetParameterValue("入社年月日", wareki(dr["入社年月日"].ToString().Substring(0,4) + "/" + dr["入社年月日"].ToString().Substring(4,2) + "/" + dr["入社年月日"].ToString().Substring(6,2), "ggy年M月d日"));
                //2020-04-07 iwai-tamura upd end ------     

                //表示用在籍期間・現職経験
                int intYear = 0;
                int intMonth = 0;
                string strView = "";
                intYear = ((int)dr["在籍月数"]/12);
                intMonth = ((int)dr["在籍月数"]%12);
                if (intYear!=0){
                   strView = intYear.ToString("0年");
                }
                strView = strView + intMonth.ToString("0ヶ月");
                cr.SetParameterValue("当社在籍年数", DataConv.IfNull(strView, ""));    

                strView = "";
                intYear = ((int)dr["現職経験月数"]/12);
                intMonth = ((int)dr["現職経験月数"]%12);
                if (intYear!=0){
                   strView = intYear.ToString("0年");
                }
                strView = strView + intMonth.ToString("0ヶ月");
                cr.SetParameterValue("現職経験年数", DataConv.IfNull(strView, ""));    
                
                //2020-04-07 iwai-tamura upd str ------
                //西暦表示に変更
                cr.SetParameterValue("生年月日", nengappi(dr["生年月日"].ToString().Substring(0,4) + "/" + dr["生年月日"].ToString().Substring(4,2) + "/" + dr["生年月日"].ToString().Substring(6,2), "yyyy年M月d日"));
                //cr.SetParameterValue("生年月日", wareki(dr["生年月日"].ToString().Substring(0,4) + "/" + dr["生年月日"].ToString().Substring(4,2) + "/" + dr["生年月日"].ToString().Substring(6,2), "ggy年M月d日"));
                //2020-04-07 iwai-tamura upd end ------     
                cr.SetParameterValue("年齢", DataConv.IfNull(dr["年齢"].ToString()+"歳", ""));

                //家族扶養同居チェック
                for (int i = 1; i <= 8; i++)
                {
                    cr.SetParameterValue("家族構成同居区分_"+i, DataConv.IfNull(checkView(dr["家族構成同居区分_"+i].ToString()), ""));
                    cr.SetParameterValue("家族構成扶養区分_"+i, DataConv.IfNull(checkView(dr["家族構成扶養区分_"+i].ToString()), ""));
                    cr.SetParameterValue("家族構成年齢_"+i, DataConv.IfNull(ageView(dr["家族構成年齢_"+i].ToString()), ""));
                }

                //2021-03-26 iwai-tamura add-str ------
                //一次入力、二次入力時のグレーアウト処理
                bool bolPrimaryBackColor = true;
                bool bolSecondaryBackColor = true;
	            switch (dr["自己申告状態区分"].ToString()) {
	                case "0":
	                case "1":
                        bolPrimaryBackColor = true;
                        bolSecondaryBackColor = false;
	                    break;
	                case "2":
	                case "3":
                        bolPrimaryBackColor = false;
                        bolSecondaryBackColor = true;
	                    break;
	                default:
	                    //上記以外 ReadOnly
	                    break;
	            }
                cr.ReportDefinition.ReportObjects["背景担当職務1"].ObjectFormat.EnableSuppress = bolSecondaryBackColor;
                cr.ReportDefinition.ReportObjects["背景担当職務2"].ObjectFormat.EnableSuppress = bolSecondaryBackColor;
                cr.ReportDefinition.ReportObjects["背景担当職務3"].ObjectFormat.EnableSuppress = bolSecondaryBackColor;
                cr.ReportDefinition.ReportObjects["背景適性能力開発1"].ObjectFormat.EnableSuppress = bolPrimaryBackColor;
                cr.ReportDefinition.ReportObjects["背景適性能力開発2"].ObjectFormat.EnableSuppress = bolPrimaryBackColor;
                cr.ReportDefinition.ReportObjects["背景職務変更配置換1"].ObjectFormat.EnableSuppress = bolPrimaryBackColor;
                cr.ReportDefinition.ReportObjects["背景職務変更配置換2"].ObjectFormat.EnableSuppress = bolPrimaryBackColor;
                cr.ReportDefinition.ReportObjects["背景能力開発1"].ObjectFormat.EnableSuppress = bolSecondaryBackColor;
                cr.ReportDefinition.ReportObjects["背景能力開発2"].ObjectFormat.EnableSuppress = bolSecondaryBackColor;
                cr.ReportDefinition.ReportObjects["背景能力開発3"].ObjectFormat.EnableSuppress = bolSecondaryBackColor;
                cr.ReportDefinition.ReportObjects["背景上司記入"].ObjectFormat.EnableSuppress = bolSecondaryBackColor;
                //2021-03-26 iwai-tamura add-end ------
            } catch(Exception ex) {
                // エラー
                nlog.Error(System.Reflection.MethodBase.GetCurrentMethod().Name + " error " + ex.ToString());
                throw;
            } finally {
                //終了
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " end");
            }
        }


        /// <summary>
        /// 自己申告書データを帳票パラメータに直接セット
        /// </summary>
        /// <param name="cr">クリスタルレポート</param>
        /// <param name="dr">自己申告書DataRow</param>
        /// <returns>セット後のクリスタルレポート</returns>
        private void SelfDeclareB12BaseSetting(ref SelfDeclareReportB12 cr, DataRow dr) {
            try {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");
                //datetimeに変換できたら和暦、だめだったら空をreturn
                Func<string, string, string> wareki = delegate(string val, string format) {
                    DateTime dt = new DateTime();
                    return DateTime.TryParse(val, out dt) ? DataConv.Date2Jcal(dt, format) : "";
                };

                //2020-04-07 iwai-tamura upd str ------
                Func<string, string, string> nengappi = delegate(string val, string format) {
                    DateTime dt = new DateTime();
                    return DateTime.TryParse(val, out dt) ? dt.ToString(format): "";
                };
                //2020-04-07 iwai-tamura upd end ------     

                //年齢表示用
                Func<string, string> ageView = (val1) => {
                    if(!string.IsNullOrEmpty(val1)) {
                        return val1+"歳";
                    }
                    return null;
                };

                //チェックボックス表示用
                Func<string, string> checkView = (val1) => {
                    if(!string.IsNullOrEmpty(val1)) {
                        return (val1 == "1")? "☑": "□";
                    }
                    return "□";
                };

                //2020-04-07 iwai-tamura upd str ------
                //西暦表示に変更
                //2020-12-24 iwai-tamura upd str ------

                //2021-04-05 iwai-tamura upd str ------
                switch (dr["自己申告状態区分"].ToString()) {
	                case "0":
	                case "1":
                        cr.SetParameterValue("現在年月日", nengappi(dr["年度"].ToString() + "/12/31", "yyyy年M月d日現在"));
	                    break;
	                case "2":
	                case "3":
                        cr.SetParameterValue("現在年月日", nengappi((Int32.Parse(dr["年度"].ToString()) + 1 ) + "/3/31", "yyyy年M月d日現在"));
                        
	                    break;
	                default:
	                    //上記以外 ReadOnly
	                    break;
	            }
                //cr.SetParameterValue("現在年月日", nengappi(dr["年度"].ToString() + "/12/31", "yyyy年M月d日現在"));
                //2021-04-05 iwai-tamura upd end ------

                cr.SetParameterValue("タイトル年度", nengappi(dr["年度"].ToString() + "/01/01", "yyyy年度"));
                //cr.SetParameterValue("現在年月日", nengappi(dr["年度"].ToString() + "/03/31", "yyyy年M月d日現在"));
                //cr.SetParameterValue("タイトル年度", nengappi(dr["年度"].ToString() + "/04/01", "yyyy年度"));
                //2020-12-24 iwai-tamura upd end ------
                //cr.SetParameterValue("現在年月日", wareki(dr["年度"].ToString() + "/03/31", "ggy年M月d日現在"));
                //cr.SetParameterValue("タイトル年度", wareki(dr["年度"].ToString() + "/04/01", "ggy年度"));
                //2020-04-07 iwai-tamura upd end ------     
                cr.SetParameterValue("氏名", DataConv.IfNull(dr["氏名"].ToString(), ""));
                cr.SetParameterValue("社員番号", DataConv.IfNull(dr["社員番号"].ToString(), ""));
                cr.SetParameterValue("所属", DataConv.IfNull(dr["所属名"].ToString(), ""));
                cr.SetParameterValue("役職名", DataConv.IfNull(dr["役職名"].ToString(), ""));
                cr.SetParameterValue("資格", DataConv.IfNull(dr["資格名"].ToString(), ""));

                //和暦に変換してセット
                //2020-04-07 iwai-tamura upd str ------
                //西暦表示に変更
                cr.SetParameterValue("入社年月日", nengappi(dr["入社年月日"].ToString().Substring(0,4) + "/" + dr["入社年月日"].ToString().Substring(4,2) + "/" + dr["入社年月日"].ToString().Substring(6,2), "yyyy年M月d日"));
                //cr.SetParameterValue("入社年月日", wareki(dr["入社年月日"].ToString().Substring(0,4) + "/" + dr["入社年月日"].ToString().Substring(4,2) + "/" + dr["入社年月日"].ToString().Substring(6,2), "ggy年M月d日"));
                //2020-04-07 iwai-tamura upd end ------     

                //表示用在籍期間・現職経験
                int intYear = 0;
                int intMonth = 0;
                string strView = "";
                intYear = ((int)dr["在籍月数"]/12);
                intMonth = ((int)dr["在籍月数"]%12);
                if (intYear!=0){
                   strView = intYear.ToString("0年");
                }
                strView = strView + intMonth.ToString("0ヶ月");
                cr.SetParameterValue("当社在籍年数", DataConv.IfNull(strView, ""));    

                strView = "";
                intYear = ((int)dr["現職経験月数"]/12);
                intMonth = ((int)dr["現職経験月数"]%12);
                if (intYear!=0){
                   strView = intYear.ToString("0年");
                }
                strView = strView + intMonth.ToString("0ヶ月");
                cr.SetParameterValue("現職経験年数", DataConv.IfNull(strView, ""));    
                
                //2020-04-07 iwai-tamura upd str ------
                //西暦表示に変更
                cr.SetParameterValue("生年月日", nengappi(dr["生年月日"].ToString().Substring(0,4) + "/" + dr["生年月日"].ToString().Substring(4,2) + "/" + dr["生年月日"].ToString().Substring(6,2), "yyyy年M月d日"));
                //cr.SetParameterValue("生年月日", wareki(dr["生年月日"].ToString().Substring(0,4) + "/" + dr["生年月日"].ToString().Substring(4,2) + "/" + dr["生年月日"].ToString().Substring(6,2), "ggy年M月d日"));
                //2020-04-07 iwai-tamura upd end ------     
                cr.SetParameterValue("年齢", DataConv.IfNull(dr["年齢"].ToString()+"歳", ""));

                //家族扶養同居チェック
                for (int i = 1; i <= 8; i++)
                {
                    cr.SetParameterValue("家族構成同居区分_"+i, DataConv.IfNull(checkView(dr["家族構成同居区分_"+i].ToString()), ""));
                    cr.SetParameterValue("家族構成扶養区分_"+i, DataConv.IfNull(checkView(dr["家族構成扶養区分_"+i].ToString()), ""));
                    cr.SetParameterValue("家族構成年齢_"+i, DataConv.IfNull(ageView(dr["家族構成年齢_"+i].ToString()), ""));
                }

                //2021-03-26 iwai-tamura add-str ------
                //一次入力、二次入力時のグレーアウト処理
                bool bolPrimaryBackColor = true;
                bool bolSecondaryBackColor = true;
	            switch (dr["自己申告状態区分"].ToString()) {
	                case "0":
	                case "1":
                        bolPrimaryBackColor = true;
                        bolSecondaryBackColor = false;
	                    break;
	                case "2":
	                case "3":
                        bolPrimaryBackColor = false;
                        bolSecondaryBackColor = true;
	                    break;
	                default:
	                    //上記以外 ReadOnly
	                    break;
	            }
                cr.ReportDefinition.ReportObjects["背景担当職務1"].ObjectFormat.EnableSuppress = bolSecondaryBackColor;
                cr.ReportDefinition.ReportObjects["背景担当職務2"].ObjectFormat.EnableSuppress = bolSecondaryBackColor;
                cr.ReportDefinition.ReportObjects["背景担当職務3"].ObjectFormat.EnableSuppress = bolSecondaryBackColor;
                cr.ReportDefinition.ReportObjects["背景適性能力開発1"].ObjectFormat.EnableSuppress = bolPrimaryBackColor;
                cr.ReportDefinition.ReportObjects["背景適性能力開発2"].ObjectFormat.EnableSuppress = bolPrimaryBackColor;
                cr.ReportDefinition.ReportObjects["背景職務変更配置換1"].ObjectFormat.EnableSuppress = bolPrimaryBackColor;
                cr.ReportDefinition.ReportObjects["背景職務変更配置換2"].ObjectFormat.EnableSuppress = bolPrimaryBackColor;
                cr.ReportDefinition.ReportObjects["背景能力開発1"].ObjectFormat.EnableSuppress = bolSecondaryBackColor;
                cr.ReportDefinition.ReportObjects["背景能力開発2"].ObjectFormat.EnableSuppress = bolSecondaryBackColor;
                cr.ReportDefinition.ReportObjects["背景能力開発3"].ObjectFormat.EnableSuppress = bolSecondaryBackColor;
                cr.ReportDefinition.ReportObjects["背景上司記入"].ObjectFormat.EnableSuppress = bolSecondaryBackColor;
                //2021-03-26 iwai-tamura add-end ------
            } catch(Exception ex) {
                // エラー
                nlog.Error(System.Reflection.MethodBase.GetCurrentMethod().Name + " error " + ex.ToString());
                throw;
            } finally {
                //終了
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " end");
            }
        }


        /// <summary>
        /// 自己申告書データを帳票パラメータに直接セット
        /// </summary>
        /// <param name="cr">クリスタルレポート</param>
        /// <param name="dr">自己申告書DataRow</param>
        /// <returns>セット後のクリスタルレポート</returns>
        private void SelfDeclareC11BaseSetting(ref SelfDeclareReportC11 cr, DataRow dr) {
            try {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");
                //datetimeに変換できたら和暦、だめだったら空をreturn
                Func<string, string, string> wareki = delegate(string val, string format) {
                    DateTime dt = new DateTime();
                    return DateTime.TryParse(val, out dt) ? DataConv.Date2Jcal(dt, format) : "";
                };

                //2020-04-07 iwai-tamura upd str ------
                Func<string, string, string> nengappi = delegate(string val, string format) {
                    DateTime dt = new DateTime();
                    return DateTime.TryParse(val, out dt) ? dt.ToString(format): "";
                };
                //2020-04-07 iwai-tamura upd end ------     

                //年齢表示用
                Func<string, string> ageView = (val1) => {
                    if(!string.IsNullOrEmpty(val1)) {
                        return val1+"歳";
                    }
                    return null;
                };

                //チェックボックス表示用
                Func<string, string> checkView = (val1) => {
                    if(!string.IsNullOrEmpty(val1)) {
                        return (val1 == "1")? "☑": "□";
                    }
                    return "□";
                };

                //2020-04-07 iwai-tamura upd str ------
                //西暦表示に変更
                //2020-12-24 iwai-tamura upd str ------

                //2021-04-05 iwai-tamura upd str ------
                switch (dr["自己申告状態区分"].ToString()) {
	                case "0":
	                case "1":
                        cr.SetParameterValue("現在年月日", nengappi(dr["年度"].ToString() + "/12/31", "yyyy年M月d日現在"));
	                    break;
	                case "2":
	                case "3":
                        cr.SetParameterValue("現在年月日", nengappi((Int32.Parse(dr["年度"].ToString()) + 1 ) + "/3/31", "yyyy年M月d日現在"));
                        
	                    break;
	                default:
	                    //上記以外 ReadOnly
	                    break;
	            }
                //cr.SetParameterValue("現在年月日", nengappi(dr["年度"].ToString() + "/12/31", "yyyy年M月d日現在"));
                //2021-04-05 iwai-tamura upd end ------

                cr.SetParameterValue("タイトル年度", nengappi(dr["年度"].ToString() + "/01/01", "yyyy年度"));
                //cr.SetParameterValue("現在年月日", nengappi(dr["年度"].ToString() + "/03/31", "yyyy年M月d日現在"));
                //cr.SetParameterValue("タイトル年度", nengappi(dr["年度"].ToString() + "/04/01", "yyyy年度"));
                //2020-12-24 iwai-tamura upd end ------
                //cr.SetParameterValue("現在年月日", wareki(dr["年度"].ToString() + "/03/31", "ggy年M月d日現在"));
                //cr.SetParameterValue("タイトル年度", wareki(dr["年度"].ToString() + "/04/01", "ggy年度"));
                //2020-04-07 iwai-tamura upd end ------     
                cr.SetParameterValue("氏名", DataConv.IfNull(dr["氏名"].ToString(), ""));
                cr.SetParameterValue("社員番号", DataConv.IfNull(dr["社員番号"].ToString(), ""));
                cr.SetParameterValue("所属", DataConv.IfNull(dr["所属名"].ToString(), ""));
                cr.SetParameterValue("役職名", DataConv.IfNull(dr["役職名"].ToString(), ""));
                cr.SetParameterValue("資格", DataConv.IfNull(dr["資格名"].ToString(), ""));

                //和暦に変換してセット
                //2020-04-07 iwai-tamura upd str ------
                //西暦表示に変更
                cr.SetParameterValue("入社年月日", nengappi(dr["入社年月日"].ToString().Substring(0,4) + "/" + dr["入社年月日"].ToString().Substring(4,2) + "/" + dr["入社年月日"].ToString().Substring(6,2), "yyyy年M月d日"));
                //cr.SetParameterValue("入社年月日", wareki(dr["入社年月日"].ToString().Substring(0,4) + "/" + dr["入社年月日"].ToString().Substring(4,2) + "/" + dr["入社年月日"].ToString().Substring(6,2), "ggy年M月d日"));
                //2020-04-07 iwai-tamura upd end ------     

                //表示用在籍期間・現職経験
                int intYear = 0;
                int intMonth = 0;
                string strView = "";
                intYear = ((int)dr["在籍月数"]/12);
                intMonth = ((int)dr["在籍月数"]%12);
                if (intYear!=0){
                   strView = intYear.ToString("0年");
                }
                strView = strView + intMonth.ToString("0ヶ月");
                cr.SetParameterValue("当社在籍年数", DataConv.IfNull(strView, ""));    

                strView = "";
                intYear = ((int)dr["現職経験月数"]/12);
                intMonth = ((int)dr["現職経験月数"]%12);
                if (intYear!=0){
                   strView = intYear.ToString("0年");
                }
                strView = strView + intMonth.ToString("0ヶ月");
                cr.SetParameterValue("現職経験年数", DataConv.IfNull(strView, ""));    
                
                //2020-04-07 iwai-tamura upd str ------
                //西暦表示に変更
                cr.SetParameterValue("生年月日", nengappi(dr["生年月日"].ToString().Substring(0,4) + "/" + dr["生年月日"].ToString().Substring(4,2) + "/" + dr["生年月日"].ToString().Substring(6,2), "yyyy年M月d日"));
                //cr.SetParameterValue("生年月日", wareki(dr["生年月日"].ToString().Substring(0,4) + "/" + dr["生年月日"].ToString().Substring(4,2) + "/" + dr["生年月日"].ToString().Substring(6,2), "ggy年M月d日"));
                //2020-04-07 iwai-tamura upd end ------     
                cr.SetParameterValue("年齢", DataConv.IfNull(dr["年齢"].ToString()+"歳", ""));

                //家族扶養同居チェック
                for (int i = 1; i <= 8; i++)
                {
                    cr.SetParameterValue("家族構成同居区分_"+i, DataConv.IfNull(checkView(dr["家族構成同居区分_"+i].ToString()), ""));
                    cr.SetParameterValue("家族構成扶養区分_"+i, DataConv.IfNull(checkView(dr["家族構成扶養区分_"+i].ToString()), ""));
                    cr.SetParameterValue("家族構成年齢_"+i, DataConv.IfNull(ageView(dr["家族構成年齢_"+i].ToString()), ""));
                }

                //2021-03-26 iwai-tamura add-str ------
                //一次入力、二次入力時のグレーアウト処理
                bool bolPrimaryBackColor = true;
                bool bolSecondaryBackColor = true;
	            switch (dr["自己申告状態区分"].ToString()) {
	                case "0":
	                case "1":
                        bolPrimaryBackColor = true;
                        bolSecondaryBackColor = false;
	                    break;
	                case "2":
	                case "3":
                        bolPrimaryBackColor = false;
                        bolSecondaryBackColor = true;
	                    break;
	                default:
	                    //上記以外 ReadOnly
	                    break;
	            }
                cr.ReportDefinition.ReportObjects["背景職務変更配置換1"].ObjectFormat.EnableSuppress = bolPrimaryBackColor;
                cr.ReportDefinition.ReportObjects["背景職務変更配置換2"].ObjectFormat.EnableSuppress = bolPrimaryBackColor;
                cr.ReportDefinition.ReportObjects["背景担当職務1"].ObjectFormat.EnableSuppress = bolSecondaryBackColor;
                cr.ReportDefinition.ReportObjects["背景担当職務2"].ObjectFormat.EnableSuppress = bolSecondaryBackColor;
                cr.ReportDefinition.ReportObjects["背景能力開発1"].ObjectFormat.EnableSuppress = bolSecondaryBackColor;
                cr.ReportDefinition.ReportObjects["背景他1"].ObjectFormat.EnableSuppress = bolPrimaryBackColor;
                //2021-03-26 iwai-tamura add-end ------
            } catch(Exception ex) {
                // エラー
                nlog.Error(System.Reflection.MethodBase.GetCurrentMethod().Name + " error " + ex.ToString());
                throw;
            } finally {
                //終了
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " end");
            }
        }


        /// <summary>
        /// 自己申告書データを帳票パラメータに直接セット
        /// </summary>
        /// <param name="cr">クリスタルレポート</param>
        /// <param name="dr">自己申告書DataRow</param>
        /// <returns>セット後のクリスタルレポート</returns>
        private void SelfDeclareC12BaseSetting(ref SelfDeclareReportC12 cr, DataRow dr) {
            try {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");
                //datetimeに変換できたら和暦、だめだったら空をreturn
                Func<string, string, string> wareki = delegate(string val, string format) {
                    DateTime dt = new DateTime();
                    return DateTime.TryParse(val, out dt) ? DataConv.Date2Jcal(dt, format) : "";
                };

                //2020-04-07 iwai-tamura upd str ------
                Func<string, string, string> nengappi = delegate(string val, string format) {
                    DateTime dt = new DateTime();
                    return DateTime.TryParse(val, out dt) ? dt.ToString(format): "";
                };
                //2020-04-07 iwai-tamura upd end ------     

                //年齢表示用
                Func<string, string> ageView = (val1) => {
                    if(!string.IsNullOrEmpty(val1)) {
                        return val1+"歳";
                    }
                    return null;
                };

                //チェックボックス表示用
                Func<string, string> checkView = (val1) => {
                    if(!string.IsNullOrEmpty(val1)) {
                        return (val1 == "1")? "☑": "□";
                    }
                    return "□";
                };

                //2020-04-07 iwai-tamura upd str ------
                //西暦表示に変更
                //2020-12-24 iwai-tamura upd str ------

                //2021-04-05 iwai-tamura upd str ------
                switch (dr["自己申告状態区分"].ToString()) {
	                case "0":
	                case "1":
                        cr.SetParameterValue("現在年月日", nengappi(dr["年度"].ToString() + "/12/31", "yyyy年M月d日現在"));
	                    break;
	                case "2":
	                case "3":
                        cr.SetParameterValue("現在年月日", nengappi((Int32.Parse(dr["年度"].ToString()) + 1 ) + "/3/31", "yyyy年M月d日現在"));
                        
	                    break;
	                default:
	                    //上記以外 ReadOnly
	                    break;
	            }
                //cr.SetParameterValue("現在年月日", nengappi(dr["年度"].ToString() + "/12/31", "yyyy年M月d日現在"));
                //2021-04-05 iwai-tamura upd end ------

                cr.SetParameterValue("タイトル年度", nengappi(dr["年度"].ToString() + "/01/01", "yyyy年度"));
                //cr.SetParameterValue("現在年月日", nengappi(dr["年度"].ToString() + "/03/31", "yyyy年M月d日現在"));
                //cr.SetParameterValue("タイトル年度", nengappi(dr["年度"].ToString() + "/04/01", "yyyy年度"));
                //2020-12-24 iwai-tamura upd end ------
                //cr.SetParameterValue("現在年月日", wareki(dr["年度"].ToString() + "/03/31", "ggy年M月d日現在"));
                //cr.SetParameterValue("タイトル年度", wareki(dr["年度"].ToString() + "/04/01", "ggy年度"));
                //2020-04-07 iwai-tamura upd end ------     
                cr.SetParameterValue("氏名", DataConv.IfNull(dr["氏名"].ToString(), ""));
                cr.SetParameterValue("社員番号", DataConv.IfNull(dr["社員番号"].ToString(), ""));
                cr.SetParameterValue("所属", DataConv.IfNull(dr["所属名"].ToString(), ""));
                cr.SetParameterValue("役職名", DataConv.IfNull(dr["役職名"].ToString(), ""));
                cr.SetParameterValue("資格", DataConv.IfNull(dr["資格名"].ToString(), ""));

                //和暦に変換してセット
                //2020-04-07 iwai-tamura upd str ------
                //西暦表示に変更
                cr.SetParameterValue("入社年月日", nengappi(dr["入社年月日"].ToString().Substring(0,4) + "/" + dr["入社年月日"].ToString().Substring(4,2) + "/" + dr["入社年月日"].ToString().Substring(6,2), "yyyy年M月d日"));
                //cr.SetParameterValue("入社年月日", wareki(dr["入社年月日"].ToString().Substring(0,4) + "/" + dr["入社年月日"].ToString().Substring(4,2) + "/" + dr["入社年月日"].ToString().Substring(6,2), "ggy年M月d日"));
                //2020-04-07 iwai-tamura upd end ------     

                //表示用在籍期間・現職経験
                int intYear = 0;
                int intMonth = 0;
                string strView = "";
                intYear = ((int)dr["在籍月数"]/12);
                intMonth = ((int)dr["在籍月数"]%12);
                if (intYear!=0){
                   strView = intYear.ToString("0年");
                }
                strView = strView + intMonth.ToString("0ヶ月");
                cr.SetParameterValue("当社在籍年数", DataConv.IfNull(strView, ""));    

                strView = "";
                intYear = ((int)dr["現職経験月数"]/12);
                intMonth = ((int)dr["現職経験月数"]%12);
                if (intYear!=0){
                   strView = intYear.ToString("0年");
                }
                strView = strView + intMonth.ToString("0ヶ月");
                cr.SetParameterValue("現職経験年数", DataConv.IfNull(strView, ""));    
                
                //2020-04-07 iwai-tamura upd str ------
                //西暦表示に変更
                cr.SetParameterValue("生年月日", nengappi(dr["生年月日"].ToString().Substring(0,4) + "/" + dr["生年月日"].ToString().Substring(4,2) + "/" + dr["生年月日"].ToString().Substring(6,2), "yyyy年M月d日"));
                //cr.SetParameterValue("生年月日", wareki(dr["生年月日"].ToString().Substring(0,4) + "/" + dr["生年月日"].ToString().Substring(4,2) + "/" + dr["生年月日"].ToString().Substring(6,2), "ggy年M月d日"));
                //2020-04-07 iwai-tamura upd end ------     
                cr.SetParameterValue("年齢", DataConv.IfNull(dr["年齢"].ToString()+"歳", ""));

                //家族扶養同居チェック
                for (int i = 1; i <= 8; i++)
                {
                    cr.SetParameterValue("家族構成同居区分_"+i, DataConv.IfNull(checkView(dr["家族構成同居区分_"+i].ToString()), ""));
                    cr.SetParameterValue("家族構成扶養区分_"+i, DataConv.IfNull(checkView(dr["家族構成扶養区分_"+i].ToString()), ""));
                    cr.SetParameterValue("家族構成年齢_"+i, DataConv.IfNull(ageView(dr["家族構成年齢_"+i].ToString()), ""));
                }

                //2021-03-26 iwai-tamura add-str ------
                //一次入力、二次入力時のグレーアウト処理
                bool bolPrimaryBackColor = true;
                bool bolSecondaryBackColor = true;
	            switch (dr["自己申告状態区分"].ToString()) {
	                case "0":
	                case "1":
                        bolPrimaryBackColor = true;
                        bolSecondaryBackColor = false;
	                    break;
	                case "2":
	                case "3":
                        bolPrimaryBackColor = false;
                        bolSecondaryBackColor = true;
	                    break;
	                default:
	                    //上記以外 ReadOnly
	                    break;
	            }
                cr.ReportDefinition.ReportObjects["背景職務変更配置換1"].ObjectFormat.EnableSuppress = bolPrimaryBackColor;
                cr.ReportDefinition.ReportObjects["背景職務変更配置換2"].ObjectFormat.EnableSuppress = bolPrimaryBackColor;
                cr.ReportDefinition.ReportObjects["背景担当職務1"].ObjectFormat.EnableSuppress = bolSecondaryBackColor;
                cr.ReportDefinition.ReportObjects["背景担当職務2"].ObjectFormat.EnableSuppress = bolSecondaryBackColor;
                cr.ReportDefinition.ReportObjects["背景能力開発1"].ObjectFormat.EnableSuppress = bolSecondaryBackColor;
                cr.ReportDefinition.ReportObjects["背景他1"].ObjectFormat.EnableSuppress = bolPrimaryBackColor;
                //2021-03-26 iwai-tamura add-end ------

            } catch(Exception ex) {
                // エラー
                nlog.Error(System.Reflection.MethodBase.GetCurrentMethod().Name + " error " + ex.ToString());
                throw;
            } finally {
                //終了
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " end");
            }
        }
        //2020-03-31 iwai-tamura upd end -----






















        
        
        
        
        /// <summary>
        /// 自己申告書データを帳票パラメータに直接セット
        /// </summary>
        /// <param name="cr">クリスタルレポート</param>
        /// <param name="dr">自己申告書DataRow</param>
        /// <returns>セット後のクリスタルレポート</returns>
        private void SelfDeclareABaseSetting(ref SelfDeclareReportA11 cr, DataRow dr) {
            //※※※※一時的に変更※※※※※※※※※※※※※※※※※※※※※※※※
            try {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");
                //datetimeに変換できたら和暦、だめだったら空をreturn
                Func<string, string, string> wareki = delegate(string val, string format) {
                    DateTime dt = new DateTime();
                    return DateTime.TryParse(val, out dt) ? DataConv.Date2Jcal(dt, format) : "";
                };

                //2020-04-07 iwai-tamura upd str ------
                Func<string, string, string> nengappi = delegate(string val, string format) {
                    DateTime dt = new DateTime();
                    return DateTime.TryParse(val, out dt) ? dt.ToString(format): "";
                };
                //2020-04-07 iwai-tamura upd end ------     

                //年齢表示用
                Func<string, string> ageView = (val1) => {
                    if(!string.IsNullOrEmpty(val1)) {
                        return val1+"歳";
                    }
                    return null;
                };

                //チェックボックス表示用
                Func<string, string> checkView = (val1) => {
                    if(!string.IsNullOrEmpty(val1)) {
                        return (val1 == "1")? "☑": "□";
                    }
                    return "□";
                };

                //2020-04-07 iwai-tamura upd str ------
                //西暦表示に変更
                //2020-12-24 iwai-tamura upd str ------

                //2021-04-05 iwai-tamura upd str ------
                switch (dr["自己申告状態区分"].ToString()) {
	                case "0":
	                case "1":
                        cr.SetParameterValue("現在年月日", nengappi(dr["年度"].ToString() + "/12/31", "yyyy年M月d日現在"));
	                    break;
	                case "2":
	                case "3":
                        cr.SetParameterValue("現在年月日", nengappi((Int32.Parse(dr["年度"].ToString()) + 1 ) + "/3/31", "yyyy年M月d日現在"));
                        
	                    break;
	                default:
	                    //上記以外 ReadOnly
	                    break;
	            }
                //cr.SetParameterValue("現在年月日", nengappi(dr["年度"].ToString() + "/12/31", "yyyy年M月d日現在"));
                //2021-04-05 iwai-tamura upd end ------

                cr.SetParameterValue("タイトル年度", nengappi(dr["年度"].ToString() + "/01/01", "yyyy年度"));
                //cr.SetParameterValue("現在年月日", nengappi(dr["年度"].ToString() + "/03/31", "yyyy年M月d日現在"));
                //cr.SetParameterValue("タイトル年度", nengappi(dr["年度"].ToString() + "/04/01", "yyyy年度"));
                //2020-12-24 iwai-tamura upd end ------
                //cr.SetParameterValue("現在年月日", wareki(dr["年度"].ToString() + "/03/31", "ggy年M月d日現在"));
                //cr.SetParameterValue("タイトル年度", wareki(dr["年度"].ToString() + "/04/01", "ggy年度"));
                //2020-04-07 iwai-tamura upd end ------     
                cr.SetParameterValue("氏名", DataConv.IfNull(dr["氏名"].ToString(), ""));
                cr.SetParameterValue("社員番号", DataConv.IfNull(dr["社員番号"].ToString(), ""));
                cr.SetParameterValue("所属", DataConv.IfNull(dr["所属名"].ToString(), ""));
                cr.SetParameterValue("役職名", DataConv.IfNull(dr["役職名"].ToString(), ""));
                cr.SetParameterValue("資格", DataConv.IfNull(dr["資格名"].ToString(), ""));

                //和暦に変換してセット
                //2020-04-07 iwai-tamura upd str ------
                //西暦表示に変更
                cr.SetParameterValue("入社年月日", nengappi(dr["入社年月日"].ToString().Substring(0,4) + "/" + dr["入社年月日"].ToString().Substring(4,2) + "/" + dr["入社年月日"].ToString().Substring(6,2), "yyyy年M月d日"));
                //cr.SetParameterValue("入社年月日", wareki(dr["入社年月日"].ToString().Substring(0,4) + "/" + dr["入社年月日"].ToString().Substring(4,2) + "/" + dr["入社年月日"].ToString().Substring(6,2), "ggy年M月d日"));
                //2020-04-07 iwai-tamura upd end ------     

                //表示用在籍期間・現職経験
                int intYear = 0;
                int intMonth = 0;
                string strView = "";
                intYear = ((int)dr["在籍月数"]/12);
                intMonth = ((int)dr["在籍月数"]%12);
                if (intYear!=0){
                   strView = intYear.ToString("0年");
                }
                strView = strView + intMonth.ToString("0ヶ月");
                cr.SetParameterValue("当社在籍年数", DataConv.IfNull(strView, ""));    

                strView = "";
                intYear = ((int)dr["現職経験月数"]/12);
                intMonth = ((int)dr["現職経験月数"]%12);
                if (intYear!=0){
                   strView = intYear.ToString("0年");
                }
                strView = strView + intMonth.ToString("0ヶ月");
                cr.SetParameterValue("現職経験年数", DataConv.IfNull(strView, ""));    
                
                //2020-04-07 iwai-tamura upd str ------
                //西暦表示に変更
                cr.SetParameterValue("生年月日", nengappi(dr["生年月日"].ToString().Substring(0,4) + "/" + dr["生年月日"].ToString().Substring(4,2) + "/" + dr["生年月日"].ToString().Substring(6,2), "yyyy年M月d日"));
                //cr.SetParameterValue("生年月日", wareki(dr["生年月日"].ToString().Substring(0,4) + "/" + dr["生年月日"].ToString().Substring(4,2) + "/" + dr["生年月日"].ToString().Substring(6,2), "ggy年M月d日"));
                //2020-04-07 iwai-tamura upd end ------     
                cr.SetParameterValue("年齢", DataConv.IfNull(dr["年齢"].ToString()+"歳", ""));

                //家族扶養同居チェック
                for (int i = 1; i <= 8; i++)
                {
                    cr.SetParameterValue("家族構成同居区分_"+i, DataConv.IfNull(checkView(dr["家族構成同居区分_"+i].ToString()), ""));
                    cr.SetParameterValue("家族構成扶養区分_"+i, DataConv.IfNull(checkView(dr["家族構成扶養区分_"+i].ToString()), ""));
                    cr.SetParameterValue("家族構成年齢_"+i, DataConv.IfNull(ageView(dr["家族構成年齢_"+i].ToString()), ""));
                }


            } catch(Exception ex) {
                // エラー
                nlog.Error(System.Reflection.MethodBase.GetCurrentMethod().Name + " error " + ex.ToString());
                throw;
            } finally {
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
        private void SelfDeclareBBaseSetting(ref SelfDeclareReportB12 cr, DataRow dr) {
            //※※※※一時的に変更※※※※※※※※※※※※※※※※※※※※※※※※
            try {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");
                //datetimeに変換できたら和暦、だめだったら空をreturn
                Func<string, string, string> wareki = delegate(string val, string format) {
                    DateTime dt = new DateTime();
                    return DateTime.TryParse(val, out dt) ? DataConv.Date2Jcal(dt, format) : "";
                };

                //2020-04-07 iwai-tamura upd str ------
                Func<string, string, string> nengappi = delegate(string val, string format) {
                    DateTime dt = new DateTime();
                    return DateTime.TryParse(val, out dt) ? dt.ToString(format): "";
                };
                //2020-04-07 iwai-tamura upd end ------     

                //年齢表示用
                Func<string, string> ageView = (val1) => {
                    if(!string.IsNullOrEmpty(val1)) {
                        return val1+"歳";
                    }
                    return null;
                };

                //チェックボックス表示用
                Func<string, string> checkView = (val1) => {
                    if(!string.IsNullOrEmpty(val1)) {
                        return (val1 == "1")? "☑": "□";
                    }
                    return "□";
                };

                //2020-04-07 iwai-tamura upd str ------
                //西暦表示に変更
                //2020-12-24 iwai-tamura upd str ------

                //2021-04-05 iwai-tamura upd str ------
                switch (dr["自己申告状態区分"].ToString()) {
	                case "0":
	                case "1":
                        cr.SetParameterValue("現在年月日", nengappi(dr["年度"].ToString() + "/12/31", "yyyy年M月d日現在"));
	                    break;
	                case "2":
	                case "3":
                        cr.SetParameterValue("現在年月日", nengappi((Int32.Parse(dr["年度"].ToString()) + 1 ) + "/3/31", "yyyy年M月d日現在"));
                        
	                    break;
	                default:
	                    //上記以外 ReadOnly
	                    break;
	            }
                //cr.SetParameterValue("現在年月日", nengappi(dr["年度"].ToString() + "/12/31", "yyyy年M月d日現在"));
                //2021-04-05 iwai-tamura upd end ------

                cr.SetParameterValue("タイトル年度", nengappi(dr["年度"].ToString() + "/01/01", "yyyy年度"));
                //cr.SetParameterValue("現在年月日", nengappi(dr["年度"].ToString() + "/03/31", "yyyy年M月d日現在"));
                //cr.SetParameterValue("タイトル年度", nengappi(dr["年度"].ToString() + "/04/01", "yyyy年度"));
                //2020-12-24 iwai-tamura upd end ------
                //cr.SetParameterValue("現在年月日", wareki(dr["年度"].ToString() + "/03/31", "ggy年M月d日現在"));
                //cr.SetParameterValue("タイトル年度", wareki(dr["年度"].ToString() + "/04/01", "ggy年度"));
                //2020-04-07 iwai-tamura upd end ------     
                cr.SetParameterValue("氏名", DataConv.IfNull(dr["氏名"].ToString(), ""));
                cr.SetParameterValue("社員番号", DataConv.IfNull(dr["社員番号"].ToString(), ""));
                cr.SetParameterValue("所属", DataConv.IfNull(dr["所属名"].ToString(), ""));
                cr.SetParameterValue("役職名", DataConv.IfNull(dr["役職名"].ToString(), ""));
                cr.SetParameterValue("資格", DataConv.IfNull(dr["資格名"].ToString(), ""));

                //和暦に変換してセット
                //2020-04-07 iwai-tamura upd str ------
                //西暦表示に変更
                cr.SetParameterValue("入社年月日", nengappi(dr["入社年月日"].ToString().Substring(0,4) + "/" + dr["入社年月日"].ToString().Substring(4,2) + "/" + dr["入社年月日"].ToString().Substring(6,2), "yyyy年M月d日"));
                //cr.SetParameterValue("入社年月日", wareki(dr["入社年月日"].ToString().Substring(0,4) + "/" + dr["入社年月日"].ToString().Substring(4,2) + "/" + dr["入社年月日"].ToString().Substring(6,2), "ggy年M月d日"));
                //2020-04-07 iwai-tamura upd end ------     

                //表示用在籍期間・現職経験
                int intYear = 0;
                int intMonth = 0;
                string strView = "";
                intYear = ((int)dr["在籍月数"]/12);
                intMonth = ((int)dr["在籍月数"]%12);
                if (intYear!=0){
                   strView = intYear.ToString("0年");
                }
                strView = strView + intMonth.ToString("0ヶ月");
                cr.SetParameterValue("当社在籍年数", DataConv.IfNull(strView, ""));    

                strView = "";
                intYear = ((int)dr["現職経験月数"]/12);
                intMonth = ((int)dr["現職経験月数"]%12);
                if (intYear!=0){
                   strView = intYear.ToString("0年");
                }
                strView = strView + intMonth.ToString("0ヶ月");
                cr.SetParameterValue("現職経験年数", DataConv.IfNull(strView, ""));    
                
                //2020-04-07 iwai-tamura upd str ------
                //西暦表示に変更
                cr.SetParameterValue("生年月日", nengappi(dr["生年月日"].ToString().Substring(0,4) + "/" + dr["生年月日"].ToString().Substring(4,2) + "/" + dr["生年月日"].ToString().Substring(6,2), "yyyy年M月d日"));
                //cr.SetParameterValue("生年月日", wareki(dr["生年月日"].ToString().Substring(0,4) + "/" + dr["生年月日"].ToString().Substring(4,2) + "/" + dr["生年月日"].ToString().Substring(6,2), "ggy年M月d日"));
                //2020-04-07 iwai-tamura upd end ------     
                cr.SetParameterValue("年齢", DataConv.IfNull(dr["年齢"].ToString()+"歳", ""));

                //家族扶養同居チェック
                for (int i = 1; i <= 8; i++)
                {
                    cr.SetParameterValue("家族構成同居区分_"+i, DataConv.IfNull(checkView(dr["家族構成同居区分_"+i].ToString()), ""));
                    cr.SetParameterValue("家族構成扶養区分_"+i, DataConv.IfNull(checkView(dr["家族構成扶養区分_"+i].ToString()), ""));
                    cr.SetParameterValue("家族構成年齢_"+i, DataConv.IfNull(ageView(dr["家族構成年齢_"+i].ToString()), ""));
                }
            } catch(Exception ex) {
                // エラー
                nlog.Error(System.Reflection.MethodBase.GetCurrentMethod().Name + " error " + ex.ToString());
                throw;
            } finally {
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
        private void SelfDeclareCBaseSetting(ref SelfDeclareReportC12 cr, DataRow dr) {
            //※※※※一時的に変更※※※※※※※※※※※※※※※※※※※※※※※※
            try {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");
                //datetimeに変換できたら和暦、だめだったら空をreturn
                Func<string, string, string> wareki = delegate(string val, string format) {
                    DateTime dt = new DateTime();
                    return DateTime.TryParse(val, out dt) ? DataConv.Date2Jcal(dt, format) : "";
                };

                //2020-04-07 iwai-tamura upd str ------
                Func<string, string, string> nengappi = delegate(string val, string format) {
                    DateTime dt = new DateTime();
                    return DateTime.TryParse(val, out dt) ? dt.ToString(format): "";
                };
                //2020-04-07 iwai-tamura upd end ------     

                //年齢表示用
                Func<string, string> ageView = (val1) => {
                    if(!string.IsNullOrEmpty(val1)) {
                        return val1+"歳";
                    }
                    return null;
                };

                //チェックボックス表示用
                Func<string, string> checkView = (val1) => {
                    if(!string.IsNullOrEmpty(val1)) {
                        return (val1 == "1")? "☑": "□";
                    }
                    return "□";
                };

                //2020-04-07 iwai-tamura upd str ------
                //西暦表示に変更
                //2020-12-24 iwai-tamura upd str ------

                //2021-04-05 iwai-tamura upd str ------
                switch (dr["自己申告状態区分"].ToString()) {
	                case "0":
	                case "1":
                        cr.SetParameterValue("現在年月日", nengappi(dr["年度"].ToString() + "/12/31", "yyyy年M月d日現在"));
	                    break;
	                case "2":
	                case "3":
                        cr.SetParameterValue("現在年月日", nengappi((Int32.Parse(dr["年度"].ToString()) + 1 ) + "/3/31", "yyyy年M月d日現在"));
                        
	                    break;
	                default:
	                    //上記以外 ReadOnly
	                    break;
	            }
                //cr.SetParameterValue("現在年月日", nengappi(dr["年度"].ToString() + "/12/31", "yyyy年M月d日現在"));
                //2021-04-05 iwai-tamura upd end ------

                cr.SetParameterValue("タイトル年度", nengappi(dr["年度"].ToString() + "/01/01", "yyyy年度"));
                //cr.SetParameterValue("現在年月日", nengappi(dr["年度"].ToString() + "/03/31", "yyyy年M月d日現在"));
                //cr.SetParameterValue("タイトル年度", nengappi(dr["年度"].ToString() + "/04/01", "yyyy年度"));
                //2020-12-24 iwai-tamura upd end ------
                //cr.SetParameterValue("現在年月日", wareki(dr["年度"].ToString() + "/03/31", "ggy年M月d日現在"));
                //cr.SetParameterValue("タイトル年度", wareki(dr["年度"].ToString() + "/04/01", "ggy年度"));
                //2020-04-07 iwai-tamura upd end ------     
                cr.SetParameterValue("氏名", DataConv.IfNull(dr["氏名"].ToString(), ""));
                cr.SetParameterValue("社員番号", DataConv.IfNull(dr["社員番号"].ToString(), ""));
                cr.SetParameterValue("所属", DataConv.IfNull(dr["所属名"].ToString(), ""));
                cr.SetParameterValue("役職名", DataConv.IfNull(dr["役職名"].ToString(), ""));
                cr.SetParameterValue("資格", DataConv.IfNull(dr["資格名"].ToString(), ""));

                //和暦に変換してセット
                //2020-04-07 iwai-tamura upd str ------
                //西暦表示に変更
                cr.SetParameterValue("入社年月日", nengappi(dr["入社年月日"].ToString().Substring(0,4) + "/" + dr["入社年月日"].ToString().Substring(4,2) + "/" + dr["入社年月日"].ToString().Substring(6,2), "yyyy年M月d日"));
                //cr.SetParameterValue("入社年月日", wareki(dr["入社年月日"].ToString().Substring(0,4) + "/" + dr["入社年月日"].ToString().Substring(4,2) + "/" + dr["入社年月日"].ToString().Substring(6,2), "ggy年M月d日"));
                //2020-04-07 iwai-tamura upd end ------     

                //表示用在籍期間・現職経験
                int intYear = 0;
                int intMonth = 0;
                string strView = "";
                intYear = ((int)dr["在籍月数"]/12);
                intMonth = ((int)dr["在籍月数"]%12);
                if (intYear!=0){
                   strView = intYear.ToString("0年");
                }
                strView = strView + intMonth.ToString("0ヶ月");
                cr.SetParameterValue("当社在籍年数", DataConv.IfNull(strView, ""));    

                strView = "";
                intYear = ((int)dr["現職経験月数"]/12);
                intMonth = ((int)dr["現職経験月数"]%12);
                if (intYear!=0){
                   strView = intYear.ToString("0年");
                }
                strView = strView + intMonth.ToString("0ヶ月");
                cr.SetParameterValue("現職経験年数", DataConv.IfNull(strView, ""));    
                
                //2020-04-07 iwai-tamura upd str ------
                //西暦表示に変更
                cr.SetParameterValue("生年月日", nengappi(dr["生年月日"].ToString().Substring(0,4) + "/" + dr["生年月日"].ToString().Substring(4,2) + "/" + dr["生年月日"].ToString().Substring(6,2), "yyyy年M月d日"));
                //cr.SetParameterValue("生年月日", wareki(dr["生年月日"].ToString().Substring(0,4) + "/" + dr["生年月日"].ToString().Substring(4,2) + "/" + dr["生年月日"].ToString().Substring(6,2), "ggy年M月d日"));
                //2020-04-07 iwai-tamura upd end ------     
                cr.SetParameterValue("年齢", DataConv.IfNull(dr["年齢"].ToString()+"歳", ""));

                //家族扶養同居チェック
                for (int i = 1; i <= 8; i++)
                {
                    cr.SetParameterValue("家族構成同居区分_"+i, DataConv.IfNull(checkView(dr["家族構成同居区分_"+i].ToString()), ""));
                    cr.SetParameterValue("家族構成扶養区分_"+i, DataConv.IfNull(checkView(dr["家族構成扶養区分_"+i].ToString()), ""));
                    cr.SetParameterValue("家族構成年齢_"+i, DataConv.IfNull(ageView(dr["家族構成年齢_"+i].ToString()), ""));
                }
            } catch(Exception ex) {
                // エラー
                nlog.Error(System.Reflection.MethodBase.GetCurrentMethod().Name + " error " + ex.ToString());
                throw;
            } finally {
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
        private void SelfDeclareDBaseSetting(ref SelfDeclareReportD cr, DataRow dr) {
            try {
                //開始
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");
                //datetimeに変換できたら和暦、だめだったら空をreturn
                Func<string, string, string> wareki = delegate(string val, string format) {
                    DateTime dt = new DateTime();
                    return DateTime.TryParse(val, out dt) ? DataConv.Date2Jcal(dt, format) : "";
                };

                //2020-04-07 iwai-tamura upd str ------
                Func<string, string, string> nengappi = delegate(string val, string format) {
                    DateTime dt = new DateTime();
                    return DateTime.TryParse(val, out dt) ? dt.ToString(format): "";
                };
                //2020-04-07 iwai-tamura upd end ------     

                //年齢表示用
                Func<string, string> ageView = (val1) => {
                    if(!string.IsNullOrEmpty(val1)) {
                        return val1+"歳";
                    }
                    return null;
                };

                //チェックボックス表示用
                Func<string, string> checkView = (val1) => {
                    if(!string.IsNullOrEmpty(val1)) {
                        return (val1 == "1")? "☑": "□";
                    }
                    return "□";
                };

                //2020-04-07 iwai-tamura upd str ------
                //西暦表示に変更
                //2020-12-24 iwai-tamura upd str ------
                cr.SetParameterValue("現在年月日", nengappi(dr["年度"].ToString() + "/12/31", "yyyy年M月d日現在"));
                cr.SetParameterValue("タイトル年度", nengappi(dr["年度"].ToString() + "/01/01", "yyyy年度"));
                //cr.SetParameterValue("現在年月日", nengappi(dr["年度"].ToString() + "/03/31", "yyyy年M月d日現在"));
                //cr.SetParameterValue("タイトル年度", nengappi(dr["年度"].ToString() + "/04/01", "yyyy年度"));
                //2020-12-24 iwai-tamura upd end ------
                //cr.SetParameterValue("現在年月日", wareki(dr["年度"].ToString() + "/03/31", "ggy年M月d日現在"));
                //cr.SetParameterValue("タイトル年度", wareki(dr["年度"].ToString() + "/04/01", "ggy年度"));
                //2020-04-07 iwai-tamura upd end ------                
                
                cr.SetParameterValue("氏名", DataConv.IfNull(dr["氏名"].ToString(), ""));
                cr.SetParameterValue("社員番号", DataConv.IfNull(dr["社員番号"].ToString(), ""));
                cr.SetParameterValue("所属", DataConv.IfNull(dr["所属名"].ToString(), ""));
                cr.SetParameterValue("役職名", DataConv.IfNull(dr["役職名"].ToString(), ""));
                cr.SetParameterValue("資格", DataConv.IfNull(dr["資格名"].ToString(), ""));

                ////和暦に変換してセット
                //cr.SetParameterValue("入社年月日", wareki(dr["入社年月日"].ToString().Substring(0,4) + "/" + dr["入社年月日"].ToString().Substring(4,2) + "/" + dr["入社年月日"].ToString().Substring(6,2), "ggy年M月d日"));

                ////表示用在籍期間・現職経験
                //int intYear = 0;
                //int intMonth = 0;
                //string strView = "";
                //intYear = ((int)dr["在籍月数"]/12);
                //intMonth = ((int)dr["在籍月数"]%12);
                //if (intYear!=0){
                //   strView = intYear.ToString("0年");
                //}
                //strView = strView + intMonth.ToString("0ヶ月");
                //cr.SetParameterValue("当社在籍年数", DataConv.IfNull(strView, ""));    

                //strView = "";
                //intYear = ((int)dr["現職経験月数"]/12);
                //intMonth = ((int)dr["現職経験月数"]%12);
                //if (intYear!=0){
                //   strView = intYear.ToString("0年");
                //}
                //strView = strView + intMonth.ToString("0ヶ月");
                //cr.SetParameterValue("現職経験年数", DataConv.IfNull(strView, ""));    
                
                //cr.SetParameterValue("生年月日", wareki(dr["生年月日"].ToString().Substring(0,4) + "/" + dr["生年月日"].ToString().Substring(4,2) + "/" + dr["生年月日"].ToString().Substring(6,2), "ggy年M月d日"));
                //cr.SetParameterValue("年齢", DataConv.IfNull(dr["年齢"].ToString()+"歳", ""));

                ////家族扶養同居チェック
                //for (int i = 1; i <= 8; i++)
                //{
                //    cr.SetParameterValue("家族構成同居区分_"+i, DataConv.IfNull(checkView(dr["家族構成同居区分_"+i].ToString()), ""));
                //    cr.SetParameterValue("家族構成扶養区分_"+i, DataConv.IfNull(checkView(dr["家族構成扶養区分_"+i].ToString()), ""));
                //    cr.SetParameterValue("家族構成年齢_"+i, DataConv.IfNull(ageView(dr["家族構成年齢_"+i].ToString()), ""));
                //}


            } catch(Exception ex) {
                // エラー
                nlog.Error(System.Reflection.MethodBase.GetCurrentMethod().Name + " error " + ex.ToString());
                throw;
            } finally {
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
        private void CarrierSheetBaseSetting(ref CarrierSheetReport cr, DataRow dr) {
            try {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");
                //datetimeに変換できたら和暦、だめだったら空をreturn
                Func<string, string, string> wareki = delegate(string val, string format) {
                    DateTime dt = new DateTime();
                    return DateTime.TryParse(val, out dt) ? DataConv.Date2Jcal(dt, format) : "";
                };

                //2020-04-07 iwai-tamura upd str ------
                Func<string, string, string> nengappi = delegate(string val, string format) {
                    DateTime dt = new DateTime();
                    return DateTime.TryParse(val, out dt) ? dt.ToString(format): "";
                };
                //2020-04-07 iwai-tamura upd end ------     

                Func<string, string, string> nengetsu = (val1,val2)=> {
                    string val = "";
                    if(!string.IsNullOrEmpty(val1)) {
                        val += val1+"年";
                    }
                    if(!string.IsNullOrEmpty(val2)) {
                         val +=  val2+"月";
                    }

                    return val;
                };

                Func<string, string, string> kikan = (val1,val2)=> {
                    string val = "";
                    if(!string.IsNullOrEmpty(val1)) {
                        val += val1+"年";
                    }
                    if(!string.IsNullOrEmpty(val2)) {
                         val +=  val2+"ヶ月";
                    }

                    return val;
                };

                //年齢表示用
                Func<string, string> ageView = (val1) => {
                    if(!string.IsNullOrEmpty(val1)) {
                        return val1+"歳";
                    }
                    return null;
                };

                //和暦に変換してセット
                //2020-04-07 iwai-tamura upd str ------
                //西暦表示に変更
                //2020-12-24 iwai-tamura upd str ------
                cr.SetParameterValue("現在年月日", nengappi(dr["年度"].ToString() + "/12/31", "yyyy年M月d日現在"));
                cr.SetParameterValue("タイトル年度", nengappi(dr["年度"].ToString() + "/01/01", "yyyy年度"));
                //cr.SetParameterValue("現在年月日", nengappi(dr["年度"].ToString() + "/03/31", "yyyy年M月d日現在"));
                //cr.SetParameterValue("タイトル年度", nengappi(dr["年度"].ToString() + "/04/01", "yyyy年度"));
                //2020-12-24 iwai-tamura upd end ------
                //cr.SetParameterValue("現在年月日", wareki(dr["年度"].ToString() + "/03/31", "ggy年M月d日現在"));
                //cr.SetParameterValue("タイトル年度", wareki(dr["年度"].ToString() + "/04/01", "ggy年度"));
                //2020-04-07 iwai-tamura upd end ------

                cr.SetParameterValue("氏名", DataConv.IfNull(dr["氏名"].ToString(), ""));
                cr.SetParameterValue("社員番号", DataConv.IfNull(dr["社員番号"].ToString(), ""));
                cr.SetParameterValue("所属", DataConv.IfNull(dr["所属名"].ToString(), ""));
                cr.SetParameterValue("役職名", DataConv.IfNull(dr["役職名"].ToString(), ""));
                cr.SetParameterValue("資格", DataConv.IfNull(dr["資格名"].ToString(), ""));

                //和暦に変換してセット
                //2020-04-07 iwai-tamura upd str ------
                //西暦表示に変更
                cr.SetParameterValue("入社年月日", nengappi(dr["入社年月日"].ToString().Substring(0,4) + "/" + dr["入社年月日"].ToString().Substring(4,2) + "/" + dr["入社年月日"].ToString().Substring(6,2), "yyyy年M月d日"));
                //cr.SetParameterValue("入社年月日", wareki(dr["入社年月日"].ToString().Substring(0,4) + "/" + dr["入社年月日"].ToString().Substring(4,2) + "/" + dr["入社年月日"].ToString().Substring(6,2), "ggy年M月d日"));
                //2020-04-07 iwai-tamura upd end ------     

                //表示用在籍期間・現職経験
                int intYear = 0;
                int intMonth = 0;
                string strView = "";
                intYear = ((int)dr["在籍月数"]/12);
                intMonth = ((int)dr["在籍月数"]%12);
                if (intYear!=0){
                   strView = intYear.ToString("0年");
                }
                strView = strView + intMonth.ToString("0ヶ月");
                cr.SetParameterValue("当社在籍年数", DataConv.IfNull(strView, ""));    

                strView = "";
                intYear = ((int)dr["現職経験月数"]/12);
                intMonth = ((int)dr["現職経験月数"]%12);
                if (intYear!=0){
                   strView = intYear.ToString("0年");
                }
                strView = strView + intMonth.ToString("0ヶ月");
                cr.SetParameterValue("現職経験年数", DataConv.IfNull(strView, ""));    
                
                strView = "";
                intYear = ((int)dr["現資格在級月数"]/12);
                intMonth = ((int)dr["現資格在級月数"]%12);
                if (intYear!=0){
                   strView = intYear.ToString("0年");
                }
                strView = strView + intMonth.ToString("0ヶ月");
                cr.SetParameterValue("現資格在級年数", DataConv.IfNull(strView, ""));    

                
                //2020-04-07 iwai-tamura upd str ------
                //西暦表示に変更
                cr.SetParameterValue("生年月日", nengappi(dr["生年月日"].ToString().Substring(0,4) + "/" + dr["生年月日"].ToString().Substring(4,2) + "/" + dr["生年月日"].ToString().Substring(6,2), "yyyy年M月d日"));
                //cr.SetParameterValue("生年月日", wareki(dr["生年月日"].ToString().Substring(0,4) + "/" + dr["生年月日"].ToString().Substring(4,2) + "/" + dr["生年月日"].ToString().Substring(6,2), "ggy年M月d日"));
                //2020-04-07 iwai-tamura upd end ------     
                cr.SetParameterValue("年齢", DataConv.IfNull(dr["年齢"].ToString()+"歳", ""));


                //表示用社外歴
                cr.SetParameterValue("社外職歴年月1", nengetsu(dr["社外職歴年_1"].ToString(),dr["社外職歴月_1"].ToString()));    
                cr.SetParameterValue("社外職歴年月1", nengetsu(dr["社外職歴年_1"].ToString(),dr["社外職歴月_1"].ToString()));    
                cr.SetParameterValue("社外職歴年月2", nengetsu(dr["社外職歴年_2"].ToString(),dr["社外職歴月_2"].ToString()));    
                cr.SetParameterValue("社外職歴年月3", nengetsu(dr["社外職歴年_3"].ToString(),dr["社外職歴月_3"].ToString()));    
                cr.SetParameterValue("社外職歴年月4", nengetsu(dr["社外職歴年_4"].ToString(),dr["社外職歴月_4"].ToString()));    

                cr.SetParameterValue("社外職歴期間1", kikan(dr["社外職歴期間年_1"].ToString(),dr["社外職歴期間月_1"].ToString()));    
                cr.SetParameterValue("社外職歴期間2", kikan(dr["社外職歴期間年_2"].ToString(),dr["社外職歴期間月_2"].ToString()));    
                cr.SetParameterValue("社外職歴期間3", kikan(dr["社外職歴期間年_3"].ToString(),dr["社外職歴期間月_3"].ToString()));    
                cr.SetParameterValue("社外職歴期間4", kikan(dr["社外職歴期間年_4"].ToString(),dr["社外職歴期間月_4"].ToString()));    



                //表示用社内歴
                cr.SetParameterValue("社内職歴年月1", nengetsu(dr["社内職歴年_1"].ToString(),dr["社内職歴月_1"].ToString()));    
                cr.SetParameterValue("社内職歴年月2", nengetsu(dr["社内職歴年_2"].ToString(),dr["社内職歴月_2"].ToString()));    
                cr.SetParameterValue("社内職歴年月3", nengetsu(dr["社内職歴年_3"].ToString(),dr["社内職歴月_3"].ToString()));    
                cr.SetParameterValue("社内職歴年月4", nengetsu(dr["社内職歴年_4"].ToString(),dr["社内職歴月_4"].ToString()));    
                cr.SetParameterValue("社内職歴年月5", nengetsu(dr["社内職歴年_5"].ToString(),dr["社内職歴月_5"].ToString()));    
                cr.SetParameterValue("社内職歴年月6", nengetsu(dr["社内職歴年_6"].ToString(),dr["社内職歴月_6"].ToString()));    
                cr.SetParameterValue("社内職歴年月7", nengetsu(dr["社内職歴年_7"].ToString(),dr["社内職歴月_7"].ToString()));    
                cr.SetParameterValue("社内職歴年月8", nengetsu(dr["社内職歴年_8"].ToString(),dr["社内職歴月_8"].ToString()));    
                cr.SetParameterValue("社内職歴年月9", nengetsu(dr["社内職歴年_9"].ToString(),dr["社内職歴月_9"].ToString()));    
                cr.SetParameterValue("社内職歴年月10", nengetsu(dr["社内職歴年_10"].ToString(),dr["社内職歴月_10"].ToString()));    

                cr.SetParameterValue("社内職歴期間1", kikan(dr["社内職歴期間年_1"].ToString(),dr["社内職歴期間月_1"].ToString()));    
                cr.SetParameterValue("社内職歴期間2", kikan(dr["社内職歴期間年_2"].ToString(),dr["社内職歴期間月_2"].ToString()));    
                cr.SetParameterValue("社内職歴期間3", kikan(dr["社内職歴期間年_3"].ToString(),dr["社内職歴期間月_3"].ToString()));    
                cr.SetParameterValue("社内職歴期間4", kikan(dr["社内職歴期間年_4"].ToString(),dr["社内職歴期間月_4"].ToString()));    
                cr.SetParameterValue("社内職歴期間5", kikan(dr["社内職歴期間年_5"].ToString(),dr["社内職歴期間月_5"].ToString()));    
                cr.SetParameterValue("社内職歴期間6", kikan(dr["社内職歴期間年_6"].ToString(),dr["社内職歴期間月_6"].ToString()));    
                cr.SetParameterValue("社内職歴期間7", kikan(dr["社内職歴期間年_7"].ToString(),dr["社内職歴期間月_7"].ToString()));    
                cr.SetParameterValue("社内職歴期間8", kikan(dr["社内職歴期間年_8"].ToString(),dr["社内職歴期間月_8"].ToString()));    
                cr.SetParameterValue("社内職歴期間9", kikan(dr["社内職歴期間年_9"].ToString(),dr["社内職歴期間月_9"].ToString()));    
                cr.SetParameterValue("社内職歴期間10", kikan(dr["社内職歴期間年_10"].ToString(),dr["社内職歴期間月_10"].ToString()));    

                //表示用能力開発資格
                cr.SetParameterValue("能力開発資格年月1", nengetsu(dr["能力開発資格年_1"].ToString(),dr["能力開発資格月_1"].ToString()));    
                cr.SetParameterValue("能力開発資格年月2", nengetsu(dr["能力開発資格年_2"].ToString(),dr["能力開発資格月_2"].ToString()));    
                cr.SetParameterValue("能力開発資格年月3", nengetsu(dr["能力開発資格年_3"].ToString(),dr["能力開発資格月_3"].ToString()));    
                cr.SetParameterValue("能力開発資格年月4", nengetsu(dr["能力開発資格年_4"].ToString(),dr["能力開発資格月_4"].ToString()));    
                cr.SetParameterValue("能力開発資格年月5", nengetsu(dr["能力開発資格年_5"].ToString(),dr["能力開発資格月_5"].ToString()));    
                cr.SetParameterValue("能力開発資格年月6", nengetsu(dr["能力開発資格年_6"].ToString(),dr["能力開発資格月_6"].ToString()));    
                cr.SetParameterValue("能力開発資格年月7", nengetsu(dr["能力開発資格年_7"].ToString(),dr["能力開発資格月_7"].ToString()));    
                cr.SetParameterValue("能力開発資格年月8", nengetsu(dr["能力開発資格年_8"].ToString(),dr["能力開発資格月_8"].ToString()));    
                cr.SetParameterValue("能力開発資格年月9", nengetsu(dr["能力開発資格年_9"].ToString(),dr["能力開発資格月_9"].ToString()));    
                cr.SetParameterValue("能力開発資格年月10", nengetsu(dr["能力開発資格年_10"].ToString(),dr["能力開発資格月_10"].ToString()));    
                cr.SetParameterValue("能力開発資格年月11", nengetsu(dr["能力開発資格年_11"].ToString(),dr["能力開発資格月_11"].ToString()));    
                cr.SetParameterValue("能力開発資格年月12", nengetsu(dr["能力開発資格年_12"].ToString(),dr["能力開発資格月_12"].ToString()));    

                //表示用能力開発教育
                cr.SetParameterValue("能力開発教育年月1", nengetsu(dr["能力開発教育年_1"].ToString(),dr["能力開発教育月_1"].ToString()));    
                cr.SetParameterValue("能力開発教育年月2", nengetsu(dr["能力開発教育年_2"].ToString(),dr["能力開発教育月_2"].ToString()));    
                cr.SetParameterValue("能力開発教育年月3", nengetsu(dr["能力開発教育年_3"].ToString(),dr["能力開発教育月_3"].ToString()));    
                cr.SetParameterValue("能力開発教育年月4", nengetsu(dr["能力開発教育年_4"].ToString(),dr["能力開発教育月_4"].ToString()));    
                cr.SetParameterValue("能力開発教育年月5", nengetsu(dr["能力開発教育年_5"].ToString(),dr["能力開発教育月_5"].ToString()));    
                cr.SetParameterValue("能力開発教育年月6", nengetsu(dr["能力開発教育年_6"].ToString(),dr["能力開発教育月_6"].ToString()));    
                cr.SetParameterValue("能力開発教育年月7", nengetsu(dr["能力開発教育年_7"].ToString(),dr["能力開発教育月_7"].ToString()));    
                cr.SetParameterValue("能力開発教育年月8", nengetsu(dr["能力開発教育年_8"].ToString(),dr["能力開発教育月_8"].ToString()));    
                cr.SetParameterValue("能力開発教育年月9", nengetsu(dr["能力開発教育年_9"].ToString(),dr["能力開発教育月_9"].ToString()));    
                cr.SetParameterValue("能力開発教育年月10", nengetsu(dr["能力開発教育年_10"].ToString(),dr["能力開発教育月_10"].ToString()));    

            
            } catch(Exception ex) {
                // エラー
                nlog.Error(System.Reflection.MethodBase.GetCurrentMethod().Name + " error " + ex.ToString());
                throw;
            } finally {
                //終了
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " end");
            }
        }



        /// <summary>
        /// 承認者情報をレポートにセット(大区分の説明)
        /// </summary>
        /// <param name="cr">SelfDeclareReportA</param>
        /// <param name="dTable">承認者情報</param>
        private void SelfDeclareAApprovalSetting(ref SelfDeclareReportA cr, DataTable dTable) {

            try {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");

                if(dTable.Rows.Count == 0) {
                    //string[] signs = new string[] { "111", "112", "113", "114"
                    //                             , "211", "212", "213", "214"
                    //                             , "221", "223", "224" };
                    //foreach(string sign in signs) cr.SetParameterValue(sign + "承認者", "");
                    cr.SetParameterValue("111承認者", "");
                    cr.SetParameterValue("112承認者", "");
                    cr.SetParameterValue("113承認者", "");
                    cr.SetParameterValue("114承認者", "");
                    cr.SetParameterValue("211承認者", "");
                    cr.SetParameterValue("212承認者", "");
                    cr.SetParameterValue("213承認者", "");
                    cr.SetParameterValue("214承認者", "");
                    cr.SetParameterValue("221承認者", "");
                    cr.SetParameterValue("223承認者", "");
                    cr.SetParameterValue("224承認者", "");
                } else {
                    foreach(DataRow row in dTable.Rows) {
                        string param = row["大区分"].ToString() + row["中区分"].ToString()
                            + row["小区分"].ToString() + "承認者";
                        cr.SetParameterValue(param, DataConv.IfNull(row["氏名"].ToString(), ""));
                    }
                }
            } catch(Exception ex) {
                // エラー
                nlog.Error(System.Reflection.MethodBase.GetCurrentMethod().Name + " error " + ex.ToString());
                throw;
            } finally {
                //終了
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " end");
            }
        }

        /// <summary>
        /// 承認者情報をレポートにセット(大区分の説明)
        /// </summary>
        /// <param name="cr">SelfDeclareReportA</param>
        /// <param name="dTable">承認者情報</param>
        private void SelfDeclareBApprovalSetting(ref SelfDeclareReportB cr, DataTable dTable) {

            try {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");

                if(dTable.Rows.Count == 0) {
                    //string[] signs = new string[] { "111", "112", "113", "114"
                    //                             , "211", "212", "213", "214"
                    //                             , "221", "223", "224" };
                    //foreach(string sign in signs) cr.SetParameterValue(sign + "承認者", "");
                    cr.SetParameterValue("111承認者", "");
                    cr.SetParameterValue("112承認者", "");
                    cr.SetParameterValue("113承認者", "");
                    cr.SetParameterValue("114承認者", "");
                    cr.SetParameterValue("211承認者", "");
                    cr.SetParameterValue("212承認者", "");
                    cr.SetParameterValue("213承認者", "");
                    cr.SetParameterValue("214承認者", "");
                    cr.SetParameterValue("221承認者", "");
                    cr.SetParameterValue("223承認者", "");
                    cr.SetParameterValue("224承認者", "");
                } else {
                    foreach(DataRow row in dTable.Rows) {
                        string param = row["大区分"].ToString() + row["中区分"].ToString()
                            + row["小区分"].ToString() + "承認者";
                        cr.SetParameterValue(param, DataConv.IfNull(row["氏名"].ToString(), ""));
                    }
                }
            } catch(Exception ex) {
                // エラー
                nlog.Error(System.Reflection.MethodBase.GetCurrentMethod().Name + " error " + ex.ToString());
                throw;
            } finally {
                //終了
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " end");
            }
        }

        /// <summary>
        /// 承認者情報をレポートにセット(大区分の説明)
        /// </summary>
        /// <param name="cr">SelfDeclareReportA</param>
        /// <param name="dTable">承認者情報</param>
        private void SelfDeclareCApprovalSetting(ref SelfDeclareReportC cr, DataTable dTable) {

            try {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");

                if(dTable.Rows.Count == 0) {
                    //string[] signs = new string[] { "111", "112", "113", "114"
                    //                             , "211", "212", "213", "214"
                    //                             , "221", "223", "224" };
                    //foreach(string sign in signs) cr.SetParameterValue(sign + "承認者", "");
                    cr.SetParameterValue("111承認者", "");
                    cr.SetParameterValue("112承認者", "");
                    cr.SetParameterValue("113承認者", "");
                    cr.SetParameterValue("114承認者", "");
                    cr.SetParameterValue("211承認者", "");
                    cr.SetParameterValue("212承認者", "");
                    cr.SetParameterValue("213承認者", "");
                    cr.SetParameterValue("214承認者", "");
                    cr.SetParameterValue("221承認者", "");
                    cr.SetParameterValue("223承認者", "");
                    cr.SetParameterValue("224承認者", "");
                } else {
                    foreach(DataRow row in dTable.Rows) {
                        string param = row["大区分"].ToString() + row["中区分"].ToString()
                            + row["小区分"].ToString() + "承認者";
                        cr.SetParameterValue(param, DataConv.IfNull(row["氏名"].ToString(), ""));
                    }
                }
            } catch(Exception ex) {
                // エラー
                nlog.Error(System.Reflection.MethodBase.GetCurrentMethod().Name + " error " + ex.ToString());
                throw;
            } finally {
                //終了
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " end");
            }
        }

        /// <summary>
        /// 承認者情報をレポートにセット(大区分の説明)
        /// </summary>
        /// <param name="cr">SelfDeclareReportA</param>
        /// <param name="dTable">承認者情報</param>
        private void SelfDeclareDApprovalSetting(ref SelfDeclareReportD cr, DataTable dTable) {

            try {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");

                if(dTable.Rows.Count == 0) {
                    //string[] signs = new string[] { "111", "112", "113", "114"
                    //                             , "211", "212", "213", "214"
                    //                             , "221", "223", "224" };
                    //foreach(string sign in signs) cr.SetParameterValue(sign + "承認者", "");
                    cr.SetParameterValue("111承認者", "");
                    cr.SetParameterValue("112承認者", "");
                    cr.SetParameterValue("113承認者", "");
                    cr.SetParameterValue("114承認者", "");
                    cr.SetParameterValue("211承認者", "");
                    cr.SetParameterValue("212承認者", "");
                    cr.SetParameterValue("213承認者", "");
                    cr.SetParameterValue("214承認者", "");
                    cr.SetParameterValue("221承認者", "");
                    cr.SetParameterValue("223承認者", "");
                    cr.SetParameterValue("224承認者", "");
                } else {
                    foreach(DataRow row in dTable.Rows) {
                        string param = row["大区分"].ToString() + row["中区分"].ToString()
                            + row["小区分"].ToString() + "承認者";
                        cr.SetParameterValue(param, DataConv.IfNull(row["氏名"].ToString(), ""));
                    }
                }
            } catch(Exception ex) {
                // エラー
                nlog.Error(System.Reflection.MethodBase.GetCurrentMethod().Name + " error " + ex.ToString());
                throw;
            } finally {
                //終了
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " end");
            }
        }

        /// <summary>
        /// 承認者情報をレポートにセット(大区分の説明)
        /// </summary>
        /// <param name="cr">SelfDeclareReportA</param>
        /// <param name="dTable">承認者情報</param>
        private void CarrierSheetApprovalSetting(ref CarrierSheetReport cr, DataTable dTable) {

            try {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");

                if(dTable.Rows.Count == 0) {
                    //string[] signs = new string[] { "111", "112", "113", "114"
                    //                             , "211", "212", "213", "214"
                    //                             , "221", "223", "224" };
                    //foreach(string sign in signs) cr.SetParameterValue(sign + "承認者", "");
                    cr.SetParameterValue("承認11氏名", "");
                    cr.SetParameterValue("承認12氏名", "");
                    cr.SetParameterValue("承認13氏名", "");
                    cr.SetParameterValue("承認21氏名", "");
                    cr.SetParameterValue("承認22氏名", "");
                    cr.SetParameterValue("承認23氏名", "");
                    cr.SetParameterValue("承認31氏名", "");
                    cr.SetParameterValue("承認32氏名", "");
                    cr.SetParameterValue("承認33氏名", "");
                } else {
                    foreach(DataRow row in dTable.Rows) {
                        string param = "承認" + row["大区分"].ToString() + row["小区分"].ToString() + "氏名";
                        cr.SetParameterValue(param, DataConv.IfNull(row["氏名"].ToString(), ""));
                    }
                }
            } catch(Exception ex) {
                // エラー
                nlog.Error(System.Reflection.MethodBase.GetCurrentMethod().Name + " error " + ex.ToString());
                throw;
            } finally {
                //終了
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " end");
            }
        }
        
        /// <summary>
        /// 目標管理シートの基本(ヘッダー)を取得
        /// </summary>
        /// <param name="keyVal">目標管理番号</param>
        /// <returns>目標管理基本情報のDataRow</returns>
        private DataRow GetBasicSelfDeclareRow(string keyVal, DbManager dm, string tblType) {
            try {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");

                string sql = ""; //クエリ生成

                sql = "SELECT * FROM SD_T自己申告書共通基本Data WHERE 管理番号 = @key ";

                DataTable dt = new DataTable();
                DataSet dataSet = new DataSet();
                using(IDbCommand cmd = dm.CreateCommand(sql)) {
                    DbHelper.AddDbParameter(cmd, "@key", DbType.Int32);
                    //パラメータ設定
                    var parameters = cmd.Parameters.Cast<IDbDataParameter>().ToArray<IDbDataParameter>();
                    parameters[0].Value = int.Parse(keyVal);
                    //クエリ実行
                    IDataAdapter da = dm.CreateSqlDataAdapter(cmd);
                    da.Fill(dataSet);
                }

                //実行結果確認
                if(dataSet.Tables[0].Rows.Count == 0) {
                    //0件字はエラー?
                }
                return dataSet.Tables[0].Rows[0];
            } catch(Exception ex) {
                // エラー
                nlog.Error(System.Reflection.MethodBase.GetCurrentMethod().Name + " error " + ex.ToString());
                throw;
            } finally {
                //終了
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " end");
            }
        }

        /// <summary>
        /// 自己申告書データ
        /// </summary>
        /// <param name="keyVal"></param>
        /// <param name="pattern"></param>
        /// <returns></returns>
        private DataTable GetSelfDeclareTable(string keyVal, DbManager dm, string tblType) {
            try {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");

                string sql = "";
                sql = "SELECT "
                    + "共通.管理番号"
                    + ",共通.年度"
                    + ",共通.社員番号"
                    + ",共通.所属番号"
                    + ",共通.自己申告書種別Code"
                    //2021-03-26 iwai-tamura add-str ------
                    + ",共通.確定フラグ"
                    + ",共通.自己申告状態区分"
                    //2021-03-26 iwai-tamura add-end ------
                    + ",共通.氏名"
                    + ",共通.Kana"
                    + ",共通.所属名"
                    + ",共通.役職番号"
                    + ",共通.役職名"
                    + ",共通.職掌番号"
                    + ",共通.職掌名"
                    + ",共通.資格番号"
                    + ",共通.資格名"
                    + ",共通.入社年月日"
                    + ",共通.在籍月数"
                    + ",共通.現職経験月数"
                    + ",共通.生年月日"
                    + ",共通.年齢"
                    + ",共通.郵便番号_1"
                    + ",共通.郵便番号_2"
                    + ",共通.住所"
                    + ",共通.住所形態区分"
                    + ",共通.住所形態内容"
                    + ",共通.家族構成人数"
                    + ",共通.家族構成続柄区分_1"
                    + ",CASE WHEN 共通.家族構成続柄区分_1='ZZ' THEN 共通.家族構成続柄内容_1_Other ELSE 共通.家族構成続柄内容_1 END 家族構成続柄内容_1"
                    + ",共通.家族構成続柄内容_1_Other"
                    + ",共通.家族構成年齢_1"
                    + ",共通.家族構成職業学年_1"
                    + ",共通.家族構成同居区分_1"
                    + ",共通.家族構成扶養区分_1"
                    + ",共通.家族構成続柄区分_2"
                    + ",CASE WHEN 共通.家族構成続柄区分_2='ZZ' THEN 共通.家族構成続柄内容_2_Other ELSE 共通.家族構成続柄内容_2 END 家族構成続柄内容_2"
                    + ",共通.家族構成続柄内容_2_Other"
                    + ",共通.家族構成年齢_2"
                    + ",共通.家族構成職業学年_2"
                    + ",共通.家族構成同居区分_2"
                    + ",共通.家族構成扶養区分_2"
                    + ",共通.家族構成続柄区分_3"
                    + ",CASE WHEN 共通.家族構成続柄区分_3='ZZ' THEN 共通.家族構成続柄内容_3_Other ELSE 共通.家族構成続柄内容_3 END 家族構成続柄内容_3"
                    + ",共通.家族構成続柄内容_3_Other"
                    + ",共通.家族構成年齢_3"
                    + ",共通.家族構成職業学年_3"
                    + ",共通.家族構成同居区分_3"
                    + ",共通.家族構成扶養区分_3"
                    + ",共通.家族構成続柄区分_4"
                    + ",CASE WHEN 共通.家族構成続柄区分_4='ZZ' THEN 共通.家族構成続柄内容_4_Other ELSE 共通.家族構成続柄内容_4 END 家族構成続柄内容_4"
                    + ",共通.家族構成続柄内容_4_Other"
                    + ",共通.家族構成年齢_4"
                    + ",共通.家族構成職業学年_4"
                    + ",共通.家族構成同居区分_4"
                    + ",共通.家族構成扶養区分_4"
                    + ",共通.家族構成続柄区分_5"
                    + ",CASE WHEN 共通.家族構成続柄区分_5='ZZ' THEN 共通.家族構成続柄内容_5_Other ELSE 共通.家族構成続柄内容_5 END 家族構成続柄内容_5"
                    + ",共通.家族構成続柄内容_5_Other"
                    + ",共通.家族構成年齢_5"
                    + ",共通.家族構成職業学年_5"
                    + ",共通.家族構成同居区分_5"
                    + ",共通.家族構成扶養区分_5"
                    + ",共通.家族構成続柄区分_6"
                    + ",CASE WHEN 共通.家族構成続柄区分_6='ZZ' THEN 共通.家族構成続柄内容_6_Other ELSE 共通.家族構成続柄内容_6 END 家族構成続柄内容_6"
                    + ",共通.家族構成続柄内容_6_Other"
                    + ",共通.家族構成年齢_6"
                    + ",共通.家族構成職業学年_6"
                    + ",共通.家族構成同居区分_6"
                    + ",共通.家族構成扶養区分_6"
                    + ",共通.家族構成続柄区分_7"
                    + ",CASE WHEN 共通.家族構成続柄区分_7='ZZ' THEN 共通.家族構成続柄内容_7_Other ELSE 共通.家族構成続柄内容_7 END 家族構成続柄内容_7"
                    + ",共通.家族構成続柄内容_7_Other"
                    + ",共通.家族構成年齢_7"
                    + ",共通.家族構成職業学年_7"
                    + ",共通.家族構成同居区分_7"
                    + ",共通.家族構成扶養区分_7"
                    + ",共通.家族構成続柄区分_8"
                    + ",CASE WHEN 共通.家族構成続柄区分_8='ZZ' THEN 共通.家族構成続柄内容_8_Other ELSE 共通.家族構成続柄内容_8 END 家族構成続柄内容_8"
                    + ",共通.家族構成続柄内容_8_Other"
                    + ",共通.家族構成年齢_8"
                    + ",共通.家族構成職業学年_8"
                    + ",共通.家族構成同居区分_8"
                    + ",共通.家族構成扶養区分_8"
                    + ",共通.健康状態区分"
                    + ",共通.健康状態内容"
                    + ",共通.健康状態不順状態";


                //明細
                switch( tblType ){
                    case "A11":
                        sql += ",明細.担当職務_1"
                            + ",明細.担当職務_2"
                            + ",明細.担当職務_3"
                            + ",明細.担当職務_4"
                            + ",明細.適性能力開発区分_1_1_1"
                            + ",明細.適性能力開発内容_1_1_1"
                            + ",明細.適性能力開発区分_1_1_2"
                            + ",明細.適性能力開発内容_1_1_2"
                            + ",明細.適性能力開発内容_1_1_Other"
                            + ",明細.適性能力開発区分_1_2_1"
                            + ",明細.適性能力開発内容_1_2_1"
                            + ",明細.適性能力開発区分_1_2_2"
                            + ",明細.適性能力開発内容_1_2_2"
                            + ",明細.適性能力開発内容_1_2_Other"
                            + ",明細.適性能力開発内容_2"
                            + ",明細.職務変更配置換内容_1"
                            + ",明細.職務変更配置換区分_1_1"
                            + ",CASE WHEN 明細.職務変更配置換内容_1_1='ZZ' THEN 明細.職務変更配置換内容_1_1_Other ELSE 明細.職務変更配置換内容_1_1 END 職務変更配置換内容_1_1 "
                            + ",明細.職務変更配置換内容_1_1_Other"
                            + ",明細.職務変更配置換区分_1_2"
                            + ",CASE WHEN 明細.職務変更配置換内容_1_2='ZZ' THEN 明細.職務変更配置換内容_1_2_Other ELSE 明細.職務変更配置換内容_1_2 END 職務変更配置換内容_1_2 "
                            + ",明細.職務変更配置換内容_1_2_Other"
                            + ",明細.職務変更配置換区分_2_1"
                            + ",明細.職務変更配置換内容_2_1"
                            + ",明細.職務変更配置換区分_2_2"
                            + ",明細.職務変更配置換内容_2_2"
                            + ",明細.職務変更配置換区分_2_3"
                            + ",明細.職務変更配置換内容_2_3"
                            + ",明細.職務変更配置換内容_2_Other"
                            + ",明細.自由意見内容"
                            + ",明細.上司記入欄内容"
                            + ",基本情報11.社員番号 AS 承認11社員番号"
                            + ",基本情報11.氏名 AS 承認11氏名"
                            + ",基本情報12.社員番号 AS 承認12社員番号"
                            + ",基本情報12.氏名 AS 承認12氏名"
                            + " FROM SD_T自己申告書共通基本Data AS 共通"
                            + "  LEFT JOIN SD_T自己申告書明細Data" + tblType + " AS 明細 ON 共通.管理番号 = 明細.管理番号"
                            + "  LEFT JOIN SD_T自己申告書承認情報 AS 承認11 ON 共通.管理番号 = 承認11.管理番号 AND 承認11.大区分 = 1 AND 承認11.小区分 = 1 "
                            + "  LEFT JOIN SD_M人事Data基本情報 AS 基本情報11 ON 承認11.承認社員番号 = 基本情報11.社員番号"
                            + "  LEFT JOIN SD_T自己申告書承認情報 AS 承認12 ON 共通.管理番号 = 承認12.管理番号 AND 承認12.大区分 = 1 AND 承認12.小区分 = 2"
                            + "  LEFT JOIN SD_M人事Data基本情報 AS 基本情報12 ON 承認12.承認社員番号 = 基本情報12.社員番号"
                            + " WHERE 明細.管理番号 = @key";
                        break;

                    case  "A12":
                    case  "A13":
                        sql += "   ,明細.担当職務_1 "
                            + "    ,明細.担当職務区分_2_1 "
                            + "    ,CASE WHEN 明細.担当職務区分_2_1 IS NULL THEN '' ELSE '[' + 明細.担当職務区分_2_1 + ']' + 明細.担当職務内容_2_1 END 担当職務内容_2_1 "
                            + "    ,明細.担当職務区分_2_2 "
                            + "    ,CASE WHEN 明細.担当職務区分_2_2 IS NULL THEN '' ELSE '[' + 明細.担当職務区分_2_2 + ']' + 明細.担当職務内容_2_2 END 担当職務内容_2_2 "
                            + "    ,明細.担当職務区分_2_3 "
                            + "    ,CASE WHEN 明細.担当職務区分_2_3 IS NULL THEN '' ELSE '[' + 明細.担当職務区分_2_3 + ']' + 明細.担当職務内容_2_3 END 担当職務内容_2_3 "
                            + "    ,明細.担当職務区分_2_4 "
                            + "    ,CASE WHEN 明細.担当職務区分_2_4 IS NULL THEN '' ELSE '[' + 明細.担当職務区分_2_4 + ']' + 明細.担当職務内容_2_4 END 担当職務内容_2_4 "
                            + "    ,明細.担当職務区分_2_5 "
                            + "    ,CASE WHEN 明細.担当職務区分_2_5 IS NULL THEN '' ELSE '[' + 明細.担当職務区分_2_5 + ']' + 明細.担当職務内容_2_5 END 担当職務内容_2_5 "
                            + "    ,明細.担当職務区分_2_6 "
                            + "    ,CASE WHEN 明細.担当職務区分_2_6 IS NULL THEN '' ELSE '[' + 明細.担当職務区分_2_6 + ']' + 明細.担当職務内容_2_6 END 担当職務内容_2_6 "
                            + "    ,明細.担当職務区分_2_7 "
                            + "    ,CASE WHEN 明細.担当職務区分_2_7 IS NULL THEN '' ELSE '[' + 明細.担当職務区分_2_7 + ']' + 明細.担当職務内容_2_7 END 担当職務内容_2_7 "
                            + "    ,明細.担当職務_3 "
                            + "    ,明細.担当職務_4 "
                            + "    ,明細.担当職務_5 "
                            + "    ,明細.担当職務_6 "
                            + "    ,明細.適性区分_1_1_1 "
                            + "    ,明細.適性内容_1_1_1 "
                            + "    ,明細.適性区分_1_1_2 "
                            + "    ,明細.適性内容_1_1_2 "
                            + "    ,明細.適性内容_1_1_Other "
                            + "    ,明細.適性区分_1_2_1 "
                            + "    ,明細.適性内容_1_2_1 "
                            + "    ,明細.適性区分_1_2_2 "
                            + "    ,明細.適性内容_1_2_2 "
                            + "    ,明細.適性内容_1_2_Other "
                            + "    ,明細.適性職務内容_2_1 "
                            + "    ,明細.適性資格免許_2_1 "
                            + "    ,明細.適性遂行Level区分_2_1 "
                            + "    ,CASE WHEN 明細.適性遂行Level区分_2_1 IS NULL THEN '' ELSE '[' + 明細.適性遂行Level区分_2_1 + ']' + 明細.適性遂行Level内容_2_1 END 適性遂行Level内容_2_1 "
                            + "    ,明細.適性職務内容_2_2 "
                            + "    ,明細.適性資格免許_2_2 "
                            + "    ,明細.適性遂行Level区分_2_2 "
                            + "    ,CASE WHEN 明細.適性遂行Level区分_2_2 IS NULL THEN '' ELSE '[' + 明細.適性遂行Level区分_2_2 + ']' + 明細.適性遂行Level内容_2_2 END 適性遂行Level内容_2_2 "
                            + "    ,明細.適性職務内容_2_3 "
                            + "    ,明細.適性資格免許_2_3 "
                            + "    ,明細.適性遂行Level区分_2_3 "
                            + "    ,CASE WHEN 明細.適性遂行Level区分_2_3 IS NULL THEN '' ELSE '[' + 明細.適性遂行Level区分_2_3 + ']' + 明細.適性遂行Level内容_2_3 END 適性遂行Level内容_2_3 "
                            + "    ,明細.適性職務内容_2_4 "
                            + "    ,明細.適性資格免許_2_4 "
                            + "    ,明細.適性遂行Level区分_2_4 "
                            + "    ,CASE WHEN 明細.適性遂行Level区分_2_4 IS NULL THEN '' ELSE '[' + 明細.適性遂行Level区分_2_4 + ']' + 明細.適性遂行Level内容_2_4 END 適性遂行Level内容_2_4 "
                            + "    ,明細.適性職務内容_2_5 "
                            + "    ,明細.適性資格免許_2_5 "
                            + "    ,明細.適性遂行Level区分_2_5 "
                            + "    ,CASE WHEN 明細.適性遂行Level区分_2_5 IS NULL THEN '' ELSE '[' + 明細.適性遂行Level区分_2_5 + ']' + 明細.適性遂行Level内容_2_5 END 適性遂行Level内容_2_5 "
                            + "    ,明細.適性_3 "
                            + "    ,明細.配置換_1 "
                            + "    ,明細.配置換区分_1_1 "
                            + "    ,CASE WHEN 明細.配置換区分_1_1='ZZ' THEN 明細.配置換内容_1_1_Other ELSE 明細.配置換内容_1_1 END 配置換内容_1_1 "
                            + "    ,明細.配置換内容_1_1_Other "
                            + "    ,明細.配置換区分_1_2 "
                            + "    ,CASE WHEN 明細.配置換区分_1_2='ZZ' THEN 明細.配置換内容_1_2_Other ELSE 明細.配置換内容_1_2 END 配置換内容_1_2 "
                            + "    ,明細.配置換内容_1_2_Other "
                            + "    ,明細.配置換区分_2_1 "
                            + "    ,明細.配置換内容_2_1 "
                            + "    ,明細.配置換区分_2_2 "
                            + "    ,明細.配置換内容_2_2 "
                            + "    ,明細.配置換区分_2_3 "
                            + "    ,明細.配置換内容_2_3 "
                            + "    ,明細.配置換内容_2_Other "
                            + "    ,明細.能力開発_1_1 "
                            + "    ,明細.能力開発区分_1_2 "
                            + "    ,明細.能力開発内容_1_2 "
                            + "    ,明細.能力開発_1_3 "
                            + "    ,明細.能力開発_2 "
                            + "    ,明細.能力開発_3 "
                            + "    ,明細.自由意見内容 "
                            + "    ,明細.上司記入欄内容 "
                            + "    ,基本情報11.社員番号 AS 承認11社員番号"
                            + "    ,基本情報11.氏名 AS 承認11氏名"
                            + "    ,基本情報12.社員番号 AS 承認12社員番号"
                            + "    ,基本情報12.氏名 AS 承認12氏名"
                            + " FROM SD_T自己申告書共通基本Data AS 共通"
                            + "  LEFT JOIN SD_T自己申告書明細Data" + tblType + " AS 明細 ON 共通.管理番号 = 明細.管理番号"
                            + "  LEFT JOIN SD_T自己申告書承認情報 AS 承認11 ON 共通.管理番号 = 承認11.管理番号 AND 承認11.大区分 = 1 AND 承認11.小区分 = 1 "
                            + "  LEFT JOIN SD_M人事Data基本情報 AS 基本情報11 ON 承認11.承認社員番号 = 基本情報11.社員番号"
                            + "  LEFT JOIN SD_T自己申告書承認情報 AS 承認12 ON 共通.管理番号 = 承認12.管理番号 AND 承認12.大区分 = 1 AND 承認12.小区分 = 2"
                            + "  LEFT JOIN SD_M人事Data基本情報 AS 基本情報12 ON 承認12.承認社員番号 = 基本情報12.社員番号"
                            + " WHERE 明細.管理番号 = @key";
                        
                        break;
                    
                    case "B11":
                    case "B12":
                        sql += ",明細.担当職務_1"
                            + ",明細.担当職務_2"
                            + ",明細.担当職務_3"
                            + ",明細.適性能力開発区分_1_1_1"
                            + ",明細.適性能力開発内容_1_1_1"
                            + ",明細.適性能力開発区分_1_1_2"
                            + ",明細.適性能力開発内容_1_1_2"
                            + ",明細.適性能力開発内容_1_1_Other"
                            + ",明細.適性能力開発区分_1_2_1"
                            + ",明細.適性能力開発内容_1_2_1"
                            + ",明細.適性能力開発区分_1_2_2"
                            + ",明細.適性能力開発内容_1_2_2"
                            + ",明細.適性能力開発内容_1_2_Other"
                            + ",明細.適性能力開発内容_2"
                            + ",明細.職務変更配置換内容_1"
                            + ",明細.職務変更配置換区分_1_1"
                            + ",CASE WHEN 明細.職務変更配置換内容_1_1='ZZ' THEN 明細.職務変更配置換内容_1_1_Other ELSE 明細.職務変更配置換内容_1_1 END 職務変更配置換内容_1_1 "
                            + ",明細.職務変更配置換内容_1_1_Other"
                            + ",明細.職務変更配置換区分_1_2"
                            + ",CASE WHEN 明細.職務変更配置換内容_1_2='ZZ' THEN 明細.職務変更配置換内容_1_2_Other ELSE 明細.職務変更配置換内容_1_2 END 職務変更配置換内容_1_2 "
                            + ",明細.職務変更配置換内容_1_2_Other"
                            + ",明細.職務変更配置換区分_2_1"
                            + ",明細.職務変更配置換内容_2_1"
                            + ",明細.職務変更配置換区分_2_2"
                            + ",明細.職務変更配置換内容_2_2"
                            + ",明細.職務変更配置換区分_2_3"
                            + ",明細.職務変更配置換内容_2_3"
                            + ",明細.職務変更配置換内容_2_Other"
                            + ",明細.能力開発_1_1 "
                            + ",明細.能力開発区分_1_2 "
                            + ",明細.能力開発内容_1_2 "
                            + ",明細.能力開発_1_3 "
                            + ",明細.能力開発_2 "
                            + ",明細.能力開発_3 "
                            + ",明細.自由意見内容"
                            + ",明細.上司記入欄内容"
                            + ",基本情報11.社員番号 AS 承認11社員番号"
                            + ",基本情報11.氏名 AS 承認11氏名"
                            + ",基本情報12.社員番号 AS 承認12社員番号"
                            + ",基本情報12.氏名 AS 承認12氏名"
                            + " FROM SD_T自己申告書共通基本Data AS 共通"
                            + "  LEFT JOIN SD_T自己申告書明細Data" + tblType + " AS 明細 ON 共通.管理番号 = 明細.管理番号"
                            + "  LEFT JOIN SD_T自己申告書承認情報 AS 承認11 ON 共通.管理番号 = 承認11.管理番号 AND 承認11.大区分 = 1 AND 承認11.小区分 = 1 "
                            + "  LEFT JOIN SD_M人事Data基本情報 AS 基本情報11 ON 承認11.承認社員番号 = 基本情報11.社員番号"
                            + "  LEFT JOIN SD_T自己申告書承認情報 AS 承認12 ON 共通.管理番号 = 承認12.管理番号 AND 承認12.大区分 = 1 AND 承認12.小区分 = 2"
                            + "  LEFT JOIN SD_M人事Data基本情報 AS 基本情報12 ON 承認12.承認社員番号 = 基本情報12.社員番号"
                            + " WHERE 明細.管理番号 = @key";
                        break;
                    
                    case  "C11":    //自己申告書C表
                    case  "C12":    //自己申告書C表
                        sql += "   ,明細.配置換_1 "
                            + "    ,明細.配置換区分_2_1 "
                            + "    ,明細.配置換内容_2_1 "
                            + "    ,明細.配置換区分_2_2 "
                            + "    ,明細.配置換内容_2_2 "
                            + "    ,明細.配置換区分_2_3 "
                            + "    ,明細.配置換内容_2_3 "
                            + "    ,明細.配置換内容_2_Other "
                            + "    ,明細.担当職務_1 "
                            + "    ,明細.担当職務_2 "
                            + "    ,明細.能力開発_1 "
                            + "    ,明細.その他 "
                            + "    ,明細.自由意見内容 "
                            + "    ,明細.上司記入欄内容 "
                            + "    ,基本情報11.社員番号 AS 承認11社員番号"
                            + "    ,基本情報11.氏名 AS 承認11氏名"
                            + "    ,基本情報12.社員番号 AS 承認12社員番号"
                            + "    ,基本情報12.氏名 AS 承認12氏名"
                            + " FROM SD_T自己申告書共通基本Data AS 共通"
                            + "  LEFT JOIN SD_T自己申告書明細Data" + tblType + " AS 明細 ON 共通.管理番号 = 明細.管理番号"
                            + "  LEFT JOIN SD_T自己申告書承認情報 AS 承認11 ON 共通.管理番号 = 承認11.管理番号 AND 承認11.大区分 = 1 AND 承認11.小区分 = 1 "
                            + "  LEFT JOIN SD_M人事Data基本情報 AS 基本情報11 ON 承認11.承認社員番号 = 基本情報11.社員番号"
                            + "  LEFT JOIN SD_T自己申告書承認情報 AS 承認12 ON 共通.管理番号 = 承認12.管理番号 AND 承認12.大区分 = 1 AND 承認12.小区分 = 2"
                            + "  LEFT JOIN SD_M人事Data基本情報 AS 基本情報12 ON 承認12.承認社員番号 = 基本情報12.社員番号"
                            + " WHERE 明細.管理番号 = @key";
                        
                        break;
                    case  "A01":    //自己申告書A表
                        sql += ",明細.担当職務_1"
                            + ",明細.担当職務_2"
                            + ",明細.担当職務_3"
                            + ",明細.担当職務_4"
                            + ",明細.適性能力開発区分_1_1_1"
                            + ",明細.適性能力開発内容_1_1_1"
                            + ",明細.適性能力開発区分_1_1_2"
                            + ",明細.適性能力開発内容_1_1_2"
                            + ",明細.適性能力開発内容_1_1_Other"
                            + ",明細.適性能力開発区分_1_2_1"
                            + ",明細.適性能力開発内容_1_2_1"
                            + ",明細.適性能力開発区分_1_2_2"
                            + ",明細.適性能力開発内容_1_2_2"
                            + ",明細.適性能力開発内容_1_2_Other"
                            + ",明細.適性能力開発内容_2"
                            + ",明細.職務変更配置換内容_1"
                            + ",明細.職務変更配置換区分_1_1"
                            + ",CASE WHEN 明細.職務変更配置換内容_1_1='ZZ' THEN 明細.職務変更配置換内容_1_1_Other ELSE 明細.職務変更配置換内容_1_1 END 職務変更配置換内容_1_1 "
                            + ",明細.職務変更配置換内容_1_1_Other"
                            + ",明細.職務変更配置換区分_1_2"
                            + ",CASE WHEN 明細.職務変更配置換内容_1_2='ZZ' THEN 明細.職務変更配置換内容_1_2_Other ELSE 明細.職務変更配置換内容_1_2 END 職務変更配置換内容_1_2 "
                            + ",明細.職務変更配置換内容_1_2_Other"
                            + ",明細.職務変更配置換区分_2_1"
                            + ",明細.職務変更配置換内容_2_1"
                            + ",明細.職務変更配置換区分_2_2"
                            + ",明細.職務変更配置換内容_2_2"
                            + ",明細.職務変更配置換区分_2_3"
                            + ",明細.職務変更配置換内容_2_3"
                            + ",明細.職務変更配置換内容_2_Other"
                            + ",明細.自由意見内容"
                            + ",明細.上司記入欄内容"
                            + ",基本情報11.社員番号 AS 承認11社員番号"
                            + ",基本情報11.氏名 AS 承認11氏名"
                            + ",基本情報12.社員番号 AS 承認12社員番号"
                            + ",基本情報12.氏名 AS 承認12氏名"
                            + " FROM SD_T自己申告書共通基本Data AS 共通"
                            + "  LEFT JOIN SD_T自己申告書明細DataA01 AS 明細 ON 共通.管理番号 = 明細.管理番号"
                            + "  LEFT JOIN SD_T自己申告書承認情報 AS 承認11 ON 共通.管理番号 = 承認11.管理番号 AND 承認11.大区分 = 1 AND 承認11.小区分 = 1 "
                            + "  LEFT JOIN SD_M人事Data基本情報 AS 基本情報11 ON 承認11.承認社員番号 = 基本情報11.社員番号"
                            + "  LEFT JOIN SD_T自己申告書承認情報 AS 承認12 ON 共通.管理番号 = 承認12.管理番号 AND 承認12.大区分 = 1 AND 承認12.小区分 = 2"
                            + "  LEFT JOIN SD_M人事Data基本情報 AS 基本情報12 ON 承認12.承認社員番号 = 基本情報12.社員番号"
                            + " WHERE 明細.管理番号 = @key";
                        break;
    
                    case  "B01":    //自己申告書B表
                        sql += "   ,明細.担当職務_1 "
                            + "    ,明細.担当職務区分_2_1 "
                            + "    ,CASE WHEN 明細.担当職務区分_2_1 IS NULL THEN '' ELSE '[' + 明細.担当職務区分_2_1 + ']' + 明細.担当職務内容_2_1 END 担当職務内容_2_1 "
                            + "    ,明細.担当職務区分_2_2 "
                            + "    ,CASE WHEN 明細.担当職務区分_2_2 IS NULL THEN '' ELSE '[' + 明細.担当職務区分_2_2 + ']' + 明細.担当職務内容_2_2 END 担当職務内容_2_2 "
                            + "    ,明細.担当職務区分_2_3 "
                            + "    ,CASE WHEN 明細.担当職務区分_2_3 IS NULL THEN '' ELSE '[' + 明細.担当職務区分_2_3 + ']' + 明細.担当職務内容_2_3 END 担当職務内容_2_3 "
                            + "    ,明細.担当職務区分_2_4 "
                            + "    ,CASE WHEN 明細.担当職務区分_2_4 IS NULL THEN '' ELSE '[' + 明細.担当職務区分_2_4 + ']' + 明細.担当職務内容_2_4 END 担当職務内容_2_4 "
                            + "    ,明細.担当職務区分_2_5 "
                            + "    ,CASE WHEN 明細.担当職務区分_2_5 IS NULL THEN '' ELSE '[' + 明細.担当職務区分_2_5 + ']' + 明細.担当職務内容_2_5 END 担当職務内容_2_5 "
                            + "    ,明細.担当職務区分_2_6 "
                            + "    ,CASE WHEN 明細.担当職務区分_2_6 IS NULL THEN '' ELSE '[' + 明細.担当職務区分_2_6 + ']' + 明細.担当職務内容_2_6 END 担当職務内容_2_6 "
                            + "    ,明細.担当職務区分_2_7 "
                            + "    ,CASE WHEN 明細.担当職務区分_2_7 IS NULL THEN '' ELSE '[' + 明細.担当職務区分_2_7 + ']' + 明細.担当職務内容_2_7 END 担当職務内容_2_7 "
                            + "    ,明細.担当職務_3 "
                            + "    ,明細.担当職務_4 "
                            + "    ,明細.担当職務_5 "
                            + "    ,明細.担当職務_6 "
                            + "    ,明細.適性区分_1_1_1 "
                            + "    ,明細.適性内容_1_1_1 "
                            + "    ,明細.適性区分_1_1_2 "
                            + "    ,明細.適性内容_1_1_2 "
                            + "    ,明細.適性内容_1_1_Other "
                            + "    ,明細.適性区分_1_2_1 "
                            + "    ,明細.適性内容_1_2_1 "
                            + "    ,明細.適性区分_1_2_2 "
                            + "    ,明細.適性内容_1_2_2 "
                            + "    ,明細.適性内容_1_2_Other "
                            + "    ,明細.適性職務内容_2_1 "
                            + "    ,明細.適性資格免許_2_1 "
                            + "    ,明細.適性遂行Level区分_2_1 "
                            + "    ,CASE WHEN 明細.適性遂行Level区分_2_1 IS NULL THEN '' ELSE '[' + 明細.適性遂行Level区分_2_1 + ']' + 明細.適性遂行Level内容_2_1 END 適性遂行Level内容_2_1 "
                            + "    ,明細.適性職務内容_2_2 "
                            + "    ,明細.適性資格免許_2_2 "
                            + "    ,明細.適性遂行Level区分_2_2 "
                            + "    ,CASE WHEN 明細.適性遂行Level区分_2_2 IS NULL THEN '' ELSE '[' + 明細.適性遂行Level区分_2_2 + ']' + 明細.適性遂行Level内容_2_2 END 適性遂行Level内容_2_2 "
                            + "    ,明細.適性職務内容_2_3 "
                            + "    ,明細.適性資格免許_2_3 "
                            + "    ,明細.適性遂行Level区分_2_3 "
                            + "    ,CASE WHEN 明細.適性遂行Level区分_2_3 IS NULL THEN '' ELSE '[' + 明細.適性遂行Level区分_2_3 + ']' + 明細.適性遂行Level内容_2_3 END 適性遂行Level内容_2_3 "
                            + "    ,明細.適性職務内容_2_4 "
                            + "    ,明細.適性資格免許_2_4 "
                            + "    ,明細.適性遂行Level区分_2_4 "
                            + "    ,CASE WHEN 明細.適性遂行Level区分_2_4 IS NULL THEN '' ELSE '[' + 明細.適性遂行Level区分_2_4 + ']' + 明細.適性遂行Level内容_2_4 END 適性遂行Level内容_2_4 "
                            + "    ,明細.適性職務内容_2_5 "
                            + "    ,明細.適性資格免許_2_5 "
                            + "    ,明細.適性遂行Level区分_2_5 "
                            + "    ,CASE WHEN 明細.適性遂行Level区分_2_5 IS NULL THEN '' ELSE '[' + 明細.適性遂行Level区分_2_5 + ']' + 明細.適性遂行Level内容_2_5 END 適性遂行Level内容_2_5 "
                            + "    ,明細.適性_3 "
                            + "    ,明細.配置換_1 "
                            + "    ,明細.配置換区分_1_1 "
                            + "    ,CASE WHEN 明細.配置換区分_1_1='ZZ' THEN 明細.配置換内容_1_1_Other ELSE 明細.配置換内容_1_1 END 配置換内容_1_1 "
                            + "    ,明細.配置換内容_1_1_Other "
                            + "    ,明細.配置換区分_1_2 "
                            + "    ,CASE WHEN 明細.配置換区分_1_2='ZZ' THEN 明細.配置換内容_1_2_Other ELSE 明細.配置換内容_1_2 END 配置換内容_1_2 "
                            + "    ,明細.配置換内容_1_2_Other "
                            + "    ,明細.配置換区分_2_1 "
                            + "    ,明細.配置換内容_2_1 "
                            + "    ,明細.配置換区分_2_2 "
                            + "    ,明細.配置換内容_2_2 "
                            + "    ,明細.配置換区分_2_3 "
                            + "    ,明細.配置換内容_2_3 "
                            + "    ,明細.配置換内容_2_Other "
                            + "    ,明細.能力開発_1_1 "
                            + "    ,明細.能力開発区分_1_2 "
                            + "    ,明細.能力開発内容_1_2 "
                            + "    ,明細.能力開発_1_3 "
                            + "    ,明細.能力開発_2 "
                            + "    ,明細.能力開発_3 "
                            + "    ,明細.自由意見内容 "
                            + "    ,明細.上司記入欄内容 "
                            + "    ,基本情報11.社員番号 AS 承認11社員番号"
                            + "    ,基本情報11.氏名 AS 承認11氏名"
                            + "    ,基本情報12.社員番号 AS 承認12社員番号"
                            + "    ,基本情報12.氏名 AS 承認12氏名"
                            + " FROM SD_T自己申告書共通基本Data AS 共通"
                            + "  LEFT JOIN SD_T自己申告書明細DataB01 AS 明細 ON 共通.管理番号 = 明細.管理番号"
                            + "  LEFT JOIN SD_T自己申告書承認情報 AS 承認11 ON 共通.管理番号 = 承認11.管理番号 AND 承認11.大区分 = 1 AND 承認11.小区分 = 1 "
                            + "  LEFT JOIN SD_M人事Data基本情報 AS 基本情報11 ON 承認11.承認社員番号 = 基本情報11.社員番号"
                            + "  LEFT JOIN SD_T自己申告書承認情報 AS 承認12 ON 共通.管理番号 = 承認12.管理番号 AND 承認12.大区分 = 1 AND 承認12.小区分 = 2"
                            + "  LEFT JOIN SD_M人事Data基本情報 AS 基本情報12 ON 承認12.承認社員番号 = 基本情報12.社員番号"
                            + " WHERE 明細.管理番号 = @key";
                        
                        break;

                    case  "C01":    //自己申告書C表
                        sql += "   ,明細.配置換_1 "
                            + "    ,明細.配置換区分_2_1 "
                            + "    ,明細.配置換内容_2_1 "
                            + "    ,明細.配置換区分_2_2 "
                            + "    ,明細.配置換内容_2_2 "
                            + "    ,明細.配置換区分_2_3 "
                            + "    ,明細.配置換内容_2_3 "
                            + "    ,明細.配置換内容_2_Other "
                            + "    ,明細.担当職務_1 "
                            + "    ,明細.担当職務_2 "
                            + "    ,明細.能力開発_1 "
                            + "    ,明細.その他 "
                            + "    ,明細.自由意見内容 "
                            + "    ,明細.上司記入欄内容 "
                            + "    ,基本情報11.社員番号 AS 承認11社員番号"
                            + "    ,基本情報11.氏名 AS 承認11氏名"
                            + "    ,基本情報12.社員番号 AS 承認12社員番号"
                            + "    ,基本情報12.氏名 AS 承認12氏名"
                            + " FROM SD_T自己申告書共通基本Data AS 共通"
                            + "  LEFT JOIN SD_T自己申告書明細DataC01 AS 明細 ON 共通.管理番号 = 明細.管理番号"
                            + "  LEFT JOIN SD_T自己申告書承認情報 AS 承認11 ON 共通.管理番号 = 承認11.管理番号 AND 承認11.大区分 = 1 AND 承認11.小区分 = 1 "
                            + "  LEFT JOIN SD_M人事Data基本情報 AS 基本情報11 ON 承認11.承認社員番号 = 基本情報11.社員番号"
                            + "  LEFT JOIN SD_T自己申告書承認情報 AS 承認12 ON 共通.管理番号 = 承認12.管理番号 AND 承認12.大区分 = 1 AND 承認12.小区分 = 2"
                            + "  LEFT JOIN SD_M人事Data基本情報 AS 基本情報12 ON 承認12.承認社員番号 = 基本情報12.社員番号"
                            + " WHERE 明細.管理番号 = @key";
                        
                        break;

                    //2021-12-24 iwai-tamura add-str ------
                    case  "A20":
                        sql += "   ,明細.担当職務_1 "
                            + "    ,明細.担当職務区分_2_1 "
                            + "    ,CASE WHEN 明細.担当職務区分_2_1 IS NULL THEN '' ELSE '[' + 明細.担当職務区分_2_1 + ']' + 明細.担当職務内容_2_1 END 担当職務内容_2_1 "
                            + "    ,明細.担当職務区分_2_2 "
                            + "    ,CASE WHEN 明細.担当職務区分_2_2 IS NULL THEN '' ELSE '[' + 明細.担当職務区分_2_2 + ']' + 明細.担当職務内容_2_2 END 担当職務内容_2_2 "
                            + "    ,明細.担当職務区分_2_3 "
                            + "    ,CASE WHEN 明細.担当職務区分_2_3 IS NULL THEN '' ELSE '[' + 明細.担当職務区分_2_3 + ']' + 明細.担当職務内容_2_3 END 担当職務内容_2_3 "
                            + "    ,明細.担当職務区分_2_4 "
                            + "    ,CASE WHEN 明細.担当職務区分_2_4 IS NULL THEN '' ELSE '[' + 明細.担当職務区分_2_4 + ']' + 明細.担当職務内容_2_4 END 担当職務内容_2_4 "
                            + "    ,明細.担当職務区分_2_5 "
                            + "    ,CASE WHEN 明細.担当職務区分_2_5 IS NULL THEN '' ELSE '[' + 明細.担当職務区分_2_5 + ']' + 明細.担当職務内容_2_5 END 担当職務内容_2_5 "
                            + "    ,明細.担当職務区分_2_6 "
                            + "    ,CASE WHEN 明細.担当職務区分_2_6 IS NULL THEN '' ELSE '[' + 明細.担当職務区分_2_6 + ']' + 明細.担当職務内容_2_6 END 担当職務内容_2_6 "
                            + "    ,明細.担当職務区分_2_7 "
                            + "    ,CASE WHEN 明細.担当職務区分_2_7 IS NULL THEN '' ELSE '[' + 明細.担当職務区分_2_7 + ']' + 明細.担当職務内容_2_7 END 担当職務内容_2_7 "
                            + "    ,明細.担当職務_3 "
                            + "    ,明細.適性区分_1_1_1 "
                            + "    ,明細.適性内容_1_1_1 "
                            + "    ,明細.適性区分_1_1_2 "
                            + "    ,明細.適性内容_1_1_2 "
                            + "    ,明細.適性内容_1_1_Other "
                            + "    ,明細.適性区分_1_2_1 "
                            + "    ,明細.適性内容_1_2_1 "
                            + "    ,明細.適性区分_1_2_2 "
                            + "    ,明細.適性内容_1_2_2 "
                            + "    ,明細.適性内容_1_2_Other "
                            + "    ,明細.適性職務内容_2_1 "
                            + "    ,明細.適性資格免許_2_1 "
                            + "    ,明細.適性遂行Level区分_2_1 "
                            + "    ,CASE WHEN 明細.適性遂行Level区分_2_1 IS NULL THEN '' ELSE '[' + 明細.適性遂行Level区分_2_1 + ']' + 明細.適性遂行Level内容_2_1 END 適性遂行Level内容_2_1 "
                            + "    ,明細.適性職務内容_2_2 "
                            + "    ,明細.適性資格免許_2_2 "
                            + "    ,明細.適性遂行Level区分_2_2 "
                            + "    ,CASE WHEN 明細.適性遂行Level区分_2_2 IS NULL THEN '' ELSE '[' + 明細.適性遂行Level区分_2_2 + ']' + 明細.適性遂行Level内容_2_2 END 適性遂行Level内容_2_2 "
                            + "    ,明細.適性職務内容_2_3 "
                            + "    ,明細.適性資格免許_2_3 "
                            + "    ,明細.適性遂行Level区分_2_3 "
                            + "    ,CASE WHEN 明細.適性遂行Level区分_2_3 IS NULL THEN '' ELSE '[' + 明細.適性遂行Level区分_2_3 + ']' + 明細.適性遂行Level内容_2_3 END 適性遂行Level内容_2_3 "
                            + "    ,明細.適性職務内容_2_4 "
                            + "    ,明細.適性資格免許_2_4 "
                            + "    ,明細.適性遂行Level区分_2_4 "
                            + "    ,CASE WHEN 明細.適性遂行Level区分_2_4 IS NULL THEN '' ELSE '[' + 明細.適性遂行Level区分_2_4 + ']' + 明細.適性遂行Level内容_2_4 END 適性遂行Level内容_2_4 "
                            + "    ,明細.適性職務内容_2_5 "
                            + "    ,明細.適性資格免許_2_5 "
                            + "    ,明細.適性遂行Level区分_2_5 "
                            + "    ,CASE WHEN 明細.適性遂行Level区分_2_5 IS NULL THEN '' ELSE '[' + 明細.適性遂行Level区分_2_5 + ']' + 明細.適性遂行Level内容_2_5 END 適性遂行Level内容_2_5 "
                            + "    ,明細.適性_3 "
                            + "    ,明細.配置換_1 "
                            + "    ,明細.配置換区分_1_1 "
                            + "    ,CASE WHEN 明細.配置換区分_1_1='ZZ' THEN 明細.配置換内容_1_1_Other ELSE 明細.配置換内容_1_1 END 配置換内容_1_1 "
                            + "    ,明細.配置換内容_1_1_Other "
                            + "    ,明細.配置換区分_1_2 "
                            + "    ,CASE WHEN 明細.配置換区分_1_2='ZZ' THEN 明細.配置換内容_1_2_Other ELSE 明細.配置換内容_1_2 END 配置換内容_1_2 "
                            + "    ,明細.配置換内容_1_2_Other "
                            + "    ,明細.配置換区分_2_1 "
                            + "    ,明細.配置換内容_2_1 "
                            + "    ,明細.配置換区分_2_2 "
                            + "    ,明細.配置換内容_2_2 "
                            + "    ,明細.配置換区分_2_3 "
                            + "    ,明細.配置換内容_2_3 "
                            + "    ,明細.配置換内容_2_Other "
                            + "    ,明細.定年退職後生活設計_1 "
                            + "    ,明細.自由意見内容 "
                            + "    ,明細.上司記入欄内容 "
                            + "    ,基本情報11.社員番号 AS 承認11社員番号"
                            + "    ,基本情報11.氏名 AS 承認11氏名"
                            + "    ,基本情報12.社員番号 AS 承認12社員番号"
                            + "    ,基本情報12.氏名 AS 承認12氏名"
                            + " FROM SD_T自己申告書共通基本Data AS 共通"
                            + "  LEFT JOIN SD_T自己申告書明細Data" + tblType + " AS 明細 ON 共通.管理番号 = 明細.管理番号"
                            + "  LEFT JOIN SD_T自己申告書承認情報 AS 承認11 ON 共通.管理番号 = 承認11.管理番号 AND 承認11.大区分 = 1 AND 承認11.小区分 = 1 "
                            + "  LEFT JOIN SD_M人事Data基本情報 AS 基本情報11 ON 承認11.承認社員番号 = 基本情報11.社員番号"
                            + "  LEFT JOIN SD_T自己申告書承認情報 AS 承認12 ON 共通.管理番号 = 承認12.管理番号 AND 承認12.大区分 = 1 AND 承認12.小区分 = 2"
                            + "  LEFT JOIN SD_M人事Data基本情報 AS 基本情報12 ON 承認12.承認社員番号 = 基本情報12.社員番号"
                            + " WHERE 明細.管理番号 = @key";
                        
                        break;

                    case  "B20":
                        sql += "   ,明細.担当職務_1 "
                            + "    ,明細.担当職務区分_2_1 "
                            + "    ,CASE WHEN 明細.担当職務区分_2_1 IS NULL THEN '' ELSE '[' + 明細.担当職務区分_2_1 + ']' + 明細.担当職務内容_2_1 END 担当職務内容_2_1 "
                            + "    ,明細.担当職務区分_2_2 "
                            + "    ,CASE WHEN 明細.担当職務区分_2_2 IS NULL THEN '' ELSE '[' + 明細.担当職務区分_2_2 + ']' + 明細.担当職務内容_2_2 END 担当職務内容_2_2 "
                            + "    ,明細.担当職務区分_2_3 "
                            + "    ,CASE WHEN 明細.担当職務区分_2_3 IS NULL THEN '' ELSE '[' + 明細.担当職務区分_2_3 + ']' + 明細.担当職務内容_2_3 END 担当職務内容_2_3 "
                            + "    ,明細.担当職務区分_2_4 "
                            + "    ,CASE WHEN 明細.担当職務区分_2_4 IS NULL THEN '' ELSE '[' + 明細.担当職務区分_2_4 + ']' + 明細.担当職務内容_2_4 END 担当職務内容_2_4 "
                            + "    ,明細.担当職務区分_2_5 "
                            + "    ,CASE WHEN 明細.担当職務区分_2_5 IS NULL THEN '' ELSE '[' + 明細.担当職務区分_2_5 + ']' + 明細.担当職務内容_2_5 END 担当職務内容_2_5 "
                            + "    ,明細.担当職務区分_2_6 "
                            + "    ,CASE WHEN 明細.担当職務区分_2_6 IS NULL THEN '' ELSE '[' + 明細.担当職務区分_2_6 + ']' + 明細.担当職務内容_2_6 END 担当職務内容_2_6 "
                            + "    ,明細.担当職務区分_2_7 "
                            + "    ,CASE WHEN 明細.担当職務区分_2_7 IS NULL THEN '' ELSE '[' + 明細.担当職務区分_2_7 + ']' + 明細.担当職務内容_2_7 END 担当職務内容_2_7 "
                            + "    ,明細.担当職務区分_2_8 "
                            + "    ,CASE WHEN 明細.担当職務区分_2_8 IS NULL THEN '' ELSE '[' + 明細.担当職務区分_2_8 + ']' + 明細.担当職務内容_2_8 END 担当職務内容_2_8 "
                            + "    ,明細.担当職務_3 "
                            + "    ,明細.担当職務_4 "
                            + "    ,明細.適性区分_1_1_1 "
                            + "    ,明細.適性内容_1_1_1 "
                            + "    ,明細.適性区分_1_1_2 "
                            + "    ,明細.適性内容_1_1_2 "
                            + "    ,明細.適性内容_1_1_Other "
                            + "    ,明細.適性区分_1_2_1 "
                            + "    ,明細.適性内容_1_2_1 "
                            + "    ,明細.適性区分_1_2_2 "
                            + "    ,明細.適性内容_1_2_2 "
                            + "    ,明細.適性内容_1_2_Other "
                            + "    ,明細.適性職務内容_2_1 "
                            + "    ,明細.適性資格免許_2_1 "
                            + "    ,明細.適性遂行Level区分_2_1 "
                            + "    ,CASE WHEN 明細.適性遂行Level区分_2_1 IS NULL THEN '' ELSE '[' + 明細.適性遂行Level区分_2_1 + ']' + 明細.適性遂行Level内容_2_1 END 適性遂行Level内容_2_1 "
                            + "    ,明細.適性職務内容_2_2 "
                            + "    ,明細.適性資格免許_2_2 "
                            + "    ,明細.適性遂行Level区分_2_2 "
                            + "    ,CASE WHEN 明細.適性遂行Level区分_2_2 IS NULL THEN '' ELSE '[' + 明細.適性遂行Level区分_2_2 + ']' + 明細.適性遂行Level内容_2_2 END 適性遂行Level内容_2_2 "
                            + "    ,明細.適性職務内容_2_3 "
                            + "    ,明細.適性資格免許_2_3 "
                            + "    ,明細.適性遂行Level区分_2_3 "
                            + "    ,CASE WHEN 明細.適性遂行Level区分_2_3 IS NULL THEN '' ELSE '[' + 明細.適性遂行Level区分_2_3 + ']' + 明細.適性遂行Level内容_2_3 END 適性遂行Level内容_2_3 "
                            + "    ,明細.適性職務内容_2_4 "
                            + "    ,明細.適性資格免許_2_4 "
                            + "    ,明細.適性遂行Level区分_2_4 "
                            + "    ,CASE WHEN 明細.適性遂行Level区分_2_4 IS NULL THEN '' ELSE '[' + 明細.適性遂行Level区分_2_4 + ']' + 明細.適性遂行Level内容_2_4 END 適性遂行Level内容_2_4 "
                            + "    ,明細.適性職務内容_2_5 "
                            + "    ,明細.適性資格免許_2_5 "
                            + "    ,明細.適性遂行Level区分_2_5 "
                            + "    ,CASE WHEN 明細.適性遂行Level区分_2_5 IS NULL THEN '' ELSE '[' + 明細.適性遂行Level区分_2_5 + ']' + 明細.適性遂行Level内容_2_5 END 適性遂行Level内容_2_5 "
                            + "    ,明細.適性_3 "
                            + "    ,明細.配置換_1 "
                            + "    ,明細.配置換区分_1_1 "
                            + "    ,CASE WHEN 明細.配置換区分_1_1='ZZ' THEN 明細.配置換内容_1_1_Other ELSE 明細.配置換内容_1_1 END 配置換内容_1_1 "
                            + "    ,明細.配置換内容_1_1_Other "
                            + "    ,明細.配置換区分_1_2 "
                            + "    ,CASE WHEN 明細.配置換区分_1_2='ZZ' THEN 明細.配置換内容_1_2_Other ELSE 明細.配置換内容_1_2 END 配置換内容_1_2 "
                            + "    ,明細.配置換内容_1_2_Other "
                            + "    ,明細.配置換区分_2_1 "
                            + "    ,明細.配置換内容_2_1 "
                            + "    ,明細.配置換区分_2_2 "
                            + "    ,明細.配置換内容_2_2 "
                            + "    ,明細.配置換区分_2_3 "
                            + "    ,明細.配置換内容_2_3 "
                            + "    ,明細.配置換内容_2_Other "
                            + "    ,明細.定年退職後生活設計_1 "
                            + "    ,明細.自由意見内容 "
                            + "    ,明細.上司記入欄内容 "
                            + "    ,基本情報11.社員番号 AS 承認11社員番号"
                            + "    ,基本情報11.氏名 AS 承認11氏名"
                            + "    ,基本情報12.社員番号 AS 承認12社員番号"
                            + "    ,基本情報12.氏名 AS 承認12氏名"
                            + " FROM SD_T自己申告書共通基本Data AS 共通"
                            + "  LEFT JOIN SD_T自己申告書明細Data" + tblType + " AS 明細 ON 共通.管理番号 = 明細.管理番号"
                            + "  LEFT JOIN SD_T自己申告書承認情報 AS 承認11 ON 共通.管理番号 = 承認11.管理番号 AND 承認11.大区分 = 1 AND 承認11.小区分 = 1 "
                            + "  LEFT JOIN SD_M人事Data基本情報 AS 基本情報11 ON 承認11.承認社員番号 = 基本情報11.社員番号"
                            + "  LEFT JOIN SD_T自己申告書承認情報 AS 承認12 ON 共通.管理番号 = 承認12.管理番号 AND 承認12.大区分 = 1 AND 承認12.小区分 = 2"
                            + "  LEFT JOIN SD_M人事Data基本情報 AS 基本情報12 ON 承認12.承認社員番号 = 基本情報12.社員番号"
                            + " WHERE 明細.管理番号 = @key";
                        
                        break;                    

                    case  "C20":
                        sql += "   ,明細.配置換_1 "
                            + "    ,明細.配置換区分_2_1 "
                            + "    ,明細.配置換内容_2_1 "
                            + "    ,明細.配置換区分_2_2 "
                            + "    ,明細.配置換内容_2_2 "
                            + "    ,明細.配置換区分_2_3 "
                            + "    ,明細.配置換内容_2_3 "
                            + "    ,明細.配置換内容_2_Other "
                            + "    ,明細.定年退職後生活設計_1 "
                            + "    ,明細.自由意見内容 "
                            + "    ,明細.上司記入欄内容 "
                            + "    ,基本情報11.社員番号 AS 承認11社員番号"
                            + "    ,基本情報11.氏名 AS 承認11氏名"
                            + "    ,基本情報12.社員番号 AS 承認12社員番号"
                            + "    ,基本情報12.氏名 AS 承認12氏名"
                            + " FROM SD_T自己申告書共通基本Data AS 共通"
                            + "  LEFT JOIN SD_T自己申告書明細Data" + tblType + " AS 明細 ON 共通.管理番号 = 明細.管理番号"
                            + "  LEFT JOIN SD_T自己申告書承認情報 AS 承認11 ON 共通.管理番号 = 承認11.管理番号 AND 承認11.大区分 = 1 AND 承認11.小区分 = 1 "
                            + "  LEFT JOIN SD_M人事Data基本情報 AS 基本情報11 ON 承認11.承認社員番号 = 基本情報11.社員番号"
                            + "  LEFT JOIN SD_T自己申告書承認情報 AS 承認12 ON 共通.管理番号 = 承認12.管理番号 AND 承認12.大区分 = 1 AND 承認12.小区分 = 2"
                            + "  LEFT JOIN SD_M人事Data基本情報 AS 基本情報12 ON 承認12.承認社員番号 = 基本情報12.社員番号"
                            + " WHERE 明細.管理番号 = @key";
                        
                        break;                    
                    //2021-12-24 iwai-tamura add-end ------
                    
                    case  "D01":    //自己申告書B表
                        sql += "   ,明細.職務変更配置換区分_1_1"
                            + "    ,明細.職務変更配置換内容_1_1"
                            + "    ,明細.職務変更配置換区分_1_2"
                            + "    ,明細.職務変更配置換内容_1_2"
                            + "    ,明細.職務変更配置換区分_2_1_1"
                            + "    ,明細.職務変更配置換内容_2_1_1"
                            + "    ,CAST(明細.職務変更配置換年_2_1_1 AS VARCHAR(2)) as 職務変更配置換年_2_1_1"
                            + "    ,明細.職務変更配置換区分_2_1_2_1"
                            + "    ,明細.職務変更配置換内容_2_1_2_1"
                            + "    ,明細.職務変更配置換区分_2_1_2_2"
                            + "    ,明細.職務変更配置換内容_2_1_2_2"
                            + "    ,明細.職務変更配置換区分_2_1_2_3"
                            + "    ,明細.職務変更配置換内容_2_1_2_3"
                            + "    ,CAST(明細.職務変更配置換時間_2_1_2_Hour AS VARCHAR(2)) as 職務変更配置換時間_2_1_2_Hour"
                            + "    ,CAST(明細.職務変更配置換内容_2_1_2_Minute AS VARCHAR(2)) as 職務変更配置換内容_2_1_2_Minute"
                            + "    ,明細.職務変更配置換内容_2_1_2_Other"
                            + "    ,明細.職務変更配置換区分_2_2_1"
                            + "    ,明細.職務変更配置換内容_2_2_1"
                            + "    ,明細.職務変更配置換区分_2_2_2_1"
                            + "    ,明細.職務変更配置換内容_2_2_2_1"
                            + "    ,明細.職務変更配置換区分_2_2_2_2"
                            + "    ,明細.職務変更配置換内容_2_2_2_2"
                            + "    ,明細.職務変更配置換区分_2_2_2_3"
                            + "    ,明細.職務変更配置換内容_2_2_2_3"
                            + "    ,CAST(明細.職務変更配置換時間_2_2_2_Hour AS VARCHAR(2)) as 職務変更配置換時間_2_2_2_Hour"
                            + "    ,CAST(明細.職務変更配置換内容_2_2_2_Minute AS VARCHAR(2)) as 職務変更配置換内容_2_2_2_Minute"
                            + "    ,明細.職務変更配置換内容_2_2_2_Other"
                            + "    ,明細.職務変更配置換区分_3_1"
                            + "    ,明細.職務変更配置換内容_3_1"
                            + "    ,明細.職務変更配置換区分_3_2"
                            + "    ,明細.職務変更配置換内容_3_2"
                            + "    ,明細.職務変更配置換区分_4_1_1"
                            + "    ,明細.職務変更配置換内容_4_1_1"
                            + "    ,明細.職務変更配置換区分_4_1_2"
                            + "    ,明細.職務変更配置換内容_4_1_2"
                            + "    ,明細.職務変更配置換区分_4_1_3"
                            + "    ,明細.職務変更配置換内容_4_1_3"
                            + "    ,明細.職務変更配置換内容_4_1_Other"
                            + "    ,明細.職務変更配置換区分_4_1_Location"
                            + "    ,明細.職務変更配置換内容_4_1_Location"
                            + "    ,明細.職務変更配置換区分_4_2_1_1"
                            + "    ,明細.職務変更配置換内容_4_2_1_1"
                            + "    ,明細.職務変更配置換区分_4_2_1_2"
                            + "    ,明細.職務変更配置換内容_4_2_1_2"
                            + "    ,明細.職務変更配置換区分_4_2_2_1"
                            + "    ,明細.職務変更配置換内容_4_2_2_1"
                            + "    ,明細.職務変更配置換区分_4_2_2_2"
                            + "    ,明細.職務変更配置換内容_4_2_2_2"
                            + "    ,明細.職務変更配置換_5"
                            + "    ,明細.職務変更配置換_6"
                            + "    ,明細.退職生活設計区分_1_1"
                            + "    ,明細.退職生活設計内容_1_1"
                            + "    ,明細.退職生活設計_1_2_1"
                            + "    ,明細.退職生活設計_1_2_2"
                            + "    ,明細.退職生活設計_1_2_3"
                            + "    ,明細.退職生活設計_2"
                            + "    ,明細.退職生活設計_3"
                            + "    ,明細.自由意見内容"
                            + "    ,基本情報21.社員番号 AS 承認21社員番号"
                            + "    ,基本情報21.氏名 AS 承認21氏名"
                            + "    ,基本情報22.社員番号 AS 承認22社員番号"
                            + "    ,基本情報22.氏名 AS 承認22氏名 "
                            + " FROM SD_T自己申告書共通基本Data AS 共通 "
                            + " LEFT JOIN SD_T自己申告書明細DataD01 AS 明細 ON 共通.管理番号 = 明細.管理番号 "
                            + " LEFT JOIN SD_T自己申告書承認情報 AS 承認21 ON 共通.管理番号 = 承認21.管理番号 AND 承認21.大区分 = 2 AND 承認21.小区分 = 1 "
                            + " LEFT JOIN SD_M人事Data基本情報 AS 基本情報21 ON 承認21.承認社員番号 = 基本情報21.社員番号 "
                            + " LEFT JOIN SD_T自己申告書承認情報 AS 承認22 ON 共通.管理番号 = 承認22.管理番号 AND 承認22.大区分 = 2 AND 承認22.小区分 = 2 "
                            + " LEFT JOIN SD_M人事Data基本情報 AS 基本情報22 ON 承認22.承認社員番号 = 基本情報22.社員番号 "
                            + " WHERE 明細.管理番号 = @key";
                        
                        break;
                            
                    case  "CH01":   //キャリアシート
                        sql += "    ,基本.身分ｺｰﾄﾞ AS 身分番号 "
                            + "     ,基本.身分名 AS 身分名 "
                            + "     ,基本.現資格在級月数 "
                            + "     ,明細.社外職歴年_1 "
                            + "     ,明細.社外職歴月_1 "
                            + "     ,明細.社外職歴内容_1 "
                            + "     ,明細.社外職務業務_1 "
                            + "     ,明細.社外職歴期間年_1 "
                            + "     ,明細.社外職歴期間月_1 "
                            + "     ,明細.社外職歴年_2 "
                            + "     ,明細.社外職歴月_2 "
                            + "     ,明細.社外職歴内容_2 "
                            + "     ,明細.社外職務業務_2 "
                            + "     ,明細.社外職歴期間年_2 "
                            + "     ,明細.社外職歴期間月_2 "
                            + "     ,明細.社外職歴年_3 "
                            + "     ,明細.社外職歴月_3 "
                            + "     ,明細.社外職歴内容_3 "
                            + "     ,明細.社外職務業務_3 "
                            + "     ,明細.社外職歴期間年_3 "
                            + "     ,明細.社外職歴期間月_3 "
                            + "     ,明細.社外職歴年_4 "
                            + "     ,明細.社外職歴月_4 "
                            + "     ,明細.社外職歴内容_4 "
                            + "     ,明細.社外職務業務_4 "
                            + "     ,明細.社外職歴期間年_4 "
                            + "     ,明細.社外職歴期間月_4 "
                            + "     ,明細.社内職歴年_1 "
                            + "     ,明細.社内職歴月_1 "
                            + "     ,明細.社内職歴所属_1 "
                            + "     ,明細.社内職歴業務区分_1 "
                            + "     ,CASE WHEN 明細.社内職歴業務区分_1='ZZ' THEN 明細.社内職歴業務内容_1_Other ELSE 明細.社内職歴業務内容_1 END 社内職歴業務内容_1 "
                            + "     ,明細.社内職歴業務内容_1_Other "
                            + "     ,明細.社内職歴期間年_1 "
                            + "     ,明細.社内職歴期間月_1 "
                            + "     ,明細.社内職歴年_2 "
                            + "     ,明細.社内職歴月_2 "
                            + "     ,明細.社内職歴所属_2 "
                            + "     ,明細.社内職歴業務区分_2 "
                            + "     ,CASE WHEN 明細.社内職歴業務区分_2='ZZ' THEN 明細.社内職歴業務内容_2_Other ELSE 明細.社内職歴業務内容_2 END 社内職歴業務内容_2 "
                            + "     ,明細.社内職歴業務内容_2_Other "
                            + "     ,明細.社内職歴期間年_2 "
                            + "     ,明細.社内職歴期間月_2 "
                            + "     ,明細.社内職歴年_3 "
                            + "     ,明細.社内職歴月_3 "
                            + "     ,明細.社内職歴所属_3 "
                            + "     ,明細.社内職歴業務区分_3 "
                            + "     ,CASE WHEN 明細.社内職歴業務区分_3='ZZ' THEN 明細.社内職歴業務内容_3_Other ELSE 明細.社内職歴業務内容_3 END 社内職歴業務内容_3 "
                            + "     ,明細.社内職歴業務内容_3_Other "
                            + "     ,明細.社内職歴期間年_3 "
                            + "     ,明細.社内職歴期間月_3 "
                            + "     ,明細.社内職歴年_4 "
                            + "     ,明細.社内職歴月_4 "
                            + "     ,明細.社内職歴所属_4 "
                            + "     ,明細.社内職歴業務区分_4 "
                            + "     ,CASE WHEN 明細.社内職歴業務区分_4='ZZ' THEN 明細.社内職歴業務内容_4_Other ELSE 明細.社内職歴業務内容_4 END 社内職歴業務内容_4 "
                            + "     ,明細.社内職歴業務内容_4_Other "
                            + "     ,明細.社内職歴期間年_4 "
                            + "     ,明細.社内職歴期間月_4 "
                            + "     ,明細.社内職歴年_5 "
                            + "     ,明細.社内職歴月_5 "
                            + "     ,明細.社内職歴所属_5 "
                            + "     ,明細.社内職歴業務区分_5 "
                            + "     ,CASE WHEN 明細.社内職歴業務区分_5='ZZ' THEN 明細.社内職歴業務内容_5_Other ELSE 明細.社内職歴業務内容_5 END 社内職歴業務内容_5 "
                            + "     ,明細.社内職歴業務内容_5_Other "
                            + "     ,明細.社内職歴期間年_5 "
                            + "     ,明細.社内職歴期間月_5 "
                            + "     ,明細.社内職歴年_6 "
                            + "     ,明細.社内職歴月_6 "
                            + "     ,明細.社内職歴所属_6 "
                            + "     ,明細.社内職歴業務区分_6 "
                            + "     ,CASE WHEN 明細.社内職歴業務区分_6='ZZ' THEN 明細.社内職歴業務内容_6_Other ELSE 明細.社内職歴業務内容_6 END 社内職歴業務内容_6 "
                            + "     ,明細.社内職歴業務内容_6_Other "
                            + "     ,明細.社内職歴期間年_6 "
                            + "     ,明細.社内職歴期間月_6 "
                            + "     ,明細.社内職歴年_7 "
                            + "     ,明細.社内職歴月_7 "
                            + "     ,明細.社内職歴所属_7 "
                            + "     ,明細.社内職歴業務区分_7 "
                            + "     ,CASE WHEN 明細.社内職歴業務区分_7='ZZ' THEN 明細.社内職歴業務内容_7_Other ELSE 明細.社内職歴業務内容_7 END 社内職歴業務内容_7 "
                            + "     ,明細.社内職歴業務内容_7_Other "
                            + "     ,明細.社内職歴期間年_7 "
                            + "     ,明細.社内職歴期間月_7 "
                            + "     ,明細.社内職歴年_8 "
                            + "     ,明細.社内職歴月_8 "
                            + "     ,明細.社内職歴所属_8 "
                            + "     ,明細.社内職歴業務区分_8 "
                            + "     ,CASE WHEN 明細.社内職歴業務区分_8='ZZ' THEN 明細.社内職歴業務内容_8_Other ELSE 明細.社内職歴業務内容_8 END 社内職歴業務内容_8 "
                            + "     ,明細.社内職歴業務内容_8_Other "
                            + "     ,明細.社内職歴期間年_8 "
                            + "     ,明細.社内職歴期間月_8 "
                            + "     ,明細.社内職歴年_9 "
                            + "     ,明細.社内職歴月_9 "
                            + "     ,明細.社内職歴所属_9 "
                            + "     ,明細.社内職歴業務区分_9 "
                            + "     ,CASE WHEN 明細.社内職歴業務区分_9='ZZ' THEN 明細.社内職歴業務内容_9_Other ELSE 明細.社内職歴業務内容_9 END 社内職歴業務内容_9 "
                            + "     ,明細.社内職歴業務内容_9_Other "
                            + "     ,明細.社内職歴期間年_9 "
                            + "     ,明細.社内職歴期間月_9 "
                            + "     ,明細.社内職歴年_10 "
                            + "     ,明細.社内職歴月_10 "
                            + "     ,明細.社内職歴所属_10 "
                            + "     ,明細.社内職歴業務区分_10 "
                            + "     ,CASE WHEN 明細.社内職歴業務区分_10='ZZ' THEN 明細.社内職歴業務内容_10_Other ELSE 明細.社内職歴業務内容_10 END 社内職歴業務内容_10 "
                            + "     ,明細.社内職歴業務内容_10_Other "
                            + "     ,明細.社内職歴期間年_10 "
                            + "     ,明細.社内職歴期間月_10 "
                            + "     ,明細.能力開発資格年_1 "
                            + "     ,明細.能力開発資格月_1 "
                            + "     ,明細.能力開発資格名称_1 "
                            + "     ,明細.能力開発資格種類_1 "
                            + "     ,明細.能力開発資格年_2 "
                            + "     ,明細.能力開発資格月_2 "
                            + "     ,明細.能力開発資格名称_2 "
                            + "     ,明細.能力開発資格種類_2 "
                            + "     ,明細.能力開発資格年_3 "
                            + "     ,明細.能力開発資格月_3 "
                            + "     ,明細.能力開発資格名称_3 "
                            + "     ,明細.能力開発資格種類_3 "
                            + "     ,明細.能力開発資格年_4 "
                            + "     ,明細.能力開発資格月_4 "
                            + "     ,明細.能力開発資格名称_4 "
                            + "     ,明細.能力開発資格種類_4 "
                            + "     ,明細.能力開発資格年_5 "
                            + "     ,明細.能力開発資格月_5 "
                            + "     ,明細.能力開発資格名称_5 "
                            + "     ,明細.能力開発資格種類_5 "
                            + "     ,明細.能力開発資格年_6 "
                            + "     ,明細.能力開発資格月_6 "
                            + "     ,明細.能力開発資格名称_6 "
                            + "     ,明細.能力開発資格種類_6 "
                            + "     ,明細.能力開発資格年_7 "
                            + "     ,明細.能力開発資格月_7 "
                            + "     ,明細.能力開発資格名称_7 "
                            + "     ,明細.能力開発資格種類_7 "
                            + "     ,明細.能力開発資格年_8 "
                            + "     ,明細.能力開発資格月_8 "
                            + "     ,明細.能力開発資格名称_8 "
                            + "     ,明細.能力開発資格種類_8 "
                            + "     ,明細.能力開発資格年_9 "
                            + "     ,明細.能力開発資格月_9 "
                            + "     ,明細.能力開発資格名称_9 "
                            + "     ,明細.能力開発資格種類_9 "
                            + "     ,明細.能力開発資格年_10 "
                            + "     ,明細.能力開発資格月_10 "
                            + "     ,明細.能力開発資格名称_10 "
                            + "     ,明細.能力開発資格種類_10 "
                            + "     ,明細.能力開発資格年_11 "
                            + "     ,明細.能力開発資格月_11 "
                            + "     ,明細.能力開発資格名称_11 "
                            + "     ,明細.能力開発資格種類_11 "
                            + "     ,明細.能力開発資格年_12 "
                            + "     ,明細.能力開発資格月_12 "
                            + "     ,明細.能力開発資格名称_12 "
                            + "     ,明細.能力開発資格種類_12 "
                            + "     ,明細.能力開発教育年_1 "
                            + "     ,明細.能力開発教育月_1 "
                            + "     ,明細.能力開発教育名称_1 "
                            + "     ,明細.能力開発教育備考_1 "
                            + "     ,明細.能力開発教育年_2 "
                            + "     ,明細.能力開発教育月_2 "
                            + "     ,明細.能力開発教育名称_2 "
                            + "     ,明細.能力開発教育備考_2 "
                            + "     ,明細.能力開発教育年_3 "
                            + "     ,明細.能力開発教育月_3 "
                            + "     ,明細.能力開発教育名称_3 "
                            + "     ,明細.能力開発教育備考_3 "
                            + "     ,明細.能力開発教育年_4 "
                            + "     ,明細.能力開発教育月_4 "
                            + "     ,明細.能力開発教育名称_4 "
                            + "     ,明細.能力開発教育備考_4 "
                            + "     ,明細.能力開発教育年_5 "
                            + "     ,明細.能力開発教育月_5 "
                            + "     ,明細.能力開発教育名称_5 "
                            + "     ,明細.能力開発教育備考_5 "
                            + "     ,明細.能力開発教育年_6 "
                            + "     ,明細.能力開発教育月_6 "
                            + "     ,明細.能力開発教育名称_6 "
                            + "     ,明細.能力開発教育備考_6 "
                            + "     ,明細.能力開発教育年_7 "
                            + "     ,明細.能力開発教育月_7 "
                            + "     ,明細.能力開発教育名称_7 "
                            + "     ,明細.能力開発教育備考_7 "
                            + "     ,明細.能力開発教育年_8 "
                            + "     ,明細.能力開発教育月_8 "
                            + "     ,明細.能力開発教育名称_8 "
                            + "     ,明細.能力開発教育備考_8 "
                            + "     ,明細.能力開発教育年_9 "
                            + "     ,明細.能力開発教育月_9 "
                            + "     ,明細.能力開発教育名称_9 "
                            + "     ,明細.能力開発教育備考_9 "
                            + "     ,明細.能力開発教育年_10 "
                            + "     ,明細.能力開発教育月_10 "
                            + "     ,明細.能力開発教育名称_10 "
                            + "     ,明細.能力開発教育備考_10 "
                            + "     ,明細.自由意見内容 "
                            + "     ,基本情報11.社員番号 AS 承認11社員番号 "
                            + "     ,基本情報11.氏名 AS 承認11氏名 "
                            + "     ,基本情報12.社員番号 AS 承認12社員番号 "
                            + "     ,基本情報12.氏名 AS 承認12氏名 "
                            + "     ,基本情報13.社員番号 AS 承認13社員番号 "
                            + "     ,基本情報13.氏名 AS 承認13氏名 "
                            + "     ,基本情報21.社員番号 AS 承認21社員番号 "
                            + "     ,基本情報21.氏名 AS 承認21氏名 "
                            + "     ,基本情報22.社員番号 AS 承認22社員番号 "
                            + "     ,基本情報22.氏名 AS 承認22氏名 "
                            + "     ,基本情報23.社員番号 AS 承認23社員番号 "
                            + "     ,基本情報23.氏名 AS 承認23氏名 "
                            + "     ,基本情報31.社員番号 AS 承認31社員番号 "
                            + "     ,基本情報31.氏名 AS 承認31氏名 "
                            + "     ,基本情報32.社員番号 AS 承認32社員番号 "
                            + "     ,基本情報32.氏名 AS 承認32氏名 "
                            + "     ,基本情報33.社員番号 AS 承認33社員番号 "
                            + "     ,基本情報33.氏名 AS 承認33氏名 "
                            + " FROM SD_T自己申告書共通基本Data AS 共通 "
                            + " LEFT JOIN SD_M自己申告書基本情報 AS 基本 ON 共通.年度 = 基本.年度 AND 共通.社員番号 = 基本.社員番号 "
                            + " LEFT JOIN SD_TCareerSheet01 AS 明細 ON 共通.管理番号 = 明細.管理番号 "
                            + " LEFT JOIN SD_TCareerSheet承認情報 AS 承認11 ON 共通.管理番号 = 承認11.管理番号 AND 承認11.大区分 = 1 AND 承認11.小区分 = 1 "
                            + " LEFT JOIN SD_M人事Data基本情報 AS 基本情報11 ON 承認11.承認社員番号 = 基本情報11.社員番号 "
                            + " LEFT JOIN SD_TCareerSheet承認情報 AS 承認12 ON 共通.管理番号 = 承認12.管理番号 AND 承認12.大区分 = 1 AND 承認12.小区分 = 2 "
                            + " LEFT JOIN SD_M人事Data基本情報 AS 基本情報12 ON 承認12.承認社員番号 = 基本情報12.社員番号 "
                            + " LEFT JOIN SD_TCareerSheet承認情報 AS 承認13 ON 共通.管理番号 = 承認13.管理番号 AND 承認13.大区分 = 1 AND 承認13.小区分 = 3 "
                            + " LEFT JOIN SD_M人事Data基本情報 AS 基本情報13 ON 承認13.承認社員番号 = 基本情報13.社員番号 "
                            + " LEFT JOIN SD_TCareerSheet承認情報 AS 承認21 ON 共通.管理番号 = 承認21.管理番号 AND 承認21.大区分 = 2 AND 承認21.小区分 = 1 "
                            + " LEFT JOIN SD_M人事Data基本情報 AS 基本情報21 ON 承認21.承認社員番号 = 基本情報21.社員番号 "
                            + " LEFT JOIN SD_TCareerSheet承認情報 AS 承認22 ON 共通.管理番号 = 承認22.管理番号 AND 承認22.大区分 = 2 AND 承認22.小区分 = 2 "
                            + " LEFT JOIN SD_M人事Data基本情報 AS 基本情報22 ON 承認22.承認社員番号 = 基本情報22.社員番号 "
                            + " LEFT JOIN SD_TCareerSheet承認情報 AS 承認23 ON 共通.管理番号 = 承認23.管理番号 AND 承認23.大区分 = 2 AND 承認23.小区分 = 3 "
                            + " LEFT JOIN SD_M人事Data基本情報 AS 基本情報23 ON 承認23.承認社員番号 = 基本情報23.社員番号 "
                            + " LEFT JOIN SD_TCareerSheet承認情報 AS 承認31 ON 共通.管理番号 = 承認31.管理番号 AND 承認31.大区分 = 3 AND 承認31.小区分 = 1 "
                            + " LEFT JOIN SD_M人事Data基本情報 AS 基本情報31 ON 承認31.承認社員番号 = 基本情報31.社員番号 "
                            + " LEFT JOIN SD_TCareerSheet承認情報 AS 承認32 ON 共通.管理番号 = 承認32.管理番号 AND 承認32.大区分 = 3 AND 承認32.小区分 = 2 "
                            + " LEFT JOIN SD_M人事Data基本情報 AS 基本情報32 ON 承認32.承認社員番号 = 基本情報32.社員番号 "
                            + " LEFT JOIN SD_TCareerSheet承認情報 AS 承認33 ON 共通.管理番号 = 承認33.管理番号 AND 承認33.大区分 = 3 AND 承認33.小区分 = 3 "
                            + " LEFT JOIN SD_M人事Data基本情報 AS 基本情報33 ON 承認33.承認社員番号 = 基本情報33.社員番号 "
                            + " WHERE 明細.管理番号 = @key";
                        break;
                }


                DataTable dt = new DataTable();
                DataSet dataSet = new DataSet();
                using(IDbCommand cmd = dm.CreateCommand(sql)) {
                    DbHelper.AddDbParameter(cmd, "@key", DbType.Int32);
                    //パラメータ設定
                    var parameters = cmd.Parameters.Cast<IDbDataParameter>().ToArray<IDbDataParameter>();
                    parameters[0].Value = int.Parse(keyVal);
                    //クエリ実行
                    IDataAdapter da = dm.CreateSqlDataAdapter(cmd);
                    da.Fill(dataSet);
                }

                return dataSet.Tables[0];

            } catch(Exception ex) {
                // エラー
                nlog.Error(System.Reflection.MethodBase.GetCurrentMethod().Name + " error " + ex.ToString());
                throw;
            } finally {
                //終了
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " end");
            }
        }


        /// <summary>
        /// 自己申告書データ
        /// </summary>
        /// <param name="keyVal"></param>
        /// <param name="pattern"></param>
        /// <returns></returns>
        //private DataTable GetSelfDeclareTableXls(string keyVal, DbManager dm, string tblType) {
        //    try {
        //        //開始
        //        nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");

        //        string sql = "";
        //        sql = "SELECT "
        //            + "共通.管理番号"
        //            + ",共通.年度"
        //            + ",共通.社員番号"
        //            + ",共通.所属番号"
        //            + ",共通.自己申告書種別Code"
        //            + ",共通.氏名"
        //            + ",共通.Kana"
        //            + ",共通.所属名"
        //            + ",共通.役職番号"
        //            + ",共通.役職名"
        //            + ",共通.職掌番号"
        //            + ",共通.職掌名"
        //            + ",共通.資格番号"
        //            + ",共通.資格名"
        //            + ",共通.入社年月日"
        //            + ",共通.在籍月数"
        //            + ",共通.現職経験月数"
        //            + ",共通.生年月日"
        //            + ",共通.年齢"
        //            + ",共通.郵便番号_1"
        //            + ",共通.郵便番号_2"
        //            + ",共通.住所"
        //            + ",共通.住所形態区分"
        //            + ",共通.住所形態内容"
        //            + ",共通.家族構成人数"
        //            + ",共通.家族構成続柄区分_1"
        //            + ",CASE WHEN 共通.家族構成続柄区分_1='ZZ' THEN 共通.家族構成続柄内容_1_Other ELSE 共通.家族構成続柄内容_1 END 家族構成続柄内容_1"
        //            + ",共通.家族構成続柄内容_1_Other"
        //            + ",共通.家族構成年齢_1"
        //            + ",共通.家族構成職業学年_1"
        //            + ",共通.家族構成同居区分_1"
        //            + ",共通.家族構成扶養区分_1"
        //            + ",共通.家族構成続柄区分_2"
        //            + ",CASE WHEN 共通.家族構成続柄区分_2='ZZ' THEN 共通.家族構成続柄内容_2_Other ELSE 共通.家族構成続柄内容_2 END 家族構成続柄内容_2"
        //            + ",共通.家族構成続柄内容_2_Other"
        //            + ",共通.家族構成年齢_2"
        //            + ",共通.家族構成職業学年_2"
        //            + ",共通.家族構成同居区分_2"
        //            + ",共通.家族構成扶養区分_2"
        //            + ",共通.家族構成続柄区分_3"
        //            + ",CASE WHEN 共通.家族構成続柄区分_3='ZZ' THEN 共通.家族構成続柄内容_3_Other ELSE 共通.家族構成続柄内容_3 END 家族構成続柄内容_3"
        //            + ",共通.家族構成続柄内容_3_Other"
        //            + ",共通.家族構成年齢_3"
        //            + ",共通.家族構成職業学年_3"
        //            + ",共通.家族構成同居区分_3"
        //            + ",共通.家族構成扶養区分_3"
        //            + ",共通.家族構成続柄区分_4"
        //            + ",CASE WHEN 共通.家族構成続柄区分_4='ZZ' THEN 共通.家族構成続柄内容_4_Other ELSE 共通.家族構成続柄内容_4 END 家族構成続柄内容_4"
        //            + ",共通.家族構成続柄内容_4_Other"
        //            + ",共通.家族構成年齢_4"
        //            + ",共通.家族構成職業学年_4"
        //            + ",共通.家族構成同居区分_4"
        //            + ",共通.家族構成扶養区分_4"
        //            + ",共通.家族構成続柄区分_5"
        //            + ",CASE WHEN 共通.家族構成続柄区分_5='ZZ' THEN 共通.家族構成続柄内容_5_Other ELSE 共通.家族構成続柄内容_5 END 家族構成続柄内容_5"
        //            + ",共通.家族構成続柄内容_5_Other"
        //            + ",共通.家族構成年齢_5"
        //            + ",共通.家族構成職業学年_5"
        //            + ",共通.家族構成同居区分_5"
        //            + ",共通.家族構成扶養区分_5"
        //            + ",共通.家族構成続柄区分_6"
        //            + ",CASE WHEN 共通.家族構成続柄区分_6='ZZ' THEN 共通.家族構成続柄内容_6_Other ELSE 共通.家族構成続柄内容_6 END 家族構成続柄内容_6"
        //            + ",共通.家族構成続柄内容_6_Other"
        //            + ",共通.家族構成年齢_6"
        //            + ",共通.家族構成職業学年_6"
        //            + ",共通.家族構成同居区分_6"
        //            + ",共通.家族構成扶養区分_6"
        //            + ",共通.家族構成続柄区分_7"
        //            + ",CASE WHEN 共通.家族構成続柄区分_7='ZZ' THEN 共通.家族構成続柄内容_7_Other ELSE 共通.家族構成続柄内容_7 END 家族構成続柄内容_7"
        //            + ",共通.家族構成続柄内容_7_Other"
        //            + ",共通.家族構成年齢_7"
        //            + ",共通.家族構成職業学年_7"
        //            + ",共通.家族構成同居区分_7"
        //            + ",共通.家族構成扶養区分_7"
        //            + ",共通.家族構成続柄区分_8"
        //            + ",CASE WHEN 共通.家族構成続柄区分_8='ZZ' THEN 共通.家族構成続柄内容_8_Other ELSE 共通.家族構成続柄内容_8 END 家族構成続柄内容_8"
        //            + ",共通.家族構成続柄内容_8_Other"
        //            + ",共通.家族構成年齢_8"
        //            + ",共通.家族構成職業学年_8"
        //            + ",共通.家族構成同居区分_8"
        //            + ",共通.家族構成扶養区分_8"
        //            + ",共通.健康状態区分"
        //            + ",共通.健康状態内容"
        //            + ",共通.健康状態不順状態";


        //        //明細
        //        switch( tblType ){
        //            case  "A01":    //自己申告書A表
        //                sql += ",明細.担当職務_1"
        //                    + ",明細.担当職務_2"
        //                    + ",明細.担当職務_3"
        //                    + ",明細.担当職務_4"
        //                    + ",明細.適性能力開発区分_1_1_1"
        //                    + ",明細.適性能力開発内容_1_1_1"
        //                    + ",明細.適性能力開発区分_1_1_2"
        //                    + ",明細.適性能力開発内容_1_1_2"
        //                    + ",明細.適性能力開発内容_1_1_Other"
        //                    + ",明細.適性能力開発区分_1_2_1"
        //                    + ",明細.適性能力開発内容_1_2_1"
        //                    + ",明細.適性能力開発区分_1_2_2"
        //                    + ",明細.適性能力開発内容_1_2_2"
        //                    + ",明細.適性能力開発内容_1_2_Other"
        //                    + ",明細.適性能力開発内容_2"
        //                    + ",明細.職務変更配置換内容_1"
        //                    + ",明細.職務変更配置換区分_1_1"
        //                    + ",CASE WHEN 明細.職務変更配置換内容_1_1='ZZ' THEN 明細.職務変更配置換内容_1_1_Other ELSE 明細.職務変更配置換内容_1_1 END 職務変更配置換内容_1_1 "
        //                    + ",明細.職務変更配置換内容_1_1_Other"
        //                    + ",明細.職務変更配置換区分_1_2"
        //                    + ",CASE WHEN 明細.職務変更配置換内容_1_2='ZZ' THEN 明細.職務変更配置換内容_1_2_Other ELSE 明細.職務変更配置換内容_1_2 END 職務変更配置換内容_1_2 "
        //                    + ",明細.職務変更配置換内容_1_2_Other"
        //                    + ",明細.職務変更配置換区分_2_1"
        //                    + ",明細.職務変更配置換内容_2_1"
        //                    + ",明細.職務変更配置換区分_2_2"
        //                    + ",明細.職務変更配置換内容_2_2"
        //                    + ",明細.職務変更配置換区分_2_3"
        //                    + ",明細.職務変更配置換内容_2_3"
        //                    + ",明細.職務変更配置換内容_2_Other"
        //                    + ",明細.自由意見内容"
        //                    + ",明細.上司記入欄内容"
        //                    + ",基本情報11.社員番号 AS 承認11社員番号"
        //                    + ",基本情報11.氏名 AS 承認11氏名"
        //                    + ",基本情報12.社員番号 AS 承認12社員番号"
        //                    + ",基本情報12.氏名 AS 承認12氏名"
        //                    + " FROM SD_T自己申告書共通基本Data AS 共通"
        //                    + "  LEFT JOIN SD_T自己申告書明細DataA01 AS 明細 ON 共通.管理番号 = 明細.管理番号"
        //                    + "  LEFT JOIN SD_T自己申告書承認情報 AS 承認11 ON 共通.管理番号 = 承認11.管理番号 AND 承認11.大区分 = 1 AND 承認11.小区分 = 1 "
        //                    + "  LEFT JOIN SD_M人事Data基本情報 AS 基本情報11 ON 承認11.承認社員番号 = 基本情報11.社員番号"
        //                    + "  LEFT JOIN SD_T自己申告書承認情報 AS 承認12 ON 共通.管理番号 = 承認12.管理番号 AND 承認12.大区分 = 1 AND 承認12.小区分 = 2"
        //                    + "  LEFT JOIN SD_M人事Data基本情報 AS 基本情報12 ON 承認12.承認社員番号 = 基本情報12.社員番号"
        //                    + " WHERE 明細.管理番号 = @key";
        //                break;
    
        //            case  "B01":    //自己申告書B表
        //                sql += "   ,明細.担当職務_1 "
        //                    + "    ,明細.担当職務区分_2_1 "
        //                    + "    ,CASE WHEN 明細.担当職務区分_2_1 IS NULL THEN '' ELSE '[' + 明細.担当職務区分_2_1 + ']' + 明細.担当職務内容_2_1 END 担当職務内容_2_1 "
        //                    + "    ,明細.担当職務区分_2_2 "
        //                    + "    ,CASE WHEN 明細.担当職務区分_2_2 IS NULL THEN '' ELSE '[' + 明細.担当職務区分_2_2 + ']' + 明細.担当職務内容_2_2 END 担当職務内容_2_2 "
        //                    + "    ,明細.担当職務区分_2_3 "
        //                    + "    ,CASE WHEN 明細.担当職務区分_2_3 IS NULL THEN '' ELSE '[' + 明細.担当職務区分_2_3 + ']' + 明細.担当職務内容_2_3 END 担当職務内容_2_3 "
        //                    + "    ,明細.担当職務区分_2_4 "
        //                    + "    ,CASE WHEN 明細.担当職務区分_2_4 IS NULL THEN '' ELSE '[' + 明細.担当職務区分_2_4 + ']' + 明細.担当職務内容_2_4 END 担当職務内容_2_4 "
        //                    + "    ,明細.担当職務区分_2_5 "
        //                    + "    ,CASE WHEN 明細.担当職務区分_2_5 IS NULL THEN '' ELSE '[' + 明細.担当職務区分_2_5 + ']' + 明細.担当職務内容_2_5 END 担当職務内容_2_5 "
        //                    + "    ,明細.担当職務区分_2_6 "
        //                    + "    ,CASE WHEN 明細.担当職務区分_2_6 IS NULL THEN '' ELSE '[' + 明細.担当職務区分_2_6 + ']' + 明細.担当職務内容_2_6 END 担当職務内容_2_6 "
        //                    + "    ,明細.担当職務区分_2_7 "
        //                    + "    ,CASE WHEN 明細.担当職務区分_2_7 IS NULL THEN '' ELSE '[' + 明細.担当職務区分_2_7 + ']' + 明細.担当職務内容_2_7 END 担当職務内容_2_7 "
        //                    + "    ,明細.担当職務_3 "
        //                    + "    ,明細.担当職務_4 "
        //                    + "    ,明細.担当職務_5 "
        //                    + "    ,明細.担当職務_6 "
        //                    + "    ,明細.適性区分_1_1_1 "
        //                    + "    ,明細.適性内容_1_1_1 "
        //                    + "    ,明細.適性区分_1_1_2 "
        //                    + "    ,明細.適性内容_1_1_2 "
        //                    + "    ,明細.適性内容_1_1_Other "
        //                    + "    ,明細.適性区分_1_2_1 "
        //                    + "    ,明細.適性内容_1_2_1 "
        //                    + "    ,明細.適性区分_1_2_2 "
        //                    + "    ,明細.適性内容_1_2_2 "
        //                    + "    ,明細.適性内容_1_2_Other "
        //                    + "    ,明細.適性職務内容_2_1 "
        //                    + "    ,明細.適性資格免許_2_1 "
        //                    + "    ,明細.適性遂行Level区分_2_1 "
        //                    + "    ,CASE WHEN 明細.適性遂行Level区分_2_1 IS NULL THEN '' ELSE '[' + 明細.適性遂行Level区分_2_1 + ']' + 明細.適性遂行Level内容_2_1 END 適性遂行Level内容_2_1 "
        //                    + "    ,明細.適性職務内容_2_2 "
        //                    + "    ,明細.適性資格免許_2_2 "
        //                    + "    ,明細.適性遂行Level区分_2_2 "
        //                    + "    ,CASE WHEN 明細.適性遂行Level区分_2_2 IS NULL THEN '' ELSE '[' + 明細.適性遂行Level区分_2_2 + ']' + 明細.適性遂行Level内容_2_2 END 適性遂行Level内容_2_2 "
        //                    + "    ,明細.適性職務内容_2_3 "
        //                    + "    ,明細.適性資格免許_2_3 "
        //                    + "    ,明細.適性遂行Level区分_2_3 "
        //                    + "    ,CASE WHEN 明細.適性遂行Level区分_2_3 IS NULL THEN '' ELSE '[' + 明細.適性遂行Level区分_2_3 + ']' + 明細.適性遂行Level内容_2_3 END 適性遂行Level内容_2_3 "
        //                    + "    ,明細.適性職務内容_2_4 "
        //                    + "    ,明細.適性資格免許_2_4 "
        //                    + "    ,明細.適性遂行Level区分_2_4 "
        //                    + "    ,CASE WHEN 明細.適性遂行Level区分_2_4 IS NULL THEN '' ELSE '[' + 明細.適性遂行Level区分_2_4 + ']' + 明細.適性遂行Level内容_2_4 END 適性遂行Level内容_2_4 "
        //                    + "    ,明細.適性職務内容_2_5 "
        //                    + "    ,明細.適性資格免許_2_5 "
        //                    + "    ,明細.適性遂行Level区分_2_5 "
        //                    + "    ,CASE WHEN 明細.適性遂行Level区分_2_5 IS NULL THEN '' ELSE '[' + 明細.適性遂行Level区分_2_5 + ']' + 明細.適性遂行Level内容_2_5 END 適性遂行Level内容_2_5 "
        //                    + "    ,明細.適性_3 "
        //                    + "    ,明細.配置換_1 "
        //                    + "    ,明細.配置換区分_1_1 "
        //                    + "    ,CASE WHEN 明細.配置換区分_1_1='ZZ' THEN 明細.配置換内容_1_1_Other ELSE 明細.配置換内容_1_1 END 配置換内容_1_1 "
        //                    + "    ,明細.配置換内容_1_1_Other "
        //                    + "    ,明細.配置換区分_1_2 "
        //                    + "    ,CASE WHEN 明細.配置換区分_1_2='ZZ' THEN 明細.配置換内容_1_2_Other ELSE 明細.配置換内容_1_2 END 配置換内容_1_2 "
        //                    + "    ,明細.配置換内容_1_2_Other "
        //                    + "    ,明細.配置換区分_2_1 "
        //                    + "    ,明細.配置換内容_2_1 "
        //                    + "    ,明細.配置換区分_2_2 "
        //                    + "    ,明細.配置換内容_2_2 "
        //                    + "    ,明細.配置換区分_2_3 "
        //                    + "    ,明細.配置換内容_2_3 "
        //                    + "    ,明細.配置換内容_2_Other "
        //                    + "    ,明細.能力開発_1_1 "
        //                    + "    ,明細.能力開発区分_1_2 "
        //                    + "    ,明細.能力開発内容_1_2 "
        //                    + "    ,明細.能力開発_1_3 "
        //                    + "    ,明細.能力開発_2 "
        //                    + "    ,明細.能力開発_3 "
        //                    + "    ,明細.自由意見内容 "
        //                    + "    ,明細.上司記入欄内容 "
        //                    + "    ,基本情報11.社員番号 AS 承認11社員番号"
        //                    + "    ,基本情報11.氏名 AS 承認11氏名"
        //                    + "    ,基本情報12.社員番号 AS 承認12社員番号"
        //                    + "    ,基本情報12.氏名 AS 承認12氏名"
        //                    + " FROM SD_T自己申告書共通基本Data AS 共通"
        //                    + "  LEFT JOIN SD_T自己申告書明細DataB01 AS 明細 ON 共通.管理番号 = 明細.管理番号"
        //                    + "  LEFT JOIN SD_T自己申告書承認情報 AS 承認11 ON 共通.管理番号 = 承認11.管理番号 AND 承認11.大区分 = 1 AND 承認11.小区分 = 1 "
        //                    + "  LEFT JOIN SD_M人事Data基本情報 AS 基本情報11 ON 承認11.承認社員番号 = 基本情報11.社員番号"
        //                    + "  LEFT JOIN SD_T自己申告書承認情報 AS 承認12 ON 共通.管理番号 = 承認12.管理番号 AND 承認12.大区分 = 1 AND 承認12.小区分 = 2"
        //                    + "  LEFT JOIN SD_M人事Data基本情報 AS 基本情報12 ON 承認12.承認社員番号 = 基本情報12.社員番号"
        //                    + " WHERE 共通.管理番号 = @key";
                        
        //                break;

        //            case  "C01":    //自己申告書C表
        //                sql += "   ,明細.配置換_1 "
        //                    + "    ,明細.配置換区分_2_1 "
        //                    + "    ,明細.配置換内容_2_1 "
        //                    + "    ,明細.配置換区分_2_2 "
        //                    + "    ,明細.配置換内容_2_2 "
        //                    + "    ,明細.配置換区分_2_3 "
        //                    + "    ,明細.配置換内容_2_3 "
        //                    + "    ,明細.配置換内容_2_Other "
        //                    + "    ,明細.担当職務_1 "
        //                    + "    ,明細.担当職務_2 "
        //                    + "    ,明細.能力開発_1 "
        //                    + "    ,明細.その他 "
        //                    + "    ,明細.自由意見内容 "
        //                    + "    ,明細.上司記入欄内容 "
        //                    + "    ,基本情報11.社員番号 AS 承認11社員番号"
        //                    + "    ,基本情報11.氏名 AS 承認11氏名"
        //                    + "    ,基本情報12.社員番号 AS 承認12社員番号"
        //                    + "    ,基本情報12.氏名 AS 承認12氏名"
        //                    + " FROM SD_T自己申告書共通基本Data AS 共通"
        //                    + "  LEFT JOIN SD_T自己申告書明細DataC01 AS 明細 ON 共通.管理番号 = 明細.管理番号"
        //                    + "  LEFT JOIN SD_T自己申告書承認情報 AS 承認11 ON 共通.管理番号 = 承認11.管理番号 AND 承認11.大区分 = 1 AND 承認11.小区分 = 1 "
        //                    + "  LEFT JOIN SD_M人事Data基本情報 AS 基本情報11 ON 承認11.承認社員番号 = 基本情報11.社員番号"
        //                    + "  LEFT JOIN SD_T自己申告書承認情報 AS 承認12 ON 共通.管理番号 = 承認12.管理番号 AND 承認12.大区分 = 1 AND 承認12.小区分 = 2"
        //                    + "  LEFT JOIN SD_M人事Data基本情報 AS 基本情報12 ON 承認12.承認社員番号 = 基本情報12.社員番号"
        //                    + " WHERE 共通.管理番号 = @key";
                        
        //                break;

        //            case  "D01":    //自己申告書B表
        //                sql += "   ,明細.職務変更配置換区分_1_1"
        //                    + "    ,明細.職務変更配置換内容_1_1"
        //                    + "    ,明細.職務変更配置換区分_1_2"
        //                    + "    ,明細.職務変更配置換内容_1_2"
        //                    + "    ,明細.職務変更配置換区分_2_1_1"
        //                    + "    ,明細.職務変更配置換内容_2_1_1"
        //                    + "    ,CAST(明細.職務変更配置換年_2_1_1 AS VARCHAR(2)) as 職務変更配置換年_2_1_1"
        //                    + "    ,明細.職務変更配置換区分_2_1_2_1"
        //                    + "    ,明細.職務変更配置換内容_2_1_2_1"
        //                    + "    ,明細.職務変更配置換区分_2_1_2_2"
        //                    + "    ,明細.職務変更配置換内容_2_1_2_2"
        //                    + "    ,明細.職務変更配置換区分_2_1_2_3"
        //                    + "    ,明細.職務変更配置換内容_2_1_2_3"
        //                    + "    ,CAST(明細.職務変更配置換時間_2_1_2_Hour AS VARCHAR(2)) as 職務変更配置換時間_2_1_2_Hour"
        //                    + "    ,CAST(明細.職務変更配置換内容_2_1_2_Minute AS VARCHAR(2)) as 職務変更配置換内容_2_1_2_Minute"
        //                    + "    ,明細.職務変更配置換内容_2_1_2_Other"
        //                    + "    ,明細.職務変更配置換区分_2_2_1"
        //                    + "    ,明細.職務変更配置換内容_2_2_1"
        //                    + "    ,明細.職務変更配置換区分_2_2_2_1"
        //                    + "    ,明細.職務変更配置換内容_2_2_2_1"
        //                    + "    ,明細.職務変更配置換区分_2_2_2_2"
        //                    + "    ,明細.職務変更配置換内容_2_2_2_2"
        //                    + "    ,明細.職務変更配置換区分_2_2_2_3"
        //                    + "    ,明細.職務変更配置換内容_2_2_2_3"
        //                    + "    ,CAST(明細.職務変更配置換時間_2_2_2_Hour AS VARCHAR(2)) as 職務変更配置換時間_2_2_2_Hour"
        //                    + "    ,CAST(明細.職務変更配置換内容_2_2_2_Minute AS VARCHAR(2)) as 職務変更配置換内容_2_2_2_Minute"
        //                    + "    ,明細.職務変更配置換内容_2_2_2_Other"
        //                    + "    ,明細.職務変更配置換区分_3_1"
        //                    + "    ,明細.職務変更配置換内容_3_1"
        //                    + "    ,明細.職務変更配置換区分_3_2"
        //                    + "    ,明細.職務変更配置換内容_3_2"
        //                    + "    ,明細.職務変更配置換区分_4_1_1"
        //                    + "    ,明細.職務変更配置換内容_4_1_1"
        //                    + "    ,明細.職務変更配置換区分_4_1_2"
        //                    + "    ,明細.職務変更配置換内容_4_1_2"
        //                    + "    ,明細.職務変更配置換区分_4_1_3"
        //                    + "    ,明細.職務変更配置換内容_4_1_3"
        //                    + "    ,明細.職務変更配置換内容_4_1_Other"
        //                    + "    ,明細.職務変更配置換区分_4_1_Location"
        //                    + "    ,明細.職務変更配置換内容_4_1_Location"
        //                    + "    ,明細.職務変更配置換区分_4_2_1_1"
        //                    + "    ,明細.職務変更配置換内容_4_2_1_1"
        //                    + "    ,明細.職務変更配置換区分_4_2_1_2"
        //                    + "    ,明細.職務変更配置換内容_4_2_1_2"
        //                    + "    ,明細.職務変更配置換区分_4_2_2_1"
        //                    + "    ,明細.職務変更配置換内容_4_2_2_1"
        //                    + "    ,明細.職務変更配置換区分_4_2_2_2"
        //                    + "    ,明細.職務変更配置換内容_4_2_2_2"
        //                    + "    ,明細.職務変更配置換_5"
        //                    + "    ,明細.職務変更配置換_6"
        //                    + "    ,明細.退職生活設計区分_1_1"
        //                    + "    ,明細.退職生活設計内容_1_1"
        //                    + "    ,明細.退職生活設計_1_2_1"
        //                    + "    ,明細.退職生活設計_1_2_2"
        //                    + "    ,明細.退職生活設計_1_2_3"
        //                    + "    ,明細.退職生活設計_2"
        //                    + "    ,明細.退職生活設計_3"
        //                    + "    ,明細.自由意見内容"
        //                    + "    ,基本情報21.社員番号 AS 承認21社員番号"
        //                    + "    ,基本情報21.氏名 AS 承認21氏名"
        //                    + "    ,基本情報22.社員番号 AS 承認22社員番号"
        //                    + "    ,基本情報22.氏名 AS 承認22氏名 "
        //                    + " FROM SD_T自己申告書共通基本Data AS 共通 "
        //                    + " LEFT JOIN SD_T自己申告書明細DataD01 AS 明細 ON 共通.管理番号 = 明細.管理番号 "
        //                    + " LEFT JOIN SD_T自己申告書承認情報 AS 承認21 ON 共通.管理番号 = 承認21.管理番号 AND 承認21.大区分 = 2 AND 承認21.小区分 = 1 "
        //                    + " LEFT JOIN SD_M人事Data基本情報 AS 基本情報21 ON 承認21.承認社員番号 = 基本情報21.社員番号 "
        //                    + " LEFT JOIN SD_T自己申告書承認情報 AS 承認22 ON 共通.管理番号 = 承認22.管理番号 AND 承認22.大区分 = 2 AND 承認22.小区分 = 2 "
        //                    + " LEFT JOIN SD_M人事Data基本情報 AS 基本情報22 ON 承認22.承認社員番号 = 基本情報22.社員番号 "
        //                    + " WHERE 共通.管理番号 = @key";
                        
        //                break;
                            
        //            case  "CH01":   //キャリアシート
        //                sql += "   ,明細.社外職歴年_1"
        //                    + "     ,明細.社外職歴月_1 "
        //                    + "     ,明細.社外職歴内容_1 "
        //                    + "     ,明細.社外職務業務_1 "
        //                    + "     ,明細.社外職歴期間年_1 "
        //                    + "     ,明細.社外職歴期間月_1 "
        //                    + "     ,明細.社外職歴年_2 "
        //                    + "     ,明細.社外職歴月_2 "
        //                    + "     ,明細.社外職歴内容_2 "
        //                    + "     ,明細.社外職務業務_2 "
        //                    + "     ,明細.社外職歴期間年_2 "
        //                    + "     ,明細.社外職歴期間月_2 "
        //                    + "     ,明細.社外職歴年_3 "
        //                    + "     ,明細.社外職歴月_3 "
        //                    + "     ,明細.社外職歴内容_3 "
        //                    + "     ,明細.社外職務業務_3 "
        //                    + "     ,明細.社外職歴期間年_3 "
        //                    + "     ,明細.社外職歴期間月_3 "
        //                    + "     ,明細.社外職歴年_4 "
        //                    + "     ,明細.社外職歴月_4 "
        //                    + "     ,明細.社外職歴内容_4 "
        //                    + "     ,明細.社外職務業務_4 "
        //                    + "     ,明細.社外職歴期間年_4 "
        //                    + "     ,明細.社外職歴期間月_4 "
        //                    + "     ,明細.社内職歴年_1 "
        //                    + "     ,明細.社内職歴月_1 "
        //                    + "     ,明細.社内職歴所属_1 "
        //                    + "     ,明細.社内職歴業務区分_1 "
        //                    + "     ,CASE WHEN 明細.社内職歴業務区分_1='ZZ' THEN 明細.社内職歴業務内容_1_Other ELSE 明細.社内職歴業務内容_1 END 社内職歴業務内容_1 "
        //                    + "     ,明細.社内職歴業務内容_1_Other "
        //                    + "     ,明細.社内職歴期間年_1 "
        //                    + "     ,明細.社内職歴期間月_1 "
        //                    + "     ,明細.社内職歴年_2 "
        //                    + "     ,明細.社内職歴月_2 "
        //                    + "     ,明細.社内職歴所属_2 "
        //                    + "     ,明細.社内職歴業務区分_2 "
        //                    + "     ,CASE WHEN 明細.社内職歴業務区分_2='ZZ' THEN 明細.社内職歴業務内容_2_Other ELSE 明細.社内職歴業務内容_2 END 社内職歴業務内容_2 "
        //                    + "     ,明細.社内職歴業務内容_2_Other "
        //                    + "     ,明細.社内職歴期間年_2 "
        //                    + "     ,明細.社内職歴期間月_2 "
        //                    + "     ,明細.社内職歴年_3 "
        //                    + "     ,明細.社内職歴月_3 "
        //                    + "     ,明細.社内職歴所属_3 "
        //                    + "     ,明細.社内職歴業務区分_3 "
        //                    + "     ,CASE WHEN 明細.社内職歴業務区分_3='ZZ' THEN 明細.社内職歴業務内容_3_Other ELSE 明細.社内職歴業務内容_3 END 社内職歴業務内容_3 "
        //                    + "     ,明細.社内職歴業務内容_3_Other "
        //                    + "     ,明細.社内職歴期間年_3 "
        //                    + "     ,明細.社内職歴期間月_3 "
        //                    + "     ,明細.社内職歴年_4 "
        //                    + "     ,明細.社内職歴月_4 "
        //                    + "     ,明細.社内職歴所属_4 "
        //                    + "     ,明細.社内職歴業務区分_4 "
        //                    + "     ,CASE WHEN 明細.社内職歴業務区分_4='ZZ' THEN 明細.社内職歴業務内容_4_Other ELSE 明細.社内職歴業務内容_4 END 社内職歴業務内容_4 "
        //                    + "     ,明細.社内職歴業務内容_4_Other "
        //                    + "     ,明細.社内職歴期間年_4 "
        //                    + "     ,明細.社内職歴期間月_4 "
        //                    + "     ,明細.社内職歴年_5 "
        //                    + "     ,明細.社内職歴月_5 "
        //                    + "     ,明細.社内職歴所属_5 "
        //                    + "     ,明細.社内職歴業務区分_5 "
        //                    + "     ,CASE WHEN 明細.社内職歴業務区分_5='ZZ' THEN 明細.社内職歴業務内容_5_Other ELSE 明細.社内職歴業務内容_5 END 社内職歴業務内容_5 "
        //                    + "     ,明細.社内職歴業務内容_5_Other "
        //                    + "     ,明細.社内職歴期間年_5 "
        //                    + "     ,明細.社内職歴期間月_5 "
        //                    + "     ,明細.社内職歴年_6 "
        //                    + "     ,明細.社内職歴月_6 "
        //                    + "     ,明細.社内職歴所属_6 "
        //                    + "     ,明細.社内職歴業務区分_6 "
        //                    + "     ,CASE WHEN 明細.社内職歴業務区分_6='ZZ' THEN 明細.社内職歴業務内容_6_Other ELSE 明細.社内職歴業務内容_6 END 社内職歴業務内容_6 "
        //                    + "     ,明細.社内職歴業務内容_6_Other "
        //                    + "     ,明細.社内職歴期間年_6 "
        //                    + "     ,明細.社内職歴期間月_6 "
        //                    + "     ,明細.社内職歴年_7 "
        //                    + "     ,明細.社内職歴月_7 "
        //                    + "     ,明細.社内職歴所属_7 "
        //                    + "     ,明細.社内職歴業務区分_7 "
        //                    + "     ,CASE WHEN 明細.社内職歴業務区分_7='ZZ' THEN 明細.社内職歴業務内容_7_Other ELSE 明細.社内職歴業務内容_7 END 社内職歴業務内容_7 "
        //                    + "     ,明細.社内職歴業務内容_7_Other "
        //                    + "     ,明細.社内職歴期間年_7 "
        //                    + "     ,明細.社内職歴期間月_7 "
        //                    + "     ,明細.社内職歴年_8 "
        //                    + "     ,明細.社内職歴月_8 "
        //                    + "     ,明細.社内職歴所属_8 "
        //                    + "     ,明細.社内職歴業務区分_8 "
        //                    + "     ,CASE WHEN 明細.社内職歴業務区分_8='ZZ' THEN 明細.社内職歴業務内容_8_Other ELSE 明細.社内職歴業務内容_8 END 社内職歴業務内容_8 "
        //                    + "     ,明細.社内職歴業務内容_8_Other "
        //                    + "     ,明細.社内職歴期間年_8 "
        //                    + "     ,明細.社内職歴期間月_8 "
        //                    + "     ,明細.社内職歴年_9 "
        //                    + "     ,明細.社内職歴月_9 "
        //                    + "     ,明細.社内職歴所属_9 "
        //                    + "     ,明細.社内職歴業務区分_9 "
        //                    + "     ,CASE WHEN 明細.社内職歴業務区分_9='ZZ' THEN 明細.社内職歴業務内容_9_Other ELSE 明細.社内職歴業務内容_9 END 社内職歴業務内容_9 "
        //                    + "     ,明細.社内職歴業務内容_9_Other "
        //                    + "     ,明細.社内職歴期間年_9 "
        //                    + "     ,明細.社内職歴期間月_9 "
        //                    + "     ,明細.社内職歴年_10 "
        //                    + "     ,明細.社内職歴月_10 "
        //                    + "     ,明細.社内職歴所属_10 "
        //                    + "     ,明細.社内職歴業務区分_10 "
        //                    + "     ,CASE WHEN 明細.社内職歴業務区分_10='ZZ' THEN 明細.社内職歴業務内容_10_Other ELSE 明細.社内職歴業務内容_10 END 社内職歴業務内容_10 "
        //                    + "     ,明細.社内職歴業務内容_10_Other "
        //                    + "     ,明細.社内職歴期間年_10 "
        //                    + "     ,明細.社内職歴期間月_10 "
        //                    + "     ,明細.能力開発資格年_1 "
        //                    + "     ,明細.能力開発資格月_1 "
        //                    + "     ,明細.能力開発資格名称_1 "
        //                    + "     ,明細.能力開発資格種類_1 "
        //                    + "     ,明細.能力開発資格年_2 "
        //                    + "     ,明細.能力開発資格月_2 "
        //                    + "     ,明細.能力開発資格名称_2 "
        //                    + "     ,明細.能力開発資格種類_2 "
        //                    + "     ,明細.能力開発資格年_3 "
        //                    + "     ,明細.能力開発資格月_3 "
        //                    + "     ,明細.能力開発資格名称_3 "
        //                    + "     ,明細.能力開発資格種類_3 "
        //                    + "     ,明細.能力開発資格年_4 "
        //                    + "     ,明細.能力開発資格月_4 "
        //                    + "     ,明細.能力開発資格名称_4 "
        //                    + "     ,明細.能力開発資格種類_4 "
        //                    + "     ,明細.能力開発資格年_5 "
        //                    + "     ,明細.能力開発資格月_5 "
        //                    + "     ,明細.能力開発資格名称_5 "
        //                    + "     ,明細.能力開発資格種類_5 "
        //                    + "     ,明細.能力開発資格年_6 "
        //                    + "     ,明細.能力開発資格月_6 "
        //                    + "     ,明細.能力開発資格名称_6 "
        //                    + "     ,明細.能力開発資格種類_6 "
        //                    + "     ,明細.能力開発資格年_7 "
        //                    + "     ,明細.能力開発資格月_7 "
        //                    + "     ,明細.能力開発資格名称_7 "
        //                    + "     ,明細.能力開発資格種類_7 "
        //                    + "     ,明細.能力開発資格年_8 "
        //                    + "     ,明細.能力開発資格月_8 "
        //                    + "     ,明細.能力開発資格名称_8 "
        //                    + "     ,明細.能力開発資格種類_8 "
        //                    + "     ,明細.能力開発資格年_9 "
        //                    + "     ,明細.能力開発資格月_9 "
        //                    + "     ,明細.能力開発資格名称_9 "
        //                    + "     ,明細.能力開発資格種類_9 "
        //                    + "     ,明細.能力開発資格年_10 "
        //                    + "     ,明細.能力開発資格月_10 "
        //                    + "     ,明細.能力開発資格名称_10 "
        //                    + "     ,明細.能力開発資格種類_10 "
        //                    + "     ,明細.能力開発資格年_11 "
        //                    + "     ,明細.能力開発資格月_11 "
        //                    + "     ,明細.能力開発資格名称_11 "
        //                    + "     ,明細.能力開発資格種類_11 "
        //                    + "     ,明細.能力開発資格年_12 "
        //                    + "     ,明細.能力開発資格月_12 "
        //                    + "     ,明細.能力開発資格名称_12 "
        //                    + "     ,明細.能力開発資格種類_12 "
        //                    + "     ,明細.能力開発教育年_1 "
        //                    + "     ,明細.能力開発教育月_1 "
        //                    + "     ,明細.能力開発教育名称_1 "
        //                    + "     ,明細.能力開発教育備考_1 "
        //                    + "     ,明細.能力開発教育年_2 "
        //                    + "     ,明細.能力開発教育月_2 "
        //                    + "     ,明細.能力開発教育名称_2 "
        //                    + "     ,明細.能力開発教育備考_2 "
        //                    + "     ,明細.能力開発教育年_3 "
        //                    + "     ,明細.能力開発教育月_3 "
        //                    + "     ,明細.能力開発教育名称_3 "
        //                    + "     ,明細.能力開発教育備考_3 "
        //                    + "     ,明細.能力開発教育年_4 "
        //                    + "     ,明細.能力開発教育月_4 "
        //                    + "     ,明細.能力開発教育名称_4 "
        //                    + "     ,明細.能力開発教育備考_4 "
        //                    + "     ,明細.能力開発教育年_5 "
        //                    + "     ,明細.能力開発教育月_5 "
        //                    + "     ,明細.能力開発教育名称_5 "
        //                    + "     ,明細.能力開発教育備考_5 "
        //                    + "     ,明細.能力開発教育年_6 "
        //                    + "     ,明細.能力開発教育月_6 "
        //                    + "     ,明細.能力開発教育名称_6 "
        //                    + "     ,明細.能力開発教育備考_6 "
        //                    + "     ,明細.能力開発教育年_7 "
        //                    + "     ,明細.能力開発教育月_7 "
        //                    + "     ,明細.能力開発教育名称_7 "
        //                    + "     ,明細.能力開発教育備考_7 "
        //                    + "     ,明細.能力開発教育年_8 "
        //                    + "     ,明細.能力開発教育月_8 "
        //                    + "     ,明細.能力開発教育名称_8 "
        //                    + "     ,明細.能力開発教育備考_8 "
        //                    + "     ,明細.能力開発教育年_9 "
        //                    + "     ,明細.能力開発教育月_9 "
        //                    + "     ,明細.能力開発教育名称_9 "
        //                    + "     ,明細.能力開発教育備考_9 "
        //                    + "     ,明細.能力開発教育年_10 "
        //                    + "     ,明細.能力開発教育月_10 "
        //                    + "     ,明細.能力開発教育名称_10 "
        //                    + "     ,明細.能力開発教育備考_10 "
        //                    + "     ,明細.自由意見内容 "
        //                    + "     ,基本情報11.社員番号 AS 承認11社員番号 "
        //                    + "     ,基本情報11.氏名 AS 承認11氏名 "
        //                    + "     ,基本情報12.社員番号 AS 承認12社員番号 "
        //                    + "     ,基本情報12.氏名 AS 承認12氏名 "
        //                    + "     ,基本情報13.社員番号 AS 承認13社員番号 "
        //                    + "     ,基本情報13.氏名 AS 承認13氏名 "
        //                    + "     ,基本情報21.社員番号 AS 承認21社員番号 "
        //                    + "     ,基本情報21.氏名 AS 承認21氏名 "
        //                    + "     ,基本情報22.社員番号 AS 承認22社員番号 "
        //                    + "     ,基本情報22.氏名 AS 承認22氏名 "
        //                    + "     ,基本情報23.社員番号 AS 承認23社員番号 "
        //                    + "     ,基本情報23.氏名 AS 承認23氏名 "
        //                    + "     ,基本情報31.社員番号 AS 承認31社員番号 "
        //                    + "     ,基本情報31.氏名 AS 承認31氏名 "
        //                    + "     ,基本情報32.社員番号 AS 承認32社員番号 "
        //                    + "     ,基本情報32.氏名 AS 承認32氏名 "
        //                    + "     ,基本情報33.社員番号 AS 承認33社員番号 "
        //                    + "     ,基本情報33.氏名 AS 承認33氏名 "
        //                    + " FROM SD_T自己申告書共通基本Data AS 共通 "
        //                    + " LEFT JOIN SD_TCareerSheet01 AS 明細 ON 共通.管理番号 = 明細.管理番号 "
        //                    + " LEFT JOIN SD_TCareerSheet承認情報 AS 承認11 ON 共通.管理番号 = 承認11.管理番号 AND 承認11.大区分 = 1 AND 承認11.小区分 = 1 "
        //                    + " LEFT JOIN SD_M人事Data基本情報 AS 基本情報11 ON 承認11.承認社員番号 = 基本情報11.社員番号 "
        //                    + " LEFT JOIN SD_TCareerSheet承認情報 AS 承認12 ON 共通.管理番号 = 承認12.管理番号 AND 承認12.大区分 = 1 AND 承認12.小区分 = 2 "
        //                    + " LEFT JOIN SD_M人事Data基本情報 AS 基本情報12 ON 承認12.承認社員番号 = 基本情報12.社員番号 "
        //                    + " LEFT JOIN SD_TCareerSheet承認情報 AS 承認13 ON 共通.管理番号 = 承認13.管理番号 AND 承認13.大区分 = 1 AND 承認13.小区分 = 3 "
        //                    + " LEFT JOIN SD_M人事Data基本情報 AS 基本情報13 ON 承認13.承認社員番号 = 基本情報13.社員番号 "
        //                    + " LEFT JOIN SD_TCareerSheet承認情報 AS 承認21 ON 共通.管理番号 = 承認21.管理番号 AND 承認21.大区分 = 2 AND 承認21.小区分 = 1 "
        //                    + " LEFT JOIN SD_M人事Data基本情報 AS 基本情報21 ON 承認21.承認社員番号 = 基本情報21.社員番号 "
        //                    + " LEFT JOIN SD_TCareerSheet承認情報 AS 承認22 ON 共通.管理番号 = 承認22.管理番号 AND 承認22.大区分 = 2 AND 承認22.小区分 = 2 "
        //                    + " LEFT JOIN SD_M人事Data基本情報 AS 基本情報22 ON 承認22.承認社員番号 = 基本情報22.社員番号 "
        //                    + " LEFT JOIN SD_TCareerSheet承認情報 AS 承認23 ON 共通.管理番号 = 承認23.管理番号 AND 承認23.大区分 = 2 AND 承認23.小区分 = 3 "
        //                    + " LEFT JOIN SD_M人事Data基本情報 AS 基本情報23 ON 承認23.承認社員番号 = 基本情報23.社員番号 "
        //                    + " LEFT JOIN SD_TCareerSheet承認情報 AS 承認31 ON 共通.管理番号 = 承認31.管理番号 AND 承認31.大区分 = 3 AND 承認31.小区分 = 1 "
        //                    + " LEFT JOIN SD_M人事Data基本情報 AS 基本情報31 ON 承認31.承認社員番号 = 基本情報31.社員番号 "
        //                    + " LEFT JOIN SD_TCareerSheet承認情報 AS 承認32 ON 共通.管理番号 = 承認32.管理番号 AND 承認32.大区分 = 3 AND 承認32.小区分 = 2 "
        //                    + " LEFT JOIN SD_M人事Data基本情報 AS 基本情報32 ON 承認32.承認社員番号 = 基本情報32.社員番号 "
        //                    + " LEFT JOIN SD_TCareerSheet承認情報 AS 承認33 ON 共通.管理番号 = 承認33.管理番号 AND 承認33.大区分 = 3 AND 承認33.小区分 = 3 "
        //                    + " LEFT JOIN SD_M人事Data基本情報 AS 基本情報33 ON 承認33.承認社員番号 = 基本情報33.社員番号 "
        //                    + " WHERE 共通.管理番号 = @key";
        //                break;
        //        }


        //        DataTable dt = new DataTable();
        //        DataSet dataSet = new DataSet();
        //        using(IDbCommand cmd = dm.CreateCommand(sql)) {
        //            DbHelper.AddDbParameter(cmd, "@key", DbType.Int32);
        //            //パラメータ設定
        //            var parameters = cmd.Parameters.Cast<IDbDataParameter>().ToArray<IDbDataParameter>();
        //            parameters[0].Value = int.Parse(keyVal);
        //            //クエリ実行
        //            IDataAdapter da = dm.CreateSqlDataAdapter(cmd);
        //            da.Fill(dataSet);
        //        }

        //        return dataSet.Tables[0];

        //    } catch(Exception ex) {
        //        // エラー
        //        nlog.Error(System.Reflection.MethodBase.GetCurrentMethod().Name + " error " + ex.ToString());
        //        throw;
        //    } finally {
        //        //終了
        //        nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " end");
        //    }
        //}


























        // 2017-03-31 sbc-sagara add str 一括Excel出力ボタン追加
        /// <summary>
        /// Excel作成用
        /// </summary>
        /// <param name="row">目標管理基本データ</param>
        /// <param name="dataSet">目標管理承認データ</param>
        /// <param name="dt">目標管理詳細データ</param>
        /// <returns>SQL Create文</returns>
        private String CreateQuery(DataRow row,DataSet dataSet,DataTable dt)
        {
            StringBuilder sb1 = new StringBuilder();
            sb1.Append("CREATE TABLE [Sheet1](");


            //目標管理基本データを設定
            foreach (DataColumn rowcolumn in row.Table.Columns)
            {
                sb1.Append(rowcolumn.ColumnName + " text,");
            }
            
            //目標管理承認データを設定
            foreach (DataTable dataSettable in dataSet.Tables)
            {
                string columnname = "";
                foreach (DataRow dataSetrow in dataSettable.Rows)
                {
                    columnname = dataSetrow["大区分"].ToString() + dataSetrow["中区分"].ToString() + dataSetrow["小区分"].ToString() + "承認者";
                    sb1.Append(columnname + " text,");
                }
            }
            
            //目標管理詳細データを設定
            for (int i = 1; i <= dt.Rows.Count; i++)
            {
                foreach (DataColumn column in dt.Columns)
                {
                    if (sb1.ToString().IndexOf(column.ColumnName + " text,") < 0)
                    {
                        sb1.Append(column.ColumnName + i + " text,");
                    }
                }
            }
            
            
            sb1.Remove(sb1.Length - 1, 1);
            sb1.Append(")");

            return sb1.ToString();
        }

        /// <summary>
        /// Excelデータ挿入用
        /// </summary>
        /// <param name="row">目標管理基本データ</param>
        /// <param name="dataSet">目標管理承認データ</param>
        /// <param name="dt">目標管理詳細データ</param>
        /// <returns>SQL Insert文</returns>
        private String InsertQuery(DataRow row, DataSet dataSet, DataTable dt)
        {
            StringBuilder sb1 = new StringBuilder();
            StringBuilder sb2 = new StringBuilder();
            sb1.Append("INSERT INTO [Sheet1] (");
            sb2.Append(" values (");


            //目標管理基本データを設定
            foreach (DataColumn rowcolumn in row.Table.Columns)
            {
                sb1.Append(rowcolumn.ColumnName + ",");
                sb2.Append("?,");
            }

            //目標管理承認データを設定
            foreach (DataTable dataSettable in dataSet.Tables)
            {
                string columnname = "";
                foreach (DataRow dataSetrow in dataSettable.Rows)
                {
                    columnname = dataSetrow["大区分"].ToString() + dataSetrow["中区分"].ToString() + dataSetrow["小区分"].ToString() + "承認者";
                    sb1.Append(columnname + ",");
                    sb2.Append("?,");
                }
            }
            
            //目標管理詳細データを設定
            for (int i = 1; i <= dt.Rows.Count; i++)
            {
                foreach (DataColumn column in dt.Columns)
                {
                    if (sb1.ToString().IndexOf(column.ColumnName + ",") < 0)
                    {
                        sb1.Append(column.ColumnName + i + ",");
                        sb2.Append("?,");
                    }
                }
            }

            
            sb1.Remove(sb1.Length - 1, 1);
            sb2.Remove(sb2.Length - 1, 1);
            sb1.Append(")");
            sb2.Append(")");
            return sb1.ToString() + sb2.ToString();
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
        // 2017-03-31 sbc-sagara add end 一括Excel出力ボタン追加
    }
}
