/*エンターキー無効化*/
$(document).keypress(function (e) {
    // エンターキーだったら無効にする
    if (e.key === 'Enter') {
        return false;
    }
});

/* ラジオチェック戻し */
$('input:radio').dblclick(function () {
    var nombre = $(this).attr('id');
    var checked = $(this).is(":checked");
    if (checked) {
        $("input[id=" + nombre + "]:radio").prop("checked", false);
    }
});

$(function () {
    //初回 入力制御
    $('#Head_SpouseDeduction_ResidentCheck').change()

    //選択項目初期セット
    var obj = $('#BasicDeduction_CalcType_' + $('#Head_BasicDeduction_EarningsType').val());
    obj.prop("checked", true);

    obj = $('#SpouseDeduction_EarningsType_' + $('#Head_SpouseDeduction_EarningsType').val());
    obj.prop("checked", true);

    obj = $('#AdjustmentDeduction_ConditionType_' + $('#Head_AdjustmentDeduction_ConditionType').val());
    obj.prop("checked", true);
    AdjustmentDeduction_ConditionControl();




    //datepicker設定
    $.datepicker.setDefaults($.datepicker.regional['ja']);
    $('.ymd-control').datepicker();

    //入力モード制御
    switch ($('#Head_InputMode').val()) {
        //本人入力,管理入力
        case 'SelfInput':
        case 'adminInput':
            //$(".form-control").attr('disabled', true);
            //$(".input-control").attr('disabled', true);
            //$("button.input-control").remove();
            break;

        //本人確定
        case 'SelfConfim':
        case 'adminConfim':
            $(".form-control").attr('disabled', true);
            $(".input-control").attr('disabled', true);
            $(".btn-calc").attr('disabled', true);
            $("button.input-control").remove();
            break;

        default:
            $(".form-control").attr('disabled', true);
            $(".input-control").attr('disabled', true);
            $(".btn-calc").attr('disabled', true);
            $("button.input-control").remove();

            break;
    }
});

//日付削除ボタン
$('button.input-control').click(function () {
    $('#' + this.value).val('');
});


//計算ボタン
//給与所得者の基礎控除
$('#btn-basicDeduction-calc').click(function () {
    calcBasicDeduction();
});
function calcBasicDeduction() {
    var strMessage = ""
    var bolReturn = false

    $("#Head_BasicDeduction_TotalEarnings").val(null);
    $("#Head_BasicDeduction_EarningsType").val(null);
    $("#Head_BasicDeduction_CalcType").val(null);
    $("#Head_BasicDeduction_DeductionAmount").val(null);
    $("#BasicDeduction_CalcType_1").prop("checked", false);
    $("#BasicDeduction_CalcType_2").prop("checked", false);
    $("#BasicDeduction_CalcType_3").prop("checked", false);
    $("#BasicDeduction_CalcType_4").prop("checked", false);
    $("#BasicDeduction_CalcType_5").prop("checked", false);
    $("#BasicDeduction_CalcType_6").prop("checked", false);


    //金額計算処理
    var intTotalAmount = null;
    var strEarningsType = "";
    var strCalcType = "";
    var intDeductionAmount = null;


    if ($("#Head_BasicDeduction_Income").val() == '') { $("#Head_BasicDeduction_Income").val(0) }
    if ($("#Head_BasicDeduction_OtherIncome").val() == '') { $("#Head_BasicDeduction_OtherIncome").val(0) }


    intTotalAmount = Number($("#Head_BasicDeduction_Income").val()) + Number($("#Head_BasicDeduction_OtherIncome").val());

    if (intTotalAmount <= 9000000) {
        strEarningsType = "1"
        strCalcType = "A"
        intDeductionAmount = "480000"
    } else if (intTotalAmount <= 9500000) {
        strEarningsType = "2"
        strCalcType = "B"
        intDeductionAmount = "480000"
    } else if (intTotalAmount <= 10000000) {
        strEarningsType = "3"
        strCalcType = "C"
        intDeductionAmount = "480000"
    } else if (intTotalAmount <= 24000000) {
        strEarningsType = "4"
        intDeductionAmount = "480000"
    } else if (intTotalAmount <= 24500000) {
        strEarningsType = "5"
        intDeductionAmount = "320000"
    } else if (intTotalAmount <= 25000000) {
        strEarningsType = "6"
        intDeductionAmount = "160000"
    } else {
    }

    $("#Head_BasicDeduction_TotalEarnings").val(intTotalAmount);
    $("#Head_BasicDeduction_EarningsType").val(strEarningsType);
    $("#Head_BasicDeduction_CalcType").val(strCalcType);
    $("#Head_BasicDeduction_DeductionAmount").val(intDeductionAmount);
    $("#BasicDeduction_CalcType_" + strEarningsType).prop("checked", true);

    bolReturn = true;
    return bolReturn;
}


