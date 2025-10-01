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

    // Statusに基づいてボタンの使用可否を設定
    if (status === "1") {
        // [1:通常運用]すべてのボタンを有効にする
        btnHuyouReg.disabled = false;
        btnHokenReg.disabled = false;
        btnHaiguuReg.disabled = false;
        btnHuyouPdf.disabled = false;
        btnHokenPdf.disabled = false;
        btnHaiguuPdf.disabled = false;
    } else if (status === "2") {
        // [2:通常運用]扶養控除と扶養控除のPDFボタンを有効にする
        btnHuyouReg.disabled = false;
        btnHokenReg.disabled = true;
        btnHaiguuReg.disabled = true;
        btnHuyouPdf.disabled = false;
        btnHokenPdf.disabled = true;
        btnHaiguuPdf.disabled = true;
    } else if (status === "3") {
        // [3:通常運用]扶養控除のPDFボタンのみ有効にする
        btnHuyouReg.disabled = true;
        btnHokenReg.disabled = true;
        btnHaiguuReg.disabled = true;
        btnHuyouPdf.disabled = false;
        btnHokenPdf.disabled = true;
        btnHaiguuPdf.disabled = true;
    } else {
        // それ以外の場合、すべてのボタンを無効にする
        btnHuyouReg.disabled = true;
        btnHokenReg.disabled = true;
        btnHaiguuReg.disabled = true;
        btnHuyouPdf.disabled = true;
        btnHokenPdf.disabled = true;
        btnHaiguuPdf.disabled = true;
    }


});

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


