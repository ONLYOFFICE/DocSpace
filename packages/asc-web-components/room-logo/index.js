import React from "react";
import PropTypes from "prop-types";
import { ReactSVG } from "react-svg";

import StyledLogoContainer from "./styled-room-logo";

const RoomLogo = ({ id, className, style, type, isPrivacy }) => {
  const getIcon = () => {
    switch (type) {
      case "view":
        return isPrivacy
          ? "/static/images/room.privacy.view.svg"
          : "/static/images/room.view.svg";
      case "review":
        return isPrivacy
          ? "/static/images/room.privacy.review.svg"
          : "/static/images/room.review.svg";
      case "fill":
        return isPrivacy
          ? "/static/images/room.privacy.fill.svg"
          : "/static/images/room.fill.svg";
      case "editing":
        return isPrivacy
          ? "/static/images/room.privacy.editing.svg"
          : "/static/images/room.editing.svg";
      case "custom":
        return isPrivacy
          ? "/static/images/room.privacy.custom.svg"
          : "/static/images/room.custom.svg";
      case "archive":
        return isPrivacy
          ? "/static/images/room.privacy.archive.svg"
          : "/static/images/room.archive.svg";
    }
  };

  const icon = getIcon();

  return (
    <StyledLogoContainer
      id={id}
      className={className}
      style={style}
      type={type}
      isPrivacy={isPrivacy}
    >
      <ReactSVG className="room-logo_icon" src={icon} />
    </StyledLogoContainer>
  );
};

RoomLogo.defaultProps = {
  isPrivacy: false,
};

RoomLogo.propTypes = {
  /** Accepts type of the room */
  type: PropTypes.oneOf([
    "view",
    "review",
    "fill",
    "editing",
    "custom",
    "archive",
  ]),
  /** Add privacy icon  */
  isPrivacy: PropTypes.bool,
  /** Accepts id  */
  id: PropTypes.string,
  /** Accepts class name  */
  className: PropTypes.string,
  /** Accepts css style  */
  style: PropTypes.oneOfType([PropTypes.object, PropTypes.array]),
};

export default React.memo(RoomLogo);