//給与所得者の配偶者控除 非居住
$('#Head_SpouseDeduction_ResidentCheck').change(function () {
    if (document.getElementById("Head_SpouseDeduction_ResidentCheck").checked == true) {
        $("#Head_SpouseDeduction_Address").removeAttr("disabled");
    } else {
        $("#Head_SpouseDeduction_Address").attr('disabled', true);
        $("#Head_SpouseDeduction_Address").val(null);
    }
});

//給与所得者の配偶者控除 計算
$('#btn-spouseDeduction-calc').click(function () {
    calcSpouseDeduction();
});
function calcSpouseDeduction(varArt) {
    var strMessage = ""
    var bolReturn = false
    if (typeof varArt === 'undefined') varArt = true;

    $("#Head_SpouseDeduction_TotalEarnings").val(null);
    $("#Head_SpouseDeduction_EarningsType").val(null);
    $("#Head_SpouseDeduction_CalcType").val(null);
    $("#Head_SpouseDeduction_DeductionAmount").val(null);
    $("#Head_SpouseDeduction_SpecialDeductionAmount").val(null);
    $("#SpouseDeduction_EarningsType_1").prop("checked", false);
    $("#SpouseDeduction_EarningsType_2").prop("checked", false);
    $("#SpouseDeduction_EarningsType_3").prop("checked", false);
    $("#SpouseDeduction_EarningsType_4").prop("checked", false);

    //金額計算処理
    var intTotalAmount = null;
    var strEarningsType = "";
    var strCalcType = "";
    var intDeductionAmount = null;
    var intSpDeductionAmount = null;
    var strBasicDeductionEarningsType = $('#Head_BasicDeduction_EarningsType').val();
    var aryCalcTable = [
        [0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0],
        [0, 48, 38, 38, 36, 31, 26, 21, 16, 11, 6, 3],
        [0, 32, 26, 26, 24, 21, 18, 14, 11, 8, 4, 2],
        [0, 16, 13, 13, 12, 11, 9, 7, 6, 4, 2, 1],
    ]

    //基礎控除チェック
    if (strBasicDeductionEarningsType == "" || strBasicDeductionEarningsType >= "4") {
        strMessage = "基礎控除の計算がされていない。もしくは適用外なので計算できません。";
        if (varArt) { alert(strMessage) };
        return true
    }

    if ($("#Head_SpouseDeduction_Income").val() == '') { $("#Head_SpouseDeduction_Income").val(0) }
    if ($("#Head_SpouseDeduction_OtherIncome").val() == '') { $("#Head_SpouseDeduction_OtherIncome").val(0) }

    intTotalAmount = Number($("#Head_SpouseDeduction_Income").val()) + Number($("#Head_SpouseDeduction_OtherIncome").val());

    //年齢チェック
    var strYear = $('#Head_SpouseDeduction_BirthdayYear').val();
    var strMonth = $('#Head_SpouseDeduction_BirthdayMonth').val();
    var strDay = $('#Head_SpouseDeduction_BirthdayDay').val();
    if (strMonth.length < 2) {
        strMonth = "0" + strMonth;
    }
    if (strDay.length < 2) {
        strDay = "0" + strDay;
    }
    var strDate = strYear + strMonth + strDay;
    if (!checkDate('#Head_SpouseDeduction_Birthday', '0', '')) {
        strMessage = "配偶者の生年月日が不正です。確認してください。";
        alert(strMessage);

        return true;
    }
    if (strYear == "") {
        strMessage = "配偶者の生年月日が入力されていないので計算できません。確認してください。";
        if (varArt) { alert(strMessage) };
        return true;
    }

    var cDate = String(Number($('#Head_SheetYear').val()) - 69) + '01' + '01';


    if (intTotalAmount <= 480000) {
        if (strDate <= cDate) {
            //70歳以上
            strEarningsType = "1"
            strCalcType = "1"
        } else {
            //70歳未満
            strEarningsType = "2"
            strCalcType = "2"
        }
    } else if (intTotalAmount <= 950000) {
        strEarningsType = "3"
        strCalcType = "3"
    } else if (intTotalAmount <= 1000000) {
        strEarningsType = "4"
        strCalcType = "4"
    } else if (intTotalAmount <= 1050000) {
        strEarningsType = "4"
        strCalcType = "5"
    } else if (intTotalAmount <= 1100000) {
        strEarningsType = "4"
        strCalcType = "6"
    } else if (intTotalAmount <= 1150000) {
        strEarningsType = "4"
        strCalcType = "7"
    } else if (intTotalAmount <= 1200000) {
        strEarningsType = "4"
        strCalcType = "8"
    } else if (intTotalAmount <= 1250000) {
        strEarningsType = "4"
        strCalcType = "9"
    } else if (intTotalAmount <= 1300000) {
        strEarningsType = "4"
        strCalcType = "10"
    } else if (intTotalAmount <= 1330000) {
        strEarningsType = "4"
        strCalcType = "11"
    } else {
    }
    if (strEarningsType == "1" || strEarningsType == "2") {
        intDeductionAmount = aryCalcTable[strBasicDeductionEarningsType][strCalcType] * 10000;
    } else if (strEarningsType == "3" || strEarningsType == "4") {
        intSpDeductionAmount = aryCalcTable[strBasicDeductionEarningsType][strCalcType] * 10000;
    }
    $("#Head_SpouseDeduction_TotalEarnings").val(intTotalAmount);
    $("#Head_SpouseDeduction_EarningsType").val(strEarningsType);
    $("#Head_SpouseDeduction_CalcType").val(strCalcType);
    $("#Head_SpouseDeduction_DeductionAmount").val(intDeductionAmount);
    $("#Head_SpouseDeduction_SpecialDeductionAmount").val(intSpDeductionAmount);
    $("#SpouseDeduction_EarningsType_" + strEarningsType).prop("checked", true);
    bolReturn = true;
    return bolReturn;
}


