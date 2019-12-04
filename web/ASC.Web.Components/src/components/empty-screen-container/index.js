import React from "react";
import styled from "styled-components";
import PropTypes from "prop-types";
import Text from '../text';

const EmptyContentBody = styled.div`
  margin: 0 auto;
  padding: 50px 0;
  
	display: grid;
	grid-template-areas: 
    "img header"
    "img desc"
    "img button";
  min-width: 320px;
  max-width: 742px;

  .ec-image {
    grid-area: img;
    margin: auto 0;
  }

  .ec-header {
    grid-area: header;
  }

  .ec-desc {
    grid-area: desc; 
    padding-top: 5px;
  }

  .ec-buttons {
    grid-area: button; 
    padding-top: 10px;
  }

  @media (orientation: portrait) {
    @media (max-width: 700px) {
      .ec-image {
        height: 21vw;
      }

      .ec-header {
        font-size: 3.5vw;
      }

      .ec-desc {
        font-size: 2.4vw;
      }
    }

    @media (max-width: 480px) {
      .ec-image {
        display: none;
      }

      .ec-header {
        font-size: 4.75vw;
      }

      .ec-desc {
        font-size: 3.7vw; 
      }
    }
  }
`;

const EmptyContentImage = styled.img.attrs(props => ({
  src: props.imageSrc,
  alt: props.imageAlt
}))`
  background: no-repeat 0 0 transparent;
`;

const EmptyScreenContainer = props => {
  const { imageSrc, imageAlt, headerText, descriptionText, buttons } = props;
  return (

    <EmptyContentBody {...props}>

      <EmptyContentImage imageSrc={imageSrc} imageAlt={imageAlt} className="ec-image" />

      {headerText && (
        <Text as="span" color="#333333" fontSize={24} className="ec-header">{headerText}</Text>
      )}

      {descriptionText && (
        <Text as="span" color="#737373" fontSize={14} className="ec-desc">{descriptionText}</Text>
      )}

      {buttons && (
        <div className="ec-buttons">
          {buttons}
        </div>
      )}

    </EmptyContentBody>
  );
};

EmptyScreenContainer.propTypes = {
  imageSrc: PropTypes.string,
  imageAlt: PropTypes.string,
  headerText: PropTypes.string,
  descriptionText: PropTypes.string,
  buttons: PropTypes.any
};

export default EmptyScreenContainer;
