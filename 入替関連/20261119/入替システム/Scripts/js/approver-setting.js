/*
 * 設定ボタンクリック時
 */
$(function () {
  $('#view0,#view1, #view2, #view3, #view4, #view5, #view6, #view7, #view8, #view9, #view10, #view11, #view12, #view13').click(function () {
    /*エラーチェック*/                                                         //テキストボックスの値
    var selected = $('#Detail_' + this.value + '__SetSearch_Select').val();         //リストボックスの値
    var directed = $('#Detail_' + this.value + '__SetSearch_DirectEmpNum').val();   //テキストボックスの値
    if (typeof selected == "undefined") selected = "";
    if (typeof directed == "undefined") directed = "";
    
    if (!(selected + directed)) {
      showAlert("エラー", "権限者を選択してください。");
      return;
    }

    var strE = selected;
    strE = directed;
    if (strE == $('#Head_EmployeeNo').val()){
      showAlert("エラー", "対象者と異なる社員番号を選択してください。");
      return;
    }

    showMessage('確認', '決裁権限者を設定しますか。', this.id + "dmy", true);
  });
});

/*
 * 保存ボタンクリック時
 */
$(function () {
  $('#dmysave').click(function () {
    showMessage('確認', '決裁権限詳細を保存しますか。', 'savebutton', true);
  });
});

/*
 * クリアボタンクリック時
 */
$(function () {
  $('#clear0,#clear1, #clear2, #clear3, #clear4, #clear5, #clear6, #clear7, #clear8, #clear9, #clear10, #clear11, #clear12, #clear13').click(function () {
    showMessage('確認', '決裁権限者をクリアしますか。', this.id + "dmy", true);
    ///*エラーチェック*/                                                         //テキストボックスの値
    //var selected = $('#Detail_' + this.value + '__SetSearch_Select').val();         //リストボックスの値
    //var directed = $('#Detail_' + this.value + '__SetSearch_DirectEmpNum').val();   //テキストボックスの値
    //if (typeof selected == "undefined") selected = "";
    //if (typeof directed == "undefined") directed = "";

    //if (!(selected + directed)) {
    //  showAlert("エラー", "権限者を選択してください。");
    //  return;
    //}

    //var strE = selected;
    //strE = directed;
    //if (strE == $('#Head_EmployeeNo').val()) {
    //  showAlert("エラー", "対象者と異なる社員番号を選択してください。");
    //  return;
    //}

    //showMessage('確認', '決裁権限者を設定しますか。', this.id + "dmy", true);
  });
});