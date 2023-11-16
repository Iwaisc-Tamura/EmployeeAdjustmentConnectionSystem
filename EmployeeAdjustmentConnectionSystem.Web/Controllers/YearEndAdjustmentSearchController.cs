using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using EmployeeAdjustmentConnectionSystem.BL.YearEndAdjustmentSearch;
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
using EmployeeAdjustmentConnectionSystem.Log.Common;
using EmployeeAdjustmentConnectionSystem.BL.YearEndAdjustmentReports;

namespace EmployeeAdjustmentConnectionSystem.Web.Controllers {
    /// <summary>
    /// 年調関連一括検索コントローラー
    /// </summary>
    public class YearEndAdjustmentSearchController : Controller {

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
                YearEndAdjustmentSearchViewModels model = new YearEndAdjustmentSearchViewModels {
                    Search = new YearEndAdjustmentSearchModel(),
                    SearchResult = new List<YearEndAdjustmentSearchListModel>(),
                    Down = new SelfDeclareDownLoadModel()
                };

                //検索年度に直近の年度を表示
                using(DbManager dm = new DbManager())
                using(IDbCommand cmd = dm.CreateCommand("SELECT MAX(対象年度) AS 対象年度 FROM TEM991管理情報"))
                using(IDataReader reader = cmd.ExecuteReader()) {
                    while(reader.Read()) {
                        model.Search.Year = reader["対象年度"].ToString();
                    }
                }

