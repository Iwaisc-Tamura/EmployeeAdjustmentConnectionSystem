using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using EmployeeAdjustmentConnectionSystem.BL.SelfDeclareSearch;
using EmployeeAdjustmentConnectionSystem.COM.Models;
using EmployeeAdjustmentConnectionSystem.COM.Util.Controll;
using EmployeeAdjustmentConnectionSystem.COM.Util.Database;
using System.IO;
using EmployeeAdjustmentConnectionSystem.BL.Login;
using System.Web.Services;
using EmployeeAdjustmentConnectionSystem.COM.Util.Config;
using System.Configuration;
using EmployeeAdjustmentConnectionSystem.COM.Entity.Session;
using EmployeeAdjustmentConnectionSystem.COM.Util.File;
using EmployeeAdjustmentConnectionSystem.Log.Common;    //2019-10-02 iwai-tamura add

namespace EmployeeAdjustmentConnectionSystem.Web.Controllers {
    /// <summary>
    /// 目標管理検索コントローラー
    /// </summary>
    public class SelfDeclareSearchController : Controller {

        #region メンバ変数
        /// <summary>
        /// ロガー
        /// </summary>
        private static NLog.Logger nlog = NLog.LogManager.GetCurrentClassLogger();

        #endregion

        /// <summary>
        /// 初期表示
        /// </summary>
        /// <returns></returns>
        public ActionResult Search() {
            try {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");

                //ログイン判定
                //2016-04-14 iwai-tamura upd str -----
                //if (!(new LoginBL()).IsLogin()){
                //    TempData["Confirmation"] = string.Format("権限が無い為、この機能は使えません。");
                //    return RedirectToAction("Login", "Login");
                //}
                if (!(new LoginBL()).IsLogin()) return RedirectToAction("Login", "Login");
                //2016-04-14 iwai-tamura upd end -----

                //初期化
                SelfDeclareSearchViewModels model = new SelfDeclareSearchViewModels {
                    Search = new SelfDeclareSearchModel(),
                    SearchResult = new List<SelfDeclareSearchListModel>(),
                    Down = new SelfDeclareDownLoadModel()
                };

                //2020-03-31 iwai-tamura upd str -----
                //検索年度に直近の年度を表示
                using(DbManager dm = new DbManager())
                using(IDbCommand cmd = dm.CreateCommand("SELECT MAX(年度) AS 対象年度 FROM SD_T自己申告書共通基本Data"))
                using(IDataReader reader = cmd.ExecuteReader()) {
                    while(reader.Read()) {
                        model.Search.Year = reader["対象年度"].ToString();
                    }
                }
                //model.Search.Year = "2019";
                //2020-03-31 iwai-tamura upd end -----

                //2016-01-21 iwai-tamura upd str -----
                //TempDataある場合
                if (TempData["SelfDeclareSearch"] != null)
                {
                    //詳細設定画面から戻った時に前回の検索結果データを表示する
                    model.Search = (SelfDeclareSearchModel)TempData["SelfDeclareSearch"];
                    ModelState.Clear();
                    //2020-03-31 iwai-tamura upd str -----
                    if ((string)Session["SearchType"] == "D")
                    {
                        return View((new SelfDeclareSearchBL()).SearchD(model, (LoginUser)Session["LoginUser"]));
                    //if ((string)Session["SearchType"] == "Sub")
                    //{
                    //    //「部下表示」ボタンにて検索した場合
                    //    return View((new SelfDeclareSearchBL()).SubSearch(model, (LoginUser)Session["LoginUser"]));
                    //2020-03-31 iwai-tamura upd end -----

                    }else{
                        //「検索」ボタンにて検索した場合
                        return View((new SelfDeclareSearchBL()).Search(model, (LoginUser)Session["LoginUser"]));
                    }
                }
                // 2017-03-31 sbc-sagara add str
                else
                {
                    Session["SearchType"] = "";
                }
                // 2017-03-31 sbc-sagara add end
                //2016-01-21 iwai-tamura upd end -----

                //表示
                return View(model);
            } catch(Exception ex) {
                //エラー
                nlog.Error(System.Reflection.MethodBase.GetCurrentMethod().Name + " error " + ex.ToString());
                TempData["Error"] = ex.ToString();
                return View("Error");
            } finally {
                //終了
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " end");
            }
        }

