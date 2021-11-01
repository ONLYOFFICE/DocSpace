import React from "react";
import PropTypes from "prop-types";
import { ReactSVG } from "react-svg";
import styled from "styled-components";
import Button from "../button";
import { mobile, tablet } from "../utils/device";

const StyledButton = styled(Button)`
  border: none;
  padding: 4px 10px 0 10px;
  height: 50px;
  min-width: fit-content;

  :hover,
  :active {
    border: none;
    background-color: unset;
  }

  .btnIcon {
    padding-right: 8px;
  }

  @media ${tablet} {
    display: flex;
    flex-direction: column;
    height: 60px;
    padding: 12px 12px 0 12px;
    .btnIcon {
      padding: 0;
      margin: 0 auto;
    }
  }

  @media ${mobile} {
    padding: 18px 16px 0 16px;
    height: 50px;
    font-size: 0;
    line-height: 0;
  }
`;

const GroupMenuItem = ({ item }) => {
  const { label, disabled, onClick, iconUrl } = item;
  return (
    <StyledButton
      label={label}
      isDisabled={disabled}
      onClick={onClick}
      icon={<ReactSVG src={iconUrl} className="combo-button_selected-icon" />}
    />
  );
};

GroupMenuItem.propTypes = {
  item: PropTypes.object,
};

export default GroupMenuItem;
