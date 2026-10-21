/*
 * ログアウトボタンクリック時
 */
$('#dmylogin').click(function () {
    //ボタンクリック
    showLogout(false, 'loginbutton');
});

$(function () {
    var status = document.getElementById("ServerStatus").value;

    // 扶養控除等申告書ボタンとPDFボタンを取得
    var btnHuyouReg = document.querySelector('[name="Transition"][value="HuyouDeclareRegister"]');
    var btnHokenReg = document.querySelector('[name="Transition"][value="HokenDeclareRegister"]');
    var btnHaiguuReg = document.querySelector('[name="Transition"][value="HaiguuDeclareRegister"]');
    var btnHuyouPdf = document.querySelector('[name="PrintHuyou"][value="HuyouDeclareReport"]');
    var btnHokenPdf = document.querySelector('[name="PrintHoken"][value="HokenDeclareReport"]');
    var btnHaiguuPdf = document.querySelector('[name="PrintHaiguu"][value="HaiguuDeclareReport"]');
    //2025-12-19 iwai-matsuo add-str ------
    var btnJutakuReg = document.querySelector('[name="Transition"][value="JutakuDeclareRegister"]');
    var btnZenshokuReg = document.querySelector('[name="Transition"][value="ZenshokuDeclareRegister"]');
    //2025-12-19 iwai-matsuo add-end ------

    // Statusに基づいてボタンの使用可否を設定
    if (status === "1") {
        // [1:通常運用]すべてのボタンを有効にする
        btnHuyouReg.disabled = false;
        btnHokenReg.disabled = false;
        btnHaiguuReg.disabled = false;
        btnHuyouPdf.disabled = false;
        btnJutakuReg.disabled = false;
        btnHaiguuPdf.disabled = false;
        //2025-12-19 iwai-matsuo add-str ------
        btnJutakuReg.disabled = false;
        btnZenshokuReg.disabled = false;
        //2025-12-19 iwai-matsuo add-end ------
    } else if (status === "2") {
        // [2:通常運用]扶養控除と扶養控除のPDFボタンを有効にする
        btnHuyouReg.disabled = false;
        btnHokenReg.disabled = true;
        btnHaiguuReg.disabled = true;
        btnHuyouPdf.disabled = false;
        btnHokenPdf.disabled = true;
        btnHaiguuPdf.disabled = true;
        //2025-12-19 iwai-matsuo add-str ------
        btnJutakuReg.disabled = true;
        btnZenshokuReg.disabled = true;
        //2025-12-19 iwai-matsuo add-end ------
    } else if (status === "3") {
        // [3:通常運用]扶養控除の登録、PDFボタン、前職源泉のみ有効にする
        //2025-12-19 iwai-matsuo upd-str ------
        btnHuyouReg.disabled = false;
        //btnHuyouReg.disabled = true;
        //2025-12-19 iwai-matsuo upd-end ------
        btnHokenReg.disabled = true;
        btnHaiguuReg.disabled = true;
        btnHuyouPdf.disabled = false;
        btnHokenPdf.disabled = true;
        btnHaiguuPdf.disabled = true;
        //2025-12-19 iwai-matsuo add-str ------
        btnJutakuReg.disabled = true;
        btnZenshokuReg.disabled = false;
        //2025-12-19 iwai-matsuo add-end ------
    //2025-12-19 iwai-matsuo add-str ------
    } else if (status === "4") {
        // [4:通常運用]扶養控除のPDFボタンのみ有効にする
        btnHuyouReg.disabled = true;
        btnHokenReg.disabled = true;
        btnHaiguuReg.disabled = true;
        btnHuyouPdf.disabled = false;
        btnHokenPdf.disabled = true;
        btnHaiguuPdf.disabled = true;
        btnJutakuReg.disabled = true;
        btnZenshokuReg.disabled = true;
    //2025-12-19 iwai-matsuo add-end ------
    } else {
        // それ以外の場合、すべてのボタンを無効にする
        btnHuyouReg.disabled = true;
        btnHokenReg.disabled = true;
        btnHaiguuReg.disabled = true;
        btnHuyouPdf.disabled = true;
        btnHokenPdf.disabled = true;
        btnHaiguuPdf.disabled = true;
        //2025-12-19 iwai-matsuo add-str ------
        btnJutakuReg.disabled = true;
        btnZenshokuReg.disabled = true;
        //2025-12-19 iwai-matsuo add-end ------
    }
});

//2025-11-18 iwai-tamura upd-str ------
/*
 * 過去分PDF出力ボタンクリック時
 */
$('#DmyPrintHistory').click(function () {

    var comment = "";

    //値を取得
    var intHistoryYear = $('#HistoryYear').val();
    var strReportType = $('#HistoryPrintType').val();

    //対象処理入力チェック
    if (!intHistoryYear) {
        showAlert("エラー", "対象年度を選択してください。")
        return;
    }
    if (!strReportType) {
        showAlert("エラー", "対象帳票を選択してください。")
        return;
    }

    //確認メッセージ本文作成
    comment = '過去帳票の出力を実行しますか？'
    showMessage('確認', comment, 'PrintHistory', false);
});




// 要素の取得
const openHuyouAttachmentFileModalButton = document.getElementById('openHuyouAttachmentFileModalButton');
const closeHuyouAttachmentFileModalButton = document.getElementById('closeHuyouAttachmentFileModalButton');
const huyouAttachmentFileModal = document.getElementById('huyouAttachmentFileModal');
const huyouAttachmentFileModalOverlay = document.getElementById('huyouAttachmentFileModal-overlay');
const huyouAttachmentFilePath = document.getElementById('HuyouAttachmentFilePath');