        /// <summary>
        /// 検索
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ActionName("Search")]
        [AcceptButton(ButtonName = "SearchEx")]
        public ActionResult SearchEx(SelfDeclareSearchViewModels model, string value) {
            try {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");

                //2019-10-02 iwai-tamura add-str ------
                CommonLog.WriteOperationLog((((LoginUser)Session["LoginUser"]).UserCode),"検索開始","");
                //2019-10-02 iwai-tamura add-end ------

                //ログイン判定
                if(!(new LoginBL()).IsLogin()) return RedirectToAction("Login", "Login");

                //検索時、検索方法を保存するよう変更
                Session["SearchType"] = "Main";
                
                //表示
                return View((new SelfDeclareSearchBL()).Search(model, (LoginUser)Session["LoginUser"]));
                
            } catch(Exception ex) {
                //エラー
                nlog.Error(System.Reflection.MethodBase.GetCurrentMethod().Name + " error " + ex.ToString());
                TempData["Error"] = ex.ToString();
                return View("Error");
            } finally {
                //終了
                //2019-10-02 iwai-tamura add-str ------
                CommonLog.WriteOperationLog((((LoginUser)Session["LoginUser"]).UserCode),"検索終了","");
                //2019-10-02 iwai-tamura add-end ------

                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " end");
            }
        }


        //2020-03-31 iwai-tamura add-str ------
        /// <summary>
        /// 検索
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ActionName("Search")]
        [AcceptButton(ButtonName = "SearchD")]
        public ActionResult SearchD(SelfDeclareSearchViewModels model, string value) {
            try {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");

                CommonLog.WriteOperationLog((((LoginUser)Session["LoginUser"]).UserCode),"検索開始","");

                //ログイン判定
                if(!(new LoginBL()).IsLogin()) return RedirectToAction("Login", "Login");

                //検索時、検索方法を保存するよう変更
                Session["SearchType"] = "D";
                
                //表示
                return View((new SelfDeclareSearchBL()).SearchD(model, (LoginUser)Session["LoginUser"]));
                
            } catch(Exception ex) {
                //エラー
                nlog.Error(System.Reflection.MethodBase.GetCurrentMethod().Name + " error " + ex.ToString());
                TempData["Error"] = ex.ToString();
                return View("Error");
            } finally {
                //終了
                CommonLog.WriteOperationLog((((LoginUser)Session["LoginUser"]).UserCode),"検索終了","");

                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " end");
            }
        }
        //2020-03-31 iwai-tamura add-end ------
        
        /// <summary>
        /// 本人入力
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ActionName("Search")]
        [AcceptButton(ButtonName = "SelfInput")]
        public ActionResult SelfInput(SelfDeclareSearchViewModels model, string value) {
            try {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");

                //ログイン判定
                if(!(new LoginBL()).IsLogin()) return RedirectToAction("Login", "Login");

                //2019-10-02 iwai-tamura add-str ------
                CommonLog.WriteOperationLog((((LoginUser)Session["LoginUser"]).UserCode),"入力開始","");
                //2019-10-02 iwai-tamura add-end ------

                //2016-01-21 iwai-tamura add str -----
                //検索条件セット
                TempData["SelfDeclareSearch"] = model.Search;
                //2016-01-21 iwai-tamura add end -----


                //入力画面へ
                int? id = (new SelfDeclareSearchBL()).GetManageNo((LoginUser)Session["LoginUser"]);
                if(id != null) {
                    return RedirectToAction("Index", "SelfDeclareManagement", new { id = id, tableType = "G" });
                }
                //表示
                return View((new SelfDeclareSearchBL()).Search(model, (LoginUser)Session["LoginUser"]));
            } catch(Exception ex) {
                //エラー
                nlog.Error(System.Reflection.MethodBase.GetCurrentMethod().Name + " error " + ex.ToString());
                TempData["Error"] = ex.ToString();
                return View("Error");
            } finally {
                //終了
                //2019-10-02 iwai-tamura add-str ------
                CommonLog.WriteOperationLog((((LoginUser)Session["LoginUser"]).UserCode),"入力終了","");
                //2019-10-02 iwai-tamura add-end ------

                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " end");
            }
        }

        /// <summary>
        /// 部下表示
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ActionName("Search")]
        [AcceptButton(ButtonName = "SubView")]
        public ActionResult SubView(SelfDeclareSearchViewModels model, string value) {
            try {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");

                //2019-10-02 iwai-tamura add-str ------
                CommonLog.WriteOperationLog((((LoginUser)Session["LoginUser"]).UserCode),"表示開始","");
                //2019-10-02 iwai-tamura add-end ------

                //ログイン判定
                if(!(new LoginBL()).IsLogin()) return RedirectToAction("Login", "Login");

                //2016-01-21 iwai-tamura add str -----
                //検索時、検索方法を保存するよう変更
                Session["SearchType"] = "Sub";
                //2016-01-21 iwai-tamura add end -----

                //表示
                return View((new SelfDeclareSearchBL()).SubSearch(model, (LoginUser)Session["LoginUser"]));
                
            } catch(Exception ex) {
                //エラー
                nlog.Error(System.Reflection.MethodBase.GetCurrentMethod().Name + " error " + ex.ToString());
                TempData["Error"] = ex.ToString();
                return View("Error");
            } finally {
                //終了
                //2019-10-02 iwai-tamura add-str ------
                CommonLog.WriteOperationLog((((LoginUser)Session["LoginUser"]).UserCode),"表示終了","");
                //2019-10-02 iwai-tamura add-end ------

                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " end");
            }
        }

        /// <summary>
        /// 表示
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ActionName("Search")]
        [ButtonHandler(ButtonName = "View")]
        //[AcceptButton(ButtonName = "View")]
        public ActionResult ViewRedirect(SelfDeclareSearchViewModels model, string value) {
            try {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");

                //2019-10-02 iwai-tamura add-str ------
                CommonLog.WriteOperationLog((((LoginUser)Session["LoginUser"]).UserCode),"表示開始","");
                //2019-10-02 iwai-tamura add-end ------

                //ログイン判定
                if(!(new LoginBL()).IsLogin()) return RedirectToAction("Login", "Login");

                //2016-01-21 iwai-tamura add str -----
                //検索条件セット
                TempData["SelfDeclareSearch"] = model.Search;
                //2016-01-21 iwai-tamura add end -----

                //詳細へ
                var val = value.Split(',');

                //2020-03-31 iwai-tamura upd str ------
                //自己申告パターンにより分岐を行う
                switch( val[1] ){
                    case  "A11": //一般職 執務職
                        return RedirectToAction("Index", "SelfDeclareRegisterA11", new { id = val[0], tableType = val[1] });

                    case  "A12": //一般職 エリア総括職
                        return RedirectToAction("Index", "SelfDeclareRegisterA12", new { id = val[0], tableType = val[1] });

                    case  "A13": //一般職 総括職
                        return RedirectToAction("Index", "SelfDeclareRegisterA13", new { id = val[0], tableType = val[1] });

                    //2021-12-24 iwai-tamura upd str ------
                    //2021年度よりフォーマット変更
                    case  "A20": //一般職
                        return RedirectToAction("Index", "SelfDeclareRegisterA20", new { id = val[0], tableType = val[1] });
                    //2021-12-24 iwai-tamura upd end ------

                    case  "B11": //准管理職 執務職・エリア総括職
                        return RedirectToAction("Index", "SelfDeclareRegisterB11", new { id = val[0], tableType = val[1] });

                    case  "B12": //准管理職 総括職
                        return RedirectToAction("Index", "SelfDeclareRegisterB12", new { id = val[0], tableType = val[1] });

                    //2021-12-24 iwai-tamura upd str ------
                    //2021年度よりフォーマット変更
                    case  "B20": //准管理職
                        return RedirectToAction("Index", "SelfDeclareRegisterB20", new { id = val[0], tableType = val[1] });
                    //2021-12-24 iwai-tamura upd end ------

                    case  "C11": //GM1級・GM2級
                        return RedirectToAction("Index", "SelfDeclareRegisterC11", new { id = val[0], tableType = val[1] });

                    case  "C12": //M1級
                        return RedirectToAction("Index", "SelfDeclareRegisterC12", new { id = val[0], tableType = val[1] });

                    //2021-12-24 iwai-tamura upd str ------
                    //2021年度よりフォーマット変更
                    case  "C20": //管理職
                        return RedirectToAction("Index", "SelfDeclareRegisterC20", new { id = val[0], tableType = val[1] });
                    //2021-12-24 iwai-tamura upd end ------

                    default:　//指定職掌以外
                        return RedirectToAction("Search");
                }
                ////職掌番号により自己申告書の分岐を行う
                //switch( val[2] ){
                //    case  "30": //専任職
                //        return RedirectToAction("Index", "SelfDeclareRegisterA", new { id = val[0], tableType = val[1] });
                //    case  "20": //総括職
                //        return RedirectToAction("Index", "SelfDeclareRegisterB", new { id = val[0], tableType = val[1] });
                //    case  "11": //管理職
                ////2019-04-01 iwai-tamura add str ------
                //    case  "14": //専任職(管理職)
                ////2019-04-01 iwai-tamura add end ------
                //        return RedirectToAction("Index", "SelfDeclareRegisterC", new { id = val[0], tableType = val[1] });
                //    default:　//指定職掌以外
                //        return RedirectToAction("Search");
                //}
                //2020-03-31 iwai-tamura upd end ------

            } catch(Exception ex) {
                //エラー
                nlog.Error(System.Reflection.MethodBase.GetCurrentMethod().Name + " error " + ex.ToString());
                TempData["Error"] = ex.ToString();
                return View("Error");
            } finally {
                //終了
                //2019-10-02 iwai-tamura add-str ------
                CommonLog.WriteOperationLog((((LoginUser)Session["LoginUser"]).UserCode),"表示終了","");
                //2019-10-02 iwai-tamura add-end ------

                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " end");
            }
        }

        /// <summary>
        /// 単票出力
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ActionName("Search")]
        [ButtonHandler(ButtonName = "RowPrintAtoC")]
        //[AcceptButton(ButtonName = "View")]
        public ActionResult RowPrintAtoC(SelfDeclareSearchViewModels model, string value) {
            try {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");

                //2019-10-02 iwai-tamura add-str ------
                CommonLog.WriteOperationLog((((LoginUser)Session["LoginUser"]).UserCode),"PDF出力開始","AtoC");
                //2019-10-02 iwai-tamura add-end ------

                //ログイン判定
                if(!(new LoginBL()).IsLogin()) return RedirectToAction("Login", "Login");
                
                //帳票出力ロジックを実行
                //帳票作成ディレクトリを取得
                SelfDeclareSearchPrintBL bl = new SelfDeclareSearchPrintBL(Server.MapPath(this.GetTempDir(WebConfig.GetConfigFile())));
                //結果
                //model.Down.DownloadPath = bl.Print(new string[] { value });
                //model.Down.DownloadFlag = string.IsNullOrEmpty(model.Down.DownloadPath) ? false : true;

                ////表示
                //return View((new SelfDeclareSearchBL()).Search(model, (LoginUser)Session["LoginUser"]));
                //DL処理変更
                var dlpath = bl.PrintAtoC(new string[] { value });
                string mappath = Server.MapPath(this.GetTempDir(WebConfig.GetConfigFile()) + dlpath);
                string fileName = Path.GetFileName(mappath);
                return File(mappath, "application/zip", fileName);

            } catch(Exception ex) {
                //エラー
                nlog.Error(System.Reflection.MethodBase.GetCurrentMethod().Name + " error " + ex.ToString());
                TempData["Error"] = ex.ToString();
                return View("Error");
            } finally {
                //終了
                //2019-10-02 iwai-tamura add-str ------
                CommonLog.WriteOperationLog((((LoginUser)Session["LoginUser"]).UserCode),"PDF出力終了","AtoC");
                //2019-10-02 iwai-tamura add-end ------

                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " end");
            }
        }

        /// <summary>
        /// 単票出力
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ActionName("Search")]
        [ButtonHandler(ButtonName = "RowPrintD")]
        //[AcceptButton(ButtonName = "View")]
        public ActionResult RowPrintD(SelfDeclareSearchViewModels model, string value) {
            try {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");

                //2019-10-02 iwai-tamura add-str ------
                CommonLog.WriteOperationLog((((LoginUser)Session["LoginUser"]).UserCode),"PDF出力開始","D");
                //2019-10-02 iwai-tamura add-end ------

                //ログイン判定
                if(!(new LoginBL()).IsLogin()) return RedirectToAction("Login", "Login");
                
                //帳票出力ロジックを実行
                //帳票作成ディレクトリを取得
                SelfDeclareSearchPrintBL bl = new SelfDeclareSearchPrintBL(Server.MapPath(this.GetTempDir(WebConfig.GetConfigFile())));
                //結果
                //model.Down.DownloadPath = bl.Print(new string[] { value });
                //model.Down.DownloadFlag = string.IsNullOrEmpty(model.Down.DownloadPath) ? false : true;

                ////表示
                //return View((new SelfDeclareSearchBL()).Search(model, (LoginUser)Session["LoginUser"]));
                //DL処理変更
                var dlpath = bl.PrintD(new string[] { value });
                string mappath = Server.MapPath(this.GetTempDir(WebConfig.GetConfigFile()) + dlpath);
                string fileName = Path.GetFileName(mappath);
                return File(mappath, "application/zip", fileName);

            } catch(Exception ex) {
                //エラー
                nlog.Error(System.Reflection.MethodBase.GetCurrentMethod().Name + " error " + ex.ToString());
                TempData["Error"] = ex.ToString();
                return View("Error");
            } finally {
                //終了
                //2019-10-02 iwai-tamura add-str ------
                CommonLog.WriteOperationLog((((LoginUser)Session["LoginUser"]).UserCode),"PDF出力終了","D");
                //2019-10-02 iwai-tamura add-end ------

                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " end");
            }
        }

        /// <summary>
        /// 単票出力
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ActionName("Search")]
        [ButtonHandler(ButtonName = "RowPrintCarrier")]
        //[AcceptButton(ButtonName = "View")]
        public ActionResult RowPrintCarrier(SelfDeclareSearchViewModels model, string value) {
            try {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");

                //2019-10-02 iwai-tamura add-str ------
                CommonLog.WriteOperationLog((((LoginUser)Session["LoginUser"]).UserCode),"PDF出力開始","Career");
                //2019-10-02 iwai-tamura add-end ------

                //ログイン判定
                if(!(new LoginBL()).IsLogin()) return RedirectToAction("Login", "Login");
                
                //帳票出力ロジックを実行
                //帳票作成ディレクトリを取得
                SelfDeclareSearchPrintBL bl = new SelfDeclareSearchPrintBL(Server.MapPath(this.GetTempDir(WebConfig.GetConfigFile())));
                //結果
                //model.Down.DownloadPath = bl.Print(new string[] { value });
                //model.Down.DownloadFlag = string.IsNullOrEmpty(model.Down.DownloadPath) ? false : true;

                ////表示
                //return View((new SelfDeclareSearchBL()).Search(model, (LoginUser)Session["LoginUser"]));
                //DL処理変更
                var dlpath = bl.PrintCarrier(new string[] { value });
                string mappath = Server.MapPath(this.GetTempDir(WebConfig.GetConfigFile()) + dlpath);
                string fileName = Path.GetFileName(mappath);
                return File(mappath, "application/zip", fileName);

            } catch(Exception ex) {
                //エラー
                nlog.Error(System.Reflection.MethodBase.GetCurrentMethod().Name + " error " + ex.ToString());
                TempData["Error"] = ex.ToString();
                return View("Error");
            } finally {
                //終了
                //2019-10-02 iwai-tamura add-str ------
                CommonLog.WriteOperationLog((((LoginUser)Session["LoginUser"]).UserCode),"PDF出力終了","Career");
                //2019-10-02 iwai-tamura add-end ------

                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " end");
            }
        }

        /// <summary>
        /// 表示
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ActionName("Search")]
        [ButtonHandler(ButtonName = "ViewD")]
        //[AcceptButton(ButtonName = "ViewD")]
        public ActionResult ViewDRedirect(SelfDeclareSearchViewModels model, string value) {
            try {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");

                //2019-10-02 iwai-tamura add-str ------
                CommonLog.WriteOperationLog((((LoginUser)Session["LoginUser"]).UserCode),"表示開始","D");
                //2019-10-02 iwai-tamura add-end ------

                //ログイン判定
                if(!(new LoginBL()).IsLogin()) return RedirectToAction("Login", "Login");

                //2016-01-21 iwai-tamura add str -----
                //検索条件セット
                TempData["SelfDeclareSearch"] = model.Search;
                //2016-01-21 iwai-tamura add end -----

                //詳細へ
                var val = value.Split(',');

                //自己申告書　Ｄ表登録画面へ
                return RedirectToAction("Index", "SelfDeclareRegisterD", new { id = val[0] });

            } catch(Exception ex) {
                //エラー
                nlog.Error(System.Reflection.MethodBase.GetCurrentMethod().Name + " error " + ex.ToString());
                TempData["Error"] = ex.ToString();
                return View("Error");
            } finally {
                //終了
                //2019-10-02 iwai-tamura add-str ------
                CommonLog.WriteOperationLog((((LoginUser)Session["LoginUser"]).UserCode),"表示終了","D");
                //2019-10-02 iwai-tamura add-end ------

                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " end");
            }
        }

        /// <summary>
        /// キャリアシート表示
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ActionName("Search")]
        [ButtonHandler(ButtonName = "ViewCarrier")]
        //[AcceptButton(ButtonName = "ViewD")]
        public ActionResult ViewCarrierRedirect(SelfDeclareSearchViewModels model, string value) {
            try {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");

                //2019-10-02 iwai-tamura add-str ------
                CommonLog.WriteOperationLog((((LoginUser)Session["LoginUser"]).UserCode),"表示開始","Career");
                //2019-10-02 iwai-tamura add-end ------

                //ログイン判定
                if(!(new LoginBL()).IsLogin()) return RedirectToAction("Login", "Login");

                //2016-01-21 iwai-tamura add str -----
                //検索条件セット
                TempData["SelfDeclareSearch"] = model.Search;
                //2016-01-21 iwai-tamura add end -----

                //詳細へ
                var val = value.Split(',');

                //キャリアシート登録画面へ
                return RedirectToAction("Index", "CareerSheetRegister", new { id = val[0], tableType = val[1] });

            } catch(Exception ex) {
                //エラー
                nlog.Error(System.Reflection.MethodBase.GetCurrentMethod().Name + " error " + ex.ToString());
                TempData["Error"] = ex.ToString();
                return View("Error");
            } finally {
                //終了
                //2019-10-02 iwai-tamura add-str ------
                CommonLog.WriteOperationLog((((LoginUser)Session["LoginUser"]).UserCode),"表示終了","Career");
                //2019-10-02 iwai-tamura add-end ------

                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " end");
            }
        }


        /// <summary>
        /// 戻る
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ActionName("Search")]
        [AcceptButton(ButtonName = "Back")]
        public ActionResult Back(SelfDeclareSearchViewModels model, string value) {
            try {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");
                //トップへ
                return RedirectToAction("Index", "Top");
            } catch(Exception ex) {
                //エラー
                nlog.Error(System.Reflection.MethodBase.GetCurrentMethod().Name + " error " + ex.ToString());
                TempData["Error"] = ex.ToString();
                return View("Error");
            } finally {
                //終了
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " end");
            }
        }

        /// <summary>
        /// 帳票作成
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ActionName("Search")]
        //[ButtonHandler(ButtonName = "Print")]
        [AcceptButton(ButtonName = "PrintAtoC")]
        public ActionResult PrintAtoC(SelfDeclareSearchViewModels model, string[] selPrint) {
            try {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");

                //対象選択エラーチェック
                if(selPrint == null) {
                    //エラー判定
                    ModelState.AddModelError("", "出力対象を選択してください。");
                    // 2017-03-31 sbc-sagara add str 選択エラー時検索結果再表示対応
                    if ((string)Session["SearchType"] == "Main") {
                        return View("Search", (new SelfDeclareSearchBL()).Search(model, (LoginUser)Session["LoginUser"]));
                    } else if ((string)Session["SearchType"] == "Sub") {
                        return View("Search", (new SelfDeclareSearchBL()).SubSearch(model, (LoginUser)Session["LoginUser"]));
                    }
                    // 2017-03-31 sbc-sagara add end 選択エラー時検索結果再表示対応
                    return View("Search", model);
                }
                
                //2019-10-02 iwai-tamura add-str ------
                CommonLog.WriteOperationLog((((LoginUser)Session["LoginUser"]).UserCode),"PDF一括出力開始","AtoC");
                //2019-10-02 iwai-tamura add-end ------

                //帳票出力ロジックを実行
                //帳票作成ディレクトリを取得
                SelfDeclareSearchPrintBL bl = new SelfDeclareSearchPrintBL(Server.MapPath(this.GetTempDir(WebConfig.GetConfigFile())));
                //結果
                //model.Down.DownloadPath = bl.Print(selPrint);
                //model.Down.DownloadFlag = string.IsNullOrEmpty(model.Down.DownloadPath) ? false : true;
                ////表示
                //return View((new SelfDeclareSearchBL()).Search(model, (LoginUser)Session["LoginUser"]));

                //DL処理変更
                var dlpath = bl.PrintAtoC(selPrint);
                string mappath = Server.MapPath(this.GetTempDir(WebConfig.GetConfigFile()) + dlpath);
                string fileName = Path.GetFileName(mappath);
                return File(mappath, "application/zip", fileName);
            } catch(Exception ex) {
                // エラー
                nlog.Error(System.Reflection.MethodBase.GetCurrentMethod().Name + " error " + ex.ToString());
                TempData["Error"] = ex.ToString();
                return View("Error");
            } finally {
                //終了
                //2019-10-02 iwai-tamura add-str ------
                CommonLog.WriteOperationLog((((LoginUser)Session["LoginUser"]).UserCode),"PDF一括出力終了","AtoC");
                //2019-10-02 iwai-tamura add-end ------

                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " end");
            }
        }

        /// <summary>
        /// 帳票作成
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ActionName("Search")]
        //[ButtonHandler(ButtonName = "Print")]
        [AcceptButton(ButtonName = "PrintD")]
        public ActionResult PrintD(SelfDeclareSearchViewModels model, string[] selPrint) {
            try {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");

                //対象選択エラーチェック
                if(selPrint == null) {
                    //エラー判定
                    ModelState.AddModelError("", "出力対象を選択してください。");
                    // 2017-03-31 sbc-sagara add str 選択エラー時検索結果再表示対応
                    if ((string)Session["SearchType"] == "Main") {
                        return View("Search", (new SelfDeclareSearchBL()).Search(model, (LoginUser)Session["LoginUser"]));
                    } else if ((string)Session["SearchType"] == "Sub") {
                        return View("Search", (new SelfDeclareSearchBL()).SubSearch(model, (LoginUser)Session["LoginUser"]));
                    }
                    // 2017-03-31 sbc-sagara add end 選択エラー時検索結果再表示対応
                    return View("Search", model);
                }
                
                //2019-10-02 iwai-tamura add-str ------
                CommonLog.WriteOperationLog((((LoginUser)Session["LoginUser"]).UserCode),"PDF一括出力開始","D");
                //2019-10-02 iwai-tamura add-end ------

                //帳票出力ロジックを実行
                //帳票作成ディレクトリを取得
                SelfDeclareSearchPrintBL bl = new SelfDeclareSearchPrintBL(Server.MapPath(this.GetTempDir(WebConfig.GetConfigFile())));
                //結果
                //model.Down.DownloadPath = bl.Print(selPrint);
                //model.Down.DownloadFlag = string.IsNullOrEmpty(model.Down.DownloadPath) ? false : true;
                ////表示
                //return View((new SelfDeclareSearchBL()).Search(model, (LoginUser)Session["LoginUser"]));

                //DL処理変更
                var dlpath = bl.PrintD(selPrint);
                string mappath = Server.MapPath(this.GetTempDir(WebConfig.GetConfigFile()) + dlpath);
                string fileName = Path.GetFileName(mappath);
                return File(mappath, "application/zip", fileName);
            } catch(Exception ex) {
                // エラー
                nlog.Error(System.Reflection.MethodBase.GetCurrentMethod().Name + " error " + ex.ToString());
                TempData["Error"] = ex.ToString();
                return View("Error");
            } finally {
                //終了
                //2019-10-02 iwai-tamura add-str ------
                CommonLog.WriteOperationLog((((LoginUser)Session["LoginUser"]).UserCode),"PDF一括出力終了","D");
                //2019-10-02 iwai-tamura add-end ------

                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " end");
            }
        }

        /// <summary>
        /// 帳票作成
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ActionName("Search")]
        //[ButtonHandler(ButtonName = "Print")]
        [AcceptButton(ButtonName = "PrintCareer")]
        public ActionResult PrintCareer(SelfDeclareSearchViewModels model, string[] selPrint) {
            try {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");

                //対象選択エラーチェック
                if(selPrint == null) {
                    //エラー判定
                    ModelState.AddModelError("", "出力対象を選択してください。");
                    // 2017-03-31 sbc-sagara add str 選択エラー時検索結果再表示対応
                    if ((string)Session["SearchType"] == "Main") {
                        return View("Search", (new SelfDeclareSearchBL()).Search(model, (LoginUser)Session["LoginUser"]));
                    } else if ((string)Session["SearchType"] == "Sub") {
                        return View("Search", (new SelfDeclareSearchBL()).SubSearch(model, (LoginUser)Session["LoginUser"]));
                    }
                    // 2017-03-31 sbc-sagara add end 選択エラー時検索結果再表示対応
                    return View("Search", model);
                }
                
                //2019-10-02 iwai-tamura add-str ------
                CommonLog.WriteOperationLog((((LoginUser)Session["LoginUser"]).UserCode),"PDF一括出力開始","Career");
                //2019-10-02 iwai-tamura add-end ------

                //帳票出力ロジックを実行
                //帳票作成ディレクトリを取得
                SelfDeclareSearchPrintBL bl = new SelfDeclareSearchPrintBL(Server.MapPath(this.GetTempDir(WebConfig.GetConfigFile())));
                //結果
                //model.Down.DownloadPath = bl.Print(selPrint);
                //model.Down.DownloadFlag = string.IsNullOrEmpty(model.Down.DownloadPath) ? false : true;
                ////表示
                //return View((new SelfDeclareSearchBL()).Search(model, (LoginUser)Session["LoginUser"]));

                //DL処理変更
                var dlpath = bl.PrintCarrier(selPrint);
                string mappath = Server.MapPath(this.GetTempDir(WebConfig.GetConfigFile()) + dlpath);
                string fileName = Path.GetFileName(mappath);
                return File(mappath, "application/zip", fileName);
            } catch(Exception ex) {
                // エラー
                nlog.Error(System.Reflection.MethodBase.GetCurrentMethod().Name + " error " + ex.ToString());
                TempData["Error"] = ex.ToString();
                return View("Error");
            } finally {
                //終了
                //2019-10-02 iwai-tamura add-str ------
                CommonLog.WriteOperationLog((((LoginUser)Session["LoginUser"]).UserCode),"PDF一括出力終了","Career");
                //2019-10-02 iwai-tamura add-end ------

                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " end");
            }
        }
        
        /// <summary>
        /// 一括承認
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ActionName("Search")]
        //[ButtonHandler(ButtonName = "Print")]
        [AcceptButton(ButtonName = "Sign")]
        public ActionResult Sign(SelfDeclareSearchViewModels model, string[] selPrint) {
            try {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");

                //対象選択エラーチェック
                if(selPrint == null) {
                    //エラー判定
                    ModelState.AddModelError("", "承認対象を選択してください。");
                    // 2017-03-31 sbc-sagara add str
                    if ((string)Session["SearchType"] == "Sub")
                    {
                        //「部下表示」ボタンにて検索した場合
                        return View(new SelfDeclareSearchBL().SubSearch(model, (LoginUser)Session["LoginUser"]));
                    }
                    else
                    {
                        //「検索」ボタンにて検索した場合
                        return View(new SelfDeclareSearchBL().Search(model, (LoginUser)Session["LoginUser"]));
                    }
                    // 2017-03-31 sbc-sagara add end
                    return View("Search", model);
                }

                //2019-10-02 iwai-tamura add-str ------
                CommonLog.WriteOperationLog((((LoginUser)Session["LoginUser"]).UserCode),"一括承認開始","");
                //2019-10-02 iwai-tamura add-end ------

                //帳票出力ロジックを実行
                //帳票作成ディレクトリを取得
                SelfDeclareSearchBL bl = new SelfDeclareSearchBL();

                //2016-01-21 iwai-tamura upd end -----
                int cntTaget = selPrint.Length;                                         //選択した件数
                int cntUpdate = bl.Sign(selPrint, (LoginUser)Session["LoginUser"]);     //承認処理：承認した件数を取得
                //bl.Sign(selPrint,(LoginUser)Session["LoginUser"]);
                //2016-01-21 iwai-tamura upd end -----                

                //結果
                //model.Down.DownloadFlag = true;
                //2016-01-21 iwai-tamura upd str -----
                //承認結果を表示するよう変更
                TempData["Success"] = string.Format("{0}名中{1}名承認しました", cntTaget, cntUpdate);
                //TempData["Success"] = "承認しました";
                //2016-01-21 iwai-tamura upd end -----                

                //表示
                //2016-01-21 iwai-tamura upd str -----
                if ((string)Session["SearchType"] == "Sub")
                {
                    //「部下表示」ボタンにて検索した場合
                    return View(bl.SubSearch(model, (LoginUser)Session["LoginUser"]));
                }
                else
                {
                    //「検索」ボタンにて検索した場合
                    return View(bl.Search(model, (LoginUser)Session["LoginUser"]));
                }
                //return View(bl.Search(model, (LoginUser)Session["LoginUser"]));
                //2016-01-21 iwai-tamura upd end -----                
            } catch(Exception ex) {
                // エラー
                nlog.Error(System.Reflection.MethodBase.GetCurrentMethod().Name + " error " + ex.ToString());
                TempData["Error"] = ex.ToString();
                return View("Error");
            } finally {
                //終了
                //2019-10-02 iwai-tamura add-str ------
                CommonLog.WriteOperationLog((((LoginUser)Session["LoginUser"]).UserCode),"一括承認終了","");
                //2019-10-02 iwai-tamura add-end ------

                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " end");
            }
        }


        /// <summary>
        /// AtoC表 Excel作成
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ActionName("Search")]
        //[ButtonHandler(ButtonName = "Print")]
        [AcceptButton(ButtonName = "PrintAtoCXls")]
        public ActionResult PrintAtoCXls(SelfDeclareSearchViewModels model, string[] selPrint,string tblType)
        {
            try
            {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");
                
                //対象選択エラーチェック
                if (selPrint == null)
                {
                    //エラー判定
                    ModelState.AddModelError("", "出力対象を選択してください。");
                    
                    if ((string)Session["SearchType"] == "Main") {
                        return View("Search", (new SelfDeclareSearchBL()).Search(model, (LoginUser)Session["LoginUser"]));
                    } else if ((string)Session["SearchType"] == "Sub") {
                        return View("Search", (new SelfDeclareSearchBL()).SubSearch(model, (LoginUser)Session["LoginUser"]));
                    }
                    return View("Search", model);
                }
                
                //管理者判定
                if (((LoginUser)Session["LoginUser"]).自己申告書DATA出力 != "1")
                {
                    TempData["Confirmation"] = string.Format("権限が無い為、この機能は使えません。");
                    if ((string)Session["SearchType"] == "Main") {
                        return View("Search", (new SelfDeclareSearchBL()).Search(model, (LoginUser)Session["LoginUser"]));
                    } else if ((string)Session["SearchType"] == "Sub") {
                        return View("Search", (new SelfDeclareSearchBL()).SubSearch(model, (LoginUser)Session["LoginUser"]));
                    }
                    return View("Search", model);                
                }

                //2019-10-02 iwai-tamura add-str ------
                CommonLog.WriteOperationLog((((LoginUser)Session["LoginUser"]).UserCode),"Excel出力開始",tblType);
                //2019-10-02 iwai-tamura add-end ------

                //帳票出力ロジックを実行
                //帳票作成ディレクトリを取得
                SelfDeclareSearchPrintBL bl = new SelfDeclareSearchPrintBL(Server.MapPath(this.GetTempDir(WebConfig.GetConfigFile())));

                //DL処理変更
                var dlpath = bl.PrintXls(selPrint,tblType);
                string mappath = Server.MapPath(this.GetTempDir(WebConfig.GetConfigFile()) + dlpath);
                string fileName = Path.GetFileName(mappath);
                return File(mappath, "application/zip", fileName);
            }
            catch (Exception ex)
            {
                // エラー
                nlog.Error(System.Reflection.MethodBase.GetCurrentMethod().Name + " error " + ex.ToString());
                TempData["Error"] = ex.ToString();
                return View("Error");
            }
            finally
            {
                //終了
                //2019-10-02 iwai-tamura add-str ------
                CommonLog.WriteOperationLog((((LoginUser)Session["LoginUser"]).UserCode),"Excel出力終了",tblType);
                //2019-10-02 iwai-tamura add-end ------

                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " end");
            }
        }
        //2021-12-24 iwai-tamura add-str ------
        /// <summary>
        /// A20表 Excel作成
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ActionName("Search")]
        [AcceptButton(ButtonName = "PrintA20Xls")]
        public ActionResult PrintA20Xls(SelfDeclareSearchViewModels model, string[] selPrint)
        {
            return PrintAtoCXls( model,selPrint,"A20");
        }
        
        /// <summary>
        /// B20表 Excel作成
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ActionName("Search")]
        [AcceptButton(ButtonName = "PrintB20Xls")]
        public ActionResult PrintB20Xls(SelfDeclareSearchViewModels model, string[] selPrint)
        {
            return PrintAtoCXls( model,selPrint,"B20");
        }
        
        /// <summary>
        /// C20表 Excel作成
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ActionName("Search")]
        [AcceptButton(ButtonName = "PrintC20Xls")]
        public ActionResult PrintC20Xls(SelfDeclareSearchViewModels model, string[] selPrint)
        {
            return PrintAtoCXls( model,selPrint,"C20");
        }
        //2021-12-24 iwai-tamura add-end ------


        //2020-04-10 iwai-tamura add-str ------
        /// <summary>
        /// A11表 Excel作成
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ActionName("Search")]
        //[ButtonHandler(ButtonName = "Print")]
        [AcceptButton(ButtonName = "PrintA11Xls")]
        public ActionResult PrintA11Xls(SelfDeclareSearchViewModels model, string[] selPrint)
        {
            return PrintAtoCXls( model,selPrint,"A11");
        }
        
        /// <summary>
        /// A12表 Excel作成
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ActionName("Search")]
        //[ButtonHandler(ButtonName = "Print")]
        [AcceptButton(ButtonName = "PrintA12Xls")]
        public ActionResult PrintA12Xls(SelfDeclareSearchViewModels model, string[] selPrint)
        {
            return PrintAtoCXls( model,selPrint,"A12");
        }
        
        /// <summary>
        /// A13表 Excel作成
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ActionName("Search")]
        //[ButtonHandler(ButtonName = "Print")]
        [AcceptButton(ButtonName = "PrintA13Xls")]
        public ActionResult PrintA13Xls(SelfDeclareSearchViewModels model, string[] selPrint)
        {
            return PrintAtoCXls( model,selPrint,"A13");
        }
        
        /// <summary>
        /// B11表 Excel作成
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ActionName("Search")]
        //[ButtonHandler(ButtonName = "Print")]
        [AcceptButton(ButtonName = "PrintB11Xls")]
        public ActionResult PrintB11Xls(SelfDeclareSearchViewModels model, string[] selPrint)
        {
            return PrintAtoCXls( model,selPrint,"B11");
        }
        
        /// <summary>
        /// B12表 Excel作成
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ActionName("Search")]
        //[ButtonHandler(ButtonName = "Print")]
        [AcceptButton(ButtonName = "PrintB12Xls")]
        public ActionResult PrintB12Xls(SelfDeclareSearchViewModels model, string[] selPrint)
        {
            return PrintAtoCXls( model,selPrint,"B12");
        }
        
        /// <summary>
        /// C11表 Excel作成
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ActionName("Search")]
        //[ButtonHandler(ButtonName = "Print")]
        [AcceptButton(ButtonName = "PrintC11Xls")]
        public ActionResult PrintC11Xls(SelfDeclareSearchViewModels model, string[] selPrint)
        {
            return PrintAtoCXls( model,selPrint,"C11");
        }
        
        /// <summary>
        /// C12表 Excel作成
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ActionName("Search")]
        //[ButtonHandler(ButtonName = "Print")]
        [AcceptButton(ButtonName = "PrintC12Xls")]
        public ActionResult PrintC12Xls(SelfDeclareSearchViewModels model, string[] selPrint)
        {
            return PrintAtoCXls( model,selPrint,"C12");
        }
        //2020-04-10 iwai-tamura add-end ------
        
        /// <summary>
        /// A表 Excel作成
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ActionName("Search")]
        //[ButtonHandler(ButtonName = "Print")]
        [AcceptButton(ButtonName = "PrintAXls")]
        public ActionResult PrintAXls(SelfDeclareSearchViewModels model, string[] selPrint)
        {
            try
            {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");
                
                //対象選択エラーチェック
                if (selPrint == null)
                {
                    //エラー判定
                    ModelState.AddModelError("", "出力対象を選択してください。");
                    
                    if ((string)Session["SearchType"] == "Main") {
                        return View("Search", (new SelfDeclareSearchBL()).Search(model, (LoginUser)Session["LoginUser"]));
                    } else if ((string)Session["SearchType"] == "Sub") {
                        return View("Search", (new SelfDeclareSearchBL()).SubSearch(model, (LoginUser)Session["LoginUser"]));
                    }
                    return View("Search", model);
                }
                
                //管理者判定
                if (((LoginUser)Session["LoginUser"]).自己申告書DATA出力 != "1")
                {
                    TempData["Confirmation"] = string.Format("権限が無い為、この機能は使えません。");
                    if ((string)Session["SearchType"] == "Main") {
                        return View("Search", (new SelfDeclareSearchBL()).Search(model, (LoginUser)Session["LoginUser"]));
                    } else if ((string)Session["SearchType"] == "Sub") {
                        return View("Search", (new SelfDeclareSearchBL()).SubSearch(model, (LoginUser)Session["LoginUser"]));
                    }
                    return View("Search", model);                
                }

                //2019-10-02 iwai-tamura add-str ------
                CommonLog.WriteOperationLog((((LoginUser)Session["LoginUser"]).UserCode),"Excel出力開始","A");
                //2019-10-02 iwai-tamura add-end ------

                //帳票出力ロジックを実行
                //帳票作成ディレクトリを取得
                SelfDeclareSearchPrintBL bl = new SelfDeclareSearchPrintBL(Server.MapPath(this.GetTempDir(WebConfig.GetConfigFile())));

                //DL処理変更
                var dlpath = bl.PrintXls(selPrint,"A01");
                string mappath = Server.MapPath(this.GetTempDir(WebConfig.GetConfigFile()) + dlpath);
                string fileName = Path.GetFileName(mappath);
                return File(mappath, "application/zip", fileName);
            }
            catch (Exception ex)
            {
                // エラー
                nlog.Error(System.Reflection.MethodBase.GetCurrentMethod().Name + " error " + ex.ToString());
                TempData["Error"] = ex.ToString();
                return View("Error");
            }
            finally
            {
                //終了
                //2019-10-02 iwai-tamura add-str ------
                CommonLog.WriteOperationLog((((LoginUser)Session["LoginUser"]).UserCode),"Excel出力終了","A");
                //2019-10-02 iwai-tamura add-end ------

                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " end");
            }
        }


        /// <summary>
        /// B表 Excel作成
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ActionName("Search")]
        //[ButtonHandler(ButtonName = "Print")]
        [AcceptButton(ButtonName = "PrintBXls")]
        public ActionResult PrintBXls(SelfDeclareSearchViewModels model, string[] selPrint)
        {
            try
            {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");
                
                //対象選択エラーチェック
                if (selPrint == null)
                {
                    //エラー判定
                    ModelState.AddModelError("", "出力対象を選択してください。");
                    
                    if ((string)Session["SearchType"] == "Main") {
                        return View("Search", (new SelfDeclareSearchBL()).Search(model, (LoginUser)Session["LoginUser"]));
                    } else if ((string)Session["SearchType"] == "Sub") {
                        return View("Search", (new SelfDeclareSearchBL()).SubSearch(model, (LoginUser)Session["LoginUser"]));
                    }
                    return View("Search", model);
                }
                
                //管理者判定
                if (((LoginUser)Session["LoginUser"]).自己申告書DATA出力 != "1")
                {
                    TempData["Confirmation"] = string.Format("権限が無い為、この機能は使えません。");
                    if ((string)Session["SearchType"] == "Main") {
                        return View("Search", (new SelfDeclareSearchBL()).Search(model, (LoginUser)Session["LoginUser"]));
                    } else if ((string)Session["SearchType"] == "Sub") {
                        return View("Search", (new SelfDeclareSearchBL()).SubSearch(model, (LoginUser)Session["LoginUser"]));
                    }
                    return View("Search", model);                
                }

                //2019-10-02 iwai-tamura add-str ------
                CommonLog.WriteOperationLog((((LoginUser)Session["LoginUser"]).UserCode),"Excel出力開始","B");
                //2019-10-02 iwai-tamura add-end ------

                //帳票出力ロジックを実行
                //帳票作成ディレクトリを取得
                SelfDeclareSearchPrintBL bl = new SelfDeclareSearchPrintBL(Server.MapPath(this.GetTempDir(WebConfig.GetConfigFile())));

                //DL処理変更
                var dlpath = bl.PrintXls(selPrint,"B01");
                string mappath = Server.MapPath(this.GetTempDir(WebConfig.GetConfigFile()) + dlpath);
                string fileName = Path.GetFileName(mappath);
                return File(mappath, "application/zip", fileName);
            }
            catch (Exception ex)
            {
                // エラー
                nlog.Error(System.Reflection.MethodBase.GetCurrentMethod().Name + " error " + ex.ToString());
                TempData["Error"] = ex.ToString();
                return View("Error");
            }
            finally
            {
                //終了
                //2019-10-02 iwai-tamura add-str ------
                CommonLog.WriteOperationLog((((LoginUser)Session["LoginUser"]).UserCode),"Excel出力終了","B");
                //2019-10-02 iwai-tamura add-end ------

                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " end");
            }
        }
        
        /// <summary>
        /// C表 Excel作成
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ActionName("Search")]
        //[ButtonHandler(ButtonName = "Print")]
        [AcceptButton(ButtonName = "PrintCXls")]
        public ActionResult PrintCXls(SelfDeclareSearchViewModels model, string[] selPrint)
        {
            try
            {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");
                
                //対象選択エラーチェック
                if (selPrint == null)
                {
                    //エラー判定
                    ModelState.AddModelError("", "出力対象を選択してください。");
                    
                    if ((string)Session["SearchType"] == "Main") {
                        return View("Search", (new SelfDeclareSearchBL()).Search(model, (LoginUser)Session["LoginUser"]));
                    } else if ((string)Session["SearchType"] == "Sub") {
                        return View("Search", (new SelfDeclareSearchBL()).SubSearch(model, (LoginUser)Session["LoginUser"]));
                    }
                    return View("Search", model);
                }
                
                //管理者判定
                if (((LoginUser)Session["LoginUser"]).自己申告書DATA出力 != "1")
                {
                    TempData["Confirmation"] = string.Format("権限が無い為、この機能は使えません。");
                    if ((string)Session["SearchType"] == "Main") {
                        return View("Search", (new SelfDeclareSearchBL()).Search(model, (LoginUser)Session["LoginUser"]));
                    } else if ((string)Session["SearchType"] == "Sub") {
                        return View("Search", (new SelfDeclareSearchBL()).SubSearch(model, (LoginUser)Session["LoginUser"]));
                    }
                    return View("Search", model);                
                }

                //2019-10-02 iwai-tamura add-str ------
                CommonLog.WriteOperationLog((((LoginUser)Session["LoginUser"]).UserCode),"Excel出力開始","C");
                //2019-10-02 iwai-tamura add-end ------

                //帳票出力ロジックを実行
                //帳票作成ディレクトリを取得
                SelfDeclareSearchPrintBL bl = new SelfDeclareSearchPrintBL(Server.MapPath(this.GetTempDir(WebConfig.GetConfigFile())));

                //DL処理変更
                var dlpath = bl.PrintXls(selPrint,"C01");
                string mappath = Server.MapPath(this.GetTempDir(WebConfig.GetConfigFile()) + dlpath);
                string fileName = Path.GetFileName(mappath);
                return File(mappath, "application/zip", fileName);
            }
            catch (Exception ex)
            {
                // エラー
                nlog.Error(System.Reflection.MethodBase.GetCurrentMethod().Name + " error " + ex.ToString());
                TempData["Error"] = ex.ToString();
                return View("Error");
            }
            finally
            {
                //終了
                //2019-10-02 iwai-tamura add-str ------
                CommonLog.WriteOperationLog((((LoginUser)Session["LoginUser"]).UserCode),"Excel出力終了","C");
                //2019-10-02 iwai-tamura add-end ------

                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " end");
            }
        }        
        

        /// <summary>
        /// D表 Excel作成
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ActionName("Search")]
        //[ButtonHandler(ButtonName = "Print")]
        [AcceptButton(ButtonName = "PrintDXls")]
        public ActionResult PrintDXls(SelfDeclareSearchViewModels model, string[] selPrint)
        {
            try
            {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");
                
                //対象選択エラーチェック
                if (selPrint == null)
                {
                    //エラー判定
                    ModelState.AddModelError("", "出力対象を選択してください。");
                    
                    if ((string)Session["SearchType"] == "Main") {
                        return View("Search", (new SelfDeclareSearchBL()).Search(model, (LoginUser)Session["LoginUser"]));
                    } else if ((string)Session["SearchType"] == "Sub") {
                        return View("Search", (new SelfDeclareSearchBL()).SubSearch(model, (LoginUser)Session["LoginUser"]));
                    }
                    return View("Search", model);
                }
                
                //管理者判定
                if (((LoginUser)Session["LoginUser"]).自己申告書DATA出力 != "1")
                {
                    TempData["Confirmation"] = string.Format("権限が無い為、この機能は使えません。");
                    if ((string)Session["SearchType"] == "Main") {
                        return View("Search", (new SelfDeclareSearchBL()).Search(model, (LoginUser)Session["LoginUser"]));
                    } else if ((string)Session["SearchType"] == "Sub") {
                        return View("Search", (new SelfDeclareSearchBL()).SubSearch(model, (LoginUser)Session["LoginUser"]));
                    }
                    return View("Search", model);                
                }

                //2019-10-02 iwai-tamura add-str ------
                CommonLog.WriteOperationLog((((LoginUser)Session["LoginUser"]).UserCode),"Excel出力開始","D");
                //2019-10-02 iwai-tamura add-end ------

                //帳票出力ロジックを実行
                //帳票作成ディレクトリを取得
                SelfDeclareSearchPrintBL bl = new SelfDeclareSearchPrintBL(Server.MapPath(this.GetTempDir(WebConfig.GetConfigFile())));

                //DL処理変更
                var dlpath = bl.PrintXls(selPrint,"D01");
                string mappath = Server.MapPath(this.GetTempDir(WebConfig.GetConfigFile()) + dlpath);
                string fileName = Path.GetFileName(mappath);
                return File(mappath, "application/zip", fileName);
            }
            catch (Exception ex)
            {
                // エラー
                nlog.Error(System.Reflection.MethodBase.GetCurrentMethod().Name + " error " + ex.ToString());
                TempData["Error"] = ex.ToString();
                return View("Error");
            }
            finally
            {
                //終了
                //2019-10-02 iwai-tamura add-str ------
                CommonLog.WriteOperationLog((((LoginUser)Session["LoginUser"]).UserCode),"Excel出力終了","D");
                //2019-10-02 iwai-tamura add-end ------

                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " end");
            }
        }        
        
        /// <summary>
        /// キャリアシート Excel作成
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ActionName("Search")]
        //[ButtonHandler(ButtonName = "Print")]
        [AcceptButton(ButtonName = "PrintCareerXls")]
        public ActionResult PrintCareerXls(SelfDeclareSearchViewModels model, string[] selPrint)
        {
            try
            {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");
                
                //対象選択エラーチェック
                if (selPrint == null)
                {
                    //エラー判定
                    ModelState.AddModelError("", "出力対象を選択してください。");
                    
                    if ((string)Session["SearchType"] == "Main") {
                        return View("Search", (new SelfDeclareSearchBL()).Search(model, (LoginUser)Session["LoginUser"]));
                    } else if ((string)Session["SearchType"] == "Sub") {
                        return View("Search", (new SelfDeclareSearchBL()).SubSearch(model, (LoginUser)Session["LoginUser"]));
                    }
                    return View("Search", model);
                }
                
                //管理者判定
                if (((LoginUser)Session["LoginUser"]).自己申告書DATA出力 != "1")
                {
                    TempData["Confirmation"] = string.Format("権限が無い為、この機能は使えません。");
                    if ((string)Session["SearchType"] == "Main") {
                        return View("Search", (new SelfDeclareSearchBL()).Search(model, (LoginUser)Session["LoginUser"]));
                    } else if ((string)Session["SearchType"] == "Sub") {
                        return View("Search", (new SelfDeclareSearchBL()).SubSearch(model, (LoginUser)Session["LoginUser"]));
                    }
                    return View("Search", model);                
                }

                //2019-10-02 iwai-tamura add-str ------
                CommonLog.WriteOperationLog((((LoginUser)Session["LoginUser"]).UserCode),"Excel出力開始","Career");
                //2019-10-02 iwai-tamura add-end ------

                //帳票出力ロジックを実行
                //帳票作成ディレクトリを取得
                SelfDeclareSearchPrintBL bl = new SelfDeclareSearchPrintBL(Server.MapPath(this.GetTempDir(WebConfig.GetConfigFile())));

                //DL処理変更
                var dlpath = bl.PrintXls(selPrint,"CH01");
                string mappath = Server.MapPath(this.GetTempDir(WebConfig.GetConfigFile()) + dlpath);
                string fileName = Path.GetFileName(mappath);
                return File(mappath, "application/zip", fileName);
            }
            catch (Exception ex)
            {
                // エラー
                nlog.Error(System.Reflection.MethodBase.GetCurrentMethod().Name + " error " + ex.ToString());
                TempData["Error"] = ex.ToString();
                return View("Error");
            }
            finally
            {
                //終了
                //2019-10-02 iwai-tamura add-str ------
                CommonLog.WriteOperationLog((((LoginUser)Session["LoginUser"]).UserCode),"Excel出力終了","Career");
                //2019-10-02 iwai-tamura add-end ------

                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " end");
            }
        }        
        
        /// <summary>
        /// コンフィグから帳票出力フォルダ取得
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        private string GetTempDir(Configuration config) {
            try {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");
                return FileUtil.GetTempDir(config, "DOWNLOAD_TEMP_DIR_O");
            } catch(Exception ex) {
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