//所得金額調整控除 要件選択制御
function ConditionTypeSelect(obj,type) {
    if (obj.checked == true) {
        $("#Head_AdjustmentDeduction_ConditionType").val(type);
        $("#AdjustmentDeduction_ConditionType_1").prop("checked", false);
        $("#AdjustmentDeduction_ConditionType_2").prop("checked", false);
        $("#AdjustmentDeduction_ConditionType_3").prop("checked", false);
        $("#AdjustmentDeduction_ConditionType_4").prop("checked", false);
        $("#AdjustmentDeduction_ConditionType_" + type).prop("checked", true);
    } else {
        $("#AdjustmentDeduction_ConditionType_1").prop("checked", false);
        $("#AdjustmentDeduction_ConditionType_2").prop("checked", false);
        $("#AdjustmentDeduction_ConditionType_3").prop("checked", false);
        $("#AdjustmentDeduction_ConditionType_4").prop("checked", false);
        $("#Head_AdjustmentDeduction_ConditionType").val(null);
    }

    //入力可能項目制御
    AdjustmentDeduction_ConditionControl();
}

//所得金額調整控除 要件別入力制御
function AdjustmentDeduction_ConditionControl() {
    //入力可能項目制御
    $(".AdjustmentDeduction_isRead1").attr('disabled', true);
    $(".AdjustmentDeduction_isRead2").attr('disabled', true);
    switch ($("#Head_AdjustmentDeduction_ConditionType").val()) {
        case "1":
            //★項目入力可能
            $(".AdjustmentDeduction_isRead2").removeAttr("disabled");
            $("#Head_AdjustmentDeduction_ReportType").prop("checked", true);

            //☆項目のリセット
            $(".AdjustmentDeduction_isRead1").each(function (i, e) {
                $(e).val(null);
                $(e).prop("checked", false);
            });
            break;

        case "2":
        case "3":
            //☆★項目入力可能
            $(".AdjustmentDeduction_isRead1").removeAttr("disabled");
            $(".AdjustmentDeduction_isRead2").removeAttr("disabled");
            $('#Head_AdjustmentDeduction_ResidentCheck').change();
            break;

        case "4":
            //☆項目入力可能
            $(".AdjustmentDeduction_isRead1").removeAttr("disabled");

            //★項目のリセット
            $(".AdjustmentDeduction_isRead2").each(function (i, e) {
                $(e).val(null);
                $(e).prop("checked", false);
            });
            $('#Head_AdjustmentDeduction_ResidentCheck').change();
            break;

        default:
            //☆項目のリセット
            $(".AdjustmentDeduction_isRead1").each(function (i, e) {
                $(e).val(null);
                $(e).prop("checked", false);
            });
            //★項目のリセット
            $(".AdjustmentDeduction_isRead2").each(function (i, e) {
                $(e).val(null);
                $(e).prop("checked", false);
            });
            break;
    }
}

