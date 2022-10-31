import { RoomsType } from "@docspace/common/constants";

export const getDefaultRoomLogo = (item: {
  roomType: number;
  isArchive: undefined | boolean;
  isPrivate: undefined | boolean;
}) => {
  const { roomType, isArchive, isPrivate } = item;

  if (isArchive) {
    return isPrivate
      ? "/static/images/room.privacy.archive.svg"
      : "/static/images/room.with-border.archive.svg";
  }

  switch (roomType) {
    case RoomsType.FillingFormsRoom:
      return isPrivate
        ? "/static/images/room.privacy.fill.svg"
        : "/static/images/room.with-border.fill.svg";
    case RoomsType.EditingRoom:
      return isPrivate
        ? "/static/images/room.privacy.editing.svg"
        : "/static/images/room.with-border.editing.svg";
    case RoomsType.ReviewRoom:
      return isPrivate
        ? "/static/images/room.privacy.review.svg"
        : "/static/images/room.with-border.review.svg";
    case RoomsType.ReadOnlyRoom:
      return isPrivate
        ? "/static/images/room.privacy.view.svg"
        : "/static/images/room.with-border.view.svg";
    case RoomsType.CustomRoom:
      return isPrivate
        ? "/static/images/room.privacy.custom.svg"
        : "/static/images/room.with-border.custom.svg";
    default:
      return isPrivate
        ? "/static/images/room.privacy.custom.svg"
        : "/static/images/room.with-border.custom.svg";
  }
};

export const getCustomRoomLogo = (
  item: {
    logo: {
      small: string | "";
      medium: string | "";
      large: string | "";
    };
  },
  size: "small" | "medium" | "large"
) => {
  const { small, medium, large } = item.logo;

  switch (size) {
    case "small":
      return small;
    case "medium":
      return medium;
    case "large":
      return large;
  }
};
