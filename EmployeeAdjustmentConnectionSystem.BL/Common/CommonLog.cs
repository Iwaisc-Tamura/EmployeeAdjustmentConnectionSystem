using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using EmployeeAdjustmentConnectionSystem.COM.Util.Database;
using EmployeeAdjustmentConnectionSystem.COM.Entity.Session;
using System.Web.Mvc;
using System.Data;
using System.Runtime.CompilerServices;

namespace EmployeeAdjustmentConnectionSystem.Log.Common {
    /// <summary>
    /// 全体共通LOG取得ロジック
    /// </summary>
    public class CommonLog {
        /// <summary>
        /// 共通ログモジュール
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool WriteOperationLog(string UserCode,string ProcComment,string Comment,[CallerFilePath] string filePath = "",[CallerMemberName] string callerMemberName = "") {
            //[引数]
            //UserCode:ログインID
            //ProcComment:処理内容
            //Comment:その他コメントを記載
            //filePath:オブジェクトファイル名(自動取得)
            //callerMemberName:メソッド名(自動取得)
            
            //クエリ生成
            string sql = "INSERT INTO "
                + "TEM990OperationLog  "
                + " ("
                + "  OperationDateTime"
                + "  ,UserID"
                + "  ,ObjectName"
                + "  ,ProcedureName"
                + "  ,ProcedureComment"
                + "  ,Comment"
                + " )"
                + " VALUES"
                + " ("
                + "  GETDATE()"
                + "  ,@id"
                + "  ,@ObjectName"
                + "  ,@ProcedureName"
                + "  ,@ProcedureComment"
                + "  ,@Comment"
                + " );";
                
            //クエリ実行
            using (DbManager dbm = new DbManager())
            using(IDbCommand cmd = dbm.CreateCommand(sql)){
                DbHelper.AddDbParameter(cmd, "@id", DbType.String);
                DbHelper.AddDbParameter(cmd, "@ObjectName", DbType.String);
                DbHelper.AddDbParameter(cmd, "@ProcedureName", DbType.String);
                DbHelper.AddDbParameter(cmd, "@ProcedureComment", DbType.String);
                DbHelper.AddDbParameter(cmd, "@Comment", DbType.String);

                //パラメータ設定
                var parameters = cmd.Parameters.Cast<IDbDataParameter>().ToArray<IDbDataParameter>();
                parameters[0].Value = UserCode;
                parameters[1].Value = System.IO.Path.GetFileName(filePath);
                parameters[2].Value = callerMemberName;
                parameters[3].Value = ProcComment;
                parameters[4].Value = Comment;
                cmd.ExecuteNonQuery();
            }
            return true;
        }
    }
}
