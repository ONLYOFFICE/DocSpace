import React from "react";
import PropTypes from "prop-types";
import styled, { css } from "styled-components";
import Heading from "@appserver/components/heading";
import { isMobileOnly, isTablet } from "react-device-detect";
import MenuIcon from "@appserver/components/public/static/images/menu.react.svg";
import { tablet, mobile } from "@appserver/components/utils/device";
import Base from "@appserver/components/themes/base";

const StyledCatalogHeader = styled.div`
  padding: 12px 20px 13px;
  display: flex;
  justify-content: flex-start;
  align-items: center;

  @media ${tablet} {
    padding: 16px 16px 17px;
    margin: 0;
    justify-content: ${(props) => (props.showText ? "flex-start" : "center")};
  }

  @media ${mobile} {
    border-bottom: ${(props) => props.theme.catalog.header.borderBottom};
    padding: 12px 16px 12px;
    margin-bottom: 16px !important;
  }

  ${isTablet &&
  css`
    padding: 16px 16px 17px;
    justify-content: ${(props) => (props.showText ? "flex-start" : "center")};
    margin: 0;
  `}

  ${isMobileOnly &&
  css`
    border-bottom: ${(props) =>
      props.theme.catalog.header.borderBottom} !important;
    padding: 12px 16px 12px !important;
    margin-bottom: 16px !important;
  `}
`;

StyledCatalogHeader.defaultProps = { theme: Base };

const StyledHeading = styled(Heading)`
  margin: 0;
  padding: 0;
  font-weight: bold;
  line-height: 28px;
  @media ${tablet} {
    display: ${(props) => (props.showText ? "block" : "none")};
    margin-left: ${(props) => props.showText && "12px"};
  }

  ${isTablet &&
  css`
    display: ${(props) => (props.showText ? "block" : "none")};
    margin-left: ${(props) => props.showText && "12px"};
  `}

  @media ${mobile} {
    margin-left: 0;
  }

  ${isMobileOnly &&
  css`
    margin-left: 0 !important;
  `}
`;

const StyledIconBox = styled.div`
  display: none;
  align-items: center;
  height: 28px;

  @media ${tablet} {
    display: flex;
  }

  @media ${mobile} {
    display: none;
  }

  ${isTablet &&
  css`
    display: flex !important;
  `}

  ${isMobileOnly &&
  css`
    display: none !important;
  `}
`;

const StyledMenuIcon = styled(MenuIcon)`
  display: block;
  width: 20px;
  height: 20px;

  cursor: pointer;

  path {
    fill: ${(props) => props.theme.catalog.header.iconFill};
  }
`;

StyledMenuIcon.defaultProps = { theme: Base };

const CatalogHeader = (props) => {
  const { showText, children, onClick, ...rest } = props;

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

CatalogHeader.displayName = "CatalogHeader";

export default React.memo(CatalogHeader);
