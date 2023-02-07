import React from "react";
import PropTypes from "prop-types";

import Backdrop from "@docspace/components/backdrop";

import { StyledControlContainer, StyledCrossIcon } from "../styled-article";

const ArticleBackdrop = ({ onClick, ...rest }) => {
  return (
    <>
      <StyledControlContainer onClick={onClick} {...rest}>
        <StyledCrossIcon />
      </StyledControlContainer>
      <Backdrop
        onClick={onClick}
        visible={true}
        zIndex={210}
        withBackground={true}
      />
    </>
  );
};

ArticleBackdrop.propTypes = {
  showText: PropTypes.bool,
  onClick: PropTypes.func,
};

export default React.memo(ArticleBackdrop);
