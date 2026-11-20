/*
 * 一括作成ボタンクリック時
 */
$('#DmyDoBulk').click(function () {

    var comment = "";
    //値を取得
    var year = $('#BulkYear').val();
    var bulkType = $('#BulkType option:selected').val();
    var bulkDuration = $('#BulkDuration option:selected').text();

    comment = checkError(year, bulkType, bulkDuration, 'employee', 'bulk');

    if (comment) {
        showAlert("エラー", comment);
        return;
    }

  //未選択、未入力は空に
  year = year === '' ? '' : year + '年度 ';
  var duration = bulkType === '' ? '' : bulkDuration + ' ';
  //目標管理の場合は期間は空にする
  duration = bulkType === 'Objective' ? '' : duration;
  //確認メッセージ本文作成
  comment = year + duration + $('#BulkType option:selected').text() + '初期データ一括作成を実行しますか？'
  showMessage('確認', comment, 'DoBulk', true);
});

/*
 * 個別作成ボタンクリック時
 */
$('#DmyDoDesignate').click(function () {

  var comment = "";
  //値を取得
  var year = $('#DesignateYaer').val();
  var dType = $('#DesignateType option:selected').val();
  var dDuration = $('#DesignateDuration option:selected').text();
  var dEmployee = $('#DesignateEmployeeNo').val();

  //一括作成と同じエラーチェックを実行
  comment = checkError(year, dType, dDuration,dEmployee,'designate');

  if (comment) {
    showAlert("エラー", comment);
    return;
  }

  //未選択、未入力は空に
  year = year === '' ? '' : year + '年度 ';
  var duration = dDuration === '' ? '' : dDuration + ' ';
  var employee = dEmployee === '' ? '' : '社員番号' + dEmployee + ' ';
  //目標管理の場合は期間は空にする
  duration = dType === 'Objective' ? '' : duration;
  //確認メッセージ本文作成
  comment = year + duration + employee + $('#DesignateType option:selected').text() + '初期データ個別作成を実行しますか？'
  showMessage('確認', comment, 'DoDesignate', true);
});


// 2018-03-20 iwai-tamura upd str ------
/*
 * 異動によるデータ引継ぎ登録ボタンクリック時
 */
$('#DmyDoTakeOverMoveSingle').click(function () {

    var comment = "";

    //値を取得
    var year = $('#TakeOverMoveSingleYaer').val();
    var dType = $('#TakeOverMoveSingleType option:selected').val();
    var dBefEmployeeNo = $('#TakeOverMoveSingleBefEmployeeNo').val();
    var dBefDepartment = $('#TakeOverMoveSingleBefDepartment').val();
    var dAftEmployeeNo = $('#TakeOverMoveSingleAftEmployeeNo').val();
    var dAftDepartment = $('#TakeOverMoveSingleAftDepartment').val();

    //対象年度入力チェック
    if (!isFinite(year) || year.length != 4 || year.match(/^[0]/)) {
        showAlert("エラー", "年度は半角数字4桁で入力してください。")
        return;
    }

    //対象処理入力チェック
    if (!dType) {
        showAlert("エラー", "対象処理を入力してください。")
        return;
    }

    //前任社員番号入力チェック
    if (!dBefEmployeeNo) {
        showAlert("エラー", "前任者の社員番号を入力してください。")
        return;
    }

    //前任所属番号入力チェック
    if (!dBefDepartment) {
        showAlert("エラー", "前任者の所属番号を入力してください。")
        return;
    }

    //後任社員番号入力チェック
    if (!dAftEmployeeNo) {
        showAlert("エラー", "後任者の社員番号を入力してください。")
        return;
    }

    //前任所属番号入力チェック
    if (!dAftDepartment) {
        showAlert("エラー", "後任者の所属番号を入力してください。")
        return;
    }

    //未選択、未入力は空に
    year = year === '' ? '' : year + '年度 ';
    var Befemployee = dBefEmployeeNo === '' ? '' : '前任 社員番号:' + dBefEmployeeNo + ' ';
    Befemployee = dBefDepartment === '' ? '' : Befemployee + ' 所属番号' + dBefDepartment + ' ';
    var Aftemployee = dAftEmployeeNo === '' ? '' : '後任 社員番号:' + dAftEmployeeNo + ' ';
    Aftemployee = dAftDepartment === '' ? '' : Aftemployee + ' 所属番号' + dAftDepartment + ' ';

    //確認メッセージ本文作成
    comment = year + $('#TakeOverMoveSingleType option:selected').text() + "<br />" + Befemployee + "<br />" + Aftemployee + "<br />" + 'データの引継ぎを実行しますか？'
    showMessage('確認', comment, 'DoTakeOverMoveSingle', true);

});


