import React from "react";
import PropTypes from "prop-types";
import styled from "styled-components";
import { Text } from "asc-web-components";

const TitleContainer = styled.div`
  width: auto;
  &:hover {
    .selectable {
      text-decoration: underline;
    }
  }

  .title-content {
    display: flex;
    flex-wrap: wrap;
    justify-content: center;

    .title-image-wrapper {
      padding: 0 16px;
      position: relative;

      @media (min-width: 768px) {
        flex: 0 0 auto;
        width: auto;
        max-width: 100%;
      }

      .title-image {
        border: none;
        height: 241px;
        width: 240px;
        cursor: pointer;
      }
    }

    .title-text-wrapper {
      padding: 0 16px;
      position: relative;
      width: 100%;

      @media (min-width: 768px) {
        flex: 0 0 auto;
        width: auto;
        max-width: 50%;
      }
    }
    .title-text {
      flex: 1 1 auto;
      padding: 1.25rem;

      .title-text-header {
        margin: 46px 0 11px 0;
        cursor: pointer;
      }
      .title-text-description {
        line-height: 20px;
      }
    }
  }

  .sub-title-content {
    text-align: center;
    flex: 1 1 auto;
    padding: 1.25rem;
    cursor: pointer;

    .sub-title-image {
      border: none;
      height: 100px;
      width: 100px;
    }
    .sub-title-text {
      margin: 16px 0 16px 0;
      text-align: center;
    }
  }
`;

const ModuleTile = props => {
  //console.log("ModuleTile render");
  const { title, imageUrl, link, description, isPrimary, onClick } = props;

  const handleClick = (e, link) => onClick && onClick(e, link);

  return (
    <TitleContainer>
      {isPrimary ? (
        <div className="title-content">
          <div className="title-image-wrapper">
            <img
              className="title-image selectable"
              src={imageUrl}
              onClick={handleClick.bind(link)}
            />
          </div>

          <div className="title-text-wrapper">
            <div onClick={handleClick.bind(link)} className="title-text">
              <Text.Body fontSize={36} className="title-text-header selectable">
                {title}
              </Text.Body>
              <Text.Body fontSize={12} className="title-text-description">
                {description}
              </Text.Body>
            </div>
          </div>
        </div>
      ) : (
        <div className="sub-title-content selectable">
          <div>
            <img
              className="sub-title-image"
              src={imageUrl}
              onClick={handleClick.bind(link)}
            />
          </div>
          <div>
            <div>
              <Text.Body
                fontSize={18}
                className="sub-title-text"
                onClick={handleClick.bind(link)}
              >
                {title}
              </Text.Body>
            </div>
          </div>
        </div>
      )}
    </TitleContainer>
  );
};

ModuleTile.propTypes = {
  title: PropTypes.string.isRequired,
  imageUrl: PropTypes.string.isRequired,
  link: PropTypes.string.isRequired,
  description: PropTypes.string,
  isPrimary: PropTypes.bool,
  onClick: PropTypes.func.isRequired
};

ModuleTile.defaultProps = {
  isPrimary: false,
  description: ""
};

export default ModuleTile;
