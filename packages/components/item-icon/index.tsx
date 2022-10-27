import React from "react";
import { ReactSVG } from "react-svg";

import * as Styled from "./index.styled";
import { getCustomRoomLogo, getDefaultRoomLogo } from "./helpers/getRoomLogo";

export interface ItemIconProps {
  item: {
    id: number;
    icon: string;
    logo?: {
      small: string;
      medium: string;
      large: string;
      original: string;
    };
    fileExst: string;
    isRoom: boolean;
    roomType: number;
    isPrivate?: boolean;
    isArchive?: boolean;
    [x: string]: any;
  };
  roomLogoSize?: "small" | "medium" | "large";
}

const ItemIcon = ({ item, roomLogoSize = "medium" }: ItemIconProps) => {
  const { icon, logo, isRoom, roomType, isArchive, isPrivate } = item;

  if (isRoom) {
    const roomLogo = !!logo?.original
      ? getCustomRoomLogo({ logo }, roomLogoSize)
      : getDefaultRoomLogo({ roomType, isArchive, isPrivate });

    return (
      <Styled.RoomIconWrapper
        className="room-logo_room_wrapper"
        size={roomLogoSize}
        isPrivate={isPrivate}
      >
        <ReactSVG className="room-logo_room_icon" src={roomLogo} />
      </Styled.RoomIconWrapper>
    );
  }

  return <Styled.FileIcon className={`react-svg-icon`} src={icon} />;
};

export default ItemIcon;

// export default inject(({ filesStore, treeFoldersStore }) => {
//   // const { type, extension, id } = filesStore.fileActionStore;

//   return {
//     viewAs: filesStore.viewAs,
//     // isPrivacy: treeFoldersStore.isPrivacyFolder,
//     // actionType: type,
//     // actionExtension: extension,
//     // actionId: id,
//   };
// })(observer(ItemIcon));
