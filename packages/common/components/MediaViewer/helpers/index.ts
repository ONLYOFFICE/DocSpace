import type {
  ContextMenuModel,
  NullOrUndefined,
  PlaylistType,
  SeparatorType,
} from "../types";

export const mediaTypes = Object.freeze({
  audio: 1,
  video: 2,
  pdf: 3,
});

export enum KeyboardEventKeys {
  ArrowUp = "ArrowUp",
  ArrowDown = "ArrowDown",
  ArrowRight = "ArrowRight",
  ArrowLeft = "ArrowLeft",
  Escape = "Escape",
  Space = "Space",
  Delete = "Delete",
  KeyS = "KeyS",
  Numpad1 = "Numpad1",
  Digit1 = "Digit1",

  NumpadAdd = "NumpadAdd",
  NumpadSubtract = "NumpadSubtract",
  Equal = "Equal",
  Minus = "Minus",
}

export enum ToolbarActionType {
  Panel = 0,
  ZoomIn = 1,
  ZoomOut = 2,
  Prev = 3,
  Next = 4,
  RotateLeft = 5,
  RotateRight = 6,
  Reset = 7,
  Close = 8,
  ScaleX = 9,
  ScaleY = 10,
  Download = 11,
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
  ".pdf": { supply: "pdf", type: mediaTypes.pdf },
} as Record<string, { supply: string; type: number } | undefined>;

export function isVideo(fileExst: string): boolean {
  return mapSupplied[fileExst]?.type === mediaTypes.video;
}

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
  return arg?.isSeparator !== undefined && arg.isSeparator;
};

export const convertToTwoDigitString = (time: number): string => {
  return time < 10 ? `0${time}` : time.toString();
};

export const formatTime = (time: number): string => {
  if (isNullOrUndefined(time) || isNaN(time) || time <= 0) return "00:00";

  let seconds: number = Math.floor(time % 60);
  let minutes: number = Math.floor(time / 60) % 60;
  let hours: number = Math.floor(time / 3600);

  const convertedHours = convertToTwoDigitString(hours);
  const convertedMinutes = convertToTwoDigitString(minutes);
  const convertedSeconds = convertToTwoDigitString(seconds);

  if (hours === 0) {
    return `${convertedMinutes}:${convertedSeconds}`;
  }
  return `${convertedHours}:${convertedMinutes}:${convertedSeconds}`;
};

export const compareTo = (a: number, b: number) => {
  return Math.trunc(a) > Math.trunc(b);
};
