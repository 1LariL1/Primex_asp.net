let 
    btnReg = document.querySelector('.form__reg'), //присваивание переменных классам стиля
    btnReg2 = document.querySelector('.login__reg'),
    btnLog = document.querySelector('.form__log'),
    btnLog2 = document.querySelector('.reg__login'),
    reg = document.querySelector('.heading__reg'),
    log = document.querySelector('.heading__login');
    btnOpen = document.querySelector('.li-button');


function showReg() { //функция показать блок
    log.classList.add('b__show'); //при нажатии на кнопку добавляется новый класс
    reg.classList.add('b__show-two'); //при нажатии на кнопку убирается класс
}

function showReg2() { //функция показать блок
    log.classList.add('b__show'); //при нажатии на кнопку добавляется новый класс
    reg.classList.add('b__show-two'); //при нажатии на кнопку убирается класс
}

function showLog() { //функция показать блок
    reg.classList.remove('b__show-two'); //при нажатии на кнопку убирается класс
    log.classList.remove('b__show'); //при нажатии на кнопку добавляется новый класс
}

function showLog2() { //функция показать блок
    reg.classList.remove('b__show-two'); //при нажатии на кнопку убирается класс
    log.classList.remove('b__show'); //при нажатии на кнопку добавляется новый класс
}


btnReg.addEventListener('click', showReg);              //далее: функции срабатывают при нажатии на определенные кнопки
btnReg2.addEventListener('click', showReg2);
btnLog.addEventListener('click', showLog);
btnLog2.addEventListener('click', showLog2);
console.log(btnReg) 
console.log(btnReg2) 
console.log(btnLog) 
console.log(btnLog2) 
