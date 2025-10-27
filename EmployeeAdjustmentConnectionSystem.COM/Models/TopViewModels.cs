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

        //2025-99-99 iwai-tamura add-str ------
        /// <summary>
        /// 扶養控除申告書添付ファイルアップロード用
        /// </summary>
        /// 
        public string HuyouAttachmentFilePath { get; set; }
        public HttpPostedFileWrapper HuyouAttachmentUploadFile { get; set; }

        //2025-99-99 iwai-tamura add-end ------
    }
}
