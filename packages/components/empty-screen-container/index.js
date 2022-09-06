import React from "react";
import PropTypes from "prop-types";

import Text from "../text";
import {
  EmptyContentBody,
  EmptyContentImage,
} from "./styled-empty-screen-container";

import { isMobile } from "react-device-detect";

const EmptyScreenContainer = (props) => {
  const {
    imageSrc,
    imageAlt,
    headerText,
    subheadingText,
    descriptionText,
    buttons,
    imageStyle,
    buttonStyle,
  } = props;
  return (
    <EmptyContentBody {...props}>
      <EmptyContentImage
        imageSrc={imageSrc}
        imageAlt={imageAlt}
        style={!isMobile ? imageStyle : {}}
        className="ec-image"
      />

      {headerText && (
        <Text
          as="span"
          fontSize="19px"
          fontWeight="700"
          className="ec-header"
          noSelect
        >
          {headerText}
        </Text>
      )}

      {subheadingText && (
        <Text as="span" fontWeight="600" className="ec-subheading" noSelect>
          {subheadingText}
        </Text>
      )}

      {descriptionText && (
        <Text
          as="span"
          color="#6A7378"
          fontSize="12px"
          className="ec-desc"
          noSelect
        >
          {descriptionText}
        </Text>
      )}

      {buttons && (
        <div className="ec-buttons" style={buttonStyle}>
          {buttons}
        </div>
      )}
    </EmptyContentBody>
  );
};

EmptyScreenContainer.propTypes = {
  /** Image url source */
  imageSrc: PropTypes.string,
  /** Alternative image text */
  imageAlt: PropTypes.string,
  /** Header text */
  headerText: PropTypes.string,
  /** Subheading text */
  subheadingText: PropTypes.string,
  /** Description text */
  descriptionText: PropTypes.oneOfType([PropTypes.object, PropTypes.string]),
  /** Content of EmptyContentButtonsContainer */
  buttons: PropTypes.any,
  /** Accepts class */
  className: PropTypes.string,
  /** Accepts id */
  id: PropTypes.string,
  /** Accepts css style */
  style: PropTypes.oneOfType([PropTypes.object, PropTypes.array]),
};

export default EmptyScreenContainer;