//所得金額調整控除 本人同居制御
$('#Head_AdjustmentDeduction_ResidentCheck').change(function () {
    if (document.getElementById("Head_AdjustmentDeduction_ResidentCheck").checked == true) {
        $(".AdjustmentDeductionAddress_isRead").attr('disabled', true);
        $(".AdjustmentDeductionAddress_isRead").val(null);
    } else {
        $(".AdjustmentDeductionAddress_isRead").removeAttr("disabled");
    }
});



/*###### 入力チェック ######*/
/*所属番号*/
$('#Head_DepartmentNo').change(function () {
    var strVal = $('#Head_DepartmentNo').val()
    var strMessage = ""
    var bolReturn = false
    if (strVal.length != 5) {
        strMessage = "5文字で入力してください。";
    } else if ($.isNumeric(strVal) == false) {
        strMessage = "数値でで入力してください。";
    } else {
        bolReturn = true
    }
    $("#Head_DepartmentNo_mes").text(strMessage);
    return bolReturn;
});

/*チェック関数*/
/*日付*/
function checkDate(id, proc,cDate) {
    var strYear = $(id + 'Year').val();
    var strMonth = $(id + 'Month').val();
    var strDay = $(id + 'Day').val();
    var strMessage = "";
    var bolReturn = false;

    $(id + "_mes").text("");

    if (strYear == "" && strMonth == "" && strDay == "") return true;
    if (!strYear || !strMonth || !strDay) {
        strMessage = "年月日が正しく入力されていません。";
        $(id + "_mes").text(strMessage);
        return false;
    } else if (!String(strYear).match(/^[0-9]{4}$/) || !String(strMonth).match(/^[0-9]{1,2}$/) || !String(strDay).match(/^[0-9]{1,2}$/)) {
        strMessage = "年月日が正しく入力されていません。";
        $(id + "_mes").text(strMessage);
        return false;
    } else {
        var dateObj = new Date(strYear, strMonth - 1, strDay),
            dateObjStr = dateObj.getFullYear() + '' + (dateObj.getMonth() + 1) + '' + dateObj.getDate(),
            checkDateStr = strYear + '' + strMonth + '' + strDay;
        if (dateObjStr != checkDateStr) {
            strMessage = "年月日が正しく入力されていません。";
            $(id + "_mes").text(strMessage);
            return false;
        } else {
            bolReturn = true;
        }
    }

    //年齢チェック
    if (strMonth.length<2){
        strMonth = "0" + strMonth;
    }
    if (strDay.length < 2) {
        strDay = "0" + strDay;
    }
    switch (proc) {
        case "1":
            var strDate = strYear + strMonth + strDay;
            if (strDate > cDate) {
                strMessage = "基準日の年齢が16歳未満です。";
                $(id + "_mes").text(strMessage);
                return false;
            }
            break;
        case "2":
            var strDate = strYear + strMonth + strDay;
            if (strDate <= cDate) {
                strMessage = "基準日の年齢が16歳を超えてます。";
                $(id + "_mes").text(strMessage);
                return false;
            }
            break;
    }

    $(id + "_mes").text(strMessage);
    return bolReturn;
}

//半角カナチェック
$('.checkKana').change(function () {
    var strId = "#" + this.id.slice(0, -1)  //共通利用するため一旦変換(Kana_1,Kana_2⇒"Kana_)"
    var strMessage = "";
    var bolReturn = false;

    var reg = new RegExp(/^[ｦ-ﾟ]*$/);   //使用可能文字指定(半角カナのみ)

    //Kana_1とKana_2に半角カナ以外が入力されていないかチェック
    if (reg.test($(strId + "1").val()) && reg.test($(strId + "2").val())) {
        bolReturn = true;
    } else {
        strMessage = "半角カナ以外は入力できません。"
        bolReturn = false;
    }
    $(strId + "_mes").text(strMessage);
    return bolReturn;
});


