import React from "react";
import styled from "styled-components";
import PropTypes from "prop-types";
import { isMobileOnly } from "react-device-detect";
import MenuIcon from "@appserver/components/public/static/images/menu.react.svg";
import { mobile } from "@appserver/components/utils/device";
import { Base } from "@appserver/components/themes";

const StyledIconBox = styled.div`
  @media ${mobile} {
    display: flex;
  }
  display: ${isMobileOnly ? "flex" : "none"};
  align-items: center;

  padding-left: 16px;
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
