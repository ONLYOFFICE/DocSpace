import React from "react";
import { ReactSVG } from "react-svg";
import PeopleIcon from "PUBLIC_DIR/images/people.react.svg?url";
import CrossReactSvg from "PUBLIC_DIR/images/cross.react.svg?url";
import IconButton from "@docspace/components/icon-button";
import { StyledPublicRoomBar } from "./StyledPublicRoom";

const PublicRoomBar = (props) => {
  const { headerText, bodyText, iconName, onClose, ...rest } = props;

  return (
    <StyledPublicRoomBar {...rest}>
      <div className="text-container">
        <div className="header-body">
          <div className="header-icon">
            <ReactSVG src={iconName ? iconName : PeopleIcon} />
          </div>
          <div>{headerText}</div>
        </div>
        <div className="body-container">{bodyText}</div>
      </div>

      {/* <IconButton
        className="close-icon"
        size={8}
        iconName={CrossReactSvg}
        onClick={onClose}
        color="#657077"
      /> */}
    </StyledPublicRoomBar>
  );
};

export default PublicRoomBar;
