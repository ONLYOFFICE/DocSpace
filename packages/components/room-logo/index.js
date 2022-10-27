import React from "react";
import PropTypes from "prop-types";
import { ReactSVG } from "react-svg";
import { isMobile } from "react-device-detect";

import { RoomsType } from "@docspace/common/constants";

import Checkbox from "../checkbox";

import { StyledContainer, StyledLogoContainer } from "./styled-room-logo";

const RoomLogo = ({
  id,
  className,
  style,
  type,
  isPrivacy,
  isArchive,
  withCheckbox,
  isChecked,
  isIndeterminate,
  onChange,
}) => {
  const getIcon = () => {
    if (isArchive) {
      return isPrivacy
        ? "/static/images/room.privacy.archive.svg"
        : "/static/images/room.archive.svg";
    }

    switch (type) {
      case RoomsType.FillingFormsRoom:
        return isPrivacy
          ? "/static/images/room.privacy.fill.svg"
          : "/static/images/room.with-border.fill.svg";
      case RoomsType.EditingRoom:
        return isPrivacy
          ? "/static/images/room.privacy.editing.svg"
          : "/static/images/room.with-border.editing.svg";
      case RoomsType.ReviewRoom:
        return isPrivacy
          ? "/static/images/room.privacy.review.svg"
          : "/static/images/room.with-border.review.svg";
      case RoomsType.ReadOnlyRoom:
        return isPrivacy
          ? "/static/images/room.privacy.view.svg"
          : "/static/images/room.with-border.view.svg";
      case RoomsType.CustomRoom:
        return isPrivacy
          ? "/static/images/room.privacy.custom.svg"
          : "/static/images/room.with-border.custom.svg";
      default:
        return isPrivacy
          ? "/static/images/room.privacy.custom.svg"
          : "/static/images/room.with-border.custom.svg";
    }
  };

  const onSelect = () => {
    if (!isMobile) return;

    onChange && onChange();
  };

  const icon = getIcon();

  return (
    <StyledContainer id={id} className={className} style={style}>
      <StyledLogoContainer
        className="room-logo_icon-container"
        type={type}
        isPrivacy={isPrivacy}
        isArchive={isArchive}
        onClick={onSelect}
      >
        <ReactSVG className="room-logo_icon" src={icon} />
      </StyledLogoContainer>

      {withCheckbox && (
        <Checkbox
          className="room-logo_checkbox checkbox"
          isChecked={isChecked}
          isIndeterminate={isIndeterminate}
          onChange={onChange}
        />
      )}
    </StyledContainer>
  );
};

RoomLogo.defaultProps = {
  isPrivacy: false,
  isArchive: false,
  withCheckbox: false,
  isChecked: false,
  isIndeterminate: false,
};

RoomLogo.propTypes = {
  /** Accepts type of the room */
  type: PropTypes.number,
  /** Add privacy icon  */
  isPrivacy: PropTypes.bool,
  /** Add archive icon  */
  isArchive: PropTypes.bool,
  /** Add checkbox when row/tile is hovered or checked  */
  withCheckbox: PropTypes.bool,
  /** Add checked state to checkbox  */
  isChecked: PropTypes.bool,
  /** Add indeterminate state to checkbox  */
  isIndeterminate: PropTypes.bool,
  /** Add onChange checkbox callback function  */
  onChange: PropTypes.func,
  /** Accepts id  */
  id: PropTypes.string,
  /** Accepts class name  */
  className: PropTypes.string,
  /** Accepts css style  */
  style: PropTypes.oneOfType([PropTypes.object, PropTypes.array]),
};

export default React.memo(RoomLogo);
