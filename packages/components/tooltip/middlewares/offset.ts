import { evaluate, getAlignment, getSide, getSideAxis } from "../utils";

import type { Coords, Derivable, Middleware, MiddlewareState } from "./types";

type OffsetValue =
  | number
  | Partial<{
      mainAxis: number;
      crossAxis: number;
      alignmentAxis: number | null;
    }>;

export type OffsetOptions = OffsetValue | Derivable<OffsetValue>;

export async function convertValueToCoords(
  state: MiddlewareState,
  options: OffsetOptions
): Promise<Coords> {
  const { placement, platform, elements } = state;
  const rtl = await platform.isRTL?.(elements.floating);

  const side = getSide(placement);
  const alignment = getAlignment(placement);
  const isVertical = getSideAxis(placement) === "y";
  const mainAxisMulti = ["left", "top"].includes(side) ? -1 : 1;
  const crossAxisMulti = rtl && isVertical ? -1 : 1;
  const rawValue = evaluate(options, state);

  // eslint-disable-next-line prefer-const
  let { mainAxis, crossAxis, alignmentAxis } =
    typeof rawValue === "number"
      ? { mainAxis: rawValue, crossAxis: 0, alignmentAxis: null }
      : { mainAxis: 0, crossAxis: 0, alignmentAxis: null, ...rawValue };

  if (alignment && typeof alignmentAxis === "number") {
    crossAxis = alignment === "end" ? alignmentAxis * -1 : alignmentAxis;
  }

  return isVertical
    ? { x: crossAxis * crossAxisMulti, y: mainAxis * mainAxisMulti }
    : { x: mainAxis * mainAxisMulti, y: crossAxis * crossAxisMulti };
}

export const offset = (options: OffsetOptions = 0): Middleware => ({
  name: "offset",
  options,
  async fn(state) {
    const { x, y } = state;
    const diffCoords = await convertValueToCoords(state, options);

    return {
      x: x + diffCoords.x,
      y: y + diffCoords.y,
      data: diffCoords,
    };
  },
});
