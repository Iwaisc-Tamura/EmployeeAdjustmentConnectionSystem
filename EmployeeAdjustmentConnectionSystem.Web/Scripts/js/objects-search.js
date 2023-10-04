/*
 * 全選択・全解除をクリック時
 */
$('.allCheck input').click(function () {
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

/*
 * 本人入力ボタンクリック時
 */
$('#dmyselfinput').click(function () {
  //ローディングパネル表示
  //showLoading();
  //ボタンクリック
  $('#selfinputbutton').trigger('click');
});

/*
 * 帳票出力ボタンクリック時
 */
$('#dmyprint').click(function () {
  showMessage('目標管理 帳票出力', '出力しますか？', 'printbutton', false);
});

//2022-04-28 iwai-tamura upd-str ---
/*
 * 業務申告帳票出力ボタンクリック時
 */
$('#dmyprintduties').click(function () {
    showMessage('業務申告書 帳票出力', '出力しますか？', 'dutiesprintbutton', false);
});
//2022-04-28 iwai-tamura upd-end ---



/*
 * 部下表示ボタンクリック時
 */
$('#dmysubview').click(function () {
  //ローディングパネル表示
  showLoading();
  //ボタンクリック
  $('#subviewbutton').trigger('click');
});

/*
 * 一括承認ボタンクリック時
 */
$('#dmysign').click(function () {
  //ボタンクリック
  showMessage('一括承認', '承認しますか？', 'signbutton', true);
});

//2017-03-31 sbc-sagara add str 一括Excel出力ボタン追加
// 
/*
 * 一括Excelボタンクリック時
 */
$('#dmyprintxls').click(function () {
    showMessage('Excel出力', '出力しますか？', 'printxlsbutton', false);
});
//2017-03-31 sbc-sagara add end 一括Excel出力ボタン追加
