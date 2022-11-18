export const generateLogo = (width, height, text, fill = "#000") => {
  const canvas = document.createElement("canvas");
  canvas.width = width;
  canvas.height = height;
  const ctx = canvas.getContext("2d");
  ctx.fillStyle = "transparent";
  ctx.clearRect(0, 0, width, height);
  ctx.fillStyle = fill;
  ctx.textAlign = "start";
  ctx.textBaseline = "top";
  ctx.font = `18px Arial`;
  ctx.fillText(text, 0, 0);
  return canvas.toDataURL();
};
