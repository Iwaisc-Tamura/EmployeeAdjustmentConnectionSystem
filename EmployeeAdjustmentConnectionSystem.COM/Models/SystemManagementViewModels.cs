using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.Web; // 2017-04-30 sbc-sagara add ファイルアップロード機能追加
using System.Web.Mvc; // 2017-04-30 sbc-sagara add CSV出力機能追加

namespace EmployeeAdjustmentConnectionSystem.COM.Models {

    /// <summary>
    /// システム管理用モデル
    /// </summary>
    public class SystemManagementModels {
        /// <summary>
        /// 一括作成対象
        /// </summary>
        //[Display(Name = "対象")]
        //[Required(ErrorMessage = "{0}は必須です。")]
        public string BulkType { get; set; }
        /// <summary>
        /// 一括作成年度
        /// </summary>
        public string BulkYear { get; set; }
        /// <summary>
        /// 一括作成期区分
        /// </summary>
        public string BulkDuration { get; set; }

        /// <summary>
        /// 個別作成処理対象
        /// </summary>
        public string DesignateType { get; set; }
        /// <summary>
        /// 個別作成年度
        /// </summary>
        public string DesignateYaer { get; set; }
        /// <summary>
        /// 個別作成期間区分
        /// </summary>
        public string DesignateDuration { get; set; }
        /// <summary>
        /// 個別作成社員番号
        /// </summary>
        public string DesignateEmployeeNo { get; set; }

        // 2018-03-20 iwai-tamura upd str ------
        /// <summary>
        /// 異動による個別引継ぎ 処理対象
        /// </summary>
        public string TakeOverMoveSingleType { get; set; }
        /// <summary>
        /// 異動による個別引継ぎ 年度
        /// </summary>
        public string TakeOverMoveSingleYaer { get; set; }
        /// <summary>
        /// 異動による個別引継ぎ 前任社員番号
        /// </summary>
        public string TakeOverMoveSingleBefEmployeeNo { get; set; }
        /// <summary>
        /// 異動による個別引継ぎ 前任所属番号
        /// </summary>
        public string TakeOverMoveSingleBefDepartment { get; set; }
        /// <summary>
        /// 異動による個別引継ぎ 後任社員番号
        /// </summary>
        public string TakeOverMoveSingleAftEmployeeNo { get; set; }
        /// <summary>
        /// 異動による個別引継ぎ 後任所属番号
        /// </summary>
        public string TakeOverMoveSingleAftDepartment { get; set; }

        /// <summary>
        /// 異動による一括引継ぎ 処理対象
        /// </summary>
        public string TakeOverMoveBulkType { get; set; }
        /// <summary>
        /// 異動による一括引継ぎ 年度
        /// </summary>
        public string TakeOverMoveBulkYaer { get; set; }
        /// <summary>
        /// 異動による一括引継ぎ アップロードファイル
        /// </summary>
        public HttpPostedFileWrapper TakeOverMoveBulkUploadFile { get; set; }

         /// <summary>
        /// 組編による個別引継ぎ 処理対象
        /// </summary>
        public string TakeOverAmendmentCompanyType { get; set; }
        /// <summary>
        /// 組編による個別引継ぎ 年度
        /// </summary>
        public string TakeOverAmendmentCompanyYaer { get; set; }
        /// <summary>
        /// 組編による個別引継ぎ 社員番号
        /// </summary>
        public string TakeOverAmendmentCompanyEmployeeNo { get; set; }
        /// <summary>
        /// 組編による個別引継ぎ 組編前所属番号
        /// </summary>
        public string TakeOverAmendmentCompanyBefDepartment { get; set; }
        /// <summary>
        /// 組編による個別引継ぎ 組編後所属番号
        /// </summary>
        public string TakeOverAmendmentCompanyAftDepartment { get; set; }       

