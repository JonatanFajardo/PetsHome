(function () {
    'use strict';

    var FORMATTER = new Intl.NumberFormat('en-US', {
        minimumFractionDigits: 2,
        maximumFractionDigits: 2
    });

    function format(raw) {
        if (raw === '' || raw === '-' || raw === null || raw === undefined) return '';
        var num = parseFloat(raw);
        if (isNaN(num)) return '';
        return FORMATTER.format(num);
    }

    function clean(value) {
        if (value === null || value === undefined) return '';
        return String(value).replace(/,/g, '').trim();
    }

    function attach(input) {
        if (input.dataset.moneyBound === '1') return;
        input.dataset.moneyBound = '1';

        if (input.value) {
            var initial = clean(input.value);
            if (initial !== '' && !isNaN(parseFloat(initial))) {
                input.value = format(initial);
            }
        }

        input.addEventListener('focus', function () {
            input.value = clean(input.value);
        });

        input.addEventListener('blur', function () {
            var raw = clean(input.value);
            input.value = raw === '' ? '' : format(raw);
        });

        input.addEventListener('input', function () {
            input.value = input.value.replace(/[^\d.,\-]/g, '');
        });

        var form = input.closest('form');
        if (form && !form.dataset.moneyFormBound) {
            form.dataset.moneyFormBound = '1';
            form.addEventListener('submit', function () {
                form.querySelectorAll('input.js-money').forEach(function (el) {
                    el.value = clean(el.value);
                });
            }, true);
        }
    }

    function init(root) {
        (root || document).querySelectorAll('input.js-money').forEach(attach);
    }

    if (window.jQuery && window.jQuery.validator) {
        var originalNumber = window.jQuery.validator.methods.number;
        window.jQuery.validator.methods.number = function (value, element) {
            if (element.classList && element.classList.contains('js-money')) {
                if (this.optional(element)) return true;
                return /^-?\d{1,3}(,\d{3})*(\.\d+)?$|^-?\d+(\.\d+)?$/.test(value);
            }
            return originalNumber.call(this, value, element);
        };
    }

    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', function () { init(); });
    } else {
        init();
    }

    window.MoneyInput = { init: init, attach: attach, clean: clean, format: format };
})();
