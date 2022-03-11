import React from "react";
import PropTypes from "prop-types";

import Backdrop from "@appserver/components/backdrop";

import { StyledControlContainer, StyledCrossIcon } from "../styled-catalog";

const CatalogBackdrop = ({ showText, onClick, ...rest }) => {
  return (
    <>
      <StyledControlContainer onClick={onClick} {...rest}>
        <StyledCrossIcon />
      </StyledControlContainer>
      <Backdrop visible={true} zIndex={201} withBackground={true} />
    </>
  );
};

CatalogBackdrop.propTypes = {
  showText: PropTypes.bool,
  onClick: PropTypes.func,
};

export default React.memo(CatalogBackdrop);
