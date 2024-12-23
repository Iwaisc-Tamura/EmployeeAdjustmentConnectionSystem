﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using EmployeeAdjustmentConnectionSystem.COM.Models;
using EmployeeAdjustmentConnectionSystem.COM.Util.Controll;
using EmployeeAdjustmentConnectionSystem.COM.Util.Database;
using EmployeeAdjustmentConnectionSystem.COM.Entity.Session;
using EmployeeAdjustmentConnectionSystem.BL.Login;
using EmployeeAdjustmentConnectionSystem.Bl.Top;
using System.Collections;
using EmployeeAdjustmentConnectionSystem.BL.SelfDeclareSearch;
using EmployeeAdjustmentConnectionSystem.COM.Util.Config;
using System.IO;
using EmployeeAdjustmentConnectionSystem.COM.Util.File;
using System.Configuration;
using EmployeeAdjustmentConnectionSystem.BL.YearEndAdjustmentReports;
using System.Web.Services.Description;

namespace EmployeeAdjustmentConnectionSystem.Web.Controllers {

    /// <summary>
    /// TOP画面用コントローラー
    /// </summary>
    public class TopController : Controller {
        #region メンバ変数
        /// <summary>
        /// ロガー
        /// </summary>
        private static NLog.Logger nlog = NLog.LogManager.GetCurrentClassLogger();

        /// <summary>
        /// ボタンリンクアクションテーブル
        /// </summary>
        /// <remarks>
        /// 目標管理、職能判定、システム管理、パスワード変更、ログアウト
        /// </remarks>
        private Hashtable linkTable = new Hashtable() { { "ObjectivesSearch","Search" }
                                                      , { "SkillSearch","Search" }
                                                      , { "SystemManagement", "Index" }
                                                      , { "ChangePassword", "ChangePassword"} 
                                                      , { "Login", "Login" } 
                                                      , { "HuyouDeclareRegister", "Index"}
                                                      , { "HokenDeclareRegister", "Index"}
                                                      , { "HaiguuDeclareRegister", "Index"}
                                                    //2023-11-20 iwai-tamura add str -----
                                                      , { "YearEndAdjustmentSearch", "Search"}
                                                    //2023-11-20 iwai-tamura add end -----                                                        
            };
        #endregion

