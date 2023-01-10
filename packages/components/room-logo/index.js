import React from "react";
import PropTypes from "prop-types";
import { ReactSVG } from "react-svg";
import { isMobile } from "react-device-detect";

import ArchiveSvg32Url from "../../../public/images/icons/32/room/archive.svg?url";
import CustomSvg32Url from "../../../public/images/icons/32/room/custom.svg?url";
import EditingSvg32Url from "../../../public/images/icons/32/room/editing.svg?url";
import FillingFormSvg32Url from "../../../public/images/icons/32/room/filling.form.svg?url";
import ReviewSvg32Url from "../../../public/images/icons/32/room/review.svg?url";
import ViewOnlySvg32Url from "../../../public/images/icons/32/room/view.only.svg?url";

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
      return ArchiveSvg32Url;
    }

    switch (type) {
      case RoomsType.ReadOnlyRoom:
        return isPrivacy;
        ViewOnlySvg32Url;
      case RoomsType.ReviewRoom:
        return isPrivacy;
        ReviewSvg32Url;
      case RoomsType.FillingFormsRoom:
        return FillingFormSvg32Url;
      case RoomsType.EditingRoom:
        return EditingSvg32Url;
      case RoomsType.CustomRoom:
        return CustomSvg32Url;
      default:
        return "";
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
