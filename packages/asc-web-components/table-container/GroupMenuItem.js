import React from "react";
import PropTypes from "prop-types";
import { ReactSVG } from "react-svg";
import styled from "styled-components";
import Button from "../button";
import { mobile, tablet } from "../utils/device";
import { Base } from "../themes";

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
    path {
      fill: ${(props) => props.theme.button.color.base};
    }
  }

  :hover,
  :active {
    border: none;
    background-color: unset;

    svg {
      path {
        fill: ${(props) => props.theme.button.color.baseHover};
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

  @media ${mobile} {
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