/*
 * 異動によるデータ一括引継ぎ登録ボタンクリック時
 */
$('#DmyDoTakeOverMoveBulk').click(function () {

    var comment = "";
    //値を取得
    var year = $('#TakeOverMoveBulkYaer').val();
    var dType = $('#TakeOverMoveBulkType option:selected').val();
    var dUploadFile = $('#TakeOverMoveBulkUploadFile').val();

    //対象年度入力チェック
    if (!isFinite(year) || year.length != 4 || year.match(/^[0]/)) {
        showAlert("エラー", "年度は半角数字4桁で入力してください。")
        return;
    }

    //対象処理入力チェック
    if (!dType) {
        showAlert("エラー", "対象処理を入力してください。")
        return;
    }

    //対象処理入力チェック
    if (!dUploadFile) {
        showAlert("エラー", "ファイルが選択されていません。")
        return;
    }
    
    //未選択、未入力は空に
    year = year === '' ? '' : year + '年度 ';

    //確認メッセージ本文作成
    comment = year + $('#TakeOverMoveSingleType option:selected').text() + "<br />" + 'データの一括引継ぎを実行しますか？'
    showMessage('確認', comment, 'DoTakeOverMoveBulk', true);

});


/*
 * 組織規程改正によるデータ引継ぎ処理ボタンクリック時
 */
$('#DmyDoTakeOverAmendmentCompany').click(function () {

    var comment = "";

    //値を取得
    var year = $('#TakeOverAmendmentCompanyYaer').val();
    var dType = $('#TakeOverAmendmentCompanyType option:selected').val();
    var dEmployeeNo = $('#TakeOverAmendmentCompanyEmployeeNo').val();
    var dBefDepartment = $('#TakeOverAmendmentCompanyBefDepartment').val();
    var dAftDepartment = $('#TakeOverAmendmentCompanyAftDepartment').val();

    //対象年度入力チェック
    if (!isFinite(year) || year.length != 4 || year.match(/^[0]/)) {
        showAlert("エラー", "年度は半角数字4桁で入力してください。")
        return;
    }

    //対象処理入力チェック
    if (!dType) {
        showAlert("エラー", "対象処理を入力してください。")
        return;
    }

    //社員番号入力チェック
    if (!dEmployeeNo) {
        showAlert("エラー", "社員番号を入力してください。")
        return;
    }

    //組編前所属番号入力チェック
    if (!dBefDepartment) {
        showAlert("エラー", "引継元の所属番号を入力してください。")
        return;
    }

    //組編後所属番号入力チェック
    if (!dAftDepartment) {
        showAlert("エラー", "引継先の所属番号を入力してください。")
        return;
    }

    //未選択、未入力は空に
    year = year === '' ? '' : year + '年度 ';
    var employee = dEmployeeNo === '' ? '' : '社員番号:' + dEmployeeNo + ' ';
    var BefDepartment = dBefDepartment === '' ? '' : '引継元所属番号:' + dBefDepartment + ' ';
    var AftDepartment = dAftDepartment === '' ? '' : '引継先所属番号:' + dAftDepartment + ' ';

    //確認メッセージ本文作成
    comment = year + $('#TakeOverAmendmentCompanyType option:selected').text() + "<br />" + employee + "<br />" + BefDepartment + AftDepartment + "<br />" + 'データの引継ぎを実行しますか？'
    showMessage('確認', comment, 'DoTakeOverAmendmentCompany', true);

});

/*
 * 組編によるデータ一括引継ぎ登録ボタンクリック時
 */
