
/*
 * datepicker設定
 */
$(function () {
  $.datepicker.setDefaults($.datepicker.regional['ja']);
  switch ($('#Head_InputMode').val()) {

    //2017-07-21 iwai-tamura upd-str ------
    //設定本人
    case 'ObjSelfSign':
        //期中の異動日
        $('#Head_TransferYear').datepicker();
        break;
    //2017-07-21 iwai-tamura upd-end ------

    //設定面談者
    case 'ObjInterviewSign':
      //設定実施日
      $('#Head_ObjYear').datepicker();
      //2017-07-21 iwai-tamura upd-str ------
      //期中の異動日
      $('#Head_TransferYear').datepicker();
      //2017-07-21 iwai-tamura upd-end ------
      break;

    //達成本人
    case 'AchvSelfSign':
      //人事異動、本人評価日
      $('#Head_TransferYear').datepicker();
      $('#Head_SelfEvalMonth').datepicker();
      break;
      //達成本人
    case 'AchvInterviewSign':
      //人事異動、達成実施日
      $('#Head_TransferYear').datepicker();
      $('#Head_AchvYear').datepicker();
      break;
    default:
      break;
  }
});

/*
 * ラジオチェック戻し
 */
$('input:radio').dblclick(function () {
  var nombre = $(this).attr('id');
  var checked = $(this).is(":checked");
  if (checked) {
    $("input[id=" + nombre + "]:radio").prop("checked", false);
  }
});

/*
 * 保存ボタンクリック時
 */
