using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Web;

namespace EmployeeAdjustmentConnectionSystem.COM.Models {
    /// <summary>
    /// TOP用モデル
    /// </summary>
    public class TopViewModel {
        /// <summary>
        /// お知らせ内容
        /// </summary>
        public string Announcement { get; set; }
        //2017-08-31 iwai-tamura upd-str ------
        public int ObjectivesWaitingApprover { get; set; }
        public int SkillWaitingApprover { get; set; }
        //2017-08-31 iwai-tamura upd-end ------

        //2025-11-18 iwai-tamura add-str ------
		/// <summary>
		/// 扶養控除申告書管理者確定区分
		/// </summary>
		public string HuyouDecisionType { get; set; }

        /// <summary>
        /// 扶養控除申告書添付ファイルパス
        /// </summary>
        public string HuyouAttachmentFilePath { get; set; }

        /// <summary>
        /// 扶養控除申告書添付アップロードファイル
        /// </summary>
        public HttpPostedFileWrapper HuyouAttachmentUploadFile { get; set; }

        /// <summary>
        /// 過去分出力処理用　年度
        /// </summary>
        public int? HistoryYear { get; set; }

        /// <summary>
        /// 過去分出力処理用　対象帳票
        /// </summary>
        public string HistoryPrintType { get; set; }
        //2025-11-18 iwai-tamura add-end ------
    }
}
