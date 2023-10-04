using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Transactions;
using EmployeeAdjustmentConnectionSystem.BL.Common;
using EmployeeAdjustmentConnectionSystem.BL.SelfDeclareRegister;
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
    /// 目標管理入力コントローラー
    /// </summary>
    public class SelfDeclareRegisterBController : Controller {
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
        public ActionResult Index(int? id, string tableType) {
            try {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");

                //ログイン判定
                if(!(new LoginBL()).IsLogin()) return RedirectToAction("Login", "Login");

                //ビジネスロジックから取得
                //return View((new SelfDeclareRegisterBBL(tableType)).Select(id));
                var bl = new SelfDeclareRegisterBBL();
                var model = bl.Select(id);
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
        /// 承認
        /// </summary>
        /// <param name="model">目標管理入力モデル</param>
        /// <param name="value">ボタンのValue</param>
        /// <returns>ビュー</returns>
        [HttpPost]
        [ActionName("Index")]
        [ButtonHandler(ButtonName = "Signature")]
        //[AcceptButton(ButtonName = "Signature")]
        public ActionResult Sign(SelfDeclareRegisterBViewModels model, string value) {
            try {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");

                //2019-10-02 iwai-tamura add-str ------
                CommonLog.WriteOperationLog((((LoginUser)Session["LoginUser"]).UserCode),"承認開始","");
                //2019-10-02 iwai-tamura add-end ------

                //ログイン判定
                if(!(new LoginBL()).IsLogin()) return RedirectToAction("Login", "Login");

                //セッションからログイン情報取得
                LoginUser lu = (LoginUser)Session["LoginUser"];

                //ビジネスロジックから
                SelfDeclareRegisterBBL bl = new SelfDeclareRegisterBBL();

                //承認
                bl.Sign(model, value, lu);

                var mNo = DataConv.IntParse(model.Head.ManageNo);
                ModelState.Clear();
                //再表示
                bl = new SelfDeclareRegisterBBL();
                model = bl.Select(mNo);
                bl.SetMode(model, lu);
                TempData["Success"] = "承認しました";
                return View(model);

                ////再表示
                //return View(bl.Select(mNo));
            } catch(Exception ex) {
                // エラー
                nlog.Error(System.Reflection.MethodBase.GetCurrentMethod().Name + " error " + ex.ToString());
                TempData["Error"] = ex.ToString();
                return View("Error");
            } finally {
                //終了
                //2019-10-02 iwai-tamura add-str ------
                CommonLog.WriteOperationLog((((LoginUser)Session["LoginUser"]).UserCode),"承認終了","");
                //2019-10-02 iwai-tamura add-end ------

                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " end");
            }
        }

        /// <summary>
        /// 承認キャンセル
        /// </summary>
        /// <param name="model">目標管理入力モデル</param>
        /// <param name="value">ボタンのValue</param>
        /// <returns>ビュー</returns>
        [HttpPost]
        [ActionName("Index")]
        [ButtonHandler(ButtonName = "SignCancel")]
        //[AcceptButton(ButtonName = "SignCancel")]
        public ActionResult SignCancel(SelfDeclareRegisterBViewModels model, string value) {
            try {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");

                //2019-10-02 iwai-tamura add-str ------
                CommonLog.WriteOperationLog((((LoginUser)Session["LoginUser"]).UserCode),"承認キャンセル開始","");
                //2019-10-02 iwai-tamura add-end ------

                //ログイン判定
                if(!(new LoginBL()).IsLogin()) return RedirectToAction("Login", "Login");

                //セッションからログイン情報取得
                LoginUser lu = (LoginUser)Session["LoginUser"];

                //ビジネスロジックから
                SelfDeclareRegisterBBL bl = new SelfDeclareRegisterBBL();

                //承認
                bl.Sign(model, value, lu, false);

                var mNo = DataConv.IntParse(model.Head.ManageNo);
                ModelState.Clear();
                //再表示
                bl = new SelfDeclareRegisterBBL();
                model = bl.Select(mNo);
                bl.SetMode(model, lu);
                TempData["Success"] = "承認をキャンセルしました";
                return View(model);
            } catch(Exception ex) {
                // エラー
                nlog.Error(System.Reflection.MethodBase.GetCurrentMethod().Name + " error " + ex.ToString());
                TempData["Error"] = ex.ToString();
                return View("Error");
            } finally {
                //終了
                //2019-10-02 iwai-tamura add-str ------
                CommonLog.WriteOperationLog((((LoginUser)Session["LoginUser"]).UserCode),"承認キャンセル終了","");
                //2019-10-02 iwai-tamura add-end ------

                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " end");
            }
        }

        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="model">目標管理入力モデル</param>
        /// <returns>ビュー</returns>
        [HttpPost]
        [ActionName("Index")]
        //[ButtonHandler(ButtonName = "Save")]
        [AcceptButton(ButtonName = "Save")]
        public ActionResult Save(SelfDeclareRegisterBViewModels model) {
            try {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");

                //2019-10-02 iwai-tamura add-str ------
                CommonLog.WriteOperationLog((((LoginUser)Session["LoginUser"]).UserCode),"保存開始","");
                //2019-10-02 iwai-tamura add-end ------

                //ログイン判定
                if(!(new LoginBL()).IsLogin()) return RedirectToAction("Login", "Login");

                //セッションからログイン情報取得
                LoginUser lu = (LoginUser)Session["LoginUser"];

                //ビジネスロジック
                SelfDeclareRegisterBBL bl = new SelfDeclareRegisterBBL();

                //保存
                bl.Save(model);

                var mNo = DataConv.IntParse(model.Head.ManageNo);
                var tableType = model.Head.SelfDeclareCode;
                ModelState.Clear();
                //再表示
                bl = new SelfDeclareRegisterBBL();
                model = bl.Select(mNo);
                bl.SetMode(model, lu);
                TempData["Success"] = "保存しました";
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
        /// 前回データ取得
        /// </summary>
        /// <param name="model">自己申告書入力モデル</param>
        /// <returns>ビュー</returns>
        [HttpPost]
        [ActionName("Index")]
        //[ButtonHandler(ButtonName = "PrevDataCopy")]
        [AcceptButton(ButtonName = "PrevDataCopy")]
        public ActionResult PrevDataCopy(SelfDeclareRegisterBViewModels model) {
            try {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");

                //2019-10-02 iwai-tamura add-str ------
                CommonLog.WriteOperationLog((((LoginUser)Session["LoginUser"]).UserCode),"前回データ取得開始","");
                //2019-10-02 iwai-tamura add-end ------

                //ログイン判定
                if(!(new LoginBL()).IsLogin()) return RedirectToAction("Login", "Login");

                //セッションからログイン情報取得
                LoginUser lu = (LoginUser)Session["LoginUser"];

                //ビジネスロジック
                SelfDeclareRegisterBBL bl = new SelfDeclareRegisterBBL();

                //保存
                bl.PrevDataCopy(model,lu);

                var mNo = DataConv.IntParse(model.Head.ManageNo);
                var tableType = model.Head.SelfDeclareCode;
                ModelState.Clear();
                //再表示
                bl = new SelfDeclareRegisterBBL();
                model = bl.Select(mNo);
                bl.SetMode(model, lu);
                TempData["Success"] = "前回データをコピーしました。";
                return View(model);
            } catch(Exception ex) {
                // エラー
                nlog.Error(System.Reflection.MethodBase.GetCurrentMethod().Name + " error " + ex.ToString());
                TempData["Error"] = ex.ToString();
                return View("Error");
            } finally {
                //終了
                //2019-10-02 iwai-tamura add-str ------
                CommonLog.WriteOperationLog((((LoginUser)Session["LoginUser"]).UserCode),"前回データ取得終了","");
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
        public ActionResult Back(SelfDeclareRegisterBViewModels model, string value) {
            try {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");
                //検索画面へ
                return RedirectToAction("Search", "SelfDeclareSearch");
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