$('#dmysave').click(function () {

  if ($('#Head_IsRireki').val() === 'True') {
      // 2017-03-31 sbc-sagara upd str アラート表示形式統一
      //alert('履歴データの為、保存出来ません');
      showAlert('エラー', '履歴データの為、保存出来ません');
      // 2017-03-31 sbc-sagara upd end
    return;
  }

  //文章表示オーバー確認
  if (!sentenceCheck('tmpsave',this.value)) {
      return;
  }

  ////入力チェック 無いに等しい
  ////2017-03-31 sbc-sagara add str
  //message = '';

  ////入力チェック
  //if (!$('#Head_PostalCode_1').prop('readonly') && itemlineCheck(jQuery.trim($('#Head_BusinessPlanning').val()), 3, 1) == false) {
  //    message = '郵便番号';
  //} else if (!$('#Head_PostalCode_2').prop('readonly') && itemlineCheck(jQuery.trim($('#Head_BusinessPlanning').val()), 4, 1) == false){
  //    message = '郵便番号';
  //} else if (!$('#Head_Address').prop('readonly') && itemlineCheck(jQuery.trim($('#Head_Address').val()), 35, 3) == false) {
  //    message = '住所';
  //} else if (!$('#Head_PostalCode_2').prop('readonly') && itemlineCheck(jQuery.trim($('#Head_PostalCode_2').val()), 4, 1) == false) {
  //    message = '郵便番号';
  //} else if (!$('#Head_PostalCode_2').prop('readonly') && itemlineCheck(jQuery.trim($('#Head_BusinessPlanning').val()), 4, 1) == false) {
  //    message = '郵便番号';
  //} else if (!$('#Head_PostalCode_2').prop('readonly') && itemlineCheck(jQuery.trim($('#Head_BusinessPlanning').val()), 4, 1) == false) {
  //    message = '郵便番号';
  //} else if (!$('#Head_PostalCode_2').prop('readonly') && itemlineCheck(jQuery.trim($('#Head_BusinessPlanning').val()), 4, 1) == false) {
  //    message = '郵便番号';
  //} else if (!$('#Head_PostalCode_2').prop('readonly') && itemlineCheck(jQuery.trim($('#Head_BusinessPlanning').val()), 4, 1) == false) {
  //    message = '郵便番号';
  //} else if (!$('#Head_PostalCode_2').prop('readonly') && itemlineCheck(jQuery.trim($('#Head_BusinessPlanning').val()), 4, 1) == false) {
  //    message = '郵便番号';
  //} else if (!$('#Head_PostalCode_2').prop('readonly') && itemlineCheck(jQuery.trim($('#Head_BusinessPlanning').val()), 4, 1) == false) {
  //    message = '郵便番号';
  //} else if (!$('#Head_PostalCode_2').prop('readonly') && itemlineCheck(jQuery.trim($('#Head_BusinessPlanning').val()), 4, 1) == false) {
  //    message = '郵便番号';
  //} else if (!$('#Head_PostalCode_2').prop('readonly') && itemlineCheck(jQuery.trim($('#Head_BusinessPlanning').val()), 4, 1) == false) {
  //    message = '郵便番号';
  //}

  
  //if (!$('#Body_TransferDepartment_1').prop('readonly') && itemlineCheck(jQuery.trim($('#Head_BusinessPlanning').val()), 48, 6) == false) {
  //    message = '配置換えについて(1)';
  //} else if (!$('#Head_DepartmentPolicy').prop('readonly') && itemlineCheck(jQuery.trim($('#Head_DepartmentPolicy').val()), 48, 16) == false) {
  //    message = '所属部門';
  //}

  ////入力チェック(目標/達成水準、行数)
  //if (!$('#Head_BusinessPlanning').prop('readonly') && itemlineCheck(jQuery.trim($('#Head_BusinessPlanning').val()), 48, 16) == false) {
  //    message = '会社経営計画';
  //} else if (!$('#Head_DepartmentPolicy').prop('readonly') && itemlineCheck(jQuery.trim($('#Head_DepartmentPolicy').val()), 48, 16) == false) {
  //    message = '所属部門';
  //}
  //if (message != '') {
  //    showSaveMessageAndAlertEx(message);
  //    return;
  //}
  //  // 目標項目の数繰り返し
  //for (i = 0; $('#ObjList_' + i + '__ObjItem').length == 1; i++) {
  //    if (!$('#ObjList_' + i + '__ObjItem').prop('readonly') && itemlineCheck(jQuery.trim($('#ObjList_' + i + '__ObjItem').val()), 20, 12) == false) {
  //        message = '区分' + (i + 1) + 'の目標項目';
  //    } else if (!$('#ObjList_' + i + '__AchvLevel').prop('readonly') && itemlineCheck(jQuery.trim($('#ObjList_' + i + '__AchvLevel').val()), 20, 12) == false) {
  //        message = '区分' + (i + 1) + 'の達成水準・期限';
  //    } else if (!$('#ObjList_' + i + '__ObjPolicys').prop('readonly') && itemlineCheck(jQuery.trim($('#ObjList_' + i + '__ObjPolicys').val()), 30, 12) == false) {
  //        message = '区分' + (i + 1) + 'の目標達成のための施策・手段';
  //    } else if (!$('#ObjList_' + i + '__BestMetrics').prop('readonly') && itemlineCheck(jQuery.trim($('#ObjList_' + i + '__BestMetrics').val()), 15, 5) == false) {
  //        message = '区分' + (i + 1) + 'の大幅達成';
  //    } else if (!$('#ObjList_' + i + '__BetterMetrics').prop('readonly') && itemlineCheck(jQuery.trim($('#ObjList_' + i + '__BetterMetrics').val()), 15, 5) == false) {
  //        message = '区分' + (i + 1) + 'の達成';
  //    } else if (!$('#ObjList_' + i + '__SelfComment').prop('readonly') && itemlineCheck(jQuery.trim($('#ObjList_' + i + '__SelfComment').val()), 18, 11) == false) {
  //        message = '区分' + (i + 1) + 'の本人評価コメント';
  //    } else if (!$('#ObjList_' + i + '__BossComment').prop('readonly') && itemlineCheck(jQuery.trim($('#ObjList_' + i + '__BossComment').val()), 18, 11) == false) {
  //        message = '区分' + (i + 1) + 'の上司評価コメント';
  //    }
  //    if (message != '') {
  //        showSaveMessageAndAlertEx(message);
  //        return;
  //    }
  //}
  ////2017-03-31 sbc-sagara add end

  //ボタンクリック
  //showMessage('途中保存確認', '保存しますか？', 'savebutton', true);
  showMessageEx('途中保存確認', '保存しますか？', 'savebutton', true);

});

/*
 * 前回データ取得ボタンクリック時
 */
