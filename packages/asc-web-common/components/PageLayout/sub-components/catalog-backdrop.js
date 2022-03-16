import React from "react";
import PropTypes from "prop-types";
import styled from "styled-components";
import CrossIcon from "@appserver/components/public/static/images/cross.react.svg";
import Base from "@appserver/components/themes/base";

const StyledControlContainer = styled.div`
  background: ${(props) => props.theme.catalog.control.background};
  width: 24px;
  height: 24px;
  position: absolute;
  top: 30px;
  right: 10px;
  border-radius: 100px;
  cursor: pointer;
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 290;
`;

StyledControlContainer.defaultProps = { theme: Base };

const StyledCrossIcon = styled(CrossIcon)`
  width: 12px;
  height: 12px;
  path {
    fill: ${(props) => props.theme.catalog.control.fill};
  }
`;

StyledCrossIcon.defaultProps = { theme: Base };

const CatalogBackdrop = (props) => {
  const { showText, onClick, ...rest } = props;
  return (
    <StyledControlContainer onClick={onClick} {...rest}>
      <StyledCrossIcon />
    </StyledControlContainer>
  );
};

CatalogBackdrop.propTypes = {
  showText: PropTypes.bool,
  onClick: PropTypes.func,
};

export default React.memo(CatalogBackdrop);