                //TempDataある場合
                if (TempData["YearEndAdjustmentSearch"] != null)
                {
                    //詳細設定画面から戻った時に前回の検索結果データを表示する
                    model.Search = (YearEndAdjustmentSearchModel)TempData["YearEndAdjustmentSearch"];
                    ModelState.Clear();
                    //2020-03-31 iwai-tamura upd str -----
                    if ((string)Session["SearchType"] == "D")
                    {
                        return View((new YearEndAdjustmentSearchBL()).SearchD(model, (LoginUser)Session["LoginUser"]));
                    //if ((string)Session["SearchType"] == "Sub")
                    //{
                    //    //「部下表示」ボタンにて検索した場合
                    //    return View((new YearEndAdjustmentSearchBL()).SubSearch(model, (LoginUser)Session["LoginUser"]));
                    //2020-03-31 iwai-tamura upd end -----

                    }else{
                        //「検索」ボタンにて検索した場合
                        return View((new YearEndAdjustmentSearchBL()).Search(model, (LoginUser)Session["LoginUser"]));
                    }
                }
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
        public ActionResult SearchEx(YearEndAdjustmentSearchViewModels model, string value) {
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
                return View((new YearEndAdjustmentSearchBL()).Search(model, (LoginUser)Session["LoginUser"]));
                
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
        public ActionResult SearchD(YearEndAdjustmentSearchViewModels model, string value) {
            try {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");

                CommonLog.WriteOperationLog((((LoginUser)Session["LoginUser"]).UserCode),"検索開始","");

                //ログイン判定
                if(!(new LoginBL()).IsLogin()) return RedirectToAction("Login", "Login");

                //検索時、検索方法を保存するよう変更
                Session["SearchType"] = "D";
                
                //表示
                return View((new YearEndAdjustmentSearchBL()).SearchD(model, (LoginUser)Session["LoginUser"]));
                
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
        public ActionResult SelfInput(YearEndAdjustmentSearchViewModels model, string value) {
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
                TempData["YearEndAdjustmentSearch"] = model.Search;
                //2016-01-21 iwai-tamura add end -----


                //入力画面へ
                int? id = (new YearEndAdjustmentSearchBL()).GetManageNo((LoginUser)Session["LoginUser"]);
                if(id != null) {
                    return RedirectToAction("Index", "SelfDeclareManagement", new { id = id, tableType = "G" });
                }
                //表示
                return View((new YearEndAdjustmentSearchBL()).Search(model, (LoginUser)Session["LoginUser"]));
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
        public ActionResult SubView(YearEndAdjustmentSearchViewModels model, string value) {
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
                return View((new YearEndAdjustmentSearchBL()).SubSearch(model, (LoginUser)Session["LoginUser"]));
                
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
        public ActionResult ViewRedirect(YearEndAdjustmentSearchViewModels model, string value) {
            try {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");

                CommonLog.WriteOperationLog((((LoginUser)Session["LoginUser"]).UserCode),"表示開始","");

                //ログイン判定
                if(!(new LoginBL()).IsLogin()) return RedirectToAction("Login", "Login");

                //検索条件セット
                TempData["YearEndAdjustmentSearch"] = model.Search;

                //詳細へ　(対象帳票,年度,社員番号)
                var val = value.Split(',');


                //対象登録ボタン条件
                switch( val[0] ){
                    case  "HuyouDeclare": //扶養控除
                        return RedirectToAction("Index", "HuyouDeclareRegister", new { intSheetYear = val[1], strEmployeeNo = val[2] ,bolAdminMode = true});

                    case  "HokenDeclare": //保険料控除
                        return RedirectToAction("Index", "HokenDeclareRegister", new { intSheetYear = val[1], strEmployeeNo = val[2] ,bolAdminMode = true});

                    case  "HaiguuDeclare": //基礎控除
                        return RedirectToAction("Index", "HaiguuDeclareRegister", new { intSheetYear = val[1], strEmployeeNo = val[2] ,bolAdminMode = true});

                    default:　//指定職掌以外
                        return RedirectToAction("Search");
                }

            } catch(Exception ex) {
                //エラー
                nlog.Error(System.Reflection.MethodBase.GetCurrentMethod().Name + " error " + ex.ToString());
                TempData["Error"] = ex.ToString();
                return View("Error");
            } finally {
                //終了
                CommonLog.WriteOperationLog((((LoginUser)Session["LoginUser"]).UserCode),"表示終了","");

                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " end");
            }
        }

        /// <summary>
        /// 単票出力
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ActionName("Search")]
        [ButtonHandler(ButtonName = "Print")]
        //[AcceptButton(ButtonName = "View")]
        public ActionResult RowPrintYearEndAdjustment(YearEndAdjustmentSearchViewModels model, string value)
        {
            try
            {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");

                CommonLog.WriteOperationLog((((LoginUser)Session["LoginUser"]).UserCode), "PDF出力開始", "AtoC");

                //ログイン判定
                if (!(new LoginBL()).IsLogin()) return RedirectToAction("Login", "Login");

                //詳細へ　(対象帳票,年度,社員番号)
                var val = value.Split(',');

                //帳票出力ロジックを実行
                //帳票作成ディレクトリを取得
                YearEndAdjustmentPrintBL bl = new YearEndAdjustmentPrintBL(Server.MapPath(this.GetTempDir(WebConfig.GetConfigFile())));

                //DL処理変更
                //対象登録ボタン条件
                string dlpath = string.Empty;
                switch( val[0] ){
                    case  "HuyouDeclare": //扶養控除
                        dlpath = bl.PrintHuyouDeclare(new string[] { String.Join(",", val[1], val[2]) });
                        break;

                    case  "HokenDeclare": //保険料控除
                        dlpath = bl.PrintHokenDeclare(new string[] { String.Join(",", val[1], val[2]) });
                        break;

                    case  "HaiguuDeclare": //基礎控除
                        dlpath = bl.PrintHaiguuDeclare(new string[] { String.Join(",", val[1], val[2]) });
                        break;

                    default:　//指定職掌以外
                        break;
                }

                string mappath = Server.MapPath(this.GetTempDir(WebConfig.GetConfigFile()) + dlpath);
                string fileName = Path.GetFileName(mappath);
                return File(mappath, "application/zip", fileName);

            }
            catch (Exception ex)
            {
                //エラー
                nlog.Error(System.Reflection.MethodBase.GetCurrentMethod().Name + " error " + ex.ToString());
                TempData["Error"] = ex.ToString();
                return View("Error");
            }
            finally
            {
                //終了
                CommonLog.WriteOperationLog((((LoginUser)Session["LoginUser"]).UserCode), "PDF出力終了", "AtoC");

                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " end");
            }
        }

        ///////////// <summary>
        ///////////// 単票出力
        ///////////// </summary>
        ///////////// <returns></returns>
        //////////[HttpPost]
        //////////[ActionName("Search")]
        //////////[ButtonHandler(ButtonName = "RowPrintD")]
        ////////////[AcceptButton(ButtonName = "View")]
        //////////public ActionResult RowPrintD(SelfDeclareSearchViewModels model, string value) {
        //////////    try {
        //////////        //開始
        //////////        nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");

        //////////        //2019-10-02 iwai-tamura add-str ------
        //////////        CommonLog.WriteOperationLog((((LoginUser)Session["LoginUser"]).UserCode),"PDF出力開始","D");
        //////////        //2019-10-02 iwai-tamura add-end ------

        //////////        //ログイン判定
        //////////        if(!(new LoginBL()).IsLogin()) return RedirectToAction("Login", "Login");

        //////////        //帳票出力ロジックを実行
        //////////        //帳票作成ディレクトリを取得
        //////////        SelfDeclareSearchPrintBL bl = new SelfDeclareSearchPrintBL(Server.MapPath(this.GetTempDir(WebConfig.GetConfigFile())));
        //////////        //結果
        //////////        //model.Down.DownloadPath = bl.Print(new string[] { value });
        //////////        //model.Down.DownloadFlag = string.IsNullOrEmpty(model.Down.DownloadPath) ? false : true;

        //////////        ////表示
        //////////        //return View((new SelfDeclareSearchBL()).Search(model, (LoginUser)Session["LoginUser"]));
        //////////        //DL処理変更
        //////////        var dlpath = bl.PrintD(new string[] { value });
        //////////        string mappath = Server.MapPath(this.GetTempDir(WebConfig.GetConfigFile()) + dlpath);
        //////////        string fileName = Path.GetFileName(mappath);
        //////////        return File(mappath, "application/zip", fileName);

        //////////    } catch(Exception ex) {
        //////////        //エラー
        //////////        nlog.Error(System.Reflection.MethodBase.GetCurrentMethod().Name + " error " + ex.ToString());
        //////////        TempData["Error"] = ex.ToString();
        //////////        return View("Error");
        //////////    } finally {
        //////////        //終了
        //////////        //2019-10-02 iwai-tamura add-str ------
        //////////        CommonLog.WriteOperationLog((((LoginUser)Session["LoginUser"]).UserCode),"PDF出力終了","D");
        //////////        //2019-10-02 iwai-tamura add-end ------

        //////////        nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " end");
        //////////    }
        //////////}

        ///////////// <summary>
        ///////////// 単票出力
        ///////////// </summary>
        ///////////// <returns></returns>
        //////////[HttpPost]
        //////////[ActionName("Search")]
        //////////[ButtonHandler(ButtonName = "RowPrintCarrier")]
        ////////////[AcceptButton(ButtonName = "View")]
        //////////public ActionResult RowPrintCarrier(SelfDeclareSearchViewModels model, string value) {
        //////////    try {
        //////////        //開始
        //////////        nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");

        //////////        //2019-10-02 iwai-tamura add-str ------
        //////////        CommonLog.WriteOperationLog((((LoginUser)Session["LoginUser"]).UserCode),"PDF出力開始","Career");
        //////////        //2019-10-02 iwai-tamura add-end ------

        //////////        //ログイン判定
        //////////        if(!(new LoginBL()).IsLogin()) return RedirectToAction("Login", "Login");

        //////////        //帳票出力ロジックを実行
        //////////        //帳票作成ディレクトリを取得
        //////////        SelfDeclareSearchPrintBL bl = new SelfDeclareSearchPrintBL(Server.MapPath(this.GetTempDir(WebConfig.GetConfigFile())));
        //////////        //結果
        //////////        //model.Down.DownloadPath = bl.Print(new string[] { value });
        //////////        //model.Down.DownloadFlag = string.IsNullOrEmpty(model.Down.DownloadPath) ? false : true;

        //////////        ////表示
        //////////        //return View((new SelfDeclareSearchBL()).Search(model, (LoginUser)Session["LoginUser"]));
        //////////        //DL処理変更
        //////////        var dlpath = bl.PrintCarrier(new string[] { value });
        //////////        string mappath = Server.MapPath(this.GetTempDir(WebConfig.GetConfigFile()) + dlpath);
        //////////        string fileName = Path.GetFileName(mappath);
        //////////        return File(mappath, "application/zip", fileName);

        //////////    } catch(Exception ex) {
        //////////        //エラー
        //////////        nlog.Error(System.Reflection.MethodBase.GetCurrentMethod().Name + " error " + ex.ToString());
        //////////        TempData["Error"] = ex.ToString();
        //////////        return View("Error");
        //////////    } finally {
        //////////        //終了
        //////////        //2019-10-02 iwai-tamura add-str ------
        //////////        CommonLog.WriteOperationLog((((LoginUser)Session["LoginUser"]).UserCode),"PDF出力終了","Career");
        //////////        //2019-10-02 iwai-tamura add-end ------

        //////////        nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " end");
        //////////    }
        //////////}

        ///////////// <summary>
        ///////////// 表示
        ///////////// </summary>
        ///////////// <returns></returns>
        //////////[HttpPost]
        //////////[ActionName("Search")]
        //////////[ButtonHandler(ButtonName = "ViewD")]
        ////////////[AcceptButton(ButtonName = "ViewD")]
        //////////public ActionResult ViewDRedirect(YearEndAdjustmentSearchViewModels model, string value) {
        //////////    try {
        //////////        //開始
        //////////        nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");

        //////////        //2019-10-02 iwai-tamura add-str ------
        //////////        CommonLog.WriteOperationLog((((LoginUser)Session["LoginUser"]).UserCode),"表示開始","D");
        //////////        //2019-10-02 iwai-tamura add-end ------

        //////////        //ログイン判定
        //////////        if(!(new LoginBL()).IsLogin()) return RedirectToAction("Login", "Login");

        //////////        //2016-01-21 iwai-tamura add str -----
        //////////        //検索条件セット
        //////////        TempData["YearEndAdjustmentSearch"] = model.Search;
        //////////        //2016-01-21 iwai-tamura add end -----

        //////////        //詳細へ
        //////////        var val = value.Split(',');

        //////////        //自己申告書　Ｄ表登録画面へ
        //////////        return RedirectToAction("Index", "SelfDeclareRegisterD", new { id = val[0] });

        //////////    } catch(Exception ex) {
        //////////        //エラー
        //////////        nlog.Error(System.Reflection.MethodBase.GetCurrentMethod().Name + " error " + ex.ToString());
        //////////        TempData["Error"] = ex.ToString();
        //////////        return View("Error");
        //////////    } finally {
        //////////        //終了
        //////////        //2019-10-02 iwai-tamura add-str ------
        //////////        CommonLog.WriteOperationLog((((LoginUser)Session["LoginUser"]).UserCode),"表示終了","D");
        //////////        //2019-10-02 iwai-tamura add-end ------

        //////////        nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " end");
        //////////    }
        //////////}

        ///////////// <summary>
        ///////////// キャリアシート表示
        ///////////// </summary>
        ///////////// <returns></returns>
        //////////[HttpPost]
        //////////[ActionName("Search")]
        //////////[ButtonHandler(ButtonName = "ViewCarrier")]
        ////////////[AcceptButton(ButtonName = "ViewD")]
        //////////public ActionResult ViewCarrierRedirect(YearEndAdjustmentSearchViewModels model, string value) {
        //////////    try {
        //////////        //開始
        //////////        nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");

        //////////        //2019-10-02 iwai-tamura add-str ------
        //////////        CommonLog.WriteOperationLog((((LoginUser)Session["LoginUser"]).UserCode),"表示開始","Career");
        //////////        //2019-10-02 iwai-tamura add-end ------

        //////////        //ログイン判定
        //////////        if(!(new LoginBL()).IsLogin()) return RedirectToAction("Login", "Login");

        //////////        //2016-01-21 iwai-tamura add str -----
        //////////        //検索条件セット
        //////////        TempData["YearEndAdjustmentSearch"] = model.Search;
        //////////        //2016-01-21 iwai-tamura add end -----

        //////////        //詳細へ
        //////////        var val = value.Split(',');

        //////////        //キャリアシート登録画面へ
        //////////        return RedirectToAction("Index", "CareerSheetRegister", new { id = val[0], tableType = val[1] });

        //////////    } catch(Exception ex) {
        //////////        //エラー
        //////////        nlog.Error(System.Reflection.MethodBase.GetCurrentMethod().Name + " error " + ex.ToString());
        //////////        TempData["Error"] = ex.ToString();
        //////////        return View("Error");
        //////////    } finally {
        //////////        //終了
        //////////        //2019-10-02 iwai-tamura add-str ------
        //////////        CommonLog.WriteOperationLog((((LoginUser)Session["LoginUser"]).UserCode),"表示終了","Career");
        //////////        //2019-10-02 iwai-tamura add-end ------

        //////////        nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " end");
        //////////    }
        //////////}


        /// <summary>
        /// 戻る
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ActionName("Search")]
        [AcceptButton(ButtonName = "Back")]
        public ActionResult Back(YearEndAdjustmentSearchViewModels model, string value)
        {
            try
            {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");
                //トップへ
                return RedirectToAction("Index", "Top");
            }
            catch (Exception ex)
            {
                //エラー
                nlog.Error(System.Reflection.MethodBase.GetCurrentMethod().Name + " error " + ex.ToString());
                TempData["Error"] = ex.ToString();
                return View("Error");
            }
            finally
            {
                //終了
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " end");
            }
        }