//金額チェック
function checkMoney(id, maxMoney) {
    var strVal = $(id).val();
    var strMessage = "";
    var bolReturn = false;
    strVal = strVal.replace(/[^0-9]+/i, '');    //数値のみ入力可能
    if (strVal.length >0) strVal = Number(strVal);
    $(id).val(strVal);
    if (strVal == "") {
        bolReturn = true;
    } else if ($.isNumeric(strVal) == false) {
        strMessage = "数値でで入力してください。";
    } else if ((strVal) > maxMoney) {
        strMessage = maxMoney + "円以上は入力できません。";
    } else {
        bolReturn = true;
    }
    $(id.id + "_mes").text(strMessage);
    return bolReturn;
}


//一括チェック
function checkAll() {
    message = '';
    var aryCheckInput = "";
    var aryCheckSelect = "";

    ///本人情報チェック
    //チェック項目チェック
    if (document.getElementById("Head_MyNumberCheck").checked == false) {
        message = '必須項目が入力されていません。<br/>確認してください。';
    }
    //入力項目チェック
    aryCheckInput = ["Head_DepartmentNo"
                    , "Head_Name1"
                    , "Head_Name2"
                    , "Head_Kana1"
                    , "Head_Kana2"
                    , "Head_Address"
    ];
    $.each(aryCheckInput, function (index, value) {
        if (document.getElementById(value).value == "") {
            message = '必須項目が入力されていません。<br/>確認してください。';
        }

    })

    ////選択項目チェック
    //aryCheckSelect = ["Head.SpouseCheck"
    //                , "Head.SpouseCheck"
    //];
    //$.each(aryCheckSelect, function (index, value) {
    //    if ($("input[name='" + value + "']:checked").length === 0) {
    //        message = '必須項目が入力されていません。<br/>確認してください。';
    //    }
    //})

    //画面のエラー項目全チェック
    $(".check-comment").each(function (i, e) {
        if ($(e).text().length > 1) {
            message = '入力に誤りがあります。<br/>確認してください。';
        }
    });


    return message
}

//計算チェック
function checkAllCalc() {
    if (!calcBasicDeduction(false)) { return false }
    if (!calcSpouseDeduction(false)) { return false }
    return true;
}



/*
 * 途中保存ボタンクリック時
 */
$('#dmykeep').click(function () {
    message = '';

    //入力チェック
    message = checkAll();
    if (message != '') {
        showAlert('確認',message)
        return;
    }
    if (!checkAllCalc()) { return; }


  //ボタンクリック
  showMessageEx('途中保存確認', '保存しますか？', 'keepbutton', true);
});

/*
 * 承認保存ボタンクリック時
 */
$('#dmysave').click(function () {
    message = '';

    //入力チェック
    message = checkAll();
    if (message != '') {
        showAlert('確認', message)
        return;
    }
    if (!checkAllCalc()) { return; }

    //ボタンクリック
    showMessageEx('提出確認', '提出しますか？', 'savebutton', true);
});

/*
 * 承認キャンセルボタンクリック時
 */
$('#dmySignCancel').click(function () {
    //ボタンクリック
    showMessageEx('取消確認', '提出状態を取消しますか？', 'signcancel', true);
});




/*
 * ダイアログメッセージ表示(OkCancel)
 */
function showMessageEx(title, message, buttonid, isLoding) {

  // ダイアログのメッセージを設定
  $('#show_dialog').html(message);

  // ダイアログを作成
  $('#show_dialog').dialog({
    modal: true,
    title: title,
    buttons: {
      'OK': function () {
          $(this).dialog('close');
          $('#' + buttonid).trigger('click');
      },
      'キャンセル': function () {
        $(this).dialog('close');
        return;
      }
    },
    open: function () {						// キャンセルボタンにフォーカスをあてる
      $(this).siblings('.ui-dialog-buttonpane').find('button:eq(1)').focus();
    }
  });
}

/*
 * ダイアログメッセージ表示(PDF)
 */
