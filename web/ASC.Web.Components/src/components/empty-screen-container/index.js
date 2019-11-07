import React from "react";
import styled from "styled-components";
import PropTypes from "prop-types";
import { Text } from '../text';
import { mobile } from '../../utils/device'

const EmptyContentContainer = styled.div`
  margin: 0;
  display: flex;
  justify-content: center;
  align-items: center;
  height: 100vh;
`;

const EmptyContentBody = styled.div`
	display: grid;
	grid-template-areas: 
    "img header"
    "img desc"
    "img button";
	grid-template-rows: auto auto auto;
  grid-template-columns: 150px 1fr;
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

  @media (max-height: 400px) and (orientation: landscape){
    padding: 20px;

    .ec-image {
      height: 40vh;
    }

    .ec-header {
      font-size: 18px;
    }

    .ec-desc {
      font-size: 2.25vw; 
    }
  }

  @media ${mobile} {
    grid-template-areas: 
    "header"
    "desc"
    "button";
    grid-template-columns: 1fr;
    padding: 20px;

    .ec-image {
      display: none;
    }

    .ec-header {
      font-size: 18px;
    }
  } 
`;

const EmptyContentImage = styled.img.attrs(props => ({
  src: props.imageSrc,
  alt: props.imageAlt
}))`
  background: no-repeat 0 0 transparent;
`;

const EmptyContentButtonsContainer = styled.div``;

const EmptyScreenContainer = props => {
  const { imageSrc, imageAlt, headerText, descriptionText, buttons } = props;
  return (
    <EmptyContentContainer>
      <EmptyContentBody {...props}>

        <EmptyContentImage imageSrc={imageSrc} imageAlt={imageAlt} className="ec-image"/>

        {headerText && (
          <Text.Body as="span" color="#333333" fontSize={24} className="ec-header">{headerText}</Text.Body>
        )}
        
        {descriptionText && (          
          <Text.Body as="span" color="#737373" fontSize={14} className="ec-desc">{descriptionText}</Text.Body>
        )}

        {buttons && (
          <EmptyContentButtonsContainer className="ec-buttons">
            {buttons}
          </EmptyContentButtonsContainer>
        )}

      </EmptyContentBody>
    </EmptyContentContainer>
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
