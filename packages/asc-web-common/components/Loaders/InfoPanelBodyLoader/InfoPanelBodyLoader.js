import PropTypes from "prop-types";
import React from "react";

import RectangleLoader from "../RectangleLoader";
import {
  StyledAccessRow,
  StyledInfoRoomBody,
  StyledPropertiesTable,
  StyledPropertyRow,
  StyledSubtitle,
  StyledThumbnail,
  StyledIcon,
  StyledTitle,
} from "./StyledInfoPanelBodyLoader";

const InfoPanelBodyLoader = ({
  id,
  className,
  style,
  isFolder,
  hasThumbnail,
  isVirtualRoom,
  ...rest
}) => {
  const {} = rest;

  const randomNumber = (min, max) => Math.random() * (max - min) + min;

  const customRectangleLoader = (width, height, rounded) => (
    <RectangleLoader
      width={"" + width}
      height={"" + height}
      borderRadius={"" + rounded}
    />
  );

  // const properties = isFolder
  //   ? [19, 19, 17.6, 17.6, 17.6, 19, 17.6]
  //   : [19, 19, 17.6, 17.6, 17.6, 17.6, 17.6, 19, 17.6, 17.6, 17.6];

  const properties = isFolder
    ? [19, 17.6, 17.6, 17.6, 19, 17.6]
    : [19, 17.6, 17.6, 17.6, 17.6, 17.6, 19, 17.6, 17.6, 17.6];

  return (
    <StyledInfoRoomBody id={id} className={className} style={style}>
      <StyledTitle>
        {customRectangleLoader(32, 32, 3)}
        {customRectangleLoader(250, 22, 3)}
      </StyledTitle>

      {hasThumbnail ? (
        <StyledThumbnail>
          {customRectangleLoader(320.2, 200, 6)}
        </StyledThumbnail>
      ) : (
        <StyledIcon>{customRectangleLoader(96, 96, 5)}</StyledIcon>
      )}

      <StyledSubtitle>{customRectangleLoader(200, 20, 3)}</StyledSubtitle>

      <StyledPropertiesTable>
        {properties.map((property, i) => (
          <StyledPropertyRow key={i}>
            {customRectangleLoader("100%", property, 3)}
            {customRectangleLoader("100%", property, 3)}
          </StyledPropertyRow>
        ))}
      </StyledPropertiesTable>

      {/* <StyledSubtitle>Who has access</StyledSubtitle>

      <StyledAccessRow>
        {customRectangleLoader(32, 32, 16)}
        <div className="divider"></div>
        {customRectangleLoader(32, 32, 16)}
        {customRectangleLoader(32, 32, 16)}
      </StyledAccessRow> */}
    </StyledInfoRoomBody>
  );
};

InfoPanelBodyLoader.propTypes = {
  id: PropTypes.string,
  className: PropTypes.string,
  style: PropTypes.object,
};

InfoPanelBodyLoader.defaultProps = {
  id: undefined,
  className: undefined,
  style: undefined,
};

export default InfoPanelBodyLoader;