$('#dmyprevdatacopy').click(function () {
    showMessageEx('データ上書き確認', '前回データで上書きします。よろしいですか？    \n※入力途中のデータはすべて破棄されます。', 'prevdatacopybutton', true);
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
          //2016-05-23 iwai-tamura upd str 途中保存のPDF出力をやめる -----
          $('#' + buttonid).trigger('click');
          //showPrintMessage('PDF出力確認', 'PDFを出力しますか？', buttonid, 'save2printbutton');
          //2016-05-23 iwai-tamura upd str -----
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
  $('#dmy11, #dmy12, #dmy21, #dmy22, #dmy211, #dmy212, #dmy213, #dmy214, #dmy221, #dmy223, #dmy224').click(function () {

    var msg = '承認画面　承認しますか？';
    switch (this.id) {
      case 'dmy11':
      case 'dmy21':
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

    //文章表示オーバー確認
    if (!sentenceCheck('Signature', this.value)) {
        return;
    }



    ////2016-05-23 iwai-tamura upd str  -----
    ////ウェイトチェック(本人)
    //if (this.id === 'dmy111' ||
    //    this.id === 'dmy112') {
    //    if (selfWeightCheck() == false) {
    //        // 2017-03-31 sbc-sagara upd str アラート表示形式統一
    //        //alert('ウェイトは合計１００%になるように入力してください');
    //        showAlert('エラー', 'ウェイトは合計１００%になるように入力してください');
    //        // 2017-03-31 sbc-sagara upd end
    //        return;
    //    }
    //}
    //  //ウェイトチェック(上司)
    //if (this.id === 'dmy112') {
    //    if (bossWeightCheck() == false) {
    //        // 2017-03-31 sbc-sagara upd str アラート表示形式統一
    //        //alert('ウェイトは合計１００%になるように入力してください');
    //        showAlert('エラー', 'ウェイトは合計１００%になるように入力してください');
    //        // 2017-03-31 sbc-sagara upd end
    //        return;
    //    }
    //}
    ////2016-05-23 iwai-tamura upd end  -----

    ////目標面談
    //if (this.id === 'dmy112') {
    //  if (objDateCheck() == false) {
    //      // 2017-03-31 sbc-sagara upd str アラート表示形式統一
    //      //alert('目標設定面談実施日に正しい日付を入力してください');
    //      showAlert('エラー', '目標設定面談実施日に正しい日付を入力してください');
    //      // 2017-03-31 sbc-sagara upd end
    //    return;
    //  }
    //}
    ////達成面談
    //if (this.id === 'dmy212') {
    //  if (achvDateCheck() == false) {
    //      // 2017-03-31 sbc-sagara upd str アラート表示形式統一
    //      //alert('達成度評価面談実施日に正しい日付を入力してください');
    //      showAlert('エラー', '達成度評価面談実施日に正しい日付を入力してください');
    //      // 2017-03-31 sbc-sagara upd end
    //    return;
    //  }
    //}
    ////異動
    //if (this.id === 'dmy211' ||
    //    this.id === 'dmy212') {
    //  if (trnsDateCheck() == false) {
    //      // 2017-03-31 sbc-sagara upd str アラート表示形式統一
    //      //alert('期中の人事異動に正しい日付を入力してください');
    //      showAlert('エラー', '期中の人事異動に正しい日付を入力してください');
    //      // 2017-03-31 sbc-sagara upd end
    //    return;
    //  }
    //}

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







    ////2017-03-31 sbc-sagara add str
    ////入力チェック(目標/達成水準、行数)
    //if (!$('#Head_BusinessPlanning').prop('readonly') && itemlineCheck(jQuery.trim($('#Head_BusinessPlanning').val()), 48, 16) == false) {
    //    message = '会社経営計画';
    //    showSaveMessageAndAlert(message, msg, this.value);
    //    return;
    //} else if (!$('#Head_DepartmentPolicy').prop('readonly') && itemlineCheck(jQuery.trim($('#Head_DepartmentPolicy').val()), 48, 16) == false) {
    //    message = '所属部門';
    //    showSaveMessageAndAlert(message, msg, this.value);
    //    return;
    //}
    //// 目標項目の数繰り返し
    //for (i = 0; $('#ObjList_' + i + '__ObjItem').length == 1; i++) {
    //    if (!$('#ObjList_' + i + '__ObjItem').prop('readonly') && itemlineCheck(jQuery.trim($('#ObjList_' + i + '__ObjItem').val()), 20, 12) == false) {
    //        message = '区分' + (i + 1) + 'の目標項目';
    //        showSaveMessageAndAlert(message, msg, this.value);
    //        return;
    //    } else if (!$('#ObjList_' + i + '__AchvLevel').prop('readonly') && itemlineCheck(jQuery.trim($('#ObjList_' + i + '__AchvLevel').val()), 20, 12) == false) {
    //        message = '区分' + (i + 1) + 'の達成水準・期限';
    //        showSaveMessageAndAlert(message, msg, this.value);
    //        return;
    //    } else if (!$('#ObjList_' + i + '__ObjPolicys').prop('readonly') && itemlineCheck(jQuery.trim($('#ObjList_' + i + '__ObjPolicys').val()), 30, 12) == false) {
    //        message = '区分' + (i + 1) + 'の目標達成のための施策・手段';
    //        showSaveMessageAndAlert(message, msg, this.value);
    //        return;
    //    } else if (!$('#ObjList_' + i + '__BestMetrics').prop('readonly') && itemlineCheck(jQuery.trim($('#ObjList_' + i + '__BestMetrics').val()), 15, 5) == false) {
    //        message = '区分' + (i + 1) + 'の大幅達成';
    //        showSaveMessageAndAlert(message, msg, this.value);
    //        return;
    //    } else if (!$('#ObjList_' + i + '__BetterMetrics').prop('readonly') && itemlineCheck(jQuery.trim($('#ObjList_' + i + '__BetterMetrics').val()), 15, 5) == false) {
    //        message = '区分' + (i + 1) + 'の達成';
    //        showSaveMessageAndAlert(message, msg, this.value);
    //        return;
    //    } else if (!$('#ObjList_' + i + '__SelfComment').prop('readonly') && itemlineCheck(jQuery.trim($('#ObjList_' + i + '__SelfComment').val()), 18, 11) == false) {
    //        message = '区分' + (i + 1) + 'の本人評価コメント';
    //        showSaveMessageAndAlert(message, msg, this.value);
    //        return;
    //    } else if (!$('#ObjList_' + i + '__BossComment').prop('readonly') && itemlineCheck(jQuery.trim($('#ObjList_' + i + '__BossComment').val()), 18, 11) == false) {
    //        message = '区分' + (i + 1) + 'の上司評価コメント';
    //        showSaveMessageAndAlert(message, msg, this.value);
    //        return;
    //    }
    //}
    //2017-03-31 sbc-sagara add end


    //ボタンクリック
    showMessage('確認', msg, this.value, true);
  });
});

/*
 * 承認キャンセルボタンクリック時
 */
$(function () {
  $('#dmyc11, #dmyc12, #dmyc21, #dmyc22, #dmyc211, #dmyc212, #dmyc213, #dmyc214, #dmyc221, #dmyc223, #dmyc224').click(function () {

    var msg = '承認画面　承認をキャンセルしますか？';
    switch (this.id) {
      case 'dmyc11':
      case 'dmyc21':
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
 * 入力文字数確認
 */
function sentenceCheck(type,val) {

  message = '';

    //入力チェック
  if (!$('#Head_PostalCode_1').prop('readonly') && itemlineCheck(jQuery.trim($('#Head_BusinessPlanning').val()), 3, 1) == false) {
      message = '郵便番号';
  } else if (!$('#Head_PostalCode_2').prop('readonly') && itemlineCheck(jQuery.trim($('#Head_BusinessPlanning').val()), 4, 1) == false) {
      message = '郵便番号';
  } else if (!$('#Head_Address').prop('readonly') && itemlineCheck(jQuery.trim($('#Head_Address').val()), 35, 3) == false) {
      message += '　住所<br />';
  }


  switch ($('#Head_InputMode').val()) {
      case 'AtoCSelfSign':
      case 'DSelfSign':
          if (!$('#Body_ChargeDuty_1').prop('readonly') && itemlineCheck(jQuery.trim($('#Body_ChargeDuty_1').val()), 48, 6) == false) {
              message += '　担当職務について(1)<br />';
          }
          if (!$('#Body_ChargeDuty_2').prop('readonly') && itemlineCheck(jQuery.trim($('#Body_ChargeDuty_2').val()), 48, 6) == false) {
              message += '　担当職務について(2)<br />';
          }
          if (!$('#Body_ChargeDuty_3').prop('readonly') && itemlineCheck(jQuery.trim($('#Body_ChargeDuty_3').val()), 48, 6) == false) {
              message += '　担当職務について(3)<br />';
          }
          if (!$('#Body_ChargeDuty_4').prop('readonly') && itemlineCheck(jQuery.trim($('#Body_ChargeDuty_4').val()), 48, 6) == false) {
              message += '　担当職務について(4)<br />';
          }
          if (!$('#Body_ChargeDuty_5').prop('readonly') && itemlineCheck(jQuery.trim($('#Body_ChargeDuty_5').val()), 48, 6) == false) {
              message += '　担当職務について(5)<br />';
          }
          if (!$('#Body_ChargeDuty_6').prop('readonly') && itemlineCheck(jQuery.trim($('#Body_ChargeDuty_6').val()), 48, 6) == false) {
              message += '　担当職務について(6)<br />';
          }
          if (!$('#Body_AptitudeDevelop_1').prop('readonly') && itemlineCheck(jQuery.trim($('#Body_AptitudeDevelop_1').val()), 48, 6) == false) {
              message += '　能力開発について(1)<br />';
          }
          if (!$('#Body_AptitudeDevelop_2_Content').prop('readonly') && itemlineCheck(jQuery.trim($('#Body_AptitudeDevelop_2_Content').val()), 48, 6) == false) {
              message += '　適性・能力開発について(2)<br />';
          }
          if (!$('#Body_Aptitude_3').prop('readonly') && itemlineCheck(jQuery.trim($('#Body_Aptitude_3').val()), 48, 6) == false) {
              message += '　適性について(3)<br />';
          }
          if (!$('#Body_TransferDutyDepartment_1_Content').prop('readonly') && itemlineCheck(jQuery.trim($('#Body_TransferDutyDepartment_1_Content').val()), 48, 8) == false) {
              message += '　職務変更・配置換えについて(1)<br />';
          }
          if (!$('#Body_TransferDepartment_1').prop('readonly') && itemlineCheck(jQuery.trim($('#Body_TransferDepartment_1').val()), 48, 6) == false) {
              message += '　配置換えについて(1)<br />';
          }

          if (!$('#Body_AptitudeDevelop_1_1').prop('readonly') && itemlineCheck(jQuery.trim($('#Body_AptitudeDevelop_1_1').val()), 48, 6) == false) {
              message += '　能力開発について(1) [内容]<br />';
          }
          if (!$('#Body_AptitudeDevelop_1_3').prop('readonly') && itemlineCheck(jQuery.trim($('#Body_AptitudeDevelop_1_3').val()), 48, 6) == false) {
              message += '　能力開発について(1) [理由]<br />';
          }
          if (!$('#Body_AptitudeDevelop_2').prop('readonly') && itemlineCheck(jQuery.trim($('#Body_AptitudeDevelop_2').val()), 48, 6) == false) {
              message += '　能力開発について(2)<br />';
          }
          if (!$('#Body_AptitudeDevelop_3').prop('readonly') && itemlineCheck(jQuery.trim($('#Body_AptitudeDevelop_3').val()), 48, 6) == false) {
              message += '　能力開発について(3)<br />';
          }

          if (!$('#Body_OtherComment').prop('readonly') && itemlineCheck(jQuery.trim($('#Body_OtherComment').val()), 48, 6) == false) {
              message += '　その他<br />';
          }
          if (!$('#Body_FreeComment').prop('readonly') && itemlineCheck(jQuery.trim($('#Body_FreeComment').val()), 48, 8) == false) {
              message += '　自由意見<br />';
          }

          break;

          
      case 'AtoCBossSign':
      case 'DBossSign':
          if (!$('#Body_BossComment').prop('readonly') && itemlineCheck(jQuery.trim($('#Body_BossComment').val()), 48, 4) == false) {
              message += '　上司記入欄<br />';
          }
          break;

      default:
          break;
  }

  if (message != '') {
      switch (type) {
          case 'tmpsave':
          case '11':
          case '21':
              showSaveMessageAndAlertEx(message);
              return;
              break;
          case 'Signature':
              msg = '登録確認画面　登録しますか？';
              showSaveMessageAndAlert(message,msg,val);
              return;
              break;
          default:
              return;
              break;
      }
  }
  return true;
}



///*
// * 日付削除
// */
//$(function () {
//  $('#moveCancel, #objCancel, #achvCancel').click(function () {
//    $('#' + this.value).val('');
//  });
//});

///*
// * 目標入力チェック
// */
//function itemCheck() {
//  for (i = 0; i < 3;i++){
//    if (jQuery.trim($('#ObjList_' + i + '__ObjItem').val()) === '') {
//      return false;
//    }

//  }
//  return true;
//}

////目標面談日チェック
//function objDateCheck() {
//  var value = $('#Head_ObjYear').val();
//  if (jQuery.trim(value) === '') {
//    return false;
//  }
//  var ds = Date.parse(value);
//  var min = Date.parse(parseInt($('#Head_JcalYear').val()) - 1 + '/4/1');
//  var max = Date.parse(parseInt($('#Head_JcalYear').val()) + 1 + '/12/31');
//  //var fds = new Date(ds);
//  //var mind = new Date(min);
//  //var maxd = new Date(max);
//  if (!(min <= ds && ds <= max)) {
//    return false;
//  }
//  return true;
//}

////達成面談日チェック
//function achvDateCheck() {
//  var value = $('#Head_AchvYear').val();
//  if (jQuery.trim(value) === '') {
//    return false;
//  }
//  var ds = Date.parse(value);
//  var min = Date.parse(parseInt($('#Head_JcalYear').val()) - 1 + '/4/1');
//  var max = Date.parse(parseInt($('#Head_JcalYear').val()) + 1 + '/12/31');
//  if (!(min <= ds && ds <= max)) {
//    return false;
//  }
//  return true;
//}

////異動日チェック
//function trnsDateCheck() {
//  var value = $('#Head_TransferYear').val();
//  if (jQuery.trim(value) === '') {
//    return true;
//  }
//  var ds = Date.parse(value);
//  var min = Date.parse(parseInt($('#Head_JcalYear').val()) + '/4/1');
//  var max = Date.parse(parseInt($('#Head_JcalYear').val()) + 1 + '/3/31');
//  if (!(min <= ds && ds <= max)) {
//    return false;
//  }
//  return true;
//}


////2016-05-23 iwai-tamura upd str  -----
////ウェイトチェック
//function selfWeightCheck() {
//    var value = 0;
//    for (i = 0; i < 5; i++) {
//        value += Number(jQuery.trim($('#ObjList_' + i + '__SelfWeight').val()))
//    }
//    if (!(value === 100)){
//        return false;
//    }
//    return true;
//}

//function bossWeightCheck() {
//    var value = 0;
//    for (i = 0; i < 5; i++) {
//        value += Number(jQuery.trim($('#ObjList_' + i + '__BossWeight').val()))
//    }
//    if (!(value === 100)) {
//        return false;
//    }
//    return true;
//}
////2016-05-23 iwai-tamura upd end -----

////2017-04-11 iwai-tamura upd str  -----
////達成度・プロセスチェック
//function selfAchvCheck() {
//    for (i = 0; i < 5; i++) {
//        if(Number(jQuery.trim($('#ObjList_' + i + '__SelfWeight').val())) > 0){
//            if(jQuery.trim($('#ObjList_' + i + '__SelfAchv').val()) === ""){
//                return false;
//            }
//        }
//    }
//    return true;
//}

//function bossAchvCheck() {
//    for (i = 0; i < 5; i++) {
//        if(Number(jQuery.trim($('#ObjList_' + i + '__BossWeight').val())) > 0){
//            if(jQuery.trim($('#ObjList_' + i + '__BossAchv').val()) === ""){
//                return false;
//            }
//        }
//    }
//    return true;
//}
//function selfProcessCheck() {
//    for (i = 0; i < 5; i++) {
//        if (Number(jQuery.trim($('#ObjList_' + i + '__SelfWeight').val())) > 0) {
//            if (jQuery.trim($('#ObjList_' + i + '__SelfProcess').val()) === "") {
//                return false;
//            }
//        }
//    }
//    return true;
//}

//function bossProcessCheck() {
//    for (i = 0; i < 5; i++) {
//        if (Number(jQuery.trim($('#ObjList_' + i + '__BossWeight').val())) > 0) {
//            if (jQuery.trim($('#ObjList_' + i + '__BossProcess').val()) === "") {
//                return false;
//            }
//        }
//    }
//    return true;
//}
////2017-04-11 iwai-tamura upd end  -----