$('#DmyDoTakeOverAmendmentCompanyBulk').click(function () {

    var comment = "";
    //値を取得
    var year = $('#TakeOverAmendmentCompanyBulkYaer').val();
    var dType = $('#TakeOverAmendmentCompanyBulkType option:selected').val();
    var dUploadFile = $('#TakeOverAmendmentCompanyBulkUploadFile').val();

    //対象年度入力チェック
    if (!isFinite(year) || year.length != 4 || year.match(/^[0]/)) {
        showAlert("エラー", "年度は半角数字4桁で入力してください。")
        return;
    }

    //対象処理入力チェック
    if (!dType) {
        showAlert("エラー", "対象処理を入力してください。")
        return;
    }

    //対象処理入力チェック
    if (!dUploadFile) {
        showAlert("エラー", "ファイルが選択されていません。")
        return;
    }

    //未選択、未入力は空に
    year = year === '' ? '' : year + '年度 ';

    //確認メッセージ本文作成
    comment = year + $('#TakeOverAmendmentCompanySingleType option:selected').text() + "<br />" + 'データの一括引継ぎを実行しますか？'
    showMessage('確認', comment, 'DoTakeOverAmendmentCompanyBulk', true);

});
// 2018-03-20 iwai-tamura upd end ------



/*
 * 確定処理ボタンクリック時
 */
$('#DmyDoCommit').click(function () {
  var comment = "";
  //値を取得
  var commitArea = $('#CommitArea option:selected').val();
  var year = $('#CommitYear').val();
  var commitType = $('#CommitType option:selected').val();
  var commitDuration = $('#CommitDuration option:selected').text();

    //一括作成と同じエラーチェックを実行
  comment = checkError(year, commitType, commitDuration, "employee", commitArea);

  //システム管理 職能支社確定エラーチェック
  if (commitType == "Skill" && commitArea == "1") {
    showAlert("エラー", "支社確定は目標管理のみです。")
    return;
  }

  if (comment) {
    showAlert("エラー", comment);
    return;
  }

  //未選択、未入力は空に
  year = year === '' ? '' : year + '年度 ';
  var duration = commitDuration === '' ? '' : commitDuration + ' ';
  //目標管理の場合は期間は空にする
  duration = $('#CommitType option:selected').val() === 'Objective' ? '' : duration;
  //確認メッセージ本文作成
  comment = $('#CommitArea option:selected').text() + " " + year + duration + $('#CommitType option:selected').text() + '確定処理を実行しますか？'
  showMessage('確定', comment, 'DoCommit', true);
});

/*
 * パスワードリセットボタンクリック時
 */
$('#DmyResetPwd').click(function () {
  //未選択、未入力は空に
  var employee = $('#ResetEmployeeNo').val() === '' ? '' : '社員番号' + $('#ResetEmployeeNo').val() + ' ';

  var comment = "";
  //一括作成と同じエラーチェックを実行(社員番号のみ入力チェック)
  comment = checkError('2015', 'type', 'duration', employee, 'area');
  if (comment) {
    showAlert("エラー", comment);
    return;
  }

  //確認メッセージ本文作成
  var comment = employee + 'パスワードリセットを実行しますか？'
  showMessage('パスワードリセット', comment, 'ResetPwd', true);
});

/*
 * お知らせ登録ボタンクリック時
 */
$('#DmyCreateInfo').click(function () {
  showMessage('お知らせ登録', 'お知らせ情報登録を実行しますか？', 'CreateInfo', true);
});


// 2017-03-31 sbc-sagara add str パスワード確認・アカウント追加機能追加
/*
 * アカウント追加ボタンクリック時
 */