        /// <summary>
        /// 扶養控除一括帳票作成
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ActionName("Search")]
        //[ButtonHandler(ButtonName = "Print")]
        [AcceptButton(ButtonName = "PrintBatchHuyou")]
        public ActionResult PrintBatchHuyou(YearEndAdjustmentSearchViewModels model, string[] selPrint) {
            try {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");

                //対象選択エラーチェック
                if (selPrint == null) {
                    //エラー判定
                    ModelState.AddModelError("", "出力対象を選択してください。");
                    if ((string)Session["SearchType"] == "Main") {
                        return View("Search", (new YearEndAdjustmentSearchBL()).Search(model, (LoginUser)Session["LoginUser"]));
                    } else if ((string)Session["SearchType"] == "Sub") {
                        return View("Search", (new YearEndAdjustmentSearchBL()).SubSearch(model, (LoginUser)Session["LoginUser"]));
                    }
                    return View("Search", model);
                }

                CommonLog.WriteOperationLog((((LoginUser)Session["LoginUser"]).UserCode), "PDF一括出力開始", "Hoyuu");

                //帳票出力ロジックを実行
                //帳票作成ディレクトリを取得
                YearEndAdjustmentPrintBL bl = new YearEndAdjustmentPrintBL(Server.MapPath(this.GetTempDir(WebConfig.GetConfigFile())));

                //DL処理変更
                var dlpath = bl.PrintHuyouDeclare(selPrint);
                string mappath = Server.MapPath(this.GetTempDir(WebConfig.GetConfigFile()) + dlpath);
                string fileName = Path.GetFileName(mappath);
                return File(mappath, "application/zip", fileName);
            }
            catch (Exception ex) {
                // エラー
                nlog.Error(System.Reflection.MethodBase.GetCurrentMethod().Name + " error " + ex.ToString());
                TempData["Error"] = ex.ToString();
                return View("Error");
            }
            finally {
                //終了
                CommonLog.WriteOperationLog((((LoginUser)Session["LoginUser"]).UserCode), "PDF一括出力終了", "Hoyuu");

                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " end");
            }
        }

        /// <summary>
        /// 保険料控除一括帳票作成
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ActionName("Search")]
        //[ButtonHandler(ButtonName = "Print")]
        [AcceptButton(ButtonName = "PrintBatchHuyou")]
        public ActionResult PrintBatchHoken(YearEndAdjustmentSearchViewModels model, string[] selPrint) {
            try {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");

                //対象選択エラーチェック
                if (selPrint == null) {
                    //エラー判定
                    ModelState.AddModelError("", "出力対象を選択してください。");
                    if ((string)Session["SearchType"] == "Main") {
                        return View("Search", (new YearEndAdjustmentSearchBL()).Search(model, (LoginUser)Session["LoginUser"]));
                    } else if ((string)Session["SearchType"] == "Sub") {
                        return View("Search", (new YearEndAdjustmentSearchBL()).SubSearch(model, (LoginUser)Session["LoginUser"]));
                    }
                    return View("Search", model);
                }

                CommonLog.WriteOperationLog((((LoginUser)Session["LoginUser"]).UserCode), "PDF一括出力開始", "Hoyuu");

                //帳票出力ロジックを実行
                //帳票作成ディレクトリを取得
                YearEndAdjustmentPrintBL bl = new YearEndAdjustmentPrintBL(Server.MapPath(this.GetTempDir(WebConfig.GetConfigFile())));

                //DL処理変更
                var dlpath = bl.PrintHokenDeclare(selPrint);
                string mappath = Server.MapPath(this.GetTempDir(WebConfig.GetConfigFile()) + dlpath);
                string fileName = Path.GetFileName(mappath);
                return File(mappath, "application/zip", fileName);
            }
            catch (Exception ex) {
                // エラー
                nlog.Error(System.Reflection.MethodBase.GetCurrentMethod().Name + " error " + ex.ToString());
                TempData["Error"] = ex.ToString();
                return View("Error");
            }
            finally {
                //終了
                CommonLog.WriteOperationLog((((LoginUser)Session["LoginUser"]).UserCode), "PDF一括出力終了", "Hoyuu");

                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " end");
            }
        }


        /// <summary>
        /// 基礎控除一括帳票作成
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ActionName("Search")]
        //[ButtonHandler(ButtonName = "Print")]
        [AcceptButton(ButtonName = "PrintBatchHaiguu")]
        public ActionResult PrintBatchHaiguu(YearEndAdjustmentSearchViewModels model, string[] selPrint) {
            try {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");

                //対象選択エラーチェック
                if (selPrint == null) {
                    //エラー判定
                    ModelState.AddModelError("", "出力対象を選択してください。");
                    if ((string)Session["SearchType"] == "Main") {
                        return View("Search", (new YearEndAdjustmentSearchBL()).Search(model, (LoginUser)Session["LoginUser"]));
                    } else if ((string)Session["SearchType"] == "Sub") {
                        return View("Search", (new YearEndAdjustmentSearchBL()).SubSearch(model, (LoginUser)Session["LoginUser"]));
                    }
                    return View("Search", model);
                }

                CommonLog.WriteOperationLog((((LoginUser)Session["LoginUser"]).UserCode), "PDF一括出力開始", "Hoyuu");

                //帳票出力ロジックを実行
                //帳票作成ディレクトリを取得
                YearEndAdjustmentPrintBL bl = new YearEndAdjustmentPrintBL(Server.MapPath(this.GetTempDir(WebConfig.GetConfigFile())));

                //DL処理変更
                var dlpath = bl.PrintHaiguuDeclare(selPrint);
                string mappath = Server.MapPath(this.GetTempDir(WebConfig.GetConfigFile()) + dlpath);
                string fileName = Path.GetFileName(mappath);
                return File(mappath, "application/zip", fileName);
            }
            catch (Exception ex) {
                // エラー
                nlog.Error(System.Reflection.MethodBase.GetCurrentMethod().Name + " error " + ex.ToString());
                TempData["Error"] = ex.ToString();
                return View("Error");
            }
            finally {
                //終了
                CommonLog.WriteOperationLog((((LoginUser)Session["LoginUser"]).UserCode), "PDF一括出力終了", "Hoyuu");

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
        [AcceptButton(ButtonName = "SignBatchHuyou")]
        public ActionResult SignBatchHuyou(YearEndAdjustmentSearchViewModels model, string[] selPrint) {
            try {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");

                //対象選択エラーチェック
                if (selPrint == null) {
                    //エラー判定
                    ModelState.AddModelError("", "確定対象を選択してください。");
                    if ((string)Session["SearchType"] == "Main") {
                        return View("Search", (new YearEndAdjustmentSearchBL()).Search(model, (LoginUser)Session["LoginUser"]));
                    } else if ((string)Session["SearchType"] == "Sub") {
                        return View("Search", (new YearEndAdjustmentSearchBL()).SubSearch(model, (LoginUser)Session["LoginUser"]));
                    }
                    return View("Search", model);
                }

                CommonLog.WriteOperationLog((((LoginUser)Session["LoginUser"]).UserCode), "一括承認開始", "");

                //帳票出力ロジックを実行
                //帳票作成ディレクトリを取得
                YearEndAdjustmentSearchBL bl = new YearEndAdjustmentSearchBL();

                int cntTaget = selPrint.Length;                                         //選択した件数
                int cntUpdate = bl.Sign("Huyou",selPrint, (LoginUser)Session["LoginUser"]);     //承認処理：承認した件数を取得

                //結果
                //model.Down.DownloadFlag = true;
                //2016-01-21 iwai-tamura upd str -----
                //承認結果を表示するよう変更
                TempData["Success"] = string.Format("{0}名中{1}名承認しました", cntTaget, cntUpdate);
                //TempData["Success"] = "承認しました";
                //2016-01-21 iwai-tamura upd end -----                

                //表示
                //2016-01-21 iwai-tamura upd str -----
                if ((string)Session["SearchType"] == "Sub") {
                    //「部下表示」ボタンにて検索した場合
                    return View(bl.SubSearch(model, (LoginUser)Session["LoginUser"]));
                } else {
                    //「検索」ボタンにて検索した場合
                    return View(bl.Search(model, (LoginUser)Session["LoginUser"]));
                }
                //return View(bl.Search(model, (LoginUser)Session["LoginUser"]));
                //2016-01-21 iwai-tamura upd end -----                
            }
            catch (Exception ex) {
                // エラー
                nlog.Error(System.Reflection.MethodBase.GetCurrentMethod().Name + " error " + ex.ToString());
                TempData["Error"] = ex.ToString();
                return View("Error");
            }
            finally {
                //終了
                //2019-10-02 iwai-tamura add-str ------
                CommonLog.WriteOperationLog((((LoginUser)Session["LoginUser"]).UserCode), "一括承認終了", "");
                //2019-10-02 iwai-tamura add-end ------

                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " end");
            }
        }






        ///////////// <summary>
        ///////////// 帳票作成
        ///////////// </summary>
        ///////////// <returns></returns>
        //////////[HttpPost]
        //////////[ActionName("Search")]
        ////////////[ButtonHandler(ButtonName = "Print")]
        //////////[AcceptButton(ButtonName = "PrintD")]
        //////////public ActionResult PrintD(SelfDeclareSearchViewModels model, string[] selPrint) {
        //////////    try {
        //////////        //開始
        //////////        nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");

        //////////        //対象選択エラーチェック
        //////////        if(selPrint == null) {
        //////////            //エラー判定
        //////////            ModelState.AddModelError("", "出力対象を選択してください。");
        //////////            // 2017-03-31 sbc-sagara add str 選択エラー時検索結果再表示対応
        //////////            if ((string)Session["SearchType"] == "Main") {
        //////////                return View("Search", (new YearEndAdjustmentSearchBL()).Search(model, (LoginUser)Session["LoginUser"]));
        //////////            } else if ((string)Session["SearchType"] == "Sub") {
        //////////                return View("Search", (new YearEndAdjustmentSearchBL()).SubSearch(model, (LoginUser)Session["LoginUser"]));
        //////////            }
        //////////            // 2017-03-31 sbc-sagara add end 選択エラー時検索結果再表示対応
        //////////            return View("Search", model);
        //////////        }

        //////////        //2019-10-02 iwai-tamura add-str ------
        //////////        CommonLog.WriteOperationLog((((LoginUser)Session["LoginUser"]).UserCode),"PDF一括出力開始","D");
        //////////        //2019-10-02 iwai-tamura add-end ------

        //////////        //帳票出力ロジックを実行
        //////////        //帳票作成ディレクトリを取得
        //////////        SelfDeclareSearchPrintBL bl = new SelfDeclareSearchPrintBL(Server.MapPath(this.GetTempDir(WebConfig.GetConfigFile())));
        //////////        //結果
        //////////        //model.Down.DownloadPath = bl.Print(selPrint);
        //////////        //model.Down.DownloadFlag = string.IsNullOrEmpty(model.Down.DownloadPath) ? false : true;
        //////////        ////表示
        //////////        //return View((new SelfDeclareSearchBL()).Search(model, (LoginUser)Session["LoginUser"]));

        //////////        //DL処理変更
        //////////        var dlpath = bl.PrintD(selPrint);
        //////////        string mappath = Server.MapPath(this.GetTempDir(WebConfig.GetConfigFile()) + dlpath);
        //////////        string fileName = Path.GetFileName(mappath);
        //////////        return File(mappath, "application/zip", fileName);
        //////////    } catch(Exception ex) {
        //////////        // エラー
        //////////        nlog.Error(System.Reflection.MethodBase.GetCurrentMethod().Name + " error " + ex.ToString());
        //////////        TempData["Error"] = ex.ToString();
        //////////        return View("Error");
        //////////    } finally {
        //////////        //終了
        //////////        //2019-10-02 iwai-tamura add-str ------
        //////////        CommonLog.WriteOperationLog((((LoginUser)Session["LoginUser"]).UserCode),"PDF一括出力終了","D");
        //////////        //2019-10-02 iwai-tamura add-end ------

        //////////        nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " end");
        //////////    }
        //////////}

        ///////////// <summary>
        ///////////// 帳票作成
        ///////////// </summary>
        ///////////// <returns></returns>
        //////////[HttpPost]
        //////////[ActionName("Search")]
        ////////////[ButtonHandler(ButtonName = "Print")]
        //////////[AcceptButton(ButtonName = "PrintCareer")]
        //////////public ActionResult PrintCareer(SelfDeclareSearchViewModels model, string[] selPrint) {
        //////////    try {
        //////////        //開始
        //////////        nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");

        //////////        //対象選択エラーチェック
        //////////        if(selPrint == null) {
        //////////            //エラー判定
        //////////            ModelState.AddModelError("", "出力対象を選択してください。");
        //////////            // 2017-03-31 sbc-sagara add str 選択エラー時検索結果再表示対応
        //////////            if ((string)Session["SearchType"] == "Main") {
        //////////                return View("Search", (new YearEndAdjustmentSearchBL()).Search(model, (LoginUser)Session["LoginUser"]));
        //////////            } else if ((string)Session["SearchType"] == "Sub") {
        //////////                return View("Search", (new YearEndAdjustmentSearchBL()).SubSearch(model, (LoginUser)Session["LoginUser"]));
        //////////            }
        //////////            // 2017-03-31 sbc-sagara add end 選択エラー時検索結果再表示対応
        //////////            return View("Search", model);
        //////////        }

        //////////        //2019-10-02 iwai-tamura add-str ------
        //////////        CommonLog.WriteOperationLog((((LoginUser)Session["LoginUser"]).UserCode),"PDF一括出力開始","Career");
        //////////        //2019-10-02 iwai-tamura add-end ------

        //////////        //帳票出力ロジックを実行
        //////////        //帳票作成ディレクトリを取得
        //////////        SelfDeclareSearchPrintBL bl = new SelfDeclareSearchPrintBL(Server.MapPath(this.GetTempDir(WebConfig.GetConfigFile())));
        //////////        //結果
        //////////        //model.Down.DownloadPath = bl.Print(selPrint);
        //////////        //model.Down.DownloadFlag = string.IsNullOrEmpty(model.Down.DownloadPath) ? false : true;
        //////////        ////表示
        //////////        //return View((new SelfDeclareSearchBL()).Search(model, (LoginUser)Session["LoginUser"]));

        //////////        //DL処理変更
        //////////        var dlpath = bl.PrintCarrier(selPrint);
        //////////        string mappath = Server.MapPath(this.GetTempDir(WebConfig.GetConfigFile()) + dlpath);
        //////////        string fileName = Path.GetFileName(mappath);
        //////////        return File(mappath, "application/zip", fileName);
        //////////    } catch(Exception ex) {
        //////////        // エラー
        //////////        nlog.Error(System.Reflection.MethodBase.GetCurrentMethod().Name + " error " + ex.ToString());
        //////////        TempData["Error"] = ex.ToString();
        //////////        return View("Error");
        //////////    } finally {
        //////////        //終了
        //////////        //2019-10-02 iwai-tamura add-str ------
        //////////        CommonLog.WriteOperationLog((((LoginUser)Session["LoginUser"]).UserCode),"PDF一括出力終了","Career");
        //////////        //2019-10-02 iwai-tamura add-end ------

        //////////        nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " end");
        //////////    }
        //////////}



        ///////////// <summary>
        ///////////// AtoC表 Excel作成
        ///////////// </summary>
        ///////////// <returns></returns>
        //////////[HttpPost]
        //////////[ActionName("Search")]
        ////////////[ButtonHandler(ButtonName = "Print")]
        //////////[AcceptButton(ButtonName = "PrintAtoCXls")]
        //////////public ActionResult PrintAtoCXls(YearEndAdjustmentSearchViewModels model, string[] selPrint,string tblType)
        //////////{
        //////////    try
        //////////    {
        //////////        //開始
        //////////        nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");

        //////////        //対象選択エラーチェック
        //////////        if (selPrint == null)
        //////////        {
        //////////            //エラー判定
        //////////            ModelState.AddModelError("", "出力対象を選択してください。");

        //////////            if ((string)Session["SearchType"] == "Main") {
        //////////                return View("Search", (new YearEndAdjustmentSearchBL()).Search(model, (LoginUser)Session["LoginUser"]));
        //////////            } else if ((string)Session["SearchType"] == "Sub") {
        //////////                return View("Search", (new YearEndAdjustmentSearchBL()).SubSearch(model, (LoginUser)Session["LoginUser"]));
        //////////            }
        //////////            return View("Search", model);
        //////////        }

        //////////        //管理者判定
        //////////        if (((LoginUser)Session["LoginUser"]).自己申告書DATA出力 != "1")
        //////////        {
        //////////            TempData["Confirmation"] = string.Format("権限が無い為、この機能は使えません。");
        //////////            if ((string)Session["SearchType"] == "Main") {
        //////////                return View("Search", (new YearEndAdjustmentSearchBL()).Search(model, (LoginUser)Session["LoginUser"]));
        //////////            } else if ((string)Session["SearchType"] == "Sub") {
        //////////                return View("Search", (new YearEndAdjustmentSearchBL()).SubSearch(model, (LoginUser)Session["LoginUser"]));
        //////////            }
        //////////            return View("Search", model);                
        //////////        }

        //////////        //2019-10-02 iwai-tamura add-str ------
        //////////        CommonLog.WriteOperationLog((((LoginUser)Session["LoginUser"]).UserCode),"Excel出力開始",tblType);
        //////////        //2019-10-02 iwai-tamura add-end ------

        //////////        //帳票出力ロジックを実行
        //////////        //帳票作成ディレクトリを取得
        //////////        SelfDeclareSearchPrintBL bl = new SelfDeclareSearchPrintBL(Server.MapPath(this.GetTempDir(WebConfig.GetConfigFile())));

        //////////        //DL処理変更
        //////////        var dlpath = bl.PrintXls(selPrint,tblType);
        //////////        string mappath = Server.MapPath(this.GetTempDir(WebConfig.GetConfigFile()) + dlpath);
        //////////        string fileName = Path.GetFileName(mappath);
        //////////        return File(mappath, "application/zip", fileName);
        //////////    }
        //////////    catch (Exception ex)
        //////////    {
        //////////        // エラー
        //////////        nlog.Error(System.Reflection.MethodBase.GetCurrentMethod().Name + " error " + ex.ToString());
        //////////        TempData["Error"] = ex.ToString();
        //////////        return View("Error");
        //////////    }
        //////////    finally
        //////////    {
        //////////        //終了
        //////////        //2019-10-02 iwai-tamura add-str ------
        //////////        CommonLog.WriteOperationLog((((LoginUser)Session["LoginUser"]).UserCode),"Excel出力終了",tblType);
        //////////        //2019-10-02 iwai-tamura add-end ------

        //////////        nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " end");
        //////////    }
        //////////}

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
