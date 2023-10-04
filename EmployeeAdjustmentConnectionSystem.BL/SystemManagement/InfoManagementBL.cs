using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmployeeAdjustmentConnectionSystem.COM.Models;
using EmployeeAdjustmentConnectionSystem.COM.Util.Database;
using System.Data;
using EmployeeAdjustmentConnectionSystem.COM.Util.Convert;
using System.Web.Mvc;   //2017-09-15 iwai-tamura add


namespace EmployeeAdjustmentConnectionSystem.BL.SystemManagement {
    /// <summary>
    /// お知らせビジネスロジック
    /// </summary>
    public class InfoManagementBL {

        #region メンバ変数
        /// <summary>
        /// ロガー
        /// </summary>
        private static NLog.Logger nlog = NLog.LogManager.GetCurrentClassLogger();

        #endregion

        /// <summary>
        /// お知らせ情報検索
        /// </summary>
        /// <param name="model">システム管理のお知らせ情報model</param>
        /// <returns>システム管理画面お知らせのモデル</returns>
        public SystemManagementModels SearchInfo(){
            try{
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");

                SystemManagementModels model = new SystemManagementModels();
                //クエリ生成と実行
                using(DbManager dbm = new DbManager())
                using (IDbCommand cmd = dbm.CreateCommand("select 項番,お知らせ内容 from SD_Mお知らせ;"))   
                using(IDataReader reader = cmd.ExecuteReader()){
                    while (reader.Read()) { 
                        model.Infomation = reader["お知らせ内容"].ToString();
                        model.Key = reader["項番"].ToString();
                    }
                }
                //2017-09-15 iwai-tamura upd-str ------
                using (DbManager dm = new DbManager())
                {
                    SelectListItem[] BrItems = GetBranchItem(dm);
                    model.BranchItems = BrItems;
                }
                //2017-09-15 iwai-tamura upd-end ------

                //結果返却
                return model;
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
        /// お知らせ内容の登録・更新
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public SystemManagementModels SaveInfo(SystemManagementModels model) {
            try {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");
                
                //クエリ生成
                string sql = "update "
                    + "	SD_Mお知らせ  "
                    + "set "
                    + "	お知らせ内容 = @val "
                    + "where  "
                    + "	項番 = @key "
                    + "if @@ROWCOUNT = 0  "
                    + "insert into SD_Mお知らせ "
                    + " (お知らせ内容) "
                    + " values "
                    + " (@val); ";
                
                //クエリ実行
                using (DbManager dbm = new DbManager())
                using(IDbCommand cmd = dbm.CreateCommand(sql)){
                    DbHelper.AddDbParameter(cmd, "@val", DbType.String);
                    DbHelper.AddDbParameter(cmd, "@key", DbType.Int32);
                    //パラメータ設定
                    var parameters = cmd.Parameters.Cast<IDbDataParameter>().ToArray<IDbDataParameter>();
                    parameters[0].Value = DataConv.IfNull(model.Infomation);
                    parameters[1].Value = DataConv.IntParse(model.Key, 0);
                    cmd.ExecuteNonQuery();
                }

                //最新のお知らせテーブルの内容を取得
                model = SearchInfo();
                
                //クエリ実行につかったmodelを返却
                return model;
            } catch (Exception ex) {
                // エラー
                nlog.Error(System.Reflection.MethodBase.GetCurrentMethod().Name + " error " + ex.ToString());
                throw;
            } finally {
                //終了
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " end");
            }
        }

        //2017-09-15 iwai-tamura upd-str ------
        /// <summary>
        /// 事業所取得
        /// </summary>
        /// <param name="dm"></param>
        /// <returns></returns>
        public SelectListItem[] GetBranchItem(DbManager dm)
        {
            try
            {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");

                //string OfficeNum = ((SkillDiscriminantSystem.COM.Entity.Session.LoginUser)
                //    (HttpContext.Current.Session["LoginUser"])).職能集計表検索対象.ToString();

                string sql = "";
                sql = "SELECT 事業所番号,事業所名称"
                    + "  FROM TM909事業所名Master"
                    + " WHERE 1 = 1"
                    //+ (OfficeNum != "K" ? "   AND 事業所番号 = '" + OfficeNum + "'" : "")
                    + " ORDER BY 事業所番号";

                DataTable dt = new DataTable();
                DataSet dataSet = new DataSet();
                using (IDbCommand cmd = dm.CreateCommand(sql))
                {
                    //クエリ実行
                    IDataAdapter da = dm.CreateSqlDataAdapter(cmd);
                    da.Fill(dataSet);
                }

                SelectListItem[] BranchList = new SelectListItem[dataSet.Tables[0].Rows.Count];

                for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
                {
                    DataRow dr = dataSet.Tables[0].Rows[i];
                    SelectListItem BranchItems = new SelectListItem();
                    BranchItems.Value = dr["事業所番号"].ToString();
                    BranchItems.Text = dr["事業所名称"].ToString();
                    BranchList[i] = BranchItems;
                }

                return BranchList;
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
        //2017-09-15 iwai-tamura upd-end ------

    }
}
