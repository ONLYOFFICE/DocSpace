export const generateLogo = (
  width,
  height,
  text,
  fontSize = 18,
  fontColor = "#000",
  isEditorLogo = false
) => {
  const canvas = document.createElement("canvas");
  canvas.width = isEditorLogo ? "154" : width;
  canvas.height = isEditorLogo ? "27" : height;
  const ctx = canvas.getContext("2d");
  ctx.fillStyle = "transparent";
  ctx.clearRect(0, 0, width, height);
  ctx.fillStyle = fontColor;
  ctx.textAlign = "start";
  ctx.textBaseline = "top";
  ctx.font = `${fontSize}px Arial`;
  ctx.fillText(text, 0, height / 2 - fontSize / 2);

  return canvas.toDataURL();
};

export const getLogoOptions = (index, text) => {
  switch (index) {
    case 0:
      return { fontSize: 18, text: text };
    case 1:
      return { fontSize: 44, text: text };
    case 2:
      return { fontSize: 16, text: text.trim().charAt(0) };
    case 3:
      return { fontSize: 16, text: text, isEditorLogo: true };
    case 4:
      return { fontSize: 16, text: text, isEditorLogo: true };
    case 5:
      return { fontSize: 30, text: text.trim().charAt(0) };
    case 6:
      return { fontSize: 32, text: text };
    default:
      return { fontSize: 18, text: text };
  }
};
