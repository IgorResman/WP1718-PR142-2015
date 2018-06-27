
function validateLogin() {
    $("#loginForm").validate({
        rules: {
            korisnickoIme: {
                required: true,
                minlength: 4
            },
            lozinka: {
                required: true,
                minlength: 5
            }
        },
        messages: {
            korisnickoIme: {
                required: "Morate uneti ovo polje",
                minlength: "Korisnicko ime mora biti minimum 4 karaktera"
            },
            lozinka: {
                required: "Morate uneti ovo polje",
                minlength: "Lozinka mora biti minimum 4 karaktera"
            }
        },
    });
}