function showPrintMessage(title, message, buttonid1, buttonid2) {

  // ダイアログのメッセージを設定
  $('#show_dialog').html(message);

  // ダイアログを作成
  $('#show_dialog').dialog({
    modal: true,
    title: title,
    buttons: {
      'OK': function () {
        $(this).dialog('close');
        $('#' + buttonid2).trigger('click');
        return;
      },
      'キャンセル': function () {
        $(this).dialog('close');
        $('#' + buttonid1).trigger('click');
        return;
      }
    },
    open: function () {						// キャンセルボタンにフォーカスをあてる
      $(this).siblings('.ui-dialog-buttonpane').find('button:eq(1)').focus();
    }
  });
}

/*
 * 承認ボタンクリック時
 */
$(function () {
  $('#dmy111, #dmy112, #dmy113, #dmy114, #dmy211, #dmy212, #dmy213, #dmy214, #dmy221, #dmy223, #dmy224').click(function () {

    var msg = '承認画面　承認しますか？';
    switch (this.id) {
      case 'dmy111':
      case 'dmy211':
        msg = '登録確認画面　登録しますか？';
        break;
      default:
        break;
    }

    if ($('#Head_IsRireki').val() === 'True') {
        // 2017-03-31 sbc-sagara upd str アラート表示形式統一
        //alert('履歴データの為、承認出来ません');
        showAlert('エラー', '履歴データの為、承認出来ません');
        // 2017-03-31 sbc-sagara upd end
      return;
    }

    //入力チェック(目標)
    if (this.id === 'dmy111' ||
        this.id === 'dmy112') {
        if (itemCheck() == false) {
            // 2017-03-31 sbc-sagara upd str アラート表示形式統一
            //alert('目標は最低限３つ入力してください');
            showAlert('エラー', '目標は最低限３つ入力してください');
            // 2017-03-31 sbc-sagara upd end
          return;
        }
    }

    //2016-05-23 iwai-tamura upd str  -----
    //ウェイトチェック(本人)
    if (this.id === 'dmy111' ||
        this.id === 'dmy112') {
        if (selfWeightCheck() == false) {
            // 2017-03-31 sbc-sagara upd str アラート表示形式統一
            //alert('ウェイトは合計１００%になるように入力してください');
            showAlert('エラー', 'ウェイトは合計１００%になるように入力してください');
            // 2017-03-31 sbc-sagara upd end
            return;
        }
    }
      //ウェイトチェック(上司)
    if (this.id === 'dmy112') {
        if (bossWeightCheck() == false) {
            // 2017-03-31 sbc-sagara upd str アラート表示形式統一
            //alert('ウェイトは合計１００%になるように入力してください');
            showAlert('エラー', 'ウェイトは合計１００%になるように入力してください');
            // 2017-03-31 sbc-sagara upd end
            return;
        }
    }
    //2016-05-23 iwai-tamura upd end  -----

    //目標面談
    if (this.id === 'dmy112') {
      if (objDateCheck() == false) {
          // 2017-03-31 sbc-sagara upd str アラート表示形式統一
          //alert('目標設定面談実施日に正しい日付を入力してください');
          showAlert('エラー', '目標設定面談実施日に正しい日付を入力してください');
          // 2017-03-31 sbc-sagara upd end
        return;
      }
    }
    //達成面談
    if (this.id === 'dmy212') {
      if (achvDateCheck() == false) {
          // 2017-03-31 sbc-sagara upd str アラート表示形式統一
          //alert('達成度評価面談実施日に正しい日付を入力してください');
          showAlert('エラー', '達成度評価面談実施日に正しい日付を入力してください');
          // 2017-03-31 sbc-sagara upd end
        return;
      }
    }
    //異動
    if (this.id === 'dmy211' ||
        this.id === 'dmy212') {
      if (trnsDateCheck() == false) {
          // 2017-03-31 sbc-sagara upd str アラート表示形式統一
          //alert('期中の人事異動に正しい日付を入力してください');
          showAlert('エラー', '期中の人事異動に正しい日付を入力してください');
          // 2017-03-31 sbc-sagara upd end
        return;
      }
    }

    //2017-04-11 iwai-tamura upd str  -----
    ////達成度・プロセスチェック(本人)
    //if (this.id === 'dmy211') {
    //    if (selfAchvCheck() == false) {
    //        showAlert('エラー', '入力されていない達成度があります。確認してください。');
    //        return;
    //    }
    //    if (selfProcessCheck() == false) {
    //        showAlert('エラー', '入力されていないプロセスがあります。確認してください。');
    //        return;
    //    }
    //}
    //  //達成度・プロセスチェック(上司)
    //if (this.id === 'dmy212') {
    //    if (bossAchvCheck() == false) {
    //        showAlert('エラー', '入力されていない達成度があります。確認してください。');
    //        return;
    //    }
    //    if (bossProcessCheck() == false) {
    //        showAlert('エラー', '入力されていないプロセスがあります。確認してください。');
    //        return;
    //    }
    //}
      //2017-04-11 iwai-tamura upd end  -----







    //2017-03-31 sbc-sagara add str
    //入力チェック(目標/達成水準、行数)
    if (!$('#Head_BusinessPlanning').prop('readonly') && itemlineCheck(jQuery.trim($('#Head_BusinessPlanning').val()), 48, 16) == false) {
        message = '会社経営計画';
        showSaveMessageAndAlert(message, msg, this.value);
        return;
    } else if (!$('#Head_DepartmentPolicy').prop('readonly') && itemlineCheck(jQuery.trim($('#Head_DepartmentPolicy').val()), 48, 16) == false) {
        message = '所属部門';
        showSaveMessageAndAlert(message, msg, this.value);
        return;
    }
    // 目標項目の数繰り返し
    for (i = 0; $('#ObjList_' + i + '__ObjItem').length == 1; i++) {
        if (!$('#ObjList_' + i + '__ObjItem').prop('readonly') && itemlineCheck(jQuery.trim($('#ObjList_' + i + '__ObjItem').val()), 20, 12) == false) {
            message = '区分' + (i + 1) + 'の目標項目';
            showSaveMessageAndAlert(message, msg, this.value);
            return;
        } else if (!$('#ObjList_' + i + '__AchvLevel').prop('readonly') && itemlineCheck(jQuery.trim($('#ObjList_' + i + '__AchvLevel').val()), 20, 12) == false) {
            message = '区分' + (i + 1) + 'の達成水準・期限';
            showSaveMessageAndAlert(message, msg, this.value);
            return;
        } else if (!$('#ObjList_' + i + '__ObjPolicys').prop('readonly') && itemlineCheck(jQuery.trim($('#ObjList_' + i + '__ObjPolicys').val()), 30, 12) == false) {
            message = '区分' + (i + 1) + 'の目標達成のための施策・手段';
            showSaveMessageAndAlert(message, msg, this.value);
            return;
        } else if (!$('#ObjList_' + i + '__BestMetrics').prop('readonly') && itemlineCheck(jQuery.trim($('#ObjList_' + i + '__BestMetrics').val()), 15, 5) == false) {
            message = '区分' + (i + 1) + 'の大幅達成';
            showSaveMessageAndAlert(message, msg, this.value);
            return;
        } else if (!$('#ObjList_' + i + '__BetterMetrics').prop('readonly') && itemlineCheck(jQuery.trim($('#ObjList_' + i + '__BetterMetrics').val()), 15, 5) == false) {
            message = '区分' + (i + 1) + 'の達成';
            showSaveMessageAndAlert(message, msg, this.value);
            return;
        } else if (!$('#ObjList_' + i + '__SelfComment').prop('readonly') && itemlineCheck(jQuery.trim($('#ObjList_' + i + '__SelfComment').val()), 18, 11) == false) {
            message = '区分' + (i + 1) + 'の本人評価コメント';
            showSaveMessageAndAlert(message, msg, this.value);
            return;
        } else if (!$('#ObjList_' + i + '__BossComment').prop('readonly') && itemlineCheck(jQuery.trim($('#ObjList_' + i + '__BossComment').val()), 18, 11) == false) {
            message = '区分' + (i + 1) + 'の上司評価コメント';
            showSaveMessageAndAlert(message, msg, this.value);
            return;
        }
    }
    //2017-03-31 sbc-sagara add end


    //ボタンクリック
    showMessage('確認', msg, this.value, true);
  });
});

