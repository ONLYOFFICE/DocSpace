import styled from "styled-components";
import { Base } from "@docspace/components/themes";

import {
  getSizeInPxPrivate,
  getSizeInPx,
  getPrivateIconCss,
  getBorderRadiusInPx,
} from "./helpers/getRoomLogoSize";

export const RoomLogoWrapper = styled.div<{
  size: "small" | "medium" | "large";
}>`
  height: ${({ size }) => getSizeInPx(size)};
  width: ${({ size }) => getSizeInPx(size)};
`;

export const DefaultRoomLogo = styled.div<{
  size: "small" | "medium" | "large";
  isPrivate: boolean | undefined;
}>`
  svg {
    height: ${({ size, isPrivate }) =>
      isPrivate ? getSizeInPxPrivate(size) : getSizeInPx(size)};
    width: ${({ size, isPrivate }) =>
      isPrivate ? getSizeInPxPrivate(size) : getSizeInPx(size)};
  }
`;

export const CustomRoomLogo = styled.div<{
  theme: any;
  size: "small" | "medium" | "large";
}>`
  height: ${({ size }) => getSizeInPx(size)};
  width: ${({ size }) => getSizeInPx(size)};

  img {
    border-radius: ${({ size }) => getBorderRadiusInPx(size)};
    outline: 1px solid
      ${({ theme }) => theme.filesSection.tilesView.tile.roomLogoBorderColor};
    height: ${({ size }) => getSizeInPx(size)};
    width: ${({ size }) => getSizeInPx(size)};
  }

  .room-logo_room_custom-icon_privacy {
    position: relative;
    margin: 0;
    ${({ size }) => getPrivateIconCss(size)};

    svg {
      width: 100%;
      height: 100%;
      g {
        path:first-child {
          fill: ${({ theme }) =>
            theme.filesSection.tilesView.tile.roomLogoPrivacyBgColor};
        }
      }
    }
  }
`;

CustomRoomLogo.defaultProps = {
  theme: Base,
};
