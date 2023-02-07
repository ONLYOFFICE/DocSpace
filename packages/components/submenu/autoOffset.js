import { tablet } from "../utils/device";
import DomHelpers from "../utils/domHelpers";

const paddingGap = 14;
const flexGap = 4;
const offset = 32;
const wrapperPadding = DomHelpers.getViewport() <= tablet ? 16 : 20;

export const countAutoOffset = (data, submenuItemsRef) => {
  const [marker, itemsAndGaps, itemOnMarker] = countParams(
    data,
    submenuItemsRef
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
    itemOnMarker.end - marker < 7.5 &&
    itemOnMarker !== itemsAndGaps[itemsAndGaps.length - 2]
  ) {
    return itemOnMarker.end - marker + offset * 2 - wrapperPadding;
  }
  return 0;
};

export const countAutoFocus = (itemId, data, submenuItemsRef) => {
  const [marker, itemsAndGaps, itemOnMarker] = countParams(
    data,
    submenuItemsRef
  );

  const [focusedItem] = itemsAndGaps.filter((obj) => obj.id === itemId);
  const submenuWidth = submenuItemsRef.current.offsetWidth;

  if (itemOnMarker?.id && focusedItem.id === itemOnMarker.id)
    return focusedItem.end - marker;
  if (
    focusedItem.start < marker - submenuWidth ||
    focusedItem.start - offset < marker - submenuWidth
  )
    return focusedItem.start - marker + submenuWidth - wrapperPadding - offset;
  return 0;
};

const countParams = (data, submenuItemsRef) => {
  const refCurrent = submenuItemsRef.current;

  const texts = data.map((d) => countText(d.name));
  const itemsAndGaps = countItemsAndGaps(texts);

  const submenuWidth = refCurrent.offsetWidth;
  const marker = refCurrent.scrollLeft + submenuWidth - wrapperPadding;

  const [itemOnMarker] = itemsAndGaps.filter(
    (obj) => obj.start < marker && marker < obj.end
  );

  return [marker, itemsAndGaps, itemOnMarker];
};

const countText = (text) => {
  const inputText = text;
  const font = "600 13px open sans";
  const canvas = document.createElement("canvas");
  const context = canvas.getContext("2d");
  context.font = font;
  return { id: text, width: context.measureText(inputText).width };
};

const countItemsAndGaps = (texts) => {
  const result = [];

  texts.forEach(({ id, width }) => {
    if (!result.length)
      result.push(
        {
          type: "gap",
          length: paddingGap,
          start: 0,
          end: paddingGap + wrapperPadding,
        },
        {
          id: id,
          type: "item",
          length: width,
          start: paddingGap,
          end: paddingGap + width,
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
          id: id,
          type: "item",
          length: width,
          start: lastItem.end + paddingGap * 2 + flexGap,
          end: lastItem.end + paddingGap * 2 + flexGap + width,
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
