import React from "react";
import styled from "styled-components";
import PropTypes from "prop-types";
import { isMobileOnly } from "react-device-detect";
import MenuIcon from "@docspace/components/public/static/images/menu.react.svg";
import { mobile } from "@docspace/components/utils/device";
import { Base } from "@docspace/components/themes";

const StyledIconBox = styled.div`
  @media ${mobile} {
    display: flex;
  }
  display: ${isMobileOnly ? "flex" : "none"};
  align-items: center;

  padding-left: 16px;
  -webkit-tap-highlight-color: rgba(0, 0, 0, 0);
`;

const StyledMenuIcon = styled(MenuIcon)`
  width: 20px;
  height: 20px;

  path {
    fill: ${(props) => props.theme.catalog.headerBurgerColor};
  }

  cursor: pointer;
`;

StyledMenuIcon.defaultProps = { theme: Base };

const HeaderCatalogBurger = (props) => {
  const { isProduct, onClick, ...rest } = props;

  return (
    <StyledIconBox onClick={onClick} name="catalog-burger" {...rest}>
      <StyledMenuIcon />
    </StyledIconBox>
  );
};

HeaderCatalogBurger.propTypes = {
  isProduct: PropTypes.bool,
  onClick: PropTypes.func,
};

export default React.memo(HeaderCatalogBurger);
