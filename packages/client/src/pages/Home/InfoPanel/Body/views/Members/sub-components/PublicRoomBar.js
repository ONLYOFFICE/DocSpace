import React from "react";
import { ReactSVG } from "react-svg";
import PeopleIcon from "PUBLIC_DIR/images/people.react.svg?url";
import CrossReactSvg from "PUBLIC_DIR/images/cross.react.svg?url";
import IconButton from "@docspace/components/icon-button";
import { StyledPublicRoomBar } from "./StyledPublicRoom";
import Text from "@docspace/components/text";

const PublicRoomBar = (props) => {
  const { headerText, bodyText, iconName, onClose, ...rest } = props;

  return (
    <StyledPublicRoomBar {...rest}>
      <div className="text-container">
        <div className="header-body">
          <div className="header-icon">
            <ReactSVG src={iconName ? iconName : PeopleIcon} />
          </div>
          <Text className="text-container_header" fontWeight={600}>
            {headerText}
          </Text>
        </div>
        <Text className="text-container_body" fontWeight={400}>
          {bodyText}
        </Text>
      </div>

      {/* <IconButton
        className="close-icon"
        size={8}
        iconName={CrossReactSvg}
        onClick={onClose}
      /> */}
    </StyledPublicRoomBar>
  );
};

export default PublicRoomBar;
