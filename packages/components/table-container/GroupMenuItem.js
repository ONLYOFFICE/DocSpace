import React from "react";
import PropTypes from "prop-types";
import { ReactSVG } from "react-svg";
import styled from "styled-components";
import Button from "../button";
import { mobile, tablet, hugeMobile } from "../utils/device";
import { Base } from "../themes";
import { isChrome, browserVersion } from "react-device-detect";

const StyledButton = styled(Button)`
  border: none;
  padding: 5px 10px 0 10px;
  height: 50px;
  min-width: fit-content;

  background-color: ${(props) => props.theme.button.backgroundColor.base};

  :hover {
    background-color: ${(props) =>
      props.theme.button.backgroundColor.baseHover};
  }
  :active {
    background-color: ${(props) =>
      props.theme.button.backgroundColor.baseActive};
  }

  svg {
    path[fill] {
      fill: ${(props) => props.theme.button.color.base};
    }

    path[stroke] {
      stroke: ${(props) => props.theme.button.color.base};
    }
  }

  :hover,
  :active {
    border: none;
    background-color: unset;
  }

  :hover {
    svg {
      path[fill] {
        fill: ${(props) => props.theme.button.color.baseHover};
      }

      path[stroke] {
        stroke: ${(props) => props.theme.button.color.baseHover};
      }
    }
  }

  :active {
    svg {
      path[fill] {
        fill: ${(props) => props.theme.button.color.baseActive};
      }

      path[stroke] {
        stroke: ${(props) => props.theme.button.color.baseActive};
      }
    }
  }

  .btnIcon {
    padding-right: 8px;
  }

  .button-content {
    @media ${tablet} {
      flex-direction: column;
      gap: 0px;
    }

    ${isChrome &&
    browserVersion <= 85 &&
    `
    /* TODO: remove if editors core version 85+ */
      > div {
        margin-right: 8px;

        @media ${tablet} {
          margin-right: 0px;
        }
      }
    `}
  }

  @media ${tablet} {
    display: flex;
    justify-content: center;
    flex-direction: column;
    height: 60px;
    padding: 0px 12px;
    .btnIcon {
      padding: 0;
      margin: 0 auto;
    }
  }

  @media ${mobile}, ${hugeMobile} {
    padding: 0 16px;
    height: 50px;
    font-size: 0;
    line-height: 0;
  }
`;

StyledButton.defaultProps = { theme: Base };

const GroupMenuItem = ({ item }) => {
  const { label, disabled, onClick, iconUrl, title } = item;
  return (
    <>
      {disabled ? (
        <></>
      ) : (
        <StyledButton
          label={label}
          title={title || label}
          isDisabled={disabled}
          onClick={onClick}
          icon={
            <ReactSVG src={iconUrl} className="combo-button_selected-icon" />
          }
        />
      )}
    </>
  );
};

GroupMenuItem.propTypes = {
  item: PropTypes.object,
};

export default GroupMenuItem;
