import React from "react";
import PropTypes from "prop-types";
import { ReactSVG } from "react-svg";
import styled from "styled-components";
import Button from "../button";
import { tablet } from "../utils/device";

const StyledButton = styled(Button)`
  border: none;
  padding: 0px 10px 0 10px;

  :hover {
    border: none;
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
