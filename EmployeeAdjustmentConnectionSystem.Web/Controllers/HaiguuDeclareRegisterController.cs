﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Transactions;
using EmployeeAdjustmentConnectionSystem.BL.Common;
using EmployeeAdjustmentConnectionSystem.BL.HaiguuDeclareRegister;
using EmployeeAdjustmentConnectionSystem.COM.Enum;
using EmployeeAdjustmentConnectionSystem.COM.Models;
using EmployeeAdjustmentConnectionSystem.COM.Util.Controll;
using EmployeeAdjustmentConnectionSystem.COM.Util.Convert;
using EmployeeAdjustmentConnectionSystem.COM.Util.Database;
using EmployeeAdjustmentConnectionSystem.COM.Entity.Session;
using EmployeeAdjustmentConnectionSystem.BL.Login;
using EmployeeAdjustmentConnectionSystem.BL.SelfDeclareSearch;
using EmployeeAdjustmentConnectionSystem.COM.Util.File;
using EmployeeAdjustmentConnectionSystem.COM.Util.Config;
using System.IO;
using EmployeeAdjustmentConnectionSystem.Log.Common;    //2019-10-02 iwai-tamura add

namespace EmployeeAdjustmentConnectionSystem.Web.Controllers {
    /// <summary>
    /// 扶養控除申告書入力コントローラー
    /// </summary>
    public class HaiguuDeclareRegisterController : Controller {
        #region メンバ変数
        /// <summary>
        /// ロガー
        /// </summary>
        private static NLog.Logger nlog = NLog.LogManager.GetCurrentClassLogger();

        #endregion

        /// <summary>
        /// 初期表示
        /// </summary>
        /// <param name="id">管理番号</param>
        /// <param name="tableType">テーブルタイプ</param>
        /// <returns>ビュー</returns>
        public ActionResult Index(int? intSheetYear, string strEmployeeNo,bool bolAdminMode) {
            try {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");

                //ログイン判定
                if(!(new LoginBL()).IsLogin()) return RedirectToAction("Login", "Login");

                //ビジネスロジックから取得
                //return View((new HaiguuDeclareRegisterBL(tableType)).Select(id));
                var bl = new HaiguuDeclareRegisterBL();
                var model = bl.Select(intSheetYear,strEmployeeNo,bolAdminMode);
                var lu = (LoginUser)Session["LoginUser"];
                bl.SetMode(model, lu);
                
                return View(model);
            } catch(Exception ex) {
                // エラー
                nlog.Error(System.Reflection.MethodBase.GetCurrentMethod().Name + " error " + ex.ToString());
                TempData["Error"] = ex.ToString();
                return View("Error");
            } finally {
                //終了
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " end");
            }
        }

