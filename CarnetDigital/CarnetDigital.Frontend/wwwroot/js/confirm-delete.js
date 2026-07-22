// Reutilizable por Areas/Carreras/Instituciones: llena el modal de confirmación y apunta el form de borrado al elemento clickeado.
document.addEventListener("DOMContentLoaded", function () {
    var modalEl = document.getElementById("deleteModal");
    if (!modalEl) return;

    var deleteForm = document.getElementById("deleteForm");
    var deleteModalLabel = document.getElementById("deleteModalNombre");

    modalEl.addEventListener("show.bs.modal", function (event) {
        var trigger = event.relatedTarget;
        var deleteUrl = trigger.getAttribute("data-delete-url");
        var nombre = trigger.getAttribute("data-nombre");

        deleteForm.setAttribute("action", deleteUrl);
        if (deleteModalLabel) {
            deleteModalLabel.textContent = nombre || "";
        }
    });
});
