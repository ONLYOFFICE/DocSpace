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
    "img main"
    "img button";
	grid-template-rows: auto auto auto;
  grid-template-columns: 150px 1fr;
  min-width: 320px;
  max-width: 742px;

  .ec-body {
    grid-area: header;
  }

  @media ${mobile} {
    grid-template-areas: 
    "header"
    "main"
    "button";
    grid-template-columns: 1fr;
    padding: 20px;

    .ec-body {
      font-size: 18px;
    }
  } 

  @media (max-height: 400px) and (orientation: landscape){
    padding: 20px;

    .ec-body {
      font-size: 18px;
    }
  }
`;

const EmptyContentImage = styled.img.attrs(props => ({
  src: props.imageSrc,
  alt: props.imageAlt
}))`
  background: no-repeat 0 0 transparent;
  grid-area: img; 
  margin: auto 0;

  @media ${mobile} {      
    display: none;
  }

  @media (max-height: 400px) and (orientation: landscape){
    height: 40vh;
  }
`;

const EmptyContentDescriptionContainer = styled.div`
  grid-area: main; 
  padding-top: 5px;

  @media (max-height: 400px) and (orientation: landscape){
    span {
      font-size: 2.25vw; 
    }
  }
`;

const EmptyContentButtonsContainer = styled.div`
  grid-area: button; 
  padding-top: 10px;
`;

const EmptyScreenContainer = props => {
  const { imageSrc, imageAlt, headerText, descriptionText, buttons } = props;
  return (
    <EmptyContentContainer>
      <EmptyContentBody {...props}>

        <EmptyContentImage imageSrc={imageSrc} imageAlt={imageAlt} />

        {headerText && (
          <Text.Body as="span" color="#333333" fontSize={24} className="ec-body">{headerText}</Text.Body>
        )}
        
        {descriptionText && (
          <EmptyContentDescriptionContainer>
            <Text.Body as="span" color="#737373" fontSize={14}>{descriptionText}</Text.Body>
          </EmptyContentDescriptionContainer>
        )}

        {buttons && (
          <EmptyContentButtonsContainer>
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
