
/*
 * ラジオチェック戻し
 */
$('input:radio').dblclick(function () {
  var nombre = $(this).attr('id');
  var checked = $(this).is(":checked");
  if (checked) {
    $('input[id=' + selectorEscape(nombre) + ']:radio').prop("checked", false);
  }
});

/*
 * 保存ボタンクリック時
 */
$('#dmysave').click(function () {
  if ($('#Head_IsRireki').val() === 'True') {
    // 2017-03-31 sbc-sagara upd str
    //alert('履歴データの為、保存出来ません');
    showAlert('エラー','履歴データの為、保存出来ません');
    // 2017-03-31 sbc-sagara upd end
    return;
  }

  // 2017-03-31 sbc-sagara add str 部下有無による必須入力チェック機能を追加

  // 2017-03-31 sbc-sagara add str 部下有無による必須入力チェック機能修正
  // 2018-03-20 iwai-tamura upd str ------
  // 部下の有無入力可能時にチェックを行う。
  // 2020-10-21 iwai-tamura upd str ------
  //if (check1 = $('.HasSubordinate').attr('checked') != undefined) {

  //    if ($('.HasSubordinate').length > 0 && $('.HasSubordinate:checked').length < 1) {
  //        showAlert('エラー', '部下の有無に対して有効なDATAが入力されていません｡');
  //        return;
  //    }

  //    if ($('.HasSubordinate:checked').val() == "Any") {
  //        if ($(".Seiseki_Required").length > 0) {
  //            if ($('.Seiseki_Required option:selected').text() == "") {
  //                showAlert('エラー', '部下の有無に対して有効なDATAが入力されていません｡');
  //                return;
  //            }
  //        }
  //        if ($(".Jyoui_Required").length > 0) {
  //            if ($('.Jyoui_Required option:selected').text() == "") {
  //                showAlert('エラー', '部下の有無に対して有効なDATAが入力されていません｡');
  //                return;
  //            }
  //        }
  //        if ($(".Hoyuu_Required").length > 0) {
  //            if ($('.Hoyuu_Required option:selected').text() == "") {
  //                showAlert('エラー', '部下の有無に対して有効なDATAが入力されていません｡');
  //                return;
  //            }
  //        }
  //    }

  //}
  // 2020-10-21 iwai-tamura upd end ------

  //if ($('.HasSubordinate').length > 0 && $('.HasSubordinate:checked').length < 1) {
  //    showAlert('エラー', '部下の有無に対して有効なDATAが入力されていません｡');
  //    return;
  //}
  //if ($('.HasSubordinate:checked').val() == "Any") {
  //    if ($(".Seiseki_Required").length > 0) {
  //        if ($('.Seiseki_Required option:selected').text() == "") {
  //            showAlert('エラー', '部下の有無に対して有効なDATAが入力されていません｡');
  //            return;
  //        }
  //    }
  //    if ($(".Jyoui_Required").length > 0) {
  //        if ($('.Jyoui_Required option:selected').text() == "") {
  //            showAlert('エラー', '部下の有無に対して有効なDATAが入力されていません｡');
  //            return;
  //        }
  //    }
  //    if ($(".Hoyuu_Required").length > 0) {
  //        if ($('.Hoyuu_Required option:selected').text() == "") {
  //            showAlert('エラー', '部下の有無に対して有効なDATAが入力されていません｡');
  //            return;
  //        }
  //    }
  //}
  // 2017-03-31 sbc-sagara add end 部下有無による必須入力チェック機能を追加
  // 2018-03-20 iwai-tamura upd end ------

  // 2017-03-31 sbc-sagara add str 文字数・行数チェック
  message = '';

  // 2018-03-20 iwai-tamura upd str ------
  if (!($('#Head_InputMode').val() == 'ReadOnly') && isExistStrDate(jQuery.trim($('#Head_ReassignmentDate').val())) != 0) {
      alert('異動日が不正です。確認してください。(入力例：2000/12/31)');
      return;
  }
  //2018-03-20 iwai-tamura upd end ------
  

  // 2018-03-20 iwai-tamura upd str ------
  // 出力領域を増やしたため行数を変更
  if (!$('#Reasons_0__PrimaryDecision').prop('readonly') && itemlineCheck(jQuery.trim($('#Reasons_0__PrimaryDecision').val()), 18, 10) == false) {
      message = '1次判定者の業績判定';
  } else if (!$('#Reasons_1__PrimaryDecision').prop('readonly') && itemlineCheck(jQuery.trim($('#Reasons_1__PrimaryDecision').val()), 18, 10) == false) {
      message = '1次判定者の情意判定';
  } else if (!$('#Reasons_2__PrimaryDecision').prop('readonly') && itemlineCheck(jQuery.trim($('#Reasons_2__PrimaryDecision').val()), 18, 10) == false) {
      message = '1次判定者の保有判定';
  } else if (!$('#Reasons_0__SecondaryDecision').prop('readonly') && itemlineCheck(jQuery.trim($('#Reasons_0__SecondaryDecision').val()), 18, 10) == false) {
      message = '2次判定者の業績判定';
  } else if (!$('#Reasons_1__SecondaryDecision').prop('readonly') && itemlineCheck(jQuery.trim($('#Reasons_1__SecondaryDecision').val()), 18, 10) == false) {
      message = '2次判定者の情意判定';
  } else if (!$('#Reasons_2__SecondaryDecision').prop('readonly') && itemlineCheck(jQuery.trim($('#Reasons_2__SecondaryDecision').val()), 18, 10) == false) {
      message = '2次判定者の保有判定';
  }
  //if (!$('#Reasons_0__PrimaryDecision').prop('readonly') && itemlineCheck(jQuery.trim($('#Reasons_0__PrimaryDecision').val()), 18, 8) == false) {
  //    message = '1次判定者の業績判定';
  //} else if (!$('#Reasons_1__PrimaryDecision').prop('readonly') && itemlineCheck(jQuery.trim($('#Reasons_1__PrimaryDecision').val()), 18, 8) == false) {
  //    message = '1次判定者の情意判定';
  //} else if (!$('#Reasons_2__PrimaryDecision').prop('readonly') && itemlineCheck(jQuery.trim($('#Reasons_2__PrimaryDecision').val()), 18, 8) == false) {
  //    message = '1次判定者の保有判定';
  //} else if (!$('#Reasons_0__SecondaryDecision').prop('readonly') && itemlineCheck(jQuery.trim($('#Reasons_0__SecondaryDecision').val()), 18, 8) == false) {
  //    message = '2次判定者の業績判定';
  //} else if (!$('#Reasons_1__SecondaryDecision').prop('readonly') && itemlineCheck(jQuery.trim($('#Reasons_1__SecondaryDecision').val()), 18, 8) == false) {
  //    message = '2次判定者の情意判定';
  //} else if (!$('#Reasons_2__SecondaryDecision').prop('readonly') && itemlineCheck(jQuery.trim($('#Reasons_2__SecondaryDecision').val()), 18, 8) == false) {
  //    message = '2次判定者の保有判定';
  //}
  // 2018-03-20 iwai-tamura upd end ------


  if (message != '') {
      showSaveMessageAndAlertEx(message);
      return;
  }
  // 2017-03-31 sbc-sagara end str 文字数・行数チェック

  //ボタンクリック
  //showMessage('保存確認', '保存しますか？', 'savebutton', true);
  showMessageEx('途中保存確認', '保存しますか？', 'savebutton', true);
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
        //2016-10-06 iwai-tamura upd str 途中保存のPDF出力をやめる -----
        $('#' + buttonid).trigger('click');
        //showPrintMessage('PDF出力確認', 'PDFを出力しますか？', buttonid, 'save2printbutton');
        //2016-10-06 iwai-tamura upd str 途中保存のPDF出力をやめる -----
      },
      'キャンセル': function () {
        $(this).dialog('close');
        return;
      }
    },
    open: function () { // キャンセルボタンにフォーカスをあてる
      $(this).siblings('.ui-dialog-buttonpane').find('button:eq(1)').focus();
    }
  });
}

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
    open: function () { // キャンセルボタンにフォーカスをあてる
      $(this).siblings('.ui-dialog-buttonpane').find('button:eq(1)').focus();
    }
  });
}