/*
 * 承認キャンセルボタンクリック時
 */
$(function () {
  $('#dmyc111, #dmyc112, #dmyc113, #dmyc114, #dmyc211, #dmyc212, #dmyc213, #dmyc214, #dmyc221, #dmyc223, #dmyc224').click(function () {

    var msg = '承認画面　承認をキャンセルしますか？';
    switch (this.id) {
      case 'dmyc111':
      case 'dmyc211':
        msg = '登録確認画面　登録をキャンセルしますか？';
        break;
      default:
        break;
    }

    if ($('#Head_IsRireki').val() === 'True') {
        // 2017-03-31 sbc-sagara upd str アラート表示形式統一
        //alert('履歴データの為、承認キャンセル出来ません');
        showAlert('エラー', '履歴データの為、承認キャンセル出来ません');
        // 2017-03-31 sbc-sagara upd end
      return;
    }

    //ボタンクリック
    showMessage('確認', msg, this.value, true);
  });
});



/*
 * 目標入力チェック
 */
function itemCheck() {
  for (i = 0; i < 3;i++){
    if (jQuery.trim($('#ObjList_' + i + '__ObjItem').val()) === '') {
      return false;
    }

  }
  return true;
}

//目標面談日チェック
function objDateCheck() {
  var value = $('#Head_ObjYear').val();
  if (jQuery.trim(value) === '') {
    return false;
  }
  var ds = Date.parse(value);
  var min = Date.parse(parseInt($('#Head_JcalYear').val()) - 1 + '/4/1');
  var max = Date.parse(parseInt($('#Head_JcalYear').val()) + 1 + '/12/31');
  //var fds = new Date(ds);
  //var mind = new Date(min);
  //var maxd = new Date(max);
  if (!(min <= ds && ds <= max)) {
    return false;
  }
  return true;
}