        /// <summary>
        /// 組編による一括引継ぎ 処理対象
        /// </summary>
        public string TakeOverAmendmentCompanyBulkType { get; set; }
        /// <summary>
        /// 組編による一括引継ぎ 年度
        /// </summary>
        public string TakeOverAmendmentCompanyBulkYaer { get; set; }
        /// <summary>
        /// 組編による一括引継ぎ アップロードファイル
        /// </summary>
        public HttpPostedFileWrapper TakeOverAmendmentCompanyBulkUploadFile { get; set; }
        
        
        // 2018-03-20 iwai-tamura upd end ------


        /// <summary>
        /// 確定処理区分
        /// </summary>
        public string CommitArea { get; set; }
        /// <summary>
        /// 確定処理対象
        /// </summary>
        public string CommitType { get; set; }
        /// <summary>
        /// 確定処理年度
        /// </summary>
        public string CommitYear { get; set; }
        /// <summary>
        /// 確定処理期間区分
        /// </summary>
        public string CommitDuration { get; set; }
        /// <summary>
        /// 確定処理結果内容
        /// </summary>
        public string CommitResult { get; set; }

        // 2017-03-31 sbc-sagara add str アカウント追加用パラメータ
        /// <summary>
        /// 作成社員番号
        /// </summary>
        public string AddEmployeeNo { get; set; }

        /// <summary>
        /// 作成パスワード
        /// </summary>
        public string AddPassword { get; set; }
        // 2017-03-31 sbc-sagara add end

        // 2017-06-16 iwai-tamura add str アカウント参照追加機能
        /// <summary>
        /// 参照元社員番号
        /// </summary>
        public string ReferenceEmployeeNo { get;set;}
        // 2017-06-16 iwai-tamura add end アカウント参照追加機能
        
        /// <summary>
        /// リセット社員番号
        /// </summary>
        public string ResetEmployeeNo { get; set; }

        /// <summary>
        /// お知らせの内容
        /// </summary>
        public string Infomation { get; set; }
        /// <summary>
        /// 項番(主キー)
        /// </summary>
        public string Key { get; set; }

        // 2017-04-30 sbc-sagara str ファイルアップロード,Excel出力追加
        /// <summary>
        /// アップロード対象
        /// </summary>
        public string UploadTarget { get; set; }

        /// <summary>
        /// アップロード年度
        /// </summary>
        public string UploadYear { get; set; }

        ///// <summary>
        ///// アップロード期区分
        ///// </summary>
        //public string UploadDuration { get; set; }

        ///// <summary>
        ///// アップロード職能区分
        ///// </summary>
        //public string UploadDesignateType {  get; set; }

        /// <summary>
        /// アップロードファイル
        /// </summary>
        public HttpPostedFileWrapper UploadFile { get; set; }

        /// <summary>
        /// 出力対象
        /// </summary>
        public string OutputTarget { get; set; }

        /// <summary>
        /// 出力年度
        /// </summary>
        public string OutputYear { get; set; }

        // 2017-09-15 iwai-tamura add str-----
        /// <summary>
        /// 対象支社
        /// </summary>
        public string OutputBranch { get; set; }

        /// <summary>
        /// 支社ドロップダウンボックスアイテム
        /// </summary>
        public IList<SelectListItem> BranchItems { get; set; }

        // 2017-09-15 iwai-tamura add end -----

        ///// <summary>
        ///// 出力期区分
        ///// </summary>
        //public string OutputDuration { get; set; }

        ///// <summary>
        ///// 出力職能区分
        ///// </summary>
        //public string OutputDesignateType {  get; set; }
        // 2017-04-30 sbc-sagara end ファイルアップロード,Excel出力追加




        [CustomValidation(typeof(SystemManagementModels), "DisplayError")]
        public string ErrorMessage { get; set; }

        public static ValidationResult DisplayError(string value) {

            if (!string.IsNullOrEmpty(value)) return new ValidationResult(value);
            return ValidationResult.Success;
        }

    }

    // 2017-09-15 iwai-tamura add str -----
    //public class SystemManagementModelItems
    //{
    //    public ExclusionModel Search { get; set; }

    //    /// <summary>
    //    /// 支社ドロップダウンボックスアイテム
    //    /// </summary>
    //    public IList<SelectListItem> BranchItems { get; set; }
    //}
    // 2017-09-15 iwai-tamura add end -----

}
