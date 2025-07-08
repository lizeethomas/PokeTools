function dragAndDrop(className) {
    interact(className).draggable({
        listeners: {
            start(event) {

                event.target.dataset.x = event.target.dataset.x || 0;
                event.target.dataset.y = event.target.dataset.y || 0;
            },
            move(event) {
                const target = event.target;

                let x = (parseFloat(target.dataset.x) || 0) + event.dx;
                let y = (parseFloat(target.dataset.y) || 0) + event.dy;

                target.style.transform = `translate(${x}px, ${y}px)`;

                target.dataset.x = x;
                target.dataset.y = y;
            },
        },
        modifiers: [
            interact.modifiers.restrictRect({
                restriction: '.main',
                endOnly: true
            })
        ]
    })
}


function dropZone(dropTarget) {
    interact(dropTarget)
        .dropzone({
            ondrop: function (event) {
                console.log("dropped");

                const id = event.relatedTarget.id;
            }
        })
        .on('dropactivate', function (event) {
            event.target.classList.add('drop-activated')
        })
};