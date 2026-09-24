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
//Usando fetch necesito que se pueda dar me gusta en las publicaciones de un blog. Cuando el usuario haga clic en el botón de "Me gusta", se debe enviar una solicitud POST al servidor para registrar el "Me gusta" y actualizar el contador de "Me gusta" en la interfaz de usuario sin recargar la página.

function darMeGusta(publicacionId) {
    fetch(`/publicaciones/${publicacionId}/me-gusta`, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        }
    })
    .then(response => response.json())
    .then(data => {
        // Actualizar el contador de "Me gusta" en la interfaz de usuario
        document.getElementById(`me-gusta-${publicacionId}`).textContent = data.meGusta;
    })
    .catch(error => {
        console.error('Error al dar "Me gusta":', error);
    });
}

//Funcion que perimite comentar una publicacion de un blog, al hacer clic en el boton de "Comentar" se debe enviar una solicitud POST al servidor para registrar el comentario y actualizar la lista de comentarios en la interfaz de usuario sin recargar la página.

function comentar(publicacionId) {
    const comentario = document.getElementById(`comentario-${publicacionId}`).value;
    fetch(`/publicaciones/${publicacionId}/comentarios`, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify({ contenido: comentario })
    })
    .then(response => response.json())
    .then(data => {
        // Actualizar la lista de comentarios en la interfaz de usuario
        const listaComentarios = document.getElementById(`comentarios-${publicacionId}`);
        listaComentarios.innerHTML += `<li>${data.contenido}</li>`;
        // Limpiar el campo de texto
        document.getElementById(`comentario-${publicacionId}`).value = '';
    })
    .catch(error => {
        console.error('Error al comentar:', error);
    });
}
