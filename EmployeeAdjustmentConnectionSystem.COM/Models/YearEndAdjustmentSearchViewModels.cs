using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using EmployeeAdjustmentConnectionSystem.COM.Models;

namespace EmployeeAdjustmentConnectionSystem.COM.Models {
    /// <summary>
    /// 年末調整照会モデル
    /// </summary>
    public class YearEndAdjustmentSearchViewModels {
        /// <summary>
        /// 検索条件
        /// </summary>
        public YearEndAdjustmentSearchModel Search { get; set; }
        /// <summary>
        /// 検索結果
        /// </summary>
        public List<YearEndAdjustmentSearchListModel> SearchResult { get; set; }
        /// <summary>
        /// ダウンロード設定値
        /// </summary>
        public SelfDeclareDownLoadModel Down { get; set; }

    }
    /// <summary>
    /// 検索条件
    /// </summary>
    public class YearEndAdjustmentSearchModel : SearchModel {
        /// <summary>
        /// 扶養控除ステータス
        /// </summary>
        public string HuyouDeclareStatus { get; set; }

        /// <summary>
        /// 保険料控除ステータス
        /// </summary>
        public string HokenDeclareStatus { get; set; }

        /// <summary>
        /// 基礎控除ステータス
        /// </summary>
        public string HaiguuDeclareStatus { get; set; }

    }
    /// <summary>
    /// 検索結果
    /// </summary>
    public class YearEndAdjustmentSearchListModel : SearchListModel {

        
        /// <summary>
        /// 扶養控除ステータス
        /// </summary>
        public string HuyouDeclareStatus { get; set; }

        /// <summary>
        /// 保険料控除ステータス
        /// </summary>
        public string HokenDeclareStatus { get; set; }

        /// <summary>
        /// 基礎控除ステータス
        /// </summary>
        public string HaiguuDeclareStatus { get; set; }


        /// <summary>
        /// 扶養控除ボタン表示
        /// </summary>
        public string HuyouDeclareButtonView { get; set; }

        /// <summary>
        /// 保険料控除ボタン表示
        /// </summary>
        public string HokenDeclareButtonView { get; set; }

        /// <summary>
        /// 基礎控除ボタン表示
        /// </summary>
        public string HaiguuDeclareButtonView { get; set; }

        /// <summary>
        /// 扶養控除ボタン表示フラグ
        /// </summary>
        public bool HuyouDeclareButtonViewFlg { get; set; }

        /// <summary>
        /// 保険料控除ボタン表示フラグ
        /// </summary>
        public bool HokenDeclareButtonViewFlg { get; set; }

        /// <summary>
        /// 基礎控除ボタン表示フラグ
        /// </summary>
        public bool HaiguuDeclareButtonViewFlg { get; set; }



        /// <summary>
        /// 自己申告書パターン
        /// </summary>
        public string SelfDecType { get; set; }

        /// <summary>
        /// 自己申告書Dパターン
        /// </summary>
        public string SelfDecDType { get; set; }

        /// <summary>
        /// 自己申告書パターン
        /// </summary>
        public string CarrierSheetType { get; set; }

        /// <summary>
        /// 職掌番号
        /// </summary>
        public string DutyNo { get; set; }
        /// <summary>
        /// 職掌名
        /// </summary>
        public string Duty { get; set; }

        /// <summary>
        /// 資格番号
        /// </summary>
        public string CompetencyNo { get; set; }
        /// <summary>
        /// 資格名
        /// </summary>
        public string Competency { get; set; }

        /// <summary>
        /// 自己申告書ボタン表示
        /// </summary>
        public string SelfDecAtoCButtonView { get; set; }
        /// <summary>
        /// 自己申告書Dボタン表示
        /// </summary>
        public string SelfDecDButtonView { get; set; }
        /// <summary>
        /// キャリアシートボタン表示
        /// </summary>
        public string CareerButtonView { get; set; }


        
        
        /// <summary>
        /// 目標設定承認
        /// </summary>
        public string ObjectivesAceept { get; set; }
        /// <summary>
        /// 達成度承認
        /// </summary>
        public string AattainmentAccept { get; set; }
        /// <summary>
        /// 達成度計
        /// </summary>
        public string AchvTotal { get; set; }
        /// <summary>
        /// プロセス計
        /// </summary>
        public string ProcessTotal { get; set; }
    }
    /// <summary>
    /// ダウンロード設定値
    /// </summary>
    public class YearEndAdjustmentDownLoadModel : DownLoadModel {
    }
}
