using System;
using System.Reflection;
using EmployeeAdjustmentConnectionSystem.COM.Enum;

namespace EmployeeAdjustmentConnectionSystem.COM.Entity.Session {

    /// <summary>
    /// セッション格納用のユーザー情報
    /// </summary>
    public class LoginUser {
        /// <summary>
        /// DB区分
        /// </summary>
        public string DbArea { get; set; }
        /// <summary>
        /// 社員コード
        /// </summary>
        public string UserCode { get; set; }
        /// <summary>
        /// 氏名
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// アクセス権限
        /// </summary>
        public Permissions Permission { get; set; }
        /// <summary>
        /// 資格NO
        /// </summary>
        public string CompetencyNo { get; set; }
        /// <summary>
        /// 所属番号
        /// </summary>
        public string DepartmentNo { get; set; }
        /// <summary>
        /// 所属名称
        /// </summary>
        public string DepartmentName { get; set; }
        /// <summary>
        /// 役職番号
        /// </summary>
        public string PostNo { get; set; }
        /// <summary>
        /// 管理アクセス権有無
        /// </summary>
        public bool IsRoot { get; set; }
        /// <summary>
        /// システム管理者判断
        /// </summary>
        public bool IsRootUser { get; set; }

        //2023-11-20 iwai-tamura add str -----
        /// <summary>
        /// 管理区分
        /// </summary>
        public string IsAdminNo { get; set; }

        /// <summary>
        /// システム管理年度
        /// </summary>
        public int IsYear { get; set; }
        //2023-11-20 iwai-tamura add end -----

        //2023-12-15 iwai-tamura add str -----
        /// <summary>
        /// サーバステータス
        /// </summary>
        public string IsServerStatus { get; set; }
        //2023-12-15 iwai-tamura add end -----




        //2016-01-21 iwai-tamura add str -----
        /// <summary>
        /// 管理職判断
        /// </summary>
        public bool IsPost { get; set; }
        //2016-01-21 iwai-tamura add end -----        

        //2019-10-02 iwai-tamura add str -----
        /// <summary>
        /// 管理職判断(副長・課長代理を除外)
        /// </summary>
        public bool IsPost2 { get; set; }
        //2019-10-02 iwai-tamura add end -----        

        // 2017-03-31 sbc-sagara add str 支社管理機能の追加
        /// <summary>
        /// メインメニュー「システム管理」使用可能制御
        /// </summary>
        public string SYS管理MENU { get; set; }

        /// <summary>
        /// 初期データ作成処理使用可能制御
        /// </summary>
        public string SYS管理初期処理 { get; set; }

        // 2018-03-20 iwai-tamura add str データ引継ぎ処理機能の追加
        /// <summary>
        /// 異動によるデータ引継ぎ処理機能使用可能制御
        /// </summary>
        public string SYS管理異動DATA引継処理 { get; set; }

        /// <summary>
        /// 組織規程改正によるデータ引継ぎ処理機能使用可能制御
        /// </summary>
        public string SYS管理組編DATA引継処理 { get; set; }
        // 2018-03-20 iwai-tamura add end データ引継ぎ処理機能の追加
        
        /// <summary>
        /// 確定処理使用可能制御
        /// </summary>
        public string SYS管理確定処理 { get; set; }

        /// <summary>
        /// ユーザー追加処理使用可能制御
        /// </summary>
        public string SYS管理USER追加 { get; set; }

        /// <summary>
        /// パスワード確認処理使用可能制御
        /// </summary>
        public string SYS管理PASS確認 { get; set; }

        /// <summary>
        /// パスワードリセット機能使用可能制御
        /// </summary>
        public string SYS管理PASSRESET { get; set; }

        //2017-09-15 iwai-tamura add end -----        
        /// <summary>
        /// 決裁権限マスタ出力機能使用可能制御
        /// </summary>
        public string SYS管理決裁権限出力 { get; set; }
        /// <summary>
        /// 決裁権限マスタ取込機能使用可能制御
        /// </summary>
        public string SYS管理決裁権限取込 { get; set; }
        //2017-09-15 iwai-tamura add end -----        
        /// <summary>
        /// お知らせ機能使用可能制御
        /// </summary>
        public string SYS管理連絡事項 { get; set; }

        /// <summary>
        /// 特別検索対象範囲_目標
        /// </summary>
        public string 目標検索対象 { get; set; }

        /// <summary>
        /// データ出力機能使用可能制御_目標
        /// </summary>
        public string 目標DATA出力 { get; set; }

        /// <summary>
        /// 特別検索対象範囲_職能
        /// </summary>
        public string 職能検索対象 { get; set; }

        /// <summary>
        /// データ出力機能使用可能制御_職能
        /// </summary>
        public string 職能DATA出力 { get; set; }

        /// <summary>
        /// 一括入力機能使用可能制御
        /// </summary>
        public string 職能一括入力 { get; set; }

        /// <summary>
        /// メインメニュー「集計表出力」ボタン表示制御
        /// </summary>
        public string 職能集計表MENU { get; set; }

        /// <summary>
        /// 集計表の出力対象支社設定
        /// </summary>
        public string 職能集計表検索対象 { get; set; }

        /// <summary>
        /// メインメニュー「判定除外登録」ボタン表示制御
        /// </summary>
        public string 職能判定除外 { get; set; }

        /// <summary>
        /// 特別検索対象範囲_決裁権限
        /// </summary>
        public string 決裁権限検索対象 { get; set; }

        /// <summary>
        /// 決裁者範囲制限解除機能
        /// </summary>
        public string 決裁権限登録制限 { get; set; }
        // 2017-03-31 sbc-sagara add end 支社管理機能の追加

        // 2018-99-99 iwai-tamura add str 自己申告書関連の管理機能追加
        /// <summary>
        /// 特別検索対象範囲_自己申告書検索画面
        /// </summary>
        public string 自己申告書検索対象 { get; set; }
        /// <summary>
        /// データ出力機能使用可能制御_自己申告書
        /// </summary>
        public string 自己申告書DATA出力 { get; set; }
        /// <summary>
        /// 特別検索対象範囲_自己申告書決裁権限
        /// </summary>
        public string 自己申告書決裁権限検索対象 { get; set; }
        /// <summary>
        /// 自己申告書決裁者範囲制限解除機能
        /// </summary>
        public string 自己申告書決裁権限登録制限 { get; set; }        
        // 2018-99-99 iwai-tamura add end 自己申告書関連の管理機能追加

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public LoginUser() {
            //全フィールドに空文字セット リフレクション版
            Type type = this.GetType();
            PropertyInfo[] pInfos = type.GetProperties();
            object value;
            foreach (PropertyInfo pinfo in pInfos) {
                value = null;
                if (pinfo.PropertyType == typeof(string))
                    value = "";
                else if (pinfo.PropertyType == typeof(bool))
                    value = false;

                pinfo.SetValue(this, value, null);
            }
        }
    }
}
