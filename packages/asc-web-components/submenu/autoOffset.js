import { tablet } from "../utils/device";
import DomHelpers from "../utils/domHelpers";

const paddingGap = 14;
const flexGap = 4;
const offset = 32;
const wrapperPadding = DomHelpers.getViewport() <= tablet ? 16 : 20;

export const countAutoOffset = (data, submenuItemsRef) => {
  const refCurrent = submenuItemsRef.current;

  const textWidths = data.map((d) => countTextWidth(d.name));
  const itemsAndGaps = countItemsAndGaps(textWidths);

  const submenuWidth = refCurrent.offsetWidth;
  const marker = refCurrent.scrollLeft + submenuWidth - wrapperPadding;

  const [itemOnMarker] = itemsAndGaps.filter(
    (obj) => obj.start < marker && marker < obj.end
  );

  if (itemOnMarker === undefined) return 0;
  if (
    itemOnMarker.type === "gap" &&
    itemOnMarker !== itemsAndGaps[itemsAndGaps.length - 1]
  )
    return itemOnMarker.end - marker + offset - wrapperPadding;
  if (itemOnMarker.type === "item" && marker - itemOnMarker.start < 32) {
    return -(marker - itemOnMarker.start - offset) - wrapperPadding;
  }
  if (
    itemOnMarker.type === "item" &&
    itemOnMarker.end - marker < 10 &&
    itemOnMarker !== itemsAndGaps[itemsAndGaps.length - 2]
  ) {
    return itemOnMarker.end - marker + offset * 2 - wrapperPadding;
  }
  return 0;
};

const countTextWidth = (text) => {
  const inputText = text;
  const font = "600 13px open sans";
  const canvas = document.createElement("canvas");
  const context = canvas.getContext("2d");
  context.font = font;
  return context.measureText(inputText).width;
};

const countItemsAndGaps = (textWidths) => {
  const result = [];

  textWidths.forEach((textWidth) => {
    if (!result.length)
      result.push(
        {
          type: "gap",
          length: paddingGap,
          start: 0,
          end: paddingGap + wrapperPadding,
        },
        {
          type: "item",
          length: textWidth,
          start: paddingGap,
          end: paddingGap + textWidth,
        }
      );
    else {
      const lastItem = result[result.length - 1];
      result.push(
        {
          type: "gap",
          length: paddingGap * 2 + flexGap,
          start: lastItem.end,
          end: lastItem.end + paddingGap * 2 + flexGap,
        },
        {
          type: "item",
          length: textWidth,
          start: lastItem.end + paddingGap * 2 + flexGap,
          end: lastItem.end + paddingGap * 2 + flexGap + textWidth,
        }
      );
    }
  });

  result.push({
    type: "gap",
    length: paddingGap,
    start: result[result.length - 1].end,
    end: result[result.length - 1].end + paddingGap + wrapperPadding,
  });

  return result;
};
