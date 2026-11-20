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
 * 部下表示ボタンクリック時
 */
$('#dmysubview').click(function () {
  //ローディングパネル表示
  showLoading();
  //ボタンクリック
  $('#subviewbutton').trigger('click');
});
