function makeCssClassDraggable(draggableCssClass) {
    if (!draggableCssClass.startsWith("."))
        draggableCssClass = "." + draggableCssClass
    
    interact(draggableCssClass).draggable({
        inertia: true,
        listeners: {
            move(event) {
                let target = event.target
                let x = (parseFloat(target.getAttribute('data-x')) || 0) + event.dx
                let y = (parseFloat(target.getAttribute('data-y')) || 0) + event.dy
                
                target.style.transform = 'translate(' + x + 'px, ' + y + 'px)'
                
                target.setAttribute('data-x', x)
                target.setAttribute('data-y', y)
            }
        }
    })
    
    interact(draggableCssClass).styleCursor(false)
}