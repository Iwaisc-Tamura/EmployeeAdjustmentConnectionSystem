using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeAdjustmentConnectionSystem.COM.Enum {
    /// <summary>
    /// 日付分解用
    /// </summary>
    public enum DateEnum {
        /// <summary>
        /// そのまま
        /// </summary>
        ALL = 0,
        /// <summary>
        /// 年のみ
        /// </summary>
        YEAR = 1,
        /// <summary>
        /// 月のみ
        /// </summary>
        MONTH = 2,
        /// <summary>
        /// 日のみ
        /// </summary>
        DAY = 3,
        /// <summary>
        /// 年月日
        /// </summary>
        YMD = 4
    }

    /// <summary>
    /// 異動 Enum
    /// </summary>
    public enum Move {
        /// <summary>
        /// 無
        /// </summary>
        None = 0,
        /// <summary>
        /// 転入
        /// </summary>
        In = 1,
        /// <summary>
        /// 転出
        /// </summary>
        Out = 2
    }
    /// <summary>
    /// 区分 Enum
    /// </summary>
    public enum Area {
        /// <summary>
        /// 無
        /// </summary>
        None = 0,
        /// <summary>
        /// 経営
        /// </summary>
        Executive = 1,
        /// <summary>
        /// 業務
        /// </summary>
        Work = 2
    }
    /// <summary>
    /// 部下の有無
    /// </summary>
    public enum Subordinate {
        /// <summary>
        /// 無
        /// </summary>
        None = 0,
        /// <summary>
        /// 有
        /// </summary>
        Any = 1
    }

    /// <summary>
    /// 健康状態
    /// </summary>
    public enum Health {
        /// <summary>
        /// 頑丈
        /// </summary>
        Strong = 0,
        /// <summary>
        /// 普通
        /// </summary>
        Nomal = 1,
        /// <summary>
        /// 少し虚弱
        /// </summary>
        Weak = 2,
        /// <summary>
        /// 虚弱
        /// </summary>
        Frail = 3
    }

    /// <summary>
    /// 管理メッセージID
    /// </summary>
    public enum ManageMessageId {
        /// <summary>
        /// パスワード変更成功
        /// </summary>
        ChangePasswordSuccess,
        /// <summary>
        /// エラー
        /// </summary>
        Error
    }

    /// <summary>
    /// システム権限
    /// </summary>
    public enum Permissions {
        /// <summary>
        /// 一般
        /// </summary>
        Public = 0,
        /// <summary>
        /// システム管理(兼本社)
        /// </summary>
        Root = 1,
        /// <summary>
        /// 東京
        /// </summary>
        Tokyo = 2,
        /// <summary>
        /// 関東
        /// </summary>
        Kanto = 3,
        /// <summary>
        /// 大阪
        /// </summary>
        Osaka = 7,
        /// <summary>
        /// 名古屋
        /// </summary>
        Nagoya = 8,
        /// <summary>
        /// 福岡
        /// </summary>
        Fukuoka = 9
    }

    /// <summary>
    /// 年調関連登録モード
    /// </summary>
    public enum ajustMode {
        /// <summary>
        /// 無し
        /// </summary>
        None = 0,
        /// <summary>
        /// 本人入力
        /// </summary>
        SelfInput = 11,
        /// <summary>
        /// 本人登録
        /// </summary>
        SelfRegist = 12,
        /// <summary>
        /// 本人確定
        /// </summary>
        SelfConfim = 19,
        /// <summary>
        /// 管理入力
        /// </summary>
        adminInput = 21,
        /// <summary>
        /// 管理登録
        /// </summary>
        adminRegist = 22,
        /// <summary>
        /// 管理確定
        /// </summary>
        adminConfim = 29,
        /// <summary>
        /// リードオンリー
        /// </summary>
        ReadOnly = 999
    }
    /// <summary>
    /// 目標管理モード
    /// </summary>
    public enum ObjMode {
        /// <summary>
        /// 無し
        /// </summary>
        None = 0,
        /// <summary>
        /// 設定 本人
        /// </summary>
        ObjSelfSign = 111,
        /// <summary>
        /// 設定 面談者
        /// </summary>
        ObjInterviewSign = 112,
        /// <summary>
        /// 設定 上位者
        /// </summary>
        ObjInterviewHighSign = 113,
        /// <summary>
        /// 設定 部長
        /// </summary>
        ObjManagerSign = 114,
        /// <summary>
        /// 達成 本人
        /// </summary>
        AchvSelfSign = 211,
        /// <summary>
        /// 達成 面談者
        /// </summary>
        AchvInterviewSign = 212,
        /// <summary>
        /// 達成 上位者
        /// </summary>
        AchvInterviewHighSign = 213,
        /// <summary>
        /// 達成 部長
        /// </summary>
        AchvManagerSign = 214,
        /// <summary>
        /// 達成 人事
        /// </summary>
        AchvHumanResourceSign = 221,
        /// <summary>
        /// 達成 その他(未使用)
        /// </summary>
        AchvEtcSign = 222,
        /// <summary>
        /// 達成 総務
        /// </summary>
        AchvGeneralAffairsSign = 223,
        /// <summary>
        /// 達成 支社長
        /// </summary>
        AchvExecutiveSign = 224,
        /// <summary>
        /// リードオンリー
        /// </summary>
        ReadOnly=999
    }

    /// <summary>
    /// 職能職務モード
    /// </summary>
    public enum SklMode {
        /// <summary>
        /// 無し
        /// </summary>
        None = 0,
        /// <summary>
        /// 一次判定
        /// </summary>
        PrimaryDecision = 11,
        /// <summary>
        /// 二次判定
        /// </summary>
        SecondaryDecision = 12,
        /// <summary>
        /// 部門調整
        /// </summary>
        DepartmentAdjustment = 21,
        /// <summary>
        /// 支社調整
        /// </summary>
        BranchAdjustment = 22,
        /// <summary>
        /// リードオンリー
        /// </summary>
        ReadOnly = 999
    }

    /// <summary>
    /// 自己申告書モード
    /// </summary>
    public enum SelfDeclareMode {
        /// <summary>
        /// 無し
        /// </summary>
        None = 0,
        /// <summary>
        /// A～C表 本人
        /// </summary>
        AtoCSelfSign = 11,
        /// <summary>
        /// A～C表 上司
        /// </summary>
        AtoCBossSign = 12,
        /// <summary>
        /// D表 本人
        /// </summary>
        DSelfSign = 21,
        /// <summary>
        /// D表 本社人事部長
        /// </summary>
        DBossSign = 22,
        /// <summary>
        /// リードオンリー
        /// </summary>
        ReadOnly=999
    }

    /// <summary>
    /// 自己申告状態区分モード
    /// </summary>
    public enum SelfDeclareStatusType {
        /// <summary>
        /// 一次入力
        /// </summary>
        PrimaryEdit = 0,
        /// <summary>
        /// 一次確定
        /// </summary>
        PrimaryConfirm = 1,
        /// <summary>
        /// 二次入力
        /// </summary>
        SecondaryEdit = 2,
        /// <summary>
        /// 二次確定
        /// </summary>
        SecondaryConfirm = 3
    }

    /// <summary>
    /// キャリアシートモード
    /// </summary>
    public enum CareerSheetMode {
        /// <summary>
        /// 無し
        /// </summary>
        None = 0,
        /// <summary>
        /// 本人
        /// </summary>
        CareerSelfSign = 10,
        /// <summary>
        /// 所属部署副長
        /// </summary>
        OwnDeputySign = 11,
        /// <summary>
        /// 所属部署課長
        /// </summary>
        OwnSectionSign = 12,
        /// <summary>
        /// 所属部署部長
        /// </summary>
        OwnDivisionSign = 13,
        /// <summary>
        /// 支社総務部副長
        /// </summary>
        BranchDeputySign = 21,
        /// <summary>
        /// 支社総務部課長
        /// </summary>
        BranchSectionSign = 22,
        /// <summary>
        /// 支社総務部部長
        /// </summary>
        BranchDivisionSign = 23,
        /// <summary>
        /// 本社人事部副長
        /// </summary>
        HeadDeputySign = 31,
        /// <summary>
        /// 本社人事部課長
        /// </summary>
        HeadSectionSign = 32,
        /// <summary>
        /// 本社人事部部長
        /// </summary>
        HeadDivisionSign = 33,
        /// <summary>
        /// リードオンリー
        /// </summary>
        ReadOnly=999
    }


}