        /// <summary>
        /// 提出登録キャンセル
        /// </summary>
        /// <param name="model">扶養控除申告書入力モデル</param>
        /// <param name="value">ボタンのValue</param>
        /// <returns>ビュー</returns>
        [HttpPost]
        [ActionName("Index")]
        [ButtonHandler(ButtonName = "SignCancel")]
        //[AcceptButton(ButtonName = "SignCancel")]
        public ActionResult SignCancel(HaiguuDeclareRegisterViewModels model, string value) {
            try {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");

                CommonLog.WriteOperationLog((((LoginUser)Session["LoginUser"]).UserCode),"提出キャンセル開始","");

                //ログイン判定
                if(!(new LoginBL()).IsLogin()) return RedirectToAction("Login", "Login");

                //セッションからログイン情報取得
                LoginUser lu = (LoginUser)Session["LoginUser"];

                //ビジネスロジックから
                HaiguuDeclareRegisterBL bl = new HaiguuDeclareRegisterBL();

                //提出キャンセル
                bl.Sign(model, value, lu, false);

                var intSheetYear = model.Head.SheetYear;
                var strEmployeeNo = model.Head.EmployeeNo;
                var bolAdminMode = model.Head.AdminMode;
                ModelState.Clear();
                //再表示
                bl = new HaiguuDeclareRegisterBL();
                model = bl.Select(intSheetYear,strEmployeeNo,bolAdminMode);
                bl.SetMode(model, lu);
                //2023-11-20 iwai-tamura upd str -----
                if (bolAdminMode){
                    TempData["Success"] = "確定をキャンセルしました";
                } else{
                    TempData["Success"] = "提出をキャンセルしました";
                }
                //TempData["Success"] = "提出をキャンセルしました";
                //2023-11-20 iwai-tamura upd end -----
                return View(model);
            } catch(Exception ex) {
                // エラー
                nlog.Error(System.Reflection.MethodBase.GetCurrentMethod().Name + " error " + ex.ToString());
                TempData["Error"] = ex.ToString();
                return View("Error");
            } finally {
                //終了
                CommonLog.WriteOperationLog((((LoginUser)Session["LoginUser"]).UserCode),"提出キャンセル終了","");

                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " end");
            }
        }

        /// <summary>
        /// 提出保存
        /// </summary>
        /// <param name="model">扶養控除申告書入力モデル</param>
        /// <returns>ビュー</returns>
        [HttpPost]
        [ActionName("Index")]
        //[ButtonHandler(ButtonName = "Save")]
        [AcceptButton(ButtonName = "Save")]
        public ActionResult Save(HaiguuDeclareRegisterViewModels model) {
            try {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");

                CommonLog.WriteOperationLog((((LoginUser)Session["LoginUser"]).UserCode),"提出開始","");

                //ログイン判定
                if(!(new LoginBL()).IsLogin()) return RedirectToAction("Login", "Login");

                //セッションからログイン情報取得
                LoginUser lu = (LoginUser)Session["LoginUser"];

                //ビジネスロジック
                HaiguuDeclareRegisterBL bl = new HaiguuDeclareRegisterBL();

                //保存
                bl.Save(model, lu,"2");

                var intSheetYear = model.Head.SheetYear;
                var strEmployeeNo = model.Head.EmployeeNo;
                var bolAdminMode = model.Head.AdminMode;

                ModelState.Clear();
                //再表示
                bl = new HaiguuDeclareRegisterBL();
                model = bl.Select(intSheetYear,strEmployeeNo,bolAdminMode);
                bl.SetMode(model, lu);

                //2023-11-20 iwai-tamura upd str -----
                if (bolAdminMode){
                    TempData["Success"] = "確定しました";
                } else{
                    TempData["Success"] = "提出しました";
                }
                //TempData["Success"] = "提出しました";
                //2023-11-20 iwai-tamura upd end -----

                return View(model);
            } catch(Exception ex) {
                // エラー
                nlog.Error(System.Reflection.MethodBase.GetCurrentMethod().Name + " error " + ex.ToString());
                TempData["Error"] = ex.ToString();
                return View("Error");
            } finally {
                //終了
                //2019-10-02 iwai-tamura add-str ------
                CommonLog.WriteOperationLog((((LoginUser)Session["LoginUser"]).UserCode),"提出終了","");
                //2019-10-02 iwai-tamura add-end ------

                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " end");
            }
        }

        /// <summary>
        /// 途中保存
        /// </summary>
        /// <param name="model">扶養控除申告書入力モデル</param>
        /// <returns>ビュー</returns>
        [HttpPost]
        [ActionName("Index")]
        //[ButtonHandler(ButtonName = "Keep")]
        [AcceptButton(ButtonName = "Keep")]
        public ActionResult Keep(HaiguuDeclareRegisterViewModels model) {
            try {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");

                CommonLog.WriteOperationLog((((LoginUser)Session["LoginUser"]).UserCode),"保存開始","");

                //ログイン判定
                if(!(new LoginBL()).IsLogin()) return RedirectToAction("Login", "Login");

                //セッションからログイン情報取得
                LoginUser lu = (LoginUser)Session["LoginUser"];

                //ビジネスロジック
                HaiguuDeclareRegisterBL bl = new HaiguuDeclareRegisterBL();

                //保存
                bl.Save(model, lu,"1");

                var intSheetYear = model.Head.SheetYear;
                var strEmployeeNo = model.Head.EmployeeNo;
                var bolAdminMode = model.Head.AdminMode;

                ModelState.Clear();
                //再表示
                bl = new HaiguuDeclareRegisterBL();
                model = bl.Select(intSheetYear,strEmployeeNo,bolAdminMode);
                bl.SetMode(model, lu);
                TempData["Success"] = "途中保存しました";
                return View(model);
            } catch(Exception ex) {
                // エラー
                nlog.Error(System.Reflection.MethodBase.GetCurrentMethod().Name + " error " + ex.ToString());
                TempData["Error"] = ex.ToString();
                return View("Error");
            } finally {
                //終了
                //2019-10-02 iwai-tamura add-str ------
                CommonLog.WriteOperationLog((((LoginUser)Session["LoginUser"]).UserCode),"保存終了","");
                //2019-10-02 iwai-tamura add-end ------

                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " end");
            }
        }

        /// <summary>
        /// 戻る
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ActionName("Index")]
        [AcceptButton(ButtonName = "Back")]
        public ActionResult Back(HaiguuDeclareRegisterViewModels model, string value) {
            try {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");


                //2024-11-19 iwai-tamura upd-str ------
                //バーコード読取の場合トップ画面に戻る
                if ((string)Session["ReturnTopStatus"] == "1") {
                    return RedirectToAction("Index", "Top");
                }
                //2024-11-19 iwai-tamura upd-end ------

                //トップへ
                //2023-11-20 iwai-tamura upd str -----
                if (model.Head.AdminMode){
                    return RedirectToAction("Search", "YearEndAdjustmentSearch");
                } else{
                    return RedirectToAction("Index", "Top");
                }
                //return RedirectToAction("Index", "Top");
                //2023-11-20 iwai-tamura upd end -----
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
    }
}