/*
 * 承認ボタンクリック時
 */
$(function () {
  $('#dmy11, #dmy12, #dmy21, #dmy22').click(function () {
    if ($('#Head_IsRireki').val() === 'True') {      
      // 2017-03-31 sbc-sagara upd str
      //alert('履歴データの為、承認出来ません');
      showAlert('エラー','履歴データの為、承認出来ません');
      // 2017-03-31 sbc-sagara upd end
      return;
    }
    //2016-04-14 iwai-tamura del str 目標達成度は入力不可とする為、必須入力チェックは不要。-----
    //達成度
    //if ($('#Head_AchvTotal').prop('type') ==='text' && $('#Head_AchvTotal').val() === ''){
    //  alert('達成度評価点が未入力です');
    //  return;
    //}
    //if (!$.isNumeric($('#Head_AchvTotal').val())) {
    //  alert('達成度評価点に数値以外が入力されています');
    //  return;
    //}
    //プロセス
    //if ($('#Head_ProcessTotal').prop('type') ==='text' && $('#Head_ProcessTotal').val() === ''){
    //  alert('プロセス評価点が未入力です');
    //  return;
    //}
    //if (!$.isNumeric($('#Head_ProcessTotal').val())) {
    //  alert('プロセス評価点に数値以外が入力されています');
    //  return;
    //}
    //2016-04-14 iwai-tamura del end 目標達成度は入力不可とする。-----

    //Head_PrevAchvTotal
    //Head_PrevProcessTotal

    // 2018-03-20 iwai-tamura upd str ------
    switch ($('#Head_CompetencyNo').val()) {
        case "30":
        case "33":
        case "34":
        // 2019-10-09 iwai-tamura upd str ------
        case "15":
        case "35":
        // 2019-10-09 iwai-tamura upd end ------
            if ($('.JudgmentResultRequest:checked').val() == "Any") {
                showAlert('エラー', '判定結果開示希望に対して有効なDATAが入力されていません｡');
                return;
            }
            if ($('.JudgmentResultRequest').length > 0 && $('.JudgmentResultRequest:checked').length < 1) {
                showAlert('エラー', '判定結果開示希望に対して有効なDATAが入力されていません｡');
                return;
            }
            break;
    }
    // 2018-03-20 iwai-tamura upd end ------

    // 2018-03-20 iwai-tamura upd str ------
    //異動日の日付チェック
    if (!($('#Head_InputMode').val() == 'ReadOnly') && isExistStrDate(jQuery.trim($('#Head_ReassignmentDate').val())) != 0) {
        alert('異動日が不正です。確認してください。(入力例：2000/12/31)');
        return;
    }
    //2018-03-20 iwai-tamura upd end ------

    // 2018-05-09 iwai-tamura upd str ------
    //判定項目入力チェック
    // 2018-09-20 iwai-tamura upd str ------

    // 2021-03-25 iwai-tamura upd str ------
    //if (this.id === 'dmy21' ||
    //    this.id === 'dmy22') {
    //    if (itemCheck() == false) {
    //        showAlert('エラー', '未入力の判定項目があります。確認してください。');
    //        return;
    //    }
    //}
    // 2021-03-25 iwai-tamura upd end ------

    // 2018-09-20 iwai-tamura upd end ------
    //if (this.id === 'dmy21' ||
    //    this.id === 'dmy22') {
    //    if (itemCheck() == false) {
    //        showAlert('エラー', '未入力の判定項目があります。確認してください。');
    //      return;
    //    }
    //}
    // 2018-05-09 iwai-tamura upd end ------

    //2018-03-20 iwai-tamura add str ------
    if (!($('#Head_InputMode').val() == 'ReadOnly') && isExistStrDate(jQuery.trim($('#Head_ReassignmentDate').val())) != 0) {
        alert('異動日が不正です。確認してください。(入力例：2000/12/31)');
        return;
    }
    //2018-03-20 iwai-tamura add end ------

    // 2020-10-12 iwai-tamura upd str ------
    //判定要素毎にチェックするので使わない
    //if (this.id === 'dmy11') {
    //    if (ViewpointCheck() == false) {
    //        showAlert('エラー', '着眼点項目に未入力の箇所があります。確認してください。');
    //        return;
    //    }
    //}
    // 2020-10-12 iwai-tamura upd end ------

    //ボタンクリック
    showMessage('承認確認', '承認画面 承認しますか？', this.value, true);
  });
});

