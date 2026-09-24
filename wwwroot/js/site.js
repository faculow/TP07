function validarFormulario() {
    const nombre = document.getElementById('nombre').value;
    const apellido = document.getElementById('apellido').value;
    const usuario = document.getElementById('usuario').value;
    const contrasena = document.getElementById('contrasena').value;

    document.getElementById('err-nombre').innerHTML = '';
    document.getElementById('err-apellido').innerHTML = '';
    document.getElementById('err-usuario').innerHTML = '';
    document.getElementById('err-contrasena').innerHTML = '';

    let ok = true;

    if (nombre.length < 2) {
        document.getElementById('err-nombre').innerHTML = 'Ingrese un nombre válido.';
        ok = false;
    }

    if (apellido.length < 2) {
        document.getElementById('err-apellido').innerHTML = 'Ingrese un apellido válido.';
        ok = false;
    }

    if (usuario.length < 4) {
        document.getElementById('err-usuario').innerHTML = 'El usuario debe tener al menos 4 caracteres.';
        ok = false;
    }

    if (contrasena.length < 6) {
        document.getElementById('err-contrasena').innerHTML = 'La contraseña debe tener al menos 6 caracteres.';
        ok = false;
    }


    if (ok) {
        return true;
    }

    return false;
}