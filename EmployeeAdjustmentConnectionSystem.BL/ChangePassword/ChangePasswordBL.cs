using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using EmployeeAdjustmentConnectionSystem.COM.Entity.Session;
using EmployeeAdjustmentConnectionSystem.COM.Models;
using EmployeeAdjustmentConnectionSystem.COM.Util.Convert;
using EmployeeAdjustmentConnectionSystem.COM.Util.Database;
using EmployeeAdjustmentConnectionSystem.COM.Util.Encrypt;

namespace EmployeeAdjustmentConnectionSystem.BL.ChangePassword {
    /// <summary>
    /// パスワード変更ビジネスロジック
    /// </summary>
    public class ChangePasswordBL {
        #region メンバ変数
        /// <summary>
        /// ロガー
        /// </summary>
        private static NLog.Logger nlog = NLog.LogManager.GetCurrentClassLogger();

        #endregion

        /// <summary>
        /// パスワード変更
        /// </summary>
        /// <param name="model">変更用モデル</param>
        /// <param name="lu">ログインユーザー</param>
        /// <returns>結果</returns>
        public bool ChangePassword(ChangePasswordViewModelSd model, LoginUser lu) {
            try {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");

                using(var scope = new TransactionScope()) {
                    using(DbManager dm = new DbManager()) {
                        //詳細
                        var sql = "update TEM900LoginPassword"
                            + " set "
                            + " Password = @NewPassword"
                            // 2017-06-16 iwai-tamura upd str パスワード有効期限追加
                            + " ,PS更新年月日 = @NowDate"
                            // 2017-06-16 iwai-tamura upd end パスワード有効期限追加
                            + " where 社員番号 = @Id"
                            + "   and Password = @OldPassword";
                        //SQL文の型を指定
                        var cmd = dm.CreateCommand(sql);
                        DbHelper.AddDbParameter(cmd, "@NewPassword", DbType.String);
                        // 2017-06-16 iwai-tamura upd str パスワード有効期限追加
                        DbHelper.AddDbParameter(cmd, "@NowDate", DbType.String);
                        // 2017-06-16 iwai-tamura upd end パスワード有効期限追加
                        DbHelper.AddDbParameter(cmd, "@Id", DbType.String);
                        DbHelper.AddDbParameter(cmd, "@OldPassword", DbType.String);
                        //パラメータ設定
                        var parameters = cmd.Parameters.Cast<IDbDataParameter>().ToArray<IDbDataParameter>();

                        // 2017-06-16 iwai-tamura upd str パスワード有効期限追加
                        parameters[0].Value = DataConv.IfNull(EncryptUtil.EncryptString(model.NewPassword));
                        parameters[1].Value = DateTime.Today.ToString("yyyyMMdd");
                        parameters[2].Value = DataConv.IfNull(lu.UserCode);
                        parameters[3].Value = DataConv.IfNull(EncryptUtil.EncryptString(model.OldPassword));
                        //parameters[0].Value = DataConv.IfNull(EncryptUtil.EncryptString(model.NewPassword));
                        //parameters[1].Value = DataConv.IfNull(lu.UserCode);
                        //parameters[2].Value = DataConv.IfNull(EncryptUtil.EncryptString(model.OldPassword));
                        // 2017-06-16 iwai-tamura upd end パスワード有効期限追加
                        if(cmd.ExecuteNonQuery() <= 0) {
                            throw new Exception("パスワード更新エラー");
                        }
                    }
                    scope.Complete();
                }
                return true;
            } catch(Exception ex) {
                // エラー
                nlog.Error(System.Reflection.MethodBase.GetCurrentMethod().Name + " error " + ex.ToString());
                return false;
            } finally {
                //終了
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " end");
            }
        }
    }
}