// モーダルを開く処理
openHuyouAttachmentFileModalButton.addEventListener('click', () => {
    huyouAttachmentFileModalOverlay.style.display = 'block'; // オーバーレイを表示
    huyouAttachmentFileModal.style.display = 'block'; // モーダルを表示
});

// モーダルを閉じる処理
closeHuyouAttachmentFileModalButton.addEventListener('click', closeHuyouAttachmentFileModal);

// オーバーレイとモーダルを閉じる処理
function closeHuyouAttachmentFileModal() {
    huyouAttachmentFileModalOverlay.style.display = 'none'; // オーバーレイを非表示
    huyouAttachmentFileModal.style.display = 'none'; // モーダルを非表示
}


/*
 * アップロードボタンクリック時
 */
$('#DmyHuyouAttachmentFileUpload').click(function () {

    var comment = "";
    //値を取得
    var strUploadFile = $('#HuyouAttachmentUploadFile').val();

    //対象処理入力チェック
    if (!strUploadFile) {
        showAlert("エラー", "ファイルが選択されていません。")
        return;
    }

    //確認メッセージ本文作成
    comment = 'ファイルのアップロードを実行しますか？'
    showMessage('確認', comment, 'HuyouAttachmentFileUpload', true);

});

/*
 * ダウンロードボタンクリック時
 */
$('#DmyHuyouAttachmentFileDownload').click(function () {

    var comment = "";
    //値を取得
    var strFilePath = $('#HuyouAttachmentFilePath').val();

    //確認メッセージ本文作成
    comment = 'ファイルのダウンロードを実行しますか？'
    showMessage('確認', comment, 'HuyouAttachmentFileDownload', false);
});

/*
 * 削除ボタンクリック時
 */
$('#DmyHuyouAttachmentFileDelete').click(function () {

    var comment = "";
    //値を取得
    var strFilePath = $('#HuyouAttachmentFilePath').val();

    //対象処理入力チェック
    if (!strFilePath) {
        showAlert("エラー", "ファイルがありません")
        return;
    }

    //確認メッセージ本文作成
    comment = 'ファイルの削除を実行しますか？'
    showMessage('確認', comment, 'HuyouAttachmentFileDelete', true);

});
//2025-11-18 iwai-tamura upd-end ------

//2024-11-19 iwai-tamura upd-str ------
// 要素の取得
const openModalButton = document.getElementById('openCodeInputModalButton');
const closeModalButton = document.getElementById('closeModalButton');
const barcodeModal = document.getElementById('barcodeModal');
const modalOverlay = document.getElementById('modal-overlay');
const barcodeInput = document.getElementById('barcodeInput');
const specifyEmployeeNoInput = document.querySelector('input[name="specifyEmployeeNo"]');
const BarCodeTransitionButton = document.querySelector('button[name="BarCodeTransition"]');
const status = document.getElementById('status');

// 常に`input`にフォーカスが戻るようにする
document.addEventListener('click', () => {
    if (barcodeModal.style.display === 'block') {
        barcodeInput.focus(); // フォーカスを常にinputに戻す
    }
});

// モーダルを開く処理
openModalButton.addEventListener('click', () => {
    modalOverlay.style.display = 'block'; // オーバーレイを表示
    barcodeModal.style.display = 'block'; // モーダルを表示
    barcodeInput.focus(); // 入力フィールドにフォーカス
    status.textContent = 'スキャン待機中...';
});

// モーダルを閉じる処理
closeModalButton.addEventListener('click', closeModal);

// オーバーレイとモーダルを閉じる処理
function closeModal() {
    modalOverlay.style.display = 'none'; // オーバーレイを非表示
    barcodeModal.style.display = 'none'; // モーダルを非表示
    barcodeInput.value = ''; // 入力内容をクリア
}

// IMEを無効化して、常に英字入力に設定
barcodeInput.addEventListener('focus', () => {
    barcodeInput.setAttribute('inputmode', 'latin'); // 英字入力を指定
    barcodeInput.setAttribute('lang', 'en');         // 言語を英語に
    barcodeInput.focus(); // 再度フォーカスを当てる
});

// バーコードがスキャンされたときの処理
barcodeInput.addEventListener('keydown', function (event) {
    if (event.key === 'Enter') {  // Enterキーで処理を実行
        event.preventDefault(); // デフォルトのsubmitイベントを無効化

        const inputNumber = barcodeInput.value.trim();
        //inputNumber = convertToHalfWidth(inputNumber); // 全角を半角に変換

        if (inputNumber) {
           // ステータスメッセージを更新
            status.textContent = `社員番号: ${inputNumber.substring(inputNumber.length - 5) } 確認中...`;

           // 扶養控除等申告書ボタンのクリックイベントを発火
            BarCodeTransitionButton.value = inputNumber;
            BarCodeTransitionButton.click();

        }
    }
});
// フォームのsubmitイベントを制御
barcodeForm.addEventListener('submit', function (event) {
    // モーダルが表示されている間だけsubmitを無効化
    if (barcodeModal.style.display === 'block') {
        event.preventDefault(); // 送信を止める
        console.log('フォームの送信が無効化されました');
    } else {
        console.log('フォームが送信されました');
        // 通常の送信処理をここに追加する
    }
});


// 全角文字を半角に変換する関数
function convertToHalfWidth(str) {
    return str.replace(/[Ａ-Ｚａ-ｚ０-９]/g, function (s) {
        return String.fromCharCode(s.charCodeAt(0) - 0xFEE0);
    });
}
//2024-11-19 iwai-tamura upd-end ------


