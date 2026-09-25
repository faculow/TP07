function darMeGusta(publicacionId) {
    fetch(`/Home/DarMeGusta?id=${publicacionId}`, {
        method: 'POST'
    })
    .then(response => {
        if (!response.ok) throw new Error('Error al dar me gusta');
        return response.json();
    })
    .then(data => {
        document.getElementById(`me-gusta-${publicacionId}`).textContent = data.meGusta;
    })
    .catch(error => console.error('Error:', error));
}

function comentar(publicacionId) {
    const input = document.getElementById(`comentario-${publicacionId}`);
    const contenido = input.value;

    if (!contenido.trim()) return;

    fetch(`/Home/Comentar?id=${publicacionId}&contenido=${encodeURIComponent(contenido)}`, {
        method: 'POST'
    })
    .then(response => {
        if (!response.ok) throw new Error('Error al comentar');
        return response.json();
    })
    .then(data => {
        const listaComentarios = document.getElementById(`comentarios-${publicacionId}`);
        listaComentarios.innerHTML += `<li>${data.contenido}</li>`;
        input.value = '';
    })
    .catch(error => console.error('Error:', error));
}

let paginaActual = 1;
const limite = 10;

function cargarMasPublicaciones() {
    const siguientePagina = paginaActual + 1;
    const btnVerMas = document.getElementById('btn-ver-mas');

    fetch(`/Home/ObtenerPublicaciones?pagina=${siguientePagina}`)
    .then(response => {
        if (!response.ok) throw new Error('Error al obtener publicaciones');
        return response.json();
    })
    .then(publicaciones => {
        const contenedor = document.getElementById('contenedor-publicaciones');

        if (publicaciones && publicaciones.length > 0) {
            publicaciones.forEach(pub => {
                const id = pub.id || pub.Id;
                const titulo = pub.titulo || pub.Titulo;
                const descripcion = pub.descripcion || pub.Descripcion;
                const meGusta = pub.meGusta !== undefined ? pub.meGusta : (pub.MeGusta || 0);

                const article = document.createElement('article');
                article.classList.add('publicacion');
                article.style.cssText = "border: 1px solid #ccc; margin-bottom: 15px; padding: 10px;";
                article.innerHTML = `
                    <h3>${titulo}</h3>
                    <p>${descripcion}</p>
                    <div style="margin-top: 10px;">
                        <span id="me-gusta-${id}">${meGusta}</span> Me gusta
                        <button type="button" onclick="darMeGusta(${id})">Me gusta</button>
                    </div>
                    <div style="margin-top: 10px;">
                        <ul id="comentarios-${id}"></ul>
                        <input type="text" id="comentario-${id}" placeholder="Escribe un comentario...">
                        <button type="button" onclick="comentar(${id})">Comentar</button>
                    </div>
                `;
                contenedor.appendChild(article);
            });

            paginaActual = siguientePagina;
        }

        if (!publicaciones || publicaciones.length < limite) {
            if (btnVerMas) btnVerMas.style.display = 'none';
        }
    })
    .catch(error => console.error('Error al cargar más publicaciones:', error));
}