$('#DmyAddAccount').click(function () {
    //未選択、未入力は空に
    var employee = $('#AddEmployeeNo').val() === '' ? '' : jQuery.trim($('#AddEmployeeNo').val());
    //2017-06-16 iwai-tamura add str アカウント参照追加機能
    var password = $('#AddPassword').val() === '' ? '' : jQuery.trim($('#AddPassword').val());
    var reference = $('#ReferenceEmployeeNo').val() === '' ? '' : jQuery.trim($('#ReferenceEmployeeNo').val());
    //2017-06-16 iwai-tamura add end アカウント参照追加機能
    var comment = "";
    //一括作成と同じエラーチェックを実行(社員番号のみ入力チェック)
    comment = checkError('2015', 'type', 'duration', employee, 'area');

    //2017-06-16 iwai-tamura add end アカウント参照追加機能
    if (reference.length > 0) {
        // 参照元社員番号設定時、パスワード入力はエラー?
        if (password.length > 0) {
            comment += comment != "" ? "<br />" : "";
            comment += '参照元社員番号設定時、パスワードは空欄にしてください。';
        }
    } else {
        //アカウント追加用パスワード入力チェック
        if (!(password.length > 0)) {
            comment += comment != "" ? "<br />" : "";
            comment += 'パスワードを入力してください。';
        }
    }
    ////アカウント追加用パスワード入力チェック
    //if (!(jQuery.trim($('#AddPassword').val()).length > 0)) {
    //    comment += comment != "" ? "<br />" : "";
    //    comment += 'パスワードを入力してください。';
    //// 2017-05-26 iwai-tamura del str ------
    ////} else if (!(jQuery.trim($('#AddPassword').val()).length > 5)) {
    ////    comment += comment != "" ? "<br />" : "";
    ////    comment += 'パスワードの長さは6文字以上である必要があります。';
    //// 2017-05-26 iwai-tamura del end ------
    //}
    //2017-06-16 iwai-tamura add end アカウント参照追加機能

    if (comment) {
        showAlert("エラー", comment);
        return;
    }
    showMessage('アカウント追加', 'アカウント追加を実行しますか？', 'AddAccount', true);
});
/*
 * パスワード確認ボタンクリック時
 */
$('#DmyConfirmPwd').click(function () {
    //未選択、未入力は空に
    var employee = $('#ResetEmployeeNo').val() === '' ? '' : '社員番号' + $('#ResetEmployeeNo').val() + ' ';

    var comment = "";
    //一括作成と同じエラーチェックを実行(社員番号のみ入力チェック)
    comment = checkError('2015', 'type', 'duration', employee, 'area');
    if (comment) {
        showAlert("エラー", comment);
        return;
    }

    showMessage('パスワード確認', 'パスワード確認を実行しますか？', 'ConfirmPwd', true);
});
// 2017-03-31 sbc-sagara add end パスワード確認・アカウント追加機能追加


// 2017-04-30 sbc-sagara add str ファイルアップロード,CSV出力機能追加
/*
 * アップロードボタンクリック時
 */
$('#DmyUpload').click(function () {
    var year = $("#UploadYear").val() === "" ? "" : $("#UploadYear").val();
    var comment = "";

    comment = checkError(year, 'type', 'duration', 'employee', 'bulk');
    if (comment) {
        showAlert("エラー", comment);
        return;
    }

    showMessage('アップロード確認', 'アップロードを実行しますか？', 'Upload', true);
});
/*
 * Excel出力ボタンクリック時
 */
$("#DmyExcelOutput").click(function () {
    var year = $("#OutputYear").val() === "" ? "" : $("#OutputYear").val();
    var comment = "";

    comment = checkError(year, 'type', 'duration', 'employee', 'bulk');
    if (comment) {
        showAlert("エラー", comment);
        return;
    }

    showMessage('Excel出力確認', 'Excel出力を実行しますか？', 'ExcelOutput', false);
});
// 2017-04-30 sbc-sagara add end ファイルアップロード,CSV出力機能追加

/*
 * 一括作成エラーチェック
 * @param year 対象年度
 * @param type 対象データ(目標管理 or 職能職務)
 * @param duration 対象期間
 * @param employee 対象社員番号
 * @param commitArea 確定区分
 */
function checkError(year, type, duration, employee, commitArea) {

  var cmt = "";

  //社員番号入力チェック
  if (!employee) cmt = "社員番号を入力してください。";

  //2016-01-21 iwai-tamura add str -----
  //目標管理選択時の期間選択チェック
  if (type == "Objective" && duration) cmt = "対象が目標管理の場合、期間は入力できません。";
  //2016-01-21 iwai-tamura add end -----

  //職能選択時の期間選択チェック
  if (type == "Skill" && !duration) cmt = "期間を選択してください。";

  //年度入力値チェック
  if (!isFinite(year) || year.length != 4 || year.match(/^[0]/)) cmt = "年度は半角数字4桁で入力してください。";
  
  //対象選択チェック
  if (!type) cmt = "対象を選択してください。";

  //確定区分選択チェック
  if (!commitArea) cmt = "確定区分を選択してください。";

  return cmt;

}





