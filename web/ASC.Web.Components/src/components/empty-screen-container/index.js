import React from "react";
import styled from "styled-components";
import PropTypes from "prop-types";
import { Text } from '../text';

const EmptyContentContainer = styled.div`
  padding-top: 50px;
  margin: 0 auto;
  width: 687px;
  text-align: center;
  color: #373737;
  padding: 100px 0;
  min-width: 200px;
`;

const EmptyContentImage = styled.img.attrs(props => ({
  src: props.imageSrc,
  alt: props.imageAlt
}))`
  background: no-repeat 0 0 transparent;
  display: block;
  height: 150px;
  width: 150px;
`;

const EmptyContentBodyContainer = styled.div`
  text-align: left;
  padding: 10px;
`;

const EmptyContentDescriptionContainer = styled.div`
  margin: 14px auto 0;
  max-width: 600px;
`;

const EmptyContentButtonsContainer = styled.div`
  margin-top: 18px;
`;

const EmptyScreenContainer = props => {
  const { imageSrc, imageAlt, headerText, descriptionText, buttons } = props;
  return (
    <EmptyContentContainer {...props}>
      <table>
        <tbody>
          <tr>
            <td>
              <EmptyContentImage imageSrc={imageSrc} imageAlt={imageAlt} />
            </td>
            <td>
              <EmptyContentBodyContainer>
                {headerText && (
                  <Text.Body as="div" color="#333333" fontSize={24}>{headerText}</Text.Body>
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
              </EmptyContentBodyContainer>
            </td>
          </tr>
        </tbody>
      </table>
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