        /// <summary>
        /// 初期表示
        /// </summary>
        /// <returns>画面表示</returns>
        public ActionResult Index() {
            try {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");

                //ログイン判定
                if(!(new LoginBL()).IsLogin()) return RedirectToAction("Login", "Login");
                
                //ログイン情報取得
                var lu = (LoginUser)Session["LoginUser"];
                

                //2023-11-20 iwai-tamura upd str -----
                //初期化
                TempData["YearEndAdjustmentSearch"] = null;
                //2023-11-20 iwai-tamura upd end -----

                //2023-12-15 iwai-tamura upd str -----
                ViewBag.ServerStatus = lu.IsServerStatus;
                //2023-12-15 iwai-tamura upd end -----

                //表示
                return View((new TopBL()).Index());
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
        /// 画面遷移アクション
        /// </summary>
        /// <param name="value">遷移先キー名</param>
        /// <returns>画面遷移</returns>
        [ActionName("Link")]
        [ButtonHandler(ButtonName = "Transition")]
        public ActionResult TransitionAll(string value,string specifyEmployeeNo = "") {
            try {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");
                
                //ログイン判定
                if(!(new LoginBL()).IsLogin()) return RedirectToAction("Login", "Login");

                //セッション全削除判定 ログアウトボタンのみセッション削除
                if("Login".Equals(value)) Session.RemoveAll();

                //遷移
                LoginUser lu = (LoginUser)Session["LoginUser"];
                var strInputNo = "";
                var bolAdminMode = false;
                if (specifyEmployeeNo == "") {
                    strInputNo = lu.UserCode;
                } else {
                    strInputNo = specifyEmployeeNo;
                    //2024-11-19 iwai-tamura upd-str ------
                    bolAdminMode = false;
                    //bolAdminMode = true;
                    //2024-11-19 iwai-tamura upd-end ------
                }

                //2023-11-20 iwai-tamura upd str -----
                return RedirectToAction((string)linkTable[value], value, new { intSheetYear = lu.IsYear, strEmployeeNo = strInputNo ,bolAdminMode = bolAdminMode});
                //return RedirectToAction((string)linkTable[value], value, new { intSheetYear = 2022, strEmployeeNo = strInputNo ,bolAdminMode = bolAdminMode});
                //2023-11-20 iwai-tamura upd end -----
                //return RedirectToAction("Index", "HuyouDeclareRegister", new { intSheetYear = 2022, strEmployeeNo = "" });
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

        //2024-11-19 iwai-tamura upd-str ------
        /// <summary>
        /// 画面遷移アクション
        /// </summary>
        /// <param name="value">遷移先キー名</param>
        /// <returns>画面遷移</returns>
        [ActionName("Link")]
        [ButtonHandler(ButtonName = "BarCodeTransition")]
        public ActionResult BarCodeTransitionAll(string value) {
            try {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");
                
                //ログイン判定
                if(!(new LoginBL()).IsLogin()) return RedirectToAction("Login", "Login");

                //セッション全削除判定 ログアウトボタンのみセッション削除
                if("Login".Equals(value)) Session.RemoveAll();

                //遷移
                LoginUser lu = (LoginUser)Session["LoginUser"];

                //帳票区分、年度、社員番号に分割
                var strReportType = value.Substring(0, 1);
                var intYear = value.Substring(1, 2);
                var strInputNo = value.Substring(3, 5);

                // 年度の補完 (例: "24" -> "2024")
                if (int.TryParse(intYear, out int parsedYear)) {
                    intYear = (2000 + parsedYear).ToString();
                }

                // 帳票区分に基づく帳票名の設定
                string strReportName;
                switch (strReportType) {
                    case "1":
                        strReportName = "HuyouDeclareRegister";
                        break;
                    case "2":
                        strReportName = "HokenDeclareRegister";
                        break;
                    case "3":
                        strReportName = "HaiguuDeclareRegister";
                        break;
                    default:
                        TempData["Error"] = "無効なコードです。";
                        return View("Error");
                }

                Session["ReturnTopStatus"] = "1";
                return RedirectToAction((string)linkTable[strReportName], strReportName, new { intSheetYear = intYear, strEmployeeNo = strInputNo ,bolAdminMode = true});
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
        //2024-11-19 iwai-tamura upd-end ------

        //2023-11-20 iwai-tamura test-str ------
        /// <summary>
        /// 画面遷移アクション
        /// </summary>
        /// <param name="value">遷移先キー名</param>
        /// <returns>画面遷移</returns>
        [ActionName("Link")]
        [ButtonHandler(ButtonName = "ManagementTransition")]
        public ActionResult ManagementTransition(string value,string specifyEmployeeNo = "") {
            try {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");
                
                //ログイン判定
                if(!(new LoginBL()).IsLogin()) return RedirectToAction("Login", "Login");

                //セッション全削除判定 ログアウトボタンのみセッション削除
                if("Login".Equals(value)) Session.RemoveAll();

                //遷移
                LoginUser lu = (LoginUser)Session["LoginUser"];

                //2024-11-19 iwai-tamura upd-str ------
                //戻り先を検索画面に
                Session["ReturnTopStatus"] = "0";
                //2024-11-19 iwai-tamura upd-end ------


                return RedirectToAction((string)linkTable[value], value);
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
        /// 基礎控除印刷
        /// </summary>
        /// <param name="value">印刷キー名</param>
        [ActionName("Link")]
        [ButtonHandler(ButtonName = "PrintHaiguu")]
        public ActionResult PrintHaiguuReport(string value, string specifyEmployeeNo = "")
        {
            try
            {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");

                //ログイン判定
                if (!(new LoginBL()).IsLogin()) return RedirectToAction("Login", "Login");

                //セッション全削除判定 ログアウトボタンのみセッション削除
                if ("Login".Equals(value)) Session.RemoveAll();

                LoginUser lu = (LoginUser)Session["LoginUser"];
                var strInputNo = "";
                var bolAdminMode = false;
                if (specifyEmployeeNo == "")
                {
                    strInputNo = lu.UserCode;
                } else
                {
                    strInputNo = specifyEmployeeNo;
                    bolAdminMode = true;
                }

                //帳票出力ロジックを実行
                //帳票作成ディレクトリを取得
                YearEndAdjustmentPrintBL bl = new YearEndAdjustmentPrintBL(Server.MapPath(this.GetTempDir(WebConfig.GetConfigFile())));
                //結果
                //model.Down.DownloadPath = bl.Print(new string[] { value });
                //model.Down.DownloadFlag = string.IsNullOrEmpty(model.Down.DownloadPath) ? false : true;

                ////表示
                //return View((new SelfDeclareSearchBL()).Search(model, (LoginUser)Session["LoginUser"]));
                //DL処理変更
                var dlpath = bl.PrintHaiguuDeclare(new string[] { String.Join(",",lu.IsYear, strInputNo ) });
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
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " end");
            }
        }

        /// <summary>
        /// コンフィグから帳票出力フォルダ取得
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        private string GetTempDir(Configuration config)
        {
            try
            {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");
                return FileUtil.GetTempDir(config, "DOWNLOAD_TEMP_DIR_O");
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

        //2023-11-20 iwai-terao test-str ------
        /// <summary>
        /// 扶養控除申告書印刷
        /// </summary>
        /// <param name="value">印刷キー名</param>
        [ActionName("Link")]
        [ButtonHandler(ButtonName = "PrintHuyou")]
        public ActionResult PrintHuyouReport(string value, string specifyEmployeeNo = "")
        {
            try
            {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");

                //ログイン判定
                if (!(new LoginBL()).IsLogin()) return RedirectToAction("Login", "Login");

                //セッション全削除判定 ログアウトボタンのみセッション削除
                if ("Login".Equals(value)) Session.RemoveAll();

                LoginUser lu = (LoginUser)Session["LoginUser"];
                var strInputNo = "";
                var bolAdminMode = false;
                if (specifyEmployeeNo == "")
                {
                    strInputNo = lu.UserCode;
                }
                else
                {
                    strInputNo = specifyEmployeeNo;
                    bolAdminMode = true;
                }

                //帳票出力ロジックを実行
                //帳票作成ディレクトリを取得
                YearEndAdjustmentPrintBL bl = new YearEndAdjustmentPrintBL(Server.MapPath(this.GetTempDir(WebConfig.GetConfigFile())));
                //結果
                //model.Down.DownloadPath = bl.Print(new string[] { value });
                //model.Down.DownloadFlag = string.IsNullOrEmpty(model.Down.DownloadPath) ? false : true;

                ////表示
                //return View((new SelfDeclareSearchBL()).Search(model, (LoginUser)Session["LoginUser"]));
                //DL処理変更
                var dlpath = bl.PrintHuyouDeclare(new string[] { String.Join(",",lu.IsYear, strInputNo ) });
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
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " end");
            }
        }


        //2023-11-20 iwai-terao test-end ------


        //2023-11-20 iwai-terao test-str ------
        /// <summary>
        /// 保険料控除申告書印刷
        /// </summary>
        /// <param name="value">印刷キー名</param>
        [ActionName("Link")]
        [ButtonHandler(ButtonName = "PrintHoken")]
        public ActionResult PrintHokenReport(string value, string specifyEmployeeNo = "")
        {
            try
            {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");

                //ログイン判定
                if (!(new LoginBL()).IsLogin()) return RedirectToAction("Login", "Login");

                //セッション全削除判定 ログアウトボタンのみセッション削除
                if ("Login".Equals(value)) Session.RemoveAll();

                LoginUser lu = (LoginUser)Session["LoginUser"];
                var strInputNo = "";
                var bolAdminMode = false;
                if (specifyEmployeeNo == "")
                {
                    strInputNo = lu.UserCode;
                }
                else
                {
                    strInputNo = specifyEmployeeNo;
                    bolAdminMode = true;
                }

                //帳票出力ロジックを実行
                //帳票作成ディレクトリを取得
                YearEndAdjustmentPrintBL bl = new YearEndAdjustmentPrintBL(Server.MapPath(this.GetTempDir(WebConfig.GetConfigFile())));
                //結果
                //model.Down.DownloadPath = bl.Print(new string[] { value });
                //model.Down.DownloadFlag = string.IsNullOrEmpty(model.Down.DownloadPath) ? false : true;

                ////表示
                //return View((new SelfDeclareSearchBL()).Search(model, (LoginUser)Session["LoginUser"]));
                //DL処理変更
                var dlpath = bl.PrintHokenDeclare(new string[] { String.Join(",",lu.IsYear, strInputNo ) });
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
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " end");
            }
        }


        //2023-11-20 iwai-terao test-end ------
    }
}
