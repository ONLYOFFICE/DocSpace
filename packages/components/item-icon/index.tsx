import React from "react";
import { ReactSVG } from "react-svg";

import * as Styled from "./index.styled";
import { getCustomRoomLogo, getDefaultRoomLogo } from "./helpers/getRoomLogo";

export interface ItemIconProps {
  item: {
    icon?: string;

    isRoom: boolean;
    roomType: number;
    isPrivate?: boolean;
    isArchive?: boolean;
    logo?: {
      small: string;
      medium: string;
      large: string;
      original: string;
    };
    [x: string]: any;
  };
  roomLogoSize?: "small" | "medium" | "large";
}

const ItemIcon = ({ item, roomLogoSize = "medium" }: ItemIconProps) => {
  const { icon, logo, isRoom, roomType, isArchive, isPrivate } = item;

  if (isRoom) {
    const isCustomLogo = !isArchive && !!(logo && logo[roomLogoSize]);
    const roomLogo = isCustomLogo
      ? getCustomRoomLogo({ logo }, roomLogoSize)
      : getDefaultRoomLogo({ roomType, isArchive, isPrivate });

    return (
      <Styled.RoomLogoWrapper
        className="item-icon_room_wrapper"
        size={roomLogoSize}
      >
        {isCustomLogo ? (
          <Styled.CustomRoomLogo
            className="item-icon_room_custom-logo"
            size={roomLogoSize}
            isPrivate={isPrivate}
          >
            <img className="" src={roomLogo} />
            <div className="item-icon_room_custom-logo_privacy-wrapper">
              {isPrivate && (
                <ReactSVG
                  className="item-icon_room_custom-logo_privacy"
                  src={"images/privacy.with-background.svg"}
                />
              )}
            </div>
          </Styled.CustomRoomLogo>
        ) : (
          <Styled.DefaultRoomLogo
            className="item-icon_room_default-lofo"
            size={roomLogoSize}
            isPrivate={isPrivate}
          >
            <ReactSVG src={roomLogo} />
          </Styled.DefaultRoomLogo>
        )}
      </Styled.RoomLogoWrapper>
    );
  }

  return <img className={`react-svg-icon`} src={icon} />;
};

export default ItemIcon;
