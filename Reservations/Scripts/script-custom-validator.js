$(document).ready(function () {
    console.log('hereererere');
    $('#rsvnForm').validate({
        
        errorClass: 'help-block animation-slideDown', // You can change the animation class for a different entrance animation - check animations page  
        errorElement: 'div',
        errorPlacement: function (error, e) {
            e.parents('.form-group > div').append(error);
        },
        highlight: function (e) {

            $(e).closest('.form-group').removeClass('has-success has-error').addClass('has-error');
            $(e).closest('.help-block').remove();
        },
        success: function (e) {
            e.closest('.form-group').removeClass('has-success has-error');
            e.closest('.help-block').remove();
        },
        rules: {
            'RoomID': {
                required: true
            },

            'Password': {
                required: true,
                minlength: 6
            },

            'ConfirmPassword': {
                required: true,
                equalTo: '#Password'
            }
        },
        messages: {
            'RoomID': 'Alexito',

            'Password': {
                required: 'Please provide a password',
                minlength: 'Your password must be at least 6 characters long'
            },

            'ConfirmPassword': {
                required: 'Please provide a password',
                minlength: 'Your password must be at least 6 characters long',
                equalTo: 'Please enter the same password as above'
            }
        }
    });
});