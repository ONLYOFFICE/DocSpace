import React from "react";
import PropTypes from "prop-types";

import {
  StyledCatalogHeader,
  StyledHeading,
  StyledIconBox,
  StyledMenuIcon,
} from "../styled-catalog";

const CatalogHeader = ({ showText, children, onClick, ...rest }) => {
  return (
    <StyledCatalogHeader showText={showText} {...rest}>
      <StyledIconBox name="catalog-burger">
        <StyledMenuIcon onClick={onClick} />
      </StyledIconBox>

      <StyledHeading showText={showText} size="large">
        {children}
      </StyledHeading>
    </StyledCatalogHeader>
  );
};

CatalogHeader.propTypes = {
  children: PropTypes.any,
  showText: PropTypes.bool,
  onClick: PropTypes.func,
};

CatalogHeader.displayName = "Header";

export default React.memo(CatalogHeader);
