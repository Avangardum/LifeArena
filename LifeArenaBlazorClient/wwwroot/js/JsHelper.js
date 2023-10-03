function makeLifeArenaFieldDraggable(targetClass, lifeArenaBodyComponent) {
    if (!targetClass.startsWith("."))
        targetClass = "." + targetClass
    
    interact(targetClass).draggable({
        inertia: true,
        listeners: {
            move(event) {
                let target = event.target
                let x = (parseFloat(target.getAttribute('field-translate-x')) || 0) + event.dx
                let y = (parseFloat(target.getAttribute('field-translate-y')) || 0) + event.dy
                
                lifeArenaBodyComponent.invokeMethod("SetFieldTranslate", x, y)
            }
        }
    })
    
    interact(targetClass).styleCursor(false)
}

function getBoundingClientRect(elementId) {
    return document.getElementById(elementId).getBoundingClientRect()
}