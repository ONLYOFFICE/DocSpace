import {
  ContextMenuModel,
  NullOrUndefined,
  PlaylistType,
  SeparatorType,
} from "../types";

export const mediaTypes = Object.freeze({
  audio: 1,
  video: 2,
});

export enum KeyboardEventKeys {
  ArrowRight = "ArrowRight",
  ArrowLeft = "ArrowLeft",
  Escape = "Escape",
  Space = "Space",
  Delete = "Delete",
  KeyS = "KeyS",
  Numpad1 = "Numpad1",
  Digit1 = "Digit1",
}

export const mapSupplied = {
  ".aac": { supply: "m4a", type: mediaTypes.audio },
  ".flac": { supply: "mp3", type: mediaTypes.audio },
  ".m4a": { supply: "m4a", type: mediaTypes.audio },
  ".mp3": { supply: "mp3", type: mediaTypes.audio },
  ".oga": { supply: "oga", type: mediaTypes.audio },
  ".ogg": { supply: "oga", type: mediaTypes.audio },
  ".wav": { supply: "wav", type: mediaTypes.audio },

  ".f4v": { supply: "m4v", type: mediaTypes.video },
  ".m4v": { supply: "m4v", type: mediaTypes.video },
  ".mov": { supply: "m4v", type: mediaTypes.video },
  ".mp4": { supply: "m4v", type: mediaTypes.video },
  ".ogv": { supply: "ogv", type: mediaTypes.video },
  ".webm": { supply: "webmv", type: mediaTypes.video },
  ".wmv": { supply: "m4v", type: mediaTypes.video, convertable: true },
  ".avi": { supply: "m4v", type: mediaTypes.video, convertable: true },
  ".mpeg": { supply: "m4v", type: mediaTypes.video, convertable: true },
  ".mpg": { supply: "m4v", type: mediaTypes.video, convertable: true },
} as Record<string, { supply: string; type: number } | undefined>;

export const isNullOrUndefined = (arg: unknown): arg is NullOrUndefined => {
  return arg === undefined || arg === null;
};

export const findNearestIndex = (
  items: PlaylistType[],
  index: number
): number => {
  if (!Array.isArray(items) || items.length === 0 || index < 0) {
    return -1;
  }

  let found = items[0].id;
  for (const item of items) {
    if (Math.abs(item.id - index) < Math.abs(found - index)) {
      found = item.id;
    }
  }
  return found;
};

export const isSeparator = (arg: ContextMenuModel): arg is SeparatorType => {
  return arg?.isSeparator;
};
