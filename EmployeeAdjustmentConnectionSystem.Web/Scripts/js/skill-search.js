/*
 * 全選択・全解除をクリック時
 */
$('#allCheck').click(function () {
  //チェックボックス取得
  var items = $('#items').find('input');
  //全選択チェック時 true・・・全チェック、false・・・未チェック
  var isCheck = $(this).is(':checked') ? true : false;
  //置き換え
  $(items).prop('checked', isCheck);
});

/*
 * 検索ボタンクリック時
 */
$('#dmysearch').click(function () {
  //ローディングパネル表示
  showLoading();
  //ボタンクリック
  $('#searchbutton').trigger('click');
});
//2017-08-31 iwai-tamura upd-str ------
/*
 * 支社調整ボタンクリック時
 */
$('#dmybranch').click(function () {
    //ローディングパネル表示
    //showLoading();
    //ボタンクリック
    $('#branchbutton').trigger('click');
});
/*
 * 部門調整ボタンクリック時
 */
$('#dmydepartment').click(function () {
    //ローディングパネル表示
    //showLoading();
    //ボタンクリック
    $('#departmentbutton').trigger('click');
});
//2017-08-31 iwai-tamura upd-end ------

/*
 * 1次ボタンクリック時
 */
$('#dmyprimary').click(function () {
  //ローディングパネル表示
  //showLoading();
  //ボタンクリック
  $('#primarybutton').trigger('click');
});

/*
 * 2次ボタンクリック時
 */
$('#dmysecondary').click(function () {
  //ローディングパネル表示
  //showLoading();
  //ボタンクリック
  $('#secondarybutton').trigger('click');
});


/*
 * 帳票出力ボタンクリック時
 */
$('#dmyprint').click(function () {
  showMessage('帳票出力', '出力しますか？', 'printbutton', false);
});

/*
 * 一括承認ボタンクリック時
 */
$('#dmysign').click(function () {
  //ボタンクリック
  showMessage('一括承認', '承認しますか？', 'signbutton', true);
});

//2017-03-31 sbc-sagara add str 一括Excel出力ボタン追加
/*
 * 一括Excelボタンクリック時
 */
$('#dmyprintxls').click(function () {
    showMessage('Excel出力', '出力しますか？', 'printxlsbutton', false);
});
//2017-03-31 sbc-sagara add end 一括Excel出力ボタン追加

//2017-04-30 sbc-sagara add str 一括登録ボタン追加
/*
 * 一括登録ボタンクリック時
 */
$('#dmyregist').click(function () {
    //ボタンクリック
    $('#registbutton').trigger('click');
});
//2017-04-30 sbc-sagara add end 一括登録ボタン追加