/*
 * 承認キャンセルボタンクリック時
 */
$(function () {
  $('#dmyc11, #dmyc12, #dmyc21, #dmyc22').click(function () {

    var msg = '承認画面　承認をキャンセルしますか？';
    //switch (this.id) {
    //  case 'dmy111':
    //  case 'dmy211':
    //    msg = '登録確認画面　登録をキャンセルしますか？';
    //    break;
    //  default:
    //    break;
    //}

    if ($('#Head_IsRireki').val() === 'True') {            
      // 2017-03-31 sbc-sagara upd str
      //alert('履歴データの為、承認キャンセル出来ません');
      showAlert('エラー','履歴データの為、承認キャンセル出来ません');
      // 2017-03-31 sbc-sagara upd end
      return;
    }

    //ボタンクリック
    showMessage('確認', msg, this.value, true);
  });
});


// 2018-05-09 iwai-tamura upd str
/*
 * 判定項目入力チェック
 */
function itemCheck() {
    // 2018-09-20 iwai-tamura upd str ------
    //部門調整・支社調整時は保有判定以外は必須とする
    for (i = 0; i < 11; i++) {
    //for (i = 0; i < 30; i++) {
    // 2018-09-20 iwai-tamura upd end ------
        if ($('#Items_' + i + '__DepartmentAdjustment').length > 0) {
            if ((!$('#Items_' + i + '__DepartmentAdjustment').prop('readonly')) && (jQuery.trim($('#Items_' + i + '__DepartmentAdjustment').val()) === '')) {
                return false;
            }
        }
        if ($('#Items_' + i + '__BranchAdjustment').length > 0) {
            if ((!$('#Items_' + i + '__BranchAdjustment').prop('readonly')) && (jQuery.trim($('#Items_' + i + '__BranchAdjustment').val()) === '')) {
                return false;
            }
        }
    }
    return true;
}
// 2018-05-09 iwai-tamura upd end

