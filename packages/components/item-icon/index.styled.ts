import styled from "styled-components";

const getSizeInPx = (size: "small" | "medium" | "large") => {
  switch (size) {
    case "small":
      return "16px";
    case "medium":
      return "32px";
    case "large":
      return "96px";
  }
};

const getPrivateRoomLogoSizeInPx = (size: "small" | "medium" | "large") => {
  switch (size) {
    case "small":
      return "17px";
    case "medium":
      return "34px";
    case "large":
      return "102px";
  }
};

export const RoomIconWrapper = styled.div<{
  size: "small" | "medium" | "large";
  isPrivate: boolean | undefined;
}>`
  height: ${({ size }) => getSizeInPx(size)};
  width: ${({ size }) => getSizeInPx(size)};

  svg {
    height: ${({ size, isPrivate }) =>
      isPrivate ? getPrivateRoomLogoSizeInPx(size) : getSizeInPx(size)};
    width: ${({ size, isPrivate }) =>
      isPrivate ? getPrivateRoomLogoSizeInPx(size) : getSizeInPx(size)};
  }
`;

export const FileIcon = styled.img``;
