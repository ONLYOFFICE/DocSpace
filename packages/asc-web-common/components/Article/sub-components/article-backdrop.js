import React from "react";
import PropTypes from "prop-types";

import Backdrop from "@appserver/components/backdrop";

import { StyledControlContainer, StyledCrossIcon } from "../styled-article";

const ArticleBackdrop = ({ onClick, ...rest }) => {
  return (
    <>
      <StyledControlContainer onClick={onClick} {...rest}>
        <StyledCrossIcon />
      </StyledControlContainer>
      <Backdrop visible={true} zIndex={201} withBackground={true} />
    </>
  );
};

ArticleBackdrop.propTypes = {
  showText: PropTypes.bool,
  onClick: PropTypes.func,
};

export default React.memo(ArticleBackdrop);
