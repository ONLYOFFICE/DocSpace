import React from "react";
import PropTypes, { number } from "prop-types";
import { ReactSVG } from "react-svg";

import { RoomsType } from "@appserver/common/constants";

import StyledLogoContainer from "./styled-room-logo";

const RoomLogo = ({ id, className, style, type, isPrivacy, isArchive }) => {
  const getIcon = () => {
    if (isArchive) {
      return isPrivacy
        ? "/static/images/room.privacy.archive.svg"
        : "/static/images/room.archive.svg";
    }

    switch (type) {
      case RoomsType.ReadOnlyRoom:
        return isPrivacy
          ? "/static/images/room.privacy.view.svg"
          : "/static/images/room.view.svg";
      case RoomsType.ReviewRoom:
        return isPrivacy
          ? "/static/images/room.privacy.review.svg"
          : "/static/images/room.review.svg";
      case RoomsType.FillingFormsRoom:
        return isPrivacy
          ? "/static/images/room.privacy.fill.svg"
          : "/static/images/room.fill.svg";
      case RoomsType.EditingRoom:
        return isPrivacy
          ? "/static/images/room.privacy.editing.svg"
          : "/static/images/room.editing.svg";
      case RoomsType.CustomRoom:
        return isPrivacy
          ? "/static/images/room.privacy.custom.svg"
          : "/static/images/room.custom.svg";
      default:
        return isPrivacy
          ? "/static/images/room.privacy.custom.svg"
          : "/static/images/room.custom.svg";
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
      isArchive={isArchive}
    >
      <ReactSVG className="room-logo_icon" src={icon} />
    </StyledLogoContainer>
  );
};

RoomLogo.defaultProps = {
  isPrivacy: false,
  isArchive: false,
};

RoomLogo.propTypes = {
  /** Accepts type of the room */
  type: PropTypes.number,
  /** Add privacy icon  */
  isPrivacy: PropTypes.bool,
  /** Add archive icon  */
  isArchive: PropTypes.bool,
  /** Accepts id  */
  id: PropTypes.string,
  /** Accepts class name  */
  className: PropTypes.string,
  /** Accepts css style  */
  style: PropTypes.oneOfType([PropTypes.object, PropTypes.array]),
};

export default React.memo(RoomLogo);