// 2020-10-12 iwai-tamura upd str

/*
 * モーダルウィンドウ内判定同期
 */

/*
 * モーダルウィンドウとメインウィンドウの評価を同期する
 */
function itemSync() {
    for (i = 0; i < 50; i++) {
        if ($('#Items_' + i + '__PrimaryDecision').length > 0) {
            $('#Items_' + i + '__PrimaryDecision' + '_Sub').val($('#Items_' + i + '__PrimaryDecision').val());
        }
    } 
    return true;
};

function ViewpointCheck() {
    for (i = 0; i < 99; i++) {
        if ($('#Viewpoint_' + i + '__PrimaryDecision').length > 0) {
            if ($('#Viewpoint_' + i + '__PrimaryDecision').is(':checked') == false) {
                return false;
            }
        }
    }
    return true;
};

function modalViewpointCheck(checkClassName) {
    var r = true;
    if ($('.' + checkClassName + '_modalItem').val() != "") {
        $('.' + checkClassName + '_ChkSub').each(function (i, o) {
            if ($(o).length > 0) {
                if ($(o).is(':checked') == false) {
                    r = false;
                    return false;
                }
            }
        })
    }
    return r;
};


function viewpointAllcheck(mainValu, checkClassName) {
    //全選択チェック時 true・・・全チェック、false・・・未チェック
    var isCheck = $(mainValu).is(':checked') ? true : false;
    //置き換え
    $('.' + checkClassName).prop('checked', isCheck);
}

