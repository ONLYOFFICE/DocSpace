import styled, { css } from "styled-components";

import Base from "../themes/base";

import { tablet } from "../utils/device";

import Text from "@appserver/components/text";

const badgeWithoutText = css`
  position: absolute;
  top: ${(props) => props.theme.catalogItem.badgeWithoutText.position};
  right: ${(props) => props.theme.catalogItem.badgeWithoutText.position};
  border-radius: 1000px;
  background-color: ${(props) =>
    props.theme.catalogItem.badgeWithoutText.backgroundColor};

  @media ${tablet} {
    min-width: ${(props) =>
      props.theme.catalogItem.badgeWithoutText.tablet.size};
    min-height: ${(props) =>
      props.theme.catalogItem.badgeWithoutText.tablet.size};
    top: ${(props) => props.theme.catalogItem.badgeWithoutText.tablet.position};
    right: ${(props) =>
      props.theme.catalogItem.badgeWithoutText.tablet.position};
  }
`;

const StyledCatalogItemBadgeWrapper = styled.div`
  display: flex;
  align-items: center;
  justify-content: center;
  min-width: ${(props) =>
    props.showText
      ? props.theme.catalogItem.badgeWrapper.size
      : props.theme.catalogItem.badgeWithoutText.size};
  min-height: ${(props) =>
    props.showText
      ? props.theme.catalogItem.badgeWrapper.size
      : props.theme.catalogItem.badgeWithoutText.size};
  margin-left: ${(props) =>
    props.showText && props.theme.catalogItem.badgeWrapper.marginLeft};
  z-index: 3;
  ${(props) => !props.showText && badgeWithoutText}

  @media ${tablet} {
    min-width: ${(props) =>
      props.showText && props.theme.catalogItem.badgeWrapper.tablet.width};
    min-height: ${(props) =>
      props.showText && props.theme.catalogItem.badgeWrapper.tablet.height};
  }
`;

StyledCatalogItemBadgeWrapper.defaultProps = { theme: Base };

const StyledCatalogItemInitialText = styled(Text)`
  position: absolute;
  top: 3px;
  left: 0;
  text-align: center;
  width: ${(props) => props.theme.catalogItem.initialText.width};
  line-height: ${(props) => props.theme.catalogItem.initialText.lineHeight};
  color: ${(props) => props.theme.catalogItem.initialText.color};
  font-size: ${(props) => props.theme.catalogItem.initialText.fontSize};
  font-weight: ${(props) => props.theme.catalogItem.initialText.fontWeight};
  pointer-events: none;

  @media ${tablet} {
    width: ${(props) => props.theme.catalogItem.initialText.tablet.width};
    line-height: ${(props) =>
      props.theme.catalogItem.initialText.tablet.lineHeight};
    font-size: ${(props) =>
      props.theme.catalogItem.initialText.tablet.fontSize};
  }
`;

StyledCatalogItemInitialText.defaultProps = { theme: Base };

const StyledCatalogItemText = styled(Text)`
  width: ${(props) => props.theme.catalogItem.text.width};
  margin-left: ${(props) => props.theme.catalogItem.text.marginLeft};
  line-height: ${(props) => props.theme.catalogItem.text.lineHeight};
  z-index: 1;
  pointer-events: none;
  color: ${(props) => props.theme.catalogItem.text.color};
  font-size: ${(props) => props.theme.catalogItem.text.fontSize};
  font-weight: ${(props) => props.theme.catalogItem.text.fontWeight};

  @media ${tablet} {
    margin-left: ${(props) => props.theme.catalogItem.text.tablet.marginLeft};
    line-height: ${(props) => props.theme.catalogItem.text.tablet.lineHeight};
    font-size: ${(props) => props.theme.catalogItem.text.tablet.fontSize};
    font-weight: ${(props) => props.theme.catalogItem.text.tablet.fontWeight};
  }
`;

StyledCatalogItemText.defaultProps = { theme: Base };

const StyledCatalogItemImg = styled.div`
  position: relative;
  z-index: 1;
  pointer-events: none;

  svg {
    width: ${(props) => props.theme.catalogItem.img.svg.width};
    height: ${(props) => props.theme.catalogItem.img.svg.height};
  }

  @media ${tablet} {
    svg {
      width: ${(props) => props.theme.catalogItem.img.svg.tablet.width};
      height: ${(props) => props.theme.catalogItem.img.svg.tablet.height};
    }
  }
`;

StyledCatalogItemImg.defaultProps = { theme: Base };

const StyledCatalogItemSibling = styled.div`
  position: absolute;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;

  &:hover {
    background-color: ${(props) =>
      props.theme.catalogItem.sibling.hover.backgroundColor};
  }
`;

StyledCatalogItemSibling.defaultProps = { theme: Base };

const StyledCatalogItemContainer = styled.div`
  background-color: ${(props) =>
    props.theme.catalogItem.container.backgroundColor};
  display: flex;
  justify-content: ${(props) => (props.showText ? "space-between" : "center")};
  align-items: center;
  width: ${(props) => props.theme.catalogItem.container.width};
  height: ${(props) => props.theme.catalogItem.container.height};
  position: relative;
  box-sizing: border-box;
  padding: ${(props) =>
    props.showText && props.theme.catalogItem.container.padding};
  margin-bottom: ${(props) =>
    props.isEndOfBlock && props.theme.catalogItem.container.marginBottom};
  cursor: pointer;

  @media ${tablet} {
    height: ${(props) => props.theme.catalogItem.container.tablet.height};
    padding: ${(props) =>
      props.showText && props.theme.catalogItem.container.tablet.padding};
    margin-bottom: ${(props) =>
      props.isEndOfBlock &&
      props.theme.catalogItem.container.tablet.marginBottom};
  }
`;
StyledCatalogItemContainer.defaultProps = { theme: Base };

export {
  StyledCatalogItemContainer,
  StyledCatalogItemImg,
  StyledCatalogItemInitialText,
  StyledCatalogItemText,
  StyledCatalogItemSibling,
  StyledCatalogItemBadgeWrapper,
};