//達成面談日チェック
function achvDateCheck() {
  var value = $('#Head_AchvYear').val();
  if (jQuery.trim(value) === '') {
    return false;
  }
  var ds = Date.parse(value);
  var min = Date.parse(parseInt($('#Head_JcalYear').val()) - 1 + '/4/1');
  var max = Date.parse(parseInt($('#Head_JcalYear').val()) + 1 + '/12/31');
  if (!(min <= ds && ds <= max)) {
    return false;
  }
  return true;
}

//異動日チェック
function trnsDateCheck() {
  var value = $('#Head_TransferYear').val();
  if (jQuery.trim(value) === '') {
    return true;
  }
  var ds = Date.parse(value);
  var min = Date.parse(parseInt($('#Head_JcalYear').val()) + '/4/1');
  var max = Date.parse(parseInt($('#Head_JcalYear').val()) + 1 + '/3/31');
  if (!(min <= ds && ds <= max)) {
    return false;
  }
  return true;
}


//2016-05-23 iwai-tamura upd str  -----
//ウェイトチェック
function selfWeightCheck() {
    var value = 0;
    for (i = 0; i < 5; i++) {
        value += Number(jQuery.trim($('#ObjList_' + i + '__SelfWeight').val()))
    }
    if (!(value === 100)){
        return false;
    }
    return true;
}

function bossWeightCheck() {
    var value = 0;
    for (i = 0; i < 5; i++) {
        value += Number(jQuery.trim($('#ObjList_' + i + '__BossWeight').val()))
    }
    if (!(value === 100)) {
        return false;
    }
    return true;
}
//2016-05-23 iwai-tamura upd end -----

//2017-04-11 iwai-tamura upd str  -----
//達成度・プロセスチェック
function selfAchvCheck() {
    for (i = 0; i < 5; i++) {
        if(Number(jQuery.trim($('#ObjList_' + i + '__SelfWeight').val())) > 0){
            if(jQuery.trim($('#ObjList_' + i + '__SelfAchv').val()) === ""){
                return false;
            }
        }
    }
    return true;
}

function bossAchvCheck() {
    for (i = 0; i < 5; i++) {
        if(Number(jQuery.trim($('#ObjList_' + i + '__BossWeight').val())) > 0){
            if(jQuery.trim($('#ObjList_' + i + '__BossAchv').val()) === ""){
                return false;
            }
        }
    }
    return true;
}
function selfProcessCheck() {
    for (i = 0; i < 5; i++) {
        if (Number(jQuery.trim($('#ObjList_' + i + '__SelfWeight').val())) > 0) {
            if (jQuery.trim($('#ObjList_' + i + '__SelfProcess').val()) === "") {
                return false;
            }
        }
    }
    return true;
}

function bossProcessCheck() {
    for (i = 0; i < 5; i++) {
        if (Number(jQuery.trim($('#ObjList_' + i + '__BossWeight').val())) > 0) {
            if (jQuery.trim($('#ObjList_' + i + '__BossProcess').val()) === "") {
                return false;
            }
        }
    }
    return true;
}
//2017-04-11 iwai-tamura upd end  -----
