export const generateLogo = (width, height, text, fontSize = 18) => {
  const canvas = document.createElement("canvas");
  canvas.width = width;
  canvas.height = height;
  const ctx = canvas.getContext("2d");
  ctx.fillStyle = "transparent";
  ctx.clearRect(0, 0, width, height);
  ctx.fillStyle = "#000";
  ctx.textAlign = "start";
  ctx.textBaseline = "top";
  ctx.font = `${fontSize}px Arial`;
  ctx.fillText(text, 0, 0);
  return canvas.toDataURL();
};

export const getLogoOptions = (index, text) => {
  switch (index) {
    case 0:
      return { fontSize: 18, text: text };
    case 1:
      return { fontSize: 30, text: text };
    case 2:
      return { fontSize: 16, text: text.trim().charAt(0) };
    case 3:
      return { fontSize: 12, text: text };
    case 4:
      return { fontSize: 12, text: text };
    case 5:
      return { fontSize: 30, text: text.trim().charAt(0) };
    case 6:
      return { fontSize: 30, text: text };
    default:
      return { fontSize: 18, text: text };
  }
};
