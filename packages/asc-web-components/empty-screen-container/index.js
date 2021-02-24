import React from "react";
import PropTypes from "prop-types";

import Text from "../text";
import {
  EmptyContentBody,
  EmptyContentImage,
} from "./styled-empty-screen-container";

const EmptyScreenContainer = (props) => {
  const {
    imageSrc,
    imageAlt,
    headerText,
    subheadingText,
    descriptionText,
    buttons,
  } = props;
  return (
    <EmptyContentBody {...props}>
      <EmptyContentImage
        imageSrc={imageSrc}
        imageAlt={imageAlt}
        className="ec-image"
      />

      {headerText && (
        <Text as="span" fontSize="19px" fontWeight="600" className="ec-header">
          {headerText}
        </Text>
      )}

      {subheadingText && (
        <Text as="span" fontWeight="600" className="ec-subheading">
          {subheadingText}
        </Text>
      )}

      {descriptionText && (
        <Text as="span" color="#6A7378" fontSize="12px" className="ec-desc">
          {descriptionText}
        </Text>
      )}

      {buttons && <div className="ec-buttons">{buttons}</div>}
    </EmptyContentBody>
  );
};

EmptyScreenContainer.propTypes = {
  imageSrc: PropTypes.string,
  imageAlt: PropTypes.string,
  headerText: PropTypes.string,
  subheadingText: PropTypes.string,
  descriptionText: PropTypes.oneOfType([PropTypes.object, PropTypes.string]),
  buttons: PropTypes.any,
  className: PropTypes.string,
  id: PropTypes.string,
  style: PropTypes.oneOfType([PropTypes.object, PropTypes.array]),
};

export default EmptyScreenContainer;
