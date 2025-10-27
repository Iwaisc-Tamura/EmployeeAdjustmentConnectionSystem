/*エンターキー無効化*/
$(document).keypress(function (e) {
    //// エンターキーだったら無効にする
    //if (e.key === 'Enter') {
    //    return false;
    //}
});
/* Enterキーによる誤送信防止（ただしtextareaは除外） */
$(document).on('keydown', 'input, select', function (e) {
    if (e.key === 'Enter') {
        e.preventDefault(); // 送信防止
        return false;
    }
});
$(function () {
    //datepicker設定
    $.datepicker.setDefaults($.datepicker.regional['ja']);
    $('.ymd-control').datepicker();

    //入力モード制御
    switch ($('#Head_InputMode').val()) {
        //本人入力,管理入力
        case 'SelfInput':
        case 'adminInput':
            break;

        //本人確定
        case 'SelfConfim':
        case 'adminConfim':
            $(".form-control").attr('disabled', true);
            $(".input-control").attr('disabled', true);
            $(".btn-calc").attr('disabled', true);
            $("button.input-control").remove();
            $(".btn-clear").attr('disabled', true);
            $(".btn-get").attr('disabled', true);
            break;

        default:
            $(".form-control").attr('disabled', true);
            $(".input-control").attr('disabled', true);
            $(".btn-calc").attr('disabled', true);
            $("button.input-control").remove();
            $(".btn-clear").attr('disabled', true);
            $(".btn-get").attr('disabled', true);
            break;
    }
});

//戻るボタン時の必須チェック回避
document.addEventListener('DOMContentLoaded', function () {
    const backButton = document.getElementById('backbutton');
    backButton.addEventListener('click', function () {
        const requiredInputs = document.querySelectorAll('[required]');
        requiredInputs.forEach(input => {
            input.removeAttribute('required');
        });
    });
});

document.addEventListener('click', function (e) {
    console.log('clicked:', e.target, 'id=', e.target.id, 'class=', e.target.className);
}, true);


//日付削除ボタン
$('button.input-control').click(function () {
    $('#' + this.value).val('');
});


//保存直前にdisabled解除(値が送られないため)
function removeDisabled() {
    $('input').each(function (i, elem) {
        elem.disabled = false;
    });
    $('select').each(function (i, elem) {
        elem.disabled = false;
    });
}

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

    var convertedValue = zenkana2Hankana(hira2Kana($(this).val()));
    $(this).val(convertedValue);

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
    //入力項目チェック
    aryCheckInput = ["Head_DepartmentNo"
                    , "Head_Name1"
                    , "Head_Name2"
                    , "Head_Kana1"
                    , "Head_Kana2"
    ];
    $.each(aryCheckInput, function (index, value) {
        if (document.getElementById(value).value == "") {
            message = '必須項目が入力されていません。<br/>確認してください。';
        }

    })

    ///住宅借入金等特別控除申告書チェック
    //入力項目チェック
    aryCheckInput = ["Head_PaymentAmount"
        , "Head_SocialInsuranceAmount"
        , "Head_WithholdingTaxAmount"
        , "Head_CompanyName"
        , "Head_CompanyAddress"
        , "Head_RetirementDateYear"
        , "Head_RetirementDateMonth"
        , "Head_RetirementDateDay"
    ];
    $.each(aryCheckInput, function (index, value) {
        if (document.getElementById(value).value == "") {
            message = '必須項目が入力されていません。<br/>確認してください。';
        }

    })

    //画面のエラー項目全チェック
    $(".check-comment").each(function (i, e) {
        if ($(e).text().length > 1) {
            message = '入力に誤りがあります。<br/>確認してください。';
        }
    });

    return message
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
    
    //ボタンクリック
    var isAdminMode = $('#Head_AdminMode').val().toLowerCase() === 'true';
    if (isAdminMode) {
        if ($('#Head_DecisionType').val() <= '5') {
            showMessageEx('確定確認', '確定しますか？', 'savebutton', true);
        } else {
            showMessageEx('修正確認', '修正しますか？ <br><br> ※既に連携済みデータの為、連携先システムの修正も同様に行ってください。', 'savebutton', true);
        }
    } else {
        showMessageEx('提出確認', '提出しますか？', 'savebutton', true);
    }
});

/*
 * 承認キャンセルボタンクリック時
 */
$('#dmySignCancel').click(function () {
    //ボタンクリック
    var isAdminMode = $('#Head_AdminMode').val().toLowerCase() === 'true';
    if (isAdminMode) {
        showMessageEx('取消確認', '確定状態を取消しますか？', 'signcancel', true);
    } else {
        showMessageEx('取消確認', '提出状態を取消しますか？', 'signcancel', true);
    }
});

/*
 * データ削除ボタンクリック時
 */
$('#dmyDataDelete').click(function () {
    //ボタンクリック
    var isAdminMode = $('#Head_AdminMode').val().toLowerCase() === 'true';
    if (isAdminMode) {
        showMessageEx('取消確認', 'データを削除しますか？', 'datadelete', true);
    } else {
        showMessageEx('取消確認', 'データを削除しますか？', 'datadelete', true);
    }
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
