import { tablet } from "../utils/device";
import DomHelpers from "../utils/domHelpers";

const pGap = 14;
const fGap = 4;
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

  if (
    itemOnMarker.type === "gap" &&
    itemOnMarker !== itemsAndGaps[itemsAndGaps.length - 1]
  )
    return itemOnMarker.end - marker + offset - wrapperPadding;
  if (
    itemOnMarker.type === "item" &&
    itemOnMarker.end - marker < 15 &&
    itemOnMarker !== itemsAndGaps[itemsAndGaps.length - 2]
  )
    return marker - itemOnMarker.end + offset * 2;
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

  textWidths.forEach((tw) => {
    if (!result.length)
      result.push(
        {
          type: "gap",
          length: pGap,
          start: 0,
          end: pGap + wrapperPadding,
        },
        {
          type: "item",
          length: tw,
          start: pGap,
          end: pGap + tw,
        }
      );
    else {
      const lastItem = result[result.length - 1];
      result.push(
        {
          type: "gap",
          length: pGap * 2 + fGap,
          start: lastItem.end,
          end: lastItem.end + pGap * 2 + fGap,
        },
        {
          type: "item",
          length: tw,
          start: lastItem.end + pGap * 2 + fGap,
          end: lastItem.end + pGap * 2 + fGap + tw,
        }
      );
    }
  });

  result.push({
    type: "gap",
    length: pGap,
    start: result[result.length - 1].end,
    end: result[result.length - 1].end + pGap + wrapperPadding,
  });

  return result;
};
