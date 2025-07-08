window.getBoundingClientRectById = (id) => {
    const el = document.getElementById(id);
    if (!el) return null;

    const rect = el.getBoundingClientRect();
    return {
        Left: rect.left,
        Top: rect.top,
        Width: rect.width,
        Height: rect.height,
        WindowWidth: window.innerWidth,
        WindowHeight: window.innerHeight
    };
};