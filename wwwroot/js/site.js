function darMeGusta(publicacionId) {
    const btn = document.getElementById(`btn-like-${publicacionId}`);
    const span = document.getElementById(`me-gusta-${publicacionId}`);

    fetch(`/Home/DarMeGusta?id=${publicacionId}`, {
        method: 'POST'
    })
    .then(response => {
        if (!response.ok) throw new Error('Error en el servidor');
        return response.json();
    })
    .then(data => {
        if (span) span.textContent = data.meGusta;

        if (btn) {
            btn.classList.toggle('active');
        }
    })
    .catch(error => console.error('Error al dar me gusta:', error));
}

function comentar(publicacionId) {
    const input = document.getElementById(`comentario-${publicacionId}`);
    if (!input) return;

    const contenido = input.value.trim();
    if (!contenido) return;

    fetch(`/Home/Comentar?id=${publicacionId}&contenido=${encodeURIComponent(contenido)}`, {
        method: 'POST'
    })
    .then(response => {
        if (!response.ok) throw new Error('Error al enviar comentario');
        return response.json();
    })
    .then(data => {
        const listaComentarios = document.getElementById(`comentarios-${publicacionId}`);
        if (listaComentarios) {
            listaComentarios.innerHTML += `<li>${data.contenido}</li>`;
        }
        input.value = ''; // Limpia la caja de texto
    })
    .catch(error => console.error('Error al comentar:', error));
}

let paginaActual = 1;

function cargarMasPublicaciones() {
    paginaActual++;

    fetch(`/Home/ObtenerPublicaciones?pagina=${paginaActual}`)
    .then(response => {
        if (!response.ok) throw new Error('Error al cargar publicaciones');
        return response.json();
    })
    .then(publicaciones => {
        const contenedor = document.getElementById('contenedor-publicaciones');

        if (!publicaciones || publicaciones.length === 0) {
            const btnVerMas = document.getElementById('btn-ver-mas');
            if (btnVerMas) btnVerMas.style.display = 'none';
            return;
        }

        publicaciones.forEach(pub => {
            // Renderiza la imagen si existe
            const HTMLImagen = pub.imagen ? `<img src="${pub.imagen}" alt="Imagen publicación" style="max-width: 300px;" />` : '';

            let HTMLComentarios = '';
            if (pub.comentarios && pub.comentarios.length > 0) {
                pub.comentarios.forEach(com => {
                    HTMLComentarios += `<li>${com.texto}</li>`;
                });
            }

            const postHtml = `
                <article class="publicacion" style="border: 1px solid #ccc; margin-bottom: 15px; padding: 10px;">
                    <h3>${pub.titulo}</h3>
                    <p>${pub.descripcion}</p>
                    ${HTMLImagen}

                    <div style="margin-top: 10px;">
                        <span id="me-gusta-${pub.id}">${pub.cantidadLikes}</span> Me gusta
                        <button id="btn-like-${pub.id}" 
                                type="button" 
                                class="btn-like ${pub.leDioLike ? 'active' : ''}" 
                                onclick="darMeGusta(${pub.id})">
                            ♥ Me gusta
                        </button>
                    </div>

                    <div style="margin-top: 10px;">
                        <ul id="comentarios-${pub.id}">
                            ${HTMLComentarios}
                        </ul>
                        <input type="text" id="comentario-${pub.id}" placeholder="Escribe un comentario..." />
                        <button type="button" onclick="comentar(${pub.id})">Comentar</button>
                    </div>
                </article>
            `;

            contenedor.insertAdjacentHTML('beforeend', postHtml);
        });
    })
    .catch(error => console.error('Error al cargar más publicaciones:', error));
}