//モーダルウィンドウを出現させるクリックイベント
$("#modal-open").click(function () {

    //キーボード操作などにより、オーバーレイが多重起動するのを防止する
    $(this).blur();	//ボタンからフォーカスを外す
    if ($("#modal-overlay")[0]) return false;		//新しくモーダルウィンドウを起動しない (防止策1)
    //if($("#modal-overlay")[0]) $("#modal-overlay").remove() ;		//現在のモーダルウィンドウを削除して新しく起動する (防止策2)

    //オーバーレイを出現させる
    $("body").append('<div id="modal-overlay"></div>');
    //$("#modal-overlay").fadeIn("slow");
    var btnIndex = $(this).index(); // 何番目のモーダルボタンかを取得
    $("#modal-overlay").eq(btnIndex).fadeIn(); // クリックしたモーダルボタンと同じ番目のモーダルを表示する
    //コンテンツをセンタリングする
    centeringModalSyncer();

    //コンテンツをフェードインする
    $("#modal-content").eq(btnIndex).fadeIn("slow");

    //[#modal-overlay]、または[#modal-close]をクリックしたら…
    $("#modal-overlay,#modal-close").unbind().click(function () {

        //[#modal-content]と[#modal-overlay]をフェードアウトした後に…
        $("#modal-content,#modal-overlay").fadeOut("slow", function () {
            itemSync();
            //[#modal-overlay]を削除する
            $('#modal-overlay').remove();

        });

    });

});

//リサイズされたら、センタリングをする関数[centeringModalSyncer()]を実行する
$(window).resize(centeringModalSyncer);

function viewModal(itemName) {

    //キーボード操作などにより、オーバーレイが多重起動するのを防止する
    $(this).blur();	//ボタンからフォーカスを外す
    if ($("#modal-overlay")[0]) return false;		//新しくモーダルウィンドウを起動しない (防止策1)
    //if($("#modal-overlay")[0]) $("#modal-overlay").remove() ;		//現在のモーダルウィンドウを削除して新しく起動する (防止策2)

    //オーバーレイを出現させる
    $("body").append('<div id="modal-overlay"></div>');
    $("#modal-overlay").fadeIn("slow");

    //コンテンツをセンタリングする
    centeringModalSyncer();

    //コンテンツをフェードインする
    //$("#modal-content").fadeIn("slow");
    $("#"+itemName+"_modal").fadeIn("slow");
    //var btnIndex = $(this).index();
    //[#modal-overlay]、または[#modal-close]をクリックしたら…
    $("#modal-overlay,#modal-close").unbind().click(function () {
        if (modalViewpointCheck(itemName)) {
            //[#modal-content]と[#modal-overlay]をフェードアウトした後に…
            $("#" + itemName + "_modal" + ",#modal-overlay").fadeOut("slow", function () {
                itemSync();
                //[#modal-overlay]を削除する
                $('#modal-overlay').remove();

            });
        } else {
            //判定項目に値を入れてある場合、着眼点全てにチェックが入ってないとエラーとする。
            showAlert('エラー', '着眼点項目に未入力の箇所があります。確認してください。');
        }

    });

};


//センタリングを実行する関数
function centeringModalSyncer() {

    //画面(ウィンドウ)の幅、高さを取得
    var w = $(window).width();
    var h = $(window).height();

    // コンテンツ(#modal-content)の幅、高さを取得
    // jQueryのバージョンによっては、引数[{margin:true}]を指定した時、不具合を起こします。
    //		var cw = $( "#modal-content" ).outerWidth( {margin:true} );
    //		var ch = $( "#modal-content" ).outerHeight( {margin:true} );
    var cw = $(".modal-content").outerWidth();
    var ch = $(".modal-content").outerHeight();

    //センタリングを実行する
    $(".modal-content").css({ "left": ((w - cw) / 2) + "px", "top": ((h - ch) / 2) + "px" });

}

// 2020-10-12 iwai-tamura upd end



