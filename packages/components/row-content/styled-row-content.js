import styled, { css } from "styled-components";
import Base from "../themes/base";
import { tablet, size } from "../utils/device";

const truncateCss = css`
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
`;

const commonCss = css`
  margin: ${(props) => props.theme.rowContent.margin};
  font-family: "Open Sans";
  font-size: ${(props) => props.theme.rowContent.fontSize};
  font-style: ${(props) => props.theme.rowContent.fontStyle};
  font-weight: ${(props) => props.theme.rowContent.fontWeight};
`;

const containerTabletStyle = css`
  display: block;
  height: ${(props) => props.theme.rowContent.height};
`;

const mainWrapperTabletStyle = css`
  min-width: ${(props) => props.theme.rowContent.mainWrapper.minWidth};
  margin-right: ${(props) => props.theme.rowContent.mainWrapper.marginRight};
  margin-top: ${(props) => props.theme.rowContent.mainWrapper.marginTop};
  width: ${(props) => props.theme.rowContent.mainWrapper.width};
`;

const mainContainerTabletStyle = css`
  ${truncateCss};
  max-width: ${(props) => props.theme.rowContent.maxWidth};
`;

const sideInfoTabletStyle = css`
  display: block;
  min-width: ${(props) => props.theme.rowContent.sideInfo.minWidth};
  margin: ${(props) => props.theme.rowContent.sideInfo.margin};
  ${commonCss};
  color: ${(props) => props.color && props.color};
  ${truncateCss};
`;

const StyledRowContent = styled.div`
  width: 100%;
  display: inline-flex;

  ${(props) =>
    (!props.disableSideInfo &&
      props.widthProp &&
      props.widthProp <= size.tablet) ||
    props.isMobile
      ? `${containerTabletStyle}`
      : `
    @media ${tablet} {
      ${containerTabletStyle}
    }
  `}
`;
StyledRowContent.defaultProps = { theme: Base };

const MainContainerWrapper = styled.div`
  ${commonCss};
  margin-left: 0;

  display: flex;
  align-self: center;
  margin-right: auto;

  width: ${(props) =>
    props.mainContainerWidth ? props.mainContainerWidth : "140px"};
  min-width: 140px;

  ${(props) =>
    (!props.disableSideInfo &&
      props.widthProp &&
      props.widthProp <= size.tablet) ||
    props.isMobile
      ? css`
          ${mainWrapperTabletStyle}
        `
      : `
  `}
  @media ${tablet} {
    ${mainWrapperTabletStyle}
  }
`;

MainContainerWrapper.defaultProps = { theme: Base };

const MainContainer = styled.div`
  height: 20px;
  margin-right: 8px;
  max-width: 100%;

  ${(props) =>
    (props.widthProp && props.widthProp <= size.tablet) || props.isMobile
      ? `${mainContainerTabletStyle}`
      : `
    @media ${tablet} {
      ${mainContainerTabletStyle}
    }
  `}
`;
MainContainer.defaultProps = { theme: Base };

const MainIcons = styled.div`
  height: ${(props) => props.theme.rowContent.icons.height};
  align-self: center;
  white-space: nowrap;
`;
MainIcons.defaultProps = { theme: Base };

const SideContainerWrapper = styled.div`
  ${commonCss};

  ${(props) =>
    (props.widthProp && props.widthProp <= size.tablet) || props.isMobile
      ? `${truncateCss}`
      : `
    @media ${tablet} {
      ${truncateCss}
    }
  `}

  align-self: center;
  align-items: center;

  > a {
    vertical-align: middle;
  }

  width: ${(props) => (props.containerWidth ? props.containerWidth : "40px")};
  min-width: ${(props) =>
    props.containerMinWidth ? props.containerMinWidth : "40px"};
  color: ${(props) => props.color && props.color};

  ${(props) =>
    (!props.disableSideInfo &&
      props.widthProp &&
      props.widthProp <= size.tablet) ||
    props.isMobile
      ? `display: none;`
      : `
    @media ${tablet} {
      display: none;
    }
  `}
`;
SideContainerWrapper.defaultProps = { theme: Base };

const TabletSideInfo = styled.div`
  display: none;
  ${(props) => (props.color ? `color: ${props.color};` : null)}
  ${(props) =>
    (props.widthProp && props.widthProp <= size.tablet) || props.isMobile
      ? `${sideInfoTabletStyle}`
      : `
    @media ${tablet} {
      ${sideInfoTabletStyle}
    }
  `}
`;
TabletSideInfo.defaultProps = { theme: Base };

export {
  TabletSideInfo,
  SideContainerWrapper,
  MainContainer,
  MainIcons,
  MainContainerWrapper,
  StyledRowContent,
};
